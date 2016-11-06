using System;
using System.IO;
using System.Text;

namespace Thinktecture.IO.Adapters
{
	/// <summary>
	/// Adapter for <see cref="StringWriter"/>.
	/// </summary>
	public class StringWriterAdapter : TextWriterAdapter, IStringWriter
	{
		private readonly StringWriter _writer;

		/// <summary>Initializes a new instance of the <see cref="StringWriterAdapter" /> class.</summary>
		public StringWriterAdapter()
			: this(new StringWriter())
		{
		}

		/// <summary>Initializes a new instance of the <see cref="StringWriterAdapter" /> class with the specified format control.</summary>
		/// <param name="formatProvider">An <see cref="T:System.IFormatProvider" /> object that controls formatting. </param>
		public StringWriterAdapter(IFormatProvider formatProvider)
			: this(new StringWriter(formatProvider))
		{
		}

		/// <summary>Initializes a new instance of the <see cref="StringWriterAdapter" /> class that writes to the specified <see cref="T:System.Text.StringBuilder" />.</summary>
		/// <param name="sb">The StringBuilder to write to. </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="sb" /> is null. </exception>
		public StringWriterAdapter(StringBuilder sb)
			: this(new StringWriter(sb))
		{
		}

		/// <summary>Initializes a new instance of the <see cref="StringWriterAdapter" /> class that writes to the specified <see cref="T:System.Text.StringBuilder" /> and has the specified format provider.</summary>
		/// <param name="sb">The StringBuilder to write to. </param>
		/// <param name="formatProvider">An <see cref="T:System.IFormatProvider" /> object that controls formatting. </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="sb" /> is null. </exception>
		public StringWriterAdapter(StringBuilder sb, IFormatProvider formatProvider)
			: this(new StringWriter(sb,formatProvider))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StringWriterAdapter" /> class.
		/// </summary>
		/// <param name="writer">Writer to be used by the adapter.</param>
		public StringWriterAdapter(StringWriter writer) : base(writer)
		{
			if (writer == null)
				throw new ArgumentNullException(nameof(writer));

			_writer = writer;
		}

		/// <inheritdoc />
		StringWriter IStringWriter.ToImplementation()
		{
			return _writer;
		}

		/// <inheritdoc />
		public StringBuilder GetStringBuilder()
		{
			return _writer.GetStringBuilder();
		}
	}
}