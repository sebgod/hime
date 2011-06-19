﻿using System.Collections.Generic;

namespace Hime.Parsers.CF.LR
{
    class GLRStackNode
    {
        private State state;
        private Dictionary<Symbol, Dictionary<int, GLRStackNode>> previous;

        public State State { get { return state; } }
        public Dictionary<Symbol, Dictionary<int, GLRStackNode>> Previous { get { return previous; } }

        public GLRStackNode(State state)
        {
            this.state = state;
            this.previous = new Dictionary<Symbol, Dictionary<int, GLRStackNode>>();
        }

        public void AddPrevious(Symbol symbol, GLRStackNode node)
        {
            int id = node.GetHashCode();
            if (!previous.ContainsKey(symbol))
            {
                Dictionary<int, GLRStackNode> nodes = new Dictionary<int, GLRStackNode>();
                nodes.Add(id, node);
                previous.Add(symbol, nodes);
            }
            else
            {
                Dictionary<int, GLRStackNode> nodes = previous[symbol];
                if (nodes.ContainsKey(id))
                    return;
                nodes.Add(id, node);
            }
        }
    }

    class GLRSimulatorState
    {
        private IList<GLRStackNode> nodes;
        private Dictionary<int, Dictionary<int, GLRStackNode>> map;
        public IList<GLRStackNode> Nodes { get { return nodes; } }

        public GLRSimulatorState()
        {
            nodes = new List<GLRStackNode>();
            map = new Dictionary<int, Dictionary<int, GLRStackNode>>();
        }
        public GLRSimulatorState(ICollection<GLRStackNode> nodes)
        {
            this.nodes = new List<GLRStackNode>();
            this.map = new Dictionary<int, Dictionary<int, GLRStackNode>>();
            foreach (GLRStackNode node in nodes)
                Add(node);
        }
        
        public GLRStackNode Add(State state)
        {
            int stateID = state.ID;
            if (!map.ContainsKey(stateID))
            {
                GLRStackNode node = new GLRStackNode(state);
                Dictionary<int, GLRStackNode> temp = new Dictionary<int, GLRStackNode>();
                int id = node.GetHashCode();
                temp.Add(id, node);
                map.Add(stateID, temp);
                nodes.Add(node);
                return node;
            }
            else
            {
                Dictionary<int, GLRStackNode> temp = map[stateID];
                foreach (GLRStackNode node in temp.Values)
                    return node;
                return null;
            }
        }
        public GLRStackNode Add(GLRStackNode node)
        {
            int stateID = node.State.ID;
            int id = node.GetHashCode();
            if (!map.ContainsKey(stateID))
            {
                Dictionary<int, GLRStackNode> temp = new Dictionary<int, GLRStackNode>();
                temp.Add(id, node);
                map.Add(stateID, temp);
                nodes.Add(node);
            }
            else
            {
                Dictionary<int, GLRStackNode> temp = map[stateID];
                if (!temp.ContainsKey(id))
                {
                    temp.Add(id, node);
                    nodes.Add(node);
                }
            }
            return node;
        }
    }


    class GLRSimulator
    {
        private Graph graph;
        private Dictionary<int, Dictionary<Symbol, List<State>>> inverseGraph;

        public GLRSimulator(Graph graph)
        {
            this.graph = graph;
            this.inverseGraph = new Dictionary<int, Dictionary<Symbol, List<State>>>();
            BuildInverse();
        }

        private void BuildInverse()
        {
            foreach (State set in graph.Sets)
            {
                foreach (Symbol symbol in set.Children.Keys)
                {
                    State child = set.Children[symbol];
                    if (!inverseGraph.ContainsKey(child.ID))
                        inverseGraph.Add(child.ID, new Dictionary<Symbol, List<State>>());
                    Dictionary<Symbol, List<State>> inverses = inverseGraph[child.ID];
                    if (!inverses.ContainsKey(symbol))
                        inverses.Add(symbol, new List<State>());
                    List<State> parents = inverses[symbol];
                    parents.Add(set);
                }
            }
        }

        public GLRSimulatorState Simulate(GLRSimulatorState origin, Terminal lookahead)
        {
            // Nodes before reductions
            GLRSimulatorState before = new GLRSimulatorState(origin.Nodes);

            // Apply reductions
            for (int i = 0; i != before.Nodes.Count; i++)
            {
                GLRStackNode node = before.Nodes[i];
                foreach (StateActionReduce reduce in node.State.Reductions)
                {
                    if (reduce.Lookahead.SID != lookahead.SID)
                        continue;
                    GLRSimulatorState reduceOrigin = GetOrigin(node, reduce.ToReduceRule.Definition.GetChoiceAtIndex(0));
                    foreach (GLRStackNode nOrigin in reduceOrigin.Nodes)
                    {
                        if (nOrigin.State.Children.ContainsKey(reduce.ToReduceRule.Variable))
                        {
                            State next = nOrigin.State.Children[reduce.ToReduceRule.Variable];
                            GLRStackNode follower = before.Add(next);
                            follower.AddPrevious(reduce.ToReduceRule.Variable, nOrigin);
                        }
                    }
                }
            }

            // Apply shift
            GLRSimulatorState result = new GLRSimulatorState();
            foreach (GLRStackNode node in before.Nodes)
            {
                if (node.State.Children.ContainsKey(lookahead))
                {
                    State child = node.State.Children[lookahead];
                    GLRStackNode nChild = result.Add(child);
                    nChild.AddPrevious(lookahead, node);
                }
            }
            return result;
        }

        public GLRSimulatorState Simulate(State state, Item item, Terminal lookahead)
        {
            GLRSimulatorState pState = new GLRSimulatorState();
            GLRStackNode pNode = pState.Add(state);
            GLRSimulatorState result = new GLRSimulatorState();

            if (item.Action == ItemAction.Shift)
            {
                GLRStackNode next = result.Add(state.Children[item.NextSymbol]);
                next.AddPrevious(item.NextSymbol, pNode);
                return result;
            }

            GLRSimulatorState origin = GetOrigin(pState, item.BaseRule.Definition.GetChoiceAtIndex(0));
            foreach (GLRStackNode node in origin.Nodes)
            {
                if (node.State.Children.ContainsKey(item.BaseRule.Variable))
                {
                    State next = node.State.Children[item.BaseRule.Variable];
                    GLRStackNode nNode = result.Add(next);
                    nNode.AddPrevious(item.BaseRule.Variable, node);
                }
            }
            return Simulate(result, lookahead);
        }

        private GLRSimulatorState GetOrigin(GLRStackNode node, CFRuleDefinition definition)
        {
            GLRSimulatorState current = new GLRSimulatorState();
            current.Add(node);
            return GetOrigin(current, definition);
        }
        private GLRSimulatorState GetOrigin(GLRSimulatorState current, CFRuleDefinition definition)
        {
            GLRSimulatorState result = current;
            int index = definition.Length - 1;
            while (index != -1)
            {
                Symbol symbol = definition.Parts[index].Symbol;
                result = GetOrigin(result, symbol);
                index--;
            }
            return result;
        }
        private GLRSimulatorState GetOrigin(GLRSimulatorState current, Symbol symbol)
        {
            GLRSimulatorState result = new GLRSimulatorState();
            foreach (GLRStackNode node in current.Nodes)
            {
                if (node.Previous.ContainsKey(symbol))
                {
                    foreach (GLRStackNode previous in node.Previous[symbol].Values)
                        result.Add(previous);
                    continue;
                }
                if (!inverseGraph.ContainsKey(node.State.ID))
                    continue;
                Dictionary<Symbol, List<State>> inverses = inverseGraph[node.State.ID];
                if (!inverses.ContainsKey(symbol))
                    continue;
                foreach (State previous in inverses[symbol])
                {
                    GLRStackNode pNode = result.Add(previous);
                    node.AddPrevious(symbol, pNode);
                }
            }
            return result;
        }
    }
}
