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
using System.Windows.Shapes;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System.Windows.Threading;
using Newtonsoft.Json;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.IO;
using Emgu.CV.Face;

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
        public FaceRecognitionView()
        {
            InitializeComponent();
            //_faceCascade = new CascadeClassifier(@"C:\\Users\\Lion\\source\\repos\\BioEntry App\\BioEntry App\\haarcascade_frontalface_default.xml");
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
            // تبدیل تصویر چهره به بایت
            byte[] faceBytes = faceImage.ToJpegData();

            // آماده‌سازی داده‌ها برای ارسال به API
            var data = new
            {
                FaceData = Convert.ToBase64String(faceBytes)
            };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            Capture();
            // ارسال درخواست به API
            using (var client = new HttpClient())
            {



                //var response = await client.PostAsync("https://your-api-url.com/validate-face", content);
                //if (response.IsSuccessStatusCode)
                //{
                //    var result = await response.Content.ReadAsStringAsync();
                //    // پردازش نتیجه احراز هویت
                //}
                //else
                //{
                //    // پردازش خطا
                //}
            }
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

    }
}
