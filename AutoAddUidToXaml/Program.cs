using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutoAddUidToXaml
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = @"G:\Desktop\RigoriousLineSizingView.xaml";

            var addUid = new XamlUidHelper();
            addUid.ParseXaml(filePath);

            Console.WriteLine("* * * Complete! * * *");
            Console.ReadKey();
        }
    }
}
