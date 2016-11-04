﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Thinktecture.IO
{
	public class StreamWriterAdapter : TextWriterAdapter, IStreamWriter
	{
		private readonly StreamWriter _writer;

		public StreamWriterAdapter(StreamWriter writer) 
			: base(writer)
		{
			if (writer == null)
				throw new ArgumentNullException(nameof(writer));

			_writer = writer;
		}

		/// <inheritdoc />
		StreamWriter IStreamWriter.ToImplementation()
		{
			return _writer;
		}

		/// <inheritdoc />
		public bool AutoFlush
		{
			get { return _writer.AutoFlush; }
			set { _writer.AutoFlush = value; }
		}

		/// <inheritdoc />
		public IStream BaseStream => _writer.BaseStream.ToInterface();
	} 
}