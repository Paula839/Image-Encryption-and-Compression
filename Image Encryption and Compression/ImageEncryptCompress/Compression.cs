﻿using ImageEncryptCompress;
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
        for (int i = 0; i < treeLength && row < n; i++)
        {
            if (huffmanCode[i] == '0')
            {
                if (currentNode.left == null)
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
                if (currentNode.left == null)
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
                if (currentNode.left == null)
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

        CompressRed(HuffmanTree(image)[0]);
        CompressGreen(HuffmanTree(image)[1]);
        CompressBlue(HuffmanTree(image)[2]);

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


    public static List<byte> RedEncodeTreeToBytes(Dictionary<byte, string> huffman)
    {
        List<byte> storingBytes = new List<byte>();
        foreach (KeyValuePair<byte, string> h in huffman)
        {
            storingBytes.Add(Convert.ToByte(h.Value.PadLeft(8, '0'), 2));
        }

        return storingBytes;
    }
    public static List<byte> GreenEncodeTreeToBytes(Dictionary<byte, string> huffman)
    {
        List<byte> storingBytes = new List<byte>();
        foreach (KeyValuePair<byte, string> h in huffman)
        {
            storingBytes.Add(Convert.ToByte(h.Value.PadLeft(8, '0'), 2));
        }

        return storingBytes;
    }
    public static List<byte> BlueEncodeTreeToBytes(Dictionary<byte, string> huffman)
    {
        List<byte> storingBytes = new List<byte>();
        foreach (KeyValuePair<byte, string> h in huffman)
        {
            storingBytes.Add(Convert.ToByte(h.Value.PadLeft(8, '0'), 2));
        }

        return storingBytes;
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
                    if (currentNode.left == null)
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
                    if (currentNode.left == null)
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
                    if (currentNode.left == null)
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

    private static string ConvertToBinaryString(byte bytes)
    {
        StringBuilder binaryString = new StringBuilder();

        binaryString.Append(Convert.ToString(bytes, 2).PadLeft(8, '0'));
        return binaryString.ToString();
    }

    private static string ConvertToBinaryStringBytes(byte[] bytes)
    {
        StringBuilder binaryString = new StringBuilder();
        foreach (byte b in bytes)
        {
            // Convert byte to binary string representation with leading zeros
            binaryString.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
        }
        return binaryString.ToString();
    }

    static StringBuilder[] s;

    static byte padding;
    private static byte[] ConvertToBytes(StringBuilder huffmanCode)
    {
        padding = (byte)(huffmanCode.Length % 8);
        if (padding != 0)
            huffmanCode.Append('0', 8 - padding);

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
        s = CompressFile(image);

        byte redPadding = 0, greenPadding = 0, bluePadding = 0;

        byte[] redBytes = ConvertToBytes(s[0]);
        redPadding = (byte)((8 - padding) % 8);
        byte[] greenBytes = ConvertToBytes(s[1]);
        greenPadding = (byte)((8 - padding) % 8);
        byte[] blueBytes = ConvertToBytes(s[2]);
        bluePadding = (byte)((8 - padding) % 8);

        StringBuilder stringBuilder = new StringBuilder();

        string filePath = "D:\\Algorithm\\Project\\RELEASE\\[1] Image Encryption and Compression\\comp.bin";
        using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
        {
            writer.Write(image.GetLength(0));
            writer.Write(image.GetLength(1));

            writer.Write((byte)RedHuffmanCode.Count);
            foreach (KeyValuePair<byte, string> pair in RedHuffmanCode)
            {
                writer.Write(pair.Key);
                stringBuilder = new StringBuilder(pair.Value);
                byte[] x = ConvertToBytes(stringBuilder);
                writer.Write((byte)((8 - padding) % 8));
                writer.Write(x.Length);
                for (int i = 0; i < x.Length; i++)
                    writer.Write(x[i]);
            }

            writer.Write(redBytes.Length);
            writer.Write(redPadding);
            writer.Write(redBytes);

            writer.Write((byte)GreenHuffmanCode.Count);
            foreach (KeyValuePair<byte, string> pair in GreenHuffmanCode)
            {
                writer.Write(pair.Key);
                stringBuilder = new StringBuilder(pair.Value);
                byte[] x = ConvertToBytes(stringBuilder);
                writer.Write((byte)((8 - padding) % 8));
                writer.Write(x.Length);
                for (int i = 0; i < x.Length; i++)
                    writer.Write(x[i]);
            }


            writer.Write(greenBytes.Length);
            writer.Write(greenPadding);
            writer.Write(greenBytes);

            writer.Write((byte)BlueHuffmanCode.Count);
            foreach (KeyValuePair<byte, string> pair in BlueHuffmanCode)
            {
                writer.Write(pair.Key);
                stringBuilder = new StringBuilder(pair.Value);
                byte[] x = ConvertToBytes(stringBuilder);
                writer.Write((byte)((8 - padding) % 8));
                writer.Write(x.Length);
                for (int i = 0; i < x.Length; i++)
                    writer.Write(x[i]);
            }

            writer.Write(blueBytes.Length);
            writer.Write(bluePadding);
            writer.Write(blueBytes);

        }

    }
    public static RGBPixel[,] load(string filePath)
    {
        RedHuffmanCode = new Dictionary<byte, string>();
        GreenHuffmanCode = new Dictionary<byte, string>();
        BlueHuffmanCode = new Dictionary<byte, string>();

        n = 0;
        m = 0;

        string[] rgb = new string[3];
        int length = 0;
        byte[] redbytes, greenbytes, bluebytes;
        StringBuilder stringBuilder = new StringBuilder();
        byte redPadding = 0, greenPadding = 0, bluePadding = 0;
        using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
        {
            n = reader.ReadInt32();
            m = reader.ReadInt32();

            length = reader.ReadByte();
            if (length == 0) length = 256;
            for (int i = 0; i < length; i++)
            {
                byte key = reader.ReadByte();
                byte pad = reader.ReadByte();
                int len = reader.ReadInt32();
                byte[] x = new byte[len];
                for (int j = 0; j < len; j++)
                    x[j] = reader.ReadByte();

                stringBuilder = new StringBuilder(ConvertToBinaryStringBytes(x));
                stringBuilder.Remove(stringBuilder.Length - pad, pad);
                string value = stringBuilder.ToString();

                RedHuffmanCode.Add(key, value);
            }

            length = reader.ReadInt32();
            redPadding = reader.ReadByte();
            redbytes = reader.ReadBytes(length);


            length = reader.ReadByte();
            if (length == 0) length = 256;
            for (int i = 0; i < length; i++)
            {
                byte key = reader.ReadByte();
                byte pad = reader.ReadByte();
                int len = reader.ReadInt32();
                byte[] x = new byte[len];
                for (int j = 0; j < len; j++)
                    x[j] = reader.ReadByte();

                stringBuilder = new StringBuilder(ConvertToBinaryStringBytes(x));
                stringBuilder.Remove(stringBuilder.Length - pad, pad);
                string value = stringBuilder.ToString();
                GreenHuffmanCode.Add(key, value);
            }

            length = reader.ReadInt32();
            greenPadding = reader.ReadByte();
            greenbytes = reader.ReadBytes(length);


            length = reader.ReadByte();
            if (length == 0) length = 256;
            for (int i = 0; i < length; i++)
            {
                byte key = reader.ReadByte();
                byte pad = reader.ReadByte();
                int len = reader.ReadInt32();
                byte[] x = new byte[len];
                for (int j = 0; j < len; j++)
                    x[j] = reader.ReadByte();

                stringBuilder = new StringBuilder(ConvertToBinaryStringBytes(x));
                stringBuilder.Remove(stringBuilder.Length - pad, pad);
                string value = stringBuilder.ToString();
                BlueHuffmanCode.Add(key, value);
            }

            length = reader.ReadInt32();
            bluePadding = reader.ReadByte();
            bluebytes = reader.ReadBytes(length);


        }


        decompressed = new RGBPixel[n, m];

        Root[] root = ReconstructTree();
        stringBuilder = new StringBuilder();
        foreach (byte b in redbytes)
        {

            stringBuilder.Append(ConvertToBinaryString(b));
        }

        stringBuilder.Remove(stringBuilder.Length - redPadding, redPadding);
        rgb[0] = stringBuilder.ToString();

        stringBuilder = new StringBuilder();

        foreach (byte b in greenbytes)
        {
            stringBuilder.Append(ConvertToBinaryString(b));
        }

        stringBuilder.Remove(stringBuilder.Length - greenPadding, greenPadding);

        rgb[1] = stringBuilder.ToString();
        stringBuilder = new StringBuilder();

        foreach (byte b in bluebytes)
        {
            stringBuilder.Append(ConvertToBinaryString(b));
        }

        stringBuilder.Remove(stringBuilder.Length - bluePadding, bluePadding);

        rgb[2] = stringBuilder.ToString();



        DecompressRed(rgb[0], root[0]);
        DecompressGreen(rgb[1], root[1]);
        DecompressBlue(rgb[2], root[2]);
        return decompressed;

    }
}
