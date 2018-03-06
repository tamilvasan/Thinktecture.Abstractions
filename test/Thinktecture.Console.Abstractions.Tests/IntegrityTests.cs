﻿using System;
using Xunit;

namespace Thinktecture
{
	public class IntegrityTests : IntegrityTestsBase
	{
		public IntegrityTests()
			: base("Console")
		{
			CustomMappings[typeof(ConsoleCancelEventHandler)] = typeof(EventHandler<IConsoleCancelEventArgs>);
			ExcludedTypes.Add(typeof(ConsoleKeyInfo));
			ExcludeMember<EventArgs>(nameof(EventArgs.Empty));
		}

		[Fact]
		public void Should_contain_all_types()
		{
			CheckTypes();
		}

		[Fact]
		public void Should_contain_all_members()
		{
			CheckMembers();
		}
	}
}
