using System;
using System.Collections.Generic;
using System.Text;

namespace BookWebApi.Tests
{

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class TestPriorityAttribute
		: Attribute
	{

		public int Priority { get; private set; }

		public TestPriorityAttribute(int priority)
		{
			Priority = priority;
		}

	}

}