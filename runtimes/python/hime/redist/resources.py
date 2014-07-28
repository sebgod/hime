"""
Utility API for handling binary resources for the lexers and parsers
"""

from os.path import dirname
from os.path import join
from struct import unpack_from

__author__ = "Laurent Wouters <lwouters@xowl.org>"
__copyright__ = "Copyright 2014"
__license__ = "LGPL v3+"


class BinaryBuffer:
    """
    Represents a binary buffer to load data from
    """

    def __init__(self, module, name):
        """
        Initializes this buffer from the specified resource
        :param module: The module loading the resource
        :param name: The name of the resource to load from
        :return: The buffer
        """
        file = open(join(dirname(module), name), 'rb')
        self.__content = file.read()
        self.__offset = 0
        file.close()

    def __len__(self):
        """
        Gets the total length of this buffer in bytes
        :return: The total length of this buffer in bytes
        """
        return len(self.__content)

    def read_byte(self):
        """
        Reads a single byte from this buffer
        :return: The next byte
        """
        result = unpack_from('<b', self.__content, self.__offset)[0]
        self.__offset += 1
        return result

    def read_unsigned_byte(self):
        """
        Reads a single unsigned byte from this buffer
        :return: The next byte
        """
        result = unpack_from('<B', self.__content, self.__offset)[0]
        self.__offset += 1
        return result

    def read_short(self):
        """
        Reads a single short from this buffer
        :return: The next short
        """
        result = unpack_from('<h', self.__content, self.__offset)[0]
        self.__offset += 2
        return result

    def read_unsigned_short(self):
        """
        Reads a single unsigned short from this buffer
        :return: The next short
        """
        result = unpack_from('<H', self.__content, self.__offset)[0]
        self.__offset += 2
        return result

    def read_int(self):
        """
        Reads a single integer from this buffer
        :return: The next integer
        """
        result = unpack_from('<i', self.__content, self.__offset)[0]
        self.__offset += 4
        return result

    def read_unsigned_int(self):
        """
        Reads a single unsigned integer from this buffer
        :return: The next integer
        """
        result = unpack_from('<I', self.__content, self.__offset)[0]
        self.__offset += 4
        return result

    def read_unsigned_shorts(self, count):
        """
        Reads a multiple shorts from this buffer
        :param count: The number of shorts
        :return: The next shorts
        """
        result = unpack_from('<' + 'H' * count, self.__content, self.__offset)
        self.__offset += count * 2
        return result

    def read_unsigned_ints(self, count):
        """
        Reads a multiple integers from this buffer
        :param count: The number of integers
        :return: The next integers
        """
        result = unpack_from('<' + 'I' * count, self.__content, self.__offset)
        self.__offset += count * 4
        return result