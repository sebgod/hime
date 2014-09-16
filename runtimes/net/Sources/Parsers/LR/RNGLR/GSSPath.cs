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
using System;
using Hime.Redist.Utils;

namespace Hime.Redist.Parsers
{
	/// <summary>
	/// Represents a path in a GSS
	/// </summary>
	class GSSPath
	{
		/// <summary>
		/// The initial size of the label buffer
		/// </summary>
		private const int initBufferSize = 64;

		/// <summary>
		/// The last GSS node in this path
		/// </summary>
		private int last;
		/// <summary>
		/// The generation containing the last GSS node in this path
		/// </summary>
		private int generation;
		/// <summary>
		/// The labels on this GSS path
		/// </summary>
		private GSSLabel[] labels;

		/// <summary>
		/// Gets or sets the final target of this path
		/// </summary>
		public int Last
		{
			get { return last; }
			set { last = value; }
		}

		/// <summary>
		/// Gets or sets the generation containing the final target of this path
		/// </summary>
		public int Generation
		{
			get { return generation; }
			set { generation = value; }
		}

		/// <summary>
		/// Gets or sets the i-th label of the edges traversed by this path
		/// </summary>
		public GSSLabel this[int index]
		{
			get { return labels[index]; }
			set { labels[index] = value; }
		}

		/// <summary>
		/// Initializes this path
		/// </summary>
		public GSSPath(int length)
		{
			this.last = 0;
			this.labels = new GSSLabel[length < initBufferSize ? initBufferSize : length];
		}

		/// <summary>
		/// Initializes this path
		/// </summary>
		public GSSPath()
		{
			this.last = 0;
			this.labels = null;
		}

		/// <summary>
		/// Ensure the specified length of the label buffer
		/// </summary>
		/// <param name="length">The required length</param>
		public void Ensure(int length)
		{
			if (length > labels.Length)
				labels = new GSSLabel[length];
		}

		/// <summary>
		/// Copy the content of another path to this one
		/// </summary>
		/// <param name="path">The path to copy</param>
		/// <param name="length">The path's length</param>
		public void CopyLabelsFrom(GSSPath path, int length)
		{
			Array.Copy(path.labels, this.labels, length);
		}
	}
}
