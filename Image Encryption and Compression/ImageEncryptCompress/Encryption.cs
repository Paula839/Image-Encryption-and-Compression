using ImageEncryptCompress;
using System;
using System.Collections.Generic;
using System.Text;

public class Encryption
{
    public static string initialaSeed;
    public static int tapPosition;
    public static StringBuilder current_seed;
    public static int currentPosition;
    public static int currentTapPosition;
    public static int n;
  

    public static byte LFSRK(byte tap = 8)
    {

        byte result = 0;

        for (int i = 0; i < tap; i++)
        {

            int x = current_seed[(++currentPosition) % n] ^= 
                current_seed[(++currentTapPosition) % n];
            result = (byte)((result << 1) | x); //result * 2 + x

        }

        return result;
    }

    public static string ConvertToBinary(int number)
    {
        if (number == 0)
            return "0"; 

        string binary = ""; 

        while (number > 0)
        {
     
            int bit = number % 2;

            binary = bit + binary;

            number >>= 1;
        }
        return binary;
    }

    public static RGBPixel[,] Encrypt(RGBPixel[,] image, string initial_seed, int tap)
    {
        int width = image.GetLength(0);
        int height = image.GetLength(1);
        tapPosition = tap;

        current_seed = new StringBuilder();

        /*BONUS 1 : AlphaNumeric Password
         *get the initial_seed and convert each character to binary and store it in current_seedString(StringBuilder)
         *after this instead of n = initial_seed.Length use n = current_seed.Length
         */

        int seed_len = initial_seed.Length;
        Dictionary<char, string> alpha_password = new Dictionary<char, string>();
        for (int i = 0; i < seed_len; i++)
        {
            int current_char = 0;
            if (initial_seed[i] >= 'a' && initial_seed[i] <= 'z') 
            {
                current_char = initial_seed[i] - 'a' + 36;
                alpha_password[initial_seed[i]] = ConvertToBinary(current_char);
                current_seed.Append(alpha_password[initial_seed[i]]);
            }

            else if (initial_seed[i] >= 'A' && initial_seed[i] <= 'Z')
            {
                current_char = initial_seed[i] - 'A' + 10;
                alpha_password[initial_seed[i]] = ConvertToBinary(current_char);
                current_seed.Append(alpha_password[initial_seed[i]]);

            }
            else
            {
                current_char = initial_seed[i] - '0';
                alpha_password[initial_seed[i]] = ConvertToBinary(current_char);
                current_seed.Append(alpha_password[initial_seed[i]]);
            }
        }

        initialaSeed = current_seed.ToString();
        n = current_seed.Length;
        for (int i = 0; i < n; i++) current_seed[i] -= '0';
        currentPosition = 0 - 1; //p1
        currentTapPosition = n - tap - 1 - 1; // p2
        RGBPixel[,] pixlaya = (RGBPixel[,])image.Clone(); 
        byte redcolor = 0;
        byte greencolor = 0;
        byte bluecolor = 0;
        for (int i = 0; i < width; i++) //n
        {
            for (int j = 0; j < height; j++) //m
            {

                redcolor = LFSRK();
                greencolor = LFSRK();
                bluecolor = LFSRK();

                pixlaya[i, j].red = (byte)(pixlaya[i, j].red ^ redcolor);
                pixlaya[i, j].green = (byte)(pixlaya[i, j].green ^ greencolor);
                pixlaya[i, j].blue = (byte)(pixlaya[i, j].blue ^ bluecolor);
            }
        }
        return pixlaya;
    }

    //Bonus2 : Break Password
    //0 1 
    static StringBuilder randomSeed;
    static RGBPixel[,] testingImage;
    static int num;

    static double mostDeviated;
    static string bestSeed;
    static byte bestTap;
    static RGBPixel[,] bestImage;
    static int elsumElly3ayzo;
    static bool found;

    public static RGBPixel[,] BreakPassword(RGBPixel[,] image, int nn)
    {

        /*
        * All Combinations of seed using brute force recursion take(1) or leave(0) O(2^n)
        
        * Base Case : use the seed with all tap positions O(n)* and use function encode O(n^2) and calculate all frequencies O(N^2) and
         get the seed and tap position of the highest sum variable that calculates sum+= abs(frequency - 128) O(N) => O(N^3)

        */
        randomSeed = new StringBuilder();
        testingImage = image;
        num = nn;
        mostDeviated = 0;
        found = false;
        Breaking(image);
        return bestImage;
    }

    public static void Breaking(RGBPixel[,] image, int idx = 0)
    {
        if (idx == num)
        {
            for (byte tap = 0; tap < num; tap++)
            {
                testingImage = (RGBPixel[,])image.Clone();
                RGBPixel[,] elImage = Encrypt(testingImage, randomSeed.ToString(), tap);
                double deviation = CalculateImageDeviation(elImage);
                if (deviation > mostDeviated)
                {
                    mostDeviated = deviation;
                    bestSeed = randomSeed.ToString();
                    bestTap = tap;
                    bestImage = elImage;
                }
            }
            return;
        }
        randomSeed.Append('0'); //leave 
        Breaking(image, idx + 1);
        randomSeed.Remove(randomSeed.Length - 1, 1);
        randomSeed.Append('1'); // take
        Breaking(image, idx + 1);
        randomSeed.Remove(randomSeed.Length - 1, 1);
    }


    public static double CalculateColorDeviation(RGBPixel pixel)
    {
        double deviation = (Math.Pow(pixel.red - 128, 2) +
                                     Math.Pow(pixel.green - 128, 2) +
                                     Math.Pow(pixel.blue - 128, 2));
        return deviation;
    }

    public static double CalculateImageDeviation(RGBPixel[,] pixels)
    {
        int count = 0;
        double totalDeviation = 0;

        for (int x = 0; x < pixels.GetLength(0); x++)
        {
            for (int y = 0; y < pixels.GetLength(1); y++)
            {
                double deviation = CalculateColorDeviation(pixels[x, y]);
                totalDeviation += deviation;
                count++;
            }
        }

        double averageDeviation = totalDeviation / count;
        return Math.Sqrt(averageDeviation);
    }

}