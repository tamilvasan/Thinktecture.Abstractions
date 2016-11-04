﻿using System.IO;
using Thinktecture.IO;
using Thinktecture.IO.Adapters;

namespace Thinktecture
{
	public static class StringWriterExtensions
	{
		/// <summary>
		/// Converts provided writer to <see cref="IStringWriter"/>.
		/// </summary>
		/// <param name="writer">Writer to convert.</param>
		/// <returns>Converted writer.</returns>
		public static IStringWriter ToInterface(this StringWriter writer)
		{
			return new StringWriterAdapter(writer);
		}
	}
}