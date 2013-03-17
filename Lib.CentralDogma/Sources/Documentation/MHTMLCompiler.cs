﻿/*
 * Author: Laurent Wouters
 * Date: 14/09/2011
 * Time: 17:22
 * 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hime.CentralDogma.Documentation
{
    class MHTMLCompiler
    {
		private const int linebreak = 76;
		private const string boundary = "----=NextPart";
        private const string generator = "Generated by Hime.CentralDogma";
		
		private List<MHTMLSource> sources;
        private string title;


        public MHTMLCompiler(string title)
        {
			this.title = title;
            sources = new List<MHTMLSource>();
        }

        public void AddSource(MHTMLSource source) 
		{ 
			sources.Add(source); 
		}

        public void CompileTo(string file)
        {
            using (StreamWriter writer = new StreamWriter(file, false, Encoding.UTF8))
			{
	            writer.Write("From: ");
    	        writer.WriteLine("\"" + generator + "\"");
        	    writer.Write("Subject: ");
            	writer.WriteLine(title);
	            writer.Write("Date: ");
    	        writer.WriteLine(DateTime.Now.ToLongDateString());
        	    writer.WriteLine("MIME-Version: 1.0");
            	writer.WriteLine("Content-Type: multipart/related;");
	            writer.WriteLine("\ttype=\"text/html\";");
    	        writer.WriteLine("\tboundary=\"" + boundary + "\"");

        	    foreach (MHTMLSource source in sources)
            	{
	                writer.WriteLine();
    	            writer.WriteLine();
        	        writer.WriteLine("--" + boundary);

					writer.Write(source.ToMHTML(linebreak));
            	}

	            writer.WriteLine();
    	        writer.WriteLine();
        	    writer.WriteLine("--" + boundary + "--");
            	writer.Close();
        	}
		}
    }
}