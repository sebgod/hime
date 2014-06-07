#######################################################################
# Copyright (c) 2014 Laurent Wouters
# GNU Lesser General Public License
#######################################################################

__author__ = 'Laurent Wouters <lwouters@xowl.org>'


class ParseErrorType:
    """
    Specifies the type of error
    """
    UNEXPECTED_CHAR = 1
    UNEXPECTED_TOKEN = 2


class ParseError:
    """
    Represents an error in a parser
    """
    def __init__(self, p_type, position):
        """
        Initializes a new instance of the ParserError
        :param p_type: Error's type
        :param position: Error's position in the input
        :return: The error
        """
        self._type = p_type
        self._position = position
        self._message = None

    @property
    def type(self):
        """
        Gets the error's type
        :return: The error's type
        """
        return self._type

    @property
    def position(self):
        """
        Gets the error's position in the input
        :return: The error's position in the input
        """
        return self._position

    @property
    def message(self):
        """
        Gets the error's message
        :return: The error's message
        """
        return self._message

    def __str__(self):
        """
        Returns the string representation of this error
        :return: The string representation of this error
        """
        return self._message


class UnexpectedCharError(ParseError):
    """
    Represents an unexpected character error in the input stream of a lexer
    """
    def __init__(self, unexpected, position):
        """
        Initializes a new instance of the UnexpectedCharError class for the given character
        :param unexpected: The errorneous character (as a string)
        :param position: Error's position in the input
        :return: The error
        """
        super(ParseError).__init__(ParseErrorType.UNEXPECTED_CHAR, position)
        self._unexpected = unexpected
        builder = ['@']
        builder.append(str(position))
        builder.append(" Unexpected character '")
        builder.append(unexpected)
        builder.append("' (U+")
        builder.append(format(ord(unexpected), "X"))
        builder.append(')')
        self._message = ''.join(builder)

    @property
    def unexpected(self):
        """
        Gets the unexpected char
        :return: The unexpected char
        """
        return self._unexpected


class UnexpectedTokenError(ParseError):
    """
    Represents an unexpected token error in a parser
    """
    def __init__(self, token, position, expected):
        """
        Initializes a new instance of the UnexpectedTokenError class with a token and an array of expected names
        :param token: The unexpected token
        :param position: Error's position in the input
        :param expected: The expected terminals
        :return: The error
        """
        super(ParseError).__init__(ParseErrorType.UNEXPECTED_TOKEN, position)
        self._unexpected = token
        self._expected = expected
        builder = ['@']
        builder.append(str(position))
        builder.append(' Unexpected token "')
        builder.append(token.value)
        builder.append('"; expected: ')
        first = True
        for e in expected:
            if not first:
                builder.append(', ')
            builder.append(e.name)
            first = False
        self._message = ''.join(builder)

    @property
    def unexpected(self):
        """
        Gets the unexpected token
        :return: The unexpected token
        """
        return self._unexpected

    @property
    def expected(self):
        """
        Gets a list of the expected terminals
        :return: List of the expected terminals
        """
        return self._expected