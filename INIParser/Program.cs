using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INIParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var device1 = MotorOffsetGen.GetDeviceMotorOffsetPair();
            var system1 = MotorOffsetGen.GetSystemMotorOffsetPair();
            Console.WriteLine("Device");
            printSequence(device1);
            //Console.WriteLine("System");
            //printSequence(system1);

            Console.WriteLine("Remove motor 1");
            device1.Remove("Motor1");
            printSequence(device1);

            var device2 = MotorOffsetGen.GetMotorOffsetInfo();
            Console.WriteLine("Info");
            foreach(var k in device2.Keys)
                foreach(var v in device2[k])
                    Console.WriteLine(v);
            Console.ReadLine();
        }
        static void printSequence(Dictionary<string, Dictionary<string, string>> s)
        {
            foreach (var k in s.Keys)
                foreach(var v in s[k])
                    Console.WriteLine($"{v.Key} {v.Value}");
        }
    }

    class MotorOffsetGen
    {
        static Dictionary<string, Dictionary<string, string>> deviceOffsetParr;
        static Dictionary<string, Dictionary<string, string>> systemOffsetParr;
        static Dictionary<string, List<string>> motorOffsetPair; 

        static MotorOffsetGen()
        {
            motorOffsetPair = new Dictionary<string, List<string>>
            {
                { "Motor1", new List<string> { "Location11Offset", "Location22Offset", "Location33Offset" , "Location44Offset" , "Location55Offset" } },
            };
            deviceOffsetParr = new Dictionary<string, Dictionary<string, string>>
            {
                {
                    "Motor1",
                    new Dictionary<string, string>
                    {
                        {"Location1Offset", "0.0"},
                        {"Location2Offset", "0.0"},
                        {"Location3Offset", "0.0"},
                        {"Location4Offset", "0.0"},
                        {"Location5Offset", "0.0"},
                    }
                },
                {
                    "Motor2",
                    new Dictionary<string, string>
                    {
                        {"Location1Offset", "0.0"},
                        {"Location2Offset", "0.0"},
                        {"Location3Offset", "0.0"},
                        {"Location4Offset", "0.0"},
                        {"Location5Offset", "0.0"},
                    }
                },
            };
            systemOffsetParr = new Dictionary<string, Dictionary<string, string>>
            {
                {
                    "Motor1", motorOffsetPair["Motor1"].Select(x=> new Dictionary<string, string>(x, "0.0"));
                },
                {
                    "Motor2",
                    new Dictionary<string, string>
                    {
                        {"Location6Offset", "0.0"},
                        {"Location7Offset", "0.0"},
                        {"Location8Offset", "0.0"},
                        {"Location9Offset", "0.0"},
                        {"Location10Offset", "0.0"},
                    }
                },
            };
        }
        public static Dictionary<string, List<string>> GetMotorOffsetInfo()
        {
            return motorOffsetPair;
        }
        public static Dictionary<string, Dictionary<string, string>> GetDeviceMotorOffsetPair()
        {
            return deviceOffsetParr;
        }
        public static Dictionary<string, Dictionary<string, string>> GetSystemMotorOffsetPair()
        {
            return systemOffsetParr;
        }
    }
}
