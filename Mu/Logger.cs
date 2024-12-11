using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mu
{
    public static class Logger
    {
        // private static readonly string logFilePath = "userActions.log";
        private static readonly string logFilePath = @"C:\Users\USER\Desktop\c# app\Mu.Project2\Mu.Project2\logger\looger.log";
        static Logger()
        {
            string directoryPath = Path.GetDirectoryName(logFilePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        public static void Log(string message)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(logFilePath, true))
                {
                    sw.WriteLine($"{DateTime.Now}: {message}");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that might occur during logging
                Console.WriteLine($"Failed to write to log file: {ex.Message}");
            }
        }
    }
}
