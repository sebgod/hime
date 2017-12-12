/*******************************************************************************
 * Copyright (c) 2017 Association Cénotélie (cenotelie.fr)
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation, either version 3
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General
 * Public License along with this program.
 * If not, see <http://www.gnu.org/licenses/>.
 ******************************************************************************/

//! Module for text-handling APIs

pub mod utf16;
pub mod impls;

use std::result::Result;
use std::fmt::Display;
use std::fmt::Formatter;
use std::fmt::Error;

/// `Utf16C` represents a single UTF-16 code unit.
/// A UTF-16 code unit is always represented as a 16 bits unsigned integer.
/// UTF-16 code units may not represent by themselves valid Unicode code points (characters).
/// A Unicode code point (a character) is a 32-bits unsigned integer in the ranges:
/// U+0000 to U+D7FF and U+E000 to U+FFFF and U+10000 to U+10FFFF.
/// Unicode code points in the range U+D800 to U+DFFF are reserved and cannot be used.
/// UTF-16 can be used to encode a single Unicode code point in either one or two UTF-16 code units.
///
/// See [UTF-16](https://en.wikipedia.org/wiki/UTF-16) for more details.
pub type Utf16C = u16;

/// Represents a span of text in an input as a starting index and length
#[derive(Copy, Clone)]
pub struct TextSpan {
    /// The starting index
    pub index: usize,
    /// The length
    pub length: usize
}

/// Implementation of `Display` for `TextSpan`
impl Display for TextSpan {
    fn fmt(&self, f: &mut Formatter) -> Result<(), Error> {
        write!(f, "@{}+{}", self.index, self.length)
    }
}

/// Represents a position in term of line and column in a text input
#[derive(Copy, Clone)]
pub struct TextPosition {
    /// The line number
    pub line: usize,
    /// The column number
    pub column: usize
}

/// Implementation of `Display` for `TextPosition`
impl Display for TextPosition {
    fn fmt(&self, f: &mut Formatter) -> Result<(), Error> {
        write!(f, "({}, {})", self.line, self.column)
    }
}

/// Represents the context description of a position in a piece of text.
/// A context is composed of two pieces of text, the line content and the pointer.
/// For example, given the piece of text:
///
/// ```
/// public Struct Context
/// ```
///
/// A context pointing to the second word will look like:
///
/// ```
/// content = "public Struct Context"
/// pointer = "       ^^^^^^"
/// ```
pub struct TextContext {
    /// The text content being represented
    pub content: String,
    /// The pointer textual representation
    pub pointer: String
}


/// Represents the input of parser with some metadata for line endings
/// All line numbers and column numbers are 1-based.
/// Indices in the content are 0-based.
pub trait Text {
    /// Gets the number of lines
    fn get_line_count(&self) -> usize;

    /// Gets the size in number of characters
    fn get_size(&self) -> usize;

    /// Gets whether the specified index is after the end of the text represented by this object
    fn is_end(&self, index: usize) -> bool;

    /// Gets the character at the specified index
    fn get_at(&self, index: usize) -> Utf16C;

    /// Gets the substring beginning at the given index with the given length
    fn get_value(&self, index: usize, length: usize) -> String;

    /// Get the substring corresponding to the specified span
    fn get_value_for(&self, span: TextSpan) -> String {
        self.get_value(span.index, span.length)
    }

    /// Gets the starting index of the i-th line
    fn get_line_index(&self, line: usize) -> usize;

    /// Gets the length of the i-th line
    fn get_line_length(&self, line: usize) -> usize;

    /// Gets the string content of the i-th line
    fn get_line_content(&self, line: usize) -> String {
        self.get_value(self.get_line_index(line), self.get_line_length(line))
    }

    /// Gets the position at the given index
    fn get_position_at(&self, index: usize) -> TextPosition;

    /// Gets the context description for the current text at the specified position
    fn get_context_at(&self, position: TextPosition) -> TextContext {
        self.get_context_for(position, 1)
    }

    /// Gets the context description for the current text at the specified position
    fn get_context_for(&self, position: TextPosition, length: usize) -> TextContext;

    /// Gets the context description for the current text at the specified span
    fn get_context_of(&self, span: TextSpan) -> TextContext {
        let position = self.get_position_at(span.index);
        return self.get_context_for(position, span.length);
    }
}
