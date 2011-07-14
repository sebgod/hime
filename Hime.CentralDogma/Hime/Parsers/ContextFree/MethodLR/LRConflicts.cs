﻿using System.Collections.Generic;

namespace Hime.Parsers.CF.LR
{
    enum ConflictType
    {
        ShiftReduce,
        ReduceReduce,
        None
    }

    class ConflictExample
    {
        private List<Terminal> input;
        private Terminal lookahead;
        private List<Terminal> rest;

        public List<Terminal> Input { get { return input; } }
        public List<Terminal> Rest { get { return rest; } }

        public ConflictExample(Terminal l1)
        {
            input = new List<Terminal>();
            rest = new List<Terminal>();
            lookahead = l1;
        }

        public System.Xml.XmlNode GetXMLNode(System.Xml.XmlDocument doc)
        {
            System.Xml.XmlNode node = doc.CreateElement("Example");
            foreach (Terminal t in input)
                node.AppendChild(t.GetXMLNode(doc));
            node.AppendChild(doc.CreateElement("Dot"));
            node.AppendChild(lookahead.GetXMLNode(doc));
            foreach (Terminal t in rest)
                node.AppendChild(t.GetXMLNode(doc));
            return node;
        }
    }

    class Conflict : Hime.Kernel.Reporting.Entry
    {
        private string component;
        private State state;
        private ConflictType type;
        private Terminal lookahead;
        private List<Item> items;
        private List<ConflictExample> examples;
        private bool isError;
        private bool isResolved;

        public Hime.Kernel.Reporting.Level Level {
            get
            {
                if (isError) return Kernel.Reporting.Level.Error;
                return Kernel.Reporting.Level.Warning;
            }
        }
        public string Component
        {
            get { return component; }
            set { component = value; }
        }
        public State State { get { return state; } }
        public string Message { get { return ToString(); } }
        public ConflictType ConflictType { get { return type; } }
        public Terminal ConflictSymbol { get { return lookahead; } }
        public ICollection<Item> Items { get { return items; } }
        public List<ConflictExample> Examples { get { return examples; } }
        public bool IsError
        {
            get { return isError; }
            set { isError = value; }
        }
        public bool IsResolved
        {
            get { return isResolved; }
            set { isResolved = value; }
        }

        public Conflict(string component, State state, ConflictType type, Terminal lookahead)
        {
            this.component = component;
            this.state = state;
            this.type = type;
            this.lookahead = lookahead;
            this.isError = true;
            this.isResolved = false;
            items = new List<Item>();
            examples = new List<ConflictExample>();
        }
        public Conflict(string component, State state, ConflictType type)
        {
            this.component = component;
            this.state = state;
            this.type = type;
            this.isError = true;
            this.isResolved = false;
            items = new List<Item>();
            examples = new List<ConflictExample>();
        }

        public void AddItem(Item Item) { items.Add(Item); }
        public bool ContainsItem(Item Item) { return items.Contains(Item); }

        public System.Xml.XmlNode GetMessageNode(System.Xml.XmlDocument doc)
        {
            System.Xml.XmlNode element = doc.CreateElement("Conflict");

            System.Xml.XmlNode header = doc.CreateElement("Header");
            header.Attributes.Append(doc.CreateAttribute("type"));
            header.Attributes.Append(doc.CreateAttribute("set"));
            header.Attributes["type"].Value = type.ToString();
            header.Attributes["set"].Value = state.ID.ToString("X");
            header.AppendChild(lookahead.GetXMLNode(doc));
            element.AppendChild(header);

            System.Xml.XmlNode nodeItems = doc.CreateElement("Items");
            foreach (Item item in items)
                nodeItems.AppendChild(item.GetXMLNode(doc, state));
            element.AppendChild(nodeItems);


            System.Xml.XmlNode nexs = doc.CreateElement("Examples");
            foreach (ConflictExample example in examples)
                nexs.AppendChild(example.GetXMLNode(doc));
            element.AppendChild(nexs);
            return element;
        }

        public override string ToString()
        {
            System.Text.StringBuilder Builder = new System.Text.StringBuilder("Conflict ");
            if (type == ConflictType.ShiftReduce)
                Builder.Append("Shift/Reduce");
            else
                Builder.Append("Reduce/Reduce");
            Builder.Append(" in ");
            Builder.Append(state.ID.ToString("X"));
            if (lookahead != null)
            {
                Builder.Append(" on terminal '");
                Builder.Append(lookahead.ToString());
                Builder.Append("'");
            }
            Builder.Append(" for items {");
            foreach (Item Item in items)
            {
                Builder.Append(" ");
                Builder.Append(Item.ToString());
                Builder.Append(" ");
            }
            Builder.Append("}");
            return Builder.ToString();
        }
    }
}
