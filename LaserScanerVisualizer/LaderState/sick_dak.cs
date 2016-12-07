//
//
//CopyRight @ Miwalab
//2015.7.7 Created by Miwalab 
//
//

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

namespace _2015_6_29_TCPTest
{
    public class sick_dak
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
            public Save_foot(System.Windows.Point p,int a)
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
        public int datacount = 270 * 2 + 1;

        /// <会計取得用変数>
        //試しに50cm刻みで20ｍ分だから、datacount個で40次のリストをつくってみる。
        //public List<double>[][] list_Learning_background = new List<double>][40];
        /// </summary>
        /// 

        #region
        List<Double> List_heikin_sokudo;
        List<Double> List_heikin_sokudo_archiev;
        public bool RobotInfoStart = false;
        public System.Windows.Point Point_Sound_generate;
        public bool bool_save_robotPos = false;
        public System.Windows.Point[] Robot_Point_temp;
        public int Robot_Pos_Count = 0;
        public System.Windows.Point RobotPos;
        public int featurePointnum_from_rob = 0;
        public System.Windows.Point[] featurePoint_from_rob;
        public int feature_threshold = 900;
        public System.Windows.Point[] Feature_point_First_save;
        public int FeaturePoint_save_num;
        public int control_value = 0;
        public List<int>[] dist_temp_save;
        public bool[] id_for_true;
        public MainWindow mainwindow { set; get; }
        public int datanum { get; private set; }
        public int COP_count_archiev;
        public List<System.Windows.Point> List_SoundPoint;
        public System.Windows.Point Absolute_Point;
        public bool bool_Robot_Control_Start = false;
        public double firstdegree = -45;
        public int id_increment = 1;
        public System.Windows.Point bunsan;
        public System.Windows.Point bunsan_archiev;
        public System.Windows.Point[] foot_pos_child;
        public List<foot> Point_COP;
        public List<foot> Point_COP_archiev;
        public List<foot> Point_Pre_COP_archiev;
        public List<System.Windows.Point> List_Point_SoundGenerate_for_timedelay;
        public List<Double> List_velocity_soundgenerate_fot_timedelay;
        public List<System.Windows.Point> List_Purpose_Point;
        public int COP_count;
        public string str; 
        public double[] distance_save;
        public List<System.Windows.Point>[,] gridmap_for_robotPos;
        List<System.Windows.Point> List_Sound_Point;
        private byte[] resBytes;
        private static string host = "192.168.0.1";
        private static int port = 2111;
        public System.Net.Sockets.NetworkStream ns;
        public System.Text.Encoding enc;
        private string sendMsg = "";
        public System.Net.Sockets.TcpClient tcp;
        private System.IO.MemoryStream ms;
        public System.Windows.Point[] featurePoint;
        public System.Windows.Point Purpose_Point = new System.Windows.Point(0,0);
        public double min_x;
        public double max_x;
        public double min_y;
        public double max_y;
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
        private int clasta_count_archiev = 0;
        public int prefootnum;
        private static System.Timers.Timer aTimer;
        private static System.Timers.Timer timer_robot_info;
        public  System.Timers.Timer timer_save_data;
        public System.Timers.Timer timer_start_archiev;
        public System.Timers.Timer timer_get_SoundData;
        private static System.Timers.Timer timer_background;
        public  System.Timers.Timer timer_robot_control;
        public DateTime time;
        public String filename = "";
        public int count_test;
        public bool bool_is_there;
        public int count_is_there;
        public bool bool_button1_clicked;
        public bool bool_button2_clicked;
        public int SoundMode = (int)CollectiveSoundMode.NormalCollectiveSoundMode;
        public int PreSoundMode;
        public int count_foot_num;
        public int count_foot_num_archiev;
        public int count_prefoot_num;
        public int count_prefoot_num_archiev;
        public List<Double> list_Archiev_foot_pos; 
        public int ID_for_set;
        public int ID_for_set_archiev;
        public bool bool_button_Write_pos;
        public System.Windows.Threading.DispatcherTimer DT_Person_pos;
        public System.Windows.Threading.DispatcherTimer DT_featurePoint_pos;
        public int NowRobotPosture = 0;
        public CIPC_CS.CLIENT.CLIENT sender_client_media;
        public CIPC_CS.CLIENT.CLIENT sender_client_save;
        public CIPC_CS.CLIENT.CLIENT sender_client_attach;
        public CIPC_CS.CLIENT.CLIENT sender_client_calib;
        public CIPC_CS.CLIENT.CLIENT reciever_client_calib;
        public CIPC_CS.CLIENT.CLIENT receive_client_sc;
        public CIPC_CS.CLIENT.CLIENT sender_client_robot;
        public CIPC_CS.CLIENT.CLIENT sender_client_sounddata_robot;
        public double Pos_x_second_sensor = -1123.9587;
        public double Pos_y_second_sensor = 1798.7199;
        public System.Windows.Point Point_offset;
        private bool bool_is_in = false;
        public bool Mode_Second_sensor_attach = true;
        public int second_sensor_degree = 90;
        public double Range_x = 0;
        public double Range_y = 0;
        public double Position_Client_Sensor_x = 0;
        public double Position_Client_Sensor_y = 0;
        public double Rotation_Client_Sensor = 0;
        private CIPC_CS.CLIENT.CLIENT receive_feature_Pos;
        private DateTime startdt = DateTime.Now;
        private DateTime enddt = DateTime.Now;
        public TimeSpan ts;
        public List<double> List_Populality_velocity;
        public List<double> List_COP_MOVE;
        public double Populality_Velocity;
        public int Archiev_start_point = 0;
        public string filepath = "";
        public System.Windows.Point Virtual_Avatar_Pos;
        public Double Virtual_avatar_Speed;
        public System.Windows.Point Virtual_Avatar_Target_Pos;
        #endregion

        public sick_dak()
        {         
            timer_start_archiev = new System.Timers.Timer(1000);
            timer_start_archiev.Elapsed += new ElapsedEventHandler(start_archiev);
            timer_start_archiev.AutoReset = true;
            timer_start_archiev.Interval = 30;

            timer_get_SoundData = new System.Timers.Timer(1000);
            timer_get_SoundData.Elapsed += new ElapsedEventHandler(start_send_SoundData);
            timer_get_SoundData.AutoReset = true;
            timer_get_SoundData.Interval = 40;
            timer_get_SoundData.Enabled = true;

            list_Archiev_foot_pos = new List<double>();
            List_Sound_Point = new List<System.Windows.Point>();
            Point_Sound_generate = new System.Windows.Point(0,0);
            double test = map(50,100,0);
            Console.WriteLine(test);
            RobotPos = new System.Windows.Point(0,0);
            Robot_Point_temp = new System.Windows.Point[1000];
            Point_COP = new List<foot>();
            Point_COP_archiev = new List<foot>();
            Point_Pre_COP_archiev = new List<foot>();
            featurePoint_from_rob = new System.Windows.Point[50];
            for (int i = 0;i < 50;i++)
            {
                featurePoint_from_rob[i] = new System.Windows.Point(0,0);
            }
            dist_temp_save = new List<int>[datacount];
            Feature_point_First_save = new System.Windows.Point[10];
            gridmap_for_robotPos = new List<System.Windows.Point>[20, 20];
            distance_save = new double[datacount];
            for (int i = 0;i < datacount; i++)
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
            for (int i = 0;i < 50;i ++)
            {
                featurePoint[i] = new System.Windows.Point(0,0);
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
            for (int i = 0;i < 10;i++)
            {
                List_Point_SoundGenerate_for_timedelay.Add(new System.Windows.Point(0,0));
                List_velocity_soundgenerate_fot_timedelay.Add(0);
            }
            for (int i = 0;i < 60;i++)
            {
                List_heikin_sokudo.Add(0);
            }
            for (int i = 0;i < 60;i++)
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

/*            for (int i = 0;i < list_Archiev_foot_pos.Count;i++)
            {
                Console.WriteLine(list_Archiev_foot_pos[i]);
            }
            */
            try
            {
                this.LMS100_init();
            }
            catch
            {

            }
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

            for (int i = 0;i < 100;i++)
            {
                    info_PreCOP[i].foot_pos = new System.Windows.Point(0,0);
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
            RobotPos = new System.Windows.Point(0,0);
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
            for (int i = 0;i < 20;i++)
            {
                List_Populality_velocity.Add(1.5);
            }
            for (int i = 0; i < 80; i++)
            {
                List_COP_MOVE.Add(0);
            }
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
                            Double.TryParse(st,out doub);//これじゃないと成功しなかった。から文字も0になるので注意

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
        public void LMS100_init()
        {          
            this.connect();
            this.sendCommandtoLRF(0);
            
            aTimer = new System.Timers.Timer(1000);
            aTimer.Elapsed += new ElapsedEventHandler(getdata);
            aTimer.AutoReset = false;
            aTimer.Interval = 30;
            aTimer.Enabled = true;

            timer_robot_info = new System.Timers.Timer(1000);
            timer_robot_info.Elapsed += new ElapsedEventHandler(getRobotinfo);
            timer_robot_info.AutoReset = true;
            timer_robot_info.Interval = 40;
            timer_robot_info.Enabled = true;

            timer_background = new System.Timers.Timer(1000);
            timer_background.Elapsed += new ElapsedEventHandler(background_study);
            timer_background.AutoReset = true;
            timer_background.Interval = 100;

            timer_save_data = new System.Timers.Timer(1000);
            timer_save_data.Elapsed += new ElapsedEventHandler(savedata);
            timer_save_data.AutoReset = true;
            timer_save_data.Interval = 30;
            timer_save_data.Stop();

            timer_robot_control = new System.Timers.Timer(1000);
            timer_robot_control.Elapsed += new ElapsedEventHandler(robot_Control);
            timer_robot_control.AutoReset = true;
            timer_robot_control.Interval = 100;
/*
            this.DT_Person_pos = new System.Windows.Threading.DispatcherTimer();
            this.DT_Person_pos.Interval = new TimeSpan(0, 0, 0, 0, 60);
            this.DT_Person_pos.Tick += background_substraction;
            
  */          this.DT_featurePoint_pos = new System.Windows.Threading.DispatcherTimer();
            this.DT_featurePoint_pos.Interval = new TimeSpan(0, 0, 0, 0, 30);
            this.DT_featurePoint_pos.Tick += get_feature_point;
            this.DT_featurePoint_pos.Start();
        }

        public void getmaxAndMin()
        {
            double max_x = 0;
            double max_y = 0;
            double min_x = 20000;
            double min_y = 20000;
             
            for (int i = 0;i < point_x.Length;i++)
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
                if(min_y > point_y[i])
                {
                    min_y = point_y[i];
                }
            }
            Console.WriteLine("max_x:" + max_x);
            Console.WriteLine("max_y:" + max_y);
            Console.WriteLine("min_x::"+ min_x);
            Console.WriteLine("min_y:" + min_y);
        }
        public double map(double v, double beforemax, double beforemin)
        {
            return (v- beforemin) * (127 - 0) / (beforemax - beforemin) + 0;
        }
        public void sick_stop()
        {
            //DT_Person_pos.Stop();
            aTimer.Stop();
            // timer_person_pos_save.Stop();
            aTimer = null;
            //timer_person_pos_save = null;
            timer_background.Stop();
            timer_background = null;
            timer_save_data.Stop();
            timer_save_data = null;
            timer_robot_control.Stop();
            timer_robot_control = null;

        //    DT_Person_pos.Stop();
        //    DT_Person_pos = null;
            DT_featurePoint_pos.Stop();
            DT_featurePoint_pos = null;
        }
        public double List_sum(List<double> list)
        {
            double sum = 0;
            for (int i = 0;i < list.Count;i++)
            {
                sum += list[i];
            }
            return sum;
        }
        public double List_heikin(List<double> list)
        {
            return List_sum(list) / list.Count;
        }

        void start_archiev(object sender,EventArgs e)
        {
            heikin_sokudo_archiev = 0;
            Point_Pre_COP_archiev = new List<foot>(Point_COP_archiev);
            Point_COP_archiev.Clear();
            count_prefoot_num_archiev = count_foot_num_archiev;
            count_foot_num_archiev = 0;
            COP_count_archiev = 0;
            List_heikin_sokudo_archiev.RemoveAt(0);
            for (int i = 0; i < count_prefoot_num_archiev; i++)
            {
                info_prefoot_archiev[i].foot_pos = info_foot_archiev[i].foot_pos;
                info_prefoot_archiev[i].id = info_foot_archiev[i].id;
            }
            for (int i = 0; i < info_foot_archiev.Length; i++)//infofootの初期化
            {
                info_foot_archiev[i].id = 10000;
                info_foot_archiev[i].foot_pos = new System.Windows.Point(0, 0);
                info_foot_archiev[i].foot_vel = new System.Windows.Point(0, 0);
                info_foot_archiev[i].foot_axc = new System.Windows.Point(0, 0);
            }
            while (true)
            {
                //Console.WriteLine("Archiev_start_point:" + Archiev_start_point);
                if (list_Archiev_foot_pos[Archiev_start_point]!= 0)
                {
                    info_foot_archiev[count_foot_num_archiev].foot_pos = new System.Windows.Point(list_Archiev_foot_pos[Archiev_start_point], list_Archiev_foot_pos[Archiev_start_point + 1]);
                    count_foot_num_archiev++;
                    Archiev_start_point = Archiev_start_point + 2;
                }
                else
                {
                    Archiev_start_point++;
                    if (Archiev_start_point >= list_Archiev_foot_pos.Count)
                    {
                        timer_start_archiev.Stop();
                        Archiev_start_point = 0;
                    }
                    goto ExitLoop;
                }
            }
            ExitLoop:;
            //ここでいったん終わり
            for (int i = 0; i < count_foot_num_archiev; i++)
            {
                ID_for_set_archiev = 10000;
                if (info_foot_archiev[i].id == 10000)//ポイントにidが振られていない場合
                {
                    ////////////////////////クラスタ生成処理

                    for (int k = 0; k < count_prefoot_num_archiev; k++)
                    {
                        //前フレームとの比較処理
                        double len = geoLength(info_foot_archiev[i].foot_pos.X, info_foot_archiev[i].foot_pos.Y, info_prefoot_archiev[k].foot_pos.X, info_prefoot_archiev[k].foot_pos.Y);

                        if (len < 500 && info_prefoot_archiev[k].id != 10000)//前フレームの足との距離が0.3m以下
                        {
                            ID_for_set_archiev = info_prefoot_archiev[k].id;
                        }
                    }
                    for (int j = 0; j < count_foot_num_archiev; j++)
                    {
                        //現フレームとの比較処理
                        if (i != j)
                        {
                            double len = geoLength(info_foot_archiev[i].foot_pos.X, info_foot_archiev[i].foot_pos.Y, info_foot_archiev[j].foot_pos.X, info_foot_archiev[j].foot_pos.Y);
                            if (len < 500)//近くにいる足との距離が50cm以内
                            {
                                if (info_foot_archiev[j].id != 10000)
                                {
                                    ID_for_set_archiev = info_foot_archiev[j].id;
                                }
                            }

                            //現フレームとの比較処理終了
                        }
                    }
                    if (ID_for_set_archiev != 10000)//二処理でidが見つかった場合
                    {
                        info_foot_archiev[i].id = ID_for_set_archiev;
                    }
                    else
                    {
                        if (clasta_count_archiev < 500)//idが500まで上がったらリセット
                        {
                            clasta_count_archiev++;
                        }
                        else
                        {
                            clasta_count_archiev = 0;
                        }
                        info_foot_archiev[i].id = clasta_count_archiev;
                    }
                    ////////////////////////クラスタ生成処理終了
                }
            }
            for (int i = 0; i < 500; i++)//idナンバの数
            {
                int Count = 0;
                List<int> list_index_for_bunsan = new List<int>();
                List<System.Windows.Point> list_point_for_bunsan = new List<System.Windows.Point>();
                bunsan_archiev.X = 0;
                bunsan_archiev.Y = 0;
                for (int j = 0; j < count_foot_num_archiev; j++)
                {
                    if (info_foot_archiev[j].id == i)
                    {
                        //          list_for_save.Add(new Save_foot(info_foot[j].foot_pos, i));
                        list_index_for_bunsan.Add(j);
                        list_point_for_bunsan.Add(info_foot_archiev[j].foot_pos);
                        Count++;
                    }
                }
                if (Count >= 1)
                {
                    //分散を求める。
                    bunsan_archiev = this.var_point(list_point_for_bunsan);
                    if (bunsan_archiev.X > 200000 || bunsan_archiev.Y > 200000)//分散>閾値
                    {
                        for (int k = 0; k < list_index_for_bunsan.Count; k++)
                        {
                            info_foot_archiev[list_index_for_bunsan[k]].id = 10000;
                        }
                    }
                    else
                    {
                        foot COP = new foot();
                        if (list_point_for_bunsan.Count >= 1)
                        {
                            COP.foot_pos = ave_point(list_point_for_bunsan);
                            COP.id = i;
                            for (int k = 0; k < Point_Pre_COP_archiev.Count;k++)
                            {
                                if (COP.id == Point_Pre_COP_archiev[k].id)
                                {
                                    COP.foot_pos = new System.Windows.Point((9 * Point_Pre_COP_archiev[k].foot_pos.X + COP.foot_pos.X) / 10, (9 * Point_Pre_COP_archiev[k].foot_pos.Y + COP.foot_pos.Y) / 10);
                                    heikin_sokudo_archiev += geoLength(Point_Pre_COP_archiev[k].foot_pos.X * 0.001, Point_Pre_COP_archiev[k].foot_pos.Y * 0.001,COP.foot_pos.X * 0.001,COP.foot_pos.Y * 0.001) / (30 * 0.001);
                                }
                            }
                            Point_COP_archiev.Add(COP);
                            COP_count_archiev++;
                        }
                    }
                }
            }
            heikin_sokudo_archiev = heikin_sokudo_archiev / COP_count_archiev;
            List_heikin_sokudo_archiev.Add(heikin_sokudo_archiev);//60フレームの平均を取得するため
            heikin_sokudo_archiev = List_heikin(List_heikin_sokudo_archiev);
           //Console.WriteLine("heikinsokudo_archiev:" + heikin_sokudo_archiev);
            for (int i = 0; i < Point_COP_archiev.Count; i++)
            {
                info_PreCOP_archiev[count_Pre_COP_archiev] = Point_COP_archiev[i];
              //  Console.WriteLine("infoPre_COP_ID:" + info_PreCOP_archiev[count_Pre_COP_archiev].id);
                count_Pre_COP_archiev++;
                if (count_Pre_COP_archiev >= 100)
                {
                    count_Pre_COP_archiev = 0;
                }
            }
            startdt = DateTime.Now;


            if (Double.IsNaN(Purpose_Point.X))
            {
                Purpose_Point = new System.Windows.Point(0, 0);
            }

        }
        void savedata(object sender, EventArgs e)
        {
      /*      try
            {
                var append = true;

                using (var sw = new System.IO.StreamWriter("PersonPos.csv", append))
                {
                    if (COP_count >= 1)
                    {
                        for (int i = 0; i < Point_COP.Count; i++)
                        {
                            sw.Write("{0},{1},{2},", Point_COP[i].foot_pos.X, Point_COP[i].foot_pos.Y, Point_COP[i].id);
                        }
                        sw.WriteLine();
                    }
                }
            }
            catch { };*/
            try
            {
                var append = true;

                using (var sw = new System.IO.StreamWriter("footPos.csv", append))
                {
                    if (count_foot_num >= 1)
                    {
                        for (int i = 0; i < info_foot.Length;i++)
                        {
                            if (info_foot[i].foot_pos.X != 0 && info_foot[i].foot_pos.Y != 0) {
                                sw.Write("{0},{1},", info_foot[i].foot_pos.X,info_foot[i].foot_pos.Y);
                            }
                        }
                        sw.WriteLine("{0}",0);
                    }
                }
            }
            catch { };
        }
        void Sound_Mode_init()
        {
            Console.WriteLine("init");
            Virtual_Avatar_Pos = new System.Windows.Point(0,0);
            Virtual_avatar_Speed = 0;
            Virtual_Avatar_Target_Pos = new System.Windows.Point(0,0);
            timer_start_archiev.Stop();
            for (int i = 0;i < 50;i++)
            {
                info_foot_archiev[i].foot_pos = new System.Windows.Point(0,0);
                info_prefoot_archiev[i].foot_pos = new System.Windows.Point(0,0);

            }
            for (int i = 0; i < 100; i++)
            {
                info_PreCOP_archiev[i].foot_pos = new System.Windows.Point(0, 0);
                info_PreCOP_archiev[i].id = 10000;

            }
            timer_start_archiev.Stop();
            for (int i = 0; i < 10; i++)
            {
                List_Point_SoundGenerate_for_timedelay.Add(new System.Windows.Point(0, 0));
                List_velocity_soundgenerate_fot_timedelay.Add(0);
            }

        }
        void start_send_SoundData(object sender, EventArgs e)
        {
            heikinSokudo_output = 0;
            List_Sound_Point.Clear();
            List_Purpose_Point.Clear();
            if (SoundMode != PreSoundMode)
            {
                Sound_Mode_init();
            }
            switch (SoundMode)
            {
                case (int)CollectiveSoundMode.NormalCollectiveSoundMode:
                    //基本音生成モード
                    
                    background_sub();
                    for (int i = 0; i < count_foot_num; i++)
                    {
                        List_Sound_Point.Add(info_foot[i].foot_pos);
                        List_Purpose_Point.Add(info_foot[i].foot_pos);
                    }
                    Point_Sound_generate = this.ave_point(List_Sound_Point);//重心の位置

                    if (List_Purpose_Point.Count > 0)
                    {
                        Purpose_Point = this.ave_point(List_Purpose_Point);
                        Purpose_Point.X *= -0.001;
                        Purpose_Point.Y *= -0.001;
                    }
                    if (Double.IsNaN(Purpose_Point.X)　|| Double.IsNaN(Purpose_Point.Y))
                    {
                        Purpose_Point = new System.Windows.Point(0, 0);
                    }
                    heikinSokudo_output = heikin_sokudo;
                    this.SendData_Sounddata(Point_Sound_generate, heikinSokudo_output);
                    break;
                case (int)CollectiveSoundMode.ArchievCollectiveSoundMode:
                    //アーカイブ音生成モード
                    background_sub();
                    for (int i = 0; i < count_foot_num; i++)
                    {
                        List_Sound_Point.Add(info_foot[i].foot_pos);
                        List_Purpose_Point.Add(info_foot[i].foot_pos);

                    }
                    for (int i = 0; i < count_foot_num_archiev; i++)
                    {
                        List_Sound_Point.Add(info_foot_archiev[i].foot_pos);
                        List_Purpose_Point.Add(info_foot_archiev[i].foot_pos);
                    }
                    Point_Sound_generate = this.ave_point(List_Sound_Point);//重心の位置
                                                                           
                    //////////////////////////目標点設定

                    if (List_Purpose_Point.Count > 0)
                    {
                        Purpose_Point = this.ave_point(List_Purpose_Point);
                        Point_Sound_generate = new System.Windows.Point(Purpose_Point.X, Purpose_Point.Y);//重心の位置
                        Purpose_Point.X *= -0.001;
                        Purpose_Point.Y *= -0.001;
                    }
                    if (Double.IsNaN(Purpose_Point.X))
                    {
                        Purpose_Point = new System.Windows.Point(0, 0);
                    }
                    heikinSokudo_output = (heikin_sokudo + heikin_sokudo_archiev) / 2;
                    this.SendData_Sounddata(Point_Sound_generate, heikinSokudo_output);
                    break;

                case (int)CollectiveSoundMode.TimeDelayIndividualCollectiveSoundMode:
                    //時間遅れ個人
                    List_Point_SoundGenerate_for_timedelay.RemoveAt(0);
                    List_velocity_soundgenerate_fot_timedelay.RemoveAt(0);
                    background_sub();
                    for (int i = 0; i < count_foot_num; i++)
                    {
                        List_Sound_Point.Add(info_foot[i].foot_pos);
                        List_Purpose_Point.Add(info_foot[i].foot_pos);
                    }
                    Point_Sound_generate = this.ave_point(List_Sound_Point);//重心の位置

                    if (List_Purpose_Point.Count > 0)
                    {
                        Purpose_Point = this.ave_point(List_Purpose_Point);
                        Purpose_Point.X *= -0.001;
                        Purpose_Point.Y *= -0.001;
                    }
                    if (Double.IsNaN(Purpose_Point.X) || Double.IsNaN(Purpose_Point.Y))
                    {
                        Purpose_Point = new System.Windows.Point(0, 0);
                    }
                    heikinSokudo_output = heikin_sokudo;
                    List_Point_SoundGenerate_for_timedelay.Add(Point_Sound_generate);
                    List_velocity_soundgenerate_fot_timedelay.Add(heikinSokudo_output);
                    this.SendData_Sounddata(List_Point_SoundGenerate_for_timedelay[0], List_velocity_soundgenerate_fot_timedelay[0]);
                    break;
                case (int)CollectiveSoundMode.TimeDelaySumCollectiveSoundMode:
                    //時間遅れ和
                    List_Point_SoundGenerate_for_timedelay.RemoveAt(0);
                    List_velocity_soundgenerate_fot_timedelay.RemoveAt(0);
                    background_sub();
                    for (int i = 0; i < count_foot_num; i++)
                    {
                        List_Sound_Point.Add(info_foot[i].foot_pos);
                        List_Purpose_Point.Add(info_foot[i].foot_pos);
                    }
                    Point_Sound_generate = this.ave_point(List_Sound_Point);//重心の位置

                    if (List_Purpose_Point.Count > 0)
                    {
                        Purpose_Point = this.ave_point(List_Purpose_Point);
                        Purpose_Point.X *= -0.001;
                        Purpose_Point.Y *= -0.001;
                    }

                    if (Double.IsNaN(Purpose_Point.X) || Double.IsNaN(Purpose_Point.Y))
                    {
                        Purpose_Point = new System.Windows.Point(0, 0);
                    }
                    heikinSokudo_output = heikin_sokudo;
                    List_Point_SoundGenerate_for_timedelay.Add(Point_Sound_generate);
                    List_velocity_soundgenerate_fot_timedelay.Add(heikinSokudo_output);

                    System.Windows.Point Ave_Point_output = new System.Windows.Point((List_Point_SoundGenerate_for_timedelay[0].X + Point_Sound_generate.X) / 2,(List_Point_SoundGenerate_for_timedelay[0].Y + Point_Sound_generate.Y)/2);
                    Double Ave_vel_output = (List_velocity_soundgenerate_fot_timedelay[0] + heikinSokudo_output) / 2;
                    this.SendData_Sounddata(Ave_Point_output, Ave_vel_output);
                    break;
                case (int)CollectiveSoundMode.VirtualAvatarCollectiveSoundMode:
                    //仮想アバター音和生成モード
                    background_sub();

                    double len = geoLength(Virtual_Avatar_Pos.X,Virtual_Avatar_Pos.Y,Virtual_Avatar_Target_Pos.X,Virtual_Avatar_Target_Pos.Y);
                    int max_count = 1000;
                    Double max_length = 0;
                    if (len < 0.5)
                    {
                        //目標点更新
                        for (int i = 0;i < count_foot_num;i++)
                        {
                            double l = geoLength(Point_COP[i].foot_pos.X, Point_COP[i].foot_pos.Y,Virtual_Avatar_Pos.X,Virtual_Avatar_Pos.Y);
                            if (l > max_length)
                            {
                                max_count = i;
                                max_length = l;
                            }
                        }
                        if (max_count != 1000) {
                            Virtual_Avatar_Target_Pos = Point_COP[max_count].foot_pos;
                        }
                    }
                    else
                    {//仮想アバター制御

                    }
                    break;
                case (int)CollectiveSoundMode.Individual_SoundMode:
                    //個人音生成モード
                    break;
            }
            PreSoundMode = SoundMode;
        }
        void background_sub()
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
            //保存用リスト
            //    List<System.Windows.Point> list_points = new List<System.Windows.Point>();
            if (bool_button1_clicked == true)
            {
                for (int i = 0; i < datacount - 2; i++)
                {
                    if (distance_backgroundpoint[i] > 250 && distance_nextpoint[i] < 200.0)
                    {
                        if (bool_is_there)
                        {
                            count_is_there++;
                        }
                        else
                        {
                            bool_is_there = true;
                        }
                    }
                    else
                    {
                        if (bool_is_there)
                        {
                            bool_is_there = false;

                            if (count_is_there > 1)
                            {
                                //if (Math.Abs(point_x[i - (int)(count_is_there / 2)] - Point_offset.X) < this.Range_x && Math.Abs(point_y[i - (int)(count_is_there / 2)] - Point_offset.Y) < this.Range_y)
                                //  {
                                double l = geoLength(RobotPos.X * 1000, RobotPos.Y * 1000, point_x[i - (int)(count_is_there / 2)], point_y[i - (int)(count_is_there / 2)]);
                                Console.WriteLine("l:" + l);
                                if (bool_Robot_Control_Start == false || geoLength(RobotPos.X * 1000, RobotPos.Y * 1000, point_x[i - (int)(count_is_there / 2)], point_y[i - (int)(count_is_there / 2)]) > 500)
                                {
                                    //Console.WriteLine("Len:" + geoLength(RobotPos.X * 1000, RobotPos.Y * 1000, point_x[i - (int)(count_is_there / 2)], point_y[i - (int)(count_is_there / 2)]));
                                    //ロボット座標を人と認識しないようにする
                                    info_foot[count_foot_num].foot_pos.X = point_x[i - (int)(count_is_there / 2)];
                                    info_foot[count_foot_num].foot_pos.Y = point_y[i - (int)(count_is_there / 2)];
                                    list_points_attach.Add(new System.Windows.Point(info_foot[count_foot_num].foot_pos.X * 0.001, info_foot[count_foot_num].foot_pos.Y * 0.001));
                                    //            list_points.Add(new System.Windows.Point(info_foot[count_foot_num].foot_pos.X * 0.001, info_foot[count_foot_num].foot_pos.Y * 0.001));
                                    count_foot_num++;
                                    //  }
                                }
                                count_is_there = 0;

                            }
                            else
                            {
                                count_is_there = 0;
                            }
                        }
                    }

                }

            }

            for (int i = 0; i < this.datanum; i++)
            {
                bool_is_in = false;

                for (int j = 0; j < count_foot_num; j++)
                {
                    double length = geoLength(info_foot[j].foot_pos.X, info_foot[j].foot_pos.Y, foot_pos_child[i].X - Point_offset.X, foot_pos_child[i].Y - Point_offset.Y);
                    //clientセンサーとの距離の閾値
                    // Console.WriteLine("Len:" + length);
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

            //現フレームの座標取得終了
            // this.SendData_media(list_points);
            //this.SendData_save(list_points);
            this.SendData_attach(list_points_attach);

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

        public double ave_vel(List<double> v)
        {
            double vel = 0;
            for (int i  = 0;i < v.Count;i++)
            {
                vel += v[i];
            }
            return vel / v.Count;
        }
        public void offset_point()
        {
            Point_offset.X = featurePoint[0].X;
            Point_offset.Y = featurePoint[0].Y;
            Console.WriteLine("x:" + Point_offset.X);
            Console.WriteLine("y:" + Point_offset.Y);
        }
        public void connect()
        {
            tcp = new System.Net.Sockets.TcpClient(host, port);
            ns = tcp.GetStream();
            ns.ReadTimeout = 3000;
            ns.WriteTimeout = 3000;
            
        }
        public void getBackgrounddata1()
        {
            for (int i = 0; i <= datacount - 1; i++)
            {
                point_background_x[i] = point_x[i];
                point_background_y[i] = point_y[i];
            }
        }
        public void getBackgrounddata2()
        {
            bool_button2_clicked = true;
            for (int i = 0; i < datacount - 2; i++)
            {
                point_background_x[i + datacount] = (-1) * point_x[i];
                point_background_y[i + datacount] = (-1) * point_y[i];
            }
        }
        public void getBackgrounddata_once()
        {
            timer_background.Enabled = true;
            /*
            
            for (int i = 0; i < datacount - 1; i++)
            {
                point_background_x[i] = point_x[i];
                point_background_y[i] = point_y[i];


            }
            */
        }
        public void Start_Robot_Control_Pos()
        {
            if (timer_robot_control.Enabled != true)
            {
                timer_robot_control.Enabled = true;
            }
            else
            {
                timer_robot_control.Start();
            }
        }
        public void Stop_Robot_Control_Pos()
        {
            SendData_Robot_to_Control(500);

            timer_robot_control.Stop();
        }
        public void WriteCsv()
        {
            /*
            try
            {
                bool append = false;

                using (var sw = new System.IO.StreamWriter("background.csv", append))
                {
                    double deg = 0;
                    for (int i = 90; i <=450 ; i++)
                    {
                        sw.WriteLine("{0},{1},{2},{3},{4},{5},{6}",i - 90,deg, point_x[i], point_y[i],point_background_x[i],point_background_y[i],distance_backgroundpoint[i]);
                        deg += 0.5;
                    }
                }
            }
            catch (System.Exception e)
            {
            }*/
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

        public void WriteFeaturePointcsv()
        {
            try
            {
                bool append = false;

                using (var sw = new System.IO.StreamWriter("featurePoint.csv", append))
                   {
                    for (int i = 0;i < FeaturePoint_save_num; i++) { 

                        sw.WriteLine("{0},{1}", Feature_point_First_save[i].X, Feature_point_First_save[i].Y);
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
        public void readfeaturePointfromcsv()
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

        public void WriteCsv2()
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
        
        
        //センサに命令を送るメソッド
        public void sendCommandtoLRF(int mode)
        {
            ns = tcp.GetStream();
            if (mode == 0)
            {
                sendMsg = "sEN LMDscandata 1";
                //byte[] sendBytes = enc.GetBytes(((char)2).ToString() );
               // byte[] sendBytes1 = enc.GetBytes(sendMsg );
                //byte[] sendBytes2 = enc.GetBytes(((char)3).ToString());

                byte[] sendBytes = enc.GetBytes(((char)2).ToString() +sendMsg + ((char)3).ToString());
                //char[] sendBytes = sendMsg.ToCharArray();
                this.ns.Write(sendBytes, 0, sendBytes.Length);
                //this.ns.Write(sendBytes1, 0, sendBytes.Length);
                //this.ns.Write(sendBytes2, 0, sendBytes.Length);
            }
        }
        public void background_study(object sender, EventArgs e)
        {
            for (int i = 0;i < datacount;i++)
            {
                //距離データ　丸め込み　割る100
                dist_temp_save[i].Add(((int)(int_dist[i] / 100)));
            }


        }
        public void background_histgram()
        {
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
            WriteCsv();
            for (int i = 0; i < datacount; i++)
            {
                //距離データ　丸め込み　割る100
                dist_temp_save[i].Clear();
            }
        }
        public void getdata(object sender, EventArgs e)
        {
            do
            {
                resSize = this.ns.Read(resBytes, 0, resBytes.Length);
            

                //Readが0を返した時はサーバーが切断したと判断
                if (resSize == 0)
                {
                    Console.WriteLine("サーバーが切断しました。");
                    break;
                }
                s = this.enc.GetString(resBytes);
                if (s == ((char)3).ToString())
                {
                    //Console.WriteLine(str);
                    strs = str.Split(' ');
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
            /*
            for (int i = 0;i < count_feature_point_num;i++)
            {
                Console.WriteLine("featurePoint:" + featurePoint[i]);
            }
            */
            Absolute_Point = ave_point(list_for_Absolute);
            
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
        public void getroomdata()
        {
            this.max_x = point_x.Max();
            this.min_x = point_x.Min();
            this.max_y = point_y.Max();
            this.min_y = point_y.Min();
        }

        public void printData()
        {
            for (int i = 0; i < datacount; i++)
            {
                Console.Write("" + int_dist[i]);
                Console.Write(" ");
            }
        }

        //点同士のキョリを求めるメソッド
        public double geoLength(double x1, double y1, double x2, double y2)
        {
            double ret = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
            return ret;
        }

        public void CIPC_Sender_Client_Media(string ip, int remote_port, int server_port)
        {
            this.sender_client_media = new CIPC_CS.CLIENT.CLIENT(remote_port, ip, server_port, "Laserscaner_Media03", 30);
            this.sender_client_media.Setup(CIPC_CS.CLIENT.MODE.Sender);
        }
        public void CIPC_Sender_Client_Robot(string ip, int remort_port, int server_port)
        {
            this.sender_client_robot = new CIPC_CS.CLIENT.CLIENT(remort_port, ip ,server_port, "Robot_Control", 30);
            this.sender_client_robot.Setup(CIPC_CS.CLIENT.MODE.Sender);

        }
        public void CIPC_sender_Sounddata_robot(string ip, int remort_port, int server_port)
        {
            this.sender_client_sounddata_robot = new CIPC_CS.CLIENT.CLIENT(remort_port, ip, server_port, "Send_Sounddata", 30);
            this.sender_client_sounddata_robot.Setup(CIPC_CS.CLIENT.MODE.Sender);

        }
        public void CIPC_Sender_Client_save(string ip, int myport, int server_port)
        {
            this.sender_client_save = new CIPC_CS.CLIENT.CLIENT(myport, ip, server_port, "Laserscaner_save03", 30);
            this.sender_client_save.Setup(CIPC_CS.CLIENT.MODE.Sender);
        }
        public void CIPC_Sender_Client_attach(string ip, int myport, int server_port)
        {
            this.sender_client_attach = new CIPC_CS.CLIENT.CLIENT(myport, ip, server_port, "Laserscaner_attach03", 30);
            this.sender_client_attach.Setup(CIPC_CS.CLIENT.MODE.Sender);
        }
        public void CIPC_Sender_Client_calib(string ip, int myport, int server_port)
        {
            this.sender_client_calib = new CIPC_CS.CLIENT.CLIENT(myport, ip, server_port, "Laserscaner_attach03", 30);
            this.sender_client_calib.Setup(CIPC_CS.CLIENT.MODE.Sender);
        }
        void SendData_media(List<System.Windows.Point> list_point)
        {
            try
            {
                if (this.sender_client_media != null)
                {

                    UDP_PACKETS_CODER.UDP_PACKETS_ENCODER enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();

                    enc += list_point.Count;// 足の本数
                    //Console.Write("list_point_count:" + list_point.Count);
                    for (int i = 0; i < list_point.Count; i++)
                    {
                        enc += list_point[i].X;
                        enc += list_point[i].Y;
                       // Console.WriteLine("list_point:" + list_point);
                    }

                    byte[] data = enc.data;
                    sender_client_media.Update(ref data);
                }
            }
            catch { }

        }

       public void SendData_Robot_to_Control(int num)
        {
            //Console.WriteLine("Send");
            try
            {
                if (this.sender_client_robot != null)
                {

                    UDP_PACKETS_CODER.UDP_PACKETS_ENCODER enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
 
                    enc += num;
                    byte[] data = enc.data;
                    sender_client_robot.Update(ref data);
                }
            }
            catch { }

        }
        public void SendData_Sounddata(System.Windows.Point point,double v)
        {
            //Console.WriteLine("Send");
            try
            {
                if (this.sender_client_sounddata_robot != null)
                {

                    UDP_PACKETS_CODER.UDP_PACKETS_ENCODER enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();

                    enc += point.X;
                    enc += point.Y;
                    enc += v;
                    byte[] data = enc.data;
                    sender_client_sounddata_robot.Update(ref data);
                }
            }
            catch { }

        }
        void SendData_save(List<Save_foot> list_save_foot)
        {
            try
            {
                if (this.sender_client_save != null)
                {

                    UDP_PACKETS_CODER.UDP_PACKETS_ENCODER enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();

                    enc += list_save_foot.Count;// 足の本数

                    for (int i = 0; i < list_save_foot.Count; i++)
                    {
                        enc += list_save_foot[i].Save_foot_pos.X;
                        enc += list_save_foot[i].Save_foot_pos.Y;
                        enc += list_save_foot[i].id;
                    }

                    byte[] data = enc.data;
                    sender_client_save.Update(ref data);
                }
            }
            catch { }


        }
        void SendData_attach(List<System.Windows.Point> list_point)//座標統合用CIPC_CLIENT
        {
            try
            {
                if (this.sender_client_attach != null)
                {

                    UDP_PACKETS_CODER.UDP_PACKETS_ENCODER enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();

                    enc += list_point.Count;// 足の本数

                    for (int i = 0; i < list_point.Count; i++)
                    {
                        enc += list_point[i].X;
                        enc += list_point[i].Y;
                    }

                    byte[] data = enc.data;
                    sender_client_attach.Update(ref data);
                }
            }
            catch { }
        }

        // 組合せ(nCr)を求める関数
        int Combination(int n, int r)
        {
            int ans;
            int i;

            // 引数チェック
            if (n < r || n < 0 || r < 0)
            {
            }

            ans = 1;

            for (i = 0; i < r; i++)
            {
                ans *= n;
                n -= 1;
            }

            for (i = 1; i <= r; i++)
            {
                ans /= i;
            }

            return ans;
        }
        public System.Windows.Point sum_point(List<System.Windows.Point> point)
        {
            System.Windows.Point p = new System.Windows.Point(0, 0);
            for (int i = 0; i < point.Count; i++)
            {
                p.X += point[i].X;
                p.Y += point[i].Y;

            }
            return p;
        }
        public System.Windows.Point ave_point(List<System.Windows.Point> point)
        {
            System.Windows.Point p = sum_point(point);
            p.X = p.X / point.Count;
            p.Y = p.Y / point.Count;
            return p;
        }
        public System.Windows.Point var_point(List<System.Windows.Point> point)
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
        public void CIPC_Receive_featurePos_from_rob(string ip, int remote_port, int server_port)
        {
            this.receive_feature_Pos = new CIPC_CS.CLIENT.CLIENT(remote_port, ip, server_port, "featurePos_Receiver", 30);
            this.receive_feature_Pos.Setup(CIPC_CS.CLIENT.MODE.Receiver);
            this.receive_feature_Pos.DataReceived += this.DataReceived_featurePoint;
        }

        public void DataReceived_featurePoint(object sender, byte[] e)//座標統合用Recieve_Client
        {
            try
            {
                RobotInfoStart = true;
                UDP_PACKETS_CODER.UDP_PACKETS_DECODER dec = new UDP_PACKETS_CODER.UDP_PACKETS_DECODER();
                dec.Source = e;
                this.featurePointnum_from_rob = dec.get_int();
                for (int i = 0; i < this.featurePointnum_from_rob; i++)
                {
                    this.featurePoint_from_rob[i].X = dec.get_double();
                    this.featurePoint_from_rob[i].Y = dec.get_double();

                }
                RobotInfoStart = true;

            }

            catch { }


        }
        public void getRobotinfo(object sender, EventArgs e)
        {
          //  Console.WriteLine("getRobotInfo");
            if (RobotInfoStart) {
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
                                                        while (End == true) {
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
                                                        //   Console.WriteLine("ReferencePoint1:" + ReferencePoint1);
                                                        //   Console.WriteLine("ReferencePoint2:" + ReferencePoint2);
                                                        //   Console.WriteLine("ReferencePoint3:" + ReferencePoint3);
                                                        //   Console.WriteLine("Referencedist1:" + Referencedist1);
                                                        //   Console.WriteLine("Referencedist2:" + Referencedist2);
                                                        //   Console.WriteLine("Referencedist3:" + Referencedist3);

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

        public void CIPC_Receive_Client_FM(string ip, int remote_port, int server_port)
        {
            this.receive_client_sc = new CIPC_CS.CLIENT.CLIENT(remote_port, ip, server_port, "Laserscaner_child", 30);
            this.receive_client_sc.Setup(CIPC_CS.CLIENT.MODE.Receiver);
            this.receive_client_sc.DataReceived += this.DataReceived_FM;
        }


        public void DataReceived_FM(object sender, byte[] e)//座標統合用Recieve_Client
        {

            try
            {
                UDP_PACKETS_CODER.UDP_PACKETS_DECODER dec = new UDP_PACKETS_CODER.UDP_PACKETS_DECODER();
                dec.Source = e;

                this.datanum = dec.get_int();

                for (int i = 0; i < this.datanum; i++)
                {
                    this.foot_pos_child[i].X = (dec.get_double() * 1000) ;
                    this.foot_pos_child[i].Y = (dec.get_double() * 1000) ;
                    //point.X = dec.get_double();
                    ///point.Y = dec.get_double();
                    //list.Add(point);
                    //    Console.WriteLine("X:" + foot_pos_child[i].X + "Y:" + foot_pos_child[i].Y);

                }

                //Console.WriteLine("get data");
            }
            catch { }


        }
        public void CIPC_Close()
        {
            try
            {
                this.sender_client_robot.Close();
            }
            catch { }
            try
            {
                this.receive_client_sc.Close();
            }
            catch { }
            try {
                this.sender_client_attach.Close();
            }
            catch { }
            try
            {
                this.sender_client_sounddata_robot.Close();
            }
            catch { }
            try {
                this.sender_client_media.Close();
            }
            catch
            {

            }
            try { this.sender_client_save.Close();
            }
            catch { }
            try
            {
                this.sender_client_calib.Close();
            }
            catch
            {

            }
            try
            {
                this.reciever_client_calib.Close();
            }
            catch
            {

            }
            try
            {
                this.receive_feature_Pos.Close();
            }
            catch
            {

            }
            try
            {
                this.sender_client_robot.Close();
            }
            catch
            {

            }
        }
        public void CIPC_Receive_Client_Calib(string ip, int remote_port, int server_port)
        {
            this.reciever_client_calib = new CIPC_CS.CLIENT.CLIENT(remote_port, ip, server_port, "Laserscaner_calib", 30);
            this.reciever_client_calib.Setup(CIPC_CS.CLIENT.MODE.Receiver);
            this.reciever_client_calib.DataReceived += this.DataRecieve_Calib;
        }
        //キャリブ用
        public void SendData_Calib()
        {
            try
            {
                if (this.sender_client_calib != null)
                {

                    UDP_PACKETS_CODER.UDP_PACKETS_ENCODER enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();


                    for (int i = 0; i < this.datacount - 1; i++)
                    {
                        enc += this.point_background_x[i];
                        enc += this.point_background_y[i];
                    }

                    byte[] data = enc.data;
                    sender_client_calib.Update(ref data);
                }
            }
            catch { }


        }
        //キャリブデータ受け取り
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
                    Console.WriteLine(this.point_background_x_client1[i]);
                    //point.X = dec.get_double();
                    ///point.Y = dec.get_double();
                    //list.Add(point);
                    //    Console.WriteLine("X:" + foot_pos_child[i].X + "Y:" + foot_pos_child[i].Y);

                }

                //Console.WriteLine("get data");
            }
            catch { }


        }
        public void MakeFile()
        {
            try
            {
                var append = false;

                using (var sw = new System.IO.StreamWriter("RobotPos.csv", append))
                {

                }
            }
            catch 
            {

            }
        }
        public void FeaturePoint_First()
        {
            FeaturePoint_save_num = count_feature_point_num;
            for (int i = 0;i < FeaturePoint_save_num;i++) {
                Feature_point_First_save[i] = featurePoint[i]; 
               // Console.WriteLine(Feature_point_First_save[i]);
            }
            DT_featurePoint_pos.Stop();
            DT_featurePoint_pos = null;
            for (int i = 0; i < 10; i++)
            {
                featurePoint[i] = new System.Windows.Point(0, 0);
            }
            WriteFeaturePointcsv();

        }
        public void robot_Control(object sender, EventArgs e)
        {
            if (bool_Robot_Control_Start == true) {
                // Console.WriteLine("RobotPos:" + RobotPos);
                // Console.WriteLine("PPos:" + Purpose_Point);
                double len = geoLength(RobotPos.X, RobotPos.Y, Purpose_Point.X, Purpose_Point.Y);
               // Console.WriteLine("Len:" + len);
                if (len < 0.3)//30cm以内
                {
                    SendData_Robot_to_Control(500);
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
                    SendData_Robot_to_Control(control_value);


                }
            }
        }

       
    }
}