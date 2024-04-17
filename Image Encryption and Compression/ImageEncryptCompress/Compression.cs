using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEncryptCompress
{
    internal class Compression
    {

        /* Priority_Queue<>*/int BuildHuffmanTree(RGBPixel[,] image)
        {
            /*
            1.	Calculate the frequency (i.e. number of pixels) for each value 
            2.	Use a Greedy algorithm to build up a Huffman Tree, such that
            a.	smaller frequencies at bottom of the tree while larger frequencies at top
            b.	assign codes to the tree by placing a 0 on every left branch and a 1 on every right branch
            c.	use priority queue for efficient implementation of selecting the minimum at each time
            3.	A traversal of the tree from root to leaf give the Huffman code for that particular leaf value
            4.	Replace each value by its corresponding Huffman code 
            5.	Store the generated Huffman code stream together with the Huffman Tree… WHY the tree?

             */
            return 0;
        }

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
}
