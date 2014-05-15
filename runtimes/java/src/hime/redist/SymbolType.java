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
 * Represents the type of symbol
 */
class SymbolType {
    /**
     * Marks as other (used for SPPF nodes)
     */
    public static final byte None = 0;
    /**
     * Marks a token symbol
     */
    public static final byte Token = 1;
    /**
     * Marks a variable symbol
     */
    public static final byte Variable = 2;
    /**
     * Marks a virtual symbol
     */
    public static final byte Virtual = 3;
}
