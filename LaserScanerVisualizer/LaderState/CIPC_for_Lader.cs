using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaderState
{

    class CIPC_for_Lader
    {
        CIPC_CS.CLIENT.CLIENT sender_client_media;
        CIPC_CS.CLIENT.CLIENT sender_client_save;

        public CIPC_for_Lader()
        {

        }
        public void CIPC_Sender_Client_Media(string ip, int remote_port, int server_port)
        {


            this.sender_client_media = new CIPC_CS.CLIENT.CLIENT(remote_port, ip, server_port, "Laserscaner", 30);
            this.sender_client_media.Setup(CIPC_CS.CLIENT.MODE.Sender);
            // Console.WriteLine("a");
        }
        public void CIPC_Sender_Client_save(string ip, int remote_port, int server_port)
        {


            this.sender_client_save = new CIPC_CS.CLIENT.CLIENT(remote_port, ip, server_port, "Laserscaner", 30);
            this.sender_client_save.Setup(CIPC_CS.CLIENT.MODE.Sender);
            // Console.WriteLine("a");
        }

        void SendData(List<System.Windows.Point> list_point)
        {
            try
            {
                if (this.sender_client_media != null)
                {
                    //Console.WriteLine("b");

                    UDP_PACKETS_CODER.UDP_PACKETS_ENCODER enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();

                    enc += list_point.Count;// 人数

                    for (int i = 0; i < list_point.Count; i++)
                    {
                        enc += list_point[i].X;
                        enc += list_point[i].Y;
                    }

                    byte[] data = enc.data;
                    sender_client_media.Update(ref data);
                }
            }
            catch { }


        }

    }

}