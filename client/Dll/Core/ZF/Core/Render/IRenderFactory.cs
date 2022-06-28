using System;

namespace ZF.Core.Render
{
	public interface IRenderFactory
	{
		int concurrency { get; set; }

		int lingerTime { get; set; }

		bool isIdle { get; }

		T CreateInstance<T>(string filename, IRenderObject parent = null, int priority = 0, string tag = "") where T : IRenderObject;

		IRenderObject CreateInstance(Type type, string filename, IRenderObject parent = null, int priority = 0, string tag = "");

		void RemoveResource(IRenderResource resource);

		void Loop();
	}
}
