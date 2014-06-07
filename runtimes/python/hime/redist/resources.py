#######################################################################
# Copyright (c) 2014 Laurent Wouters
# GNU Lesser General Public License
#######################################################################

__author__ = 'Laurent Wouters <lwouters@xowl.org>'

from os.path import dirname
from os.path import join


def load_resource(module, name):
    """
    Loads the specified file as a binary resource
    :param name: The name of the file to load
    :return: The byte array of
    """
    file = open(join(dirname(module), name), 'rb')
    content = file.read()
    file.close()
    return content