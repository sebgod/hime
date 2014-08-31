"""
Implementation of pre-fetching lexers
"""

from hime.redist import *
from .common import *

__author__ = "Laurent Wouters <lwouters@xowl.org>"
__copyright__ = "Copyright 2014"
__license__ = "LGPL v3+"


def _is_line_ending(c1, c2):
    """
    Determines whether [c1, c2] form a line ending sequence

    Recognized sequences are:
    * [U+000D, U+000A] (this is Windows-style \r \n)
    * [U+????, U+000A] (this is unix style \n)
    * [U+000D, U+????] (this is MacOS style \r, without \n after)
    Others:
    * [?, U+000B], [?, U+000C], [?, U+0085], [?, U+2028], [?, U+2029]
    :param c1: First character
    :param c2: Second character
    :return: True if this is a line ending sequence
    """
    # other characters
    if c2 == 0x000B or c2 == 0x000C or c2 == 0x0085 or c2 == 0x2028 or c2 == 0x2029:
        return True
    # matches [\r, \n] [\r, ??] and  [??, \n]
    if c1 == 0x000D or c2 == 0x000A:
        return True
    return False


class _PrefetchedText(TokenizedText):
    """
    Stores the full content of an input lexer

    All line numbers and column numbers are 1-based.
    Indices in the content are 0-based.
    """

    class Cell:
        """
        Represents the metadata of a token
        """

        def __init__(self, terminal, start, length):
            """
            Initializes this cell
            :param terminal: The terminal's index
            :param start: Start index of the text
            :param length: Length of the token
            :return: The cell
            """
            self.terminal = terminal
            self.start = start
            self.length = length

    def __init__(self, terminals, content):
        """
        Initializes this text
        :param terminals: The terminal symbols
        :param content: The full lexer's input as a string
        :return: The text
        """
        self.__content = content
        self.__terminals = terminals
        self.__cells = []
        self.__lines = []

    def find_lines(self):
        """
        Finds all the lines in this content
        :return: None
        """
        self.__lines.append(0)
        c2 = 0
        for i in range(len(self.__content)):
            c1 = c2
            c2 = ord(self.__content[i])
            # is c1 c2 a line ending sequence?
            if _is_line_ending(c1, c2):
                # are we late to detect MacOS style?
                if c1 == 0x000D and c2 != 0x000A:
                    self.__lines.append(i)
                else:
                    self.__lines.append(i + 1)

    def add_token(self, terminal, start, length):
        """
        Adds a detected token in this text
        :param terminal: Index of the matched terminal
        :param start: Start index in the text
        :param length: Length of the token
        :return: None
        """
        self.__cells.append(_PrefetchedText.Cell(terminal, start, length))

    def get_token_at(self, index):
        """
        Gets the token at the specified index
        :param index: A token's index
        :return: The token at the specified index
        """
        cell = self.__cells[index]
        return Token(self.__terminals[cell.terminal].id, index)

    @property
    def line_count(self):
        """
        Gets the number of lines
        :return: The number of lines
        """
        if len(self.__lines) == 0:
            self.find_lines()
        return len(self.__lines)

    @property
    def size(self):
        """
        Gets the size in number of characters
        :return: The size in number of characters
        """
        return len(self.__content)

    def get_value(self, index, length):
        """
        Gets the substring beginning at the given index with the given length
        :param index: Index of the substring from the start
        :param length: Length of the substring
        :return: The substring
        """
        if length == 0:
            return ""
        return self.__content[index:index + length]

    def get_line_index(self, line):
        """
        Gets the starting index of the i-th line
        The line numbering is 1-based
        :param line: The line number
        :return: The starting index of the line
        """
        if len(self.__lines) == 0:
            self.find_lines()
        return self.__lines[line - 1]

    def get_line_length(self, line):
        """
        Gets the length of the i-th line
        The line numbering is 1-based
        :param line: The line number
        :return: The length of the line
        """
        if len(self.__lines) == 0:
            self.find_lines()
        if line == len(self.__lines):
            return len(self.__content) - self.__lines[len(self.__lines) - 1]
        return self.__lines[line] - self.__lines[line - 1]

    def get_line_content(self, line):
        """
        Gets the position at the given index
        :param line: Index from the start
        :return: The position (line and column) at the index
        """
        return self.get_value(self.get_line_index(line), self.get_line_length(line))

    def get_position_at(self, index):
        """
        Gets the position at the given index
        :param index: Index from the start
        :return: The position (line and column) at the index
        """
        line = self._find_line_at(index)
        return TextPosition(line + 1, index - self.__lines[line] + 1)

    def get_context(self, position):
        """
        Gets the context description for the current text at the specified position
        :param position: The position in this text
        :return: The context description
        """
        content = self.get_line_content(position.line)
        if len(content) == 0:
            return Context("", "^")
        end = len(content) - 1
        while end != 1 and (content[end] == '\n' or content[end] == '\r'):
            end -= 1
        start = 0
        while start < end and content[start].isspace():
            start += 1
        if position.column - 1 < start:
            start = 0
        if position.column - 1 > end:
            end = len(content) - 1
        builder = []
        for i in range(start, position.column - 1):
            builder.append('\t' if content[i] == '\t' else ' ')
        builder.append('^')
        return Context(content[start:end], ''.join(builder))

    def _find_line_at(self, index):
        """
        Finds the 0-based number of the line at the given index in the content
        :param index: The index within this content
        :return: The 0-based number of the line at the given index in the content
        """
        if len(self.__cells) == 0:
            self.find_lines()
        for i in range(1, len(self.__lines)):
            if index < self.__lines[i]:
                return i - 1
        return len(self.__lines) - 1

    @property
    def token_count(self):
        """
        Gets the number of tokens in this text
        :return: The number of tokens in this text
        """
        return len(self.__cells)

    def at(self, index):
        """
        Gets the token at the given index
        :param index: An index
        :return: The token
        """
        cell = self.__cells[index]
        terminal = self.__terminals[cell.terminal]
        return Symbol(terminal.id, terminal.name, self.get_value(cell.start, cell.length))

    def get_position(self, index):
        """
        Gets the position of the token at the given index
        :param index: The index of a token
        :return: The position (line and column) of the token
        """
        cell = self.__cells[index]
        return self.get_position_at(cell.start)


class _Match:
    """
    Represents a match in the input
    """

    def __init__(self, terminal):
        self.terminal = terminal
        self.length = 0


class PrefetchedLexer(ILexer):
    """
    Represents a lexer for a prefetched piece of text, i.e. the text is already in memory
    """

    def __init__(self, automaton, terminals, separator, input):
        """
        Initializes a new instance of the Lexer class with the given input
        :param automaton: DFA automaton for this lexer
        :param terminals: Terminals recognized by this lexer
        :param separator: SID of the separator token
        :param input: Input to this lexer
        :return: The lexer
        """
        self.__automaton = automaton
        self.__terminals = terminals
        self.__separator = separator
        self.__input = input
        self.__text = _PrefetchedText(terminals, input)
        self.__inputIndex = 0
        self.__tokenIndex = -1
        self.__handler = None

    @property
    def terminals(self):
        """
        Gets the terminals matched by this lexer
        :return: The terminals matched by this lexer
        """
        return self.__terminals

    @property
    def output(self):
        """
        Gets the lexer's output as a tokenized text
        :return: The lexer's output as a tokenized text
        """
        return self.__text

    def register_error_handler(self, handler):
        """
        Register the handler for lexical errors
        :param handler: The handler for lexical errors
        :return: None
        """
        self.__handler = handler

    def next_token(self):
        """
        Gets the next token in the input
        :return: The next token in the input
        """
        if self.__tokenIndex == -1:
            self._find_tokens()
            self.__tokenIndex = 0
        if self.__tokenIndex >= self.__text.token_count:
            return Token(SID_EPSILON, 0)
        token = self.__text.get_token_at(self.__tokenIndex)
        self.__tokenIndex += 1
        return token

    def _find_tokens(self):
        """
        Finds all the tokens in the lexer's input
        :return: None
        """
        while True:
            match = self._run_dfa()
            if match.length != 0:
                if self.__terminals[match.terminal].id != self.__separator:
                    self.__text.add_token(match.terminal, self.__inputIndex, match.length)
                self.__inputIndex += match.length
                continue
            if match.terminal == 0:
                # This is the epsilon terminal, failed to match anything
                position = self.__text.get_position_at(self.__inputIndex)
                unexpected = self.__input[self.__inputIndex]
                self.__handler(UnexpectedCharError(unexpected, position))
                continue
            # This is the dollar terminal, at the end of the input
            self.__text.add_token(match.terminal, self.__inputIndex, match.length)
            return

    def _run_dfa(self):
        """
        Runs the lexer's DFA to match a terminal in the input ahead
        :return: The matched terminal and length
        """
        if self.__inputIndex == len(self.__input):
            return _Match(1)

        result = _Match(0)
        state = 0
        i = self.__inputIndex

        while state != 0xFFFF:
            offset = self.__automaton.get_offset_of(state)
            # Is this state a matching state ?
            terminal = self.__automaton.get_state_recognized_terminal(offset)
            if terminal != 0xFFFF:
                result.terminal = terminal
                result.length = i - self.__inputIndex
            # No further transition => exit
            if self.__automaton.is_state_dead_end(offset):
                break
            # At end of the buffer
            if i == len(self.__input):
                break
            current = ord(self.__input[i])
            i += 1
            # Try to find a transition from this state with the read character
            if current <= 255:
                state = self.__automaton.get_state_cache_transition(offset, current)
            elif current <= 0xFFFF:
                state = self.__automaton.get_state_bulk_transition(offset, current)
            else:
                # not in Unicode plane 0 ...
                temp = current - 0x10000
                lead = (temp >> 10) + 0xD800
                trail = (temp & 0x03FF) + 0xDC00
                state = self.__automaton.get_state_bulk_transition(offset, lead)
                if state != 0xFFFF:
                    state = self.__automaton.get_state_bulk_transition(offset, trail)
        return result