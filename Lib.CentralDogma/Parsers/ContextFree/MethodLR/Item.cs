﻿/*
 * Author: Laurent Wouters
 * Date: 14/09/2011
 * Time: 17:22
 * 
 */
using System.Collections.Generic;

namespace Hime.Parsers.ContextFree.LR
{
    public abstract class Item
    {
        public const string dot = "●";

        protected CFRule rule;
        protected CFRuleBody definition;
        protected int dotPosition;

        public CFRule BaseRule { get { return rule; } }
        public int DotPosition { get { return dotPosition; } }
        public ItemAction Action
        {
            get
            {
                if (dotPosition != definition.Length)
                    return ItemAction.Shift;
                return ItemAction.Reduce;
            }
        }
        
        public GrammarSymbol NextSymbol { get { return definition.GetSymbolAt(dotPosition); } }
        public CFRuleBody NextChoice { get { return rule.CFBody.GetChoiceAt(dotPosition + 1); } }

        public abstract TerminalSet Lookaheads { get; }

        public Item(CFRule Rule, int DotPosition)
        {
            rule = Rule;
            definition = rule.CFBody.GetChoiceAt(0);
            dotPosition = DotPosition;
        }

        public bool Equals_Base(Item Item)
        {
            if (rule != Item.rule)
                return false;
            return (dotPosition == Item.dotPosition);
        }
        public abstract bool ItemEquals(Item item);

        public abstract Item GetChild();
        public abstract void CloseTo(List<Item> closure, Dictionary<CFRule, Dictionary<int, List<Item>>> map);

        public override bool Equals(object obj) { return ItemEquals(obj as Item); }
        public override int GetHashCode() { return base.GetHashCode(); }
        public abstract override string ToString();
        public abstract string ToString(bool ShowDecoration);

        public System.Xml.XmlNode GetXMLNode(System.Xml.XmlDocument document, State set)
        {
            System.Xml.XmlNode root = document.CreateElement("Item");
            root.Attributes.Append(document.CreateAttribute("HeadName"));
            root.Attributes.Append(document.CreateAttribute("HeadSID"));
            root.Attributes.Append(document.CreateAttribute("Conflict"));
            root.Attributes["HeadName"].Value = rule.Head.LocalName;
            root.Attributes["HeadSID"].Value = rule.Head.SID.ToString("X");
            root.Attributes["Conflict"].Value = GetXMLNode_Conflict(set, this).ToString();

            System.Xml.XmlNode action = document.CreateElement("Action");
            action.Attributes.Append(document.CreateAttribute("Type"));
            action.Attributes["Type"].Value = this.Action.ToString();
            if (this.Action == ItemAction.Shift)
                action.InnerText = set.Children[this.NextSymbol].ID.ToString("X");
            else
                action.InnerText = rule.ID.ToString("X");
            root.AppendChild(action);

            System.Xml.XmlNode symbols = document.CreateElement("Symbols");
            int i = 0;
            foreach (RuleBodyElement Part in rule.CFBody.GetChoiceAt(0).Parts)
            {
                if (i == dotPosition)
                    symbols.AppendChild(document.CreateElement("Dot"));
                symbols.AppendChild(Part.GetXMLNode(document));
                i++;
            }
            if (i == dotPosition)
                symbols.AppendChild(document.CreateElement("Dot"));
            root.AppendChild(symbols);

            System.Xml.XmlNode lnode = document.CreateElement("Lookaheads");
            foreach (Terminal terminal in Lookaheads)
            {
                System.Xml.XmlNode lookahead = terminal.GetXMLNode(document);
                lnode.AppendChild(lookahead);
            }
            root.AppendChild(lnode);

            System.Xml.XmlNode cl = document.CreateElement("ConflictLookaheads");
            foreach (Conflict conflict in set.Conflicts)
            {
                if (conflict.ContainsItem(this))
                {
                    System.Xml.XmlNode lookahead = conflict.ConflictSymbol.GetXMLNode(document);
                    cl.AppendChild(lookahead);
                }
            }
            root.AppendChild(cl);
            return root;
        }
        private ConflictType GetXMLNode_Conflict(State set, Item item)
        {
            foreach (Conflict conflict in set.Conflicts)
                if (conflict.ContainsItem(item))
                    return conflict.ConflictType;
            return ConflictType.None;
        }
    }
}
