/*******************************************************************************
 * Copyright (c) 2017 Association Cénotélie (cenotelie.fr)
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation, either version 3
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General
 * Public License along with this program.
 * If not, see <http://www.gnu.org/licenses/>.
 ******************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Hime.SDK
{
	/// <summary>
	/// Contains a set of helper methods for the support of Unicode
	/// </summary>
	/// <remarks>
	/// The current supported Unicode version is 6.3.0
	/// </remarks>
	public static class UnicodeHelper
	{
		/// <summary>
		/// The URL of the latest specification of Unicode blocks
		/// </summary>
		private const string URL_UNICODE_BLOCKS = "http://www.unicode.org/Public/UCD/latest/ucd/Blocks.txt";
		/// <summary>
		/// The URL of the latest specification of Unicode code points
		/// </summary>
		private const string URL_UNICODE_DATA = "http://www.unicode.org/Public/UCD/latest/ucd/UnicodeData.txt";

		/// <summary>
		/// Gets the latest unicode blocks from the Unicode web site
		/// </summary>
		/// <returns>
		/// The latest unicode blocks
		/// </returns>
		public static ICollection<UnicodeBlock> GetLatestBlocks()
		{
			WebClient client = new WebClient();
			byte[] buffer = client.DownloadData(URL_UNICODE_BLOCKS);
			string content = Encoding.UTF8.GetString(buffer);
			string[] lines = content.Split('\n');
			Regex exp = new Regex("(?<begin>[0-9A-F]+)\\.\\.(?<end>[0-9A-F]+);\\s+(?<name>(\\w|\\s|-)+)");
			List<UnicodeBlock> blocks = new List<UnicodeBlock>();
			foreach (string line in lines)
			{
				if (line.Length == 0)
					continue;
				if (line.StartsWith("#"))
					continue;
				Match m = exp.Match(line);
				if (!m.Success)
					continue;
				int begin = Convert.ToInt32(m.Groups["begin"].Value, 16);
				int end = Convert.ToInt32(m.Groups["end"].Value, 16);
				string name = m.Groups["name"].Value;
				// filter out the Surrogate-related blocks
				if (name.Contains("Surrogate"))
					continue;
				name = name.Replace(" ", "");
				blocks.Add(new UnicodeBlock(name, begin, end));
			}
			return blocks;
		}

		/// <summary>
		/// Gets the latest unicode categories from the Unicode web site
		/// </summary>
		/// <returns>
		/// The latest unicode categories
		/// </returns>
		public static ICollection<UnicodeCategory> GetLatestCategories()
		{
			WebClient client = new WebClient();
			byte[] buffer = client.DownloadData(URL_UNICODE_DATA);
			string content = Encoding.UTF8.GetString(buffer);
			string[] lines = content.Split('\n');
			Regex exp = new Regex("(?<code>[0-9A-F]+);([^;]+);(?<cat>[^;]+);.*");
			Dictionary<string, UnicodeCategory> categories = new Dictionary<string, UnicodeCategory>();
			string currentName = null;
			int currentSpanBegin = -1;
			int lastCP = -1;
			foreach (string line in lines)
			{
				if (line.Length == 0)
					continue;
				Match m = exp.Match(line);
				if (!m.Success)
					continue;
				int cp = Convert.ToInt32(m.Groups["code"].Value, 16);
				string cat = m.Groups["cat"].Value;
				if (cat == currentName)
				{
					lastCP = cp;
				}
				else
				{
					if (currentName != null)
					{
						if (!categories.ContainsKey(currentName))
							categories.Add(currentName, new UnicodeCategory(currentName));
						if ((currentSpanBegin < 0xD800 || currentSpanBegin >= 0xE000) && (lastCP < 0xD800 || lastCP >= 0xE000))
							categories[currentName].AddSpan(currentSpanBegin, lastCP);
					}
					currentName = cat;
					currentSpanBegin = cp;
					lastCP = cp;
				}
			}
			categories[currentName].AddSpan(currentSpanBegin, lastCP);
			return categories.Values;
		}

		/// <summary>
		/// Generates the code for the Unicode blocks data
		/// </summary>
		public static void GenerateBlocksDB()
		{
			ICollection<UnicodeBlock> blocks = GetLatestBlocks();

			StreamWriter writer = new StreamWriter("UnicodeBlocks.cs", false, new UTF8Encoding(false));
			writer.WriteLine("/*");
			writer.WriteLine(" * WARNING: this file has been generated by");
			writer.WriteLine(" * Hime Parser Generator");
			writer.WriteLine(" */");
			writer.WriteLine();
			writer.WriteLine("using System.Collections.Generic;");
			writer.WriteLine();
			writer.WriteLine("namespace Hime.SDK");
			writer.WriteLine("{");
			writer.WriteLine("\t/// <summary>");
			writer.WriteLine("\t/// Contains the supported Unicode blocks");
			writer.WriteLine("\t/// </summary>");
			writer.WriteLine("\tpublic static class UnicodeBlocks");
			writer.WriteLine("\t{");
			foreach (UnicodeBlock block in blocks)
			{
				string csName = block.Name.Replace("-", "");
				writer.WriteLine("\t\t/// <summary>");
				writer.WriteLine("\t\t/// Gets the Unicode block " + block.Name);
				writer.WriteLine("\t\t/// </summary>");
				writer.WriteLine("\t\tpublic static UnicodeBlock " + csName + " { get { return block" + csName + "; } }");
				writer.WriteLine("\t\tprivate static readonly UnicodeBlock block" + csName + " = new UnicodeBlock(\"" + block.Name + "\", 0x" + block.Span.Begin.Value.ToString("X") + ", 0x" + block.Span.End.Value.ToString("X") + ");");
			}

			writer.WriteLine();
			writer.WriteLine("\t\t/// <summary>");
			writer.WriteLine("\t\t/// The database of Unicode blocks accesible by names");
			writer.WriteLine("\t\t/// </summary>");
			writer.WriteLine("\t\tprivate static Dictionary<string, UnicodeBlock> db;");

			writer.WriteLine("\t\t/// <summary>");
			writer.WriteLine("\t\t/// Builds the blocks database");
			writer.WriteLine("\t\t/// </summary>");
			writer.WriteLine("\t\tprivate static void BuildDB()");
			writer.WriteLine("\t\t{");
			writer.WriteLine("\t\t\tdb = new Dictionary<string, UnicodeBlock>();");
			foreach (UnicodeBlock block in blocks)
			{
				string csName = block.Name.Replace("-", "");
				writer.WriteLine("\t\t\tdb.Add(\"" + block.Name + "\", " + csName + ");");
			}
			writer.WriteLine("\t\t}");

			writer.WriteLine("\t\t/// <summary>");
			writer.WriteLine("\t\t/// Gets the block with the given name");
			writer.WriteLine("\t\t/// </summary>");
			writer.WriteLine("\t\t/// <param name=\"name\">A Unicode block name</param>");
			writer.WriteLine("\t\t/// <returns>The corresponding block, or null if it does not exists</returns>");
			writer.WriteLine("\t\tpublic static UnicodeBlock GetBlock(string name)");
			writer.WriteLine("\t\t{");
			writer.WriteLine("\t\t\tif (db == null) BuildDB();");
			writer.WriteLine("\t\t\treturn !db.ContainsKey(name) ? null : db[name];");
			writer.WriteLine("\t\t}");

			writer.WriteLine("\t}");
			writer.WriteLine("}");
			writer.Close();
		}

		/// <summary>
		/// Generates the parsing tests for the unicode blocks
		/// </summary>
		public static void GenerateBlocksTests()
		{
			ICollection<UnicodeBlock> blocks = GetLatestBlocks();

			StreamWriter writer = new StreamWriter("UnicodeBlocks.suite", false, new UTF8Encoding(false));
			writer.WriteLine("fixture UnicodeBlocks");
			foreach (UnicodeBlock block in blocks)
			{
				string csName = block.Name.Replace("-", "");

				string values;
				if (block.Span.Begin.Value <= 0xFFFF)
					values = "\\u" + block.Span.Begin.Value.ToString("X4");
				else
					values = "\\u" + block.Span.Begin.Value.ToString("X8");
				writer.WriteLine();
				writer.WriteLine("test Test_UnicodeBlock_" + csName + "_LeftBound:");
				writer.WriteLine("\tgrammar Test_UnicodeBlock_" + csName + "_LeftBound { options {Axiom=\"e\";} terminals {X->ub{" + block.Name + "};} rules { e->X; } }");
				writer.WriteLine("\tparser LALR1");
				writer.WriteLine("\ton \"" + values + "\"");
				writer.WriteLine("\tyields e(X='" + values + "')");

				if (block.Span.Begin.Value <= 0xFFFF)
					values = "\\u" + block.Span.End.Value.ToString("X4");
				else
					values = "\\u" + block.Span.End.Value.ToString("X8");
				writer.WriteLine();
				writer.WriteLine("test Test_UnicodeBlock_" + csName + "_RightBound:");
				writer.WriteLine("\tgrammar Test_UnicodeBlock_" + csName + "_RightBound { options {Axiom=\"e\";} terminals {X->ub{" + block.Name + "};} rules { e->X; } }");
				writer.WriteLine("\tparser LALR1");
				writer.WriteLine("\ton \"" + values + "\"");
				writer.WriteLine("\tyields e(X='" + values + "')");
			}
			writer.Close();
		}

		/// <summary>
		/// Generates the code for the Unicode categories data
		/// </summary>
		public static void GenerateCategoriesDB()
		{
			ICollection<UnicodeCategory> temp = GetLatestCategories();
			Dictionary<string, UnicodeCategory> categories = new Dictionary<string, UnicodeCategory>();
			Dictionary<string, List<UnicodeCategory>> aggregated = new Dictionary<string, List<UnicodeCategory>>();
			foreach (UnicodeCategory cat in temp)
			{
				categories.Add(cat.Name, cat);
				string aggregator = cat.Name[0].ToString();
				if (!aggregated.ContainsKey(aggregator))
					aggregated.Add(aggregator, new List<UnicodeCategory>());
				aggregated[aggregator].Add(cat);
			}

			StreamWriter writer = new StreamWriter("UnicodeCategories.cs", false, new UTF8Encoding(false));
			writer.WriteLine("/*");
			writer.WriteLine(" * WARNING: this file has been generated by");
			writer.WriteLine(" * Hime Parser Generator");
			writer.WriteLine(" */");
			writer.WriteLine();
			writer.WriteLine("using System.Collections.Generic;");
			writer.WriteLine();
			writer.WriteLine("namespace Hime.SDK");
			writer.WriteLine("{");
			writer.WriteLine("\t/// <summary>");
			writer.WriteLine("\t/// Contains the supported Unicode categories");
			writer.WriteLine("\t/// </summary>");
			writer.WriteLine("\tpublic static class UnicodeCategories");
			writer.WriteLine("\t{");
			foreach (UnicodeCategory category in categories.Values)
			{
				writer.WriteLine("\t\t/// <summary>");
				writer.WriteLine("\t\t/// Gets the Unicode category " + category.Name);
				writer.WriteLine("\t\t/// </summary>");
				writer.WriteLine("\t\tpublic static UnicodeCategory " + category.Name);
				writer.WriteLine("\t\t{");
				writer.WriteLine("\t\t\tget");
				writer.WriteLine("\t\t\t{");
				writer.WriteLine("\t\t\t\tif (cat" + category.Name + " == null) BuildCategory" + category.Name + "();");
				writer.WriteLine("\t\t\t\treturn cat" + category.Name + ";");
				writer.WriteLine("\t\t\t}");
				writer.WriteLine("\t\t}");
				writer.WriteLine("\t\tprivate static UnicodeCategory cat" + category.Name + ";");
				writer.WriteLine("\t\tprivate static void BuildCategory" + category.Name + "()");
				writer.WriteLine("\t\t{");
				writer.WriteLine("\t\t\tcat" + category.Name + " = new UnicodeCategory(\"" + category.Name + "\");");
				foreach (UnicodeSpan span in category.Spans)
					writer.WriteLine("\t\t\tcat" + category.Name + ".AddSpan(0x" + span.Begin.Value.ToString("X") + ", 0x" + span.End.Value.ToString("X") + ");");
				writer.WriteLine("\t\t}");
			}
			foreach (string name in aggregated.Keys)
			{
				writer.WriteLine("\t\t/// <summary>");
				writer.WriteLine("\t\t/// Gets the Unicode category " + name);
				writer.WriteLine("\t\t/// </summary>");
				writer.WriteLine("\t\tpublic static UnicodeCategory " + name);
				writer.WriteLine("\t\t{");
				writer.WriteLine("\t\t\tget");
				writer.WriteLine("\t\t\t{");
				writer.WriteLine("\t\t\t\tif (cat" + name + " == null) BuildCategory" + name + "();");
				writer.WriteLine("\t\t\t\treturn cat" + name + ";");
				writer.WriteLine("\t\t\t}");
				writer.WriteLine("\t\t}");
				writer.WriteLine("\t\tprivate static UnicodeCategory cat" + name + ";");
				writer.WriteLine("\t\tprivate static void BuildCategory" + name + "()");
				writer.WriteLine("\t\t{");
				writer.WriteLine("\t\t\tcat" + name + " = new UnicodeCategory(\"" + name + "\");");
				foreach (UnicodeCategory sub in aggregated[name])
					writer.WriteLine("\t\t\tcat" + name + ".Aggregate(" + sub.Name + ");");
				writer.WriteLine("\t\t}");
			}

			writer.WriteLine();
			writer.WriteLine("\t\t/// <summary>");
			writer.WriteLine("\t\t/// The database of Unicode categories accesible by names");
			writer.WriteLine("\t\t/// </summary>");
			writer.WriteLine("\t\tprivate static Dictionary<string, UnicodeCategory> db;");

			writer.WriteLine("\t\t/// <summary>");
			writer.WriteLine("\t\t/// Builds the category database");
			writer.WriteLine("\t\t/// </summary>");
			writer.WriteLine("\t\tprivate static void BuildDB()");
			writer.WriteLine("\t\t{");
			writer.WriteLine("\t\t\tdb = new Dictionary<string, UnicodeCategory>();");
			foreach (UnicodeCategory category in categories.Values)
				writer.WriteLine("\t\t\tdb.Add(\"" + category.Name + "\", " + category.Name + ");");
			foreach (string name in aggregated.Keys)
				writer.WriteLine("\t\t\tdb.Add(\"" + name + "\", " + name + ");");
			writer.WriteLine("\t\t}");

			writer.WriteLine("\t\t/// <summary>");
			writer.WriteLine("\t\t/// Gets the category with the given name");
			writer.WriteLine("\t\t/// </summary>");
			writer.WriteLine("\t\t/// <param name=\"name\">A Unicode category name</param>");
			writer.WriteLine("\t\t/// <returns>The corresponding category, or null if it does not exists</returns>");
			writer.WriteLine("\t\tpublic static UnicodeCategory GetCategory(string name)");
			writer.WriteLine("\t\t{");
			writer.WriteLine("\t\t\tif (db == null) BuildDB();");
			writer.WriteLine("\t\t\treturn !db.ContainsKey(name) ? null : db[name];");
			writer.WriteLine("\t\t}");

			writer.WriteLine("\t}");
			writer.WriteLine("}");
			writer.Close();
		}
	}
}