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

using System;
using System.Xml;

namespace Hime.Tests.Driver
{
	/// <summary>
	/// Represents the aggrated data of a report
	/// </summary>
	public class ReportData
	{
		/// <summary>
		/// The total time spent
		/// </summary>
		public TimeSpan spent;
		/// <summary>
		/// The number of passed tests
		/// </summary>
		public int passed;
		/// <summary>
		/// The number of tests in error
		/// </summary>
		public int errors;
		/// <summary>
		/// The number of failed tests
		/// </summary>
		public int failed;
		/// <summary>
		/// The child XML element
		/// </summary>
		public XmlElement child;

		/// <summary>
		/// Aggregates two data into a new one
		/// </summary>
		/// <param name="data1">Some data</param>
		/// <param name="data2">Some data</param>
		/// <returns>The aggregated result</returns>
		public static ReportData operator+(ReportData data1, ReportData data2)
		{
			ReportData result = new ReportData();
			result.spent = data1.spent + data2.spent;
			result.passed = data1.passed + data2.passed;
			result.errors = data1.errors + data2.errors;
			result.failed = data1.failed + data2.failed;
			return result;
		}
	}
}