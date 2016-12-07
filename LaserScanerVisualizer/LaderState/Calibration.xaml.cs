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
namespace LaderState
{
    /// <summary>
    /// Calibration.xaml の相互作用ロジック
    /// </summary>
    public partial class Calibration : Window
    {
        double max_range = 10000;
        private static Point Canvas_center = new Point(375, 375);
        public List<Path> myPaths;
        public List<EllipseGeometry> Circles;
        public List<Path> myPaths_client1;
        public List<EllipseGeometry> Circles_client1;
        public sick_dak sick1;
        public double degree = 0;
        private System.Windows.Point[] Client_Background_Point;
        public List<System.Windows.Point> point_background;

        public Calibration()
        {
            InitializeComponent();

            this.MouseLeftButtonDown += (s, e) => this.DragMove();
            this.KeyDown += Callibration_Keydown;
            this.Closed += CalibWindow_Closed;
            this.Closing += CalibWindow_Closing;
            this.point_background = new List<Point>();
            sick1 = new sick_dak();
            this.myPaths = new List<Path>();
            this.Circles = new List<EllipseGeometry>();
            for (int i = 0;i < 541 - 1;i++)
            {
                this.Client_Background_Point = new System.Windows.Point[541];
            }
            this.myPaths_client1 = new List<Path>();
            this.Circles_client1 = new List<EllipseGeometry>();
            for (int i = 0; i < sick1.datacount; i++)
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

                Path myPath_client1 = new Path();
                myPath_client1.Fill = Brushes.Blue;
                myPath_client1.Stroke = Brushes.Black;
                myPath_client1.StrokeThickness = 0.5;
                myPaths_client1.Add(myPath_client1);

                EllipseGeometry Circle_client1 = new EllipseGeometry();
                Circle_client1.Center = Canvas_center;
                Circle_client1.RadiusX = 1;
                Circle_client1.RadiusY = 1;
                Circles_client1.Add(Circle_client1);
                myPaths_client1[i].Data = Circles_client1[i];
                Canvas_Lader.Children.Add(myPaths_client1[i]);
            }
        }
        void Callibration_Keydown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
        void CalibWindow_Closing(object sender, EventArgs e)
        {
          
        }
        void CalibWindow_Closed(object sender, EventArgs e)
        {
         
        }

        private void Button_Write_self_env_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < sick1.datacount - 1; i++)
            {
                this.Circles[i].Center = new Point(375 + 375 * (sick1.point_background_x[i] / max_range), 375 - 375 * (sick1.point_background_y[i] / max_range));
            }
        }

        private void Button_Write_Client1_Env_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < sick1.datacount - 1; i++)
            {
                this.Circles_client1[i].Center = new Point(375 + 375 * (sick1.point_background_x_client1[i] / max_range), 375 - 375 * (sick1.point_background_y_client1[i] / max_range));
            }
        }

        private void Button_Background_once_Click(object sender, RoutedEventArgs e)
        {
            sick1.getBackgrounddata_once();
        }

        private void Button_calib_sendside_Click(object sender, RoutedEventArgs e)
        {
            sick1.CIPC_Sender_Client_calib(this.IPadress.Text, int.Parse(this.Remote_port.Text), int.Parse(this.Server_port.Text));
        }

        private void Button_calib_receive_side_Click(object sender, RoutedEventArgs e)
        {
            sick1.CIPC_Receive_Client_Calib(this.IPadress.Text, int.Parse(this.Remote_port.Text), int.Parse(this.Server_port.Text));

        }

        private void Button_calib_send_backgrounddata_Click(object sender, RoutedEventArgs e)
        {
            sick1.SendData_Calib();
        }
        //AからみたＢの座標
        private void Button_Plus_x_Click(object sender, RoutedEventArgs e)
        {
            this.X_Value.Text = (int.Parse(X_Value.Text) + int.Parse(Text_Cahnge_Value.Text)).ToString();

            for (int i = 0; i < sick1.datacount; i++)
            {
                this.Client_Background_Point[i].X = sick1.point_background_x_client1[i] * Math.Cos(-Math.PI * this.degree / 180) - sick1.point_background_y_client1[i] * Math.Sin(-Math.PI * this.degree / 180);
                this.Client_Background_Point[i].Y = sick1.point_background_x_client1[i] * Math.Sin(-Math.PI * this.degree / 180) + sick1.point_background_y_client1[i] * Math.Cos(-Math.PI * this.degree / 180);
            }
            for (int i = 0; i < sick1.datacount - 1; i++)
            {
                this.Client_Background_Point[i].X = this.Client_Background_Point[i].X + Double.Parse(this.X_Value.Text);
                this.Client_Background_Point[i].Y = this.Client_Background_Point[i].Y + Double.Parse(this.Y_Value.Text);
            }
            for (int i = 0; i < sick1.datacount - 1; i++)
            {
                this.Circles_client1[i].Center = new Point(375 + 375 * (this.Client_Background_Point[i].X / max_range), 375 - 375 * (this.Client_Background_Point[i].Y / max_range));
            }

        }

        private void Button_Minus_x_Click(object sender, RoutedEventArgs e)
        {
            this.X_Value.Text = (int.Parse(X_Value.Text) - int.Parse(Text_Cahnge_Value.Text)).ToString();

            for (int i = 0; i < sick1.datacount; i++)
            {
                this.Client_Background_Point[i].X = sick1.point_background_x_client1[i] * Math.Cos(-Math.PI * this.degree / 180) - sick1.point_background_y_client1[i] * Math.Sin(-Math.PI * this.degree / 180);
                this.Client_Background_Point[i].Y = sick1.point_background_x_client1[i] * Math.Sin(-Math.PI * this.degree / 180) + sick1.point_background_y_client1[i] * Math.Cos(-Math.PI * this.degree / 180);
            }
            for (int i = 0; i < sick1.datacount - 1; i++)
            {
                this.Client_Background_Point[i].X = this.Client_Background_Point[i].X + Double.Parse(this.X_Value.Text);
                this.Client_Background_Point[i].Y = this.Client_Background_Point[i].Y + Double.Parse(this.Y_Value.Text);
            }
            for (int i = 0; i < sick1.datacount - 1; i++)
            {
                this.Circles_client1[i].Center = new Point(375 + 375 * (this.Client_Background_Point[i].X / max_range), 375 - 375 * (this.Client_Background_Point[i].Y / max_range));
            }
        }

        private void Button_Plus_y_Click(object sender, RoutedEventArgs e)
        {
            this.Y_Value.Text = (int.Parse(Y_Value.Text) + int.Parse(Text_Cahnge_Value.Text)).ToString();

            for (int i = 0; i < sick1.datacount; i++)
            {
                this.Client_Background_Point[i].X = sick1.point_background_x_client1[i] * Math.Cos(-Math.PI * this.degree / 180) - sick1.point_background_y_client1[i] * Math.Sin(-Math.PI * this.degree / 180);
                this.Client_Background_Point[i].Y = sick1.point_background_x_client1[i] * Math.Sin(-Math.PI * this.degree / 180) + sick1.point_background_y_client1[i] * Math.Cos(-Math.PI * this.degree / 180);
            }
            for (int i = 0; i < sick1.datacount - 1; i++)
            {
                this.Client_Background_Point[i].X = this.Client_Background_Point[i].X + Double.Parse(this.X_Value.Text);
                this.Client_Background_Point[i].Y = this.Client_Background_Point[i].Y + Double.Parse(this.Y_Value.Text);
            }
            for (int i = 0; i < sick1.datacount - 1; i++)
            {
                this.Circles_client1[i].Center = new Point(375 + 375 * (this.Client_Background_Point[i].X / max_range), 375 - 375 * (this.Client_Background_Point[i].Y / max_range));
            }
        }

        private void Button_Minus_y_Click(object sender, RoutedEventArgs e)
        {
            this.Y_Value.Text = (int.Parse(Y_Value.Text) - int.Parse(Text_Cahnge_Value.Text)).ToString();

            for (int i = 0; i < sick1.datacount; i++)
            {
                this.Client_Background_Point[i].X = sick1.point_background_x_client1[i] * Math.Cos(-Math.PI * this.degree / 180) - sick1.point_background_y_client1[i] * Math.Sin(-Math.PI * this.degree / 180);
                this.Client_Background_Point[i].Y = sick1.point_background_x_client1[i] * Math.Sin(-Math.PI * this.degree / 180) + sick1.point_background_y_client1[i] * Math.Cos(-Math.PI * this.degree / 180);
            }
            for (int i = 0; i < sick1.datacount - 1; i++)
            {
                this.Client_Background_Point[i].X = this.Client_Background_Point[i].X + Double.Parse(this.X_Value.Text);
                this.Client_Background_Point[i].Y = this.Client_Background_Point[i].Y + Double.Parse(this.Y_Value.Text);
            }
            for (int i = 0; i < sick1.datacount - 1; i++)
            {
                this.Circles_client1[i].Center = new Point(375 + 375 * (this.Client_Background_Point[i].X / max_range), 375 - 375 * (this.Client_Background_Point[i].Y / max_range));
            }
        }

        private void Button_Plus_deg_Click(object sender, RoutedEventArgs e)
        {
            this.Rotation_value.Text = (int.Parse(Rotation_value.Text) + int.Parse(Text_Cahnge_Value.Text)).ToString();

                degree += Double.Parse(this.Text_Cahnge_Value.Text);
            // Console.WriteLine("Degree:" + degree);

   /*         for (int i = 0; i < sick1.datacount - 1; i++)
            {
                this.Client_Background_Point[i].X = sick1.point_background_x_client1[i];
                this.Client_Background_Point[i].Y = sick1.point_background_x_client1[i];
            }
            */

            for (int i = 0; i < sick1.datacount; i++)
            {
                this.Client_Background_Point[i].X = sick1.point_background_x_client1[i] * Math.Cos(-Math.PI * this.degree / 180) - sick1.point_background_y_client1[i]* Math.Sin(-Math.PI * this.degree / 180);
                this.Client_Background_Point[i].Y = sick1.point_background_x_client1[i] * Math.Sin(-Math.PI * this.degree / 180) + sick1.point_background_y_client1[i] * Math.Cos(-Math.PI * this.degree / 180);
            }
            for (int i = 0; i < sick1.datacount - 1; i++)
            {
                this.Client_Background_Point[i].X = this.Client_Background_Point[i].X + Double.Parse(this.X_Value.Text);
                this.Client_Background_Point[i].Y = this.Client_Background_Point[i].Y + Double.Parse(this.Y_Value.Text);
            }
            for (int i = 0; i < sick1.datacount - 1; i++)
            {
                this.Circles_client1[i].Center = new Point(375 + 375 * (this.Client_Background_Point[i].X / max_range), 375 - 375 * (this.Client_Background_Point[i].Y / max_range));
            }
        }

        private void Button_Minus_deg_Click(object sender, RoutedEventArgs e)
        {

            this.Rotation_value.Text = (int.Parse(Rotation_value.Text) - int.Parse(Text_Cahnge_Value.Text)).ToString();

            degree -= Double.Parse(this.Text_Cahnge_Value.Text);
            // Console.WriteLine("Degree:" + degree);

            /*         for (int i = 0; i < sick1.datacount - 1; i++)
                     {
                         this.Client_Background_Point[i].X = sick1.point_background_x_client1[i];
                         this.Client_Background_Point[i].Y = sick1.point_background_x_client1[i];
                     }
                     */

            for (int i = 0; i < sick1.datacount; i++)
            {
                this.Client_Background_Point[i].X = sick1.point_background_x_client1[i] * Math.Cos(-Math.PI * this.degree / 180) - sick1.point_background_y_client1[i] * Math.Sin(-Math.PI * this.degree / 180);
                this.Client_Background_Point[i].Y = sick1.point_background_x_client1[i] * Math.Sin(-Math.PI * this.degree / 180) + sick1.point_background_y_client1[i] * Math.Cos(-Math.PI * this.degree / 180);
            }
            for (int i = 0; i < sick1.datacount - 1; i++)
            {
                this.Client_Background_Point[i].X = this.Client_Background_Point[i].X + Double.Parse(this.X_Value.Text); 
                this.Client_Background_Point[i].Y = this.Client_Background_Point[i].Y + Double.Parse(this.Y_Value.Text);
            }
            for (int i = 0; i < sick1.datacount - 1; i++)
            {
                this.Circles_client1[i].Center = new Point(375 + 375 * (this.Client_Background_Point[i].X / max_range), 375 - 375 * (this.Client_Background_Point[i].Y / max_range));
            }
        }

        private void Button_Background_end_Click(object sender, RoutedEventArgs e)
        {
            sick1.background_histgram();
        }
    }
}
