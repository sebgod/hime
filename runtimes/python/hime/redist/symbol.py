#######################################################################
# Copyright (c) 2014 Laurent Wouters
# GNU Lesser General Public License
#######################################################################

__author__ = 'Laurent Wouters <lwouters@xowl.org>'


# Symbol ID of the Epsilon terminal
SID_EPSILON = 1

# Symbol ID of the Dollar terminal
SID_DOLLAR = 2


class Symbol:
    """
    Represents a symbol in an AST
    """

    def __init__(self, sid, name, value):
        """
        Initializes this symbol
        :param sid: The symbol's unique identifier
        :param name: The symbol's name
        :param value: The symbol's value
        :return: The symbol
        """
        self.__id = sid
        self.__name = name
        self.__value = value

    @property
    def id(self):
        """
        Gets the symbol's unique identifier
        :return: The symbol's unique identifier
        """
        return self.__id

    @property
    def name(self):
        """
        Gets the symbol' s name
        :return: The symbol's name
        """
        return self.__name

    @property
    def value(self):
        """
        Gets the symbol' s value
        :return: The symbol's value
        """
        return self.__value

    def __str__(self):
        """
        Gets a string representation of this symbol
        :return: The value of this symbol
        """
        return self.__value


class Token:
    """
    Represents a token as an output element of a lexer
    """
    def __init__(self, sid, index):
        self.__sid = sid
        self.__index = index

    @property
    def symbol_id(self):
        """
        Gets the id of the terminal symbol associated to this token
        :return: The id of the terminal symbol associated to this token
        """
        return self.__sid

    @property
    def index(self):
        """
        Gets the index of this token in a lexer's stream of token
        :return: The index of this token in a lexer's stream of token
        """
        return self.__index