using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ParseMotorINI.Util
{
    public class INIParser
    {
        const uint MAX_BUFFER = 32767;
        public string iniPath { get; set; } = "";

        public long IniWriteValue(string Section, string Key, string Value)
        {
            return WritePrivateProfileString(Section, Key, Value, iniPath);
        }
        public string IniReadValue(string Section,string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, iniPath);
            return temp.ToString();
        }
        public long IniDeleteKey(string section, string key)
        {
            return WritePrivateProfileString(section, key, null, iniPath);
        }
        public bool IniWriteSection(string section, IEnumerable<KeyValuePair<string, string>> keyValues)
        {
            if (String.IsNullOrWhiteSpace(section))
                return false;
            return WritePrivateProfileSection(section, ToSectionValueString(keyValues), iniPath);
        }
        private string ToSectionValueString(IEnumerable<KeyValuePair<string, string>> keyValues)
        {
            if (keyValues == null)
                return null;
            return String.Join("\0", keyValues.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        }
        public Dictionary<string, string> IniReadSection(string section)
        {
            byte[] buffer = new byte[MAX_BUFFER];
            uint bytesReturned = GetPrivateProfileSection(section, buffer, MAX_BUFFER, iniPath);
            string allKeyValues = new string(Encoding.ASCII.GetChars(buffer), 0, (int)bytesReturned);
            return ToDictionary(allKeyValues);
        }
        static Dictionary<string, string> ToDictionary(string s)
        {
            return s.Split('\0').Select(x => x.Split('=')).Select(x =>
              { return x; }).Where(x=>x.Length==2).ToDictionary(x=>x[0], x=>x[1]);
        }
        public bool IniDeleteSection(string section)
        {
            return WritePrivateProfileSection(section, null, iniPath);
        }
        public List<string> IniGetSectionNames()
        {
            byte[] buffer = new byte[MAX_BUFFER];
            uint bytesReturned = GetPrivateProfileSectionNames(buffer, MAX_BUFFER, iniPath);
            return new string(Encoding.ASCII.GetChars(buffer), 0, (int)bytesReturned).Split('\0').ToList();
        }
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key,string val,string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key,string def, StringBuilder retVal, int size,string filePath);
        [DllImport("kernel32")]
        private static extern uint GetPrivateProfileSectionNames(byte[] pszReturnBuffer, uint nSize, string lpFileName);
        [DllImport("kernel32.dll")]
        private static extern bool WritePrivateProfileSection(string lpAppName,string lpString, string lpFileName);
        [DllImport("kernel32")]
        private static extern uint GetPrivateProfileSection(string lpAppName, byte[] lpReturnedString, uint nSize, string lpFileName);
    }
}
