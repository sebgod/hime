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

public class ParseError {
    protected ParseErrorType type;
    protected TextPosition position;
    protected String message;

    public ParseErrorType getType() {
        return type;
    }

    public TextPosition getPosition() {
        return position;
    }

    public String getMessage() {
        return message;
    }

    protected ParseError(ParseErrorType type, TextPosition position) {
        this.type = type;
        this.position = position;
        this.message = "@" + position.toString() + " ";
    }

    @Override
    public String toString() {
        return message;
    }
}
