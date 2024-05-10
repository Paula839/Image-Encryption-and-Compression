using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ImageEncryptCompress
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        RGBPixel[,] ImageMatrix;
        RGBPixelD[,] ImageMatrixStrongPassword;
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

                else if(fileExtension == ".bin")
                {
                    ImageMatrix = Compression.load(OpenedFilePath);
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
                 tap = Convert.ToUInt16(tapBox.Text);
            }
            catch {

            }

            RGBPixel[,] Encrypted = Encryption.EncodeString(ImageMatrix, seed, (byte)tap);
            Encrypted = ImageOperations.GaussianFilter1D(Encrypted, maskSize, sigma);
            ImageOperations.DisplayImage(Encrypted, pictureBox2);
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
            RGBPixel[,] Breaking = Encryption.BreakPassword(ImageMatrix, size);
            Breaking = ImageOperations.GaussianFilter1D(Breaking, maskSize, sigma);
            ImageOperations.DisplayImage(Breaking, pictureBox2);
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
            ImageOperations.DisplayImage(ImageMatrix, pictureBox2);
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
    }
}