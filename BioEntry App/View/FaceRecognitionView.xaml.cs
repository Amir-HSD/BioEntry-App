using System;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Windows.Threading;
using Newtonsoft.Json;
using System.Net.Http;
using Emgu.CV.Face;
using System.Net;
using System.Collections.Generic;

namespace BioEntry_App.View
{
    /// <summary>
    /// Interaction logic for FaceRecognitionView.xaml
    /// </summary>
    public partial class FaceRecognitionView : Window
    {
        private VideoCapture _capture;
        private CascadeClassifier _faceCascade;
        private bool _captureInProgress;
        //private readonly LBPHFaceRecognizer _faceRecognizer;
        private HttpClient _httpClient;
        public FaceRecognitionView()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            
            //_faceCascade = new CascadeClassifier(@"C:\\Users\\Lion\\source\\repos\\BioEntry App\\BioEntry App\\haarcascade_frontalface_default.xml");
            _faceCascade = new CascadeClassifier("haarcascade_frontalface_default.xml");
            //_faceRecognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 100);
        }

        private void Capture()
        {
            if (_captureInProgress)
            {
                _capture.Pause();
                _captureInProgress = false;
            }
            else
            {
                if (_capture == null)
                {
                    _capture = new VideoCapture();
                    _capture.ImageGrabbed += ProcessFrame;
                }
                _capture.Start();
                _captureInProgress = true;
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            _capture.Pause();
            _captureInProgress = false;
            this.Close();
        }

        private void FaceRecognitionWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Capture();
        }
        private void ProcessFrame(object sender, EventArgs arg)
        {
            Mat frame = new Mat();
            _capture.Retrieve(frame, 0);
            DetectAndDisplay(frame);
        }
        private void DetectAndDisplay(Mat frame)
        {
            using (var image = frame.ToImage<Bgr, Byte>())
            {
                var gray = image.Convert<Gray, Byte>();
                var faces = _faceCascade.DetectMultiScale(gray, 1.1, 10, System.Drawing.Size.Empty);

                foreach (var face in faces)
                {
                    image.Draw(face, new Bgr(0, 255, 0), 2);
                    
                    SendFaceToAPI(gray.Copy(face));
                }

                Dispatcher.Invoke(() =>
                {
                    webcamImage.Source = ToBitmapSource(image);
                });
            }
        }
        private BitmapSource ToBitmapSource(Image<Bgr, byte> image)
        {
            using (var bitmap = image.ToBitmap())
            {
                IntPtr ptr = bitmap.GetHbitmap();
                BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(ptr);
                return bitmapSource;
            }
        }
        private async void SendFaceToAPI(Image<Gray, byte> faceImage)
        {
            Capture();
            byte[] faceBytes = faceImage.ToJpegData();

            var data = new
            {
                Img = Convert.ToBase64String(faceBytes)
            };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            // Check Method For Recognition This Settings Temorary After Compelete Admin Page This Setting will change to check automaticlly
            string FRMethod = "WithApi"; // WithApp
            if (FRMethod == "WithApi")
            {
                _httpClient.BaseAddress = new Uri("http://localhost:63001/api/FaceRecognition/");
                HttpResponseMessage httpResponseMessage = await _httpClient.PostAsync(_httpClient.BaseAddress, content);
                if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var Response = JsonConvert.DeserializeObject<Dictionary<object, object>>(httpResponseMessage.Content.ReadAsStringAsync().Result);
                    Response.TryGetValue("status", out var status);
                    if (status != null)
                    {
                        if (status.ToString().Contains("successfully"))
                        {

                        }
                        else
                        {
                            Capture();
                        }
                    }
                }
                else
                {
                    Capture();
                }
            } else if (FRMethod == "WithApp")
            {
                _httpClient.BaseAddress = new Uri("http://localhost:63001/api/FaceRecognition/");
                HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync(_httpClient.BaseAddress);
                var Response = JsonConvert.DeserializeObject<Dictionary<object, object>>(httpResponseMessage.Content.ReadAsStringAsync().Result);
            }
            
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

    }
}
