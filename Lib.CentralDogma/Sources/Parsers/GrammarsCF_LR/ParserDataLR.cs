﻿/*
 * Author: Charles Hymans
 * Date: 06/08/2011
 * Time: 23:03
 * 
 */
using System.Xml;
using System.Xml.Xsl;
using System.Collections.Generic;
using System.IO;
using Hime.Utils.Reporting;
using Hime.Utils.Resources;
using Hime.Utils.Documentation;
using Hime.Utils.Graphs;

namespace Hime.Parsers.ContextFree.LR
{
    abstract class ParserDataLR : ParserData
    {
        protected Reporter reporter;
        protected CFGrammar grammar;
        protected Graph graph;
        protected List<Terminal> terminals;
        protected List<Variable> variables;
        protected List<Virtual> virtuals;
        protected List<Action> actions;
        protected List<Rule> rules;
        protected string terminalsAccessor;

        protected ICollection<Rule> GrammarRules { get { return rules; } }
        protected string IndexOfRule(Rule rule) 
		{
			return "0x" + rules.IndexOf(rule).ToString("X"); 
		}
		
        protected ICollection<Action> GrammarActions { get { return grammar.Actions; } }
        protected CFVariable GetVariable(string name) { return grammar.GetVariable(name) as CFVariable; }
        protected string GetOption(string name) { return this.grammar.GetOption(name); }
        
		abstract protected string GetBaseClassName { get; }
		
        public ParserDataLR(Reporter reporter, CFGrammar gram, Graph graph)
        {
            this.reporter = reporter;
            this.grammar = gram;
            this.graph = graph;
            this.variables = new List<Variable>(gram.Variables);
            this.virtuals = new List<Virtual>(gram.Virtuals);
            this.actions = new List<Action>(gram.Actions);
            this.rules = new List<Rule>(this.grammar.Rules);
        }

        public abstract void ExportData(BinaryWriter stream);

        protected void ExportDataProduction(BinaryWriter stream, Rule rule)
        {
            stream.Write((ushort)variables.IndexOf(rule.Head));
            if (rule.ReplaceOnProduction) stream.Write((byte)1);
            else stream.Write((byte)0);
            stream.Write((byte)(rule as CFRule).CFBody.GetChoiceAt(0).Length);
            byte length = 0;
            foreach (RuleBodyElement elem in rule.Body.Parts)
            {
                if (elem.Symbol is Virtual || elem.Symbol is Action)
                    length += 4;
                else
                    length += 2;
            }
            stream.Write(length);
            foreach (RuleBodyElement elem in rule.Body.Parts)
            {
                if (elem.Symbol is Virtual)
                {
                    if (elem.Action == RuleBodyElementAction.Drop) stream.Write((ushort)6);
                    else if (elem.Action == RuleBodyElementAction.Promote) stream.Write((ushort)7);
                    else stream.Write((ushort)4);
                    stream.Write((ushort)virtuals.IndexOf(elem.Symbol as Virtual));
                }
                else if (elem.Symbol is Action)
                {
                    stream.Write((ushort)8);
                    stream.Write((ushort)actions.IndexOf(elem.Symbol as Action));
                }
                else
                {
                    if (elem.Action == RuleBodyElementAction.Drop) stream.Write((ushort)2);
                    else if (elem.Action == RuleBodyElementAction.Promote) stream.Write((ushort)3);
                    else stream.Write((ushort)0);
                }
            }
        }

        // TODO: think about it, but shouldn't stream be a field of the class? or create a new class?
        public void ExportCode(StreamWriter stream, string className, AccessModifier modifier, string lexerClassName, IList<Terminal> expected)
        {
            this.terminals = new List<Terminal>(expected);

            stream.WriteLine("    " + modifier.ToString().ToLower() + " class " + className + " : " + this.GetBaseClassName);
            stream.WriteLine("    {");
            ExportAutomaton(stream, className);
            ExportVariables(stream);
            ExportVirtuals(stream);
            ExportActions(stream);
            ExportActionsClass(stream);
            ExportActionHooks(stream);
            ExportConstructor(stream, className, lexerClassName);
            stream.WriteLine("    }");
        }

        protected abstract void ExportAutomaton(StreamWriter stream, string className);

        protected void ExportVariables(StreamWriter stream)
        {
            stream.WriteLine("        private static readonly SymbolVariable[] variables = {");
            bool first = true;
            foreach (Variable var in variables)
            {
                if (!first) stream.WriteLine(", ");
                stream.Write("            ");
                stream.Write("new SymbolVariable(0x" + var.SID.ToString("X") + ", \"" + var.Name + "\")");
                first = false;
            }
            stream.WriteLine(" };");

            stream.WriteLine("        public enum Variables : int");
            stream.WriteLine("        {");
            for (int i = 0; i != variables.Count; i++)
            {
                stream.Write("            " + variables[i].Name + " = " + i);
                if (i == variables.Count - 1)
                    stream.WriteLine();
                else
                    stream.WriteLine(",");
            }
            stream.WriteLine("        }");
        }

        protected void ExportVirtuals(StreamWriter stream)
        {
            stream.WriteLine("        private static readonly SymbolVirtual[] virtuals = {");
            bool first = true;
            foreach (Virtual v in virtuals)
            {
                if (!first) stream.WriteLine(", ");
                stream.Write("            ");
                stream.Write("new SymbolVirtual(\"" + v.Name + "\")");
                first = false;
            }
            stream.WriteLine(" };");

            stream.WriteLine("        public enum Virtuals : int");
            stream.WriteLine("        {");
            for (int i = 0; i != virtuals.Count; i++)
            {
                stream.Write("            " + virtuals[i].Name + " = " + i);
                if (i == virtuals.Count - 1)
                    stream.WriteLine();
                else
                    stream.WriteLine(",");
            }
            stream.WriteLine("        }");
        }

        protected void ExportActions(StreamWriter stream)
        {
            stream.WriteLine("        private static readonly ParserAction[] pactions = {");
            bool first = true;
            foreach (Action action in actions)
            {
                if (!first) stream.WriteLine(", ");
                stream.Write("            ");
                stream.Write("new ParserAction(Parser" + action.Name + ")");
                first = false;
            }
            stream.WriteLine(" };");
            stream.WriteLine("        private static readonly RecognizerAction[] ractions = {");
            first = true;
            foreach (Action action in actions)
            {
                if (!first) stream.WriteLine(", ");
                stream.Write("            ");
                stream.Write("new RecognizerAction(Recognizer" + action.Name + ")");
                first = false;
            }
            stream.WriteLine(" };");
        }

        protected virtual void ExportActionsClass(StreamWriter stream)
        {
            if (actions.Count == 0)
                return;
            stream.WriteLine("        public interface ParserActions");
            stream.WriteLine("        {");
            foreach (Action action in actions)
                stream.WriteLine("           void " + action.Name + "(CSTNode sub);");
            stream.WriteLine("        }");
            stream.WriteLine("        public interface RecognizerActions");
            stream.WriteLine("        {");
            foreach (Action action in actions)
                stream.WriteLine("           void " + action.Name + "(Symbol[] body, int length);");
            stream.WriteLine("        }");
            stream.WriteLine("        private ParserActions userPActions;");
            stream.WriteLine("        private RecognizerActions userRActions;");
        }

        protected virtual void ExportActionHooks(StreamWriter stream)
        {
            foreach (Action action in actions)
                stream.WriteLine("        private void Parser" + action.Name + "(SyntaxTreeNode sub) { this.userPActions." + action.Name + "(sub); }");
            foreach (Action action in actions)
                stream.WriteLine("        private void Recognizer" + action.Name + "(Symbol[] body, int length) { this.userRActions." + action.Name + "(body, length); }");
        }

        protected virtual void ExportConstructor(StreamWriter stream, string className, string lexerClassName)
        {
            string argument = "";
            string body = "";
            if (actions.Count != 0)
            {
                argument = ", ParserActions pacts, RecognizerActions racts";
                body = "this.userPActions = acts; this.userRActions = racts;";
            }
            stream.WriteLine("        public " + className + "(" + lexerClassName + " lexer" + argument + ") : base (automaton, variables, virtuals, pactions, ractions, lexer) { " + body + " }");
        }
        
		// TODO: this method could be factored more (look at the similar code)
        public void Document(string directory)
        {
            if (Directory.Exists(directory))
                Directory.Delete(directory, true);
            Directory.CreateDirectory(directory);
            string dirRes = Path.Combine(directory, "resources");
            Directory.CreateDirectory(dirRes);

            using (ResourceAccessor accessor = new ResourceAccessor())
			{
                accessor.Export("Transforms.Hime.css", Path.Combine(dirRes, "Hime.css"));
                accessor.Export("Transforms.Hime.js", Path.Combine(dirRes, "Hime.js"));
                accessor.Export("Visuals.button_plus.gif", Path.Combine(dirRes, "button_plus.gif"));
                accessor.Export("Visuals.button_minus.gif", Path.Combine(dirRes, "button_minus.gif"));
                accessor.Export("Visuals.Hime.Logo.png", Path.Combine(dirRes, "Hime.Logo.png"));
                accessor.Export("Visuals.Hime.GoTo.png", Path.Combine(dirRes, "Hime.GoTo.png"));
                accessor.Export("Visuals.Hime.Info.png", Path.Combine(dirRes, "Hime.Info.png"));
                accessor.Export("Visuals.Hime.Warning.png", Path.Combine(dirRes, "Hime.Warning.png"));
                accessor.Export("Visuals.Hime.Error.png", Path.Combine(dirRes, "Hime.Error.png"));
                accessor.Export("Visuals.Hime.Shift.png", Path.Combine(dirRes, "Hime.Shift.png"));
                accessor.Export("Visuals.Hime.Reduce.png", Path.Combine(dirRes, "Hime.Reduce.png"));
                accessor.Export("Visuals.Hime.None.png", Path.Combine(dirRes, "Hime.None.png"));
                accessor.Export("Visuals.Hime.ShiftReduce.png", Path.Combine(dirRes, "Hime.ShiftReduce.png"));
                accessor.Export("Visuals.Hime.ReduceReduce.png", Path.Combine(dirRes, "Hime.ReduceReduce.png"));

                XmlDocument document = new XmlDocument();
                XmlNode nodeGraph = this.graph.Serialize(document);
                document.AppendChild(nodeGraph);
                document.Save(Path.Combine(directory, "data.xml"));
                accessor.AddCheckoutFile(Path.Combine(directory, "data.xml"));

                // generate index
                accessor.CheckOut("Transforms.Doc.Index.xslt", Path.Combine(directory, "Index.xslt"));
                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(Path.Combine(directory, "Index.xslt"));
                transform.Transform(Path.Combine(directory, "data.xml"), Path.Combine(directory, "index.html"));
                
                // generate sets
                accessor.CheckOut("Transforms.Doc.ParserData.xslt", Path.Combine(directory, "ParserData.xslt"));
                transform = new XslCompiledTransform();
                transform.Load(Path.Combine(directory, "ParserData.xslt"));
                foreach (XmlNode child in nodeGraph.ChildNodes)
                {
                    string temp = Path.Combine(directory, "set_" + child.Attributes["SetID"].Value);
                    XmlDocument tempXML = new XmlDocument();
                    tempXML.AppendChild(tempXML.ImportNode(child, true));
                    tempXML.Save(temp + ".xml");
                    accessor.AddCheckoutFile(temp + ".xml");
                    transform.Transform(temp + ".xml", temp + ".html");
                }

                // generate grammar
            	document = new XmlDocument();
            	document.AppendChild(SerializeGrammar(document));
                document.Save(Path.Combine(directory, "data.xml"));
            	accessor.CheckOut("Transforms.Doc.Grammar.xslt", Path.Combine(directory, "Grammar.xslt"));
                transform = new XslCompiledTransform();
                transform.Load(Path.Combine(directory, "Grammar.xslt"));
                transform.Transform(Path.Combine(directory, "data.xml"), Path.Combine(directory, "grammar.html"));

            	// export parser data
            	List<string> files = SerializeVisuals(directory);
			}
        }

        protected List<string> SerializeVisuals(string directory)
        {
            List<string> files = new List<string>();
            SerializeGraphVisual(directory, files);
            SerializeSpecifics(directory, files);
            return files;
        }

        protected void SerializeGraphVisual(string directory, List<string> results)
        {
            DOTSerializer serializer = new DOTSerializer("Parser", Path.Combine(directory, "GraphParser.dot"));
			graph.SerializeVisual(serializer);
            serializer.Close();
            results.Add(Path.Combine(directory, "GraphParser.dot"));
        }

        protected virtual void SerializeSpecifics(string directory, List<string> results) { }

        protected XmlNode SerializeGrammar(XmlDocument document)
        {
            XmlNode root = document.CreateElement("CFGrammar");
            root.Attributes.Append(document.CreateAttribute("Name"));
            root.Attributes["Name"].Value = grammar.Name;
            foreach (Variable var in grammar.Variables)
                root.AppendChild(var.GetXMLNodeWithRules(document));
            return root;
        }
    }
}
