#######################################################################
# Copyright (c) 2014 Laurent Wouters
# GNU Lesser General Public License
#######################################################################

__author__ = 'Laurent Wouters <lwouters@xowl.org>'


class AST:
    """
    Represents an Abstract Syntax Tree produced by a parser
    """
    @property
    def root(self):
        """
        Gets the root node of this tree
        :return: The root node of this tree
        """
        raise NotImplementedError('This method must be implemented by subclasses')

    def get_symbol(self, node):
        """
        Gets the symbol of the given node
        :param node: A node
        :return: The node's symbol
        """
        raise NotImplementedError('This method must be implemented by subclasses')

    def get_children_count(self, node):
        """
        Gets the number of children of the given node
        :param node: A node
        :return: The node's number of children
        """
        raise NotImplementedError('This method must be implemented by subclasses')

    def get_child(self, parent, i):
        """
        Gets the i-th child of the given node
        :param parent: A node
        :param i: The child's number
        :return: The i-th child
        """
        raise NotImplementedError('This method must be implemented by subclasses')

    def get_children(self, parent):
        """
        Gets an enumerator for the children of the given node
        :param parent: A node
        :return: An enumerator for the children
        """
        return _ChildIterator(self, parent)

    def get_position(self, node):
        """
        Gets the position in the input text of the given node
        :param node: A node
        :return: The position in the text
        """
        raise NotImplementedError('This method must be implemented by subclasses')


class ASTNode:
    """
    Represents a node in an Abstract Syntax Tree
    """
    def __init__(self, tree, index):
        """
        Initializes this node
        :param tree: The parent parse tree
        :param index: The index of this node in the parse tree
        :return: The node
        """
        self.__tree = tree
        self.__index = index

    @property
    def symbol(self):
        """
        Gets the symbol in this node
        :return: The symbol in this node
        """
        return self.__tree.get_symbol(self.__index)

    @property
    def position(self):
        """
        Gets the position in the input text of this node
        :return: The position in the input text of this node
        """
        return self.__tree.get_position(self.__index)

    @property
    def children(self):
        """
        Gets the children of this node
        :return: The children of this node
        """
        return ASTFamily(self.__tree, self.__index)

    def __str__(self):
        """
        Gets a string representation of this node
        :return: The string representation of the associated symbol
        """
        return str(self.__tree.get_symbol(self.__index))


class ASTFamily:
    """
    Represents a family of children for an ASTNode
    """
    def __init__(self, tree, parent):
        """
        Initializes this family
        :param tree: The parent parse tree
        :param parent: The index of the parent node in the parse tree
        :return: The family
        """
        self.__tree = tree
        self.__parent = parent

    @property
    def count(self):
        """
        Gets the number of children
        :return: The number of children
        """
        return self.__tree.get_children_count(self.__parent)

    def __getitem__(self, item):
        """
        Gets the i-th child
        :param item: The index of the child
        :return: The child at the given index
        """
        return self.__tree.get_child(self.__parent, item)

    def __iter__(self):
        """
        Gets an enumeration of the children
        :return: An enumeration of the children
        """
        return _ChildIterator(self.__tree, self.__parent)


class _ChildIterator:
    """
    Implements an iterator over the children of a node
    """
    def __init__(self, tree, parent):
        """
        Initializes this iterator
        :param tree: The parent tree
        :param parent: The parent node
        :return: The iterator
        """
        self.__tree = tree
        self.__parent = parent
        self.__index = 0
        self.__count = tree.get_children_count(parent)

    def next(self):
        """
        Gets the next element
        :return: The next element
        """
        if self.__index == self.__count:
            raise StopIteration
        child = self.__tree.get_child(self.__parent, self.__index)
        self.__index += 1
        return child