using Microsoft.VisualBasic;
using SharpAvi;
using SharpAvi.Codecs;
using SharpAvi.Output;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Image = System.Drawing.Image;


namespace Animation
{
    public class Recorder : IDisposable
    {
        /// <summary>
        ///  Название файла ( Путь его хранения)
        /// </summary>
        private string filename;
        /// <summary>
        /// Количество кадров в секунду и качество записи
        /// </summary>
        private int framePerSecond,
                    quality;
        /// <summary>
        /// Кодек  для кодирования
        /// </summary>
        private FourCC codec;
        /// <summary>
        /// Ширина и Высота экрана записи
        /// </summary>
        private int height,
                    width;
        /// <summary>
        /// Запись в avi файл
        /// </summary>
        private AviWriter writer;
        /// <summary>
        /// Приостановление потока записи
        /// </summary>
        private ManualResetEvent stopThread = new ManualResetEvent(false);
        /// <summary>
        /// 
        /// </summary>
        private IAviVideoStream videoStream;
        /// <summary>
        /// Поток записи экрана
        /// </summary>
        private Thread screenThread;
        /// <summary>
        /// Конструктор с параметрами
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="Codec"></param>
        /// <param name="FramePerSecond"></param>
        /// <param name="Quality"></param>
        public Recorder(string FileName, FourCC Codec, int FramePerSecond, int Quality)
        {
            filename = FileName;
            codec = Codec;
            framePerSecond = FramePerSecond;
            quality = Quality;
            width = (int)Math.Round(SystemParameters.PrimaryScreenWidth);
            height = (int)Math.Round(SystemParameters.PrimaryScreenHeight);
        }

        public Recorder()
        {

        }
        public Recorder(string FileName)
        {
            filename = FileName;
        }

        /// <summary>
        /// Метод создающий avi поток записи
        /// </summary>
        /// <returns></returns>
        private AviWriter CreateAviWriter()
        {
            return new AviWriter(filename)
            {
                FramesPerSecond = this.framePerSecond,
                EmitIndex1 = true
            };
        }
        /// <summary>
        ///  Сохранение кодировки видео в поток записи avi
        /// Uncompressed -не делает никакого реального кодирования, просто переворачивает изображение по вертикали и преобразует данные BGR32 в данные BGR24 для уменьшения размера.
        /// MotionJpeg - использует для кодирования System.Windows.Media.Imaging.JpegBitmapEncoder
        /// MPEG4 -  выполняет кодирование MPEG-4, используя Video for Windows (VfW) или Video Compression Manager (VCM) совместимый кодек, установленный в системе.
        /// </summary>
        /// <param name="writer"></param>
        /// <returns></returns>
        private IAviVideoStream СreateVideoStream(AviWriter writer)
        {
            // 
            if (codec == KnownFourCCs.Codecs.Uncompressed)
                return writer.AddUncompressedVideoStream(width, height);
            else if (codec == KnownFourCCs.Codecs.MotionJpeg)
                return writer.AddMotionJpegVideoStream(width, height, quality);
            else
            {
                return writer.AddMpeg4VideoStream(width, height, framePerSecond,
                    quality: quality,
                    codec: codec,
                    forceSingleThreadedAccess: true);
            }
        }

        #region Запись экрана
        public void ScreenRecording()
        {
            writer = CreateAviWriter();
            videoStream = СreateVideoStream(writer);
            videoStream.Name = filename;
            screenThread = new Thread(RecordScreen)
            {
                Name = typeof(Recorder).Name + ".RecordScreen",
                IsBackground = true
            };
            screenThread.Start();
        }

        private void RecordScreen()
        {
            var frameInterval = TimeSpan.FromSeconds((1 / (double)writer.FramesPerSecond));
            var buffer = new byte[width * height * 4];
            Task videoWriteTask = null;
            var timeTillnNextFrame = TimeSpan.Zero;
            while (!stopThread.WaitOne(timeTillnNextFrame))
            {
                var timeStamp = DateAndTime.Now;
                Screenshot(buffer);
                videoWriteTask?.Wait();
                videoWriteTask = videoStream.WriteFrameAsync(true, buffer, 0, buffer.Length);
                timeTillnNextFrame = timeStamp + frameInterval - DateAndTime.Now;
                if (timeTillnNextFrame < TimeSpan.Zero)
                    timeTillnNextFrame = TimeSpan.Zero;
            }

            videoWriteTask?.Wait();

        }
        private void Screenshot(byte[] buffer)
        {
            using (var bmp = new Bitmap(width, height))
            {
                using (var graphics = Graphics.FromImage(bmp))
                {

                    graphics.CopyFromScreen(System.Drawing.Point.Empty,
                        System.Drawing.Point.Empty,
                        new System.Drawing.Size(width, height),
                        CopyPixelOperation.SourceCopy);
                    graphics.Flush();
                    var bits = bmp.LockBits(new Rectangle(0, 0, width, height),
                        ImageLockMode.ReadOnly,
                        PixelFormat.Format32bppArgb);
                    Marshal.Copy(bits.Scan0, buffer, 0, buffer.Length);
                    bmp.UnlockBits(bits);
                }
            }
        }

        public void Dispose()
        {
            stopThread.Set();
            screenThread.Join();
            writer.Close();
            stopThread.Dispose();
        }
        #endregion


        #region Сохранение изображений в avi файл

        public void ImageToAvi(ObservableCollection<string> PathImage)
        {
            int width = 1024;
            int height = 720;

            var writer = new AviWriter(filename)
            {
                FramesPerSecond = 1,
                EmitIndex1 = true
            };

            var stream = writer.AddVideoStream();
            stream.Width = width;
            stream.Height = height;
            stream.Codec = KnownFourCCs.Codecs.Uncompressed;
            stream.BitsPerPixel = BitsPerPixel.Bpp32;

            foreach (var frame in PathImage)
            {
                var image = Image.FromFile(frame);
                byte[] arr = imageToByteArray(image);
                var bm = ToBitmap(arr);
                var rbm = ReduceBitmap(bm, width, height);

                byte[] fr = BitmapToByteArray(rbm);

                stream.WriteFrame(true, fr, 0, fr.Length);
            }

            writer.Close();
        }
        private static byte[] imageToByteArray(System.Drawing.Image image)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return ms.ToArray();
            }
        }
        private Bitmap ToBitmap(byte[] byteArrayIn)
        {
            var ms = new MemoryStream(byteArrayIn);
            var returnImage = Image.FromStream(ms);
            var bitmap = new Bitmap(returnImage);

            return bitmap;
        }

        private Bitmap ReduceBitmap(Bitmap original, int reducedWidth, int reducedHeight)
        {
            var reduced = new Bitmap(reducedWidth, reducedHeight);
            using (var graphics = Graphics.FromImage(reduced))
            {
                graphics.TranslateTransform(0, reduced.Height);
                graphics.ScaleTransform(1, -1);
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(original, new Rectangle(0, 0, reducedWidth, reducedHeight), new Rectangle(0, 0, original.Width, original.Height), GraphicsUnit.Pixel);
            }
            return reduced;
        }

        private static byte[] BitmapToByteArray(Bitmap bitmap)
        {
            BitmapData bmpdata = null;

            try
            {
                bmpdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                int numbytes = bmpdata.Stride * bitmap.Height;
                byte[] bytedata = new byte[numbytes];
                IntPtr ptr = bmpdata.Scan0;

                Marshal.Copy(ptr, bytedata, 0, numbytes);

                return bytedata;
            }
            finally
            {
                if (bmpdata != null)
                {
                    bitmap.UnlockBits(bmpdata);
                }
            }
        }
        #endregion
    }
}
