"""
Implementation of the AST-related APIs for the parsers
"""

from hime.redist import AST
from hime.redist import ASTNode
from hime.redist import SymbolType
from hime.redist import TreeAction
from hime.redist import get_symbol_type
from hime.redist import get_symbol_index

__author__ = "Laurent Wouters <lwouters@xowl.org>"
__copyright__ = "Copyright 2014"
__license__ = "LGPL v3+"


class ASTImpl(AST):
    """
    Represents a base class for AST implementations
    """

    class Node:
        """
        Represents a node in this AST
        """

        def __init__(self, symbol):
            """
            Initializes this node
            :param symbol: The node's symbol
            :return: The node
            """
            self.symbol = symbol
            self.count = 0
            self.first = -1

    def __init__(self, text, variables, virtuals):
        """
        Initializes this AST
        :param text: The tokenized text input
        :param variables: Table of parser variables
        :param virtuals: Table of parser virtuals
        :return: The AST
        """
        self._table_tokens = text
        self._table_variables = variables
        self._table_virtuals = virtuals
        self._nodes = []
        self._root = -1

    @property
    def root(self):
        """
        Gets the root node of this tree
        :return: The root node of this tree
        """
        return ASTNode(self, self._root)

    def get_symbol(self, node):
        """
        Gets the symbol of the given node
        :param node: A node
        :return: The node's symbol
        """
        return self.symbol_for(self._nodes[node].symbol)

    def get_children_count(self, node):
        """
        Gets the number of children of the given node
        :param node: A node
        :return: The node's number of children
        """
        return self._nodes[node].count

    def get_child(self, parent, i):
        """
        Gets the i-th child of the given node
        :param parent: A node
        :param i: The child's number
        :return: The i-th child
        """
        raise NotImplementedError('This method must be implemented by subclasses')

    def get_position(self, node):
        """
        Gets the position in the input text of the given node
        :param node: A node
        :return: The position in the text
        """
        reference = self._nodes[node].symbol
        if get_symbol_type(reference) == SymbolType.TOKEN:
            return self._table_tokens.get_position(get_symbol_index(reference))

    def symbol_for(self, reference):
        """
         Gets the symbol corresponding to the given symbol reference
        :param reference: A symbol reference
        :return: The corresponding symbol
        """
        stype = get_symbol_type(reference)
        index = get_symbol_index(reference)
        if stype == SymbolType.TOKEN:
            return self._table_tokens.at(index)
        if stype == SymbolType.VARIABLE:
            return self._table_variables[index]
        if stype == SymbolType.VIRTUAL:
            return self._table_virtuals[index]
        return None

    def store(self, nodes, index, count):
        """
        Stores some children nodes in this AST
        :param nodes: The nodes to store
        :param index: The starting index of the nodes in the data to store
        :param count: The number of nodes to store
        :return: The index of the first inserted node in this tree
        """
        result = len(self._nodes)
        self._nodes.extend(nodes[index:index + count])
        return result


class SimpleAST(ASTImpl):
    """
    Represents a simple AST with a tree structure

    The nodes are stored in sequential arrays where the children of a node are an inner sequence.
    The linkage is represented by each node storing its number of children and the index of its first child.
    """

    def __init__(self, text, variables, virtuals):
        """
        Initializes this AST
        :param text: The tokenized text input
        :param variables: Table of parser variables
        :param virtuals: Table of parser virtuals
        :return: The AST
        """
        ASTImpl.__init__(self, text, variables, virtuals)

    def get_child(self, parent, i):
        """
        Gets the i-th child of the given node
        :param parent: A node
        :param i: The child's number
        :return: The i-th child
        """
        return ASTNode(self, self._nodes[parent].first + i)

    def store_root(self, node):
        """
        Stores the root of this tree
        :param node: The root
        :return: None
        """
        self._nodes.append(node)


class GraphAST(ASTImpl):
    """
    Represents an AST using a graph structure
    """

    def __init__(self, text, variables, virtuals):
        """
        Initializes this AST
        :param text: The tokenized text input
        :param variables: Table of parser variables
        :param virtuals: Table of parser virtuals
        :return: The AST
        """
        ASTImpl.__init__(self, text, variables, virtuals)
        self._adjacency = []

    def get_child(self, parent, i):
        """
        Gets the i-th child of the given node
        :param parent: A node
        :param i: The child's number
        :return: The i-th child
        """
        return ASTNode(self, self._adjacency[self._nodes[parent].first + i])

    def store_symbol(self, symbol):
        """
        Stores the specified symbol in this AST as a new node
        :param symbol: The symbol to store
        :return: The index of the new node
        """
        result = len(self._nodes)
        self._nodes.append(ASTImpl.Node(symbol))
        return result

    def store_adjacents(self, adjacents, count):
        """
        Stores some adjacency data in this graph AST
        :param adjacents: A buffer of adjacency data
        :param count: The number of adjacents to store
        :return: The index of the data stored in this graph
        """
        result = len(self._adjacency)
        self._adjacency.extend(adjacents[0:count - 1])
        return result

    def copy_node(self, node):
        """
        Copies the provided node (and its adjacency data)
        :param node: The node to copy
        :return: The index of the copy
        """
        result = len(self._nodes)
        origin = self._nodes[node]
        clone = ASTImpl.Node(origin.symbol)
        self._nodes.append(clone)
        if origin.count != 0:
            clone.first = len(self._adjacency)
            clone.count = origin.count
            self._adjacency.extend(self._adjacency[origin.first:origin.count - 1])
        return result

    def get_adjacency(self, node, buffer, index):
        """
        Gets the adjacency data for the specified node
        :param node: The node to retrieve the adjacency data of
        :param buffer: The buffer to store the retrieved data in
        :param index: The starting index in the provided buffer
        :return: The number of adjacents
        """
        temp = self._nodes[node]


class SubTree:
    """
    Represents a sub-tree in an AST
    """

    def __init__(self, pool):
        """
        Instantiates a new sub-tree attached to the given pool
        :param pool: The pool to attach this object to
        :return: This sub-tree
        """
        self.__pool = pool
        self.__nodes = []
        self.__actions = []

    def get_label_at(self, index):
        """
        Gets the label of the node at the given index
        :param index: The index within the buffer
        :return: The label in the buffer
        """
        return self.__nodes[index].symbol

    def get_action_at(self, index):
        """
        Gets the tree action applied onto the node at the given index
        :param index: The index within the buffer
        :return: The tree action in the buffer
        """
        return self.__actions[index]

    def set_action_at(self, index, action):
        """
        Sets the tree action applied onto the node at the given index
        :param index: The index within the buffer
        :param action: The tree action to apply
        :return: None
        """
        self.__actions[index] = action

    def get_children_count_at(self, index):
        """
        Gets the number of children of the node at the given index
        :param index: The index within the buffer
        :return: The number of children
        """
        return self.__nodes[index].count

    def set_children_count_at(self, index, count):
        """
        Sets the number of children of the node at the given index
        :param index: The index within the buffer
        :param count: The number of children
        :return: None
        """
        self.__nodes[index].count = count

    @property
    def size(self):
        """
        Gets the total number of nodes in this sub-tree
        :return: The total number of nodes in this sub-tree
        """
        if self.__actions[0] != TreeAction.REPLACE:
            return self.__nodes[0].count + 1
        size = 1
        for i in range(0, self.__nodes[0].count):
            size += self.__nodes[size].count + 1
        return size