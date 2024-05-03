using ImageEncryptCompress;
using System;
using System.Collections.Generic;
using System.Text;

public class Encryption
{
    public static int current_seed;
    public static string current_password;
    public static int LFSR(int tapnum, int initial_seed, int n)
    {

        //get last digit of the initial seed 
        int last = ((initial_seed & (1 << (n - 1)))) >> (n - 1);
        
        //get tap digit of the initial seed 
        int tap = ((initial_seed & (1 << (tapnum)))) >> (tapnum);

        //shift it
        int shifted = initial_seed << 1; 

        //xor last with tap
        int xorResult = last ^ tap;

        
        int removelast = last << n;

        int res_seed = shifted + xorResult - removelast;

        current_seed = res_seed;
        //return res_seed;
        return xorResult;
    }

    public static int LFSRk(int tapnum, int initial_seed, int k, int n)
    {
        int result = 0;
        for (int i = 0; i < k; i++)
        {
            int x = LFSR(tapnum, current_seed, n);
            result = (result << 1) | x;

        }

        return result;
    }

    public static RGBPixel[,] Encode(RGBPixel[,] image, string initial_seed, int tap)
    {
        int width = image.GetLength(0);
        int height = image.GetLength(1);

        //string initial_se = "01101000010";
        int n = initial_seed.Length;
        //Console.WriteLine(n);
        int seedInt = Convert.ToInt32(initial_seed, 2);
        //Console.WriteLine(seedInt);
        current_seed = seedInt;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {

                //RED
                int redcolor = LFSRk(tap, current_seed, 8, n);
                //int forred = Convert.ToInt32(redcolor, 2);
                byte forredByte = (byte)redcolor;
                //GREEN
                int greencolor = LFSRk(tap, current_seed, 8, n);
                //int forgreen = Convert.ToInt32(greencolor, 2);
                byte forgreenByte = (byte)greencolor;

                //BLUE
                int bluecolor = LFSRk(tap, current_seed, 8, n);
                //int forblue = Convert.ToInt32(bluecolor, 2);
                byte forblueByte = (byte)bluecolor;

                image[i, j].red = (byte)(image[i, j].red ^ forredByte);
                image[i, j].green = (byte)(image[i, j].green ^ forgreenByte);
                image[i, j].blue = (byte)(image[i, j].blue ^ forblueByte);
            }
        }
        return image;
    }


    //Bonus1
    public static string StrongPassword(int tapnum, string initial_password, int n)
    {

        //get last digit of the initial seed 
        return "0";
    }

    public static string StrongPasswordK(int tapnum, string initial_password, int k, int n)
    {
        //string result = 0;
        for (int i = 0; i < k; i++)
        {
            string x = StrongPassword(tapnum, current_password, n);
            //result = (result << 1) | x;

        }


        return "0";
    }

    public static RGBPixelD[,] EncodeStrongPassword(RGBPixelD[,] image, string initial_password, int tap)
    {


        return image;
    }


    //Bonus2
    //0 1 
    public static RGBPixel[,] BreakPassword(RGBPixel[,] image, int n)
    {

        Breaking(n);
        return image;
    }

    public static void Breaking(int n)
    {
        //if 
        //encode
    }

}