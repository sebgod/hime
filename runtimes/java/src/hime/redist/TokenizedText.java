/**********************************************************************
 * Copyright (c) 2014 Laurent Wouters and others
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
 *
 * Contributors:
 *     Laurent Wouters - lwouters@xowl.org
 **********************************************************************/

package hime.redist;

/**
 * Represents the output of a lexer as a tokenized text
 */
public interface TokenizedText extends Text {
    /**
     * Gets the number of tokens in this text
     *
     * @return The number of tokens in this text
     */
    int getTokenCount();

    /**
     * Gets the token at the given index
     *
     * @param index An index
     * @return The token
     */
    Symbol at(int index);

    /**
     * Gets the position of the token at the given index
     *
     * @param tokenIndex The index of a token
     * @return The position (line and column) of the token
     */
    TextPosition getPositionOf(int tokenIndex);
}
