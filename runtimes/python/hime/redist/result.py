#######################################################################
# Copyright (c) 2014 Laurent Wouters
# GNU Lesser General Public License
#######################################################################

__author__ = 'Laurent Wouters <lwouters@xowl.org>'


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
        return self.__ast is not None

    @property
    def errors(self):
        return self.__errors

    @property
    def input(self):
        return self.__text

    @property
    def root(self):
        return self.__ast