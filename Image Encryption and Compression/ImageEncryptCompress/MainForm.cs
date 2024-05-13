using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ImageEncryptCompress
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        RGBPixel[,] ImageMatrix;
        RGBPixel[,] SmoothedImageMatrix;

        string filePath;
        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                string fileExtension = Path.GetExtension(OpenedFilePath).ToLower();
                if (fileExtension == ".bmp")
                {
                    ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                }
                
                else
                {

                }

                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
            }

            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
        }

        private void btnGaussSmooth_Click(object sender, EventArgs e)
        {

            double sigma = double.Parse(txtGaussSigma.Text);
            int maskSize = (int)nudMaskSize.Value;
            string seed = "";
            int tap = 0;
            try
            {
                 seed = seedBox.Text;
                 tap = Convert.ToInt32(tapBox.Text);
            }
            catch {

            }

            SmoothedImageMatrix = Encryption.EncodeString(ImageMatrix, seed, tap);
            ImageOperations.DisplayImage(ImageOperations.GaussianFilter1D(SmoothedImageMatrix, maskSize, sigma), pictureBox2);
        }

        private void txtWidth_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            double sigma = double.Parse(txtGaussSigma.Text);
            int maskSize = (int)nudMaskSize.Value;
            int size = 0;
            try
            {
                size = Convert.ToInt32(sizeBox.Text);
            }
            catch { 
                
            }
            SmoothedImageMatrix = Encryption.BreakPassword(ImageMatrix, size);
            SmoothedImageMatrix = ImageOperations.GaussianFilter1D(SmoothedImageMatrix, maskSize, sigma);
            ImageOperations.DisplayImage(SmoothedImageMatrix, pictureBox2);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            double sigma = double.Parse(txtGaussSigma.Text);
            int maskSize = (int)nudMaskSize.Value;
            Compression.save(ImageMatrix);
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            double sigma = double.Parse(txtGaussSigma.Text);
            int maskSize = (int)nudMaskSize.Value;

            Compression.save(ImageMatrix);
            ImageOperations.DisplayImage(ImageMatrix, pictureBox2);
        }

        private void button3_Click(object sender, EventArgs e)
        {

            Compression.load(filePath);
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            double sigma = double.Parse(txtGaussSigma.Text);
            int maskSize = (int)nudMaskSize.Value;
            string seed = "";
            int tap = 0;
            try
            {
                seed = seedBox.Text;
                tap = Convert.ToInt32(tapBox.Text);
            }
            catch
            {

            }

            SmoothedImageMatrix = Encryption.EncodeString(ImageMatrix, seed, tap);
            ImageOperations.DisplayImage(ImageOperations.GaussianFilter1D(SmoothedImageMatrix, maskSize, sigma), pictureBox2);
            Compression.save(SmoothedImageMatrix);
            stopwatch.Stop();

            Console.WriteLine("Encryption and Compression Time");
            Console.WriteLine($"Total elapsed time in seconds {stopwatch.Elapsed.TotalSeconds:F0}s");
            TimeSpan totalTime = stopwatch.Elapsed;
            string formattedTotalTime = string.Format("{0:00}:{1:00}", totalTime.Minutes, totalTime.Seconds);
            Console.WriteLine($"Total elapsed time: {formattedTotalTime}");

            string filePath = "D:\\Algorithm\\Project\\RELEASE\\[1] Image Encryption and Compression\\comp.bin";
            FileInfo fileInfo = new FileInfo(filePath);

            long fileSizeInBytes = fileInfo.Length / 1024;
            Console.WriteLine($"File size: {fileSizeInBytes} KB");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            double sigma = double.Parse(txtGaussSigma.Text);
            int maskSize = (int)nudMaskSize.Value;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            Stopwatch stopwatch = new Stopwatch();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                string fileExtension = Path.GetExtension(OpenedFilePath).ToLower();

                if (fileExtension == ".bin")
                {
                    stopwatch.Start();
                    ImageMatrix = Compression.load(OpenedFilePath);
                }
                else
                {
                }

                string seed = "";
                int tap = 0;
                try
                {
                    seed = seedBox.Text;
                    tap = Convert.ToUInt16(tapBox.Text);
                }
                catch
                {

                }

                SmoothedImageMatrix = Encryption.EncodeString(ImageMatrix, seed, (byte)tap);

                ImageOperations.DisplayImage(ImageOperations.GaussianFilter1D(ImageMatrix, maskSize, sigma), pictureBox1);
                ImageOperations.DisplayImage(ImageOperations.GaussianFilter1D(SmoothedImageMatrix, maskSize, sigma), pictureBox2);

            }

            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
            stopwatch.Stop();

            Console.WriteLine("Decompression and Decryption Time");
            Console.WriteLine($"Total elapsed time in seconds {stopwatch.Elapsed.TotalSeconds:F0}s");
            TimeSpan totalTime = stopwatch.Elapsed;
            string formattedTotalTime = string.Format("{0:00}:{1:00}", totalTime.Minutes, totalTime.Seconds);
            Console.WriteLine($"Total elapsed time: {formattedTotalTime}");
        }

        private void button6_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string OpenedFilePath = openFileDialog1.FileName;

                SmoothedImageMatrix = Compression.load(OpenedFilePath);
            }

            ImageOperations.DisplayImage(SmoothedImageMatrix, pictureBox2);

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            double sigma = double.Parse(txtGaussSigma.Text);
            int maskSize = (int)nudMaskSize.Value;
            string seed = "";
            int tap = 0;
            try
            {
                seed = seedBox.Text;
                tap = Convert.ToInt32(tapBox.Text);
            }
            catch
            {

            }

            SmoothedImageMatrix = Encryption.EncodeString(SmoothedImageMatrix, seed, tap);
            ImageOperations.DisplayImage(ImageOperations.GaussianFilter1D(SmoothedImageMatrix, maskSize, sigma), pictureBox2);
        }
    }
}