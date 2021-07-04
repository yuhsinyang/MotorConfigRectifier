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
        //public List<KeyValuePair<string, string>> IniReadSection(string section)
        //{
        //    byte[] buffer = new byte[MAX_BUFFER];
        //    uint bytesReturned = GetPrivateProfileSection(section, buffer, MAX_BUFFER, iniPath);
        //    string allKeyValues = new string(Encoding.ASCII.GetChars(buffer), 0, (int)bytesReturned);
        //    return ToKeyValueList(allKeyValues);
        //}
        //static List<KeyValuePair<string, string>> ToKeyValueList(string s)
        //{
        //    return s.Split('\0').Select(x => x.Split('=')).Select(x =>
        //      { return x; }).Where(x=>x.Length==2).Select(x => new KeyValuePair<string, string>(x[0], x[1])).ToList();
        //}
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
        //public string[] IniGetSectionNames()
        //{
        //    IntPtr pReturnedString = Marshal.AllocCoTaskMem((int)MAX_BUFFER);
        //    uint bytesReturned = GetPrivateProfileSectionNames(pReturnedString, MAX_BUFFER, iniPath);
        //    if (bytesReturned == 0)
        //        return null;
        //    string local = Marshal.PtrToStringAnsi(pReturnedString, (int)bytesReturned).ToString();
        //    Marshal.FreeCoTaskMem(pReturnedString);
        //    //use of Substring below removes terminating null for split
        //    return local.Substring(0, local.Length - 1).Split('\0');
        //}
        //public void IniGetSectionNames()
        //{
        //    byte[] buffer = new byte[MAX_BUFFER];
        //    uint bytesReturned = GetPrivateProfileSectionNames(buffer, MAX_BUFFER, iniPath);
        //    List<string> sections = new List<string>();
        //    var sectionName = new StringBuilder();
        //    for(int i = 0; i < bytesReturned; i++)
        //    {
        //        var c = (char)buffer[i];
        //        if(c!='\0')
        //        {
        //            sectionName.Append(c);
        //        }
        //        else
        //        {
        //            Console.WriteLine(sectionName);
        //            sectionName.Clear();
        //        }
        //    }

        //}
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key,string val,string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key,string def, StringBuilder retVal, int size,string filePath);
        //[DllImport("kernel32")]
        //private static extern uint GetPrivateProfileSectionNames(IntPtr pszReturnBuffer, uint nSize, string lpFileName);
        [DllImport("kernel32")]
        private static extern uint GetPrivateProfileSectionNames(byte[] pszReturnBuffer, uint nSize, string lpFileName);
        [DllImport("kernel32.dll")]
        private static extern bool WritePrivateProfileSection(string lpAppName,string lpString, string lpFileName);
        [DllImport("kernel32")]
        private static extern uint GetPrivateProfileSection(string lpAppName, byte[] lpReturnedString, uint nSize, string lpFileName);
    }
}
