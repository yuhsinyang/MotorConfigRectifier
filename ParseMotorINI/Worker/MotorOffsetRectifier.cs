using ParseMotorINI.Worker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseMotorINI.Worker
{
    class MotorOffsetRectifier
    {
        SystemOffsetRectifier _sor;
        DeviceOffsetRectifier _dor;
        
        public MotorOffsetRectifier()
        {
            _sor = new SystemOffsetRectifier();
            _dor = new DeviceOffsetRectifier();
        } 
        public void PerformConfigUpgradeFromBeforeLocalXYOffsets() => _sor.ClearAllMotorOffsetsinConfig();
        public void PerformConfigUpgradeFromAfterLocalXYOffsets() => _sor.RectifyNumberofMotorOffsetinConfig();
        public void PerformConfigDowngradeToBeforeLocalXYOffsets() => WriteTotalOffsetsToPackage();
        public void PerformConfigDowngradeToAfterLocalXYOffsets() => WriteTotalOffsetsToSystemandPackage();
        void WriteTotalOffsetsToPackage()
        {
            LoadMotorOffsetValues();

            _sor.ClearAllMotorOffsetsinConfig();

            WriteTotalOffsetstoPackages(MotorOffsetInfo.WriteAllTotalOffsetToPackage);
        }
        void WriteTotalOffsetsToSystemandPackage()
        {
            LoadMotorOffsetValues();

            _sor.ClearAllMotorOffsetsinConfig();
            _dor.LoadMotorOffsetValues(MotorOffsetInfo.GetCurrentPackageName());
            MotorOffsetInfo.WritePartialTotalOffsetToSystem(_sor.systemMotorOffsetValuePair, _dor.deviceMotorOffsetValuePair);

            WriteTotalOffsetstoPackages(MotorOffsetInfo.WritePartialTotalOffsetToPackage);
        }
        void LoadMotorOffsetValues()
        {
            _sor.ClearMotorOffsetValues();
            _dor.ClearMotorOffsetValues();

            _sor.LoadMotorOffsetValues();
        }
        void WriteTotalOffsetstoPackages(Action<Dictionary<string, Dictionary<string, string>>, Dictionary<string, Dictionary<string, string>>, string> WriteToPackage)
        {
            Stack<string> failedFiles = new Stack<string>();
            var pkgFiles = Directory.EnumerateFiles(SystemPath.packageFolder, "*.pkg", SearchOption.AllDirectories);

            foreach (var currentFile in pkgFiles)
            {
                try
                {
                    if (currentFile.Equals($"{SystemPath.packageFolder}\\Default.pkg"))
                    {
                        Console.WriteLine("Skip Default.pkg");
                        continue;
                    }
                    Console.WriteLine($"Converting: {currentFile}");
                    _dor.LoadMotorOffsetValues(currentFile);
                    WriteToPackage(_sor.systemMotorOffsetValuePair, _dor.deviceMotorOffsetValuePair, currentFile);
                }
                catch (KeyNotFoundException knex)
                {
                    failedFiles.Push($" {currentFile}, Error: {knex.Message}");
                }
                catch (Exception ex)
                {
                    failedFiles.Push($" {currentFile}, Error: {ex.Message}");
                }
            }
            if(failedFiles.Any())
            {
                Console.WriteLine("Following files are corrupted, not processed.");
                while(failedFiles.Any())
                {
                    Console.WriteLine(failedFiles.Pop());
                }
            }
        }
    }
}
