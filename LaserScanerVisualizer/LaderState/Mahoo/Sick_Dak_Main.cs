using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using LaderState;
using Microsoft.VisualBasic.FileIO;
using CIPC_CS.CLIENT;

namespace LaderState
{

    public struct foot
    {
        public System.Windows.Point foot_pos;
        public System.Windows.Point foot_vel;
        public System.Windows.Point foot_axc;
        public bool pair;
        public int id;
        public int footnum;
        public foot(System.Windows.Point pos, System.Windows.Point vel, System.Windows.Point axc, bool bool_p, int a, int num)
        {
            foot_pos = pos;
            foot_vel = vel;
            foot_axc = axc;
            pair = bool_p;
            id = a;
            footnum = num;
        }
    }
    public struct Bunsan
    {
        public System.Windows.Point Bunsan_pos;
        public int id;
        public Bunsan(System.Windows.Point pos, int a)
        {
            Bunsan_pos = pos;
            id = a;
        }
    }
    public struct Save_foot
    {
        public System.Windows.Point Save_foot_pos;
        public int id;
        public Save_foot(System.Windows.Point p, int a)
        {
            Save_foot_pos = p;
            id = a;
        }
    }
    public enum CollectiveSoundMode : int
    {
        NormalCollectiveSoundMode = 0,
        ArchievCollectiveSoundMode = 1,
        TimeDelayIndividualCollectiveSoundMode = 2,
        TimeDelaySumCollectiveSoundMode = 3,
        VirtualAvatarCollectiveSoundMode = 4,
        Individual_SoundMode = 5
    }

    public delegate void _featureData(ref System.Windows.Point[] points);
    public delegate void _humanpos(ref foot[] foots);

    /// <summary>
    /// MainSCIKの値を読む
    /// SubSickとRobotSickから情報をもらって統合
    /// 人の位置とロボットの位置、角度を算出する
    /// アーカイブ、減点、特徴点をCSVに保存
    /// </summary>
    class Sick_Dak_Main
    {
        public int datacount = 270 * 2 + 1;

        //
        #region
        List<Double> List_heikin_sokudo;
        List<Double> List_heikin_sokudo_archiev;
        public bool bool_save_robotPos = false;
        public System.Windows.Point[] Robot_Point_temp;
        public System.Windows.Point RobotPos;
        public int featurePointnum_from_rob = 0;
        public System.Windows.Point[] featurePoint_from_rob;
        public int feature_threshold = 900;
        public System.Windows.Point[] Feature_point_First_save;
        public int FeaturePoint_save_num;
        public int control_value = 0;
        public List<int>[] dist_temp_save;
        public bool[] id_for_true;
        public int datanum { get; private set; }
        public List<System.Windows.Point> List_SoundPoint;
        public System.Windows.Point Absolute_Point;
        public bool bool_Robot_Control_Start = false;
        public double firstdegree = -45;
        public System.Windows.Point bunsan;
        public System.Windows.Point[] foot_pos_child;
        public List<foot> Point_COP;
        public List<foot> Point_COP_archiev;
        public List<foot> Point_Pre_COP_archiev;
        public List<System.Windows.Point> List_Point_SoundGenerate_for_timedelay;
        public List<Double> List_velocity_soundgenerate_fot_timedelay;

        public int COP_count;
        public string str;
        public double[] distance_save;
        public List<System.Windows.Point>[,] gridmap_for_robotPos;
        private byte[] resBytes;
        private static string host = "192.168.0.1";
        private static int port = 2111;
        public System.Net.Sockets.NetworkStream ns;
        public System.Text.Encoding enc;
        private string sendMsg = "";
        public System.Net.Sockets.TcpClient tcp;
        private System.IO.MemoryStream ms;
        public System.Windows.Point[] featurePoint;

        private int resSize;
        private string[] str_dist;
        public Int32[] int_dist;
        public Int32[] int_rssi;
        private string[] str_RSSI;
        public double[] point_x;
        public double[] point_y;
        public double[] point_background_x;
        public double[] point_background_y;
        public double[] point_background_x_client1;
        public double[] point_background_y_client1;
        public double[] distance_nextpoint;
        public double[] distance_backgroundpoint;
        public double heikin_sokudo = 0;
        public double heikin_sokudo_archiev = 0;
        public int count_feature_point;
        public int count_feature_point_num;
        public bool bool_feature_point;
        public foot[] info_foot;
        public foot[] info_foot_archiev;
        public foot[] info_prefoot;
        public foot[] info_prefoot_archiev;
        public foot[] info_PreCOP;
        public foot[] info_PreCOP_archiev;
        public int count_Pre_COP;
        public int count_Pre_COP_archiev;
        private string s;
        public double[] degree_test;
        private string[] strs;
        public double heikinSokudo_output = 0;
        public string dis_str;
        private int clasta_count = 0;
        private static System.Timers.Timer aTimer;
        public System.Timers.Timer timer_save_data;
        private static System.Timers.Timer timer_background;

        public String filename = "";
        public int count_test;
        public bool bool_is_there;
        public int count_is_there;
        public bool bool_button1_clicked;
        public bool bool_button2_clicked;
        public int SoundMode = (int)CollectiveSoundMode.NormalCollectiveSoundMode;
        public int count_foot_num;
        public int count_foot_num_archiev;
        public int count_prefoot_num;
        public int count_prefoot_num_archiev;
        public List<Double> list_Archiev_foot_pos = new List<double>();
        public int ID_for_set;
        public int ID_for_set_archiev;
        public System.Windows.Threading.DispatcherTimer DT_featurePoint_pos;
        public int NowRobotPosture = 0;
        public CIPC_CS.CLIENT.CLIENT reciever_client_calib;
        public double Pos_x_second_sensor = -1123.9587;
        public double Pos_y_second_sensor = 1798.7199;
        public System.Windows.Point Point_offset;
        private bool bool_is_in = false;

        public int second_sensor_degree = 90;
        public double Range_x = 0;
        public double Range_y = 0;
        public double Position_Client_Sensor_x = 0;
        public double Position_Client_Sensor_y = 0;
        public double Rotation_Client_Sensor = 0;
        private DateTime startdt = DateTime.Now;
        private DateTime enddt = DateTime.Now;
        public TimeSpan ts;
        public List<double> List_Populality_velocity;
        public List<double> List_COP_MOVE;
        public int Archiev_start_point = 0;
        public System.Windows.Point Virtual_Avatar_Pos;
        public Double Virtual_avatar_Speed;
        public System.Windows.Point Virtual_Avatar_Target_Pos;
        //生データ
        //人の座標取得
        System.Timers.Timer timre_personPos;

        //特徴点
        //背景取得
        //保存
        String filePath;
        //Sick on Robot
        #region
        private static System.Timers.Timer timer_robot_info;
        public _featureData getFeatureData_robot;
        //ロボット制御用
        public List<System.Windows.Point> List_Purpose_Point;
        public System.Windows.Point Purpose_Point = new System.Windows.Point(0, 0);
        #endregion
        //Sick sub
        public bool Mode_Second_sensor_attach = true;
        public CIPC_CS.CLIENT.CLIENT reciever_client_attach;
        #endregion

        public Sick_Dak_Main()
        {
            RobotPos = new System.Windows.Point(0, 0);
            Robot_Point_temp = new System.Windows.Point[1000];
            Point_COP = new List<foot>();
            Point_COP_archiev = new List<foot>();
            Point_Pre_COP_archiev = new List<foot>();
            featurePoint_from_rob = new System.Windows.Point[50];
            for (int i = 0; i < 50; i++)
            {
                featurePoint_from_rob[i] = new System.Windows.Point(0, 0);
            }
            dist_temp_save = new List<int>[datacount];
            Feature_point_First_save = new System.Windows.Point[10];
            gridmap_for_robotPos = new List<System.Windows.Point>[20, 20];
            distance_save = new double[datacount];
            for (int i = 0; i < datacount; i++)
            {
                dist_temp_save[i] = new List<int>();
            }

            COP_count = 0;
            ID_for_set = 0;
            ID_for_set_archiev = 0;
            id_for_true = new bool[50];
            info_foot = new foot[50];
            info_foot_archiev = new foot[50];
            bunsan = new System.Windows.Point(0, 0);

            for (int i = 0; i < 50; i++)
            {
                info_foot[i].id = 0;
            }
            info_prefoot = new foot[50];
            info_prefoot_archiev = new foot[50];
            foot_pos_child = new System.Windows.Point[50];
            count_test = 0;
            featurePoint = new System.Windows.Point[50];
            for (int i = 0; i < 50; i++)
            {
                featurePoint[i] = new System.Windows.Point(0, 0);
            }
            int_dist = new int[datacount];
            int_rssi = new int[datacount];
            point_x = new double[datacount];
            point_y = new double[datacount];
            List_heikin_sokudo = new List<double>();
            List_heikin_sokudo_archiev = new List<double>();
            List_SoundPoint = new List<System.Windows.Point>();
            List_Point_SoundGenerate_for_timedelay = new List<System.Windows.Point>();
            List_velocity_soundgenerate_fot_timedelay = new List<double>();
            for (int i = 0; i < 10; i++)
            {
                List_Point_SoundGenerate_for_timedelay.Add(new System.Windows.Point(0, 0));
                List_velocity_soundgenerate_fot_timedelay.Add(0);
            }
            for (int i = 0; i < 60; i++)
            {
                List_heikin_sokudo.Add(0);
            }
            for (int i = 0; i < 60; i++)
            {
                List_heikin_sokudo_archiev.Add(0);
            }
            point_background_x = new double[1440];
            point_background_y = new double[1440];
            point_background_x_client1 = new double[1440];
            point_background_y_client1 = new double[1440];
            count_foot_num = 0;
            count_prefoot_num = 0;
            count_foot_num_archiev = 0;
            count_prefoot_num_archiev = 0;
            distance_nextpoint = new double[datacount];
            distance_backgroundpoint = new double[datacount];
            enc = System.Text.Encoding.ASCII;
            ms = new System.IO.MemoryStream();
            List_Purpose_Point = new List<System.Windows.Point>();
            str = "";
            resBytes = new byte[1];
            resSize = 0;
            this.dis_str = "";
            try
            {
                var append = false;

                using (var sw = new System.IO.StreamWriter("footPos.csv", append))
                {

                }
            }
            catch { };


            degree_test = new double[datacount];
            for (int i = 0; i < datacount; i++)
            {
                degree_test[i] = Math.PI * firstdegree / 180;
                firstdegree += 0.5;
                //Console.Write(degree_test[i]);
                //Console.Write(" ");
            }
            for (int i = 0; i < point_background_x.Length; i++)
            {
                point_background_x[i] = 0;
                point_background_y[i] = 0;
            }
            info_PreCOP = new foot[100];

            for (int i = 0; i < 100; i++)
            {
                info_PreCOP[i].foot_pos = new System.Windows.Point(0, 0);
                info_PreCOP[i].id = 10000;

            }
            info_PreCOP_archiev = new foot[100];
            for (int i = 0; i < 100; i++)
            {
                info_PreCOP_archiev[i].foot_pos = new System.Windows.Point(0, 0);
                info_PreCOP_archiev[i].id = 10000;

            }
            count_Pre_COP = 0;
            count_Pre_COP_archiev = 0;
            RobotPos = new System.Windows.Point(0, 0);
            bool_is_there = false;
            count_is_there = 0;
            bool_button1_clicked = false;
            bool_button2_clicked = false;
            Point_offset.X = 0;
            Point_offset.Y = 0;
            try
            {
                var append = false;

                using (var sw_Personpos = new System.IO.StreamWriter("PersonPos.csv", append))
                {

                }
            }
            catch
            {

            }

            List_COP_MOVE = new List<double>();
            List_Populality_velocity = new List<double>();
            for (int i = 0; i < 20; i++)
            {
                List_Populality_velocity.Add(1.5);
            }
            for (int i = 0; i < 80; i++)
            {
                List_COP_MOVE.Add(0);
            }
        }
        public void LMS100_init()
        {
            try
            {
                //SICKに接続
                tcp = new System.Net.Sockets.TcpClient(host, port);
                ns = tcp.GetStream();
                ns.ReadTimeout = 3000;
                ns.WriteTimeout = 3000;
                //データ受信開始
                ns = tcp.GetStream();
                sendMsg = "sEN LMDscandata 1";
                byte[] sendBytes = enc.GetBytes(((char)2).ToString() + sendMsg + ((char)3).ToString());
                this.ns.Write(sendBytes, 0, sendBytes.Length);

                //データ取得を回すタイマー
                aTimer = new System.Timers.Timer(1000);
                aTimer.Elapsed += new ElapsedEventHandler(getdata);
                aTimer.AutoReset = false;
                aTimer.Interval = 30;
                aTimer.Start();

                //特徴点だすタイマー
                this.DT_featurePoint_pos = new System.Windows.Threading.DispatcherTimer();
                this.DT_featurePoint_pos.Interval = new TimeSpan(0, 0, 0, 0, 30);
                this.DT_featurePoint_pos.Tick += get_feature_point;
                this.DT_featurePoint_pos.Start();

                //人の位置出すタイマー
                this.timre_personPos = new System.Timers.Timer();
                this.timre_personPos.Elapsed += new ElapsedEventHandler(this.background_sub);
                this.timre_personPos.Interval = 30;
                this.timre_personPos.Start();
            }
            catch { }      

        }
        public void Connect_Robot()
        {
            //ロボットからの特徴点を解析するタイマー
            timer_robot_info = new System.Timers.Timer(1000);
            timer_robot_info.Elapsed += new ElapsedEventHandler(getRobotinfo);
            timer_robot_info.AutoReset = true;
            timer_robot_info.Interval = 40;
            timer_robot_info.Enabled = true;

        }

        //データ取得と特徴点算出と人の位置算出
        void getdata(object sender, EventArgs e)
        {
            do
            {
                resSize = this.ns.Read(resBytes, 0, resBytes.Length);


                //Readが0を返した時はサーバーが切断したと判断
                if (resSize == 0)
                {
                    Console.WriteLine("サーバーが切断しました。");
                    //break;
                }
                s = this.enc.GetString(resBytes);

                //Console.Write(s);
                if (s == ((char)3).ToString())
                {
                    strs = str.Split(' ');


                    //Console.WriteLine("\n" + "point!!!!" + s);

                    //Console.WriteLine(str);
                    //strs = str.Split(' ');
                    for (int i = 0; i < strs.Length; i++)
                    {
                        if (strs[i] == "RSSI1")
                        {
                            str_RSSI = new string[545];
                            Array.Copy(strs, i + 1, str_RSSI, 0, str_RSSI.Length);
                            //Console.Write(str_RSSI.Length.ToString());

                            for (int k = 5; k < 545; k++)
                            {
                                int_rssi[k - 5] = Convert.ToInt32(str_RSSI[k], 16);
                                //    Console.Write(k);
                                //    Console.Write(":");
                            }
                        }
                        if (strs[i] == "DIST1")
                        {

                            str_dist = new string[strs.Length - i - 1];
                            Array.Copy(strs, i + 1, str_dist, 0, str_dist.Length);

                            for (int k = 5; k < datacount + 4; k++)
                            {
                                int_dist[k - 5] = Convert.ToInt32(str_dist[k], 16);

                                double x = (double)int_dist[k - 5] * Math.Cos(degree_test[k - 5]);
                                double y = (double)int_dist[k - 5] * Math.Sin(degree_test[k - 5]);
                                this.point_x[k - 5] = x * Math.Cos(-Math.PI * this.Rotation_Client_Sensor / 180) - y * Math.Sin(-Math.PI * this.Rotation_Client_Sensor / 180);
                                this.point_y[k - 5] = x * Math.Sin(-Math.PI * this.Rotation_Client_Sensor / 180) + y * Math.Cos(-Math.PI * this.Rotation_Client_Sensor / 180);
                                this.point_x[k - 5] = this.point_x[k - 5] + Position_Client_Sensor_x - Point_offset.X;
                                this.point_y[k - 5] = this.point_y[k - 5] + Position_Client_Sensor_y - Point_offset.Y;

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
                    str = "";
                }
                str += s;

                /*if (count == datacount)
                {
                    break;
                }*/


            } while (true);
            /*
            Console.WriteLine("Count:"+count);
            count = 0;
            Console.WriteLine("Finished!");
            Console.WriteLine(""+int_dist.Length);
            Console.WriteLine("Hello");
             * */
        }
        void get_feature_point(object sender, EventArgs e)
        {
            // Console.WriteLine("get feature!!");
            List<System.Windows.Point> list_for_Absolute = new List<System.Windows.Point>();
            count_feature_point = 0;
            bool_feature_point = false;
            count_feature_point_num = 0;
            for (int i = 0; i < 10; i++)
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

                        if (count_feature_point > 1)
                        {
                            featurePoint[count_feature_point_num].X = point_x[i - count_feature_point + 1];
                            featurePoint[count_feature_point_num].Y = point_y[i - count_feature_point + 1];
                            list_for_Absolute.Add(new System.Windows.Point(featurePoint[count_feature_point_num].X, featurePoint[count_feature_point_num].Y));
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

            Absolute_Point = ave_point(list_for_Absolute);

        }
        void background_sub(object sender, EventArgs e)
        {
            heikin_sokudo = 0;
            List<foot> Point_Pre_COP = new List<foot>(Point_COP);
            Point_COP.Clear();
            count_prefoot_num = count_foot_num;
            count_foot_num = 0;
            COP_count = 0;
            count_is_there = 0;
            List<System.Windows.Point> list_points_attach = new List<System.Windows.Point>();
            List_heikin_sokudo.RemoveAt(0);
            for (int i = 0; i < count_prefoot_num; i++)
            {
                info_prefoot[i].foot_pos = info_foot[i].foot_pos;
                info_prefoot[i].id = info_foot[i].id;
            }
            for (int i = 0; i < info_foot.Length; i++)//infofootの初期化
            {
                info_foot[i].id = 10000;
                info_foot[i].foot_pos = new System.Windows.Point(0, 0);
                info_foot[i].foot_vel = new System.Windows.Point(0, 0);
                info_foot[i].foot_axc = new System.Windows.Point(0, 0);
            }

            for (int i = 0; i < this.datanum; i++)
            {
                bool_is_in = false;

                for (int j = 0; j < count_foot_num; j++)
                {
                    double length = geoLength(info_foot[j].foot_pos.X, info_foot[j].foot_pos.Y, foot_pos_child[i].X - Point_offset.X, foot_pos_child[i].Y - Point_offset.Y);
                    //clientセンサーとの距離の閾値
                    if (length < 200)
                    {
                        bool_is_in = true;
                    }
                }
                if (bool_is_in == false)
                {
                    if (bool_Robot_Control_Start == false || geoLength(RobotPos.X * 1000, RobotPos.Y * 1000, foot_pos_child[i].X - Point_offset.X, foot_pos_child[i].Y - Point_offset.Y) > 400)
                    {
                        info_foot[count_foot_num].foot_pos.X = foot_pos_child[i].X - Point_offset.X;
                        info_foot[count_foot_num].foot_pos.Y = foot_pos_child[i].Y - Point_offset.Y;
                        count_foot_num++;
                    }
                }
            }



            for (int i = 0; i < count_foot_num; i++)
            {
                ID_for_set = 10000;
                if (info_foot[i].id == 10000)//ポイントにidが振られていない場合
                {
                    ////////////////////////クラスタ生成処理

                    for (int k = 0; k < count_prefoot_num; k++)
                    {
                        //前フレームとの比較処理
                        double len = geoLength(info_foot[i].foot_pos.X, info_foot[i].foot_pos.Y, info_prefoot[k].foot_pos.X, info_prefoot[k].foot_pos.Y);

                        if (len < 500 && info_prefoot[k].id != 10000)//前フレームの足との距離が0.3m以下
                        {
                            ID_for_set = info_prefoot[k].id;
                        }
                    }
                    for (int j = 0; j < count_foot_num; j++)
                    {
                        //現フレームとの比較処理
                        if (i != j)
                        {
                            double len = geoLength(info_foot[i].foot_pos.X, info_foot[i].foot_pos.Y, info_foot[j].foot_pos.X, info_foot[j].foot_pos.Y);
                            //Console.WriteLine("Len:" + len);
                            if (len < 500)//近くにいる足との距離が60cm以内
                            {
                                if (info_foot[j].id != 10000)
                                {
                                    ID_for_set = info_foot[j].id;
                                }
                            }

                            //現フレームとの比較処理終了
                        }
                    }

                    if (ID_for_set != 10000)//二処理でidが見つかった場合
                    {
                        info_foot[i].id = ID_for_set;
                    }
                    else
                    {
                        if (clasta_count < 500)//idが500まで上がったらリセット
                        {
                            clasta_count++;
                        }
                        else
                        {
                            clasta_count = 0;
                        }
                        info_foot[i].id = clasta_count;
                    }
                    ////////////////////////クラスタ生成処理終了

                }

            }

            for (int i = 0; i < 500; i++)//idナンバの数
            {
                int Count = 0;
                List<int> list_index_for_bunsan = new List<int>();
                List<System.Windows.Point> list_point_for_bunsan = new List<System.Windows.Point>();
                bunsan.X = 0;
                bunsan.Y = 0;
                for (int j = 0; j < count_foot_num; j++)
                {

                    if (info_foot[j].id == i)
                    {
                        //          list_for_save.Add(new Save_foot(info_foot[j].foot_pos, i));
                        list_index_for_bunsan.Add(j);
                        list_point_for_bunsan.Add(info_foot[j].foot_pos);
                        Count++;
                    }

                }
                if (Count >= 1)
                {
                    //分散を求める。
                    bunsan = this.var_point(list_point_for_bunsan);
                    if (bunsan.X > 200000 || bunsan.Y > 200000)//分散>閾値
                    {
                        for (int k = 0; k < list_index_for_bunsan.Count; k++)
                        {
                            info_foot[list_index_for_bunsan[k]].id = 10000;
                        }
                    }
                    else
                    {
                        foot COP = new foot();
                        if (list_point_for_bunsan.Count >= 1)
                        {
                            COP.foot_pos = ave_point(list_point_for_bunsan);
                            COP.id = i;
                            for (int k = 0; k < Point_Pre_COP.Count; k++)
                            {
                                if (COP.id == Point_Pre_COP[k].id)
                                {
                                    COP.foot_pos = new System.Windows.Point((9 * Point_Pre_COP[k].foot_pos.X + COP.foot_pos.X) / 10, (9 * Point_Pre_COP[k].foot_pos.Y + COP.foot_pos.Y) / 10);
                                    heikin_sokudo += geoLength(Point_Pre_COP[k].foot_pos.X * 0.001, Point_Pre_COP[k].foot_pos.Y * 0.001, COP.foot_pos.X * 0.001, COP.foot_pos.Y * 0.001) / (30 * 0.001);
                                }
                            }
                            Point_COP.Add(COP);
                            COP_count++;
                        }
                    }
                }
            }
            //COPはこのままだとm単位
            if (COP_count >= 1)
            {
                heikin_sokudo = heikin_sokudo / COP_count;
                List_heikin_sokudo.Add(heikin_sokudo);
                heikin_sokudo = List_heikin(List_heikin_sokudo);
            }
            else
            {
                List_heikin_sokudo.Add(0);
            }
            for (int i = 0; i < Point_COP.Count; i++)
            {
                info_PreCOP[count_Pre_COP] = Point_COP[i];
                count_Pre_COP++;
                if (count_Pre_COP >= 100)
                {
                    count_Pre_COP = 0;
                }
            }
            enddt = DateTime.Now;
            ts = enddt - startdt;

            startdt = DateTime.Now;
            for (int i = 0; i < Point_COP.Count; i++)
            {
                info_PreCOP[count_Pre_COP] = Point_COP[i];
                count_Pre_COP++;
                if (count_Pre_COP >= 100)
                {
                    count_Pre_COP = 0;
                }
            }

            //this.SendData_Sounddata(Point_Sound_generate, heikin_sokudo);
        }
        //部屋情報取得
        public void getmaxAndMin()
        {
            double max_x = 0;
            double max_y = 0;
            double min_x = 20000;
            double min_y = 20000;

            for (int i = 0; i < point_x.Length; i++)
            {
                if (max_x < point_x[i])
                {
                    max_x = point_x[i];

                }
                if (max_y < point_y[i])
                {
                    max_y = point_y[i];
                }
                if (min_x > point_x[i])
                {
                    min_x = point_x[i];
                }
                if (min_y > point_y[i])
                {
                    min_y = point_y[i];
                }
            }
            Console.WriteLine("max_x:" + max_x);
            Console.WriteLine("max_y:" + max_y);
            Console.WriteLine("min_x::" + min_x);
            Console.WriteLine("min_y:" + min_y);
        }

        //CSVデータもろもろ
        #region
        //人の位置保存（ストリームデータ）
        public void Switch_SaveHumanPos(string filePath)
        {
            //データ保存用タイマー
            if (this.timer_save_data == null)
            {
                timer_save_data = new System.Timers.Timer(1000);
                timer_save_data.Elapsed += new ElapsedEventHandler(this.SaveHumanPos);
                timer_save_data.AutoReset = true;
                timer_save_data.Interval = 30;
            }
            if (!timer_save_data.Enabled)
            {
                this.filePath = filePath;
                timer_save_data.Start();
            }
            else
            {
                timer_save_data.Stop();
            }
        }
        void SaveHumanPos(object sender, EventArgs e)
        {

            try
            {
                using (var sw = new System.IO.StreamWriter(this.filePath, true))
                {
                    if (count_foot_num >= 1)
                    {
                        for (int i = 0; i < info_foot.Length; i++)
                        {
                            if (info_foot[i].foot_pos.X != 0 && info_foot[i].foot_pos.Y != 0)
                            {
                                sw.Write("{0},{1},", info_foot[i].foot_pos.X, info_foot[i].foot_pos.Y);
                            }
                        }
                        sw.WriteLine("{0}", 0);
                    }
                }
            }
            catch { };
        }
        public void LoadData()
        {
            list_Archiev_foot_pos.Clear();
            try
            {
                // csvファイルを開く
                using (var sr = new System.IO.StreamReader(filename))
                {
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        var values = line.Split(',');
                        foreach (string value in values)
                        {
                            string st = value.ToString();
                            // System.Console.Write(Double.Parse(st));
                            double doub;
                            Double.TryParse(st, out doub);//これじゃないと成功しなかった。から文字も0になるので注意

                            list_Archiev_foot_pos.Add(doub);
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                // ファイルを開くのに失敗したとき
                System.Console.WriteLine(e.Message);
            }
        }
        //背景の保存
        //背景データキャリブ
        public void getBackgrounddata_once()
        {
            try
            {
                //背景学習タイマー
                if (timer_background == null)
                {
                    timer_background = new System.Timers.Timer(1000);
                    timer_background.Elapsed += new ElapsedEventHandler(background_study);
                    timer_background.AutoReset = true;
                    timer_background.Interval = 100;
                }

                timer_background.Enabled = true;
            }
            catch { }
            
        }
        void background_study(object sender, EventArgs e)
        {
            for (int i = 0; i < datacount; i++)
            {
                //距離データ　丸め込み　割る100
                dist_temp_save[i].Add(((int)(int_dist[i] / 100)));
            }
        }
        public void background_histgram()
        {
            try
            {
                //学習用タイマーストップ
                timer_background.Stop();
                //角度ごとに確認
                for (int i = 0; i < datacount; i++)
                {
                    //ヒストグラム生成
                    int maxcount = 0;
                    int saihinLength = 0;
                    int[] histgram = new int[200];
                    for (int a = 0; a < 200; a++)
                    {
                        histgram[a] = 0;
                    }
                    for (int j = 0; j < dist_temp_save[0].Count; j++)
                    {
                        histgram[dist_temp_save[i][j]]++;
                    }
                    for (int j = 0; j < 200; j++)
                    {
                        if (maxcount < histgram[j])
                        {
                            maxcount = histgram[j]; //
                            saihinLength = j * 100; //これが背景距離
                            distance_save[i] = (double)saihinLength;
                        }
                    }
                    for (int k = 0; k < datacount; k++)
                    {
                        double x = distance_save[k] * Math.Cos(degree_test[k]);
                        double y = distance_save[k] * Math.Sin(degree_test[k]);
                        double t_x = x * Math.Cos(-Math.PI * this.Rotation_Client_Sensor / 180) - y * Math.Sin(-Math.PI * this.Rotation_Client_Sensor / 180);
                        double t_y = x * Math.Sin(-Math.PI * this.Rotation_Client_Sensor / 180) + y * Math.Cos(-Math.PI * this.Rotation_Client_Sensor / 180);
                        t_x = t_x + Position_Client_Sensor_x - Point_offset.X;
                        t_y = t_y + Position_Client_Sensor_y - Point_offset.Y;
                        point_background_x[k] = t_x;
                        point_background_y[k] = t_y;
                    }

                }
                WriteCsv_background();
                for (int i = 0; i < datacount; i++)
                {
                    //距離データ　丸め込み　割る100
                    dist_temp_save[i].Clear();
                }
            }
            catch { }
            
        }
        public void WriteCsv_background()
        {
            try
            {
                bool append = false;

                using (var sw = new System.IO.StreamWriter("background_vision.csv", append))
                {
                    double deg = 0;
                    for (int i = 0; i < point_background_x.Length; i++)
                    {
                        sw.WriteLine("{0},{1},{2},{3}", i, deg, point_background_x[i], point_background_y[i]);
                        deg += 0.25;
                    }
                }

            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }
        public void readbackgroundfromcsv()
        {
            try
            {
                TextFieldParser parser = new TextFieldParser("background_vision.csv", System.Text.Encoding.GetEncoding("Shift_JIS"));
                parser.TextFieldType = FieldType.Delimited;

                parser.SetDelimiters(",");
                int count = 0;
                while (!parser.EndOfData)
                {
                    string[] row = parser.ReadFields();
                    point_background_x[count] = double.Parse(row[2]);
                    point_background_y[count] = double.Parse(row[3]);
                    count++;

                }
            }
            catch { }
            
        }
        //特徴点の保存
        public void FeaturePoint_First()
        {
            try
            {
                if (!DT_featurePoint_pos.IsEnabled)
                {
                    DT_featurePoint_pos.Start();
                    System.Threading.Tasks.Task.Delay(2000);
                }

                FeaturePoint_save_num = count_feature_point_num;
                for (int i = 0; i < FeaturePoint_save_num; i++)
                {
                    Feature_point_First_save[i] = featurePoint[i];
                    // Console.WriteLine(Feature_point_First_save[i]);
                }
                DT_featurePoint_pos.Stop();
                //DT_featurePoint_pos = null;
                for (int i = 0; i < 10; i++)
                {
                    featurePoint[i] = new System.Windows.Point(0, 0);
                }
                WriteFeaturePointcsv();
            } catch { }
            

        }
        public void WriteFeaturePointcsv()
        {
            try
            {
                bool append = false;

                using (var sw = new System.IO.StreamWriter("featurePoint.csv", append))
                {
                    for (int i = 0; i < FeaturePoint_save_num; i++)
                    {

                        sw.WriteLine("{0},{1}", Feature_point_First_save[i].X, Feature_point_First_save[i].Y);
                    }
                }

            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }
        public void readfeaturePointfromcsv()
        {
            try
            {
                TextFieldParser parser = new TextFieldParser("featurePoint.csv", System.Text.Encoding.GetEncoding("Shift_JIS"));
                parser.TextFieldType = FieldType.Delimited;

                parser.SetDelimiters(",");
                int count = 0;
                while (!parser.EndOfData)
                {
                    string[] row = parser.ReadFields();
                    Feature_point_First_save[count].X = double.Parse(row[0]);
                    Feature_point_First_save[count].Y = double.Parse(row[1]);
                    Console.WriteLine("fromcsv:" + Feature_point_First_save[count]);
                    count++;


                }
                FeaturePoint_save_num = count;
                DT_featurePoint_pos.Stop();
                DT_featurePoint_pos = null;
                for (int i = 0; i < 10; i++)
                {
                    featurePoint[i] = new System.Windows.Point(0, 0);
                }
            }
            catch { }
            
        }
        //原点保存
        public void offset_point()
        {
            Point_offset.X = featurePoint[0].X;
            Point_offset.Y = featurePoint[0].Y;
            Console.WriteLine("x:" + Point_offset.X);
            Console.WriteLine("y:" + Point_offset.Y);
        }
        public void WriteCsv_offset()
        {
            try
            {
                bool append = false;

                using (var sw = new System.IO.StreamWriter("offset.csv", append))
                {
                    sw.WriteLine("{0},{1}", Point_offset.X, Point_offset.Y);
                }

            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }
        public void readoffsetfromcsv()
        {
            TextFieldParser parser = new TextFieldParser("offset.csv", System.Text.Encoding.GetEncoding("Shift_JIS"));
            parser.TextFieldType = FieldType.Delimited;

            parser.SetDelimiters(",");
            while (!parser.EndOfData)
            {
                string[] row = parser.ReadFields();
                Point_offset.X = double.Parse(row[0]);
                Point_offset.Y = double.Parse(row[1]);
            }
            Console.WriteLine("x:" + Point_offset.X);
            Console.WriteLine("y:" + Point_offset.Y);
        }
        #endregion

        //SICK on Robot とのデータ授受
        #region
        //特徴点を解析(timer_robot_infoタイマーで回す）
        void getRobotinfo(object sender, EventArgs e)
        {
            if (this.getFeatureData_robot != null)
            {
                //ロボットから特徴点受け取り
                this.getFeatureData_robot(ref this.featurePoint_from_rob);

                List<System.Windows.Point> List_Temp_Robot_Pos = new List<System.Windows.Point>();//ロボット座標候補を保存するリスト、後から分散で評価する。
                List<int> List_Temp_Robot_Posture = new List<int>();
                for (int i = 0; i < featurePointnum_from_rob; i++)
                {

                    for (int j = 0; j < featurePointnum_from_rob; j++)
                    {
                        for (int k = 0; k < featurePointnum_from_rob; k++)
                        {
                            if (i != j && i != k && j != k)
                            {//同じ点同士は省略
                                double robotdist1 = geoLength(featurePoint_from_rob[j].X, featurePoint_from_rob[j].Y, featurePoint_from_rob[i].X, featurePoint_from_rob[i].Y);
                                double robotdist2 = geoLength(featurePoint_from_rob[k].X, featurePoint_from_rob[k].Y, featurePoint_from_rob[i].X, featurePoint_from_rob[i].Y);
                                for (int l = 0; l < FeaturePoint_save_num; l++)
                                {
                                    for (int m = 0; m < FeaturePoint_save_num; m++)
                                    {
                                        for (int n = 0; n < FeaturePoint_save_num; n++)
                                        {
                                            if (l != m && l != n && m != n)
                                            {

                                                double savedist1 = geoLength(Feature_point_First_save[l].X, Feature_point_First_save[l].Y, Feature_point_First_save[m].X, Feature_point_First_save[m].Y);
                                                double savedist2 = geoLength(Feature_point_First_save[l].X, Feature_point_First_save[l].Y, Feature_point_First_save[n].X, Feature_point_First_save[n].Y);
                                                if (robotdist1 != 0 && robotdist2 != 0 && savedist1 != 0 && savedist2 != 0 && Math.Abs(robotdist1 - savedist1) < 90 && Math.Abs(robotdist2 - savedist2) < 90)
                                                {
                                                    double robotdist3 = geoLength(featurePoint_from_rob[j].X, featurePoint_from_rob[j].Y, featurePoint_from_rob[k].X, featurePoint_from_rob[k].Y);
                                                    double savedist3 = geoLength(Feature_point_First_save[m].X, Feature_point_First_save[m].Y, Feature_point_First_save[n].X, Feature_point_First_save[n].Y);
                                                    if (robotdist3 != 0 && savedist3 != 0 && Math.Abs(robotdist3 - savedist3) < 90)
                                                    {
                                                        //閾値90mm,
                                                        //Feature_Point_First_save[l],[m],[n]とfeature_point_from_rob[i],[j],[k]が対応
                                                        System.Windows.Point ReferencePoint1 = new System.Windows.Point(Feature_point_First_save[l].X * 0.001, Feature_point_First_save[l].Y * 0.001);
                                                        double Referencedist1 = geoLength(0, 0, featurePoint_from_rob[i].X, featurePoint_from_rob[i].Y) * 0.001;
                                                        System.Windows.Point ReferencePoint2 = new System.Windows.Point(Feature_point_First_save[m].X * 0.001, Feature_point_First_save[m].Y * 0.001);
                                                        double Referencedist2 = geoLength(0, 0, featurePoint_from_rob[j].X, featurePoint_from_rob[j].Y) * 0.001;
                                                        System.Windows.Point ReferencePoint3 = new System.Windows.Point(Feature_point_First_save[n].X * 0.001, Feature_point_First_save[n].Y * 0.001);
                                                        double Referencedist3 = geoLength(0, 0, featurePoint_from_rob[k].X, featurePoint_from_rob[k].Y) * 0.001;
                                                        double deltaY_First = Feature_point_First_save[l].Y - Feature_point_First_save[m].Y;//
                                                        double deltaX_First = Feature_point_First_save[l].X - Feature_point_First_save[m].X;//
                                                        double First_deg = Math.Atan(deltaY_First / deltaX_First);//lとmのなす角を求める→l,mとi,jが対応しているからそのなす角の変化がロボットの姿勢

                                                        int FirstRad = (int)(First_deg * 180 / Math.PI);
                                                        if (deltaX_First >= 0)
                                                        {
                                                            if (deltaY_First >= 0)
                                                            {
                                                                FirstRad = 270 + FirstRad;
                                                            }
                                                            else if (deltaY_First < 0)
                                                            {
                                                                FirstRad = 270 + FirstRad;
                                                            }
                                                        }
                                                        else if (deltaX_First < 0)
                                                        {
                                                            if (deltaY_First >= 0)
                                                            {
                                                                FirstRad = 90 + FirstRad;
                                                            }
                                                            else if (deltaY_First < 0)
                                                            {
                                                                FirstRad = 90 + FirstRad;
                                                            }
                                                        }
                                                        double deltax = featurePoint_from_rob[i].X - featurePoint_from_rob[j].X;
                                                        double deltay = featurePoint_from_rob[i].Y - featurePoint_from_rob[j].Y;
                                                        double NowFrame_deg = Math.Atan(deltay / deltax);
                                                        int NowFrameRad = (int)(NowFrame_deg * 180 / Math.PI);

                                                        if (deltax >= 0)
                                                        {
                                                            if (deltay >= 0)
                                                            {
                                                                NowFrameRad = 270 + NowFrameRad;
                                                            }
                                                            else if (deltay < 0)
                                                            {
                                                                NowFrameRad = 270 + NowFrameRad;
                                                            }
                                                        }
                                                        else if (deltax < 0)
                                                        {
                                                            if (deltay >= 0)
                                                            {
                                                                NowFrameRad = 90 + NowFrameRad;
                                                            }
                                                            else if (deltay < 0)
                                                            {
                                                                NowFrameRad = 90 + NowFrameRad;
                                                            }
                                                        }
                                                        //     NowRobotPosture = NowFrameRad - FirstRad;
                                                        int TempRobotPosture = (NowFrameRad - FirstRad) * -1;
                                                        bool End = true;
                                                        while (End == true)
                                                        {
                                                            if (TempRobotPosture >= 360)
                                                            {
                                                                TempRobotPosture -= 360;
                                                            }
                                                            else if (TempRobotPosture < 0)
                                                            {
                                                                TempRobotPosture += 360;
                                                            }
                                                            else
                                                            {
                                                                End = false;
                                                            }
                                                        }

                                                        ////////////座標算出

                                                        double ang1 = Math.Atan((ReferencePoint2.Y - ReferencePoint1.Y) / (ReferencePoint2.X - ReferencePoint1.X));
                                                        double cos1 = Math.Cos(ang1);
                                                        double d1 = (ReferencePoint2.X - ReferencePoint1.X) / cos1;
                                                        System.Windows.Point newA1 = new System.Windows.Point(0, 0);
                                                        System.Windows.Point newB1 = new System.Windows.Point(ReferencePoint2.X - ReferencePoint1.X, ReferencePoint2.Y - ReferencePoint1.Y);

                                                        System.Windows.Point C1 = new System.Windows.Point(0, 0);
                                                        C1.X = (Math.Pow(Referencedist1, 2) - Math.Pow(Referencedist2, 2) + Math.Pow(d1, 2)) / (2 * d1);
                                                        double s1 = (Referencedist2 + Referencedist1 + d1) / 2;
                                                        C1.Y = (2 * Math.Sqrt(s1 * (s1 - Referencedist2) * (s1 - Referencedist1) * (s1 - d1))) / d1;
                                                        System.Windows.Point[] Temp_RobotPos1 = new System.Windows.Point[2];
                                                        for (int aa = 0; aa < 2; aa++)
                                                        {
                                                            Temp_RobotPos1[aa] = new System.Windows.Point(0, 0);
                                                        }
                                                        Temp_RobotPos1[0].X = C1.X * Math.Cos(ang1) + C1.Y * Math.Sin(ang1) + ReferencePoint1.X;
                                                        Temp_RobotPos1[0].Y = (C1.X * Math.Sin(ang1) - C1.Y * Math.Cos(ang1) + ReferencePoint1.Y);
                                                        Temp_RobotPos1[1].X = C1.X * Math.Cos(ang1) - C1.Y * Math.Sin(ang1) + ReferencePoint1.X;
                                                        Temp_RobotPos1[1].Y = (C1.X * Math.Sin(ang1) + C1.Y * Math.Cos(ang1) + ReferencePoint1.Y);
                                                        for (int aa = 0; aa < 2; aa++)
                                                        {
                                                            // Console.WriteLine("TempRobot①-" + aa + ":" + Temp_RobotPos1[aa]);
                                                            if (!Double.IsNaN(Temp_RobotPos1[aa].X) && !Double.IsNaN(Temp_RobotPos1[aa].Y))
                                                            {
                                                                List_Temp_Robot_Pos.Add(Temp_RobotPos1[aa]);
                                                                List_Temp_Robot_Posture.Add(TempRobotPosture);
                                                            }
                                                            if (List_Temp_Robot_Pos.Count > 150)
                                                            {
                                                                goto ExitLoop;
                                                            }


                                                        }
                                                    }
                                                }
                                            }
                                        }//n
                                    }
                                }
                            }
                        }//k

                    }
                }
            ExitLoop:
                int[] yuudo = new int[List_Temp_Robot_Pos.Count];


                int maxno = 0;
                int maxyuudo = 0;
                for (int i = 0; i < List_Temp_Robot_Pos.Count; i++)
                {
                    yuudo[i] = 0;

                    for (int j = 0; j < List_Temp_Robot_Pos.Count; j++)
                    {
                        if (i != j)
                        {
                            double Length = geoLength(List_Temp_Robot_Pos[i].X, List_Temp_Robot_Pos[i].Y, List_Temp_Robot_Pos[j].X, List_Temp_Robot_Pos[j].Y);
                            if (Length != 0 && Length < 0.25)//0.3m以下にゆうど+
                            {
                                yuudo[i] = yuudo[i] + 2;
                            }
                        }
                    }
                    if (maxyuudo < yuudo[i])
                    {
                        maxno = i;
                        maxyuudo = yuudo[i];
                    }
                }
                List<int>[] List_Robot_Posture = new List<int>[36];
                int[] Posture_max = new int[36];
                for (int i = 0; i < 36; i++)
                {
                    List_Robot_Posture[i] = new List<int>();
                    Posture_max[i] = 0;
                }
                List<System.Windows.Point> List_Rob_Pos = new List<System.Windows.Point>();
                int threshold = maxyuudo - (int)(maxyuudo * 0.08);//閾値設定尤度の最大の90%

                for (int i = 0; i < yuudo.Length; i++)
                {

                    if (yuudo[i] >= threshold)
                    {
                        int count = (int)(List_Temp_Robot_Posture[i] / 10);//10度ずつ分ける
                                                                           //  Console.WriteLine(count);
                        List_Rob_Pos.Add(List_Temp_Robot_Pos[i]);//robotの位置情報の平均化用
                        List_Robot_Posture[count].Add(List_Temp_Robot_Posture[i]);//ロボットの角度情報平均化用
                        Posture_max[count] += List_Temp_Robot_Posture[i];//角度平均化用                    
                    }
                    //次にどの角度にどれだけ入ってるかを見る。

                }
                int maxnum_forposture = 0;
                int maxcount = 0;
                for (int i = 0; i < List_Robot_Posture.Length; i++)
                {
                    if (List_Robot_Posture[i].Count > maxnum_forposture)
                    {
                        maxcount = i;
                        maxnum_forposture = List_Robot_Posture[i].Count;
                    }
                }
                if (List_Rob_Pos.Count > 0)
                {
                    RobotPos = ave_point(List_Rob_Pos);
                    NowRobotPosture = Posture_max[maxcount] / maxnum_forposture;
                    ;
                }
                else
                {
                    Console.WriteLine("Can't get Robot Pos!!");
                }
                if (bool_save_robotPos == true)
                {
                    // Console.WriteLine("RobotPos:" +RobotPos);
                    try
                    {
                        var append = true;

                        using (var sw = new System.IO.StreamWriter("RobotPos.csv", append))
                        {
                            sw.WriteLine("{0},{1}", RobotPos.X, RobotPos.Y);
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }
        //移動制御(timer_robot_controlでまわす） 
        void robot_Control(object sender, EventArgs e)
        {
            if (bool_Robot_Control_Start == true)
            {
                // Console.WriteLine("RobotPos:" + RobotPos);
                // Console.WriteLine("PPos:" + Purpose_Point);
                double len = geoLength(RobotPos.X, RobotPos.Y, Purpose_Point.X, Purpose_Point.Y);
                // Console.WriteLine("Len:" + len);
                if (len < 0.3)//30cm以内
                {
                    //SendData_Robot_to_Control(500);
                }
                else if (Purpose_Point.X != 0)
                {
                    double deltay = Purpose_Point.Y - RobotPos.Y;
                    double deltax = Purpose_Point.X - RobotPos.X;
                    double deg = Math.Atan(deltay / deltax);
                    int rad = (int)(deg * 180 / Math.PI);
                    //Console.WriteLine("deg:" + deg);
                    if (deltax >= 0)
                    {
                        if (deltay >= 0)
                        {
                            rad = 270 + rad;
                        }
                        else if (deltay < 0)
                        {
                            rad = 270 + rad;
                        }
                    }
                    else if (deltax < 0)
                    {
                        if (deltay <= 0)
                        {
                            rad = 90 + rad;
                        }
                        else if (deltay > 0)
                        {
                            rad = 90 + rad;
                        }
                    }
                    //Console.WriteLine("rad:" + rad);
                    control_value = rad - NowRobotPosture;
                    if (control_value >= 360)
                    {
                        control_value = control_value - 360;
                    }
                    else if (control_value < 0)
                    {
                        control_value = control_value + 360;
                    }
                    // SendData_Robot_to_Control(control_value);


                }
            }
        }
        #endregion

        //SICK on Sub とのデータ授受
        #region
        public void CreateCIPCClient_SubSick_Data(string ip, int remote_port, int server_port)
        {
            this.reciever_client_attach = new CIPC_CS.CLIENT.CLIENT(remote_port, ip, server_port, "Laserscaner_child", 30);
            this.reciever_client_attach.Setup(CIPC_CS.CLIENT.MODE.Receiver);
            this.reciever_client_attach.DataReceived += this.DataReceived_SubSick;
        }
        public void DataReceived_SubSick(object sender, byte[] e)
        {

            try
            {
                UDP_PACKETS_CODER.UDP_PACKETS_DECODER dec = new UDP_PACKETS_CODER.UDP_PACKETS_DECODER();
                dec.Source = e;

                this.datanum = dec.get_int();

                for (int i = 0; i < this.datanum; i++)
                {
                    this.foot_pos_child[i].X = (dec.get_double() * 1000);
                    this.foot_pos_child[i].Y = (dec.get_double() * 1000);
                }
            }
            catch { }


        }
        //キャリブデータ受け取り
        public void CreateCIPCClient_SubSick_Calib(string ip, int remote_port, int server_port)
        {
            this.reciever_client_calib = new CIPC_CS.CLIENT.CLIENT(remote_port, ip, server_port, "Laserscaner_calib", 30);
            this.reciever_client_calib.Setup(CIPC_CS.CLIENT.MODE.Receiver);
            this.reciever_client_calib.DataReceived += this.DataRecieve_Calib;
        }
        public void DataRecieve_Calib(object sender, byte[] e)
        {

            try
            {
                UDP_PACKETS_CODER.UDP_PACKETS_DECODER dec = new UDP_PACKETS_CODER.UDP_PACKETS_DECODER();
                dec.Source = e;


                for (int i = 0; i < this.datacount - 1; i++)
                {
                    this.point_background_x_client1[i] = dec.get_double();
                    this.point_background_y_client1[i] = dec.get_double();
                    //Console.WriteLine(this.point_background_x_client1[i]);

                }


            }
            catch { }


        }
        #endregion

        //他のクラスにデータを返す
        public System.Windows.Point Get_RobotPos()
        {
            return this.RobotPos;
        }
        public int Get_RobotRote()
        {
            return this.NowRobotPosture;
        }
        public List<foot> Get_ListHumanPos()
        {
            return this.Point_COP;
        }

        //終了処理
        public void Close()
        {
            if (this.reciever_client_calib != null)
            {
                this.reciever_client_calib.Close();
            }
            if(this.reciever_client_attach != null)
            {
                this.reciever_client_attach.Close();
            }
            if (aTimer != null)
            {
                aTimer.Stop();
                aTimer = null;
            }
            if (timer_background != null)
            {
                timer_background.Stop();
            }
            if (timer_save_data != null)
            {
                timer_save_data.Stop();
            }
            
            if (this.timre_personPos != null)
            {
                this.timre_personPos.Stop();
            }
            

        }
        //計算用関数
        #region
        double geoLength(double x1, double y1, double x2, double y2)
        {
            double ret = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
            return ret;
        }
        System.Windows.Point sum_point(List<System.Windows.Point> point)
        {
            System.Windows.Point p = new System.Windows.Point(0, 0);
            for (int i = 0; i < point.Count; i++)
            {
                p.X += point[i].X;
                p.Y += point[i].Y;

            }
            return p;
        }
        System.Windows.Point ave_point(List<System.Windows.Point> point)
        {
            System.Windows.Point p = sum_point(point);
            p.X = p.X / point.Count;
            p.Y = p.Y / point.Count;
            return p;
        }
        System.Windows.Point var_point(List<System.Windows.Point> point)
        {
            System.Windows.Point p = ave_point(point);
            System.Windows.Point v = new System.Windows.Point(0, 0);
            for (int i = 0; i < point.Count; i++)
            {
                v.X += (point[i].X - p.X) * (point[i].X - p.X);
                v.Y += (point[i].Y - p.Y) * (point[i].Y - p.Y);
            }
            v.X = v.X / point.Count;
            v.Y = v.Y / point.Count;
            return v;
        }
        double List_sum(List<double> list)
        {
            double sum = 0;
            for (int i = 0; i < list.Count; i++)
            {
                sum += list[i];
            }
            return sum;
        }
        double List_heikin(List<double> list)
        {
            return List_sum(list) / list.Count;
        }
        #endregion}
    }
}