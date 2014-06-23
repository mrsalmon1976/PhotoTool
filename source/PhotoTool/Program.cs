using SAFish.PhotoTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoTool
{
    public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.Run(new FormMain());
            }
            catch (Exception e)
            {
                throw;
                //string err = e.Message + "\n\n" +
                //    "Please report this error on http://software.safish.com.";
                //MessageBox.Show(null, err, "PhotoTool Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //FormMain.LogError(e);
            }
        }


    }
}
