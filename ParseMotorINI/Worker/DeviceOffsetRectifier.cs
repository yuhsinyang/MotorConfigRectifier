using ParseMotorINI.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ParseMotorINI.Worker
{
    class DeviceOffsetRectifier
    {
        INIParser packageIniParser;
        public Dictionary<string, Dictionary<string, string>> deviceMotorOffsetValuePair; 
        public DeviceOffsetRectifier()
        {
            packageIniParser = new INIParser();
            deviceMotorOffsetValuePair = new Dictionary<string, Dictionary<string, string>>();
        }
        public void LoadMotorOffsetValues(string path)
        {
            deviceMotorOffsetValuePair.Clear();
            packageIniParser.iniPath = path;
            var allMotorSections = MotorOffsetInfo.FilterMotorSectionNames(packageIniParser.IniGetSectionNames());
            MotorOffsetInfo.LoadMotorOffsetValues(path, allMotorSections.ToList(), deviceMotorOffsetValuePair);
        }
        public void ClearMotorOffsetValues() => deviceMotorOffsetValuePair.Clear();
    }
}
