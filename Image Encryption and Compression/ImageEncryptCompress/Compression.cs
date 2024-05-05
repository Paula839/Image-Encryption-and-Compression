using ImageEncryptCompress;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Root
{
    public byte color;
    public int frequency;
    public Root left;
    public Root right;

    public int CompareTo(Root other)
    {
        return frequency.CompareTo(other.frequency);
    }
}

public class PriorityQueue<T> where T : Root
{
    private List<T> heap;

    public int Count { get { return heap.Count; } }

    public PriorityQueue()
    {
        heap = new List<T>();
    }

    public void Insert(T item)
    {
        heap.Add(item);
        int i = heap.Count - 1;

        while (i > 0 && heap[i].CompareTo(heap[Parent(i)]) < 0)
        {
            Swap(i, Parent(i));
            i = Parent(i);
        }
    }

    public T ExtractMin()
    {
        if (heap.Count == 0)
            throw new InvalidOperationException("Priority queue is empty.");

        T min = heap[0];
        heap[0] = heap[heap.Count - 1];
        heap.RemoveAt(heap.Count - 1);

        MinHeapify(0);

        return min;
    }

    private void MinHeapify(int i)
    {
        int left = LeftChild(i);
        int right = RightChild(i);
        int smallest = i;

        if (left < heap.Count && heap[left].CompareTo(heap[smallest]) < 0)
            smallest = left;

        if (right < heap.Count && heap[right].CompareTo(heap[smallest]) < 0)
            smallest = right;

        if (smallest != i)
        {
            Swap(i, smallest);
            MinHeapify(smallest);
        }
    }

    private int Parent(int i)
    {
        return (i - 1) / 2;
    }

    private int LeftChild(int i)
    {
        return 2 * i + 1;
    }

    private int RightChild(int i)
    {
        return 2 * i + 2;
    }

    private void Swap(int i, int j)
    {
        T temp = heap[i];
        heap[i] = heap[j];
        heap[j] = temp;
    }
}

public static class Compression
{

    /* Priority_Queue<>*/

    public static Dictionary<byte, int> redDictionary;
    public static Dictionary<byte, int> greenDictionary;
    public static Dictionary<byte, int> blueDictionary;

    
    public struct Heap
    {


        public PriorityQueue<Root> red;
        public PriorityQueue<Root> green;
        public PriorityQueue<Root> blue;

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
        heap.red = new PriorityQueue<Root>();
        heap.green = new PriorityQueue<Root>();
        heap.blue = new PriorityQueue<Root>();

        foreach (KeyValuePair<byte, int> f in redDictionary)
        {
            Root z = new Root();
            z.color = f.Key;
            z.frequency = f.Value;
            z.left = z.right = null;
            heap.red.Insert(z);
        }

        foreach (KeyValuePair<byte, int> f in greenDictionary)
        {
            Root z = new Root();
            z.color = f.Key;
            z.frequency = f.Value;
            z.left = z.right = null;
            heap.green.Insert(z);
        }

        foreach (KeyValuePair<byte, int> f in blueDictionary)
        {
            Root z = new Root();
            z.color = f.Key;
            z.frequency = f.Value;
            z.left = z.right = null;
            heap.blue.Insert(z);
        }

        while (heap.red.Count > 1)
        {

            Root z = new Root();

            z.left = heap.red.ExtractMin();
            z.right = heap.red.ExtractMin();
            z.frequency = z.left.frequency + z.right.frequency;
            heap.red.Insert(z);
        }

        while (heap.green.Count > 1)
        {

            Root z = new Root();

            z.left = heap.green.ExtractMin();
            z.right = heap.green.ExtractMin();
            z.frequency = z.left.frequency + z.right.frequency;
            heap.green.Insert(z);
        }
        
        while (heap.blue.Count > 1)
        {

            Root z = new Root();

            z.left = heap.red.ExtractMin();
            z.right = heap.red.ExtractMin();
            z.frequency = z.left.frequency + z.right.frequency;
            heap.blue.Insert(z);
        }

        Root[] root = new Root[3];
        root[0] = heap.red.ExtractMin();
        root[1] = heap.green.ExtractMin();
        root[2] = heap.blue.ExtractMin();
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



        //TASK

        //BINARY FILE
        /*
         *
          1.	Initial seed value and tap position 
          2.	Huffman Tree
          3.	Compressed image

            001001011 //Initial Seed
            00101     //Tap Position
            0101010101001010001010101010010101010101001011  //Red Huffman Tree
            0101010101010100101010101001010101010010101010  //Green Huffman Tree
            1010010101010101010101011101010100101010101011  //Blue Huffman Tree
            01001   //Height
            10101   //W

         *
         *
         *
         *
         *
         */




















        //END TASK
        //int sizer = r.Length;
        //int sizeo = red.Length;
        //Console.WriteLine(r);
        //string filePath = "D:\\Algorithm\\Image Encryption and Compression\\Image Encryption and Compression\\Startup Code\\Image Encryption and Compression\\ImageEncryptCompress\\anabinary.bin"; // Path to the file to save
        //                                                                                                                                                                                         //string textToSave = ascii; // Text to save in the file
        //string ascii;

        //// Create a StreamWriter to write to the file
        //using (StreamWriter writer = new StreamWriter(filePath))
        //{
        //    // Write the text to the file
        //    //writer.WriteLine(textToSave);
        //    //Console.WriteLine(/*"size of text before commpression is :" + rgbCode[0].Length + "bytes" + " size after is :" + rgbCode[0].Length / (sizeof(byte)) + "bytes"*/);
        //    ascii = BinaryToAscii(rgb[0].ToString());
        //    writer.WriteLine(ascii);
        //    writer.WriteLine();

        //    writer.WriteLine("--R--");
        //    writer.WriteLine("Color - Frequency - Huffman Representation - Total Bits");
        //    int redSum = 0;
        //    foreach (KeyValuePair<byte, string> colorValues in RedHuffmanCode)
        //    {
        //        redSum += redDictionary[colorValues.Key] * colorValues.Value.Length;
        //        writer.WriteLine(colorValues.Key + " - " + redDictionary[colorValues.Key] + " - " +
        //            colorValues.Value + " - " + redDictionary[colorValues.Key] * colorValues.Value.Length);
        //    }
        //    writer.Write("Total = ");
        //    writer.Write(redSum);    
        //    writer.Write(" // ");
        //    writer.WriteLine("bits");
        //    writer.WriteLine();

        //    ascii = BinaryToAscii(rgb[1].ToString());
        //    writer.WriteLine(ascii);
        //    writer.WriteLine();

        //    writer.WriteLine("--G--");
        //    writer.WriteLine("Color - Frequency - Huffman Representation - Total Bits");
        //    int greenSum = 0;
        //    foreach (KeyValuePair<byte, string> colorValues in GreenHuffmanCode)
        //    {
        //        greenSum += greenDictionary[colorValues.Key] * colorValues.Value.Length;
        //        writer.WriteLine(colorValues.Key + " - " + greenDictionary[colorValues.Key] + " - " +
        //            colorValues.Value + " - " + greenDictionary[colorValues.Key] * colorValues.Value.Length);
        //    }
        //    writer.Write("Total = ");
        //    writer.Write(greenSum);
        //    writer.Write(" // ");
        //    writer.WriteLine("bits");
        //    writer.WriteLine();

        //    ascii = BinaryToAscii(rgb[2].ToString());
        //    writer.WriteLine(ascii);
        //    writer.WriteLine();

        //    writer.WriteLine("--B--");
        //    writer.WriteLine("Color - Frequency - Huffman Representation - Total Bits");
        //    int blueSum = 0;
        //    foreach (KeyValuePair<byte, string> colorValues in BlueHuffmanCode)
        //    {
        //        blueSum += blueDictionary[colorValues.Key] * colorValues.Value.Length;
        //        writer.WriteLine(colorValues.Key + " - " + blueDictionary[colorValues.Key] + " - " +
        //            colorValues.Value + " - " + blueDictionary[colorValues.Key] * colorValues.Value.Length);
        //    }
        //    writer.Write("Total = ");
        //    writer.Write(blueSum);
        //    writer.Write(" // ");
        //    writer.WriteLine("bits");

        //    writer.WriteLine("**Compression Output**");
        //    writer.WriteLine((redSum + greenSum + blueSum) / 8.0);

        //    //writer.WriteLine("size of text before commpression is :" + rgbCode[0].Length + "bytes" + " size after is :" + rgbCode[0].Length / (sizeof(byte)) + "bytes");

        //    //writer.WriteLine(".............RED Tree............");
        //    //Dictionary<byte, string> codes = CompressionImage.getCodes();
        //    //foreach (var kvp in codes)
        //    //{
        //    //    writer.WriteLine($"{kvp.Key}:{kvp.Value}");
        //    //}
        //    //writer.WriteLine(".............RED size............");
        //    //writer.WriteLine("size:" + sizer);
        //}
        //Console.WriteLine(ascii);
        //string binary = AsciiToBinary(ascii).Substring(0, sizer);
        // Console.WriteLine(binary);
        //Console.WriteLine(r == binary);

        //Console.WriteLine(CompressionImage.Decompress(binary, root, root));
    }
    //static string BinaryToAscii(string binaryString)
    //{
    //    // Ensure the binary string length is divisible by 8
    //    int paddingLength = 8 - (binaryString.Length % 8);
    //    if (paddingLength != 8)
    //    {
    //        binaryString = binaryString.PadRight(binaryString.Length + paddingLength, '0');
    //    }

    //    // Parse the binary string in groups of 8 bits and convert to ASCII
    //    StringBuilder asciiStringBuilder = new StringBuilder();
    //    for (int i = 0; i < binaryString.Length; i += 8)
    //    {
    //        string binaryByte = binaryString.Substring(i, 8);
    //        byte asciiByte = Convert.ToByte(binaryByte, 2);
    //        asciiStringBuilder.Append((char)asciiByte);
    //    }

    //    return asciiStringBuilder.ToString();
    //}
    //static string AsciiToBinary(string asciiString)
    //{
    //    StringBuilder binaryStringBuilder = new StringBuilder();

    //    foreach (char c in asciiString)
    //    {
    //        // Convert character to binary representation and pad with leading zeros if necessary
    //        string binaryChar = Convert.ToString(c, 2).PadLeft(8, '0');

    //        // Append binary representation of character to the binary string
    //        binaryStringBuilder.Append(binaryChar);
    //    }

    //    return binaryStringBuilder.ToString();
    //}

    //load()

}

