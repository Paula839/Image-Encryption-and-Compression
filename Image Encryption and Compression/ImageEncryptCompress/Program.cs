
using System;
using System.Collections.Generic;

using System.Windows.Forms;

namespace ImageEncryptCompress
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        ///
        


        
        [STAThread]
        static void Main()
        {
            Console.WriteLine("HE#LLO");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}