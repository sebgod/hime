﻿/*
 * Author: Laurent Wouters
 * Date: 14/09/2011
 * Time: 17:22
 * 
 */
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Hime.Kernel.Resources
{
	// TODO: why is this class needed? Think about it
    public class ResourceAccessor
	{
        private static List<ResourceAccessor> accessors = new List<ResourceAccessor>();

		private Assembly assembly;
		private string rootNamespace;
		private string defaultPath;
        private List<string> files;
        private List<System.IO.Stream> streams;
        private bool isClosed;

        public bool IsOpen { get { return !isClosed; } }
        public bool IsClosed { get { return isClosed; } }
        public ICollection<string> Files { get { return files; } }

        public ResourceAccessor()
            : this(Assembly.GetExecutingAssembly(), "Hime.Resources")
        { }
		
        public ResourceAccessor(Assembly assembly, string defaultPath)
        {
            accessors.Add(this);
            this.assembly = assembly;
            this.rootNamespace = assembly.GetName().Name;
			// TODO: should simplify this!! In particular remove this first case
            if (defaultPath == null || defaultPath == "")
                this.defaultPath = rootNamespace + ".";
            else
                this.defaultPath = rootNamespace + "." + defaultPath + ".";
            this.files = new List<string>();
            this.streams = new List<System.IO.Stream>();
            this.isClosed = false;
        }

        public void Close()
        {
            foreach (string file in files)
                System.IO.File.Delete(file);
            foreach (System.IO.Stream stream in streams)
                stream.Close();
            isClosed = true;
            accessors.Remove(this);
        }

        public void AddCheckoutFile(string fileName)
        {
            if (isClosed)
                throw new AccessorClosedException(this);
            files.Add(fileName);
        }

        public void CheckOut(string resourceName, string fileName)
        {
			Export(resourceName, fileName);
            files.Add(fileName);
        }
		
		private byte[] ReadResource(string resourceName)
		{
            if (isClosed) throw new AccessorClosedException(this);
			string resourcePath = defaultPath + resourceName;
            Stream stream = this.assembly.GetManifestResourceStream(resourcePath);
			if (stream == null) throw new ResourceNotFoundException(resourcePath);
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
			return buffer;
		}
		
        public void Export(string resourceName, string fileName)
        {
            byte[] buffer = this.ReadResource(resourceName);
            File.WriteAllBytes(fileName, buffer);
        }

        public Stream GetStreamFor(string resourceName)
        {
			// TODO: could factor this with ReadResource
            if (isClosed) throw new AccessorClosedException(this);
            Stream stream = assembly.GetManifestResourceStream(defaultPath + resourceName);
            if (stream == null) throw new ResourceNotFoundException(resourceName);
            streams.Add(stream);
            return stream;
        }

        public string GetAllTextFor(string resourceName)
		{
			byte[] buffer = ReadResource(resourceName);
            // Detect encoding and strip encoding preambule
            Encoding encoding = DetectEncoding(buffer);
            buffer = StripPreambule(buffer, encoding);
            // Return decoded text
			return new string(Encoding.UTF8.GetChars(buffer));
		}
        
        private static Encoding DetectEncoding(byte[] buffer)
        {
            if (DetectEncoding_TryEncoding(buffer, Encoding.UTF8))
                return Encoding.UTF8;
            if (DetectEncoding_TryEncoding(buffer, Encoding.Unicode))
                return Encoding.Unicode;
            if (DetectEncoding_TryEncoding(buffer, Encoding.BigEndianUnicode))
                return Encoding.BigEndianUnicode;
            if (DetectEncoding_TryEncoding(buffer, Encoding.UTF32))
                return Encoding.UTF32;
            if (DetectEncoding_TryEncoding(buffer, Encoding.ASCII))
                return Encoding.ASCII;
            return Encoding.Default;
        }
        private static bool DetectEncoding_TryEncoding(byte[] buffer, Encoding encoding)
        {
            byte[] preambule = encoding.GetPreamble();
            if (buffer.Length < preambule.Length) return false;
            for (int i = 0; i < preambule.Length; i++)
            {
                if (buffer[i] != preambule[i])
                    return false;
            }
            return true;
        }
        private static byte[] StripPreambule(byte[] buffer, System.Text.Encoding encoding)
        {
            byte[] preambule = encoding.GetPreamble();
            if (preambule.Length == 0)
                return buffer;
            byte[] newbuffer = new byte[buffer.Length - preambule.Length];
            System.Array.Copy(buffer, preambule.Length, newbuffer, 0, newbuffer.Length);
            return newbuffer;
        }
	}
}
