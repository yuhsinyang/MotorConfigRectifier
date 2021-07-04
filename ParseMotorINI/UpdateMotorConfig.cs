using ParseMotorINI.Worker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParseMotorINI
{
    class UpdateMotorConfig
    {
        static void Main(string[] args)
        {
            Console.WriteLine("**********************************************"); 
            Console.WriteLine("Eclipse Motor Location Offset Conversion Tool:");
            Console.WriteLine("**********************************************");
            Console.WriteLine("This tool is for switching versions from/to separate motor offset feature.Please specify the operation to perform:");
            Console.WriteLine("1) Upgrade from version before 1.1.3.7\n2) Upgrade from version After 1.1.3.7\n3) Downgrade to version before 1.1.3.7\n4) Downgrade to version after 1.1.3.7\n");
            Console.Write("Selection (1/2/3/4):"); 
            Regex pattern = new Regex(@"\b[1-4]\b");
            string input = Console.ReadLine();
            try
            {
                if (!pattern.IsMatch(input))
                {
                    throw new ArgumentOutOfRangeException($"Invalid input {input}");
                }
                BackupRecipe(SystemPath.configFolder, Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Recipes");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
                Environment.Exit(0);
            }
            Console.WriteLine("Backup is done. On desktop \"Recipes\"");
            
            MotorOffsetRectifier mor = new MotorOffsetRectifier();

            switch(input)
            {
                case "1":
                    mor.PerformConfigUpgradeFromBeforeLocalXYOffsets();
                    break;
                case "2":
                    mor.PerformConfigUpgradeFromAfterLocalXYOffsets();
                    break;
                case "3":
                    mor.PerformConfigDowngradeToBeforeLocalXYOffsets();
                    break;
                case "4":
                    mor.PerformConfigDowngradeToAfterLocalXYOffsets();
                    break;
            }
            Console.WriteLine("Conversion is done. Press Enter to exit...");
            Console.ReadLine();
        }
        public static void BackupRecipe(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            if(!diSource.Exists)
            {
                throw new FileNotFoundException($"Recipe folder does not exist: {diSource.FullName}, software exits.");
            }
            
            if(diTarget.Exists)
            {
                throw new FileLoadException($"Target backup folder exists already: {diTarget.FullName}, please delete the folder to continue.");
            }

            CopyAll(diSource, diTarget);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            try
            {
                Directory.CreateDirectory(target.FullName);

                // Copy each file into the new directory.
                foreach (FileInfo fi in source.GetFiles())
                {
                    Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                    fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
                }

                // Copy each subdirectory using recursion.
                foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
                {
                    DirectoryInfo nextTargetSubDir =
                        target.CreateSubdirectory(diSourceSubDir.Name);
                    CopyAll(diSourceSubDir, nextTargetSubDir);
                }
            }
            catch (UnauthorizedAccessException uaex)
            {
                Console.WriteLine(uaex);
            }
        }
    }
}
