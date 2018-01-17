/*******************************************************************************
 * Copyright (c) 2018 Association Cénotélie (cenotelie.fr)
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

//! Module for RNGLR parsers

use std::usize;

use super::*;
use super::subtree::SubTree;
use super::super::ast::Ast;
use super::super::ast::AstCell;
use super::super::ast::TableElemRef;
use super::super::ast::TableType;
use super::super::errors::ParseErrorUnexpectedToken;
use super::super::lexers::DEFAULT_CONTEXT;
use super::super::lexers::Lexer;
use super::super::lexers::TokenKernel;
use super::super::symbols::SemanticBody;
use super::super::symbols::SemanticElement;
use super::super::symbols::SemanticElementTrait;
use super::super::utils::biglist::BigList;

/// Represents a cell in a RNGLR parse table
#[derive(Copy, Clone)]
struct RNGLRAutomatonCell {
    /// The number of actions in this cell
    count: u32,
    /// Index of the cell's data
    index: u32
}

/// Represents the RNGLR parsing table and productions
pub struct RNGLRAutomaton {
    /// Index of the axiom variable
    axiom: usize,
    /// The number of columns in the LR table
    columns_count: usize,
    /// The number of states in the LR table
    states_count: usize,
    /// Map of symbol ID to column index in the LR table
    columns_map: LRColumnMap,
    /// The contexts information
    contexts: Vec<LRContexts>,
    /// The RNGLR table
    cells: Vec<RNGLRAutomatonCell>,
    /// The LR action table
    table: Vec<u16>,
    /// The table of LR productions
    productions: Vec<LRProduction>,
    /// The table of nullable variables
    nullables: Vec<u16>
}

impl RNGLRAutomaton {
    /// Initializes a new automaton from the given binary data
    pub fn new(data: &[u8]) -> RNGLRAutomaton {
        // read basic counters
        let axiom_index = read_u16(data, 0) as usize;
        let columns_count = read_u16(data, 2) as usize;
        let states_count = read_u16(data, 4) as usize;
        let actions_count = read_u32(data, 6) as usize;
        let productions_count = read_u16(data, 10) as usize;
        let nullables_count = read_u16(data, 12) as usize;
        // reads the column map
        let columns_map = LRColumnMap::new(data, 14, columns_count);
        // read the contexts table
        let mut contexts = Vec::<LRContexts>::with_capacity(states_count);
        let mut index = 14 + columns_count * 2;
        for _i in 0..states_count {
            let mut context = LRContexts::new();
            let count = read_u16(data, index);
            index += 2;
            for _j in 0..count {
                context.add(read_u16(data, index), read_u16(data, index + 2));
                index += 4
            }
            contexts.push(context);
        }
        // read the automaton cells
        let mut cells = Vec::<RNGLRAutomatonCell>::with_capacity(columns_count * states_count);
        for _i in 0..(columns_count * states_count) {
            cells.push(RNGLRAutomatonCell {
                count: read_u16(data, index) as u32,
                index: read_u32(data, index + 2)
            });
            index += 6;
        }
        // read the actions table for the automaton
        let table = read_table_u16(data, index, actions_count * 2);
        index += actions_count * 4;
        // read the production table
        let mut productions = Vec::<LRProduction>::with_capacity(productions_count);
        for _i in 0..productions_count {
            let production = LRProduction::new(data, &mut index);
            productions.push(production);
        }
        // read the nullables table
        let nullables = read_table_u16(data, index, nullables_count);
        index += nullables_count * 2;
        assert_eq!(index, data.len());
        RNGLRAutomaton {
            axiom: axiom_index,
            columns_count,
            states_count,
            columns_map,
            contexts,
            cells,
            table,
            productions,
            nullables
        }
    }

    /// Gets the index of the axiom
    pub fn get_axiom(&self) -> usize {
        self.axiom
    }

    /// Gets the number of states in this automaton
    pub fn get_states_count(&self) -> usize {
        self.states_count
    }

    /// Gets the contexts opened by the specified state
    pub fn get_contexts(&self, state: u32) -> &LRContexts {
        &self.contexts[state as usize]
    }

    /// Gets the number of GLR actions for the given state and symbol identifier
    pub fn get_actions_count(&self, state: u32, identifier: u32) -> usize {
        let column = self.columns_map.get(identifier) as usize;
        self.cells[state as usize * self.columns_count + column].count as usize
    }

    /// Gets the i-th GLR action for the given state and sid
    pub fn get_action(&self, state: u32, identifier: u32, index: usize) -> LRAction {
        let column = self.columns_map.get(identifier) as usize;
        let cell = self.cells[state as usize * self.columns_count + column];
        LRAction {
            table: &self.table,
            offset: (cell.index as usize + index) * 2
        }
    }

    /// Gets the i-th production
    pub fn get_production(&self, index: usize) -> &LRProduction {
        &self.productions[index]
    }

    /// Gets the production for the nullable variable with the given index
    pub fn get_nullable_production(&self, index: usize) -> Option<&LRProduction> {
        match self.nullables[index] {
            0xFFFF => None,
            prod_index => Some(&self.productions[prod_index as usize])
        }
    }

    /// Determine whether the given state is the accepting state
    pub fn is_accepting_state(&self, state: u32) -> bool {
        let cell = self.cells[state as usize * self.columns_count];
        if cell.count != 1 {
            false
        } else {
            self.table[(cell.index as usize) * 2] == LR_ACTION_CODE_ACCEPT
        }
    }

    /// Gets the expected terminals for the specified state
    pub fn get_expected(&self, state: u32, terminals: &'static [Symbol]) -> LRExpected {
        let mut expected = LRExpected::new();
        for (column, terminal) in terminals.iter().enumerate() {
            let cell = self.cells[state as usize * self.columns_count + column];
            for i in 0..cell.count as usize {
                let action = LRAction {
                    table: &self.table,
                    offset: (cell.index as usize + i) * 2
                };
                if action.get_code() == LR_ACTION_CODE_SHIFT {
                    expected.add_unique_shift(*terminal);
                } else if action.get_code() == LR_ACTION_CODE_REDUCE {
                    expected.add_unique_reduction(*terminal);
                }
            }
        }
        expected
    }
}

/// Represents an edge in a Graph-Structured Stack
#[derive(Copy, Clone)]
struct GSSEdge {
    /// The index of the node from which this edge starts
    from: usize,
    /// The index of the node to which this edge arrives to
    to: usize,
    /// The identifier of the SPPF node that serve as label for this edge
    label: usize
}

/// Represents a generation in a Graph-Structured Stack
/// Because GSS nodes and edges are always created sequentially,
/// a generation basically describes a span in a buffer of GSS nodes or edges
#[derive(Copy, Clone)]
struct GSSGeneration {
    /// The start index of this generation in the list of nodes
    start: usize,
    /// The number of nodes in this generation
    count: usize
}

/// Represents a path in a GSS
#[derive(Clone)]
struct GSSPath {
    /// The final target of this path
    last_node: usize,
    /// The generation containing the final target of this path
    generation: usize,
    /// The labels on this GSS path
    labels: Option<Vec<usize>>
}

impl GSSPath {
    /// Initializes this path
    pub fn new(last_node: usize, generation: usize, length: usize) -> GSSPath {
        GSSPath {
            last_node,
            generation,
            labels: if length == 0 {
                None
            } else {
                Some(Vec::<usize>::with_capacity(length))
            }
        }
    }

    /// Initializes this path
    pub fn new_length0(last_node: usize, generation: usize) -> GSSPath {
        GSSPath {
            last_node,
            generation,
            labels: None
        }
    }

    /// Initializes this path
    pub fn from(previous: &GSSPath, last_node: usize, generation: usize, label: usize) -> GSSPath {
        let mut result = GSSPath {
            last_node,
            generation,
            labels: previous.labels.clone()
        };
        result.labels.as_mut().unwrap().push(label);
        result
    }

    /// Pushes the next label
    pub fn push(&mut self, last_node: usize, generation: usize, label: usize) {
        self.last_node = last_node;
        self.generation = generation;
        self.labels.as_mut().unwrap().push(label);
    }
}

/// Represents Graph-Structured Stacks for GLR parsers
struct GSS {
    /// The label (GLR state) on the GSS node for the given index
    node_labels: BigList<u32>,
    /// The generations of nodes in this GSS
    node_generations: BigList<GSSGeneration>,
    /// The edges in this GSS
    edges: BigList<GSSEdge>,
    /// The generations for the edges
    edges_generations: BigList<GSSGeneration>,
    /// Index of the current generation
    current_generation: usize
}

impl GSS {
    /// Initializes the GSS
    pub fn new() -> GSS {
        GSS {
            node_labels: BigList::<u32>::new(0),
            node_generations: BigList::<GSSGeneration>::new(GSSGeneration { start: 0, count: 0 }),
            edges: BigList::<GSSEdge>::new(GSSEdge {
                from: 0,
                to: 0,
                label: 0
            }),
            edges_generations: BigList::<GSSGeneration>::new(GSSGeneration { start: 0, count: 0 }),
            current_generation: 0
        }
    }

    /// Gets the data of the current generation
    pub fn get_current_generation(&self) -> GSSGeneration {
        self.node_generations[self.current_generation]
    }

    /// Gets the data of the specified generation of nodes
    pub fn get_generation(&self, generation: usize) -> GSSGeneration {
        self.node_generations[generation]
    }

    /// Gets the GLR state represented by the specified node
    pub fn get_represented_state(&self, node: usize) -> u32 {
        self.node_labels[node]
    }

    /// Finds in the given generation a node representing the given GLR state
    pub fn find_node(&self, generation: usize, state: u32) -> Option<usize> {
        let data = self.node_generations[generation];
        for i in data.start..(data.start + data.count) {
            if self.node_labels[i] == state {
                return Some(i);
            }
        }
        None
    }

    /// Determines whether this instance has the required edge
    pub fn has_edge(&self, generation: usize, from: usize, to: usize) -> bool {
        let data = self.edges_generations[generation];
        for i in data.start..(data.start + data.count) {
            let edge = self.edges[i];
            if edge.from == from && edge.to == to {
                return true;
            }
        }
        false
    }

    /// Opens a new generation in this GSS
    pub fn create_generation(&mut self) -> usize {
        self.node_generations.push(GSSGeneration {
            start: self.node_labels.len(),
            count: 0
        });
        self.edges_generations.push(GSSGeneration {
            start: self.edges.len(),
            count: 0
        });
        if self.node_generations.len() == 1 {
            // this is the first generation
            self.current_generation = 0;
        } else {
            self.current_generation += 1;
        }
        self.current_generation
    }

    /// Creates a new node in the GSS
    pub fn create_node(&mut self, state: u32) -> usize {
        let node = self.node_labels.push(state);
        self.node_generations[self.current_generation].count += 1;
        node
    }

    /// Creates a new edge in the GSS
    pub fn create_edge(&mut self, from: usize, to: usize, label: usize) {
        self.edges.push(GSSEdge { from, to, label });
        self.edges_generations[self.current_generation].count += 1;
    }

    /// Retrieve the generation of the given node in this GSS
    fn get_generation_of(&self, node: usize) -> usize {
        for i in 0..self.current_generation {
            let data = self.node_generations[i];
            if node >= data.start && node < data.start + data.count {
                return i;
            }
        }
        panic!("Node not found");
    }

    /// Gets all paths in the GSS starting at the given node and with the given length
    pub fn get_paths(&self, from: usize, length: usize) -> Vec<GSSPath> {
        let mut paths = Vec::<GSSPath>::new();
        if length == 0 {
            // 0-length path, simply return a single path with the 'from' node
            paths.push(GSSPath::new_length0(from, 0));
            return paths;
        }

        // Initialize the first path
        paths.push(GSSPath::new(from, self.get_generation_of(from), length));
        // For the remaining hops
        for i in 0..length {
            // Insertion index for the compaction process
            let mut m = 0;
            let total = paths.len();
            for p in 0..total {
                // for all paths
                let last = paths[p].last_node;
                let gen_index = paths[p].generation;
                // Look for new additional paths from last
                let gen = self.edges_generations[gen_index];
                let mut first_edge: Option<GSSEdge> = None;
                for e in gen.start..(gen.start + gen.count) {
                    let edge = self.edges[e];
                    if edge.from == last {
                        match &first_edge {
                            &None => {
                                // This is the first edge
                                first_edge = Some(edge);
                            }
                            &Some(_x) => {
                                // Not the first edge
                                // Clone and extend the new path
                                let new_path = GSSPath::from(
                                    &paths[p],
                                    edge.to,
                                    self.get_generation_of(edge.to),
                                    edge.label
                                );
                                paths.push(new_path);
                            }
                        }
                    }
                }
                // Check whether there was at least one edge
                match first_edge {
                    None => {}
                    Some(edge) => {
                        // Continue the current path
                        if m != p {
                            // swap m and p paths
                            paths.swap(m, p);
                        }
                        paths[m].push(edge.to, self.get_generation_of(edge.to), edge.label);
                        // goto next
                        m += 1;
                    }
                }
            }
            // inspected all current paths
            if m != total {
                // if some previous paths have been removed
                // => compact the list if needed
                for p in total..paths.len() {
                    paths.swap(m, p);
                    m += 1;
                }
                // truncates the paths to those inserted
                // m is now the exact number of paths
                paths.truncate(m);
            }
        }
        paths
    }
}

/// Represents a reference to a Shared-Packed Parse Forest node in a specific version
#[derive(Copy, Clone)]
struct SPPFNodeRef {
    /// The identifier of the node
    node_id: u32,
    /// The version to refer to
    version: u32
}

/// Represents a version of a node in a Shared-Packed Parse Forest
#[derive(Clone)]
struct SPPFNodeVersion {
    /// The label of the node for this version
    label: TableElemRef,
    /// The children of the node for this version
    children: Option<Vec<SPPFNodeRef>>
}

impl SPPFNodeVersion {
    /// Initializes this node version without children
    pub fn new(label: TableElemRef) -> SPPFNodeVersion {
        SPPFNodeVersion {
            label,
            children: None
        }
    }

    /// Initializes this node version
    pub fn from(label: TableElemRef, buffer: &Vec<SPPFNodeRef>, count: usize) -> SPPFNodeVersion {
        if count == 0 {
            SPPFNodeVersion {
                label,
                children: None
            }
        } else {
            let mut children = Vec::<SPPFNodeRef>::with_capacity(count);
            for i in 0..count {
                children.push(buffer[i]);
            }
            SPPFNodeVersion {
                label,
                children: Some(children)
            }
        }
    }

    /// Gets the number of children
    pub fn len(&self) -> usize {
        match &self.children {
            &None => 0,
            &Some(ref children) => children.len()
        }
    }
}

/// Represents the interface for a node in a Shared-Packed Parse Forest
trait SPPFNodeTrait {
    /// Gets the identifier of this node
    fn get_node_id(&self) -> usize;

    /// Gets the original symbol for this node
    fn get_original_symbol(&self) -> TableElemRef;
}

/// Represents a node in a Shared-Packed Parse Forest
/// A node can have multiple versions
#[derive(Clone)]
struct SPPFNodeNormal {
    /// The identifier of this node
    node_id: usize,
    /// The original label of this node
    original: TableElemRef,
    /// The different versions of this node
    versions: Vec<SPPFNodeVersion>
}

impl SPPFNodeTrait for SPPFNodeNormal {
    fn get_node_id(&self) -> usize {
        self.node_id
    }

    fn get_original_symbol(&self) -> TableElemRef {
        self.original
    }
}

impl SPPFNodeNormal {
    /// Initializes this node
    pub fn new(identifier: usize, label: TableElemRef) -> SPPFNodeNormal {
        let mut versions = Vec::<SPPFNodeVersion>::new();
        versions.push(SPPFNodeVersion::new(label));
        SPPFNodeNormal {
            node_id: identifier,
            original: label,
            versions
        }
    }

    /// Initializes this node
    pub fn new_with_children(
        identifier: usize,
        original: TableElemRef,
        label: TableElemRef,
        buffer: &Vec<SPPFNodeRef>,
        count: usize
    ) -> SPPFNodeNormal {
        let mut versions = Vec::<SPPFNodeVersion>::new();
        versions.push(SPPFNodeVersion::from(label, buffer, count));
        SPPFNodeNormal {
            node_id: identifier,
            original,
            versions
        }
    }

    /// Adds a new version to this node
    pub fn new_version(
        &mut self,
        label: TableElemRef,
        buffer: &Vec<SPPFNodeRef>,
        count: usize
    ) -> SPPFNodeRef {
        self.versions
            .push(SPPFNodeVersion::from(label, buffer, count));
        SPPFNodeRef {
            node_id: self.node_id as u32,
            version: (self.versions.len() - 1) as u32
        }
    }
}

/// Represents a node in a Shared-Packed Parse Forest that can be replaced by its children
#[derive(Clone)]
struct SPPFNodeReplaceable {
    /// The identifier of this node
    node_id: usize,
    /// The original label of this node
    original: TableElemRef,
    /// The children of this node
    children: Option<Vec<SPPFNodeRef>>,
    /// The tree actions on the children of this node
    actions: Option<Vec<TreeAction>>
}

impl SPPFNodeTrait for SPPFNodeReplaceable {
    fn get_node_id(&self) -> usize {
        self.node_id
    }

    fn get_original_symbol(&self) -> TableElemRef {
        self.original
    }
}

impl SPPFNodeReplaceable {
    /// Initializes this node
    pub fn new(
        identifier: usize,
        label: TableElemRef,
        children_buffer: &Vec<SPPFNodeRef>,
        actions_buffer: &Vec<TreeAction>,
        count: usize
    ) -> SPPFNodeReplaceable {
        if count == 0 {
            SPPFNodeReplaceable {
                node_id: identifier,
                original: label,
                children: None,
                actions: None
            }
        } else {
            let mut children = Vec::<SPPFNodeRef>::with_capacity(count);
            let mut actions = Vec::<TreeAction>::with_capacity(count);
            for i in 0..count {
                children.push(children_buffer[i]);
                actions.push(actions_buffer[i]);
            }
            SPPFNodeReplaceable {
                node_id: identifier,
                original: label,
                children: Some(children),
                actions: Some(actions)
            }
        }
    }
}

/// Represents a node in a Shared-Packed Parse Forest
#[derive(Clone)]
enum SPPFNode {
    /// A normal node
    Normal(SPPFNodeNormal),
    /// A replaceable node
    Replaceable(SPPFNodeReplaceable)
}

impl SPPFNodeTrait for SPPFNode {
    fn get_node_id(&self) -> usize {
        match self {
            &SPPFNode::Normal(ref node) => node.node_id,
            &SPPFNode::Replaceable(ref node) => node.node_id
        }
    }

    fn get_original_symbol(&self) -> TableElemRef {
        match self {
            &SPPFNode::Normal(ref node) => node.original,
            &SPPFNode::Replaceable(ref node) => node.original
        }
    }
}

impl SPPFNode {
    /// Gets whether this node must be replaced by its children
    pub fn is_replaceable(&self) -> bool {
        match self {
            &SPPFNode::Normal(ref _node) => false,
            &SPPFNode::Replaceable(ref _node) => true
        }
    }
}

/// Represents the epsilon node
const EPSILON: usize = 0xFFFFFFFF;

/// Represents a Shared-Packed Parse Forest
struct SPPF {
    /// The nodes in the SPPF
    nodes: Vec<SPPFNode>
}

impl SPPF {
    /// Initializes this SPPF
    pub fn new() -> SPPF {
        SPPF {
            nodes: Vec::<SPPFNode>::new()
        }
    }

    /// Gets the SPPF node for the specified identifier
    pub fn get_node(&self, identifier: usize) -> &SPPFNode {
        &self.nodes[identifier]
    }

    /// Gets the SPPF node for the specified identifier
    pub fn get_node_mut(&mut self, identifier: usize) -> &mut SPPFNode {
        &mut self.nodes[identifier]
    }

    /// Creates a new single node in the SPPF
    pub fn new_normal_node(&mut self, label: TableElemRef) -> usize {
        let identifier = self.nodes.len();
        self.nodes
            .push(SPPFNode::Normal(SPPFNodeNormal::new(identifier, label)));
        identifier
    }

    /// Creates a new single node in the SPPF
    pub fn new_normal_node_with_children(
        &mut self,
        original: TableElemRef,
        label: TableElemRef,
        buffer: &Vec<SPPFNodeRef>,
        count: usize
    ) -> usize {
        let identifier = self.nodes.len();
        self.nodes
            .push(SPPFNode::Normal(SPPFNodeNormal::new_with_children(
                identifier,
                original,
                label,
                buffer,
                count
            )));
        identifier
    }

    /// Creates a new replaceable node in the SPPF
    pub fn new_replaceable_node(
        &mut self,
        label: TableElemRef,
        children_buffer: &Vec<SPPFNodeRef>,
        actions_buffer: &Vec<TreeAction>,
        count: usize
    ) -> usize {
        let identifier = self.nodes.len();
        self.nodes
            .push(SPPFNode::Replaceable(SPPFNodeReplaceable::new(
                identifier,
                label,
                children_buffer,
                actions_buffer,
                count
            )));
        identifier
    }
}

/// Represents a generation of GSS edges in the current history
/// The history is used to quickly find pre-existing matching GSS edges
struct HistoryPart {
    /// The GSS labels in this part
    data: Vec<usize>,
    /// The index of the represented GSS generation
    generation: usize
}

/// The data about a reduction for a SPPF
struct SPPFReduction {
    /// The length of the reduction
    length: usize,
    /// The adjacency cache for the reduction
    cache: Vec<SPPFNodeRef>,
    /// The reduction handle represented as the indices of the sub-trees in the cache
    handle_indices: Vec<usize>,
    /// The actions for the reduction
    handle_actions: Vec<TreeAction>,
    /// The stack of semantic objects for the reduction
    stack: Vec<usize>,
    /// The number of items popped from the stack
    pop_count: usize
}

/// Represents a structure that helps build a Shared Packed Parse Forest (SPPF)
/// A SPPF is a compact representation of multiple variants of an AST at once.
/// GLR algorithms originally builds the complete SPPF.
/// However we only need to build one of the variant, i.e. an AST for the user.
struct SPPFBuilder<'l> {
    /// Lexer associated to this parser
    lexer: &'l mut Lexer<'l>,
    /// The history
    history: Vec<HistoryPart>,
    /// The SPPF being built
    sppf: SPPF,
    /// The data of the current reduction
    reduction: Option<SPPFReduction>,
    /// The AST being built
    result: Ast<'l>
}

impl<'l> SemanticBody for SPPFBuilder<'l> {
    fn length(&self) -> usize {
        match self.reduction {
            None => panic!("Not in a reduction"),
            Some(ref data) => data.handle_indices.len()
        }
    }

    fn get_element_at(&self, index: usize) -> SemanticElement {
        match self.reduction {
            None => panic!("Not in a reduction"),
            Some(ref data) => {
                let reference = data.cache[data.handle_indices[index]];
                let node = self.sppf.get_node(reference.node_id as usize);
                match node {
                    &SPPFNode::Normal(ref data) => {
                        let label = data.versions[reference.version as usize].label;
                        match label.get_type() {
                            TableType::None => panic!("Not a semantic element"),
                            TableType::Token => SemanticElement::Token(
                                self.lexer.get_output().get_token(label.get_index())
                            ),
                            TableType::Variable => SemanticElement::Variable(
                                self.result.get_variables()[label.get_index()]
                            ),
                            TableType::Virtual => SemanticElement::Virtual(
                                self.result.get_virtuals()[label.get_index()]
                            )
                        }
                    }
                    &SPPFNode::Replaceable(ref _data) => {
                        panic!("Expected a normal SPPF node");
                    }
                }
            }
        }
    }
}

impl<'l> SPPFBuilder<'l> {
    /// Initializes the builder with the given stack size
    pub fn new(lexer: &'l mut Lexer<'l>, result: Ast<'l>) -> SPPFBuilder<'l> {
        SPPFBuilder {
            lexer,
            history: Vec::<HistoryPart>::new(),
            sppf: SPPF::new(),
            reduction: None,
            result
        }
    }

    /// Gets the grammar variables for this AST
    pub fn get_variables(&self) -> &'static [Symbol] {
        self.result.get_variables()
    }

    /// Gets the history part for the given GSS generation
    fn get_history_part(&mut self, generation: usize) -> Option<&mut HistoryPart> {
        let my_history = &mut self.history;
        for i in 0..my_history.len() {
            if my_history[i].generation == generation {
                return Some(&mut my_history[i]);
            }
        }
        None
    }

    /// Clears the current history
    pub fn clear_history(&mut self) {
        self.history.clear();
    }

    /// Adds the specified GSS label to the current history
    pub fn add_to_history(&mut self, generation: usize, label: usize) {
        let my_history = &mut self.history;
        for i in 0..my_history.len() {
            if my_history[i].generation == generation {
                my_history[i].data.push(label);
                return;
            }
        }
        let mut data = Vec::<usize>::new();
        data.push(label);
        my_history.push(HistoryPart { generation, data });
    }

    /// Gets the symbol on the specified GSS edge label
    pub fn get_symbol_on(&self, label: usize) -> Symbol {
        let node_label = self.sppf.get_node(label).get_original_symbol();
        match node_label.get_type() {
            TableType::None => panic!("Not a semantic element"),
            TableType::Token => self.lexer
                .get_output()
                .get_token(node_label.get_index())
                .get_symbol(),
            TableType::Variable => self.result.get_variables()[node_label.get_index()],
            TableType::Virtual => self.result.get_virtuals()[node_label.get_index()]
        }
    }

    /// Gets the GSS label already in history for the given GSS generation and symbol
    pub fn get_label_for(&self, generation: usize, reference: TableElemRef) -> usize {
        for i in 0..self.history.len() {
            let hp = &self.history[i];
            if hp.generation == generation {
                for id in hp.data.iter() {
                    let node_symbol = self.sppf.get_node(*id).get_original_symbol();
                    if node_symbol == reference {
                        return *id;
                    }
                }
            }
        }
        EPSILON
    }

    /// Creates a single node in the result SPPF an returns it
    pub fn get_single_node(&mut self, symbol: TableElemRef) -> usize {
        self.sppf.new_normal_node(symbol)
    }

    /// Prepares for the forthcoming reduction operations
    pub fn reduction_prepare(&mut self, first: usize, path: &GSSPath, length: usize) {
        let mut stack = Vec::<usize>::new();
        if length > 0 {
            let path_labels = path.labels.as_ref().unwrap();
            for i in 0..(length - 1) {
                stack.push(path_labels[length - 2 - i]);
            }
            stack.push(first);
        }
        self.reduction = Some(SPPFReduction {
            cache: Vec::<SPPFNodeRef>::with_capacity(length),
            handle_indices: Vec::<usize>::with_capacity(length),
            handle_actions: Vec::<TreeAction>::with_capacity(length),
            stack,
            pop_count: 0,
            length
        });
    }

    /// Adds the specified GSS label to the reduction cache with the given tree action
    fn reduction_add_to_cache(
        reduction: &mut SPPFReduction,
        sppf: &SPPF,
        gss_label: usize,
        action: TreeAction
    ) {
        if action == TREE_ACTION_DROP {
            return;
        }
        let node = sppf.get_node(gss_label);
        match node {
            &SPPFNode::Normal(ref normal) => {
                // this is a simple reference to an existing SPPF node
                SPPFBuilder::reduction_add_to_cache_node(reduction, normal, action);
            }
            &SPPFNode::Replaceable(ref replaceable) => {
                // this is replaceable sub-tree
                match &replaceable.children {
                    &None => {}
                    &Some(ref children) => {
                        let actions = replaceable.actions.as_ref().unwrap();
                        for i in 0..children.len() {
                            SPPFBuilder::reduction_add_to_cache(
                                reduction,
                                sppf,
                                children[i].node_id as usize,
                                actions[i]
                            );
                        }
                    }
                }
            }
        }
    }

    /// Adds the specified GSS label to the reduction cache with the given tree action
    fn reduction_add_to_cache_node(
        reduction: &mut SPPFReduction,
        node: &SPPFNodeNormal,
        action: TreeAction
    ) {
        // add the node in the cache
        reduction.cache.push(SPPFNodeRef {
            node_id: node.node_id as u32,
            version: 0
        });
        // setup the handle to point to the root
        reduction.handle_indices.push(reduction.cache.len() - 1);
        reduction.handle_actions.push(action);
        // copy the children
        match &node.versions[0].children {
            &None => {}
            &Some(ref children) => for child in children.iter() {
                reduction.cache.push(*child);
            }
        }
    }

    /// During a reduction, pops the top symbol from the stack and gives it a tree action
    pub fn reduction_pop(&mut self, action: TreeAction) {
        match self.reduction {
            None => panic!("Not in a reduction"),
            Some(ref mut reduction) => {
                let label = reduction.stack[reduction.pop_count];
                reduction.pop_count += 1;
                SPPFBuilder::reduction_add_to_cache(reduction, &self.sppf, label, action);
            }
        }
    }

    /// During a reduction, inserts a virtual symbol
    pub fn reduction_add_virtual(&mut self, index: usize, action: TreeAction) {
        if action != TREE_ACTION_DROP {
            match self.reduction {
                None => panic!("Not in a reduction"),
                Some(ref mut reduction) => {
                    let node_id = self.sppf
                        .new_normal_node(TableElemRef::new(TableType::Virtual, index));
                    reduction.cache.push(SPPFNodeRef {
                        node_id: node_id as u32,
                        version: 0
                    });
                    reduction.handle_indices.push(reduction.cache.len() - 1);
                    reduction.handle_actions.push(action);
                }
            }
        }
    }

    /// During a reduction, inserts the sub-tree of a nullable variable
    pub fn reduction_add_nullable(&mut self, nullable: usize, action: TreeAction) {
        match self.reduction {
            None => panic!("Not in a reduction"),
            Some(ref mut reduction) => {
                SPPFBuilder::reduction_add_to_cache(reduction, &self.sppf, nullable, action);
            }
        }
    }

    /// Finalizes the reduction operation
    pub fn reduce(&mut self, generation: usize, variable_index: usize, replaceable: bool) -> usize {
        let label = if replaceable {
            self.reduce_replaceable(variable_index)
        } else {
            self.reduce_normal(variable_index)
        };
        self.add_to_history(generation, label);
        label
    }

    /// Executes the reduction as a normal reduction
    pub fn reduce_normal(&mut self, variable_index: usize) -> usize {
        match self.reduction {
            None => panic!("Not in a reduction"),
            Some(ref mut reduction) => {
                let mut promoted: Option<(TableElemRef, SPPFNodeRef)> = None;
                let mut insertion = 0;

                for i in 0..reduction.handle_indices.len() {
                    if reduction.handle_actions[i] == TREE_ACTION_PROMOTE {
                        match promoted {
                            None => {}
                            Some((symbol, node_ref)) => {
                                // not the first promotion
                                // create a new version for the promoted node
                                let old_promoted_node =
                                    self.sppf.get_node_mut(node_ref.node_id as usize);
                                let old_promoted_ref = match old_promoted_node {
                                    &mut SPPFNode::Replaceable(ref _data) => {
                                        panic!("Expected a normal node")
                                    }
                                    &mut SPPFNode::Normal(ref mut normal) => {
                                        normal.new_version(symbol, &reduction.cache, insertion)
                                    }
                                };
                                // register the previously promoted reference into the cache
                                reduction.cache[0] = old_promoted_ref;
                                insertion = 1;
                            }
                        }
                        // save the new promoted node
                        let promoted_reference = reduction.cache[reduction.handle_indices[i]];
                        let promoted_node = self.sppf.get_node(promoted_reference.node_id as usize);
                        match promoted_node {
                            &SPPFNode::Replaceable(ref _data) => panic!("Expected normal node"),
                            &SPPFNode::Normal(ref normal) => {
                                let promoted_version =
                                    &normal.versions[promoted_reference.version as usize];
                                promoted = Some((promoted_version.label, promoted_reference));
                                // repack the children on the left if any
                                for c in 0..promoted_version.len() {
                                    reduction.cache[insertion] =
                                        reduction.cache[reduction.handle_indices[i] + 1 + c];
                                    insertion += 1;
                                }
                            }
                        }
                    } else {
                        // Repack the sub-root on the left
                        if insertion != reduction.handle_indices[i] {
                            reduction.cache[insertion] =
                                reduction.cache[reduction.handle_indices[i]];
                        }
                        insertion += 1;
                    }
                }

                let original_label = TableElemRef::new(TableType::Variable, variable_index);
                let current_label = match promoted {
                    None => original_label,
                    Some((symbol, node_ref)) => symbol
                };
                self.sppf.new_normal_node_with_children(
                    original_label,
                    current_label,
                    &reduction.cache,
                    insertion
                )
            }
        }
    }

    /// Executes the reduction as the reduction of a replaceable variable
    pub fn reduce_replaceable(&mut self, variable_index: usize) -> usize {
        match self.reduction {
            None => panic!("Not in a reduction"),
            Some(ref mut reduction) => {
                let mut insertion = 0;
                for i in 0..reduction.handle_indices.len() {
                    if insertion != reduction.handle_indices[i] {
                        reduction.cache[insertion] = reduction.cache[reduction.handle_indices[i]];
                    }
                    insertion += 1;
                }
                let label = TableElemRef::new(TableType::Variable, variable_index);
                self.sppf.new_replaceable_node(
                    label,
                    &reduction.cache,
                    &reduction.handle_actions,
                    reduction.handle_indices.len()
                )
            }
        }
    }

    /// Finalizes the parse tree
    pub fn commit_root(&mut self, root: usize) {
        let sppf = &self.sppf;
        let result = &mut self.result;
        let cell_root = SPPFBuilder::build_final_ast(
            sppf,
            SPPFNodeRef {
                node_id: root as u32,
                version: 0
            },
            result
        );
        result.store_root(cell_root);
    }

    /// Builds thSe final AST for the specified PPF node reference
    fn build_final_ast(sppf: &SPPF, reference: SPPFNodeRef, result: &mut Ast) -> AstCell {
        match sppf.get_node(reference.node_id as usize) {
            &SPPFNode::Replaceable(ref _data) => panic!("Expected normal SPPF node"),
            &SPPFNode::Normal(ref data) => {
                let version = &data.versions[reference.version as usize];
                match &version.children {
                    &None => AstCell {
                        label: version.label,
                        first: 0,
                        count: 0
                    },
                    &Some(ref children) => {
                        let mut buffer = Vec::<AstCell>::with_capacity(children.len());
                        for i in 0..children.len() {
                            buffer.push(SPPFBuilder::build_final_ast(sppf, children[i], result));
                        }
                        let first = result.store(&buffer, 0, buffer.len());
                        AstCell {
                            label: version.label,
                            first: first as u32,
                            count: children.len() as u32
                        }
                    }
                }
            }
        }
    }
}

/// Represents a reduction operation to be performed
/// For reduction of length 0, the node is the GSS node on which it is applied,
/// the first label then is epsilon
/// For others, the node is the SECOND GSS node on the path, not the head.
/// The first label is then the label on the transition from the head.
struct RNGLRReduction {
    /// The GSS node to reduce from
    node: usize,
    /// The LR production for the reduction
    production: usize,
    /// The first label in the GSS
    first: usize
}

/// Represents a shift operation to be performed
struct RNGLRShift {
    /// GSS node to shift from
    from: usize,
    /// The target RNGLR state
    to: usize
}

struct RNGLRParserData<'a> {
    /// The parser's automaton
    automaton: RNGLRAutomaton,
    /// The GSS for this parser
    gss: GSS,
    /// The next token
    next_token: Option<TokenKernel>,
    /// The queue of reduction operations
    reductions: Vec<RNGLRReduction>,
    /// The queue of shift operations
    shifts: Vec<RNGLRShift>,
    /// The grammar variables
    variables: &'static [Symbol],
    /// The semantic actions
    actions: &'a mut FnMut(usize, Symbol, &SemanticBody)
}

impl<'a> ContextProvider for RNGLRParserData<'a> {
    /// Gets the priority of the specified context required by the specified terminal
    /// The priority is an unsigned integer. The lesser the value the higher the priority.
    /// The absence of value represents the unavailability of the required context.
    fn get_context_priority(
        &self,
        token_count: usize,
        context: u16,
        terminal_id: u32
    ) -> Option<usize> {
        unimplemented!()
    }
}

impl<'a> RNGLRParserData<'a> {}

/// Represents a base for all RNGLR parsers
pub struct RNGLRParser<'l, 'a: 'l> {
    /// The parser's data
    data: RNGLRParserData<'a>,
    /// The AST builder
    builder: SPPFBuilder<'l>,
    /// The sub-trees for the constant nullable variables
    nullables: Vec<usize>
}

impl<'l, 'a: 'l> RNGLRParser<'l, 'a> {
    /// Initializes a new instance of the parser
    pub fn new(
        lexer: &'l mut Lexer<'l>,
        automaton: RNGLRAutomaton,
        ast: Ast<'l>,
        actions: &'a mut FnMut(usize, Symbol, &SemanticBody)
    ) -> RNGLRParser<'l, 'a> {
        let mut parser = RNGLRParser {
            data: RNGLRParserData {
                automaton,
                gss: GSS::new(),
                next_token: None,
                reductions: Vec::<RNGLRReduction>::new(),
                shifts: Vec::<RNGLRShift>::new(),
                variables: ast.get_variables(),
                actions
            },
            builder: SPPFBuilder::new(lexer, ast),
            nullables: Vec::<usize>::new()
        };
        RNGLRParser::build_nullables(
            &mut parser.builder,
            &mut parser.data.actions,
            &mut parser.nullables,
            &parser.data.automaton,
            parser.data.variables
        );
        parser
    }

    /// Builds the constant sub-trees of nullable variables
    fn build_nullables(
        builder: &mut SPPFBuilder<'l>,
        actions: &mut FnMut(usize, Symbol, &SemanticBody),
        nullables: &mut Vec<usize>,
        automaton: &RNGLRAutomaton,
        variables: &[Symbol]
    ) {
        for i in 0..variables.len() {
            nullables.push(0xFFFFFFFF);
        }

        // Get the dependency table
        let mut dependencies = RNGLRParser::build_nullables_dependencies(automaton, variables);
        // Solve and build
        let mut remaining = 1;
        while remaining > 0 {
            remaining = 0;
            let mut resolved = 0;
            for i in 0..variables.len() {
                let mut was_unresolved = false;
                let mut is_resolved = false;
                {
                    let dep = &dependencies[i];
                    if dep.is_some() {
                        was_unresolved = true;
                        let mut ok = true;
                        for r in dep.as_ref().unwrap().iter() {
                            ok = ok && dependencies[*r].is_none();
                        }
                        if ok {
                            let production = automaton.get_nullable_production(i);
                            nullables[i] = RNGLRParser::build_sppf(
                                builder,
                                actions,
                                nullables,
                                0,
                                production.unwrap(),
                                EPSILON,
                                GSSPath::new(0, 0, 0)
                            );
                            is_resolved = true;
                        }
                    }
                }
                if was_unresolved {
                    if is_resolved {
                        dependencies[i] = None;
                        resolved += 1;
                    } else {
                        remaining += 1;
                    }
                }
            }
            if resolved == 0 && remaining > 0 {
                // There is dependency cycle ...
                // That should not be possible ...
                panic!("Failed to initialize the parser, found a cycle in the nullable variables");
            }
        }
    }

    /// Builds the dependency table between nullable variables
    fn build_nullables_dependencies(
        automaton: &RNGLRAutomaton,
        variables: &[Symbol]
    ) -> Vec<Option<Vec<usize>>> {
        let mut result = Vec::<Option<Vec<usize>>>::with_capacity(variables.len());
        for i in 0..variables.len() {
            let production = automaton.get_nullable_production(i);
            match production {
                None => {
                    result.push(None);
                }
                Some(nullable_production) => {
                    result.push(Some(RNGLRParser::build_nullable_dependencies_for(
                        nullable_production
                    )));
                }
            }
        }
        result
    }

    /// Gets the dependencies on nullable variables
    fn build_nullable_dependencies_for(production: &LRProduction) -> Vec<usize> {
        let mut result = Vec::<usize>::new();
        let mut i = 0;
        while i < production.bytecode.len() {
            let op_code = production.bytecode[i];
            i += 1;
            match get_op_code_base(op_code) {
                LR_OP_CODE_BASE_SEMANTIC_ACTION => {
                    i += 1;
                }
                LR_OP_CODE_BASE_ADD_VIRTUAL => {
                    i += 1;
                }
                LR_OP_CODE_BASE_ADD_NULLABLE_VARIABLE => {
                    result.push(production.bytecode[i + 1] as usize);
                    i += 1;
                }
                _ => {
                    break;
                }
            }
        }
        result
    }

    /// Builds the SPPF
    fn build_sppf(
        builder: &mut SPPFBuilder<'l>,
        actions: &mut FnMut(usize, Symbol, &SemanticBody),
        nullables: &Vec<usize>,
        generation: usize,
        production: &LRProduction,
        first: usize,
        path: GSSPath
    ) -> usize {
        let variable = builder.get_variables()[production.head];
        builder.reduction_prepare(first, &path, production.reduction_length);
        let mut i = 0;
        while i < production.bytecode.len() {
            let op_code = production.bytecode[i];
            i += 1;
            match get_op_code_base(op_code) {
                LR_OP_CODE_BASE_SEMANTIC_ACTION => {
                    let index = production.bytecode[i] as usize;
                    i += 1;
                    actions(index, variable, builder);
                }
                LR_OP_CODE_BASE_ADD_VIRTUAL => {
                    let index = production.bytecode[i] as usize;
                    i += 1;
                    builder.reduction_add_virtual(index, get_op_code_tree_action(op_code));
                }
                LR_OP_CODE_BASE_ADD_NULLABLE_VARIABLE => {
                    let index = production.bytecode[i] as usize;
                    i += 1;
                    builder
                        .reduction_add_nullable(nullables[index], get_op_code_tree_action(op_code));
                }
                _ => {
                    builder.reduction_pop(get_op_code_tree_action(op_code));
                }
            }
        }
        builder.reduce(
            generation,
            production.head,
            production.head_action == TREE_ACTION_REPLACE
        )
    }
}

impl<'l, 'a> Parser for RNGLRParser<'l, 'a> {
    fn parse(&mut self) {}
}
