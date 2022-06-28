using System;

namespace ZF.UI
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ExportAttribute : Attribute
	{
		public bool isGroup { get; set; }

		public string path { get; set; }

		public string describe { get; set; }

		public ExportAttribute(bool isGroup, string path, string describe)
		{
			this.isGroup = isGroup;
			this.path = path;
			this.describe = describe;
		}

		public ExportAttribute(string path, string describe)
		{
			this.path = path;
			this.describe = describe;
		}
	}
}
