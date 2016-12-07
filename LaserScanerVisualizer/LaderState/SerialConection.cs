using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Drawing;
using System.IO.Ports;
namespace LaderState
{

    public class SerialConection
    {

        private string portname = "COM14";
        public SerialPort serial;

        public SerialConection()
        {
            InitializeSerial();
        }
        private void InitializeSerial()
        {

            try
            {
                serial = new SerialPort(portname, 9600);
                Console.WriteLine("Serial Open:" + portname);
                serial.Open();
            }
            catch
            {
                Console.WriteLine("Serial Error!!");
            }
        }

        public void sendToArduino(string num)
        {
            try
            {
                if (int.Parse(num) <= 255)
                {
                    serial.Write(new byte[] { Convert.ToByte(num) }, 0, 1);
                    serial.Write(new byte[] { Convert.ToByte("0") }, 0, 1);

                }
                else
                {
                    serial.Write(new byte[] { Convert.ToByte("255") }, 0, 1);
                    string amari = (int.Parse(num) - 255).ToString();
                    serial.Write(new byte[] { Convert.ToByte(amari) }, 0, 1);
                    serial.Write(new byte[] { Convert.ToByte("0") }, 0, 1);
                    Console.WriteLine("Test");
                }
            }
            catch
            {
                Console.WriteLine("Serial Error!!");
            }

        }
        public void SerialPortClose()
        {

            serial.Close();
        }
    }
}


