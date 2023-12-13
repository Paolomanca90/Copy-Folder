using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Copy_Folder
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Usage: Copy_Folder.exe <sourceFolderPath> <replicaFolderPath> <logFilePath>");
                return;
            }

            string sourceFolderPath = args[0];
            string replicaFolderPath = args[1];
            string logFilePath = args[2];

            Console.WriteLine($"Source Folder: {sourceFolderPath}");
            Console.WriteLine($"Replica Folder: {replicaFolderPath}");
            Console.WriteLine($"Log File: {logFilePath}");

            // Synchronization interval in milliseconds ( 5 mins)
            int syncInterval = 5 * 60 * 1000;

            Console.WriteLine($"Synchronization Interval: {syncInterval / 1000} seconds");

            // Create a timer to perform synchronization periodically
            Timer syncTimer = new Timer(SynchronizeFolders, new Tuple<string, string, string>(sourceFolderPath, replicaFolderPath, logFilePath), 0, syncInterval);

            // Keep the application running
            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();
        }

        static void SynchronizeFolders(object state)
        {
            var parameters = (Tuple<string, string, string>)state;
            string sourceFolderPath = parameters.Item1;
            string replicaFolderPath = parameters.Item2;
            string logFilePath = parameters.Item3;

            try
            {
                Log($"Synchronization started at {DateTime.Now}", logFilePath);

                // Perform one-way synchronization from source to replica
                CopyFolder(sourceFolderPath, replicaFolderPath, logFilePath);

                Log($"Synchronization completed at {DateTime.Now}", logFilePath);
            }
            catch (Exception ex)
            {
                Log($"Error during synchronization: {ex.Message}", logFilePath);
            }
        }

        static void CopyFolder(string sourceFolder, string destinationFolder, string logFilePath)
        {
            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }

            // Copy files
            foreach (var sourceFile in Directory.GetFiles(sourceFolder))
            {
                string destinationFile = Path.Combine(destinationFolder, Path.GetFileName(sourceFile));
                File.Copy(sourceFile, destinationFile, true);

                Log($"Copied file: {sourceFile} to {destinationFile}", logFilePath);
            }

            // Recursively copy subfolders
            foreach (var sourceSubfolder in Directory.GetDirectories(sourceFolder))
            {
                string destinationSubfolder = Path.Combine(destinationFolder, Path.GetFileName(sourceSubfolder));
                CopyFolder(sourceSubfolder, destinationSubfolder, logFilePath);
            }
        }

        static void Log(string message, string logFilePath)
        {
            string logEntry = $"{DateTime.Now}: {message}";

            Console.WriteLine(logEntry);

            // Append the log entry to the log file
            File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
        }
    }
}

//Start the program with your shell and type
//Copy_Folder.exe "C:\SourceFolder" "C:\ReplicaFolder" "C:\Log\synclog.txt"
//where Source and Replica are your folders and synclog.txt your LOG file
