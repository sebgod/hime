/**********************************************************************
* Copyright (c) 2013 Laurent Wouters and others
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
using System.Collections.Generic;

namespace Hime.CentralDogma.Grammars
{
	/// <summary>
	/// Represents a symbol in a grammar
	/// </summary>
	public class Symbol
	{
		/// <summary>
		/// Gets the unique indentifier (within a grammar) of this symbol
		/// </summary>
		public int ID { get; protected set; }

		/// <summary>
		/// Gets the name of this symbol
		/// </summary>
		public string Name { get; protected set; }

		/// <summary>
		/// Initializes this symbol
		/// </summary>
		/// <param name="sid">The symbol's unique identifier</param>
		/// <param name="name">The symbol's name</param>
		public Symbol(int sid, string name)
		{
			this.ID = sid;
			this.Name = name;
		}

		/// <summary>
		/// Represents an equality comparer for grammar symbols
		/// </summary>
		public sealed class EqualityComparer : IEqualityComparer<Symbol>
		{
			public bool Equals(Symbol x, Symbol y)
			{
				return (x.ID == y.ID);
			}
			public int GetHashCode(Symbol obj)
			{
				return obj.ID;
			}
		}

		/// <summary>
		/// Serves as a hash function for a <see cref="Hime.CentralDogma.Grammars.Symbol"/> object.
		/// </summary>
		/// <returns>
		/// A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.
		/// </returns>
		public override int GetHashCode()
		{
			return ID;
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Hime.CentralDogma.Grammars.Symbol"/>.
		/// </summary>
		/// <param name='obj'>
		/// The <see cref="System.Object"/> to compare with the current <see cref="Hime.CentralDogma.Grammars.Symbol"/>.
		/// </param>
		/// <returns>
		/// <c>true</c> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="Hime.CentralDogma.Grammars.Symbol"/>; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj)
		{
			Symbol temp  = obj as Symbol;
			if (temp == null)
				return false;
			return (this.ID == temp.ID);
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Hime.CentralDogma.Grammars.Symbol"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents the current <see cref="Hime.CentralDogma.Grammars.Symbol"/>.
		/// </returns>
		public override string ToString()
		{
			return Name;
		}
	}
}