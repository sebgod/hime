/*******************************************************************************
 * Copyright (c) 2015 Laurent Wouters and others
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
 ******************************************************************************/

package org.xowl.hime.redist;

import java.util.List;

/**
 * Represents a node in an Abstract Syntax Tree
 *
 * @author Laurent Wouters
 */
public class ASTNode implements SemanticElement {
    /**
     * The parent parse tree
     */
    private final AST tree;
    /**
     * The index of this node in the parse tree
     */
    private final int index;

    /**
     * Gets the children of this node
     *
     * @return The children of this node
     */
    public List<ASTNode> getChildren() {
        return tree.getChildren(index);
    }

    @Override
    public TextPosition getPosition() {
        return tree.getPosition(index);
    }

    @Override
    public TextSpan getSpan() {
        return tree.getSpan(index);
    }

    @Override
    public TextContext getContext() {
        return tree.getContext(index);
    }

    @Override
    public Symbol getSymbol() {
        return tree.getSymbol(index);
    }

    @Override
    public String getValue() {
        return tree.getValue(index);
    }

    /**
     * Initializes this node
     *
     * @param tree  The parent parse tree
     * @param index The index of this node in the parse tree
     */
    public ASTNode(AST tree, int index) {
        this.tree = tree;
        this.index = index;
    }

    @Override
    public String toString() {
        String name = tree.getSymbol(index).getName();
        String value = tree.getValue(index);
        if (value != null)
            return name + " = " + value;
        return name;
    }
}
