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

namespace Hime.SDK.Grammars.LR
{
	/// <summary>
	/// Represents the type of a LR conflict
	/// </summary>
	public enum ErrorType
	{
		/// <summary>
		/// Represents a Shift-Reduce conflict
		/// </summary>
		ConflictShiftReduce,
		/// <summary>
		/// Represents a Reduce-Reduce conflict
		/// </summary>
		ConflictReduceReduce,
		/// <summary>
		/// Represents the error of a contextual terminal expected outside of its context
		/// </summary>
		ErrorContextualTerminal,
	}
}