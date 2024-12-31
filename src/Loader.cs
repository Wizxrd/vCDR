using System;
using System.IO;
using System.Windows;

namespace vCDR.src
{
    public class Loader
    {
        public static bool DEBUG = false;

        public static string LoadFile(string folderPath, string fileName)
        {
            try
            {
                string folderDir;
                string filePath;
                if (DEBUG)
                {
                    folderPath = $"vCDR\\{folderPath}";
                    string binDir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                    string solutionDir = Directory.GetParent(binDir).FullName;
                    folderDir = Path.Combine(solutionDir, folderPath);
                    filePath = Path.Combine(folderDir, fileName);
                }
                else
                {
                    string exeDir = AppDomain.CurrentDomain.BaseDirectory;
                    folderDir = Path.Combine(exeDir, folderPath);
                    filePath = Path.Combine(folderDir, fileName);
                }
                if (!Directory.Exists(folderDir))
                {
                    Directory.CreateDirectory(folderDir);
                }
                if (!File.Exists(filePath))
                {
                    File.WriteAllText(filePath, "");
                }
                return filePath;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return string.Empty;
            }
        }
    }
}
