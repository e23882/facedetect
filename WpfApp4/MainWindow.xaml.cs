using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Emgu.CV;
using Emgu.CV.Structure;

namespace WpfApp4
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            imgFace1.Source = new BitmapImage(new Uri(@"D:\3.jpg"));
            imgFace2.Source = new BitmapImage(new Uri(@"D:\2.jpg"));
            ScannerFace();
        }
        List<System.Windows.Controls.Image> list = new List<System.Windows.Controls.Image>();
        int imgCount = 0;
        public void ScannerFace()
        {
            CvInvoke.UseOpenCL = CvInvoke.HaveOpenCLCompatibleGpuDevice;
            var face = new CascadeClassifier(@"D:\haarcascade_frontalface_alt.xml");
            var img = new Image<Bgr, byte>(@"D:\3.jpg");
            var img2 = new Image<Bgr, byte>(@"D:\2.jpg");
            CvInvoke.CvtColor(img, img2, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
            
            CvInvoke.EqualizeHist(img2, img2);
            var facesDetected = face.DetectMultiScale(img2, 1.1, 10, new System.Drawing.Size(50, 50));
            int count = 0;
            var b = img.ToBitmap();
            foreach (var item in facesDetected)
            {
                count++;
                var bmpOut = new Bitmap(item.Width, item.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                var g = Graphics.FromImage(bmpOut);
                g.DrawImage(b, new System.Drawing.Rectangle(0, 0, item.Width, item.Height), new System.Drawing.Rectangle(item.X, item.Y, item.Width, item.Height), GraphicsUnit.Pixel);
                g.Dispose();
                bmpOut.Save($"{count}.png", System.Drawing.Imaging.ImageFormat.Png);
                bmpOut.Dispose();
            }
            //取得檔案
            DirectoryInfo di = new DirectoryInfo(System.Environment.CurrentDirectory);
            string searchPattern = "*";
            list = new List<System.Windows.Controls.Image>();
            imgCount = 0;
            foreach (FileInfo fi in di.GetFiles(searchPattern))
            {
                if (fi.ToString().IndexOf("png") != -1 || fi.ToString().IndexOf("jpg") != -1)
                {
                    System.Windows.Controls.Image newImg = new System.Windows.Controls.Image();
                    newImg.Source = new BitmapImage(new Uri(fi.FullName));
                    list.Add(newImg);
                    newImg.Width = 50;
                    newImg.MouseEnter += NewImg_MouseEnter;
                }
            }

            foreach (var item in list)
            {
                if (imgCount > 18)
                    break;
                spResult.Children.Add(item);
                imgCount++;
            }
        }

        private void NewImg_MouseEnter(object sender, MouseEventArgs e)
        {
            var dt = (sender as System.Windows.Controls.Image).Source;
            if (dt is null)
                return;

            imgShow.Source = new BitmapImage(new Uri(dt.ToString()));
            
        }

        private void imgClick(object sender, RoutedEventArgs e)
        {

        }
        
        private void BtStart_Click(object sender, RoutedEventArgs e)
        {

        }


        int startIndex = 0;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            spResult.Children.Clear();
            var bt = (sender as Button);
            if (bt == null)
                return;
            if (bt.Tag.Equals("left"))
            {
                if(startIndex!=0)
                    startIndex--;
                int runCount = 0;
                foreach (var item in list)
                {
                    if (runCount>startIndex)
                    {
                        spResult.Children.Add(item);
                    }
                    runCount++;
                }
            }
            else if (bt.Tag.Equals("right"))
            {
                startIndex++;
                int runCount = 0;
                foreach (var item in list)
                {
                    if (runCount >= startIndex)
                    {
                        spResult.Children.Add(item);
                    }
                    runCount++;
                }
            }
        }
        
    }
}
