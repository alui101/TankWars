using Controller;
using System;
using System.Windows.Forms;
using View;

namespace SS
{
    /// <summary>
    /// Author : Ali Hassoun & Emiliano Plaza, Spring 2021
    /// 
    /// University of Utah
    /// 
    /// This is where the program begins.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            GameController gc = new GameController();
            // Start an application context and run one form inside it
            Application.Run(new Form1(gc));
        }
    }
}
