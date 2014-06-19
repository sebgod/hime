#######################################################################
# Copyright (c) 2014 Laurent Wouters
# GNU Lesser General Public License
#######################################################################

__author__ = 'Laurent Wouters <lwouters@xowl.org>'


class IParser:
    """
    Represents a parser
    """

    @property
    def variables(self):
        """
        Gets the variable symbols used by this parser
        :return: The variable symbols used by this parser
        """
        raise NotImplementedError('This method must be implemented by subclasses')

    @property
    def virtuals(self):
        """
        Gets the virtual symbols used by this parser
        :return: The virtual symbols used by this parser
        """
        raise NotImplementedError('This method must be implemented by subclasses')

    @property
    def recover_errors(self):
        """
        Gets whether the parser should try to recover from errors
        :return: Whether the parser should try to recover from errors
        """
        raise NotImplementedError('This method must be implemented by subclasses')

    @recover_errors.setter
    def recover_errors(self, recover):
        """
        Sets whether the parser should try to recover from errors
        :param recover: Whether the parser should try to recover from errors
        :return: None
        """
        raise NotImplementedError('This method must be implemented by subclasses')

    @property
    def debug_mode(self):
        """
        Gets a value indicating whether this parser is in debug mode
        :return: Whether this parser is in debug mode
        """
        raise NotImplementedError('This method must be implemented by subclasses')

    @debug_mode.setter
    def debug_mode(self, mode):
        """
        Sets a value indicating whether this parser is in debug mode
        :param mode: Whether this parser is in debug mode
        :return: None
        """
        raise NotImplementedError('This method must be implemented by subclasses')

    def parse(self):
        """
        Parses the input and returns the result
        :return: A ParseResult object containing the data about the result
        """
        raise NotImplementedError('This method must be implemented by subclasses')


class RewindableTokenStream:
    """
    Fast rewindable stream of token encapsulating a lexer
    """

    __RING_SIZE = 32

    def __init__(self, lexer):
        """
        Initializes the rewindable stream with the given lexer
        :param lexer: The encapsulated lexer
        :return: The stream
        """
        self.__lexer = lexer
        self.__ring = [None] * RewindableTokenStream.__RING_SIZE
        self.__ring_start = 0
        self.__ring_next = 0

    @property
    def is_empty(self):
        """
        Determines whether the ring buffer is empty
        :return: True if the ring is empty; otherwise false
        """
        return self.__ring_start == self.__ring_next

    def __read_ring(self):
        """
        Reads a token from the ring
        :return: The next token in the ring buffer
        """
        token = self.__ring[self.__ring_start]
        self.__ring_start += 1
        if self.__ring_start == RewindableTokenStream.__RING_SIZE:
            self.__ring_start = 0
        return token

    def __push(self, value):
        """
        Pushes the given token onto the ring
        :param value: The token to push
        :return: None
        """
        self.__ring[self.__ring_next] = value
        self.__ring_next += 1
        if self.__ring_next == RewindableTokenStream.__RING_SIZE:
            self.__ring_next = 0
        self.__ring_start = self.__ring_next

    def rewind(self, count):
        """
        Goes back in the stream
        :param count: Number of tokens to rewind
        :return: None
        """
        self.__ring_start -= count
        if self.__ring_start < 0:
            self.__ring_start += RewindableTokenStream.__RING_SIZE

    def next(self):
        """
        Gets the next token in the stream
        :return: The next token
        """
        if self.is_empty:
            return self.__read_ring()
        value = self.__lexer.next_token()
        self.__push(value)
        return value