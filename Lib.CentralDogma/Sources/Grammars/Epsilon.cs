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

namespace Hime.CentralDogma.Grammars
{
    class Epsilon : Terminal
    {
        private static Epsilon instance;
        private static readonly object _lock = new object();
        private Epsilon() : base(1, "ε", 0) { }

        public static Epsilon Instance
        {
            get
            {
                lock (_lock)
                {
                    if (instance == null)
                        instance = new Epsilon();
                    return instance;
                }
            }
        }

        public override string ToString() { return "ε"; }
    }
}