using System;
using System.Windows.Forms;

namespace FileTagEditor
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Show a simple message box with Hello World
            MessageBox.Show("Hello World", "File Tag Editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
