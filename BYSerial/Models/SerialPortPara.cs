using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Ports;


namespace BYSerial.Models
{
    public class SerialPortPara
    {
        public string PortName { get; set; } = "COM1";

        public int BaudRate { get; set; } = 9600;

        public Parity Parity { get; set; } = Parity.None;

        public int DataBits { get; set; } = 8;

        public StopBits StopBits { get; set; } = StopBits.One;
        /// <summary>
        /// dtr/dsr流控
        /// </summary>
        public bool DtrEnable { get; set; } = true;
        /// <summary>
        /// rts/cts流控
        /// </summary>
        public bool RtsEnable { get; set; } = true;

        public SerialPortPara(string portname,int baudrate,int parity,int databits,int stopbits,bool dtrenable,bool rtsenable )
        {
            PortName = portname;
            BaudRate = baudrate;
            Parity = (Parity)parity;
            DataBits = databits;
            StopBits=(StopBits)stopbits;
            DtrEnable = dtrenable;
            RtsEnable = rtsenable;
        }

    }
}
