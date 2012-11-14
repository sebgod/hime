﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LangTest
{
    partial class WinTreeView : Form
    {
        public WinTreeView(Hime.Redist.Parsers.CSTNode Root)
        {
            InitializeComponent();

            TreeNode VRoot = View.Nodes.Add(Root.Symbol.Name);
            AddSubTree(VRoot, Root);
        }

        private void AddSubTree(TreeNode VNode, Hime.Redist.Parsers.CSTNode SNode)
        {
            foreach (Hime.Redist.Parsers.CSTNode Child in SNode.Children)
            {
                TreeNode VChild = null;
                if (Child.Symbol != null)
                {
                    string name = Child.Symbol.Name;
                    string value = "";
                    if (Child.Symbol is Hime.Redist.Parsers.SymbolToken)
                        value = ": \"" + ((Hime.Redist.Parsers.SymbolToken)Child.Symbol).Value.ToString() + "\"";
                    string header = name + value;
                    VChild = VNode.Nodes.Add(header);
                }
                else
                {
                    VChild = VNode.Nodes.Add("<null>");
                }
                AddSubTree(VChild, Child);
            }
        }
    }
}