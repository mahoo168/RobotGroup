using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using Microsoft.VisualBasic.FileIO;

namespace LaderState
{
    /// <summary>
    /// SICKの生データを受け取ってロボットの特徴点を算出
    /// Sick_Main.csで統合してもらう
    /// </summary>
    public class Sick_Dak_Robot
    {
        //変数宣言
        #region
        public int datacount = 270 * 3 + 1;
        public int feature_threshold = 230;
        public double firstdegree = -45;
        public string str;
        public System.Net.Sockets.NetworkStream ns;
        public System.Text.Encoding enc;
        public System.Net.Sockets.TcpClient tcp;
        private System.IO.MemoryStream ms;
        public System.Windows.Point[] featurePoint;

        private string[] str_dist;
        public Int32[] int_dist;
        public Int32[] int_rssi;
        private string[] str_RSSI;
        public double[] point_x;
        public double[] point_y;
        public double[] point_background_x;
        public double[] point_background_y;

        public double[] distance_nextpoint;
        public double[] distance_backgroundpoint;

        private string s;
        public double[] degree_test;
        private string[] strs;
        public string dis_str;
 
        private static System.Timers.Timer aTimer;

        public System.Windows.Threading.DispatcherTimer DT_featurePoint_pos;

        public double Position_Client_Sensor_x = 0;
        public double Position_Client_Sensor_y = 0;
        public double Rotation_Client_Sensor = 0;

        //通信
        UDPClient client;
        #endregion

        public Sick_Dak_Robot()
        {
            this.featurePoint = new System.Windows.Point[50];
            for (int i = 0; i < 50; i++)
            {
                featurePoint[i] = new System.Windows.Point(0, 0);
            }
            
            int_dist = new int[datacount];
            int_rssi = new int[datacount];
            point_x = new double[datacount];
            point_y = new double[datacount];

            point_background_x = new double[1440];
            point_background_y = new double[1440];

            degree_test = new double[datacount];
            for (int i = 0; i < datacount; i++)
            {
                degree_test[i] = Math.PI * firstdegree / 180;
                firstdegree += 0.33;
               
            }
            for (int i = 0; i < point_background_x.Length; i++)
            {
                point_background_x[i] = 0;
                point_background_y[i] = 0;
            }

            distance_nextpoint = new double[datacount];
            distance_backgroundpoint = new double[datacount];
            enc = System.Text.Encoding.ASCII;
            ms = new System.IO.MemoryStream();
            str = "";
            this.dis_str = "";      
            
        }

        public void ConnectUDP(string ip, int myport)
        {
            try
            {
                if(this.client == null)
                {
                    //RaspberrPiからUDPでデータを受け取る
                    this.client = new UDPClient();
                    this.client.Client_setup_reciever(ip, myport);
                    this.client.data_recieved += dataRecieved;

                    //特徴点取得を回す
                    this.DT_featurePoint_pos = new System.Windows.Threading.DispatcherTimer();
                    this.DT_featurePoint_pos.Interval = new TimeSpan(0, 0, 0, 0, 100);
                    this.DT_featurePoint_pos.Tick += get_feature_point;
                    this.DT_featurePoint_pos.Start();
                }
                
            } catch { }   

        }
        
        //データ取得
        void get_feature_point(object sender, EventArgs e)
        {
            List<System.Windows.Point> list_sendPoint_to_server = new List<System.Windows.Point>();
            List<double> list_senddist_to_server = new List<double>();
            int  count_feature_point = 0;
            bool bool_feature_point = false;
            int  count_feature_point_num = 0;
             
            for (int i = 0; i < 50; i++)
            {
                featurePoint[i] = new System.Windows.Point(1000000, 1000000);
            }

            for (int i = 0; i < datacount - 2; i++)
            {
                if (int_rssi[i] > feature_threshold)
                {
                    if (bool_feature_point)
                    {
                        count_feature_point++;
                    }
                    else
                    {
                        bool_feature_point = true;
                    }
                }

                else
                {
                    if (bool_feature_point)
                    {
                        bool_feature_point = false;

                        if (count_feature_point > 3)
                        {
                            featurePoint[count_feature_point_num].X = point_x[i - count_feature_point + 1];
                            featurePoint[count_feature_point_num].Y = point_y[i - count_feature_point + 1];
                            list_senddist_to_server.Add(int_dist[i - count_feature_point + 1]);
                            list_sendPoint_to_server.Add(featurePoint[count_feature_point_num]);
                            count_feature_point_num++;
                            count_feature_point = 0;
                        }
                        else
                        {
                            count_feature_point = 0;
                        }
                    }
                }

            }
            if(this.featurePoint.Length != 0)
            {
                Console.WriteLine(this.featurePoint[0]);

            }
        }
        void dataRecieved(object sender, byte[] data)
        {
            //ETXまで一気に届く
            str = this.enc.GetString(data);
            strs = str.Split(' ');
            for (int i = 0; i < strs.Length; i++)
            {
                if (strs[i] == "RSSI1")
                {
                    str_RSSI = new string[datacount + 5];
                    Array.Copy(strs, i + 1, str_RSSI, 0, str_RSSI.Length);
                    //Console.Write(str_RSSI.Length.ToString());

                    for (int k = 5; k < datacount + 4; k++)
                    {
                        int_rssi[k - 5] = Convert.ToInt32(str_RSSI[k], 16);

                    }
                }
                if (strs[i] == "DIST1")
                {
                    str_dist = new string[strs.Length - i - 1];
                    Array.Copy(strs, i + 1, str_dist, 0, str_dist.Length);

                    // Console.WriteLine("" + str_dist.Length);
                    for (int k = 5; k < datacount + 4; k++)
                    {
                        int_dist[k - 5] = Convert.ToInt32(str_dist[k], 16);
                        double x = (double)int_dist[k - 5] * Math.Cos(degree_test[k - 5]);
                        double y = (double)int_dist[k - 5] * Math.Sin(degree_test[k - 5]);
                        this.point_x[k - 5] = x * Math.Cos(-Math.PI * this.Rotation_Client_Sensor / 180) - y * Math.Sin(-Math.PI * this.Rotation_Client_Sensor / 180);
                        this.point_y[k - 5] = x * Math.Sin(-Math.PI * this.Rotation_Client_Sensor / 180) + y * Math.Cos(-Math.PI * this.Rotation_Client_Sensor / 180);
                        this.point_x[k - 5] = this.point_x[k - 5] + Position_Client_Sensor_x;
                        this.point_y[k - 5] = this.point_y[k - 5] + Position_Client_Sensor_y;
                    }
                    for (int l = 0; l < datacount - 2; l++)
                    {
                        distance_nextpoint[l] = this.geoLength(point_x[l], point_y[l], point_x[l + 1], point_y[l + 1]);
                    }
                    distance_nextpoint[datacount - 1] = distance_nextpoint[datacount - 2];

                    for (int m = 0; m < datacount - 1; m++)
                    {
                        distance_backgroundpoint[m] = this.geoLength(point_x[m], point_y[m], point_background_x[m], point_background_y[m]);
                    }

                }
            }
            this.dis_str = str;
            
        }
    
        //点同士のキョリを求めるメソッド
        public double geoLength(double x1, double y1, double x2, double y2)
        {
            double ret = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
            return ret;
        }
 
        //他クラスへのデータ受け渡し
        public void Get_FeaturePoint(ref System.Windows.Point[] points)
        {
            points = this.featurePoint;
        }

        //終了処理
        public void Close()
        {
            //通信クライアント
            if(this.client != null)
            {
                this.client.Close();
            }

            //タイマー
            if (aTimer != null)
            {
                aTimer.Stop();
                aTimer = null;
            }
            if(this.DT_featurePoint_pos != null)
            {
                this.DT_featurePoint_pos.Stop();
            }
        }

    }
}