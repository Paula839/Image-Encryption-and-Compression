using ImageEncryptCompress;
using Lucene.Net.Index;
using Lucene.Net.Search;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Lucene.Net.Store.Lock;


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

            z.left = heap.blue.ExtractMin();
            z.right = heap.blue.ExtractMin();
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
    static StringBuilder[] code;
    public static void CompressRed(Root root)
    {
        //Base Case avoiding StackOverFlow / Inifinte Recursion
        if (root == null)
        {
            return;
        }

        //false -> 0 / true -> 1
        code[0].Append('0');
        CompressRed(root.left); //0 string + 0, 0
        code[0].Remove(code[0].Length - 1, 1);
        code[0].Append('1');
        CompressRed(root.right); //1
        code[0].Remove(code[0].Length - 1, 1);

        if (root.left == null && root.right == null)
            RedHuffmanCode[root.color] = code[0].ToString();
    }

    public static void CompressGreen(Root root)
    {
        //Base Case avoiding StackOverFlow / Inifinte Recursion
        if (root == null)
        {
            return;
        }

        //false -> 0 / true -> 1
        code[1].Append('0');
        CompressGreen(root.left); //0 string + 0, 0
        code[1].Remove(code[1].Length - 1, 1);
        code[1].Append('1');
        CompressGreen(root.right); //1
        code[1].Remove(code[1].Length - 1, 1);

        if (root.left == null && root.right == null)
            GreenHuffmanCode[root.color] = code[1].ToString();
    }

    public static void CompressBlue(Root root)
    {
        //Base Case avoiding StackOverFlow / Inifinte Recursion
        if (root == null)
        {
            return;
        }

        //false -> 0 / true -> 1
        code[2].Append('0');
        CompressBlue(root.left); //0 string + 0, 0
        code[2].Remove(code[2].Length - 1, 1);
        code[2].Append('1');
        CompressBlue(root.right); //1
        code[2].Remove(code[2].Length - 1, 1);

        if (root.left == null && root.right == null)
            BlueHuffmanCode[root.color] = code[2].ToString();
    }
    static bool found = false;
    static RGBPixel[,] decompressed;
    static int index = 0;
    static int n, m;
    static void DecompressRed(string huffmanCode, Root root)
    {
        Root currentNode = root;
        int treeLength = huffmanCode.Length;
        int row = 0, col = 0;
        for (int i = 0; i < treeLength; i++)
        {
            if (huffmanCode[i] == '0')
            {
                if (currentNode.left == null) //doesnt exists
                {
                    decompressed[row, col].red = currentNode.color;
                    col++;
                    row += col / m;
                    col %= m;
                    currentNode = root;
                }
                currentNode = currentNode.left;
            }
            else
            {
                if (currentNode.right == null)
                {
                    decompressed[row, col].red = currentNode.color;
                    col++;
                    row += col / m;
                    col %= m;
                    currentNode = root;
                }
                currentNode = currentNode.right;
            }
        }


    }

    static void DecompressGreen(string huffmanCode, Root root)
    {

        Root currentNode = root;
        int treeLength = huffmanCode.Length;
        int row = 0, col = 0;
        for (int i = 0; i < treeLength && row < n; i++)
        {
            if (huffmanCode[i] == '0')
            {
                if (currentNode.left == null) //doesnt exists
                {
                    decompressed[row, col].green = currentNode.color;
                    col++;
                    row += col / m;
                    col %= m;
                    currentNode = root;
                }
                currentNode = currentNode.left;
            }
            else
            {
                if (currentNode.right == null)
                {
                    decompressed[row, col].green = currentNode.color;
                    col++;
                    row += col / m;
                    col %= m;
                    currentNode = root;
                }
                currentNode = currentNode.right;
            }
        }


    }
    static void DecompressBlue(string huffmanCode, Root root)
    {
        StringBuilder ret = new StringBuilder();
        Root currentNode = root;
        int treeLength = huffmanCode.Length;
        int row = 0, col = 0;
        for (int i = 0; i < treeLength && row < n; i++)
        {
            if (huffmanCode[i] == '0')
            {
                if (currentNode.left == null) //doesnt exists
                {
                    decompressed[row, col].blue = currentNode.color;
                    col++;
                    row += col / m;
                    col %= m;
                    currentNode = root;
                }
                currentNode = currentNode.left;
            }
            else
            {
                if (currentNode.right == null)
                {
                    decompressed[row, col].blue = currentNode.color;
                    col++;
                    row += col / m;
                    col %= m;
                    currentNode = root;
                }
                currentNode = currentNode.right;
            }
        }


    }






    private static StringBuilder[] CompressFile(RGBPixel[,] image)
    {
        StringBuilder[] Scompressed = new StringBuilder[3];
        Scompressed[0] = new StringBuilder();
        Scompressed[1] = new StringBuilder();
        Scompressed[2] = new StringBuilder();

        // Populate Huffman codes dictionaries
        CompressRed(HuffmanTree(image)[0]);
        CompressGreen(HuffmanTree(image)[1]);
        CompressBlue(HuffmanTree(image)[2]);

        // Access Huffman codes dictionaries and compress image
        foreach (RGBPixel pixel in image)
        {
            if (RedHuffmanCode.ContainsKey(pixel.red))
            {
                Scompressed[0].Append(RedHuffmanCode[pixel.red]);
            }

            if (GreenHuffmanCode.ContainsKey(pixel.green))
            {
                Scompressed[1].Append(GreenHuffmanCode[pixel.green]);
            }

            if (BlueHuffmanCode.ContainsKey(pixel.blue))
            {
                Scompressed[2].Append(BlueHuffmanCode[pixel.blue]);
            }
        }

        return Scompressed;
    }

    public static byte[] EncodeToBytes(StringBuilder colorChannel)
    {
        List<byte> storingBytes = new List<byte>();
        int i = 0;
        string tmp = colorChannel.ToString();
        for (; i + 8 < colorChannel.Length; i += 8)
        {
            storingBytes.Add(Convert.ToByte(tmp.Substring(i, 8), 2));
        }
        if (i < colorChannel.Length)
        {
            storingBytes.Add(Convert.ToByte(tmp.Substring(i, (colorChannel.Length) - i), 2));
        }
        return storingBytes.ToArray();
    }

    static List<byte> Redpadding;
    static List<byte> Greenpadding;
    static List<byte> Bluepadding;

    public static List<byte> RedEncodeTreeToBytes(Dictionary<byte, string> huffman)
    {
        List<byte> storingBytes = new List<byte>();
        foreach(KeyValuePair<byte, string> h in huffman)
        {
            Redpadding.Add((byte)(8 - h.Value.Length % 8));
            storingBytes.Add(Convert.ToByte(h.Value.PadLeft(8, '0'), 2));
        }

        return storingBytes;
    }
    public static List<byte> GreenEncodeTreeToBytes(Dictionary<byte, string> huffman)
    {
        List<byte> storingBytes = new List<byte>();
        foreach(KeyValuePair<byte, string> h in huffman)
        {
            Greenpadding.Add((byte)(8 - h.Value.Length % 8));
            storingBytes.Add(Convert.ToByte(h.Value.PadLeft(8, '0'), 2));
        }

        return storingBytes;
    }
    public static List<byte> BlueEncodeTreeToBytes(Dictionary<byte, string> huffman)
    {
        List<byte> storingBytes = new List<byte>();
        foreach(KeyValuePair<byte, string> h in huffman)
        {
            Bluepadding.Add((byte)(8 - h.Value.Length % 8));
            storingBytes.Add(Convert.ToByte(h.Value.PadLeft(8, '0'), 2));
        }

        return storingBytes;
    }

    private static byte[] ConvertToBytesInt(StringBuilder huffmanCode)
    {
        int padding = 8 - huffmanCode.Length % 8;
        if (padding != 8)
            huffmanCode.Append('0', padding);

        string binaryString = huffmanCode.ToString();
        List<byte> bytes = new List<byte>();
        for (int i = 0; i < binaryString.Length; i ++)
        {
            bytes.Add(Convert.ToByte(binaryString[i]));
        }
        return bytes.ToArray();
    }


    public static Root[] ReconstructTree()

    {//0101011000110010
        Root[] root = new Root[3];
        root[0] = new Root();
        foreach (KeyValuePair<byte, string> kvp in RedHuffmanCode)
        {
            Root currentNode = root[0];
            string code = kvp.Value;
            for (int i = 0; i < code.Length; i++)
            {
                if (code[i] == '0')
                {
                    if (currentNode.left == null) //doesnt exists
                    {
                        currentNode.left = new Root();
                    }
                    currentNode = currentNode.left;
                }
                else if (code[i] == '1')
                {
                    if (currentNode.right == null)
                    {
                        currentNode.right = new Root();
                    }
                    currentNode = currentNode.right;
                }
            }

            currentNode.color = kvp.Key;
        }

        root[1] = new Root();
        foreach (KeyValuePair<byte, string> kvp in GreenHuffmanCode)
        {
            Root currentNode = root[1];
            string code = kvp.Value;
            for (int i = 0; i < code.Length; i++)
            {
                if (code[i] == '0')
                {
                    if (currentNode.left == null) //doesnt exists
                    {
                        currentNode.left = new Root();
                    }
                    currentNode = currentNode.left;
                }
                else if (code[i] == '1')
                {
                    if (currentNode.right == null)
                    {
                        currentNode.right = new Root();
                    }
                    currentNode = currentNode.right;
                }
            }

            currentNode.color = kvp.Key;
        }

        root[2] = new Root();
        foreach (KeyValuePair<byte, string> kvp in BlueHuffmanCode)
        {
            Root currentNode = root[2];
            string code = kvp.Value;
            for (int i = 0; i < code.Length; i++)
            {
                if (code[i] == '0')
                {
                    if (currentNode.left == null) //doesnt exists
                    {
                        currentNode.left = new Root();
                    }
                    currentNode = currentNode.left;
                }
                else if (code[i] == '1')
                {
                    if (currentNode.right == null)
                    {
                        currentNode.right = new Root();
                    }
                    currentNode = currentNode.right;
                }
            }

            currentNode.color = kvp.Key;
        }


        return root;
    }

    public static string ConvertToBinary(int number)
    {
        if (number == 0)
            return "0"; // Special case for zero

        string binary = ""; // Initialize an empty string to store the binary representation

        while (number > 0)
        {
            // Get the least significant bit of the number
            int bit = number % 2;

            // Prepend the bit to the binary string
            binary = bit + binary;

            // Shift the number right by 1 bit
            number >>= 1;
        }
        return binary;
    }

    private static string ConvertToBinaryString(byte bytes)
    {
        StringBuilder binaryString = new StringBuilder();

        // Convert byte to binary string representation with leading zeros
        binaryString.Append(Convert.ToString(bytes, 2).PadLeft(8, '0'));
        return binaryString.ToString();
    }


    private static string ConvertToBinaryStringPading(byte bytes)
    {
        StringBuilder binaryString = new StringBuilder();

        // Convert byte to binary string representation with leading zeros
        binaryString.Append(Convert.ToString(bytes, 2).PadRight(8, '0'));
        return binaryString.ToString();
    }

    static string[] SplitString(string str, int chunkSize)
    {
        int numChunks = str.Length / chunkSize;
        string[] chunks = new string[numChunks];
        for (int i = 0; i < numChunks; i++)
        {
            chunks[i] = str.Substring(i * chunkSize, chunkSize);
        }
        return chunks;
    }

    static StringBuilder[] s;

    private static byte[] ConvertToBytes(StringBuilder huffmanCode)
    {
        int padding = 8 - huffmanCode.Length % 8;
        if (padding != 8)
            huffmanCode.Append('0', padding);

        string binaryString = huffmanCode.ToString();
        List<byte> bytes = new List<byte>();
        for (int i = 0; i < binaryString.Length; i += 8)
        {
            string byteString = binaryString.Substring(i, 8);
            bytes.Add(Convert.ToByte(byteString, 2));
        }
        return bytes.ToArray();
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

        code = new StringBuilder[3];
        for (int i = 0; i < 3; i++) code[i] = new StringBuilder();
        //CompressRed(root[0]);
        //CompressGreen(root[1]);
        //CompressBlue(root[2]);



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


         s = CompressFile(image);

        // Combine the Huffman codes into a single string for each color channel
        StringBuilder redHuffmanCode = s[0];
        StringBuilder greenHuffmanCode = s[1];
        StringBuilder blueHuffmanCode = s[2];

        // Convert Huffman codes from strings to bytes
        byte[] redBytes = ConvertToBytes(redHuffmanCode);
        byte[] greenBytes = ConvertToBytes(greenHuffmanCode);
        byte[] blueBytes = ConvertToBytes(blueHuffmanCode);

        // Write the Huffman codes to the binary file
        StringBuilder writing = new StringBuilder();
        byte[] writingBytes = new byte[3];

        {        string filePath = "D:\\Algorithm\\Project\\RELEASE\\[1] Image Encryption and Compression\\comp.bin";
      
            using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
            {
                writer.Write(image.GetLength(0));
                writer.Write(image.GetLength(1));
                
                writer.Write(RedHuffmanCode.Count);
                foreach(KeyValuePair<byte, string> pair in RedHuffmanCode)
                {
                    writer.Write(pair.Key);
                    writer.Write(pair.Value);
                }

                writer.Write(redBytes.Length);
                writer.Write(redBytes);

                writer.Write(GreenHuffmanCode.Count);
                foreach (KeyValuePair<byte, string> pair in GreenHuffmanCode)
                {
                    writer.Write(pair.Key);
                    writer.Write(pair.Value);
                }


                writer.Write(greenBytes.Length);
                writer.Write(greenBytes);

                writer.Write(BlueHuffmanCode.Count);
                foreach (KeyValuePair<byte, string> pair in BlueHuffmanCode)
                {
                    writer.Write(pair.Key);
                    writer.Write(pair.Value);
                }


                writer.Write(blueBytes.Length);
                writer.Write(blueBytes);

            }
        }

        //string filePath = "D:\\Algorithm\\Project\\RELEASE\\[1] Image Encryption and Compression\\comp.txt";
        //string[] chunks;

        //using (StreamWriter writer = new StreamWriter(filePath))
        //{
        //    // Write the string to the file
        //    foreach (KeyValuePair<byte, string> kvp in RedHuffmanCode)
        //    {
        //        writer.Write($"{(kvp.Key)} ");
        //        chunks = SplitString((kvp.Value), 64);
        //        foreach (string chunk in chunks)
        //        {
        //            UInt64 decimalNumber = Convert.ToUInt64(chunk, 2);
        //            writer.Write(decimalNumber);
        //        }
        //        writer.WriteLine();
        //    }

        //    chunks = SplitString(rgb[0].ToString(), 64);
        //    foreach (string chunk in chunks)
        //    {
        //        UInt64 decimalNumber = Convert.ToUInt64(chunk, 2);
        //        writer.Write(decimalNumber);
        //    }
        //    writer.WriteLine();

        //    foreach (KeyValuePair<byte, string> kvp in GreenHuffmanCode)
        //    {
        //        writer.Write($"{(kvp.Key)} ");
        //        chunks = SplitString((kvp.Value), 64);
        //        foreach (string chunk in chunks)
        //        {
        //            UInt64 decimalNumber = Convert.ToUInt64(chunk, 2);
        //            writer.Write(decimalNumber);
        //        }
        //        writer.WriteLine();
        //    }
        //    chunks = SplitString(rgb[1].ToString(), 64);
        //    foreach (string chunk in chunks)
        //    {
        //        UInt64 decimalNumber = Convert.ToUInt64(chunk, 2);
        //        writer.Write(decimalNumber);
        //    }
        //    writer.WriteLine();

        //    foreach (KeyValuePair<byte, string> kvp in BlueHuffmanCode)
        //    {
        //        writer.Write($"{(kvp.Key)} ");
        //        chunks = SplitString((kvp.Value), 64);
        //        foreach (string chunk in chunks)
        //        {
        //            UInt64 decimalNumber = Convert.ToUInt64(chunk, 2);
        //            writer.Write(decimalNumber);
        //        }
        //        writer.WriteLine();
        //    }

        //    chunks = SplitString(rgb[2].ToString(), 64);
        //    foreach (string chunk in chunks)
        //    {
        //        UInt64 decimalNumber = Convert.ToUInt64(chunk, 2);
        //        writer.Write(decimalNumber);
        //    }
        //    writer.WriteLine();

        //    writer.Write($"{(image.GetLength(0))} {(image.GetLength(1))}");

        //}





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
    
    public static RGBPixel[,] load(string filePath)
    {
        RedHuffmanCode = new Dictionary<byte, string>();
        GreenHuffmanCode = new Dictionary<byte, string>();
        BlueHuffmanCode = new Dictionary<byte, string>();
       
        n = 0;
        m = 0;

        string[] rgb = new string[3];
        int length = 0;
        byte[] redaya, greenaya, bluhaya;
        StringBuilder stringHelper = new StringBuilder();
        using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
        {
            n = reader.ReadInt32();
            m = reader.ReadInt32();

            length = reader.ReadInt32();
            for (int i = 0; i < length; i++)
            {
                byte key = reader.ReadByte();
                string value = reader.ReadString();
                RedHuffmanCode.Add(key, value);
            }

            length = reader.ReadInt32();
            redaya = reader.ReadBytes(length);


            length = reader.ReadInt32();
            for (int i = 0; i < length; i++)
            {
                byte key = reader.ReadByte();
                string value = reader.ReadString();
                GreenHuffmanCode.Add(key, value);
            }

            length = reader.ReadInt32();
            greenaya = reader.ReadBytes(length);


            length = reader.ReadInt32();
            for (int i = 0; i < length; i++)
            {
                byte key = reader.ReadByte();
                string value = reader.ReadString();
                BlueHuffmanCode.Add(key, value);
            }

            length = reader.ReadInt32();
            bluhaya = reader.ReadBytes(length);


        }


        decompressed = new RGBPixel[n, m];
   
        Root[] root = ReconstructTree();

        foreach(byte b in redaya)
        {
            stringHelper.Append(ConvertToBinaryString(b));
        }
        rgb[0] = stringHelper.ToString();

        stringHelper = new StringBuilder();

        foreach(byte b in greenaya)
        {
            stringHelper.Append(ConvertToBinaryString(b));
        }

        rgb[1] = stringHelper.ToString();
        stringHelper = new StringBuilder();

        foreach(byte b in bluhaya)
        {
            stringHelper.Append(ConvertToBinaryString(b));
        }
        rgb[2] = stringHelper.ToString();

        DecompressRed(rgb[0], root[0]);
        DecompressGreen(rgb[1], root[1]);
        DecompressBlue(rgb[2], root[2]);
        return decompressed;

    }
}
