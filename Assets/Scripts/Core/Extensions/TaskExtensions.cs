using System;
using System.Threading.Tasks;
using Core.Managers;

namespace Core.Extensions
{
	public static class TaskExtensions
	{
		public static Task ContinueWithOnMainThread(this Task task, Action<Task> callback)
		{
			return task.ContinueWith(continuationTask =>
			{
				AsyncCallbackManager.Queue(() => { callback(continuationTask); });
			});
		}

		public static Task ContinueWithOnMainThread<T>(this Task<T> task, Action<Task<T>> callback)
		{
			return task.ContinueWith(continuationTask =>
			{
				AsyncCallbackManager.Queue(() => { callback(continuationTask); });
			});
		}
	}
}