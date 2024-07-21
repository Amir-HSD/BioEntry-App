using BioEntry_App.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;

namespace BioEntry_App.ViewModel
{
    public class FingerPrintViewModel
    {
        SerialPort SerialPort;

        FingerprintView FingerprintView;
        public FingerPrintViewModel(FingerprintView fpv)
        {
            FingerprintView = fpv;
            SerialPort = new SerialPort("COM3", 115200);
            OpenCloseSerialPort();
            SerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            
        }

        public void OpenCloseSerialPort()
        {
            if (!SerialPort.IsOpen)
            {
                SerialPort.Open();
            }
            else
            {
                SerialPort.Close();
            }
        }

        public void SendSerialCommand(string command)
        {
            if (SerialPort.IsOpen)
            {
                SerialPort.Write(command);
            }
        }

        public void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string data = sp.ReadLine();
            if (data != null && data != "")
            {
                if (data.Contains("ID"))
                {
                    int FingerId = Convert.ToInt32(data.Remove(0, data.IndexOf('#') + 1));
                    MessageBox.Show("Id received: " + FingerId);
                    SendSerialCommand("N");
                    
                }
            }
            
        }

        public void ScanFinger()
        {
            SendSerialCommand("S");
        }

    }
}
