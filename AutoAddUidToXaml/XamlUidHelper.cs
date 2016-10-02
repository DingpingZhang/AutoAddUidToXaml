using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
//using System.Xml.Linq;

namespace AutoAddUidToXaml
{
    public class XamlUidHelper
    {
        private List<string> _existUid = new List<string>();

        public void ParseXaml(string filePath)
        {
            if (!(File.Exists(filePath) && new Regex(".xaml$").IsMatch(filePath)))
                throw new FileNotFoundException();

            List<string> xamlLineList = ChangeTextToMarkList(File.ReadAllText(filePath));
            List<int> xamlMarkHeadIndexList = FetchMarkHeadIndex(xamlLineList);

            // 使用 Sysyem.Xml.Linq 检验正则匹配的准确性
            //List<XElement> xamlAllNodeList = this.FetchAllNodes(XElement.Load(filePath));
            //for (int i = 0; i < xamlAllNodeList.Count; i++)
            //{
            //    // --------------------------------------------------
            //    string xName = xamlLineList[xamlMarkHeadIndexList[i]].Split(new string[] { " ", "<", "/>", ">", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)[0];
            //    string[] t = xName.Split(new char[] { ':' });
            //    xName = t[t.Length - 1];
            //    //Console.WriteLine("{0:000} - {1}", i + 1, xName == xamlAllNodeList[i].Name.LocalName);
            //    // --------------------------------------------------
            //}

            for (int i = 0; i < xamlMarkHeadIndexList.Count; i++)
            {
                int index = xamlMarkHeadIndexList[i];
                string uid = GenerateUid(xamlLineList[index]);
                string markHead = xamlLineList[index];
                if (IsNeedToAddUid(markHead))
                {
                    int insertIndex = markHead.Length - 1;
                    if (markHead[markHead.Length - 2] == '/') { insertIndex--; }
                    xamlLineList[index] = markHead.Insert(insertIndex, " x:Uid=\"" + uid + "\"");
                }
            }
            string tempStr = string.Empty;
            for (int i = 0; i < xamlLineList.Count; i++)
            {
                tempStr += xamlLineList[i];
            }
            File.WriteAllText(filePath.Replace(".xaml", "_AddedUid.xaml"), tempStr);
        }

        /// <summary>
        /// 转换文本为xml标签集合（保留空格换行等格式）
        /// </summary>
        /// <param name="xamlText">Xaml文件文本</param>
        /// <returns>Xml标签集合</returns>
        private List<string> ChangeTextToMarkList(string xamlText)
        {
            var rMatchXmlMark = new Regex("[ \r\n]*<([^(!--)].+?|!--.*?--)>", RegexOptions.Singleline);
            var mMatchXmlMark = rMatchXmlMark.Matches(xamlText);
            List<string> xamlLineList = new List<string>();
            for (int i = 0; i < mMatchXmlMark.Count; i++)
            {
                xamlLineList.Add(mMatchXmlMark[i].Value);
            }
            return xamlLineList;
        }

        /// <summary>
        /// 从xml标签集合中提取标签开始标签（含不带关闭标签的独立标签）
        /// </summary>
        /// <param name="xamlMarkList">Xml标签集合</param>
        /// <returns>开始标签索引集合</returns>
        private List<int> FetchMarkHeadIndex(List<string> xamlMarkList)
        {
            var rMatchMarkHead = new Regex("^[ \r\n]*<[^/!].+?/?>", RegexOptions.Singleline);
            var markHeadIndexList = new List<int>();
            for (int i = 0; i < xamlMarkList.Count; i++)
            {
                if (rMatchMarkHead.IsMatch(xamlMarkList[i]))
                    markHeadIndexList.Add(i);
            }
            return markHeadIndexList;
        }

        /// <summary>
        /// 根据开始标签生成Uid名称
        /// </summary>
        /// <param name="markHead">开始标签</param>
        /// <returns>Uid名称</returns>
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

        /// <summary>
        /// 判断是否添加Uid（可加入规则：比如已有Name、Uid就不需要重复添加）
        /// </summary>
        /// <param name="markHead">Xaml开始标签</param>
        /// <returns>是否添加Uid</returns>
        private bool IsNeedToAddUid(string markHead)
        {
            return true;
        }

        //private List<XElement> FetchAllNodes(XElement xamlRoot)
        //{
        //    var xList = new List<XElement>();
        //    xList.Add(xamlRoot);
        //    this.SpreadXElement(xList, xamlRoot);
        //    return xList;
        //}
        //private void SpreadXElement(List<XElement> xList, XElement root)
        //{
        //    foreach (var child in root.Elements())
        //    {
        //        xList.Add(child);
        //        this.SpreadXElement(xList, child);
        //    }
        //}
    }
}
