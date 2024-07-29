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
using Newtonsoft.Json.Linq;
using BioEntry_App.Model;
using BioEntry_App.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BioEntry_App.View
{
    /// <summary>
    /// Interaction logic for FaceRecognitionView.xaml
    /// </summary>
    public partial class FaceRecognitionView : Window, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private VideoCapture _capture;
        private CascadeClassifier _faceCascade;
        private bool _captureInProgress;
        private HttpClient _httpClient;

        // property
        public int Attempts { get; set; }
        private string status;

        public string Status
        {
            get { return status; }
            set { status = value; OnPropertyChanged(); }
        }


        public FaceRecognitionView()
        {
            InitializeComponent();
            DataContext = this;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:63001/api/");
            _faceCascade = new CascadeClassifier("haarcascade_frontalface_default.xml");
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
            Attempts = 1;
            Status = $"Attempts: {Attempts}";
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
                    image.Draw(face, new Bgr(0, 255, 0), 1);
                    
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
            Status = $"Attempts: {Attempts}";
            Attempts+=1;
            byte[] faceBytes = faceImage.ToJpegData();

            var data = new
            {
                Img = Convert.ToBase64String(faceBytes)
            };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            // Check Method For Recognition This Settings Temorary After Compelete Admin Page This Setting will change to check automaticlly
            string FRMethod = "WithApp"; // WithApp, WithApi
            if (FRMethod == "WithApi")
            {
                HttpResponseMessage httpResponseMessage = await _httpClient.PostAsync(_httpClient.BaseAddress + "FaceRecognition", content);
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
            } 
            else if (FRMethod == "WithApp")
            {
                HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync(_httpClient.BaseAddress + "FaceRecognition");
                var Response = JsonConvert.DeserializeObject<Dictionary<object, object>>(httpResponseMessage.Content.ReadAsStringAsync().Result);
                Response.TryGetValue("Faces", out var JFaces);
                var Faces = JsonConvert.DeserializeObject<List<Face>>(JFaces.ToString());

                FaceRecognition faceRecognition = new FaceRecognition();

                var Result = faceRecognition.Compare(Faces, data.Img).Result;

                if (Result == null)
                {
                    Capture();
                }
                else
                {
                    var Message = "successfully";
                }

                

            }
            
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

    }
}
