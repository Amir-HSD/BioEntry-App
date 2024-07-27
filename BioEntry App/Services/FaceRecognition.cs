﻿using Emgu.CV;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BioEntry_App.Model;
using Emgu.CV.Linemod;
using System.Drawing.Imaging;
using System.IO;

namespace BioEntry_App.Services
{
    public class FaceRecognition
    {
        LBPHFaceRecognizer _faceRecognizer;
        bool Detected = false;
        public FaceRecognition()
        {
            _faceRecognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 100);
        }

        public async Task<Face> Compare(List<Face> Faces, string Img)
        {
            try
            {
                if (!File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "FaceComparator")))
                {
                    Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "FaceComparator"));
                }

                byte[] Face1byte = Convert.FromBase64String(Img);
                string Face1FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "FaceComparator") + Guid.NewGuid().ToString() + ".jpg";
                string Face2FileName;

                try
                {
                    Bitmap bitmap1 = new Bitmap(new MemoryStream(Face1byte));
                    bitmap1.Save(Face1FileName, ImageFormat.Jpeg);

                    var ImageFace1 = new Image<Gray, byte>(Face1FileName);

                    foreach (var Face in Faces)
                    {
                        byte[] Face2byte = Convert.FromBase64String(Face.Img);
                        Face2FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "FaceComparator") + Guid.NewGuid().ToString();

                        Bitmap bitmap2 = new Bitmap(new MemoryStream(Face2byte));
                        bitmap2.Save(Face2FileName, ImageFormat.Jpeg);

                        var ImageFace2 = new Image<Gray, byte>(Face2FileName);


                        VectorOfMat vectorOfMat = new VectorOfMat();
                        VectorOfInt vectorOfInt = new VectorOfInt();

                        vectorOfMat.Push(ImageFace1);
                        vectorOfInt.Push(new[] { 1 });

                        _faceRecognizer.Train(vectorOfMat, vectorOfInt);
                        var result = _faceRecognizer.Predict(ImageFace2);

                        if (result.Label == 1 && result.Distance < 50)
                        {
                            File.Delete(Face2FileName);
                            Detected = true;
                            return new Face { Id = Face.Id, Img = Face.Img };
                        }
                        else
                        {
                            File.Delete(Face2FileName);
                        }

                    }
                    return null;

                }
                catch (Exception e)
                {
                    return null;
                }
                finally
                {
                    File.Delete(Face1FileName);
                    _faceRecognizer.Dispose();
                    
                }

            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {
                
            }
        }

    }
}
