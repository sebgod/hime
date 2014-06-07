#######################################################################
# Copyright (c) 2014 Laurent Wouters
# GNU Lesser General Public License
#######################################################################

__author__ = 'Laurent Wouters <lwouters@xowl.org>'


class TextPosition:
    """
    Represents a position in term of line and column in a text input
    """
    def __init__(self, line, column):
        """
        Initializes this position with the given line and column numbers
        :param line: The line number
        :param column: The column number
        :return: The text position
        """
        self.__line = line
        self.__column = column

    @property
    def line(self):
        """
        Gets the line number
        :return: The line number
        """
        return self.__line

    @property
    def column(self):
        """
        Gets the column number
        :return: The column number
        """
        return self.__column

    def __str__(self):
        """
        Gets a string representation of this position
        :return: The string representation of this position
        """
        return "(" + self.__line + ", " + self.__column + ")"


class Text:
    """
    Represents the input of parser with some metadata for line endings

    All line numbers and column numbers are 1-based.
    Indices in the content are 0-based.
    """
    @property
    def line_count(self):
        """
        Gets the number of lines
        :return: The number of lines
        """
        raise NotImplementedError('This method must be implemented by subclasses')

    @property
    def size(self):
        """
        Gets the size in number of characters
        :return: The size in number of characters
        """
        raise NotImplementedError('This method must be implemented by subclasses')

    def get_value(self, index, length):
        """
        Gets the substring beginning at the given index with the given length
        :param index: Index of the substring from the start
        :param length: Length of the substring
        :return: The substring
        """
        raise NotImplementedError('This method must be implemented by subclasses')

    def get_line_index(self, line):
        """
        Gets the starting index of the i-th line
        The line numbering is 1-based
        :param line: The line number
        :return: The starting index of the line
        """
        raise NotImplementedError('This method must be implemented by subclasses')

    def get_line_length(self, line):
        """
        Gets the length of the i-th line
        The line numbering is 1-based
        :param line: The line number
        :return: The length of the line
        """
        raise NotImplementedError('This method must be implemented by subclasses')

    def get_line_content(self, line):
        """
        Gets the position at the given index
        :param line: Index from the start
        :return: The position (line and column) at the index
        """
        raise NotImplementedError('This method must be implemented by subclasses')

    def get_position_at(self, index):
        """
        Gets the position at the given index
        :param index: Index from the start
        :return: The position (line and column) at the index
        """
        raise NotImplementedError('This method must be implemented by subclasses')

    def get_context(self, position):
        """
        Gets the context description for the current text at the specified position
        :param position: The position in this text
        :return: The context description
        """
        raise NotImplementedError('This method must be implemented by subclasses')


class TokenizedText(Text):
    """
    Represents the output of a lexer as a tokenized text
    """
    @property
    def token_count(self):
        """
        Gets the number of tokens in this text
        :return: The number of tokens in this text
        """
        raise NotImplementedError('This method must be implemented by subclasses')

    def at(self, index):
        """
        Gets the token at the given index
        :param index: An index
        :return: The token
        """
        raise NotImplementedError('This method must be implemented by subclasses')

    def get_position(self, index):
        """
        Gets the position of the token at the given index
        :param index: The index of a token
        :return: The position (line and column) of the token
        """
        raise NotImplementedError('This method must be implemented by subclasses')