/*******************************************************************************
 * Copyright (c) 2015 Laurent Wouters and others
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
 *
 * Contributors:
 *     Laurent Wouters - lwouters@xowl.org
 ******************************************************************************/
package org.xowl.hime.redist.parsers;

import org.xowl.hime.redist.utils.BigList;
import org.xowl.hime.redist.utils.IntBigList;

import java.io.FileOutputStream;
import java.io.IOException;
import java.io.OutputStreamWriter;
import java.io.Writer;
import java.util.*;

/**
 * Represents Graph-Structured Stacks for GLR parsers
 *
 * @author Laurent Wouters
 */
class GSS {
    /**
     * The initial size of the paths buffer in this GSS
     */
    private static final int INIT_PATHS_COUNT = 64;
    /**
     * The initial size of the stack used for the traversal of this GSS
     */
    private static final int INIT_STACK_SIZE = 128;

    /**
     * Represents a set of paths
     */
    public static class PathSet {
        /**
         * The represented paths
         */
        public GSSPath[] content;
        /**
         * The number of paths in this set
         */
        public int count;

        /**
         * Initializes this set
         */
        public PathSet() {
            this.content = new GSSPath[INIT_PATHS_COUNT];
            this.count = 0;
        }
    }

    /**
     * The label (GLR state) on the GSS node for the given index
     */
    private final IntBigList nodeLabels;
    /**
     * The number of live incoming edges to the GSS node for the given index
     */
    private final IntBigList nodeIncomings;
    /**
     * The contexts available on the GSS node for the given index
     */
    private final BigList<BitSet> nodeContexts;
    /**
     * The generations in this GSS
     */
    private final BigList<GSSGeneration> nodeGenerations;

    /**
     * The edges in this GSS
     */
    private final BigList<GSSEdge> edges;
    /**
     * The generations for the edges
     */
    private final BigList<GSSGeneration> edgeGenerations;

    /**
     * Index of the current generation
     */
    private int generation;

    /**
     * A buffer of GSS paths
     */
    private final PathSet paths;

    /**
     * Stack of GSS nodes used for the traversal of the GSS
     */
    private int[] stack;

    /**
     * Initializes the GSS
     */
    public GSS() {
        this.nodeLabels = new IntBigList();
        this.nodeIncomings = new IntBigList();
        this.nodeContexts = new BigList<BitSet>(BitSet.class, BitSet[].class);
        this.nodeGenerations = new BigList<GSSGeneration>(GSSGeneration.class, GSSGeneration[].class);
        this.edges = new BigList<GSSEdge>(GSSEdge.class, GSSEdge[].class);
        this.edgeGenerations = new BigList<GSSGeneration>(GSSGeneration.class, GSSGeneration[].class);
        this.generation = -1;
        this.paths = new PathSet();
        this.stack = new int[INIT_STACK_SIZE];
    }

    /**
     * Gets the data of the specified generation
     *
     * @param generation A generation
     * @return The generation's first node
     */
    public GSSGeneration getGeneration(int generation) {
        return nodeGenerations.get(generation);
    }

    /**
     * Gets the GLR state represented by the specified node
     *
     * @param node A node
     * @return The GLR state represented by the node
     */
    public int getRepresentedState(int node) {
        return nodeLabels.get(node);
    }

    /**
     * Gets the available contexts at the specified node
     *
     * @param node A node
     * @return The available contexts at the specified node
     */
    public BitSet getContexts(int node) {
        return nodeContexts.get(node);
    }

    /**
     * Finds in the given generation a node representing the given GLR state
     *
     * @param generation A generation
     * @param state      A GLR state
     * @return The node representing the GLR state, or -1 if it is not found
     */
    public int findNode(int generation, int state) {
        GSSGeneration data = nodeGenerations.get(generation);
        for (int i = data.getStart(); i != data.getStart() + data.getCount(); i++)
            if (nodeLabels.get(i) == state)
                return i;
        return -1;
    }

    /**
     * Determines whether this instance has the required edge
     *
     * @param generation The generation of the edge's start node
     * @param from       The edge's start node
     * @param to         The edge's target node
     * @return true if this instance has the required edge; otherwise, false
     */
    public boolean hasEdge(int generation, int from, int to) {
        GSSGeneration data = nodeGenerations.get(generation);
        for (int i = data.getStart(); i != data.getStart() + data.getCount(); i++) {
            GSSEdge edge = edges.get(i);
            if (edge.getFrom() == from && edge.getTo() == to)
                return true;
        }
        return false;
    }

    /**
     * Opens a new generation in this GSS
     *
     * @return The index of the new generation
     */
    public int createGeneration() {
        if (generation != 0 && ((generation & 0xF) == 0)) {
            // the current generation is not 0 (first one) and is a multiple of 16
            // => cleanup the GSS from unreachable data
            cleanup();
        }
        nodeGenerations.add(new GSSGeneration(nodeLabels.size()));
        edgeGenerations.add(new GSSGeneration(edges.size()));
        generation++;
        return generation;
    }

    /**
     * Creates a new node in the GSS
     *
     * @param state         The GLR state represented by the node
     * @param contexts      The original context for the node
     * @param contextsCount The total number of contexts in the automaton
     * @return The node identifier
     */
    public int createNode(int state, LRContexts contexts, int contextsCount) {
        int node = nodeLabels.add(state);
        nodeIncomings.add(0);
        BitSet buffer = new BitSet(contextsCount);
        for (int i = 0; i != contexts.size(); i++)
            buffer.set(contexts.get(i), true);
        nodeContexts.add(buffer);
        GSSGeneration data = nodeGenerations.get(generation);
        data.increment();
        return node;
    }

    /**
     * Creates a new edge in the GSS
     *
     * @param from  The edge's starting node
     * @param to    The edge's target node
     * @param label The edge's label
     */
    public void createEdge(int from, int to, GSSLabel label) {
        edges.add(new GSSEdge(from, to, label));
        GSSGeneration data = edgeGenerations.get(generation);
        data.increment();
        nodeIncomings.set(to, nodeIncomings.get(to) + 1);
        nodeContexts.get(from).or(nodeContexts.get(to));
    }

    /**
     * Setups a reusable GSS path with the given length
     *
     * @param index  The index in the buffer of reusable paths
     * @param last   The last GLR state in the path
     * @param length The path's length
     */
    private void setupPath(int index, int last, int length) {
        if (index >= paths.content.length)
            paths.content = Arrays.copyOf(paths.content, paths.content.length + INIT_PATHS_COUNT);
        if (paths.content[index] == null)
            paths.content[index] = new GSSPath(length);
        else
            paths.content[index].ensure(length);
        paths.content[index].setLast(last);
        paths.content[index].setGeneration(getGenerationOf(last));
    }

    /**
     * Retrieve the generation of the given node in this GSS
     *
     * @param node A node's index
     * @return The index of the generation containing the node
     */
    private int getGenerationOf(int node) {
        for (int i = generation; i != -1; i--) {
            GSSGeneration gen = nodeGenerations.get(i);
            if (node >= gen.getStart() && node < gen.getStart() + gen.getCount())
                return i;
        }
        // should node happen
        return -1;
    }

    /**
     * Gets all paths in the GSS starting at the given node and with the given length
     *
     * @param from   The starting node
     * @param length The length of the requested paths
     * @return A set of paths in this GSS
     */
    public PathSet getPaths(int from, int length) {
        if (length == 0) {
            // use the common 0-length GSS path to avoid new memory allocation
            setupPath(0, from, 1);
            paths.count = 1;
            return paths;
        }

        // Initializes the first path
        setupPath(0, from, length);

        // The number of paths in the list
        int total = 1;
        // For the remaining hops
        for (int i = 0; i != length; i++) {
            int m = 0;          // Insertion index for the compaction process
            int next = total;   // Insertion index for new paths
            for (int p = 0; p != total; p++) {
                int last = paths.content[p].getLast();
                int genIndex = paths.content[p].getGeneration();
                // Look for new additional paths from last
                GSSGeneration gen = edgeGenerations.get(genIndex);
                int firstEdgeTarget = -1;
                GSSLabel firstEdgeLabel = null;
                for (int e = gen.getStart(); e != gen.getStart() + gen.getCount(); e++) {
                    GSSEdge edge = edges.get(e);
                    if (edge.getFrom() == last) {
                        if (firstEdgeTarget == -1) {
                            // This is the first edge
                            firstEdgeTarget = edge.getTo();
                            firstEdgeLabel = edge.getLabel();
                        } else {
                            // Not the first edge
                            // Clone and extend the new path
                            setupPath(next, edge.getTo(), length);
                            paths.content[next].copyLabelsFrom(paths.content[p], i);
                            paths.content[next].set(i, edge.getLabel());
                            // Go to next insert
                            next++;
                        }
                    }
                }
                // Check whether there was at least one edge
                if (firstEdgeTarget != -1) {
                    // Continue the current path
                    if (m != p) {
                        GSSPath t = paths.content[m];
                        paths.content[m] = paths.content[p];
                        paths.content[p] = t;
                    }
                    paths.content[m].setLast(firstEdgeTarget);
                    paths.content[m].setGeneration(getGenerationOf(firstEdgeTarget));
                    paths.content[m].set(i, firstEdgeLabel);
                    // goto next
                    m++;
                }
            }
            if (m != total) {
                // if some previous paths have been removed
                // => compact the list if needed
                for (int p = total; p != next; p++) {
                    GSSPath t = paths.content[m];
                    paths.content[m] = paths.content[p];
                    paths.content[p] = t;
                    m++;
                }
                // m is now the exact number of paths
                total = m;
            } else if (next != total) {
                // no path has been removed, but some have been added
                // => next is the exact number of paths
                total = next;
            }
        }

        paths.count = total;
        return paths;
    }

    /**
     * Cleanups this GSS by reclaiming the sub-trees on GSS labels that can no longer be reached
     */
    private void cleanup() {
        int top = -1;
        // first, enqueue all the non-reachable state of the last 16 generations
        int lastState = nodeGenerations.get(generation).getStart() - 1;
        int firstState = nodeGenerations.get(generation - 16).getStart();
        for (int i = lastState; i != firstState - 1; i--) {
            if (nodeIncomings.get(i) == 0) {
                top++;
                if (top == stack.length)
                    stack = Arrays.copyOf(stack, stack.length + INIT_STACK_SIZE);
                stack[top] = i;
            }
        }

        // traverse the GSS
        while (top != -1) {
            // pop the next state to inspect
            int origin = stack[top];
            top--;
            GSSGeneration genData = edgeGenerations.get(getGenerationOf(origin));
            for (int i = genData.getStart(); i != genData.getStart() + genData.getCount(); i++) {
                GSSEdge edge = edges.get(i);
                if (edge.getFrom() != origin)
                    continue;
                // here the edge is starting from the origin node
                // get the label on this edge and free it if necessary
                SubTree tree = edge.getLabel().getTree();
                if (tree != null)
                    tree.free();
                // decrement the target's incoming edges counter
                int counter = nodeIncomings.get(edge.getTo());
                counter--;
                nodeIncomings.set(edge.getTo(), counter);
                if (counter == 0) {
                    // the target node is now unreachable, enqueue it
                    top++;
                    if (top == stack.length)
                        stack = Arrays.copyOf(stack, stack.length + INIT_STACK_SIZE);
                    stack[top] = edge.getTo();
                }
            }
        }
    }

    /**
     * Prints this stack onto the console output
     */
    public void print() {
        try {
            OutputStreamWriter writer = new OutputStreamWriter(System.out, "UTF-8");
            printTo(writer);
            writer.close();
        } catch (IOException ex) {
            // ignore
        }
    }

    /**
     * Prints this stack into the specified file
     *
     * @param file The file to print to
     * @throws IOException When failing to write
     */
    public void printTo(String file) throws IOException {
        FileOutputStream stream = new FileOutputStream(file, false);
        OutputStreamWriter writer = new OutputStreamWriter(stream, "UTF-8");
        printTo(writer);
        writer.close();
        stream.close();
    }

    /**
     * Prints this stack with the specified writer
     *
     * @param writer A writer
     * @throws IOException When failing to write
     */
    public void printTo(Writer writer) throws IOException {
        // list of all nodes having at least one child
        Set<Integer> linked = new HashSet<Integer>();

        for (int i = generation; i != -1; i--) {
            writer.write("--- generation " + i + " ---" + System.lineSeparator());
            // Retrieve the edges in this generation
            Map<Integer, List<Integer>> myedges = new HashMap<Integer, List<Integer>>();
            GSSGeneration cedges = edgeGenerations.get(i);
            for (int j = 0; j != cedges.getCount(); j++) {
                GSSEdge edge = this.edges.get(cedges.getStart() + j);
                if (!myedges.containsKey(edge.getFrom()))
                    myedges.put(edge.getFrom(), new ArrayList<Integer>());
                myedges.get(edge.getFrom()).add(edge.getTo());
                linked.add(edge.getTo());
            }
            // Retrieve the nodes in this generation and reverse their order
            GSSGeneration cnodes = nodeGenerations.get(i);
            List<Integer> mynodes = new ArrayList<Integer>();
            for (int j = 0; j != cnodes.getCount(); j++)
                mynodes.add(cnodes.getStart() + j);
            Collections.reverse(mynodes);
            // print this generation
            for (int node : mynodes) {
                String mark = linked.contains(node) ? "node" : "head";
                if (myedges.containsKey(node)) {
                    for (int to : myedges.get(node)) {
                        int gen = getGenerationOf(to);
                        if (gen == i)
                            writer.write("\t" + mark + " " + nodeLabels.get(node) + " to " + nodeLabels.get(to) + System.lineSeparator());
                        else
                            writer.write("\t" + mark + " " + nodeLabels.get(node) + " to " + nodeLabels.get(to) + " in gen " + gen + System.lineSeparator());
                    }
                } else {
                    writer.write("\t" + mark + " " + nodeLabels.get(node) + System.lineSeparator());
                }
            }
        }
    }
}
