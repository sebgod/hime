﻿/*
 * Author: Charles Hymans
 * Date: 17/07/2011
 * Time: 20:28
 * 
 */
using System;
using log4net;
using Hime.Kernel.Resources;
using System.Xml;
using System.Xml.Xsl;
using System.IO;

namespace Hime.Kernel.Reporting
{
    public class Reporter
    {
        protected Report report;
        protected Section topSection;
        protected Section currentSection;
        protected ILog log;

        public Report Result { get { return report; } }

        private static bool configured = false;
        private static void Configure()
        {
            if (configured) return;
            log4net.Layout.PatternLayout layout = new log4net.Layout.PatternLayout("%-5p: %m%n");
            log4net.Appender.ConsoleAppender appender = new log4net.Appender.ConsoleAppender();
            appender.Layout = layout;
            log4net.Config.BasicConfigurator.Configure(appender);
            configured = true;
        }

        public Reporter()
        {
            Configure();
            log = log4net.LogManager.GetLogger(typeof(Hime.Parsers.Compiler));
            report = new Report();
        }

        public void BeginSection(string name) { currentSection = report.AddSection(name); }
        public void EndSection() { currentSection = null; }

        public void Info(string component, string message)
        {
            AddEntry(new BaseEntry(ELevel.Info, component, message));
            log.Info(component + ": " + message);
        }
        public void Warn(string component, string message)
        {
            AddEntry(new BaseEntry(ELevel.Warning, component, message));
            log.Warn(component + ": " + message);
        }
        public void Error(string component, string message)
        {
            AddEntry(new BaseEntry(ELevel.Error, component, message));
            log.Error(component + ": " + message);
        }
        public void Fatal(string component, string message)
        {
            AddEntry(new BaseEntry(ELevel.Error, component, message));
            log.Fatal(component + ": " + message);
        }
        public void Report(IEntry entry)
        {
            AddEntry(entry);
            switch (entry.Level)
            {
                case ELevel.Info: log.Info(entry.Component + ": " + entry.Message); break;
                case ELevel.Warning: log.Warn(entry.Component + ": " + entry.Message); break;
                case ELevel.Error: log.Error(entry.Component + ": " + entry.Message); break;
            }
        }
        public void Report(System.Exception exception)
        {
            Report(new ExceptionEntry(exception));
        }

        protected void AddEntry(IEntry entry)
        {
			Section section = currentSection;
            if (section == null)
            {
				// TODO: shouldn't it be an invariant that it is never null?
                if (topSection == null)
                {
                    topSection = new Section("global");
                    report.Sections.Insert(0, topSection);
                }
                section = topSection;
            }
			section.AddEntry(entry);
        }

        public void ExportMHTML(string fileName, string title)
        {
            XmlDocument document = report.GetXML(title);
            FileInfo File = new FileInfo(fileName);
            document.ChildNodes[1].Attributes["title"].Value = title;
            document.Save(fileName + ".xml");

            using (ResourceAccessor session = new ResourceAccessor())
			{
	            session.AddCheckoutFile(fileName + ".xml");
    	        session.CheckOut("Transforms.Logs.xslt", File.DirectoryName + "Logs.xslt");

	            XslCompiledTransform Transform = new XslCompiledTransform();
    	        Transform.Load(File.DirectoryName + "Logs.xslt");
        	    Transform.Transform(fileName + ".xml", fileName + ".html");
            	session.AddCheckoutFile(fileName + ".html");

	            Kernel.Documentation.MHTMLCompiler compiler = new Kernel.Documentation.MHTMLCompiler();
    	        compiler.Title = title;
        	    compiler.AddSource(new Kernel.Documentation.MHTMLSourceFileText("text/html", "utf-8", "Grammar.html", fileName + ".html"));
            	compiler.AddSource(new Kernel.Documentation.MHTMLSourceStreamText("text/css", "utf-8", "hime_data/Logs.css", session.GetStreamFor("Transforms.Logs.css")));
            	compiler.AddSource(new Kernel.Documentation.MHTMLSourceStreamText("text/javascript", "utf-8", "hime_data/Hime.js", session.GetStreamFor("Transforms.Hime.js")));

            	compiler.AddSource(new Kernel.Documentation.MHTMLSourceStreamImage("image/gif", "hime_data/button_plus.gif", session.GetStreamFor("Visuals.button_plus.gif")));
            	compiler.AddSource(new Kernel.Documentation.MHTMLSourceStreamImage("image/gif", "hime_data/button_minus.gif", session.GetStreamFor("Visuals.button_minus.gif")));
            	compiler.AddSource(new Kernel.Documentation.MHTMLSourceStreamImage("image/png", "hime_data/Hime.Logo.png", session.GetStreamFor("Visuals.Hime.Logo.png")));
            	compiler.AddSource(new Kernel.Documentation.MHTMLSourceStreamImage("image/png", "hime_data/Hime.Info.png", session.GetStreamFor("Visuals.Hime.Info.png")));
            	compiler.AddSource(new Kernel.Documentation.MHTMLSourceStreamImage("image/png", "hime_data/Hime.Warning.png", session.GetStreamFor("Visuals.Hime.Warning.png")));
            	compiler.AddSource(new Kernel.Documentation.MHTMLSourceStreamImage("image/png", "hime_data/Hime.Error.png", session.GetStreamFor("Visuals.Hime.Error.png")));
            	compiler.CompileTo(fileName);
			}
        }
    }
}
