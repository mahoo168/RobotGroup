using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using _2015_6_29_TCPTest;
using System.Threading;
using Microsoft.Win32;
namespace LaderState
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //変数宣言
        #region
        //描画用
        public System.Windows.Threading.DispatcherTimer DT;
        double length_canvas = 750;
        private static Point Canvas_center = new Point(375, 375);
        public System.Windows.Media.PointCollection Points_Lader;       
        private const int void_count = 1081;
        public DateTime front;
        public List<Line> linesCols;
        public List<Line> linesRows;
        public List<EllipseGeometry> Circles;
        public List<EllipseGeometry> PersonCircles;
        public List<EllipseGeometry> Person_archiev_Circles;
        public List<EllipseGeometry> RobotPosCircles;
        public List<EllipseGeometry> featurePointCircles;
        public List<EllipseGeometry> COPCircles;
        public List<EllipseGeometry> PreCOPCircles;
        public List<EllipseGeometry> PreCOPCircles_archiev;
        public List<EllipseGeometry> VirtualAvatar_Circle;
        public int count = 0;
        public List<Path> myPaths;
        public List<Path> PersonPath;
        public List<Path> Person_archiev_Path;
        public List<Path> Robot_Temp_Pos_Path;
        public List<Path> featurePointPath;
        public List<Path> COPPath;
        public List<Path> PreCOPPath;
        public List<Path> VirtualAvatar_Path;
        public List<Path> PreCOPPath_archiev;
        //Sick
        Sick_Dak_Main sick_Main;
        Sick_Dak_Robot sick_Robot;
        //ロボット操作
        RobotMoveContorl robotControl;
        Point analogStickPt;
        bool IsClicked_AnalogStick = false;
        double radius;
        bool IsMT = false;
        Vector purposePt;
        //保存用
        string arciveFolderPath;
        #endregion


        //init
        public MainWindow()
        {
            InitializeComponent();

            //
            this.sick_Main = new Sick_Dak_Main();
            this.sick_Robot = new Sick_Dak_Robot();
            this.sick_Main.getFeatureData_robot += this.sick_Robot.Get_FeaturePoint;
            this.robotControl = new RobotMoveContorl();
            this.robotControl.getPurPosePt += this.GetPurposePtofRobot_MT;

            //
            this.Circles = new List<EllipseGeometry>();
            this.myPaths = new List<Path>();
            this.PersonCircles = new List<EllipseGeometry>();
            this.Person_archiev_Circles = new List<EllipseGeometry>();
            this.COPCircles = new List<EllipseGeometry>();
            this.PreCOPCircles = new List<EllipseGeometry>();

            //
            this.PreCOPCircles_archiev = new List<EllipseGeometry>();
            this.RobotPosCircles = new List<EllipseGeometry>();
            this.PersonPath = new List<Path>();
            this.Person_archiev_Path = new List<Path>();
            this.Robot_Temp_Pos_Path = new List<Path>();
            this.featurePointCircles = new List<EllipseGeometry>();
            this.featurePointPath = new List<Path>();
            this.COPPath = new List<Path>();
            this.PreCOPPath = new List<Path>();
            this.PreCOPPath_archiev = new List<Path>();
            this.VirtualAvatar_Circle = new List<EllipseGeometry>();
            this.VirtualAvatar_Path = new List<Path>();
            this.linesCols = new List<Line>();
            this.linesRows = new List<Line>();

            this.CircleInit(void_count);
            this.lineinit();

            this.Points_Lader = new PointCollection();

            for (int i = 0; i < void_count; i++)
            {
                this.Points_Lader.Add(new Point(0, 0));
            }
            Circles[0].Center = new Point(0, 0);
            PersonCircles[0].Center = new Point(0, 0);
            Person_archiev_Circles[0].Center = new Point(0, 0);
            featurePointCircles[0].Center = new Point(300, 300);
            this.front = DateTime.Now;

            //描画関数まわす
            this.DT = new System.Windows.Threading.DispatcherTimer();
            this.DT.Interval = new TimeSpan(0, 0, 0, 0, 50);
            this.DT.Tick += DT_Tick;
            this.DT.Start();
            this.Comment.Content = "Connect Main SICK";

        }

        //描画
        #region
        void lineinit()
        {
            Point StartColumnPoint = new Point(0, 0);
            Point EndColumnPoint = new Point(length_canvas, 0);
            for (int i = 0; i < 9; i++)//Row
            {
                LineGeometry line_memori = new LineGeometry();
                line_memori.StartPoint = StartColumnPoint;
                line_memori.EndPoint = EndColumnPoint;
                Path memoripath = new Path();
                memoripath.Fill = Brushes.Black;
                memoripath.Stroke = Brushes.Black;
                memoripath.StrokeThickness = 0.2;
                Canvas_Lader.Children.Add(memoripath);
                memoripath.Data = line_memori;
                StartColumnPoint.Y += (length_canvas / 8);
                EndColumnPoint.Y += (length_canvas / 8);

            }
            Point StartRowPoint = new Point(0, 0);
            Point EndRowPoint = new Point(0, length_canvas);
            for (int i = 0; i < 9; i++)//Row
            {
                LineGeometry line_memori = new LineGeometry();
                line_memori.StartPoint = StartRowPoint;
                line_memori.EndPoint = EndRowPoint;
                Path memoripath = new Path();
                memoripath.Fill = Brushes.Black;
                memoripath.Stroke = Brushes.Black;
                memoripath.StrokeThickness = 0.4;
                Canvas_Lader.Children.Add(memoripath);
                memoripath.Data = line_memori;
                StartRowPoint.X += (length_canvas / 8);
                EndRowPoint.X += (length_canvas / 8);
            }

        }
        void DT_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < 50; i++)
            {
                this.COPCircles[i].Center = new Point(0, 0);

            }
            for (int i = 0; i < 100; i++)
            {
                this.PreCOPCircles[i].Center = new Point(0, 0);
            }
            //-------------------------------------------------------------------------------------------------
            //ロボットの目標点を表示
            //this.Text_PurPosePos.Text = "x:";
            //this.Text_PurPosePos.Text += sick_Main.Purpose_Point.X.ToString();
            //this.Text_PurPosePos.Text += " y:";
            //this.Text_PurPosePos.Text += sick_Main.Purpose_Point.Y.ToString();

            //this.Text_Robot_Pos.Text = "x:";
            //this.Text_Robot_Pos.Text += sick_Main.RobotPos.X.ToString();
            //this.Text_Robot_Pos.Text += " y:";
            //this.Text_Robot_Pos.Text += sick_Main.RobotPos.Y.ToString();

            //this.Text_Robot_Posture.Text = sick_Main.NowRobotPosture.ToString();

            // Text_ControlValue.Text = sick_Main.control_value.ToString();
            for (int i = 0; i < (sick_Main.datacount - 1) / 2 + 1; i++)
            {
                this.Circles[i].Center = new Point(375 + 375 * ((sick_Main.point_x[i * 2]) / 20000), 375 - 375 * ((sick_Main.point_y[i * 2]) / 20000));
            }
            for (int i = 0; i < sick_Main.featurePoint.Length; i++)
            {
                this.featurePointCircles[i].Center = new Point(375 + 375 * (sick_Main.featurePoint[i].X / 20000), 375 - 375 * (sick_Main.featurePoint[i].Y / 20000));
            }

            for (int i = 0; i < sick_Main.COP_count; i++)
            {
                this.COPCircles[i].Center = new Point(375 + 375 * (sick_Main.Point_COP[i].foot_pos.X / 20000), 375 - 375 * (sick_Main.Point_COP[i].foot_pos.Y / 20000));
                switch (sick_Main.Point_COP[i].id % 10)
                {
                    case 0:
                        COPPath[i].Fill = Brushes.Red;
                        break;
                    case 1:
                        COPPath[i].Fill = Brushes.Blue;
                        break;

                    case 2:
                        COPPath[i].Fill = Brushes.Yellow;
                        break;

                    case 3:
                        COPPath[i].Fill = Brushes.Green;
                        break;

                    case 4:
                        COPPath[i].Fill = Brushes.Violet;
                        break;

                    case 5:
                        COPPath[i].Fill = Brushes.Brown;
                        break;
                    case 6:
                        COPPath[i].Fill = Brushes.Pink;
                        break;

                    case 7:
                        COPPath[i].Fill = Brushes.LightBlue;
                        break;
                    case 8:
                        COPPath[i].Fill = Brushes.DarkBlue;
                        break;
                    case 9:
                        COPPath[i].Fill = Brushes.DarkGoldenrod;
                        break;
                    default:
                        break;

                }
            }
            for (int i = 0; i < 50; i++)
            {
                this.PersonCircles[i].Center = new Point(375 + 375 * ((sick_Main.info_foot[i].foot_pos.X) / 20000), 375 - 375 * ((sick_Main.info_foot[i].foot_pos.Y) / 20000));
                switch (sick_Main.info_foot[i].id % 10)
                {
                    case 0:
                        PersonPath[i].Fill = Brushes.Red;
                        break;
                    case 1:
                        PersonPath[i].Fill = Brushes.Blue;
                        break;

                    case 2:
                        PersonPath[i].Fill = Brushes.Yellow;
                        break;

                    case 3:
                        PersonPath[i].Fill = Brushes.Green;
                        break;

                    case 4:
                        PersonPath[i].Fill = Brushes.Violet;
                        break;

                    case 5:
                        PersonPath[i].Fill = Brushes.Brown;
                        break;
                    case 6:
                        PersonPath[i].Fill = Brushes.Pink;
                        break;

                    case 7:
                        PersonPath[i].Fill = Brushes.LightBlue;
                        break;
                    case 8:
                        PersonPath[i].Fill = Brushes.DarkBlue;
                        break;
                    case 9:
                        PersonPath[i].Fill = Brushes.DarkGoldenrod;
                        break;


                    default:
                        break;

                }
            }

            for (int i = 0; i < 100; i++)
            {

                this.PreCOPCircles[i].Center = new Point(375 + 375 * (sick_Main.info_PreCOP[i].foot_pos.X / 20000), 375 - 375 * (sick_Main.info_PreCOP[i].foot_pos.Y / 20000));
                switch (sick_Main.info_PreCOP[i].id % 10)
                {
                    case 0:
                        PreCOPPath[i].Fill = Brushes.Red;
                        break;
                    case 1:
                        PreCOPPath[i].Fill = Brushes.Blue;
                        break;

                    case 2:
                        PreCOPPath[i].Fill = Brushes.Yellow;
                        break;

                    case 3:
                        PreCOPPath[i].Fill = Brushes.Green;
                        break;

                    case 4:
                        PreCOPPath[i].Fill = Brushes.Violet;
                        break;

                    case 5:
                        PreCOPPath[i].Fill = Brushes.Brown;
                        break;
                    case 6:
                        PreCOPPath[i].Fill = Brushes.Pink;
                        break;

                    case 7:
                        PreCOPPath[i].Fill = Brushes.LightBlue;
                        break;
                    case 8:
                        PreCOPPath[i].Fill = Brushes.DarkBlue;
                        break;
                    case 9:
                        PreCOPPath[i].Fill = Brushes.DarkGoldenrod;
                        break;


                    default:
                        break;

                }
            }

            for (int i = 0; i < 100; i++)
            {

                this.PreCOPCircles_archiev[i].Center = new Point(375 + 375 * (sick_Main.info_PreCOP_archiev[i].foot_pos.X / 20000), 375 - 375 * (sick_Main.info_PreCOP_archiev[i].foot_pos.Y / 20000));
                switch (sick_Main.info_PreCOP_archiev[i].id % 10)
                {
                    case 0:
                        PreCOPPath_archiev[i].Fill = Brushes.Red;
                        break;
                    case 1:
                        PreCOPPath_archiev[i].Fill = Brushes.Blue;
                        break;

                    case 2:
                        PreCOPPath_archiev[i].Fill = Brushes.Yellow;
                        break;

                    case 3:
                        PreCOPPath_archiev[i].Fill = Brushes.Green;
                        break;

                    case 4:
                        PreCOPPath_archiev[i].Fill = Brushes.Violet;
                        break;

                    case 5:
                        PreCOPPath_archiev[i].Fill = Brushes.Brown;
                        break;
                    case 6:
                        PreCOPPath_archiev[i].Fill = Brushes.Pink;
                        break;

                    case 7:
                        PreCOPPath_archiev[i].Fill = Brushes.LightBlue;
                        break;
                    case 8:
                        PreCOPPath_archiev[i].Fill = Brushes.DarkBlue;
                        break;
                    case 9:
                        PreCOPPath_archiev[i].Fill = Brushes.DarkGoldenrod;
                        break;


                    default:
                        break;

                }
            }

            for (int i = 0; i < 50; i++)
            {
                this.Person_archiev_Circles[i].Center = new Point(375 + 375 * ((sick_Main.info_foot_archiev[i].foot_pos.X) / 20000), 375 - 375 * ((sick_Main.info_foot_archiev[i].foot_pos.Y) / 20000));
                switch (sick_Main.info_foot_archiev[i].id % 10)
                {
                    case 0:
                        Person_archiev_Path[i].Fill = Brushes.Red;
                        break;
                    case 1:
                        Person_archiev_Path[i].Fill = Brushes.Blue;
                        break;

                    case 2:
                        Person_archiev_Path[i].Fill = Brushes.Yellow;
                        break;

                    case 3:
                        Person_archiev_Path[i].Fill = Brushes.Green;
                        break;

                    case 4:
                        Person_archiev_Path[i].Fill = Brushes.Violet;
                        break;

                    case 5:
                        Person_archiev_Path[i].Fill = Brushes.Brown;
                        break;
                    case 6:
                        Person_archiev_Path[i].Fill = Brushes.Pink;
                        break;

                    case 7:
                        Person_archiev_Path[i].Fill = Brushes.LightBlue;
                        break;
                    case 8:
                        Person_archiev_Path[i].Fill = Brushes.DarkBlue;
                        break;
                    case 9:
                        Person_archiev_Path[i].Fill = Brushes.DarkGoldenrod;
                        break;


                    default:
                        break;

                }
            }


            this.RobotPosCircles[0].Center = new Point(375 + 375 * (sick_Main.RobotPos.X * 1000 / 20000), 375 - 375 * (sick_Main.RobotPos.Y * 1000 / 20000));
            this.VirtualAvatar_Circle[0].Center = new Point(375 + 375 * (sick_Main.Virtual_Avatar_Pos.X * 1000 / 20000), 375 - 375 * (sick_Main.Virtual_Avatar_Pos.Y * 1000 / 20000));

            //this.TextBlock_FPS.Text = ((double)1000 / (DateTime.Now - this.front).Milliseconds).ToString("N2");
            this.front = DateTime.Now;
            this.LineUpdate();

        }
        void LineUpdate()
        {
            for (int i = 1; i < this.linesCols.Count; i++)
            {
                linesCols[i].X1 = i * this.Grid_Main.ActualWidth / this.linesCols.Count;
                linesCols[i].X2 = i * this.Grid_Main.ActualWidth / this.linesCols.Count;
                linesCols[i].Y1 = 0;
                linesCols[i].Y2 = this.Grid_Main.ActualHeight;
            }
            for (int i = 1; i < this.linesRows.Count; i++)
            {
                linesRows[i].Y1 = i * this.Grid_Main.ActualHeight / this.linesRows.Count;
                linesRows[i].Y2 = i * this.Grid_Main.ActualHeight / this.linesRows.Count;
                linesRows[i].X1 = 0;
                linesRows[i].X2 = this.Grid_Main.ActualWidth;
            }
        }
        void CircleInit(int num)
        {
            Path rangepath = new Path();
            rangepath.Fill = Brushes.LightSeaGreen;
            rangepath.Stroke = Brushes.Black;
            rangepath.StrokeThickness = 0.5;

            EllipseGeometry range_Circle = new EllipseGeometry();
            range_Circle.Center = Canvas_center;
            range_Circle.RadiusX = 375;
            range_Circle.RadiusY = 375;
            Canvas_Lader.Children.Add(rangepath);
            rangepath.Data = range_Circle;

            for (int i = 0; i < sick_Main.datacount; i++)
            {

                Path myPath = new Path();
                myPath.Fill = Brushes.Red;
                myPath.Stroke = Brushes.Black;
                myPath.StrokeThickness = 0.5;
                myPaths.Add(myPath);

                EllipseGeometry Circle = new EllipseGeometry();
                Circle.Center = Canvas_center;
                Circle.RadiusX = 1;
                Circle.RadiusY = 1;
                Circles.Add(Circle);
                myPaths[i].Data = Circles[i];
                Canvas_Lader.Children.Add(myPaths[i]);
            }
            for (int i = 0; i < 100; i++)
            {


                Path ppath = new Path();
                ppath.Fill = Brushes.Orange;
                ppath.Stroke = Brushes.Black;
                ppath.StrokeThickness = 0.5;
                PersonPath.Add(ppath);

                Path p_archieve_path = new Path();
                p_archieve_path.Fill = Brushes.Orange;
                p_archieve_path.Stroke = Brushes.Black;
                p_archieve_path.StrokeThickness = 0.5;
                Person_archiev_Path.Add(p_archieve_path);
                EllipseGeometry p_archieve_Circle = new EllipseGeometry();
                p_archieve_Circle.Center = new Point(10000, 10000);
                p_archieve_Circle.RadiusX = 5;
                p_archieve_Circle.RadiusY = 5;
                Person_archiev_Circles.Add(p_archieve_Circle);
                Person_archiev_Path[i].Data = Person_archiev_Circles[i];
                Canvas_Lader.Children.Add(Person_archiev_Path[i]);



                EllipseGeometry pCircle = new EllipseGeometry();
                pCircle.Center = new Point(10000, 10000);
                pCircle.RadiusX = 5;
                pCircle.RadiusY = 5;
                PersonCircles.Add(pCircle);
                PersonPath[i].Data = PersonCircles[i];
                Canvas_Lader.Children.Add(PersonPath[i]);


                Path fpath = new Path();
                fpath.Fill = Brushes.Black;
                fpath.Stroke = Brushes.Black;
                fpath.StrokeThickness = 0.5;
                featurePointPath.Add(fpath);

                EllipseGeometry fCircle = new EllipseGeometry();
                fCircle.Center = new Point(10000, 10000);
                fCircle.RadiusX = 5;
                fCircle.RadiusY = 5;
                featurePointCircles.Add(fCircle);
                featurePointPath[i].Data = featurePointCircles[i];
                Canvas_Lader.Children.Add(featurePointPath[i]);
            }
            for (int i = 0; i < 50; i++)
            {
                Path path_cop = new Path();
                path_cop.Fill = Brushes.DarkOrange;
                path_cop.Stroke = Brushes.DarkOrange;
                path_cop.StrokeThickness = 0.5;
                COPPath.Add(path_cop);


                EllipseGeometry Circle_cop = new EllipseGeometry();
                Circle_cop.Center = new Point(10000, 10000);
                Circle_cop.RadiusX = 2;
                Circle_cop.RadiusY = 2;
                COPCircles.Add(Circle_cop);
                COPPath[i].Data = COPCircles[i];
                Canvas_Lader.Children.Add(COPPath[i]);


            }
            for (int i = 0; i < 100; i++)
            {
                Path path_pre_cop = new Path();
                path_pre_cop.StrokeThickness = 0.5;
                PreCOPPath.Add(path_pre_cop);


                EllipseGeometry Circle_pre_cop = new EllipseGeometry();
                Circle_pre_cop.Center = new Point(10000, 10000);
                Circle_pre_cop.RadiusX = 2;
                Circle_pre_cop.RadiusY = 2;
                PreCOPCircles.Add(Circle_pre_cop);
                PreCOPPath[i].Data = PreCOPCircles[i];
                Canvas_Lader.Children.Add(PreCOPPath[i]);

                Path path_cop_archiev = new Path();
                path_cop_archiev.StrokeThickness = 0.5;
                PreCOPPath_archiev.Add(path_cop_archiev);


                EllipseGeometry Circle_cop_archiev = new EllipseGeometry();
                Circle_cop_archiev.Center = new Point(10000, 10000);
                Circle_cop_archiev.RadiusX = 2;
                Circle_cop_archiev.RadiusY = 2;
                PreCOPCircles_archiev.Add(Circle_cop_archiev);
                PreCOPPath_archiev[i].Data = PreCOPCircles_archiev[i];
                Canvas_Lader.Children.Add(PreCOPPath_archiev[i]);
            }

            Path robotpath = new Path();
            robotpath.Fill = Brushes.Blue;
            robotpath.Stroke = Brushes.Blue;
            robotpath.StrokeThickness = 0.5;
            Robot_Temp_Pos_Path.Add(robotpath);

            EllipseGeometry robot_Circle = new EllipseGeometry();
            robot_Circle.Center = new Point(10000, 10000);
            robot_Circle.RadiusX = 2;
            robot_Circle.RadiusY = 2;
            RobotPosCircles.Add(robot_Circle);
            Robot_Temp_Pos_Path[0].Data = RobotPosCircles[0];
            Canvas_Lader.Children.Add(Robot_Temp_Pos_Path[0]);

            Path VirtualAvatarpath = new Path();
            VirtualAvatarpath.Fill = Brushes.Yellow;
            VirtualAvatarpath.Stroke = Brushes.Black;
            VirtualAvatarpath.StrokeThickness = 0.5;
            VirtualAvatar_Path.Add(VirtualAvatarpath);

            EllipseGeometry VirtualavatarCircle = new EllipseGeometry();
            VirtualavatarCircle.Center = new Point(0, 0);
            VirtualavatarCircle.RadiusX = 6;
            VirtualavatarCircle.RadiusY = 6;
            VirtualAvatar_Circle.Add(VirtualavatarCircle);
            VirtualAvatar_Path[0].Data = VirtualAvatar_Circle[0];
            Canvas_Lader.Children.Add(VirtualAvatar_Path[0]);

        }
        #endregion


        //button
        #region   
        //Main Sick
        #region
        private void button_sickconnect_Click(object sender, RoutedEventArgs e)
        {
            this.sick_Main.LMS100_init();

            
        }
        //原点
        private void Button_offset_point_Click(object sender, RoutedEventArgs e)
        {
            this.sick_Main.offset_point();
            this.sick_Main.WriteCsv_offset();
        }
        private void Offset_from_csv_Click(object sender, RoutedEventArgs e)
        {
            this.sick_Main.readoffsetfromcsv();
        }
        private void Offset_write_csv_Click(object sender, RoutedEventArgs e)
        {
            this.sick_Main.WriteCsv_offset();
        }
        //特徴点
        private void Button_featurepoint_Click(object sender, RoutedEventArgs e)
        {
            this.sick_Main.FeaturePoint_First();
            this.sick_Main.WriteFeaturePointcsv();
        }
        private void Button_FeaturePoint_from_csv_Click(object sender, RoutedEventArgs e)
        {
            this.sick_Main.readfeaturePointfromcsv();
        }
        private void Button_FeaturePoint_write_csv_Click(object sender, RoutedEventArgs e)
        {
            this.sick_Main.WriteFeaturePointcsv();
        }
        //背景データ
        private void Button_Write_Background_Once_Click(object sender, RoutedEventArgs e)
        {
            if (this.Button_Background_init.Content.ToString() == "背景学習開始")
            {
                sick_Main.getBackgrounddata_once();
                this.Button_Background_init.Content = "背景学習完了";

            }
            else
            {
                sick_Main.background_histgram();
                this.Button_Background_init.Content = "背景学習開始";
            }
        }
        private void Button_Background_From_csv_Click(object sender, RoutedEventArgs e)
        {
            sick_Main.readbackgroundfromcsv();
        }
        private void Button_save_background_Click(object sender, RoutedEventArgs e)
        {
            sick_Main.WriteCsv_background();
        }
        //アーカイブ
        private void Button_attach_folder_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog ofd = new System.Windows.Forms.FolderBrowserDialog();
            ofd.ShowDialog();
            this.arciveFolderPath = ofd.SelectedPath;
        }
        private void Button_Start_archiev_Click(object sender, RoutedEventArgs e)
        {
            this.sick_Main.Switch_SaveHumanPos(this.arciveFolderPath + "/" + this.WriteFileName.Text);
        }
        private void Button_StartGetFoot(object sender, RoutedEventArgs e)
        {
            sick_Main.bool_button1_clicked = true;
        }
        private void Button_save_Person_pos_Click(object sender, RoutedEventArgs e)
        {
            if (this.Button_save_Person_pos.Content.ToString() == "座標保存開始")
            {
                sick_Main.timer_save_data.Enabled = true;

                this.Button_save_Person_pos.Content = "座標保存停止";
            }
            else
            {
                sick_Main.timer_save_data.Stop();
                this.Button_save_Person_pos.Content = "座標保存開始";
            }

        }
        private void Button_attach_file_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = "";
            ofd.DefaultExt = "*.*";
            if (ofd.ShowDialog() == true)
            {
                this.ReadFileName.Text = System.IO.Path.GetFileName(ofd.FileName);
                sick_Main.filename = System.IO.Path.GetFileName(ofd.FileName);
                sick_Main.LoadData();
            }
            this.ReadFileName.Text = System.IO.Path.GetFileName(ofd.FileName);
        }
        #endregion
        //Robot
        #region        
        private void Button_Save_RobotPos_Click(object sender, RoutedEventArgs e)
        {
            if (this.Button_Save_RobotPos.Content.ToString() == "ロボット座標取得開始")
            {
                this.sick_Main.bool_save_robotPos = true;
                this.Button_Save_RobotPos.Content = "ロボット座標取得停止";
            }
            else
            {
                this.sick_Main.bool_save_robotPos = false;
                this.Button_Save_RobotPos.Content = "ロボット座標取得開始";
            }
        }
        private void Button_RobControl_pos_Click(object sender, RoutedEventArgs e)
        {
            if(this.Button_Start_RobControl_pos.Content.ToString() == "ロボットの位置制御開始")
            {
                sick_Main.bool_Robot_Control_Start = true;
                this.Button_Start_RobControl_pos.Content = "ロボットの位置制御停止";
            }
            else
            {
                sick_Main.bool_Robot_Control_Start = false;
                this.Button_Start_RobControl_pos.Content = "ロボットの位置制御開始";

            }
        }
        private void Button_Get_Room_info_Click(object sender, RoutedEventArgs e)
        {
            sick_Main.getmaxAndMin();
        }
        //仮想アナログスティック制御        
        void Button_RobotMT_Click(object sender, RoutedEventArgs e)
        {
            if (!this.IsMT)
            {
                this.Button_RobotMT.Content = "MT操作終了";
                this.IsMT = true;
                this.robotControl.modeIndex = 0;
            }
            else
            {
                this.Button_RobotMT.Content = "MT操作開始";
                this.IsMT = false;
            }

        }
        void AnalogStick_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.IsMT)
            {
                this.purposePt = new Vector();
                this.IsClicked_AnalogStick = true;
                this.radius = this.AnalogStick.Width / 2;
                this.analogStickPt = new Point(Canvas.GetLeft(this.AnalogStick) + this.radius, Canvas.GetTop(this.AnalogStick) + this.radius);
                Point mousePt = Mouse.GetPosition(this.AnalogStickField);

                Vector vec = mousePt - this.analogStickPt;
                vec /= vec.Length / 10;
                Canvas.SetLeft(this.AnalogStick, this.analogStickPt.X + vec.X - this.radius);
                Canvas.SetTop(this.AnalogStick, this.analogStickPt.Y + vec.Y - this.radius);
            }
            
        }
        void AnalogStick_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.IsClicked_AnalogStick)
            {
                Canvas.SetLeft(this.AnalogStick, this.analogStickPt.X - this.radius);
                Canvas.SetTop(this.AnalogStick, this.analogStickPt.Y - this.radius);
                this.IsClicked_AnalogStick = false;
            }

        }
        void AnalogStick_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.IsClicked_AnalogStick)
            {
                Point mousePt = Mouse.GetPosition(this.AnalogStickField);
                Vector vec = mousePt - this.analogStickPt;
                this.purposePt = vec / vec.Length;
                this.purposePt = (vec / vec.Length);
                vec /= vec.Length / 10;
                Canvas.SetLeft(this.AnalogStick, this.analogStickPt.X + vec.X - this.radius);
                Canvas.SetTop(this.AnalogStick, this.analogStickPt.Y + vec.Y - this.radius);
                

            }
        }
        Vector GetPurposePtofRobot_MT()
        {
            return this.purposePt;
        }
        #endregion
        //Calibrate Sub Sick
        private void Set_second_Sensor_Button_Click(object sender, RoutedEventArgs e)
        {
            sick_Main.Rotation_Client_Sensor = Double.Parse(this.Text_Second_sensor_rotation.Text);
            sick_Main.Position_Client_Sensor_x = Double.Parse(this.Text_Second_sensor_x.Text);
            sick_Main.Position_Client_Sensor_y = Double.Parse(this.Text_Second_sensor_y.Text);
        }
        //Sound
        #region
        private void Button_Send_Robot_SoundData_Click(object sender, RoutedEventArgs e)
        {
            //sick_Main.CIPC_sender_Sounddata_robot(this.IPadress.Text, int.Parse(this.Remote_port.Text), int.Parse(this.Server_port.Text));
        }
        private void Button_Normal_Collective_Sound_Click(object sender, RoutedEventArgs e)
        {
            Text_Output_Sound_Mode.Text = "基本集団音メディア";
            sick_Main.SoundMode = 0;
        }
        private void Button_Archiev_Collecttive_Sound_Click(object sender, RoutedEventArgs e)
        {
            Text_Output_Sound_Mode.Text = "アーカイブ集団音メディア";
            sick_Main.SoundMode = 1;
        }
        private void Button_TimeDelay_one_COllective_Sound_Click(object sender, RoutedEventArgs e)
        {
            Text_Output_Sound_Mode.Text = "時間遅れ集団音メディア（過去のみ）";
            sick_Main.SoundMode = 2;
        }
        private void Button_TimeDelay_sum_Collective_sound_Click(object sender, RoutedEventArgs e)
        {
            Text_Output_Sound_Mode.Text = "時間遅れ集団音メディア（過去+現フレーム）";
            sick_Main.SoundMode = 3;
        }
        private void Button_VirtualAvatar_Collective_Sound_Click(object sender, RoutedEventArgs e)
        {
            //Text_Output_Sound_Mode.Text = "仮想アバター集団音メディア";
            sick_Main.SoundMode = 4;
        }
        private void Individual_Sound_Mode_Click(object sender, RoutedEventArgs e)
        {
            //Text_Output_Sound_Mode.Text = "個人独立音生成モード";
            sick_Main.SoundMode = 5;
        }
        #endregion     
        //通信用Client
        private void Connect_btn_Click(object sender, RoutedEventArgs e)
        {
            string clientName = "";
            //ロボット上のSickのデータを取る
            if (this.RB_Robot.IsChecked == true)
            {
                this.sick_Robot.ConnectUDP(this.IP.Text, int.Parse(this.myport.Text));
                this.sick_Main.Connect_Robot();
                this.Comment.Content = "Connect Robot SICK UDP";
                clientName = "UDP:RobotSick";
              
            }
            //SubSickのデータ取得UDP
            if (this.RB_SubSick.IsChecked == true)
            {
                this.Comment.Content = "Connect Sub SICK UDP";
                clientName = "UDP:SubSic:k";

            }
            //SubSickのデータ取得CIPC
            if (this.RB_SubSick_CIPC_data.IsChecked == true)
            {
                this.sick_Main.CreateCIPCClient_SubSick_Data(this.IP.Text, int.Parse(this.myport.Text), int.Parse(this.serverport.Text));
                this.Comment.Content = "Connect Sub SICK CIPC";
                clientName = "CIPC:SubSickData:";

            }
            //SubSickのキャリブ情報取得CIPC
            if (this.RB_SubSick_CIPC_calib.IsChecked == true)
            {
                this.sick_Main.CreateCIPCClient_SubSick_Calib(this.IP.Text, int.Parse(this.myport.Text), int.Parse(this.serverport.Text));
                this.Comment.Content = "Connect Sub SICK Calib CIPC";
                clientName = "CIPC:SubSickCalib:";
            }
            //ロボット制御用のUDP
            if (this.RB_RobotControl.IsChecked == true)
            {
                this.robotControl = new RobotMoveContorl();
                this.robotControl.getHumanPos = this.sick_Main.Get_ListHumanPos;
                this.robotControl.getRobotPos = this.sick_Main.Get_RobotPos;
                this.robotControl.ConnectClient(this.IP.Text, int.Parse(this.remoteport.Text));

                clientName = "UDP:RobotCtrl:";
            }
            //リストに追加
            this.listBox_client.Items.Add(clientName + this.IP.Text + "," + this.myport.Text);
        }
        #endregion 
              

        //終了処理
        protected override void OnClosed(EventArgs e)
        {

            base.OnClosed(e);


            if (this.sick_Main != null)
            {
                this.sick_Main.Close();
            }
            if (this.sick_Robot != null)
            {
                this.sick_Robot.Close();
            }
            if (this.DT != null)
            {
                this.DT.Stop();
                this.DT = null;
            }
            if (this.robotControl != null)
            {
                this.robotControl.Close();
            }

        }

        
    }
}