using ParseMotorINI.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseMotorINI.Worker
{
    class SystemOffsetRectifier
    {
        INIParser machineIniParser;
        public Dictionary<string, Dictionary<string, string>> systemMotorOffsetValuePair;
        public SystemOffsetRectifier()
        {
            machineIniParser = new INIParser() { iniPath = SystemPath.systemIniPath } ;
            systemMotorOffsetValuePair = new Dictionary<string, Dictionary<string, string>>();
        }
        public bool ClearAllMotorOffsetsinConfig()
        {
            var allMotorSections = MotorOffsetInfo.FilterMotorSectionNames(machineIniParser.IniGetSectionNames());
            if (allMotorSections.Any())
            {
                foreach(var section in allMotorSections)
                    machineIniParser.IniDeleteSection(section);
            }
            return true;
        }
        public bool RectifyNumberofMotorOffsetinConfig()
        {
            var allMotorSections = MotorOffsetInfo.FilterMotorSectionNames(machineIniParser.IniGetSectionNames());

            var missingMotorSections = MotorOffsetInfo.GetSystemMotorSections().Except(allMotorSections);
            if(missingMotorSections.Any())
            {
                foreach (var section in missingMotorSections)
                    machineIniParser.IniWriteSection(section, MotorOffsetInfo.GetSystemMotorOffsetLocations(section).ToDictionary(x=>x, x=>"0.00"));
            }

            var unexpectedMotorSections = allMotorSections.Except(MotorOffsetInfo.GetSystemMotorSections());
            if(unexpectedMotorSections.Any())
            {
                foreach (var section in unexpectedMotorSections)
                    machineIniParser.IniDeleteSection(section);
            }
            return true;
        }
        public void ClearMotorOffsetValues() => systemMotorOffsetValuePair.Clear();
        public void LoadMotorOffsetValues()
        {
            var allMotorSections = MotorOffsetInfo.FilterMotorSectionNames(machineIniParser.IniGetSectionNames());
            MotorOffsetInfo.LoadMotorOffsetValues(SystemPath.systemIniPath, allMotorSections.ToList(), systemMotorOffsetValuePair);
        }
    }
}