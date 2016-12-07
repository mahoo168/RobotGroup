using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaderState
{
    public delegate List<foot> _getHumanPos();
    public delegate System.Windows.Point _getRobotPos();
    public delegate System.Windows.Vector _getPurPosePt();
    
    /// <summary>
    /// ロボットの目標値を計算するクラス
    /// </summary>
    class RobotMoveContorl
    {
        //UDP      
        UDPClient client_RobotControl;

        System.Threading.Thread thread;
        List<Func< List<foot>, System.Windows.Point, System.Windows.Vector>> list_calc;
        public int modeIndex = 0;
        public _getHumanPos getHumanPos;
        public _getRobotPos getRobotPos;
        public _getPurPosePt getPurPosePt;
        public RobotMoveContorl()
        {
            this.list_calc = new List<Func<List<foot>, System.Windows.Point, System.Windows.Vector>>();
            Func<List<foot>, System.Windows.Point, System.Windows.Vector> func;
            //移動制御関数の登録
            //実際に動かす関数はthis.modeIndexで指示
            func = this.MTcontrol;
            this.list_calc.Add(func);
            func = this.JusinGentenTaisho;
            this.list_calc.Add(func);

            
        }

        //メインループ
        //UDPつないだと同時にスレッドで回す
        void Mainthread()
        {
            while (true)
            {
                try
                {
                    //データ取得
                    List<foot> list_foot = this.getHumanPos();
                    System.Windows.Point robotPos = this.getRobotPos();

                    //位置算出
                    System.Windows.Vector purposePt = this.list_calc[this.modeIndex](list_foot, robotPos);

                    //角度とサイズに直す
                    int dec = (int)Math.Atan(purposePt.Y / purposePt.X);
                    int speed = (int)Math.Sqrt(purposePt.X * purposePt.X + purposePt.Y * purposePt.Y);
                    //機体へ送信
                    this.Send(dec, speed);
                }
                catch { }
            }
            
        }

        //移動制御側　ここに追加していく
        //重心原点対象
        System.Windows.Vector MTcontrol(List<foot> list_foot, System.Windows.Point robotPos)
        {
            System.Windows.Vector purposePt = this.getPurPosePt();

            return purposePt;
        }

        //重心原点対象
        System.Windows.Vector JusinGentenTaisho(List<foot> list_foot, System.Windows.Point robotPos)
        {
            System.Windows.Point center;
            List<System.Windows.Point> list_humanPos = new List<System.Windows.Point>();
            foreach(var p in list_foot)
            {
                list_humanPos.Add(p.foot_pos);
            }

            center = this.calculate_CenterPos(list_humanPos);
            center.X *= -1;
            center.Y *= -1;

            System.Windows.Vector purposePt = center -  robotPos;

            return purposePt;
        }

        //UDP通信
        public void ConnectClient(string ip, int port)
        {
            try
            {
                //
                this.client_RobotControl = new UDPClient();
                this.client_RobotControl.Client_setup_sender(ip, port);

                //
                this.thread = new System.Threading.Thread(new System.Threading.ThreadStart(this.Mainthread));
                this.thread.Start();


            } catch { }
        }
        void Send(int dec, int speed)
        {
            List<byte> data = new List<byte>();
            data.AddRange(BitConverter.GetBytes(dec));
            data.AddRange(BitConverter.GetBytes(speed));

            this.client_RobotControl.send(data.ToArray());
        }
        public void Close()
        {
            if(this.client_RobotControl != null)
            {
                this.client_RobotControl.Close();
                this.thread.Abort();
            }
        }

        //計算用
        System.Windows.Point calculate_CenterPos(List<System.Windows.Point> list_pos)
        {
            System.Windows.Point centerPos = new System.Windows.Point(0,0);
            foreach(var p in list_pos)
            {
                centerPos.X += p.X;
                centerPos.Y += p.Y;
            }
            centerPos.X /= list_pos.Count;
            centerPos.Y /= list_pos.Count;
            return centerPos;
        }


    }
}
