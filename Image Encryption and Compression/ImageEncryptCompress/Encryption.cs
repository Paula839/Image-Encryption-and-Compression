using ImageEncryptCompress;
using System;
using System.Collections.Generic;
using System.Text;

public class Encryption
{
    public static string h = "";
    public static int hh;
    public Encryption(string init_seed)
    {
        h = init_seed;
    }
    public static int LFSR(int tapnum, int initial_seed, int n)
    {
        int last = ((initial_seed & (1 << (n - 1)))) >> (n - 1);
        //Console.WriteLine(last);

        int tap = ((initial_seed & (1 << (tapnum)))) >> (tapnum);
        //Console.WriteLine(tap);

        int shifted = initial_seed << 1;

        int xorResult = last ^ tap;
        // Console.WriteLine(xorResult);

        int removelast = last * (1 << n);

        int res_seed = shifted + xorResult - removelast;

        //int res_seed = shifted | xorResult;

        //string str_seed = Convert.ToString(res_seed, 2).PadLeft(n, '0');

        //Console.WriteLine(res_seed);
        hh = res_seed;
        //return res_seed;
        return xorResult;
    }

    public static int LFSRk(int tapnum, int initial_seed, int k, int n)
    {
        //int n = initial_seed.Length;
        //Console.WriteLine(n);
        //int seedInt = Convert.ToInt32(initial_seed, 2);
        //Console.WriteLine(seedInt);
        //hh = seedInt;
        //string retpass = "";
        //string strseed = "";
        int result = 0;
        for (int i = 0; i < k; i++)
        {
            //Console.WriteLine(seedInt);
            int x = LFSR(tapnum, hh, n);
            //Console.WriteLine(x);
            result = (result << 1) | x;
            // int newseed = LFSR(tapnum, seedInt, n);
            // //ret += xorResult;
            // seedInt = newseed;
            // //Console.WriteLine(newseed);
            // strseed = Convert.ToString(newseed, 2).PadLeft(n, '0');
            // //Console.WriteLine(strseed);
            // retpass += strseed[n - 1];

        }

        // h = strseed;
        // // Console.WriteLine(retpass);
        // return retpass;
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
        hh = seedInt;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {



                //RED
                int redcolor = LFSRk(tap, hh, 8, n);
                //int forred = Convert.ToInt32(redcolor, 2);
                byte forredByte = (byte)redcolor;

                //GREEN
                int greencolor = LFSRk(tap, hh, 8, n);
                //int forgreen = Convert.ToInt32(greencolor, 2);
                byte forgreenByte = (byte)greencolor;

                //BLUE
                int bluecolor = LFSRk(tap, hh, 8, n);
                //int forblue = Convert.ToInt32(bluecolor, 2);
                byte forblueByte = (byte)bluecolor;


                //Console.WriteLine(forred);
                //Console.WriteLine(forgreen);
                //Console.WriteLine(forblue);


                image[i, j].red = (byte)(image[i, j].red ^ forredByte);
                image[i, j].green = (byte)(image[i, j].green ^ forgreenByte);
                image[i, j].blue = (byte)(image[i, j].blue ^ forblueByte);
            }
        }
        return image;
    }


    //Bonus1
    public static void StrongPassword()
    {

    }

    //Bonus2
    public static void BreakPassword()
    {

    }


}