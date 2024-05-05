using ImageEncryptCompress;
using System;
using System.Collections.Generic;
using System.Text;

public class Encryption
{
    public static StringBuilder current_seedString;
    public static int currentPosition = -1;
    public static int currentTapPosition;
    public static int n;

    public static byte LFSRkString(byte k = 8)
    {
        byte result = 0;
        for (int i = 0; i < k; i++)
        {
            int x = current_seedString[(++currentPosition) % n] ^= current_seedString[(++currentTapPosition) % n];
            result = (byte)((result << 1) | x);

        }

        return result;
    }

    public static RGBPixel[,] EncodeString(RGBPixel[,] image, string initial_seed, byte tap)
    {
        int width = image.GetLength(0);
        int height = image.GetLength(1);

      
        /*BONUS 1 : AlphaNumeric Password
         *get the initial_seed and convert each character to binary and store it in current_seedString(StringBuilder)
         *after this instead of n = initial_seed.Length use n = current_seed.Length
         */
       
        //comment these three lines
        current_seedString = new StringBuilder(initial_seed);
        n = initial_seed.Length; 
        for (int i = 0; i < n; i++) current_seedString[i] -= '0';

        currentPosition = 0 - 1;
        currentTapPosition = n - tap - 1 - 1;

        byte redcolor = 0;
        byte greencolor = 0;
        byte bluecolor = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {

                //RED
                redcolor = LFSRkString();
                greencolor = LFSRkString();
                bluecolor = LFSRkString();

                byte forredByte = redcolor;
                byte forgreenByte = greencolor;
                byte forblueByte = bluecolor;

                image[i, j].red = (byte)(image[i, j].red ^ forredByte);
                image[i, j].green = (byte)(image[i, j].green ^ forgreenByte);
                image[i, j].blue = (byte)(image[i, j].blue ^ forblueByte);
            }
        }
        return image;
    }

    //Bonus2 : Break Password
    //0 1 
    public static RGBPixel[,] BreakPassword(RGBPixel[,] image, int n)
    {

        /*
        * All Combinations of seed using brute force recursion take(1) or leave(0) O(2^n)
        
        * Base Case : use the seed with all tap positions O(n)* and use function encode O(n^2) and calculate all frequencies O(N^2) and
         get the seed and tap position of the highest sum variable that calculates sum+= abs(frequency - 128) O(N) => O(N^3)

        */

        Breaking(n);
        return image;
    }

    public static void Breaking(int n)
    {
        //if 
        //encode
    }







    //Other Solutions
    public static UInt16 current_seedByte = 0;
    public static UInt32 current_seedInt = 0;
    public static UInt64 current_seedLong = 0;
    public static string current_password;
    public static QueueTapPointer queue_seed;

    public static byte LFSRQueue(QueueTapPointer initial_seed) 
    {
        return initial_seed.Xor();
    }

    public static byte LFSKQueue(QueueTapPointer initial_seed, int k)
    {
        byte result = 0;
        for (int i = 0; i < k; i++)
        {
            byte x = LFSRQueue(initial_seed);
            result = (byte)((result << 1) | x);

        }

        return result;
    }


    public static byte LFSRByte(byte tapnum, UInt16 initial_seed, byte n)
    {

        //get last digit of the initial seed 
        byte last = (byte)(((initial_seed & (1 << (n - 1)))) >> (n - 1));

        //get tap digit of the initial seed 
        byte tap = (byte)(((initial_seed & (1 << (tapnum)))) >> (tapnum));

        //shift it
        byte shifted = (byte)(initial_seed << 1);

        //xor last with tap
        byte xorResult = (byte)(last ^ tap);


        byte removelast = (byte)(last << n);

        byte res_seed = (byte)(shifted + xorResult - removelast);

        current_seedByte = res_seed;
        //return res_seed;
        return xorResult;
    }

    public static byte LFSRkByte(byte tapnum, UInt16 initial_seed, byte k, byte n)
    {
        byte result = 0;
        for (int i = 0; i < k; i++)
        {
            byte x = LFSRByte(tapnum, current_seedByte, n);
            result = (byte)((result << 1) | x);

        }

        return result;
    }


    public static byte LFSRInt(byte tapnum, UInt32 initial_seed, byte n)
    {

        //get last digit of the initial seed 
        byte last = (byte)(((initial_seed & (1 << (n - 1)))) >> (n - 1));

        //get tap digit of the initial seed 
        byte tap = (byte)(((initial_seed & (1 << (tapnum)))) >> (tapnum));

        //shift it
        UInt32 shifted = initial_seed << 1;

        //xor last with tap
        byte xorResult = (byte)(last ^ tap);


        UInt32 removelast = (UInt32)(last << n);

        UInt32 res_seed = shifted + xorResult - removelast;

        current_seedInt = res_seed;
        //return res_seed;
        return xorResult;
    }

    public static byte LFSRkInt(byte tapnum, UInt32 initial_seed, byte k, byte n)
    {
        byte result = 0;
        for (int i = 0; i < k; i++)
        {
            int x = LFSRInt(tapnum, current_seedInt, n);
            result = (byte)((result << 1) | x);

        }

        return result;
    }


    public static byte LFSRLong(byte tapnum, UInt64 initial_seed, byte n)
    {

        //get last digit of the initial seed 
        byte last = (byte)(((initial_seed & ((UInt64)(1 << (n - 1))))) >> (n - 1));

        //get tap digit of the initial seed 
        byte tap = (byte)(((initial_seed & ((UInt64)((UInt64)(1 << (tapnum)))))) >> (tapnum));

        //shift it
        UInt64 shifted = (initial_seed << 1);

        //xor last with tap
        byte xorResult = (byte)(last ^ tap);


        UInt64 removelast = (UInt64)(last << n);
        UInt64 res_seed = shifted + xorResult - removelast;

        current_seedLong = res_seed;
        //return res_seed;
        return xorResult;
    }

    public static byte LFSRkLong(byte tapnum, UInt64 initial_seed, byte k, byte n)
    {
        byte result = 0;
        for (int i = 0; i < k; i++)
        {
            int x = LFSRLong(tapnum, current_seedLong, n);
            result = (byte)((result << 1) | x);

        }

        return result;
    }

    public static RGBPixel[,] Encode(RGBPixel[,] image, string initial_seed, byte tap)
    {
        int width = image.GetLength(0);
        int height = image.GetLength(1);

        //string initial_se = "01101000010";
        byte n = (byte)initial_seed.Length;
        //Console.WriteLine(n);
        if (n < 16)
        {
            current_seedByte = Convert.ToUInt16(initial_seed, 2);
        }

        else if (n < 32)
        {
            current_seedInt = Convert.ToUInt32(initial_seed, 2);

        }

        else if(n < 64)
        {
            current_seedLong = Convert.ToUInt64(initial_seed, 2);
        }

        else
        {
            queue_seed = new QueueTapPointer(n - tap - 1);
            for (int i = 0; i < n; i++)
            {
                queue_seed.Enqueue(initial_seed[i] - '0');
            }

        }
        
        //Console.WriteLine(seedInt);
        byte redcolor = 0;
        byte greencolor = 0;
        byte bluecolor = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {

                //RED

                if(n < 16)
                {
                    redcolor = LFSRkByte(tap, current_seedByte, 8, n);
                    greencolor = LFSRkByte(tap, current_seedByte, 8, n);
                    bluecolor = LFSRkByte(tap, current_seedByte, 8, n);
                }

                else if (n < 32)
                {
                     redcolor = LFSRkInt(tap, current_seedInt, 8, n);
                     greencolor = LFSRkInt(tap, current_seedInt, 8, n);
                     bluecolor = LFSRkInt(tap, current_seedInt, 8, n);
                }

                else if(n < 64)
                {
                    redcolor = LFSRkLong(tap, current_seedLong, 8, n);
                    greencolor = LFSRkLong(tap, current_seedLong, 8, n);
                    bluecolor = LFSRkLong(tap, current_seedLong, 8, n);
                }

                else
                {
                    redcolor = LFSKQueue(queue_seed, 8);
                    greencolor = LFSKQueue(queue_seed, 8);
                    bluecolor = LFSKQueue(queue_seed, 8);
                }
                //int forred = Convert.ToInt32(redcolor, 2);
                //GREEN
                //int forgreen = Convert.ToInt32(greencolor, 2);

                //BLUE
                //int forblue = Convert.ToInt32(bluecolor, 2);
                byte forredByte = redcolor;
                byte forgreenByte = greencolor;
                byte forblueByte = bluecolor;

                image[i, j].red = (byte)(image[i, j].red ^ forredByte);
                image[i, j].green = (byte)(image[i, j].green ^ forgreenByte);
                image[i, j].blue = (byte)(image[i, j].blue ^ forblueByte);
            }
        }
        return image;
    }




}