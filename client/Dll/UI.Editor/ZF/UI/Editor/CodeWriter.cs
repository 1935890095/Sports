using System;
using System.IO;

namespace ZF.UI.Editor
{
	public sealed class CodeWriter
	{
		private int tabCount = 0;

		private TextWriter _writer;

		private StringWriter writer;

		public CodeWriter(TextWriter writer)
		{
			_writer = writer;
			this.writer = new StringWriter();
		}

		public CodeWriter Space(int spaceLength = 1)
		{
			if (writer != null && spaceLength >= 1)
			{
				for (int i = 0; i < spaceLength; i++)
				{
					writer.Write(' ');
				}
			}
			return this;
		}

		public CodeWriter Tab(int tabLength = 1)
		{
			if (writer != null && tabLength >= 1)
			{
				for (int i = 0; i < tabLength; i++)
				{
					writer.Write('\t');
				}
			}
			return this;
		}

		public CodeWriter IncraseTab()
		{
			tabCount++;
			return this;
		}

		public CodeWriter DecreaseTab()
		{
			if (tabCount >= 1)
			{
				tabCount--;
			}
			return this;
		}

		public CodeWriter Write(string value)
		{
			if (writer != null)
			{
				writer.Write(value);
			}
			return this;
		}

		public CodeWriter WriteFormat(string format, params object[] args)
		{
			if (writer != null)
			{
				writer.Write(string.Format(format, args));
			}
			return this;
		}

		public CodeWriter WriteLine(string value = "")
		{
			if (writer != null)
			{
				for (int i = 0; i < tabCount; i++)
				{
					writer.Write('\t');
				}
				writer.Write(value + "\n");
			}
			return this;
		}

		public CodeWriter NextLine(int lineLength = 1)
		{
			for (int i = 0; i < lineLength; i++)
			{
				WriteLine();
			}
			return this;
		}

		public CodeWriter WriteLineFormat(string format, params object[] args)
		{
			if (!string.IsNullOrEmpty(format))
			{
				WriteLine(string.Format(format, args));
			}
			return this;
		}

		public CodeWriter EnterAction(Action<CodeWriter> action)
		{
			action(this);
			return this;
		}

		public CodeWriter Insert(int startIndex, string content)
		{
			writer.GetStringBuilder().Insert(startIndex, content);
			return this;
		}

		public CodeWriter Insert(int startIndex, CodeWriter writer)
		{
			return Insert(startIndex, writer.ToString());
		}

		public override string ToString()
		{
			return writer.ToString();
		}

		public void Flush()
		{
			if (_writer != null)
			{
				_writer.Write(writer.ToString());
			}
		}
	}
}
