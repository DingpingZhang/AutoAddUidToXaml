using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutoAddUidToXaml
{
    public class XamlUidHelper
    {
        private List<string> _existUid = new List<string>();

        public void ParseXaml(string filePath)
        {
            if (!(File.Exists(filePath) && new Regex(".xaml$").IsMatch(filePath)))
                throw new FileNotFoundException();

            List<string> xamlLineList = this.ChangeTextToLines(File.ReadAllText(filePath));
            List<int> xamlMarkHeadIndexList = this.FetchMarkHeadIndex(xamlLineList);

            for (int i = 0; i < xamlMarkHeadIndexList.Count; i++)
            {
                int index = xamlMarkHeadIndexList[i];
                string uid = this.GenerateUid(xamlLineList[index]);
                string temp = xamlLineList[index];
                if (temp.Length > 2 && this.IsNeedToAddUid(xamlLineList[index]))
                {
                    int insertIndex = temp.Length - 1;
                    if (temp[temp.Length - 2] == '/') { insertIndex--; }
                    xamlLineList[index] = temp.Insert(insertIndex, " x:Uid=\"" + uid + "\"");
                }
            }
            File.WriteAllLines(filePath.Replace(".xaml", "_AddedUid.xaml"), xamlLineList);
        }

        private List<string> ChangeTextToLines(string xamlText)
        {
            var rMatchXmlMark = new Regex(" *?<.+?>", RegexOptions.Singleline);
            var mMatchXmlMark = rMatchXmlMark.Matches(xamlText);
            List<string> xamlLineList = new List<string>();
            for (int i = 0; i < mMatchXmlMark.Count; i++)
            {
                xamlLineList.Add(mMatchXmlMark[i].Value);
            }
            return xamlLineList;
        }

        private List<int> FetchMarkHeadIndex(List<string> xamlLines)
        {
            var rMatchMarkHead = new Regex("^ *<[^/!].+?/?>", RegexOptions.Singleline);
            var markHeadIndexList = new List<int>();
            for (int i = 0; i < xamlLines.Count; i++)
            {
                if (rMatchMarkHead.IsMatch(xamlLines[i]))
                    markHeadIndexList.Add(i);
            }
            return markHeadIndexList;
        }

        private string GenerateUid(string markHead)
        {
            string temp = markHead.Split(new string[] { " ", "<", "/>", ">", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)[0] + "_";
            temp = temp.Replace(".", "_").Replace(":", "_");
            string uid = temp + "1";
            for (int i = 2; this._existUid.Contains(uid); i++)
            {
                uid = temp + i;
            }
            this._existUid.Add(uid);
            return uid;
        }

        private bool IsNeedToAddUid(string markHead)
        {
            return true;
        }
    }
}
