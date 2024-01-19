using System;
using System.Data.SQLite;
using System.Windows.Forms;

namespace DPM_v._0._1
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Utwórz główne okno aplikacji
            Form1 mainForm = new Form1();

            // Uruchom aplikację
            Application.Run(mainForm);
        }
    }
}
