using ImageEncryptCompress;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class Compression
{

    /* Priority_Queue<>*/

    public class Root
    {
        public byte color;
        public int frequency;
        public Root left;
        public Root right;
    }


    public static Dictionary<byte, int> redDictionary;
    public static Dictionary<byte, int> greenDictionary;
    public static Dictionary<byte, int> blueDictionary;

    
    public struct Heap
    {
        public SortedDictionary<int, LinkedList<Root>> red;
        public SortedDictionary<int, LinkedList<Root>> green;
        public SortedDictionary<int, LinkedList<Root>> blue;


    }
    public static Root[] HuffmanTree(RGBPixel[,] image)
    {
        redDictionary = new Dictionary<byte, int>();
        greenDictionary = new Dictionary<byte, int>();
        blueDictionary = new Dictionary<byte, int>();

        foreach (RGBPixel pixel in image)
        {
            if (!redDictionary.ContainsKey(pixel.red))
            {
                redDictionary.Add(pixel.red, 0);
            }

            redDictionary[pixel.red]++;

            if (!greenDictionary.ContainsKey(pixel.green))
            {
                greenDictionary.Add(pixel.green, 0);
            }

            greenDictionary[pixel.green]++;

            if (!blueDictionary.ContainsKey(pixel.blue))
            {
                blueDictionary.Add(pixel.blue, 0);
            }

            blueDictionary[pixel.blue]++;
        }

        Heap heap = new Heap();
        heap.red = new SortedDictionary<int, LinkedList<Root>>();
        heap.green = new SortedDictionary<int, LinkedList<Root>>();
        heap.blue = new SortedDictionary<int, LinkedList<Root>>();

        foreach (KeyValuePair<byte, int> f in redDictionary)
        {
            Root z = new Root();
            z.color = f.Key;
            z.frequency = f.Value;
            z.left = z.right = null;
            if (!heap.red.ContainsKey(z.frequency))
            {
                heap.red.Add(z.frequency, new LinkedList<Root>());
            }
            heap.red[z.frequency].AddFirst(z);
        }

        foreach (KeyValuePair<byte, int> f in greenDictionary)
        {
            Root z = new Root();
            z.color = f.Key;
            z.frequency = f.Value;
            z.left = z.right = null;
            if (!heap.green.ContainsKey(z.frequency))
            {
                heap.green.Add(z.frequency, new LinkedList<Root>());
            }
            heap.green[z.frequency].AddFirst(z);
        }

        foreach (KeyValuePair<byte, int> f in blueDictionary)
        {
            Root z = new Root();
            z.color = f.Key;
            z.frequency = f.Value;
            z.left = z.right = null;
            if (!heap.blue.ContainsKey(z.frequency))
            {
                heap.blue.Add(z.frequency, new LinkedList<Root>());
            }
            heap.blue[z.frequency].AddFirst(z);
        }

        while (heap.red.Count > 1 || heap.red.First().Value.Count > 1)
        {

            Root z = new Root();

            z.left = heap.red.First().Value.First();
            heap.red.First().Value.RemoveFirst();
            if (heap.red.First().Value.Count == 0)
            {
                heap.red.Remove(heap.red.First().Key);
            }
            z.right = heap.red.First().Value.First();
            heap.red.First().Value.RemoveFirst();
            if (heap.red.First().Value.Count == 0)
            {
                heap.red.Remove(heap.red.First().Key);
            }
            z.frequency = z.left.frequency + z.right.frequency;

            if (!heap.red.ContainsKey(z.frequency))
            {
                heap.red.Add(z.frequency, new LinkedList<Root>());
            }
            heap.red[z.frequency].AddFirst(z);

        }

        while (heap.green.Count > 1 || heap.green.First().Value.Count > 1)
        {

            Root z = new Root();

            z.left = heap.green.First().Value.First();
            heap.green.First().Value.RemoveFirst();
            if (heap.green.First().Value.Count == 0)
            {
                heap.green.Remove(heap.green.First().Key);
            }
            z.right = heap.green.First().Value.First();
            heap.green.First().Value.RemoveFirst();
            if (heap.green.First().Value.Count == 0)
            {
                heap.green.Remove(heap.green.First().Key);
            }
            z.frequency = z.left.frequency + z.right.frequency;

            if (!heap.green.ContainsKey(z.frequency))
            {
                heap.green.Add(z.frequency, new LinkedList<Root>());
            }
            heap.green[z.frequency].AddFirst(z);

        }

        while (heap.blue.Count > 1 || heap.blue.First().Value.Count > 1)
        {

            Root z = new Root();

            z.left = heap.blue.First().Value.First();
            heap.blue.First().Value.RemoveFirst();
            if (heap.blue.First().Value.Count == 0)
            {
                heap.blue.Remove(heap.blue.First().Key);
            }
            z.right = heap.blue.First().Value.First();
            heap.blue.First().Value.RemoveFirst();
            if (heap.blue.First().Value.Count == 0)
            {
                heap.blue.Remove(heap.blue.First().Key);
            }
            z.frequency = z.left.frequency + z.right.frequency;

            if (!heap.blue.ContainsKey(z.frequency))
            {
                heap.blue.Add(z.frequency, new LinkedList<Root>());
            }
            heap.blue[z.frequency].AddFirst(z);

        }
        Root[] root = new Root[3];
        root[0] = heap.red.First().Value.First();
        root[1] = heap.green.First().Value.First();
        root[2] = heap.blue.First().Value.First();
        return root;
    }

    static Dictionary<byte, string> RedHuffmanCode;
    static Dictionary<byte, string> GreenHuffmanCode;
    static Dictionary<byte, string> BlueHuffmanCode;
    static string code;
    public static void CompressRed(Root root)
    {
        //Base Case avoiding StackOverFlow / Inifinte Recursion
        if (root == null)
        {
            return;
        }

        //false -> 0 / true -> 1
        code += '0';
        CompressRed(root.left); //0 string + 0, 0
        code = code.Remove(code.Length - 1);
        code += '1';
        CompressRed(root.right); //1
        code = code.Remove(code.Length - 1);

        if (root.left == null && root.right == null)
            RedHuffmanCode[root.color] = code;
    }

    public static void CompressGreen(Root root)
    {
        //Base Case avoiding StackOverFlow / Inifinte Recursion
        if (root == null)
        {
            return;
        }

        //false -> 0 / true -> 1
        code += '0';
        CompressGreen(root.left); //0 string + 0, 0
        code = code.Remove(code.Length - 1);
        code += '1';
        CompressGreen(root.right); //1
        code = code.Remove(code.Length - 1);

        if (root.left == null && root.right == null)
            GreenHuffmanCode[root.color] = code;
    }

    public static void CompressBlue(Root root)
    {
        //Base Case avoiding StackOverFlow / Inifinte Recursion
        if (root == null)
        {
            return;
        }

        //false -> 0 / true -> 1
        code += '0';
        CompressBlue(root.left); //0 string + 0, 0
        code = code.Remove(code.Length - 1);
        code += '1';
        CompressBlue(root.right); //1
        code = code.Remove(code.Length - 1);

        if (root.left == null && root.right == null)
            BlueHuffmanCode[root.color] = code;
    }
    static bool found = false;
    static string decompressed = "";
    static int index = 0;
    static string Decompress(string huffmanCode, Root root, Root node)
    {
        if (node.left == null && node.right == null)
        {
            string ret = "";
            ret += node.color;
            found = true;
            return ret;
        }

        while (index < huffmanCode.Length)
        {

            if (node == root)
            {
                found = false;
            }

            if (found)
            {
                return "";
            }

            if (huffmanCode[index] == '0')
            {
                index++;
                string ret = Decompress(huffmanCode, root, node.left);
                decompressed += ret;
            }
            else
            {
                index++;
                string ret = Decompress(huffmanCode, root, node.right);
                decompressed += ret;
            }
        }
        if (node == root)
            return decompressed;
        else return "";
    }

    static StringBuilder[] CompressFile(RGBPixel[,] image)
    {


        StringBuilder[] Scompressed = new StringBuilder[3];
        Scompressed[0] = new StringBuilder();
        Scompressed[1] = new StringBuilder();
        Scompressed[2] = new StringBuilder();


        foreach (RGBPixel c in image)
        {

            Scompressed[0].Append(RedHuffmanCode[c.red]);
            Scompressed[1].Append(GreenHuffmanCode[c.green]);
            Scompressed[2].Append(BlueHuffmanCode[c.blue]);
        }
        return Scompressed;
    }

    public static void save(RGBPixel[,] image)
    {
        redDictionary = new Dictionary<byte, int>();
        greenDictionary = new Dictionary<byte, int>();
        blueDictionary = new Dictionary<byte, int>();

        RedHuffmanCode = new Dictionary<byte, string>();
        GreenHuffmanCode = new Dictionary<byte, string>();
        BlueHuffmanCode = new Dictionary<byte, string>();
        Root[] root = HuffmanTree(image);

        string[] rgbCode = new string[3];
        rgbCode[0] = "";
        rgbCode[1] = "";
        rgbCode[2] = "";
        CompressRed(root[0]);
        rgbCode[0] = code;
        code = "";
        CompressGreen(root[1]);
        rgbCode[1] = code;
        code = "";
        CompressBlue(root[2]);
        rgbCode[2] = code;
        code = "";
        StringBuilder[] rgb = CompressFile(image);
        //int sizer = r.Length;
        //int sizeo = red.Length;
        //Console.WriteLine(r);
        string filePath = "D:\\Algorithm\\Image Encryption and Compression\\Image Encryption and Compression\\Startup Code\\Image Encryption and Compression\\ImageEncryptCompress\\output.txt"; // Path to the file to save
                                                                                                                                                                                                 //string textToSave = ascii; // Text to save in the file
        string ascii;

        // Create a StreamWriter to write to the file
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // Write the text to the file
            //writer.WriteLine(textToSave);
            //Console.WriteLine(/*"size of text before commpression is :" + rgbCode[0].Length + "bytes" + " size after is :" + rgbCode[0].Length / (sizeof(byte)) + "bytes"*/);
            ascii = BinaryToAscii(rgb[0].ToString());
            writer.WriteLine(ascii);
            writer.WriteLine();

            writer.WriteLine("--R--");
            writer.WriteLine("Color - Frequency - Huffman Representation - Total Bits");
            int redSum = 0;
            foreach (KeyValuePair<byte, string> colorValues in RedHuffmanCode)
            {
                redSum += redDictionary[colorValues.Key] * colorValues.Value.Length;
                writer.WriteLine(colorValues.Key + " - " + redDictionary[colorValues.Key] + " - " +
                    colorValues.Value + " - " + redDictionary[colorValues.Key] * colorValues.Value.Length);
            }
            writer.Write("Total = ");
            writer.Write(redSum);    
            writer.Write(" // ");
            writer.WriteLine("bits");
            writer.WriteLine();

            ascii = BinaryToAscii(rgb[1].ToString());
            writer.WriteLine(ascii);
            writer.WriteLine();

            writer.WriteLine("--G--");
            writer.WriteLine("Color - Frequency - Huffman Representation - Total Bits");
            int greenSum = 0;
            foreach (KeyValuePair<byte, string> colorValues in GreenHuffmanCode)
            {
                greenSum += greenDictionary[colorValues.Key] * colorValues.Value.Length;
                writer.WriteLine(colorValues.Key + " - " + greenDictionary[colorValues.Key] + " - " +
                    colorValues.Value + " - " + greenDictionary[colorValues.Key] * colorValues.Value.Length);
            }
            writer.Write("Total = ");
            writer.Write(greenSum);
            writer.Write(" // ");
            writer.WriteLine("bits");
            writer.WriteLine();

            ascii = BinaryToAscii(rgb[2].ToString());
            writer.WriteLine(ascii);
            writer.WriteLine();

            writer.WriteLine("--B--");
            writer.WriteLine("Color - Frequency - Huffman Representation - Total Bits");
            int blueSum = 0;
            foreach (KeyValuePair<byte, string> colorValues in BlueHuffmanCode)
            {
                blueSum += blueDictionary[colorValues.Key] * colorValues.Value.Length;
                writer.WriteLine(colorValues.Key + " - " + blueDictionary[colorValues.Key] + " - " +
                    colorValues.Value + " - " + blueDictionary[colorValues.Key] * colorValues.Value.Length);
            }
            writer.Write("Total = ");
            writer.Write(blueSum);
            writer.Write(" // ");
            writer.WriteLine("bits");

            writer.WriteLine("**Compression Output**");
            writer.WriteLine((redSum + greenSum + blueSum) / 8.0);

            //writer.WriteLine("size of text before commpression is :" + rgbCode[0].Length + "bytes" + " size after is :" + rgbCode[0].Length / (sizeof(byte)) + "bytes");

            //writer.WriteLine(".............RED Tree............");
            //Dictionary<byte, string> codes = CompressionImage.getCodes();
            //foreach (var kvp in codes)
            //{
            //    writer.WriteLine($"{kvp.Key}:{kvp.Value}");
            //}
            //writer.WriteLine(".............RED size............");
            //writer.WriteLine("size:" + sizer);
        }
        //Console.WriteLine(ascii);
        //string binary = AsciiToBinary(ascii).Substring(0, sizer);
        // Console.WriteLine(binary);
        Console.WriteLine("....................................................");
        //Console.WriteLine(r == binary);

        //Console.WriteLine(CompressionImage.Decompress(binary, root, root));
    }
    static string BinaryToAscii(string binaryString)
    {
        // Ensure the binary string length is divisible by 8
        int paddingLength = 8 - (binaryString.Length % 8);
        if (paddingLength != 8)
        {
            binaryString = binaryString.PadRight(binaryString.Length + paddingLength, '0');
        }

        // Parse the binary string in groups of 8 bits and convert to ASCII
        StringBuilder asciiStringBuilder = new StringBuilder();
        for (int i = 0; i < binaryString.Length; i += 8)
        {
            string binaryByte = binaryString.Substring(i, 8);
            byte asciiByte = Convert.ToByte(binaryByte, 2);
            asciiStringBuilder.Append((char)asciiByte);
        }

        return asciiStringBuilder.ToString();
    }
    static string AsciiToBinary(string asciiString)
    {
        StringBuilder binaryStringBuilder = new StringBuilder();

        foreach (char c in asciiString)
        {
            // Convert character to binary representation and pad with leading zeros if necessary
            string binaryChar = Convert.ToString(c, 2).PadLeft(8, '0');

            // Append binary representation of character to the binary string
            binaryStringBuilder.Append(binaryChar);
        }

        return binaryStringBuilder.ToString();
    }

    //load()

}

