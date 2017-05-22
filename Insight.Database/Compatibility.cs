using System;
using System.Collections.Generic;
using System.Linq;
#if NET35
using System.Text;

// don't yell at us for not documenting these classes
#pragma warning disable 1591

namespace System.Collections.Concurrent
{
	class Placeholder { }
}

namespace System.Dynamic
{
	class Placeholder { }
}

namespace System.Threading.Tasks
{
	class Placeholder { }
}

namespace Insight.Database
{
	public class ConcurrentDictionary<TKey, TValue>
	{
		private object _lock = new object();
		private Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

		public TValue GetOrAdd(TKey key, Func<TKey, TValue> addFactory)
		{
			lock (_lock)
			{
				if (_dictionary.ContainsKey(key))
					return _dictionary[key];

				TValue value = addFactory(key);
				_dictionary.Add(key, value);
				return value;
			}
		}

		public bool TryAdd(TKey key, TValue value)
		{
			lock (_lock)
			{
				if (_dictionary.ContainsKey(key))
					return false;

				_dictionary.Add(key, value);
				return true;
			}
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			lock (_lock)
			{
				return _dictionary.TryGetValue(key, out value);
			}
		}
	}

	public abstract class dynamic
	{
		public virtual object this[string key]
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
	}

	class ExpandoObject : Dictionary<string, object>
	{
	}

	public abstract class DynamicObject : dynamic
	{
		public virtual bool TrySetMember(SetMemberBinder binder, object value)
		{
			throw new NotImplementedException();
		}

		public virtual bool TryGetMember(GetMemberBinder binder, out object result)
		{
			throw new NotImplementedException();
		}
	}

	public class SetMemberBinder
	{
		public string Name { get; set; }
	}

	public class GetMemberBinder
	{
		public string Name { get; set; }
	}

	public struct CancellationToken
	{
		public static readonly CancellationToken None = new CancellationToken();

		private CancellationTokenSource Source;

		internal CancellationToken(CancellationTokenSource source)
		{
			Source = source;
		}

		public void ThrowIfCancellationRequested()
		{
			if (Source != null && Source.IsCancelled)
				throw new AggregateException("Task was cancelled.");
		}
	}

	[Flags]
	public enum TaskContinuationOptions
	{
		None = 0,
		ExecuteSynchronously = 1,
		OnlyOnRanToCompletion = 2,
	}

	public enum TaskStatus
	{
		Running = 0,
		Canceled = 1,
		RanToCompletion = 2,
		Failed = 3
	}

	public class TaskFactory
	{
		public Task StartNew(Action action)
		{
			return new Task(action);
		}

		public Task StartNew(Action action, CancellationToken ct)
		{
			ct.ThrowIfCancellationRequested();
			return new Task(action);
		}

		public Task<TResult> StartNew<TResult>(Func<TResult> action)
		{
			return new Task<TResult>(action);
		}

		public Task<TResult> StartNew<TResult>(Func<TResult> action, CancellationToken ct)
		{
			ct.ThrowIfCancellationRequested();
			return new Task<TResult>(action);
		}

		public Task<TResult> FromAsync<TResult>(IAsyncResult begin, Func<IAsyncResult, TResult> end)
		{
			return new Task<TResult>(() => { begin.AsyncWaitHandle.WaitOne(); return end(begin); });
		}
	}

	public class TaskFactory<TResult>
	{
		public Task StartNew(Action action)
		{
			return new Task(action);
		}

		public Task StartNew(Action action, CancellationToken ct)
		{
			ct.ThrowIfCancellationRequested();
			return new Task(action);
		}

		public Task<TResult> StartNew(Func<TResult> action)
		{
			return new Task<TResult>(action);
		}

		public Task<TResult> StartNew(Func<TResult> action, CancellationToken ct)
		{
			ct.ThrowIfCancellationRequested();
			return new Task<TResult>(action);
		}

		public Task<TResult> FromAsync(IAsyncResult begin, Func<IAsyncResult, TResult> end)
		{
			return new Task<TResult>(() => { begin.AsyncWaitHandle.WaitOne(); return end(begin); });
		}
	}

	public class Task
	{
		public static TaskFactory Factory = new TaskFactory();
		public TaskStatus Status { get; private set; }
		public AggregateException Exception { get; private set; }
		public bool IsCompleted { get { return true; } }
		public bool IsCanceled { get { return false; } }
		public bool IsFaulted { get { return Exception != null; } }
		protected object InternalResult { get; set; }

		internal Task(Action action) : this(() => { action(); return null; })
		{
		}

		internal Task(Func<object> action)
		{
			try
			{
				InternalResult = action();
				Status = TaskStatus.RanToCompletion;
			}
			catch (Exception e)
			{
				Exception = new AggregateException(e);
				Status = TaskStatus.Failed;
			}
		}

		public void Wait()
		{
			if (Exception != null)
				throw Exception;
		}

		public static void WaitAll(params Task[] tasks)
		{
			foreach (var t in tasks)
				t.Wait();
		}

		public Task ContinueWith(Action<Task> action)
		{
			return new Task(() => action(this));
		}

		public Task<TResult> ContinueWith<TResult>(Func<Task, TResult> action, CancellationToken ct)
		{
			return new Task<TResult>(() => action(this));
		}

		public Task<TResult> ContinueWith<TResult>(Func<Task, TResult> action, TaskContinuationOptions options)
		{
			return new Task<TResult>(() => action(this));
		}
	}

	public class Task<T> : Task
	{
		public new static TaskFactory<T> Factory = new TaskFactory<T>();

		public T Result 
		{
			get 
			{
				if (Exception != null)
					throw Exception;
				return (T)InternalResult; 
			} 
			private set { InternalResult = value; }
		}

		internal Task(Func<T> action) : base(() => action())
		{
		}

		public Task ContinueWith(Action<Task<T>> action)
		{
			return new Task(() => action(this));
		}

		public Task<TResult> ContinueWith<TResult>(Func<Task<T>, TResult> action, CancellationToken ct)
		{
			return new Task<TResult>(() => action(this));
		}

		public Task<TResult> ContinueWith<TResult>(Func<Task<T>, TResult> action, TaskContinuationOptions options)
		{
			return new Task<TResult>(() => action(this));
		}
	}

	class TaskCompletionSource<T>
	{
		public Task<T> Task { get; private set; }
		public T Result { get; private set; }
		public Exception Exception { get; private set; }

		public void SetException(Exception e)
		{
			Exception = e;
		}

		public void SetResult(T t)
		{
			Result = t;
		}

		public void SetCanceled()
		{
		}
	}

	public static class TaskHelpers
	{
		public static Task<T> Unwrap<T>(this Task<Task<T>> task)
		{
			return new Task<T>(() => task.Result.Result);
		}
	}

	public class CancellationTokenSource
	{
		public CancellationTokenSource()
		{
			Token = new CancellationToken(this);
		}

		public void Cancel()
		{
			IsCancelled = true;
		}

		public CancellationToken Token { get; private set; }
		public bool IsCancelled { get; private set; }
	}

	public class AggregateException : Exception
	{
		public AggregateException(string message) : base (message)
		{
		}

		public AggregateException(Exception e) : base ("An exception occurred.", e)
		{
		}

		public AggregateException Flatten()
		{
			return this;
		}

		public IEnumerable<Exception> InnerExceptions
		{
			get { yield return this; }
		}
	}

	class Lazy<T>
	{
		private object _lock = new object();
		private bool _hasValue;
		private T _value;
		private Func<T> _func;

		public Lazy(Func<T> func)
		{
			_func = func;
		}

		public T Value
		{
			get
			{
				lock (_lock)
				{
					if (!_hasValue)
					{
						_value = _func();
						_hasValue = true;
					}

					return _value;
				}
			}
		}
	}
}
#endif

namespace Insight.Database.MissingExtensions
{
	/// <summary>
	/// Adds missing methods.
	/// </summary>
	public static class MissingExtensions
	{
		/// <summary>
		/// Determines if a string is null or all whitespace.
		/// </summary>
		/// <param name="value">The string to test.</param>
		/// <returns>False if the string contains at least one non-whitespace character.</returns>
		public static bool IsNullOrWhiteSpace(this string value)
		{
#if NET35
			if (value != null)
			{
				for (int i = 0; i < value.Length; i++)
				{
					if (!char.IsWhiteSpace(value[i]))
					{
						return false;
					}
				}
			}
			return true;
#else
			return String.IsNullOrWhiteSpace(value);
#endif
		}

		/// <summary>
		/// Returns the maximum value in a sequence or the default.
		/// </summary>
		/// <typeparam name="T1">The type of the sequence.</typeparam>
		/// <typeparam name="T2">The type of the value.</typeparam>
		/// <param name="list">The list to evaluate.</param>
		/// <param name="selector">A function to select the value.</param>
		/// <returns>The maximum selected value or the default.</returns>
		public static T2 MaxOrDefault<T1, T2>(this IEnumerable<T1> list, Func<T1, T2> selector)
		{
			if (!list.Any())
				return default(T2);

			return list.Max(selector);
		}
	}
}
