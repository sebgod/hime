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
using System;
using System.Collections.Generic;

namespace Hime.Tests.Driver
{
	/// <summary>
	/// Represents a test result
	/// </summary>
	public class TestResult
	{
		/// <summary>
		/// The test was successful
		/// </summary>
		public const int RESULT_SUCCESS = 0;
		/// <summary>
		/// The test failed in the end
		/// </summary>
		public const int RESULT_FAILURE_VERB = 1;
		/// <summary>
		/// The test failed in its parsing phase
		/// </summary>
		public const int RESULT_FAILURE_PARSING = 2;

		/// <summary>
		/// The execution timestamp
		/// </summary>
		private DateTime startTime;
		/// <summary>
		/// The total time spent on the test
		/// </summary>
		private TimeSpan spentTime;
		/// <summary>
		/// The result returned by the executor
		/// </summary>
		private int executorResult;
		/// <summary>
		/// The console output during the test
		/// </summary>
		private List<string> output;

		/// <summary>
		/// Starts the test
		/// </summary>
		public TestResult()
		{
			this.startTime = DateTime.Now;
		}

		/// <summary>
		/// Finishes this test
		/// </summary>
		/// <param name="result">The test result returned by the executor</param>
		/// <param name="output">The test console output</param>
		public void Finish(int result, List<string> output)
		{
			this.spentTime = (DateTime.Now - this.startTime);
			this.executorResult = result;
			this.output = output;
		}
	}
}