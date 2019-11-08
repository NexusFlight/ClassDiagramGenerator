using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClassDiagramGenerator
{
    class FileHandler
    {
        public static string SelectOpenFile(OpenFileDialog openFileDialog)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    return openFileDialog.FileName;
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                    return ex.Message;
                }
            }
            return "ERROR";
        }

        public static string SelectSaveFile(SaveFileDialog saveFileDialog)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    return saveFileDialog.FileName;
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                    return ex.Message;
                }
            }
            return "ERROR";
        }

        public static string OpenFile(string filename)
        {
            
            using (StreamReader fileReader = new StreamReader(filename))
            {
                return fileReader.ReadToEnd();
            }
            
        }

        public static void SaveBitmap(Bitmap bitmap,string fileName)
        {
            bitmap.Save(fileName);
        }
    }
}
