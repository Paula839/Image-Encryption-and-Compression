using ImageEncryptCompress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class Compression
{

    /* Priority_Queue<>*/

    public class Root
    {
        public char character;
        public int frequency;
        public Root left;
        public Root right;
    }

    static Dictionary<char, int> frequencyDictionary;

    public static Root HuffmanTree(string input)
    {
        foreach (char c in input)
        {
            if (!frequencyDictionary.ContainsKey(c))
            {
                frequencyDictionary.Add(c, 0);
            }

            frequencyDictionary[c]++;
        }

        SortedDictionary<int, LinkedList<Root>> minHeap = new SortedDictionary<int, LinkedList<Root>>();

        foreach (KeyValuePair<char, int> f in frequencyDictionary)
        {
            Root z = new Root();
            z.character = f.Key;
            z.frequency = f.Value;
            z.left = z.right = null;
            if (!minHeap.ContainsKey(z.frequency))
            {
                minHeap.Add(z.frequency, new LinkedList<Root>());
            }
            minHeap[z.frequency].AddFirst(z);
        }

        while (minHeap.Count > 1 || minHeap.First().Value.Count > 1)
        {

            Root z = new Root();

            z.left = minHeap.First().Value.First();
            minHeap.First().Value.RemoveFirst();
            if (minHeap.First().Value.Count == 0)
            {
                minHeap.Remove(minHeap.First().Key);
            }
            z.right = minHeap.First().Value.First();
            minHeap.First().Value.RemoveFirst();
            if (minHeap.First().Value.Count == 0)
            {
                minHeap.Remove(minHeap.First().Key);
            }
            z.frequency = z.left.frequency + z.right.frequency;

            if (!minHeap.ContainsKey(z.frequency))
            {
                minHeap.Add(z.frequency, new LinkedList<Root>());
            }
            minHeap[z.frequency].AddFirst(z);

        }
        Root root = minHeap.First().Value.First();
        return root;
    }

    static Dictionary<char, string> HuffmanCode;
    static string code = "";
    public static void Compress(Root root)
    {
        //Base Case avoiding StackOverFlow / Inifinte Recursion
        if (root == null)
        {
            return;
        }

        //false -> 0 / true -> 1
        code += '0';
        Compress(root.left); //0 string + 0, 0
        code = code.Remove(code.Length - 1);
        code += '1';
        Compress(root.right); //1
        code = code.Remove(code.Length - 1);

        if (root.character != '\0')
            HuffmanCode[root.character] = code;
    }
    static bool found = false;
    static string decompressed = "";
    static int index = 0;
    static string Decompress(string huffmanCode, Root root, Root node)
    {
        if (node.left == null && node.right == null)
        {
            string ret = "";
            ret += node.character;
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

    static string CompressFile(string s)
    {

        string compressed = "";
        foreach (char c in s)
        {

            compressed += HuffmanCode[c];
        }
        return compressed;
    }


    //private static  BuildHuffmanTree(RGBPixel[,] image)
    //{
    //    /*
    //    1.	Calculate the frequency (i.e. number of pixels) for each value 
    //    2.	Use a Greedy algorithm to build up a Huffman Tree, such that
    //    a.	smaller frequencies at bottom of the tree while larger frequencies at top
    //    b.	assign codes to the tree by placing a 0 on every left branch and a 1 on every right branch
    //    c.	use priority queue for efficient implementation of selecting the minimum at each time
    //    3.	A traversal of the tree from root to leaf give the Huffman code for that particular leaf value
    //    4.	Replace each value by its corresponding Huffman code 
    //    5.	Store the generated Huffman code stream together with the Huffman Tree… WHY the tree?
    //    /
    //     */
    //    return null;
    //}

    //Decompress()
    //{
    /*
     1.	Reconstruct the Huffman Tree from the compressed file
     2.	Use this tree to decode the stored binary stream as follows:
        1)	Start at the root of the tree.
        2)	Repeat until you reach an external leaf node.
            i.	Read one bit from the stream.
            ii.	Take the left branch in the tree if the bit is 0; take the right branch if it is 1.
        3)	Print the value in that external node

     */
    //}

    //save()

    //load()

}

