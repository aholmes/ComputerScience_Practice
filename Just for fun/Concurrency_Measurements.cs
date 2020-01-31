/*
Shows relative times for various scenarios of executing and waiting for Tasks.


Output will be similar to this:

1000 tasks @ 100ms delay, no task limit
Current thread count: 21
Task concurrency level: 2147483647
Aggregate time: 01m 52s 971ms (112971.6373ms)
Elapsed time: 00m 00s 113ms (113.0933ms)

1000 tasks @ 100ms delay w/ max 5 concurrent tasks
Current thread count: 24
Task concurrency level: 2147483647
Aggregate time: 01m 49s 301ms (109301.4639ms)
Elapsed time: 00m 21s 760ms (21760.9298ms)

1000 tasks @ 100ms delay w/ max 2 threads
Current thread count: 38
Task concurrency level: 2
Aggregate time: 01m 48s 921ms (108921.8361ms)
Elapsed time: 00m 00s 108ms (108.9854ms)

1000 tasks @ 100ms delay w/ max 2 threads and max 1 concurrent task
Current thread count: 20
Task concurrency level: 2
Aggregate time: 01m 50s 251ms (110251.5992ms)
Elapsed time: 01m 50s 317ms (110317.7142ms)
*/

void Main()
{
	const int GCTriggerSize = 200 * 1024 * 1024;
	// Try to limit GC to avoid discrepancies in timing.
	// No promises though!
	GC.TryStartNoGCRegion(GCTriggerSize);
	{
		// ~100ms
		var result = Execute();
		Report("1000 tasks @ 100ms delay, no task limit", result.tasks, result.elapsedTime);
	}
	GC.EndNoGCRegion();
	GC.WaitForFullGCApproach();
	GC.WaitForFullGCComplete();

	GC.TryStartNoGCRegion(GCTriggerSize);
	{
		// ~20s
		var result = Execute(maxTaskCount: 5);
		Report("1000 tasks @ 100ms delay w/ max 5 concurrent tasks", result.tasks, result.elapsedTime);
	}
	GC.EndNoGCRegion();
	GC.WaitForFullGCApproach();
	GC.WaitForFullGCComplete();

	GC.TryStartNoGCRegion(GCTriggerSize);
	{
		// ~100ms
		var ts = new LimitedConcurrencyLevelTaskScheduler(2);
		var taskFactory = new TaskFactory(ts);
		var cts = new CancellationTokenSource();

		taskFactory.StartNew(() =>
		{
			var result = Execute();

			Report("1000 tasks @ 100ms delay w/ max 2 threads", result.tasks, result.elapsedTime);
		}, cts.Token).Wait();
	}
	GC.EndNoGCRegion();
	GC.WaitForFullGCApproach();
	GC.WaitForFullGCComplete();

	GC.TryStartNoGCRegion(GCTriggerSize);
	{
		// ~1m 49s
		var ts = new LimitedConcurrencyLevelTaskScheduler(2);
		var taskFactory = new TaskFactory(ts);
		var cts = new CancellationTokenSource();

		taskFactory.StartNew(() =>
		{
			var result = Execute(maxTaskCount: 1);

			Report("1000 tasks @ 100ms delay w/ max 2 threads and max 1 concurrent task", result.tasks, result.elapsedTime);
		}, cts.Token).Wait();
	}
	GC.EndNoGCRegion();
	GC.WaitForFullGCApproach();
	GC.WaitForFullGCComplete();
}

void Report(string name, Task<Stopwatch>[] tasks, Stopwatch elapsedTime)
{
	Console.WriteLine(name);
	Console.WriteLine($"Current thread count: {Process.GetCurrentProcess().Threads.Count}");
	Console.WriteLine($"Task concurrency level: {TaskScheduler.Current.MaximumConcurrencyLevel}");

	var aggregateTime = tasks.Select(o => o.Result.Elapsed).Aggregate((a, b) => a + b);
	Console.WriteLine($"Aggregate time: {aggregateTime.ToString(@"mm\m\ ss\s\ fff\m\s")} ({aggregateTime.TotalMilliseconds}ms)");
	Console.WriteLine($"Elapsed time: {elapsedTime.Elapsed.ToString(@"mm\m\ ss\s\ fff\m\s")} ({elapsedTime.Elapsed.TotalMilliseconds}ms)\n");
}

(Task<Stopwatch>[] tasks, Stopwatch elapsedTime) Execute(int sequentialRunCount = 1000, uint maxTaskCount = 0)
{
	var tasks = new Task<Stopwatch>[sequentialRunCount];
	
	SemaphoreSlim semaphore = null;
	Stopwatch sw = new Stopwatch();
	
	if (maxTaskCount == 0)
	{
		for(var i = 0; i < sequentialRunCount; i++)
		{
			tasks[i] = foo();
		}
	
		sw.Start();
		var waitingTask = Task.WhenAll(tasks);
		// I'm using this pattern so I can log while the task is not complete
		// This would normally be `await waitingTask`
		while(!waitingTask.IsCompleted); // { Console.WriteLine("..."); }
		sw.Stop();
	}
	else
	{
		if (maxTaskCount == 1)
		{
			sw.Start();
			for (var i = 0; i < sequentialRunCount; i++)
			{
				// Using a semaphore when we have to wait on each task is pointless
				// Leaving try/catch in place so any performance penalties are the same
				// between one task or concurrent tasks
				try
				{
					tasks[i] = foo();
				}
				catch { }
				finally
				{
				}

				while (!tasks[i].IsCompleted) ; // { Console.WriteLine("..."); }
			}
			sw.Stop();
		}
		else
		{
			using(semaphore = new SemaphoreSlim((int)maxTaskCount))
			{
				sw.Start();
				for (var i = 0; i < sequentialRunCount; i++)
				{
					semaphore.Wait();

					try
					{
						tasks[i] = foo();
					}
					catch { }
					finally
					{
						semaphore.Release();
					}

					if (i > 0 && i % maxTaskCount == 0)
					{
						var waitingTask = Task.WhenAll(tasks[(int)(i - maxTaskCount)..(int)i]);
						while (!waitingTask.IsCompleted) ; // { Console.WriteLine("..."); }
					}
				}
				sw.Stop();
			}
		}
	}
	
	return (tasks, sw);
}

//HttpClient httpClient = new HttpClient();
// Define other methods, classes and namespaces here
async Task<Stopwatch> foo()
{
	var sw = new Stopwatch();

	sw.Start();
	// Substituted an actual request because they returned too quickly
	// and I needed a reliable delay to demonstrate execution time differences
	await Task.Delay(100);
	//await httpClient.GetAsync("http://localhost");
	sw.Stop();
	
	return sw;
}




#region Task Scheduler from MSFT
// https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskscheduler
public class LimitedConcurrencyLevelTaskScheduler : TaskScheduler
{
	// Indicates whether the current thread is processing work items.
	[ThreadStatic]
	private static bool _currentThreadIsProcessingItems;

	// The list of tasks to be executed 
	private readonly LinkedList<Task> _tasks = new LinkedList<Task>(); // protected by lock(_tasks)

	// The maximum concurrency level allowed by this scheduler. 
	private readonly int _maxDegreeOfParallelism;

	// Indicates whether the scheduler is currently processing work items. 
	private int _delegatesQueuedOrRunning = 0;

	// Creates a new instance with the specified degree of parallelism. 
	public LimitedConcurrencyLevelTaskScheduler(int maxDegreeOfParallelism)
	{
		if (maxDegreeOfParallelism < 1) throw new ArgumentOutOfRangeException("maxDegreeOfParallelism");
		_maxDegreeOfParallelism = maxDegreeOfParallelism;
	}

	// Queues a task to the scheduler. 
	protected sealed override void QueueTask(Task task)
	{
		// Add the task to the list of tasks to be processed.  If there aren't enough 
		// delegates currently queued or running to process tasks, schedule another. 
		lock (_tasks)
		{
			_tasks.AddLast(task);
			if (_delegatesQueuedOrRunning < _maxDegreeOfParallelism)
			{
				++_delegatesQueuedOrRunning;
				NotifyThreadPoolOfPendingWork();
			}
		}
	}

	// Inform the ThreadPool that there's work to be executed for this scheduler. 
	private void NotifyThreadPoolOfPendingWork()
	{
		ThreadPool.UnsafeQueueUserWorkItem(_ =>
		{
		   // Note that the current thread is now processing work items.
		   // This is necessary to enable inlining of tasks into this thread.
		   _currentThreadIsProcessingItems = true;
			try
			{
			   // Process all available items in the queue.
			   while (true)
				{
					Task item;
					lock (_tasks)
					{
					   // When there are no more items to be processed,
					   // note that we're done processing, and get out.
					   if (_tasks.Count == 0)
						{
							--_delegatesQueuedOrRunning;
							break;
						}

					   // Get the next item from the queue
					   item = _tasks.First.Value;
						_tasks.RemoveFirst();
					}

				   // Execute the task we pulled out of the queue
				   base.TryExecuteTask(item);
				}
			}
		   // We're done processing items on the current thread
		   finally { _currentThreadIsProcessingItems = false; }
		}, null);
	}

	// Attempts to execute the specified task on the current thread. 
	protected sealed override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
	{
		// If this thread isn't already processing a task, we don't support inlining
		if (!_currentThreadIsProcessingItems) return false;

		// If the task was previously queued, remove it from the queue
		if (taskWasPreviouslyQueued)
			// Try to run the task. 
			if (TryDequeue(task))
				return base.TryExecuteTask(task);
			else
				return false;
		else
			return base.TryExecuteTask(task);
	}

	// Attempt to remove a previously scheduled task from the scheduler. 
	protected sealed override bool TryDequeue(Task task)
	{
		lock (_tasks) return _tasks.Remove(task);
	}

	// Gets the maximum concurrency level supported by this scheduler. 
	public sealed override int MaximumConcurrencyLevel { get { return _maxDegreeOfParallelism; } }

	// Gets an enumerable of the tasks currently scheduled on this scheduler. 
	protected sealed override IEnumerable<Task> GetScheduledTasks()
	{
		bool lockTaken = false;
		try
		{
			Monitor.TryEnter(_tasks, ref lockTaken);
			if (lockTaken) return _tasks;
			else throw new NotSupportedException();
		}
		finally
		{
			if (lockTaken) Monitor.Exit(_tasks);
		}
	}
}
#endregion
