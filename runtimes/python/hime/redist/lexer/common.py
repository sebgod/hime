"""
Common API for lexers
"""

__author__ = "Laurent Wouters <lwouters@xowl.org>"
__copyright__ = "Copyright 2014"
__license__ = "LGPL v3+"


class ILexer:
    """
    Represents a lexer for a text stream
    """

    @property
    def terminals(self):
        """
        Gets the terminals matched by this lexer
        :return: The terminals matched by this lexer
        """
        raise NotImplementedError('This method must be implemented by subclasses')

    @property
    def output(self):
        """
        Gets the lexer's output as a tokenized text
        :return: The lexer's output as a tokenized text
        """
        raise NotImplementedError('This method must be implemented by subclasses')

    def register_error_handler(self, handler):
        """
        Register the handler for lexical errors
        :param handler: The handler for lexical errors
        :return: None
        """
        raise NotImplementedError('This method must be implemented by subclasses')

    def next_token(self):
        """
        Gets the next token in the input
        :return: The next token in the input
        """
        raise NotImplementedError('This method must be implemented by subclasses')


class Automaton:
    """
    Data structure for a text lexer automaton
    """

    def __init__(self, content):
        """
        Initializes a new automaton from the given binary content
        :param content: The binary content to load from
        :return: The automaton
        """
        self.__count = content.read_int()
        self.__table = content.read_unsigned_ints(self.__count)
        self.__states = content.read_unsigned_shorts((len(content) - self.__count * 4 - 4) // 2)

    @property
    def states_count(self):
        """
        Gets the number of states in this automaton
        :return: The number of states in this automaton
        """
        return self.__count

    def get_offset_of(self, state):
        """
        Get the offset of the given state in the table
        :param state: The DFA which offset shall be retrieved
        :return: The offset of the given DFA state
        """
        return self.__table[state]

    def get_state_recognized_terminal(self, offset):
        """
        Gets the recognized terminal index for the DFA at the given offset
        :param offset: The DFA state's offset
        :return: The index of the terminal recognized at this state, or 0xFFFF if none
        """
        return self.__states[offset]

    def is_state_dead_end(self, offset):
        """
        Checks whether the DFA state at the given offset does not have any transition
        :param offset: The DFA state's offset
        :return: True if the state at the given offset has no transition
        """
        return self.__states[offset + 1] == 0

    def get_state_bulk_transitions_count(self, offset):
        """
        Gets the number of non-cached transitions from the DFA state at the given offset
        :param offset: The DFA state's offset
        :return: The number of non-cached transitions
        """
        return self.__states[offset + 2] == 0

    def get_state_cache_transition(self, offset, value):
        """
        Gets the transition from the DFA state at the given offset with the input value (max 255)
        :param offset: The DFA state's offset
        :param value: The input value
        :return: The state obtained by the transition, or 0xFFFF if none is found
        """
        return self.__states[offset + 3 + value]

    def get_state_bulk_transition(self, offset, value):
        """
        Gets the transition from the DFA state at the given offset with the input value (at least 256)
        :param offset: The DFA state's offset
        :param value: The input value
        :return: The state obtained by the transition, or 0xFFFF if none is found
        """
        offset += 259
        for i in range(self.__states[offset + 2]):
            if self.__states[offset] <= value <= self.__states[offset + 1]:
                return self.__states[offset + 2]
            offset += 3
        return 0xFFFF