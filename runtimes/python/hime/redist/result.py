"""
Common API for the representation of parsing results
"""

__author__ = "Laurent Wouters <lwouters@xowl.org>"
__copyright__ = "Copyright 2014"
__license__ = "LGPL v3+"


class ParseResult:
    """
    Represents the output of a parser
    """

    def __init__(self, errors, text, ast):
        """
        Initializes this result
        :param errors: The list of errors
        :param text: The parsed text
        :param ast: The produced AST
        :return: The parse result
        """
        self.__errors = errors
        self.__text = text
        self.__ast = ast

    @property
    def is_success(self):
        """
        Gets whether the parsing was successful
        :return: True if the parsing was successful, false otherwise
        """
        return self.__ast is not None

    @property
    def errors(self):
        """
        Gets the errors encountered during the parsing
        :return: The encountered errors
        """
        return self.__errors

    @property
    def input(self):
        """
        Gets the parser's original input
        :return: The parser's original input
        """
        return self.__text

    @property
    def root(self):
        """
        Gets the root of the produced AST
        :return: The root of the produced AST
        """
        return self.__ast