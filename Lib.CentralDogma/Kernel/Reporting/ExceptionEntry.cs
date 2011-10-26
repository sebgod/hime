﻿/*
 * Author: Laurent Wouters
 * Date: 14/09/2011
 * Time: 17:22
 * 
 */
using System;
using System.Xml;

namespace Hime.Kernel.Reporting
{
    public class ExceptionEntry : IEntry
    {
        private Exception exception;

        public ELevel Level { get { return ELevel.Error; } }
        public string Component { get { return "Compiler"; } }
        public string Message { get { return "Exception " + exception.Message; } }

        public ExceptionEntry(System.Exception ex) { exception = ex; }

        public XmlNode GetMessageNode(XmlDocument doc)
        {
            XmlNode element = doc.CreateElement("Exception");
            element.Attributes.Append(doc.CreateAttribute("EID"));
            element.Attributes["EID"].Value = exception.GetHashCode().ToString();
            XmlNode message = doc.CreateElement("Message");
            message.InnerText = Message;
            element.AppendChild(message);
            XmlNode method = doc.CreateElement("Method");
            method.InnerText = exception.TargetSite.ToString();
            element.AppendChild(method);
            XmlNode stack = doc.CreateElement("Stack");
            string data = exception.StackTrace;
            string[] lines = data.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                XmlNode nl = doc.CreateElement("Line");
                nl.InnerText = line;
                stack.AppendChild(nl);
            }
            element.AppendChild(stack);
            return element;
        }
    }
}