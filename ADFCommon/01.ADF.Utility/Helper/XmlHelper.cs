using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace ADF.Utility
{
    public class XmlHelper
    {
        private XmlDocument doc;
        private XmlElement root;

        public bool IsEmpty => doc.HasChildNodes || root == null;

        public XmlHelper(Stream stream)
        {
            doc = new XmlDocument();
            if (stream != null)
                doc.Load(stream);
            root = doc.DocumentElement;
        }

        /// <summary>
        /// 获取指定XPath表达式的节点对象
        /// </summary>        
        /// <param name="xPath">XPath表达式,
        /// 范例1: @"Skill/First/SkillItem", 等效于 @"//Skill/First/SkillItem"
        /// 范例2: @"Table[USERNAME='a']" , []表示筛选,USERNAME是Table下的一个子节点.
        /// 范例3: @"ApplyPost/Item[@itemName='岗位编号']",@itemName是Item节点的属性.
        /// </param>
        public XmlNode GetNode(string xPath)
        {
            //返回XPath节点
            return root.SelectSingleNode(xPath);
        }

        public void AppendDeclaration()
        {
            XmlDeclaration xmldecl = doc.CreateXmlDeclaration("1.0", "gb2312", null);
            doc.AppendChild(xmldecl);
        }

        public XmlElement AppendRootNode(string nodeName)
        {
            XmlElement element = doc.CreateElement(nodeName);
            doc.AppendChild(element);
            return element;
        }

        public void AppendNode(string xPath, string nodeName, string text = "", List<SelectOption> properties = null)
        {
            XmlNode currentNode = root.SelectSingleNode(xPath);
            XmlElement element = doc.CreateElement(nodeName);
            element.InnerText = text;
            properties.ForEach(p => element.SetAttribute(p.name, p.value));
            currentNode.AppendChild(element);
        }
    }
}