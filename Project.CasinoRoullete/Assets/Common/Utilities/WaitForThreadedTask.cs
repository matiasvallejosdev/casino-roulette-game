using System;
using System.Threading;
 
/// <summary>
/// A CustomYieldInstruction that executes a task on a new thread and keeps waiting until it's done.
/// http://JacksonDunstan.com/articles/3746
/// </summary>
class WaitForThreadedTask : UnityEngine.CustomYieldInstruction
{
	/// <summary>
	/// If the thread is still running
	/// </summary>
	private bool isRunning;
 
	/// <summary>
	/// Start the task by starting a thread with the given priority. It immediately executes the
	/// given task. When the given task finishes, <see cref="keepWaiting"/> returns true.
	/// </summary>
	/// <param name="task">Task to execute in the thread</param>
	/// <param name="priority">Priority of the thread to execute the task in</param>
	public WaitForThreadedTask(
		Action task,
		ThreadPriority priority = ThreadPriority.Normal
	)
	{
		isRunning = true;
		new Thread(() => { task(); isRunning = false; }).Start(priority);
	}
 
	/// <summary>
	/// If the coroutine should keep waiting
	/// </summary>
	/// <value>If the thread is still running</value>
	public override bool keepWaiting { get { return isRunning; } }
}