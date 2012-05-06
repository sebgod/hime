﻿/*
 * Author: Laurent Wouters
 * Date: 14/09/2011
 * Time: 17:22
 * 
 */
using System.Collections.Generic;
using Hime.Utils.Reporting;
using System.Xml;
using System.Text;

namespace Hime.Parsers.ContextFree.LR
{
	// should not inherit from Entry, rather should build an entry when needed!!
    class Conflict : Entry
    {
        private State state;
        private ConflictType type;
        private Terminal lookahead;
        private List<Item> items;
        private List<ConflictExample> examples;
        private bool isResolved;

        public State State { get { return state; } }
        public override string Message { get { return ToString(); } }
        public ConflictType ConflictType { get { return type; } }
        public Terminal ConflictSymbol { get { return lookahead; } }
        public ICollection<Item> Items { get { return items; } }
        public List<ConflictExample> Examples { get { return examples; } }
		
        public bool IsError
        {
            set { 
                if (value) this.Level = ELevel.Error;
				else this.Level = ELevel.Warning;
			}
        }
		
        public bool IsResolved
        {
            get { return isResolved; }
            set { isResolved = value; }
        }

        public Conflict(string component, State state, ConflictType type, Terminal lookahead)
			// TODO: this is a bit strange, not ideal...
			: base(ELevel.Error, component, "")
        {
            this.state = state;
            this.type = type;
            this.lookahead = lookahead;
            this.isResolved = false;
            items = new List<Item>();
            examples = new List<ConflictExample>();
        }
		
        public Conflict(string component, State state, ConflictType type)
			: base(ELevel.Error, component, "")
        {
            this.state = state;
            this.type = type;
            this.isResolved = false;
            items = new List<Item>();
            examples = new List<ConflictExample>();
        }

        public void AddItem(Item Item) { items.Add(Item); }
        public bool ContainsItem(Item Item) { return items.Contains(Item); }

        public override XmlNode GetMessageNode(XmlDocument doc)
        {
            XmlNode element = doc.CreateElement("Conflict");

            XmlNode header = doc.CreateElement("Header");
            header.Attributes.Append(doc.CreateAttribute("type"));
            header.Attributes.Append(doc.CreateAttribute("set"));
            header.Attributes["type"].Value = type.ToString();
            header.Attributes["set"].Value = state.ID.ToString("X");
            header.AppendChild(lookahead.GetXMLNode(doc));
            element.AppendChild(header);

            XmlNode nodeItems = doc.CreateElement("Items");
            foreach (Item item in items) nodeItems.AppendChild(item.GetXMLNode(doc, state));
            element.AppendChild(nodeItems);


            XmlNode nexs = doc.CreateElement("Examples");
            foreach (ConflictExample example in examples) nexs.AppendChild(example.GetXMLNode(doc));
            element.AppendChild(nexs);
            return element;
        }

        public override string ToString()
        {
            StringBuilder Builder = new StringBuilder("Conflict ");
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
