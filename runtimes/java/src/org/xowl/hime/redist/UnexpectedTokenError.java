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

package org.xowl.hime.redist;

import java.util.Collections;
import java.util.List;

/**
 * Represents an unexpected token error in a parser
 */
public class UnexpectedTokenError extends ParseError {
    /**
     * The unexpected token
     */
    private Symbol unexpected;
    /**
     * A list of the expected terminals
     */
    private List<Symbol> expected;

    /**
     * Gets the unexpected token
     *
     * @return The unexpected token
     */
    public Symbol getUnexpectedToken() {
        return unexpected;
    }

    /**
     * Gets a list of the expected terminals
     *
     * @return A list of the expected terminals
     */
    public List<Symbol> getExpectedTerminals() {
        return expected;
    }

    /**
     * Initializes a new instance of the UnexpectedTokenError class with a token and an array of expected names
     *
     * @param token    The unexpected token
     * @param position Error's position in the input
     * @param expected The expected terminals
     */
    public UnexpectedTokenError(Symbol token, TextPosition position, List<Symbol> expected) {
        super(ParseErrorType.UnexpectedToken, position);
        this.unexpected = token;
        this.expected = Collections.unmodifiableList(expected);
        StringBuilder builder = new StringBuilder("Unexpected token \"");
        builder.append(token.getValue());
        builder.append("\"; expected: ");
        for (int i = 0; i != expected.size(); i++) {
            if (i != 0)
                builder.append(", ");
            builder.append(expected.get(i).getName());
        }
        this.message += builder.toString();
    }
}
