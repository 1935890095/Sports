using System;
using System.Collections.Generic;
using UnityEngine;

namespace XFX.Core.Render
{
	public abstract class RenderComponent : IRenderComponent
	{
		private struct Cmd
		{
			public string cmd;

			public object[] args;
		}

		private bool created_;

		private bool started_;

		private bool enabled_;

		private Queue<Cmd> queue_;

		public IRenderObject renderObject { get; private set; }

		public bool enabled
		{
			get
			{
				return enabled_;
			}
			set
			{
				if (enabled_ != value)
				{
					enabled_ = value;
					if (value)
					{
						OnEnabled();
					}
					else
					{
						OnDisable();
					}
				}
			}
		}

		void IRenderComponent.Create(IRenderObject obj)
		{
			renderObject = obj;
			try
			{
				OnPrepare();
				OnCreate();
				enabled = renderObject.active;
				created_ = true;
				while (queue_ != null && queue_.Count > 0)
				{
					Cmd cmd = queue_.Dequeue();
					((IRenderComponent)this).Command(cmd.cmd, cmd.args);
				}
				queue_ = null;
			}
			catch (Exception ex)
			{
				Debug.LogError((object)("[RenderComponent] Create error: " + ex.ToString()));
			}
		}

		void IRenderComponent.Destroy()
		{
			OnDestroy();
			renderObject = null;
		}

		void IRenderComponent.Update()
		{
			if (!started_)
			{
				started_ = true;
				OnStart();
			}
			OnUpdate();
		}

		void IRenderComponent.LateUpdate()
		{
			OnLateUpdate();
		}

		void IRenderComponent.Command(string cmd, params object[] args)
		{
			if (!created_)
			{
				if (queue_ == null)
				{
					queue_ = new Queue<Cmd>();
				}
				queue_.Enqueue(new Cmd
				{
					cmd = cmd,
					args = args
				});
			}
			else if (enabled)
			{
				OnCommand(cmd, args);
			}
		}

		[Obsolete("OnPrepare is obsoleted")]
		protected virtual void OnPrepare()
		{
		}

		protected virtual void OnCreate()
		{
		}

		protected virtual void OnStart()
		{
		}

		protected virtual void OnDestroy()
		{
		}

		protected virtual void OnEnabled()
		{
		}

		protected virtual void OnDisable()
		{
		}

		protected virtual void OnUpdate()
		{
		}

		protected virtual void OnLateUpdate()
		{
		}

		protected virtual void OnCommand(string cmd, params object[] args)
		{
		}
	}
}
