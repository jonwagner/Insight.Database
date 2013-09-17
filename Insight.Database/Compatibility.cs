#if NET35
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
	public delegate void Action<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
	public delegate void Action<T1, T2, T3, T4, T5, T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);

	class ConcurrentDictionary<TKey, TValue>
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

	class Tuple
	{
		public static Tuple<T1, T2> Create<T1, T2>(T1 t1, T2 t2)
		{
			return new Tuple<T1, T2>(t1, t2);
		}
	}

	class Tuple<T1, T2>
	{
		public T1 Item1 { get; private set; }
		public T2 Item2 { get; private set; }

		public Tuple(T1 item1, T2 item2)
		{
			Item1 = item1;
			Item2 = item2;
		}

		public override int GetHashCode()
		{
			return Item1.GetHashCode() + Item2.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			Tuple<T1, T2> other = obj as Tuple<T1, T2>;
			if (other == null)
				return false;

			if (Item1 == null && other.Item1 != null)
				return false;
			if (Item2 == null && other.Item2 != null)
				return false;

			return Item1.Equals(other.Item1) && Item2.Equals(other.Item2);
		}
	}

	public struct CancellationToken
	{
		public static readonly CancellationToken None = new CancellationToken();

		private CancellationTokenSource Source;

		internal CancellationToken(CancellationTokenSource source)
		{
			Source = source;
		}

		internal void ThrowIfCancellationRequested()
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
		public Exception Exception { get; private set; }
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

		internal Task ContinueWith(Action<Task<T>> action)
		{
			return new Task(() => action(this));
		}

		internal Task<TResult> ContinueWith<TResult>(Func<Task<T>, TResult> action, CancellationToken ct)
		{
			return new Task<TResult>(() => action(this));
		}

		internal Task<TResult> ContinueWith<TResult>(Func<Task<T>, TResult> action, TaskContinuationOptions options)
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

	static class TaskHelpers
	{
		internal static Task<T> Unwrap<T>(this Task<Task<T>> task)
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