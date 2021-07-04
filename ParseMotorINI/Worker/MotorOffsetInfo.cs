using ParseMotorINI.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParseMotorINI.Worker
{
    public class MotorOffsetInfo
    {
        static INIParser parser = new INIParser();
        public static string GetCurrentPackageName()
        {
            parser.iniPath = SystemPath.systemIniPath;
            return $"{SystemPath.packageFolder}\\{ parser.IniReadValue("MachineData", "CurrentPackage")}.pkg";
        }
        public static IEnumerable<string> FilterMotorSectionNames(List<string> sections)
        {
            Regex pattern = new Regex("Motor\\d\\d?", RegexOptions.IgnoreCase);
            return sections.Where(x => pattern.IsMatch(x));
        }
        public static bool IsLocationName(string section)
        {
            Regex pattern = new Regex(@"Location\d\d?Offset", RegexOptions.IgnoreCase);
            return pattern.IsMatch(section);
        }
        public static List<string> GetSystemMotorSections() =>  SystemMotorOffsetPair.Keys.ToList();
        public static List<string> GetSystemMotorOffsetLocations(string motorSection)
        {
            if(SystemMotorOffsetPair.ContainsKey(motorSection))
                return SystemMotorOffsetPair[motorSection];
            return new List<string>() { };
        }
        public static void LoadMotorOffsetValues(string path, List<string> sections, Dictionary<string, Dictionary<string, string>> pair)
        {
            parser.iniPath = path;
            foreach (var section in sections)
            {
                pair.Add(section, parser.IniReadSection(section));
            }
        }

        public static void WriteAllTotalOffsetToPackage(Dictionary<string, Dictionary<string, string>> system, Dictionary<string, Dictionary<string, string>> device , string packagePath)
        {
            var totalSections = device.Keys.Union(GetSystemMotorSections());
            var totalLocation = new List<string>();
            foreach (var section in totalSections)
            {
                totalLocation = device[section].Keys.Where(x => IsLocationName(x))
                .Union(GetSystemMotorOffsetLocations(section)).OrderBy(x=>x).ToList();
                
                foreach(var loc in totalLocation)
                {
                    WriteTotalOffsetToConfig(system, device, section, loc, packagePath);
                }
            }

        }
        public static void WritePartialTotalOffsetToSystem(Dictionary<string, Dictionary<string, string>> system, Dictionary<string, Dictionary<string, string>> device)
        {
           
            var totalSections = GetSystemMotorSections();
            var totalLocation = new List<string>();
            foreach (var section in totalSections)
            {
                totalLocation = GetSystemMotorOffsetLocations(section);
                
                foreach(var loc in totalLocation)
                {
                    WriteTotalOffsetToConfig(system, device, section, loc, SystemPath.systemIniPath);
                }
            }
        }
        public static void WritePartialTotalOffsetToPackage(Dictionary<string, Dictionary<string, string>> system, Dictionary<string, Dictionary<string, string>> device, string packagePath)
        {
            var totalSections = device.Keys.Union(GetSystemMotorSections());
            var totalLocation = new List<string>();
            foreach (var section in totalSections)
            {
                totalLocation = device[section].Keys.Where(x => IsLocationName(x))
                .Except(GetSystemMotorOffsetLocations(section)).OrderBy(x => x).ToList();

                foreach (var loc in totalLocation)
                {
                    WriteTotalOffsetToConfig(system, device, section, loc, packagePath);
                }
            }
        }
        static void WriteTotalOffsetToConfig(Dictionary<string, Dictionary<string, string>> system, Dictionary<string, Dictionary<string, string>> device, string section, string loc, string path)
        {
            parser.iniPath = path;
            string readOffset;
            double number1, number2, totalMotorOffset = 0.0;
            if (!IsLocationName(loc))
                return;
            //if (!(system[section].TryGetValue(loc, out readOffset) && Double.TryParse(readOffset, out number1)))
            if (!(system.ContainsKey(section) && system[section].TryGetValue(loc, out readOffset) && Double.TryParse(readOffset, out number1)))
            {
                number1 = 0.0;
                //Console.WriteLine($"System has no {section} => {loc}");
            }
            if (!(device[section].TryGetValue(loc, out readOffset) && Double.TryParse(readOffset, out number2)))
            {
                number2 = 0.0;
                //Console.WriteLine( $"Device has no {section} => {loc}");
            }
            totalMotorOffset = number1 + number2;
            parser.IniWriteValue(section, loc, $"{totalMotorOffset:F2}");
        }
        static readonly Dictionary<string, List<string>> SystemMotorOffsetPair = new Dictionary<string, List<string>>()
        {
            {
                "Motor1", new List<string>{
                    "Location1Offset",
                    "Location2Offset",
                    "Location3Offset",
                    "Location4Offset",
                    "Location5Offset", }
            },
            {
                "Motor2", new List<string>{
                    "Location1Offset",
                    "Location2Offset",
                    "Location3Offset",
                    "Location4Offset",
                    "Location5Offset", }
            },
            {
                "Motor5", new List<string>{
                    "Location1Offset",
                    "Location3Offset", }
            },
            {
                "Motor7", new List<string>{
                    "Location4Offset",
                    "Location5Offset",
                    "Location6Offset", }
            },
            {
                "Motor8", new List<string>{
                    "Location4Offset",
                    "Location5Offset",
                    "Location6Offset", }
            },
            {
                "Motor9", new List<string>{
                    "Location1Offset",
                    "Location2Offset",
                    "Location3Offset",
                    "Location4Offset",
                    "Location5Offset",
                    "Location6Offset", }
            },
            {
                "Motor10", new List<string>{
                    "Location1Offset",
                    "Location3Offset", }
            },
            {
                "Motor13", new List<string>{
                    "Location1Offset",
                    "Location2Offset",
                    "Location3Offset",
                    "Location4Offset",
                    "Location5Offset",
                    "Location6Offset",
                    "Location7Offset",
                    "Location8Offset",
                    "Location11Offset", }
            },
            {
                "Motor14", new List<string>{
                    "Location1Offset",
                    "Location2Offset",
                    "Location3Offset",
                    "Location4Offset",
                    "Location5Offset",
                    "Location6Offset",
                    "Location7Offset",
                    "Location8Offset",
                    "Location11Offset", }
            },
        };
    }
}
