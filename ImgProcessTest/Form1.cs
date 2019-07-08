using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImgProcessTest
{
    public partial class Form1 : Form
    {
        Bitmap image1 = null;
        Bitmap image2 = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                image1 = new Bitmap(openDialog.FileName);
                pictureBox1.Image = image1;
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {

            OpenFileDialog openDialog = new OpenFileDialog();
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                image2 = new Bitmap(openDialog.FileName);
                pictureBox2.Image = image2;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (compareUnsafe(image1, image2))
            {
                MessageBox.Show("Bingo.");
            }

            else
            {
                MessageBox.Show("Alakası yok.");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (compareClassic(image1, image2))
            {
                MessageBox.Show("Bingo.");
            }

            else
            {
                MessageBox.Show("Alakası yok.");
            }
        }

        private bool compareUnsafe(Bitmap bmp1, Bitmap bmp2)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            
            bool equals = true;
            Rectangle rect = new Rectangle(0, 0, bmp1.Width, bmp1.Height);
            BitmapData bmpData1 = bmp1.LockBits(rect, ImageLockMode.ReadOnly, bmp1.PixelFormat);
            BitmapData bmpData2 = bmp2.LockBits(rect, ImageLockMode.ReadOnly, bmp2.PixelFormat);
            unsafe
            {
                byte* ptr1 = (byte*)bmpData1.Scan0.ToPointer();
                byte* ptr2 = (byte*)bmpData2.Scan0.ToPointer();
                int width = rect.Width * 3; // 24bpp pixel data için
                for (int y = 0; equals && y < rect.Height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (*ptr1 != *ptr2)
                        {
                            equals = false;
                            break;
                        }
                        ptr1++;
                        ptr2++;
                    }
                    ptr1 += bmpData1.Stride - width;
                    ptr2 += bmpData2.Stride - width;
                }
            }
            bmp1.UnlockBits(bmpData1);
            bmp2.UnlockBits(bmpData2);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            label1.Text = "Süre : " + elapsedMs + " Milisaniye";

            return equals;
        }

        private bool compareClassic(Bitmap bmp1, Bitmap bmp2)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            bool equals = true;
            bool flag = true;  

            if (bmp1.Size == bmp2.Size)
            {
                for (int x = 0; x < bmp1.Width; ++x)
                {
                    for (int y = 0; y < bmp1.Height; ++y)
                    {
                        if (bmp1.GetPixel(x, y) != bmp2.GetPixel(x, y))
                        {
                            equals = false;
                            flag = false;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        break;
                    }
                }
            }
            else
            {
                equals = false;
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            label2.Text = "Süre : " + elapsedMs + " Milisaniye";

            return equals;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            float diff = 0;

            for (int y = 0; y < image1.Height; y++)
            {
                for (int x = 0; x < image1.Width; x++)
                {
                    diff += (float)Math.Abs(image1.GetPixel(x, y).R - image2.GetPixel(x, y).R) / 255;
                    diff += (float)Math.Abs(image1.GetPixel(x, y).G - image2.GetPixel(x, y).G) / 255;
                    diff += (float)Math.Abs(image1.GetPixel(x, y).B - image2.GetPixel(x, y).B) / 255;
                }
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            label3.Text = "Süre : " + elapsedMs + " Milisaniye";
            MessageBox.Show(string.Format("Fark: {0} %", 100 * diff / (image1.Width * image1.Height * 3)));
        }
    }
}
