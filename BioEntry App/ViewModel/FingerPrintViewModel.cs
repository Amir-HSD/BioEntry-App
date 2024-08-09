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
using System.Net.Http;
using System.Security.Policy;
using Newtonsoft.Json;
using BioEntry_App.Model;

namespace BioEntry_App.ViewModel
{
    public class FingerPrintViewModel
    {
        SerialPort SerialPort;

        FingerprintView FingerprintView;

        HttpClient _httpClient;
        public FingerPrintViewModel(FingerprintView fpv)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:63001/api/");
            FingerprintView = fpv;
            SerialPort = new SerialPort("COM3", 115200);
            OpenCloseSerialPort();
            SerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            
        }

        public void OpenCloseSerialPort()
        {
            try
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
            catch (Exception)
            {
                MessageBox.Show("Cant Find Module");
                FingerprintView.Close();
            }
        }

        public void SendSerialCommand(string command)
        {
            if (SerialPort.IsOpen)
            {
                SerialPort.Write(command);
            }
        }

        public async void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string data = sp.ReadLine();
            if (data != null && data != "")
            {
                if (data.Contains("ID"))
                {
                    int FingerId = Convert.ToInt32(data.Remove(0, data.IndexOf('#') + 1));
                    HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync(_httpClient.BaseAddress + $"FingerRecognition/{FingerId}");
                    SendSerialCommand("N");
                    var finger = JsonConvert.DeserializeObject<Finger>(httpResponseMessage.Content.ReadAsStringAsync().Result);
                    if (finger != null)
                    {
                        FingerprintView.Dispatcher.Invoke(new Action(() => { FingerprintView.BiometricView.Visibility = Visibility.Visible; }));
                        FingerprintView.Dispatcher.Invoke(new Action(() => { FingerprintView.BiometricView.ShowSuccessView(finger.UserId, finger.Name, finger.Family); }));
                        FingerprintView.Dispatcher.Invoke(new Action(() => { FingerprintView.Close(); }));
                    }
                }
            }
            
        }

        public void ScanFinger()
        {
            SendSerialCommand("S");
        }

    }
}
