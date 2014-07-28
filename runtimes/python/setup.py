#!/usr/bin/env python
"""
Setup script for the hime.redist Python package
"""

from distutils.core import setup

__author__ = "Laurent Wouters <lwouters@xowl.org>"
__copyright__ = "Copyright 2014"
__license__ = "LGPL v3+"

setup(
    name = 'hime.redist',
    packages = [ 'hime',
                 'hime/redist',
                 'hime/redist/lexer' ],
    version = '1.1.0',
    description = 'Redistributable Python runtime for the parsers generated with Hime',
    author = 'Laurent Wouters',
    author_email = 'lwouters@xowl.org',
    url = 'https://bitbucket.org/laurentw/hime',
    classifiers = [ 'Programming Language :: Python',
                    'Programming Language :: Python :: 3.3',
                    'Development Status :: 4 - Beta',
                    'Intended Audience :: Developers',
                    'License :: OSI Approved :: GNU Lesser General Public License v3 or later (LGPLv3+)',
                    'Natural Language :: English',
                    'Operating System :: OS Independent',
                    'Topic :: Software Development :: Compilers',
                    'Topic :: Software Development :: Interpreters',
                    'Topic :: Text Processing :: General' ]
)