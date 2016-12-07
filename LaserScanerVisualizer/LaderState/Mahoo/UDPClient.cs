using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace LaderState
{
    delegate void _recieved(object sender, byte[] data);

    class UDPClient
    {
        
        public _recieved data_recieved;

        private UdpClient client;
        private string ipAdress;
        private int myPort=0;
        private int remotePort=0;

        System.Net.IPEndPoint endPint;


        public UDPClient() { }


        //クライアントセットアップ
        public void Client_setup(string ip, int myport, int remoteport)
        {
            this.ipAdress = ip;
            this.myPort = myport;
            this.remotePort = remoteport;           
        }
        public void Client_setup_reciever(string ip, int myport)
        {
            this.ipAdress = ip;
            this.myPort = myport;
            this.endPint = new System.Net.IPEndPoint(System.Net.IPAddress.Parse(this.ipAdress), this.myPort);
        }
        public void Client_setup_sender(string ip,int remoteport)
        {
            this.ipAdress = ip;
            this.remotePort = remoteport;
        }

        //送信
        public void send(byte[] data)
        {
            try
            {
                if(this.remotePort != 0)
                {
                    this.client.Send(data, data.Length, this.ipAdress, this.remotePort);
                }
            }
            catch { }
        }
        public void send_sync(byte[] data)
        {
            try
            {
                if(this.remotePort != 0)
                {
                    this.client = new UdpClient();
                    client.BeginSend(data, data.Length, this.ipAdress, this.remotePort, send_callback, client);
                }
                
            }
            catch { }
            
        }
        void send_callback(IAsyncResult ar)
        {
            try
            {
                UdpClient sendClient = (UdpClient)ar.AsyncState;
                sendClient.EndSend(ar);
            }
            catch { }
            
        }

        //受信
        public byte[] recive()
        {
            try
            {
                if (this.client == null)
                {
                    this.client = new UdpClient(this.myPort);
                }
                byte[] data = this.client.Receive(ref this.endPint);
                return data;
            }
            catch { return null; }
            
        }
        public void start_recieve()
        {
            try
            {
                if(this.myPort != 0)
                {
                    
                    this.client = new UdpClient(this.myPort);

                    client.BeginReceive(recieve_Callback, this.client);
                }
                
            }
            catch { }
            
        }
        void recieve_Callback(IAsyncResult ar)
        {
            UdpClient recvClient = (UdpClient)ar.AsyncState;
            try
            {
                byte[] data = recvClient.EndReceive(ar, ref this.endPint);
                this.data_recieved(client, data);
                client.BeginReceive(recieve_Callback, client);
            }
            catch { }
            
        }
        
        public void Close()
        {
            if(this.client != null)
            {
                this.client.Close();
            }
            
        }

    }
}
