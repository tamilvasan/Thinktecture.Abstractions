using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Thinktecture.Runtime.CompilerServices;

namespace Thinktecture.Threading.Tasks.Adapters
{
	/// <summary>
	/// Represents an asynchronous operation.To browse the .NET Framework source code for this type, see the Reference Source.
	/// </summary>
	[AsyncMethodBuilder(typeof(AsyncTaskAdapterMethodBuilder))]
	[SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
	[SuppressMessage("ReSharper", "ConstantConditionalAccessQualifier")]
	[SuppressMessage("ReSharper", "TaskOfTMethodsWithoutAsyncSuffix")]
	public class TaskAdapter : AbstractionAdapter<Task>, ITask
	{
#if NETCOREAPP2_2
		/// <inheritdoc />
		public bool IsCompletedSuccessfully => Implementation.IsCompletedSuccessfully;
#endif

		/// <summary>Gets an ID for this <see cref="Task" /> instance.</summary>
		/// <returns>The identifier that is assigned by the system to this <see cref="Task" /> instance. </returns>
		public int Id => Implementation.Id;

		/// <summary>Returns the ID of the currently executing <see cref="Task" />.</summary>
		/// <returns>An integer that was assigned by the system to the currently-executing task.</returns>
		public static int? CurrentId => Task.CurrentId;

		/// <summary>Gets the <see cref="T:System.AggregateException" /> that caused the <see cref="Task" /> to end prematurely. If the <see cref="Task" /> completed successfully or has not yet thrown any exceptions, this will return null.</summary>
		/// <returns>The <see cref="T:System.AggregateException" /> that caused the <see cref="Task" /> to end prematurely.</returns>
		public AggregateException Exception => Implementation.Exception;

		/// <summary>Gets the <see cref="TaskStatus" /> of this task.</summary>
		/// <returns>The current <see cref="TaskStatus" /> of this task instance.</returns>
		public TaskStatus Status => Implementation.Status;

		/// <summary>Gets whether this <see cref="Task" /> instance has completed execution due to being canceled.</summary>
		/// <returns>true if the task has completed due to being canceled; otherwise false.</returns>
		public bool IsCanceled => Implementation.IsCanceled;

		/// <summary>Gets whether this <see cref="Task" /> has completed.</summary>
		/// <returns>true if the task has completed; otherwise false.</returns>
		public bool IsCompleted => Implementation.IsCompleted;

		/// <summary>Gets the <see cref="TaskCreationOptions" /> used to create this task.</summary>
		/// <returns>The <see cref="TaskCreationOptions" /> used to create this task.</returns>
		public TaskCreationOptions CreationOptions => Implementation.CreationOptions;

		WaitHandle IAsyncResult.AsyncWaitHandle => ((IAsyncResult)Implementation).AsyncWaitHandle;

		/// <summary>Gets the state object supplied when the <see cref="Task" /> was created, or null if none was supplied.</summary>
		/// <returns>An <see cref="T:System.Object" /> that represents the state data that was passed in to the task when it was created.</returns>
		public object AsyncState => Implementation.AsyncState;

		bool IAsyncResult.CompletedSynchronously => ((IAsyncResult)Implementation).CompletedSynchronously;

		/// <summary>Provides access to factory methods for creating and configuring <see cref="Task" /> and <see cref="Task{TResult}" /> instances.</summary>
		/// <returns>A factory object that can create a variety of <see cref="Task" /> and <see cref="Task{TResult}" /> objects. </returns>
		[NotNull]
		public static ITaskFactory Factory => new TaskFactoryAdapter(Task.Factory);

#if NETSTANDARD1_3 || NETSTANDARD2_0
		/// <summary>Gets a task that has already completed successfully. </summary>
		/// <returns>The successfully completed task. </returns>
		public static ITask CompletedTask { get; } = Task.CompletedTask.ToInterface();
#endif

		/// <summary>Gets whether the <see cref="Task" /> completed due to an unhandled exception.</summary>
		/// <returns>true if the task has thrown an unhandled exception; otherwise false.</returns>
		public bool IsFaulted => Implementation.IsFaulted;

		/// <summary>
		/// Initializes new intance of type <see cref="TaskAdapter"/>.
		/// </summary>
		/// <param name="task">Task to be used by the adapter.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="task" /> argument is null.</exception>
		public TaskAdapter([NotNull] Task task)
			: base(task)
		{
		}

		/// <summary>Initializes a new <see cref="TaskAdapter" /> with the specified action.</summary>
		/// <param name="action">The delegate that represents the code to execute in the task.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="action" /> argument is null.</exception>
		public TaskAdapter([NotNull] Action action)
			: this(new Task(action))
		{
		}

		/// <summary>Initializes a new <see cref="TaskAdapter" /> with the specified action and <see cref="CancellationToken" />.</summary>
		/// <param name="action">The delegate that represents the code to execute in the task.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken" /> that the new  task will observe.</param>
		/// <exception cref="ObjectDisposedException">The provided <see cref="CancellationToken" /> has already been disposed. </exception>
		/// <exception cref="ArgumentNullException">The <paramref name="action" /> argument is null.</exception>
		public TaskAdapter([NotNull] Action action, CancellationToken cancellationToken)
			: this(new Task(action, cancellationToken))
		{
		}

		/// <summary>Initializes a new <see cref="TaskAdapter" /> with the specified action and creation options.</summary>
		/// <param name="action">The delegate that represents the code to execute in the task.</param>
		/// <param name="creationOptions">The <see cref="TaskCreationOptions" /> used to customize the task's behavior. </param>
		/// <exception cref="ArgumentNullException">The <paramref name="action" /> argument is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">The <paramref name="creationOptions" /> argument specifies an invalid value for <see cref="TaskCreationOptions" />.</exception>
		public TaskAdapter([NotNull] Action action, TaskCreationOptions creationOptions)
			: this(new Task(action, creationOptions))
		{
		}

		/// <summary>Initializes a new <see cref="TaskAdapter" /> with the specified action and creation options.</summary>
		/// <param name="action">The delegate that represents the code to execute in the task.</param>
		/// <param name="cancellationToken">The <see cref="TaskFactory.CancellationToken" /> that the new task will observe.</param>
		/// <param name="creationOptions">The <see cref="TaskCreationOptions" /> used to customize the task's behavior.</param>
		/// <exception cref="ObjectDisposedException">The <see cref="CancellationTokenSource" /> that created <paramref name="cancellationToken" /> has already been disposed.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="action" /> argument is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">The <paramref name="creationOptions" /> argument specifies an invalid value for <see cref="TaskCreationOptions" />.</exception>
		public TaskAdapter([NotNull] Action action, CancellationToken cancellationToken, TaskCreationOptions creationOptions)
			: this(new Task(action, cancellationToken, creationOptions))
		{
		}

		/// <summary>Initializes a new <see cref="TaskAdapter" /> with the specified action and state.</summary>
		/// <param name="action">The delegate that represents the code to execute in the task.</param>
		/// <param name="state">An object representing data to be used by the action.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="action" /> argument is null.</exception>
		public TaskAdapter([NotNull] Action<object> action, object state)
			: this(new Task(action, state))
		{
		}

		/// <summary>Initializes a new <see cref="TaskAdapter" /> with the specified action, state, and options.</summary>
		/// <param name="action">The delegate that represents the code to execute in the task.</param>
		/// <param name="state">An object representing data to be used by the action.</param>
		/// <param name="cancellationToken">The <see cref="TaskFactory.CancellationToken" /> that that the new task will observe.</param>
		/// <exception cref="ObjectDisposedException">The <see cref="CancellationTokenSource" /> that created <paramref name="cancellationToken" /> has already been disposed.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="action" /> argument is null.</exception>
		public TaskAdapter([NotNull] Action<object> action, object state, CancellationToken cancellationToken)
			: this(new Task(action, state, cancellationToken))
		{
		}

		/// <summary>Initializes a new <see cref="TaskAdapter" /> with the specified action, state, and options.</summary>
		/// <param name="action">The delegate that represents the code to execute in the task.</param>
		/// <param name="state">An object representing data to be used by the action.</param>
		/// <param name="creationOptions">The <see cref="TaskCreationOptions" /> used to customize the task's behavior.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="action" /> argument is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">The <paramref name="creationOptions" /> argument specifies an invalid value for <see cref="TaskCreationOptions" />.</exception>
		public TaskAdapter([NotNull] Action<object> action, object state, TaskCreationOptions creationOptions)
			: this(new Task(action, state, creationOptions))
		{
		}

		/// <summary>Initializes a new <see cref="TaskAdapter" /> with the specified action, state, and options.</summary>
		/// <param name="action">The delegate that represents the code to execute in the task.</param>
		/// <param name="state">An object representing data to be used by the action.</param>
		/// <param name="cancellationToken">The <see cref="TaskFactory.CancellationToken" /> that that the new task will observe..</param>
		/// <param name="creationOptions">The <see cref="TaskCreationOptions" /> used to customize the task's behavior.</param>
		/// <exception cref="ObjectDisposedException">The <see cref="CancellationTokenSource" /> that created <paramref name="cancellationToken" /> has already been disposed.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="action" /> argument is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">The <paramref name="creationOptions" /> argument specifies an invalid value for <see cref="TaskCreationOptions" />.</exception>
		public TaskAdapter([NotNull] Action<object> action, object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions)
			: this(new Task(action, state, cancellationToken, creationOptions))
		{
		}

		/// <summary>Starts the <see cref="Task" />, scheduling it for execution to the current <see cref="TaskScheduler" />.</summary>
		/// <exception cref="ObjectDisposedException">The <see cref="Task" /> instance has been disposed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="Task" /> is not in a valid state to be started. It may have already been started, executed, or canceled, or it may have been created in a manner that doesn't support direct scheduling.</exception>
		public void Start()
		{
			Implementation.Start();
		}

		/// <summary>Starts the <see cref="Task" />, scheduling it for execution to the specified <see cref="TaskScheduler" />.</summary>
		/// <param name="scheduler">The <see cref="TaskScheduler" /> with which to associate and execute this task.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="scheduler" /> argument is null.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="Task" /> is not in a valid state to be started. It may have already been started, executed, or canceled, or it may have been created in a manner that doesn't support direct scheduling.</exception>
		/// <exception cref="ObjectDisposedException">The <see cref="Task" /> instance has been disposed.</exception>
		/// <exception cref="TaskSchedulerException">The scheduler was unable to queue this task.</exception>
		public void Start(TaskScheduler scheduler)
		{
			Implementation.Start(scheduler);
		}

		/// <summary>Runs the <see cref="Task" /> synchronously on the current <see cref="TaskScheduler" />.</summary>
		/// <exception cref="ObjectDisposedException">The <see cref="Task" /> instance has been disposed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="Task" /> is not in a valid state to be started. It may have already been started, executed, or canceled, or it may have been created in a manner that doesn't support direct scheduling.</exception>
		public void RunSynchronously()
		{
			Implementation.RunSynchronously();
		}

		/// <summary>Runs the <see cref="Task" /> synchronously on the <see cref="TaskScheduler" /> provided.</summary>
		/// <param name="scheduler">The scheduler on which to attempt to run this task inline.</param>
		/// <exception cref="ObjectDisposedException">The <see cref="Task" /> instance has been disposed.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="scheduler" /> argument is null.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="Task" /> is not in a valid state to be started. It may have already been started, executed, or canceled, or it may have been created in a manner that doesn't support direct scheduling.</exception>
		public void RunSynchronously(TaskScheduler scheduler)
		{
			Implementation.RunSynchronously(scheduler);
		}

#if NET45 || NETSTANDARD2_0
		/// <summary>Releases all resources used by the current instance of the <see cref="Task" /> class.</summary>
		/// <exception cref="T:System.InvalidOperationException">The task is not in one of the final states: <see cref="F:System.Threading.Tasks.TaskStatus.RanToCompletion" />, <see cref="F:System.Threading.Tasks.TaskStatus.Faulted" />, or <see cref="F:System.Threading.Tasks.TaskStatus.Canceled" />.</exception>
		public void Dispose()
		{
			Implementation.Dispose();
		}
#endif

		/// <summary>Gets an awaiter used to await this <see cref="Task" />.</summary>
		/// <returns>An awaiter instance.</returns>
		public TaskAwaiter GetAwaiter()
		{
			return Implementation.GetAwaiter();
		}

		/// <summary>Configures an awaiter used to await this <see cref="Task" />.</summary>
		/// <param name="continueOnCapturedContext">true to attempt to marshal the continuation back to the original context captured; otherwise, false.</param>
		/// <returns>An object used to await this task.</returns>
		public ConfiguredTaskAwaitable ConfigureAwait(bool continueOnCapturedContext)
		{
			return Implementation.ConfigureAwait(continueOnCapturedContext);
		}

		/// <summary>Waits for the <see cref="Task" /> to complete execution.</summary>
		/// <exception cref="ObjectDisposedException">The <see cref="Task" /> has been disposed.</exception>
		/// <exception cref="T:System.AggregateException">The task was canceled. The <see cref="P:System.AggregateException.InnerExceptions" /> collection contains a <see cref="TaskCanceledException" /> object. -or-An exception was thrown during the execution of the task. The <see cref="P:System.AggregateException.InnerExceptions" /> collection contains information about the exception or exceptions. </exception>
		public void Wait()
		{
			Implementation.Wait();
		}

		/// <summary>Waits for the <see cref="Task" /> to complete execution within a specified time interval.</summary>
		/// <param name="timeout">A <see cref="T:System.TimeSpan" /> that represents the number of milliseconds to wait, or a <see cref="T:System.TimeSpan" /> that represents -1 milliseconds to wait indefinitely.</param>
		/// <returns>true if the <see cref="Task" /> completed execution within the allotted time; otherwise, false.</returns>
		/// <exception cref="ObjectDisposedException">The <see cref="Task" /> has been disposed.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// <paramref name="timeout" /> is a negative number other than -1 milliseconds, which represents an infinite time-out. -or-<paramref name="timeout" /> is greater than <see cref="F:System.Int32.MaxValue" />. </exception>
		/// <exception cref="T:System.AggregateException">The task was canceled. The <see cref="P:System.AggregateException.InnerExceptions" /> collection contains a <see cref="TaskCanceledException" /> object. -or-An exception was thrown during the execution of the task. The <see cref="P:System.AggregateException.InnerExceptions" /> collection contains information about the exception or exceptions. </exception>
		public bool Wait(TimeSpan timeout)
		{
			return Implementation.Wait(timeout);
		}

		/// <summary>Waits for the <see cref="Task" /> to complete execution. The wait terminates if a cancellation token is canceled before the task completes. </summary>
		/// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete. </param>
		/// <exception cref="T:System.OperationCanceledException">The <paramref name="cancellationToken" /> was canceled.</exception>
		/// <exception cref="ObjectDisposedException">The task has been disposed.</exception>
		/// <exception cref="T:System.AggregateException">The task was canceled. The <see cref="P:System.AggregateException.InnerExceptions" /> collection contains a <see cref="TaskCanceledException" /> object. -or-An exception was thrown during the execution of the task. The <see cref="P:System.AggregateException.InnerExceptions" /> collection contains information about the exception or exceptions. </exception>
		public void Wait(CancellationToken cancellationToken)
		{
			Implementation.Wait(cancellationToken);
		}

		/// <summary>Waits for the <see cref="Task" /> to complete execution within a specified number of milliseconds.</summary>
		/// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="F:System.Threading.Timeout.Infinite" /> (-1) to wait indefinitely.</param>
		/// <returns>true if the <see cref="Task" /> completed execution within the allotted time; otherwise, false.</returns>
		/// <exception cref="ObjectDisposedException">The <see cref="Task" /> has been disposed.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// <paramref name="millisecondsTimeout" /> is a negative number other than -1, which represents an infinite time-out.</exception>
		/// <exception cref="T:System.AggregateException">The task was canceled. The <see cref="P:System.AggregateException.InnerExceptions" /> collection contains a <see cref="TaskCanceledException" /> object. -or-An exception was thrown during the execution of the task. The <see cref="P:System.AggregateException.InnerExceptions" /> collection contains information about the exception or exceptions. </exception>
		public bool Wait(int millisecondsTimeout)
		{
			return Implementation.Wait(millisecondsTimeout);
		}

		/// <summary>Waits for the <see cref="Task" /> to complete execution. The wait terminates if a timeout interval elapses or a cancellation token is canceled before the task completes. </summary>
		/// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="F:System.Threading.Timeout.Infinite" /> (-1) to wait indefinitely. </param>
		/// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete. </param>
		/// <returns>true if the <see cref="Task" /> completed execution within the allotted time; otherwise, false.</returns>
		/// <exception cref="T:System.OperationCanceledException">The <paramref name="cancellationToken" /> was canceled.</exception>
		/// <exception cref="ObjectDisposedException">The <see cref="Task" /> has been disposed.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// <paramref name="millisecondsTimeout" /> is a negative number other than -1, which represents an infinite time-out.</exception>
		/// <exception cref="T:System.AggregateException">The task was canceled. The <see cref="P:System.AggregateException.InnerExceptions" /> collection contains a <see cref="TaskCanceledException" /> object. -or-An exception was thrown during the execution of the task. The <see cref="P:System.AggregateException.InnerExceptions" /> collection contains information about the exception or exceptions. </exception>
		public bool Wait(int millisecondsTimeout, CancellationToken cancellationToken)
		{
			return Implementation.Wait(millisecondsTimeout, cancellationToken);
		}

		/// <summary>Creates a continuation that executes asynchronously when the target <see cref="Task" /> completes.</summary>
		/// <param name="continuationAction">An action to run when the <see cref="Task" /> completes. When run, the delegate will be passed the completed task as an argument.</param>
		/// <returns>A new continuation <see cref="Task" />.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationAction" /> argument is null.</exception>
		public ITask ContinueWith(Action<ITask> continuationAction)
		{
			return Implementation.ContinueWith(Convert(continuationAction)).ToInterface();
		}

		/// <summary>Creates a continuation that receives a cancellation token and executes asynchronously when the target <see cref="Task" /> completes.</summary>
		/// <param name="continuationAction">An action to run when the <see cref="Task" /> completes. When run, the delegate will be passed the completed task as an argument.</param>
		/// <param name="cancellationToken">The <see cref="P:System.Threading.Tasks.TaskFactory.CancellationToken" /> that will be assigned to the new continuation task.</param>
		/// <returns>A new continuation <see cref="Task" />.</returns>
		/// <exception cref="ObjectDisposedException">The <see cref="T:System.Threading.CancellationTokenSource" /> that created the token has already been disposed. </exception>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationAction" /> argument is null.</exception>
		public ITask ContinueWith(Action<ITask> continuationAction, CancellationToken cancellationToken)
		{
			return Implementation.ContinueWith(Convert(continuationAction), cancellationToken).ToInterface();
		}

		/// <summary>Creates a continuation that executes asynchronously when the target <see cref="Task" /> completes. The continuation uses a specified scheduler. </summary>
		/// <param name="continuationAction">An action to run when the <see cref="Task" /> completes. When run, the delegate will be passed the completed task as an argument.</param>
		/// <param name="scheduler">The <see cref="TaskScheduler" /> to associate with the continuation task and to use for its execution.</param>
		/// <returns>A new continuation <see cref="Task" />.</returns>
		/// <exception cref="ObjectDisposedException">The <see cref="Task" /> has been disposed.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationAction" /> argument is null. -or-The <paramref name="scheduler" /> argument is null.</exception>
		public ITask ContinueWith(Action<ITask> continuationAction, TaskScheduler scheduler)
		{
			return Implementation.ContinueWith(Convert(continuationAction), scheduler).ToInterface();
		}

		/// <summary>Creates a continuation that executes when the target task completes according to the specified <see cref="TaskContinuationOptions" />.</summary>
		/// <param name="continuationAction">An action to run according to the specified <paramref name="continuationOptions" />. When run, the delegate will be passed the completed task as an argument.</param>
		/// <param name="continuationOptions">Options for when the continuation is scheduled and how it behaves. This includes criteria, such as <see cref="F:System.Threading.Tasks.TaskContinuationOptions.OnlyOnCanceled" />, as well as execution options, such as <see cref="F:System.Threading.Tasks.TaskContinuationOptions.ExecuteSynchronously" />.</param>
		/// <returns>A new continuation <see cref="Task" />.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationAction" /> argument is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="continuationOptions" /> argument specifies an invalid value for <see cref="TaskContinuationOptions" />.</exception>
		public ITask ContinueWith(Action<ITask> continuationAction, TaskContinuationOptions continuationOptions)
		{
			return Implementation.ContinueWith(Convert(continuationAction), continuationOptions).ToInterface();
		}

		/// <summary>Creates a continuation that executes when the target task competes according to the specified <see cref="TaskContinuationOptions" />. The continuation receives a cancellation token and uses a specified scheduler. </summary>
		/// <param name="continuationAction">An action to run according to the specified <paramref name="continuationOptions" />. When run, the delegate will be passed the completed task as an argument.</param>
		/// <param name="cancellationToken">The <see cref="P:System.Threading.Tasks.TaskFactory.CancellationToken" /> that will be assigned to the new continuation task.</param>
		/// <param name="continuationOptions">Options for when the continuation is scheduled and how it behaves. This includes criteria, such as <see cref="F:System.Threading.Tasks.TaskContinuationOptions.OnlyOnCanceled" />, as well as execution options, such as <see cref="F:System.Threading.Tasks.TaskContinuationOptions.ExecuteSynchronously" />.</param>
		/// <param name="scheduler">The <see cref="TaskScheduler" /> to associate with the continuation task and to use for its execution.</param>
		/// <returns>A new continuation <see cref="Task" />.</returns>
		/// <exception cref="ObjectDisposedException">The <see cref="T:System.Threading.CancellationTokenSource" /> that created the token has already been disposed.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationAction" /> argument is null.-or-The <paramref name="scheduler" /> argument is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="continuationOptions" /> argument specifies an invalid value for <see cref="TaskContinuationOptions" />.</exception>
		public ITask ContinueWith(Action<ITask> continuationAction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
		{
			return Implementation.ContinueWith(Convert(continuationAction), cancellationToken, continuationOptions, scheduler).ToInterface();
		}

		/// <summary>Creates a continuation that receives caller-supplied state information and executes when the target <see cref="Task" /> completes. </summary>
		/// <param name="continuationAction">An action to run when the task completes. When run, the delegate is passed the completed task and a caller-supplied state object as arguments. </param>
		/// <param name="state">An object representing data to be used by the continuation action. </param>
		/// <returns>A new continuation task. </returns>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationAction" /> argument is null.</exception>
		public ITask ContinueWith(Action<ITask, object> continuationAction, object state)
		{
			return Implementation.ContinueWith(Convert(continuationAction), state).ToInterface();
		}

		/// <summary>Creates a continuation that receives caller-supplied state information and a cancellation token and that executes asynchronously when the target <see cref="Task" /> completes.</summary>
		/// <param name="continuationAction">An action to run when the <see cref="Task" /> completes. When run, the delegate will be passed the completed task and the caller-supplied state object as arguments.</param>
		/// <param name="state">An object representing data to be used by the continuation action.</param>
		/// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> that will be assigned to the new continuation task.</param>
		/// <returns>A new continuation <see cref="Task" />.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationAction" /> argument is null.</exception>
		/// <exception cref="ObjectDisposedException">The provided <see cref="T:System.Threading.CancellationToken" /> has already been disposed.</exception>
		public ITask ContinueWith(Action<ITask, object> continuationAction, object state, CancellationToken cancellationToken)
		{
			return Implementation.ContinueWith(Convert(continuationAction), state, cancellationToken).ToInterface();
		}

		/// <summary>Creates a continuation that receives caller-supplied state information and executes asynchronously when the target <see cref="Task" /> completes. The continuation uses a specified scheduler. </summary>
		/// <param name="continuationAction">An action to run when the <see cref="Task" /> completes.  When run, the delegate will be  passed the completed task and the caller-supplied state object as arguments.</param>
		/// <param name="state">An object representing data to be used by the continuation action.</param>
		/// <param name="scheduler">The <see cref="TaskScheduler" /> to associate with the continuation task and to use for its execution.</param>
		/// <returns>A new continuation <see cref="Task" />.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationAction" /> argument is null.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="scheduler" /> argument is null.</exception>
		public ITask ContinueWith(Action<ITask, object> continuationAction, object state, TaskScheduler scheduler)
		{
			return Implementation.ContinueWith(Convert(continuationAction), state, scheduler).ToInterface();
		}

		/// <summary>Creates a continuation that receives caller-supplied state information and executes when the target <see cref="Task" /> completes. The continuation executes based on a set of specified conditions. </summary>
		/// <param name="continuationAction">An action to run when the <see cref="Task" /> completes. When run, the delegate will be  passed the completed task and the caller-supplied state object as arguments.</param>
		/// <param name="state">An object representing data to be used by the continuation action.</param>
		/// <param name="continuationOptions">Options for when the continuation is scheduled and how it behaves. This includes criteria, such as <see cref="F:System.Threading.Tasks.TaskContinuationOptions.OnlyOnCanceled" />, as well as execution options, such as <see cref="F:System.Threading.Tasks.TaskContinuationOptions.ExecuteSynchronously" />.</param>
		/// <returns>A new continuation <see cref="Task" />.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationAction" /> argument is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="continuationOptions" /> argument specifies an invalid value for <see cref="TaskContinuationOptions" />.</exception>
		public ITask ContinueWith(Action<ITask, object> continuationAction, object state, TaskContinuationOptions continuationOptions)
		{
			return Implementation.ContinueWith(Convert(continuationAction), state, continuationOptions).ToInterface();
		}

		/// <summary>Creates a continuation that receives caller-supplied state information and a cancellation token and that executes when the target <see cref="Task" /> completes. The continuation executes based on a set of specified conditions and uses a specified scheduler. </summary>
		/// <param name="continuationAction">An action to run when the <see cref="Task" /> completes. When run, the delegate will be  passed the completed task and the caller-supplied state object as arguments.</param>
		/// <param name="state">An object representing data to be used by the continuation action.</param>
		/// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> that will be assigned to the new continuation task.</param>
		/// <param name="continuationOptions">Options for when the continuation is scheduled and how it behaves. This includes criteria, such as <see cref="F:System.Threading.Tasks.TaskContinuationOptions.OnlyOnCanceled" />, as well as execution options, such as <see cref="F:System.Threading.Tasks.TaskContinuationOptions.ExecuteSynchronously" />.</param>
		/// <param name="scheduler">The <see cref="TaskScheduler" /> to associate with the continuation task and to use for its  execution.</param>
		/// <returns>A new continuation <see cref="Task" />.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationAction" /> argument is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="continuationOptions" /> argument specifies an invalid value for <see cref="TaskContinuationOptions" />.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="scheduler" /> argument is null.</exception>
		/// <exception cref="ObjectDisposedException">The provided <see cref="T:System.Threading.CancellationToken" /> has already been disposed.</exception>
		public ITask ContinueWith(Action<ITask, object> continuationAction, object state, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
		{
			return Implementation.ContinueWith(Convert(continuationAction), state, cancellationToken, continuationOptions, scheduler).ToInterface();
		}

		/// <summary>Creates a continuation that executes asynchronously when the target <see cref="Task{TResult}" /> completes and returns a value. </summary>
		/// <param name="continuationFunction">A function to run when the <see cref="Task{TResult}" />  completes. When run, the delegate will be passed the completed task as an argument.</param>
		/// <typeparam name="TResult"> The type of the result produced by the continuation.</typeparam>
		/// <returns>A new continuation task. </returns>
		/// <exception cref="ObjectDisposedException">The <see cref="Task" /> has been disposed.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationFunction" /> argument is null.</exception>
		public ITask<TResult> ContinueWith<TResult>(Func<ITask, TResult> continuationFunction)
		{
			return Implementation.ContinueWith(Convert(continuationFunction)).ToInterface();
		}

		/// <summary>Creates a continuation that executes asynchronously when the target <see cref="Task" /> completes and returns a value. The continuation receives a cancellation token. </summary>
		/// <param name="continuationFunction">A function to run when the <see cref="Task" /> completes. When run, the delegate will be passed the completed task as an argument.</param>
		/// <param name="cancellationToken">The <see cref="P:System.Threading.Tasks.TaskFactory.CancellationToken" /> that will be assigned to the new continuation task.</param>
		/// <typeparam name="TResult"> The type of the result produced by the continuation.</typeparam>
		/// <returns>A new continuation <see cref="Task{TResult}" />.</returns>
		/// <exception cref="ObjectDisposedException">The <see cref="Task" /> has been disposed.-or-The <see cref="T:System.Threading.CancellationTokenSource" /> that created the token has already been disposed.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationFunction" /> argument is null.</exception>
		public ITask<TResult> ContinueWith<TResult>(Func<ITask, TResult> continuationFunction, CancellationToken cancellationToken)
		{
			return Implementation.ContinueWith(Convert(continuationFunction), cancellationToken).ToInterface();
		}

		/// <summary>Creates a continuation that executes asynchronously when the target <see cref="Task" /> completes and returns a value. The continuation uses a specified scheduler. </summary>
		/// <param name="continuationFunction">A function to run when the <see cref="Task" /> completes. When run, the delegate will be passed the completed task as an argument.</param>
		/// <param name="scheduler">The <see cref="TaskScheduler" /> to associate with the continuation task and to use for its execution.</param>
		/// <typeparam name="TResult"> The type of the result produced by the continuation.</typeparam>
		/// <returns>A new continuation <see cref="Task{TResult}" />.</returns>
		/// <exception cref="ObjectDisposedException">The <see cref="Task" /> has been disposed.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationFunction" /> argument is null.-or-The <paramref name="scheduler" /> argument is null.</exception>
		public ITask<TResult> ContinueWith<TResult>(Func<ITask, TResult> continuationFunction, TaskScheduler scheduler)
		{
			return Implementation.ContinueWith(Convert(continuationFunction), scheduler).ToInterface();
		}

		/// <summary>Creates a continuation that executes according to the specified continuation options and returns a value. </summary>
		/// <param name="continuationFunction">A function to run according to the condition specified in <paramref name="continuationOptions" />. When run, the delegate will be passed the completed task as an argument.</param>
		/// <param name="continuationOptions">Options for when the continuation is scheduled and how it behaves. This includes criteria, such as <see cref="F:System.Threading.Tasks.TaskContinuationOptions.OnlyOnCanceled" />, as well as execution options, such as <see cref="F:System.Threading.Tasks.TaskContinuationOptions.ExecuteSynchronously" />.</param>
		/// <typeparam name="TResult"> The type of the result produced by the continuation.</typeparam>
		/// <returns>A new continuation <see cref="Task{TResult}" />.</returns>
		/// <exception cref="ObjectDisposedException">The <see cref="Task" /> has been disposed.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationFunction" /> argument is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="continuationOptions" /> argument specifies an invalid value for <see cref="TaskContinuationOptions" />.</exception>
		public ITask<TResult> ContinueWith<TResult>(Func<ITask, TResult> continuationFunction, TaskContinuationOptions continuationOptions)
		{
			return Implementation.ContinueWith(Convert(continuationFunction), continuationOptions).ToInterface();
		}

		/// <summary>Creates a continuation that executes according to the specified continuation options and returns a value. The continuation is passed a cancellation token and uses a specified scheduler. </summary>
		/// <param name="continuationFunction">A function to run according to the specified <paramref name="continuationOptions." /> When run, the delegate will be passed the completed task as an argument.</param>
		/// <param name="cancellationToken">The <see cref="P:System.Threading.Tasks.TaskFactory.CancellationToken" /> that will be assigned to the new continuation task.</param>
		/// <param name="continuationOptions">Options for when the continuation is scheduled and how it behaves. This includes criteria, such as <see cref="F:System.Threading.Tasks.TaskContinuationOptions.OnlyOnCanceled" />, as well as execution options, such as <see cref="F:System.Threading.Tasks.TaskContinuationOptions.ExecuteSynchronously" />.</param>
		/// <param name="scheduler">The <see cref="TaskScheduler" /> to associate with the continuation task and to use for its execution.</param>
		/// <typeparam name="TResult"> The type of the result produced by the continuation.</typeparam>
		/// <returns>A new continuation <see cref="Task{TResult}" />.</returns>
		/// <exception cref="ObjectDisposedException">The <see cref="Task" /> has been disposed.-or-The <see cref="T:System.Threading.CancellationTokenSource" /> that created the token has already been disposed.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationFunction" /> argument is null.-or-The <paramref name="scheduler" /> argument is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="continuationOptions" /> argument specifies an invalid value for <see cref="TaskContinuationOptions" />.</exception>
		public ITask<TResult> ContinueWith<TResult>(Func<ITask, TResult> continuationFunction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
		{
			return Implementation.ContinueWith(Convert(continuationFunction), cancellationToken, continuationOptions, scheduler).ToInterface();
		}

		/// <summary>Creates a continuation that receives caller-supplied state information and executes asynchronously when the target <see cref="Task" /> completes and returns a value. </summary>
		/// <param name="continuationFunction">A function to run when the <see cref="Task" /> completes. When run, the delegate will be passed the completed task and the caller-supplied state object as arguments. </param>
		/// <param name="state">An object representing data to be used by the continuation function.</param>
		/// <typeparam name="TResult">The type of the result produced by the continuation.</typeparam>
		/// <returns>A new continuation <see cref="Task{TResult}" />.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationFunction" /> argument is null.</exception>
		public ITask<TResult> ContinueWith<TResult>(Func<ITask, object, TResult> continuationFunction, object state)
		{
			return Implementation.ContinueWith(Convert(continuationFunction), state).ToInterface();
		}

		/// <summary>Creates a continuation that executes asynchronously when the target <see cref="Task" /> completes and returns a value. The continuation receives caller-supplied state information and a cancellation token. </summary>
		/// <param name="continuationFunction">A function to run when the <see cref="Task" /> completes. When run, the delegate will be  passed the completed task and the caller-supplied state object as arguments.</param>
		/// <param name="state">An object representing data to be used by the continuation function.</param>
		/// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> that will be assigned to the new continuation task.</param>
		/// <typeparam name="TResult">The type of the result produced by the continuation.</typeparam>
		/// <returns>A new continuation <see cref="Task{TResult}" />.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationFunction" /> argument is null.</exception>
		/// <exception cref="ObjectDisposedException">The provided <see cref="T:System.Threading.CancellationToken" /> has already been disposed.</exception>
		public ITask<TResult> ContinueWith<TResult>(Func<ITask, object, TResult> continuationFunction, object state, CancellationToken cancellationToken)
		{
			return Implementation.ContinueWith(Convert(continuationFunction), state, cancellationToken).ToInterface();
		}

		/// <summary>Creates a continuation that executes asynchronously when the target <see cref="Task" /> completes. The continuation receives caller-supplied state information and uses a specified scheduler. </summary>
		/// <param name="continuationFunction">A function to run when the <see cref="Task" /> completes.  When run, the delegate will be  passed the completed task and the caller-supplied state object as arguments.</param>
		/// <param name="state">An object representing data to be used by the continuation function.</param>
		/// <param name="scheduler">The <see cref="TaskScheduler" /> to associate with the continuation task and to use for its execution.</param>
		/// <typeparam name="TResult">The type of the result produced by the continuation.</typeparam>
		/// <returns>A new continuation <see cref="Task{TResult}" />.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationFunction" /> argument is null.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="scheduler" /> argument is null.</exception>
		public ITask<TResult> ContinueWith<TResult>(Func<ITask, object, TResult> continuationFunction, object state, TaskScheduler scheduler)
		{
			return Implementation.ContinueWith(Convert(continuationFunction), state, scheduler).ToInterface();
		}

		/// <summary>Creates a continuation that executes based on the specified task continuation options when the target <see cref="Task" /> completes. The continuation receives caller-supplied state information. </summary>
		/// <param name="continuationFunction">A function to run when the <see cref="Task" /> completes. When run, the delegate will be  passed the completed task and the caller-supplied state object as arguments.</param>
		/// <param name="state">An object representing data to be used by the continuation function.</param>
		/// <param name="continuationOptions">Options for when the continuation is scheduled and how it behaves. This includes criteria, such as <see cref="F:System.Threading.Tasks.TaskContinuationOptions.OnlyOnCanceled" />, as well as execution options, such as <see cref="F:System.Threading.Tasks.TaskContinuationOptions.ExecuteSynchronously" />.</param>
		/// <typeparam name="TResult">The type of the result produced by the continuation.</typeparam>
		/// <returns>A new continuation <see cref="Task{TResult}" />.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationFunction" /> argument is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="continuationOptions" /> argument specifies an invalid value for <see cref="TaskContinuationOptions" />.</exception>
		public ITask<TResult> ContinueWith<TResult>(Func<ITask, object, TResult> continuationFunction, object state, TaskContinuationOptions continuationOptions)
		{
			return Implementation.ContinueWith(Convert(continuationFunction), state, continuationOptions).ToInterface();
		}

		/// <summary>Creates a continuation that executes based on the specified task continuation options when the target <see cref="Task" /> completes and returns a value. The continuation receives caller-supplied state information and a cancellation token and uses the specified scheduler. </summary>
		/// <param name="continuationFunction">A function to run when the <see cref="Task" /> completes. When run, the delegate will be  passed the completed task and the caller-supplied state object as arguments.</param>
		/// <param name="state">An object representing data to be used by the continuation function.</param>
		/// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> that will be assigned to the new continuation task.</param>
		/// <param name="continuationOptions">Options for when the continuation is scheduled and how it behaves. This includes criteria, such as <see cref="F:System.Threading.Tasks.TaskContinuationOptions.OnlyOnCanceled" />, as well as execution options, such as <see cref="F:System.Threading.Tasks.TaskContinuationOptions.ExecuteSynchronously" />.</param>
		/// <param name="scheduler">The <see cref="TaskScheduler" /> to associate with the continuation task and to use for its  execution.</param>
		/// <typeparam name="TResult">The type of the result produced by the continuation.</typeparam>
		/// <returns>A new continuation <see cref="Task{TResult}" />.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationFunction" /> argument is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="continuationOptions" /> argument specifies an invalid value for <see cref="TaskContinuationOptions" />.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="scheduler" /> argument is null.</exception>
		/// <exception cref="ObjectDisposedException">The provided <see cref="T:System.Threading.CancellationToken" /> has already been disposed.</exception>
		public ITask<TResult> ContinueWith<TResult>(Func<ITask, object, TResult> continuationFunction, object state, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
		{
			return Implementation.ContinueWith(Convert(continuationFunction), state, cancellationToken, continuationOptions, scheduler).ToInterface();
		}

		[CanBeNull]
		internal static Action<Task, object> Convert([CanBeNull] Action<ITask, object> action)
		{
			return action == null ? (Action<Task, object>)null : (t, s) => action(t.ToInterface(), s);
		}

		[CanBeNull]
		internal static Action<Task> Convert([CanBeNull] Action<ITask> action)
		{
			return action == null ? (Action<Task>)null : t => action(t.ToInterface());
		}

		[CanBeNull]
		internal static Func<Task, TResult> Convert<TResult>([CanBeNull] Func<ITask, TResult> func)
		{
			return func == null ? (Func<Task, TResult>)null : t => func(t.ToInterface());
		}

		[CanBeNull]
		internal static Func<Task, object, TResult> Convert<TResult>([CanBeNull] Func<ITask, object, TResult> func)
		{
			return func == null ? (Func<Task, object, TResult>)null : (t, s) => func(t.ToInterface(), s);
		}

		[CanBeNull]
		internal static Func<Task> Convert([CanBeNull] Func<ITask> func)
		{
			return func == null ? (Func<Task>)null : () => func().ToImplementation();
		}

		[CanBeNull]
		internal static Func<Task<TResult>> Convert<TResult>([CanBeNull] Func<ITask<TResult>> func)
		{
			return func == null ? (Func<Task<TResult>>)null : () => func().ToImplementation();
		}
	}

	/// <summary>
	/// Represents an asynchronous operation.To browse the .NET Framework source code for this type, see the Reference Source.
	/// </summary>
	/// <typeparam name="TResult">Type of the result.</typeparam>
	[AsyncMethodBuilder(typeof(AsyncTaskAdapterMethodBuilder<>))]
	[SuppressMessage("ReSharper", "TaskOfTMethodsWithoutAsyncSuffix")]
	[SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
	public class TaskAdapter<TResult> : TaskAdapter, ITask<TResult>
	{
		/// <summary>
		/// Implementation used by the adapter.
		/// </summary>
		[NotNull]
		protected new Task<TResult> Implementation { get; }

		/// <inheritdoc />
		[NotNull, EditorBrowsable(EditorBrowsableState.Never)]
		public new Task<TResult> UnsafeConvert()
		{
			return Implementation;
		}

		/// <summary>Gets the result value of this <see cref="Task{TResult}" />.</summary>
		/// <returns>The result value of this <see cref="Task{TResult}" />, which is the same type as the task's type parameter.</returns>
		/// <exception cref="T:System.AggregateException">The task was canceled. The <see cref="P:System.AggregateException.InnerExceptions" /> collection contains a <see cref="TaskCanceledException" /> object. -or-An exception was thrown during the execution of the task. The <see cref="P:System.AggregateException.InnerExceptions" /> collection contains information about the exception or exceptions. </exception>
		public TResult Result => Implementation.Result;

		/// <summary>Provides access to factory methods for creating and configuring <see cref="TaskAdapter{TResult}" /> instances.</summary>
		/// <returns>A factory object that can create a variety of <see cref="TaskAdapter{TResult}" /> objects.</returns>
		[NotNull]
		public new static ITaskFactory<TResult> Factory => new TaskFactoryAdapter<TResult>(Task<TResult>.Factory);

		/// <summary>Initializes a new <see cref="TaskAdapter{TResult}" />.</summary>
		/// <param name="task">Task to be used by adapter.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="task" /> argument is null.</exception>
		public TaskAdapter([NotNull] Task<TResult> task)
			: base(task)
		{
			Implementation = task;
		}

		/// <summary>Initializes a new <see cref="TaskAdapter{TResult}" /> with the specified function.</summary>
		/// <param name="function">The delegate that represents the code to execute in the task. When the function has completed, the task's <see cref="TaskAdapter{TResult}.Result" /> property will be set to return the result value of the function.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="function" /> argument is null.</exception>
		public TaskAdapter([NotNull] Func<TResult> function)
			: this(new Task<TResult>(function))
		{
		}

		/// <summary>Initializes a new <see cref="TaskAdapter{TResult}" /> with the specified function.</summary>
		/// <param name="function">The delegate that represents the code to execute in the task. When the function has completed, the task's <see cref="TaskAdapter{TResult}.Result" /> property will be set to return the result value of the function.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken" /> to be assigned to this task.</param>
		/// <exception cref="ObjectDisposedException">The <see cref="CancellationTokenSource" /> that created<paramref name=" cancellationToken" /> has already been disposed.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="function" /> argument is null.</exception>
		public TaskAdapter([NotNull] Func<TResult> function, CancellationToken cancellationToken)
			: this(new Task<TResult>(function, cancellationToken))
		{
		}

		/// <summary>Initializes a new <see cref="Task{TResult}" /> with the specified function and creation options.</summary>
		/// <param name="function">The delegate that represents the code to execute in the task. When the function has completed, the task's <see cref="P:System.Threading.Tasks.Task`1.Result" /> property will be set to return the result value of the function.</param>
		/// <param name="creationOptions">The <see cref="TaskCreationOptions" /> used to customize the task's behavior.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="creationOptions" /> argument specifies an invalid value for <see cref="TaskCreationOptions" />.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="function" /> argument is null.</exception>
		public TaskAdapter([NotNull] Func<TResult> function, TaskCreationOptions creationOptions)
			: this(new Task<TResult>(function, creationOptions))
		{
		}

		/// <summary>Initializes a new <see cref="Task{TResult}" /> with the specified function and creation options.</summary>
		/// <param name="function">The delegate that represents the code to execute in the task. When the function has completed, the task's <see cref="P:System.Threading.Tasks.Task`1.Result" /> property will be set to return the result value of the function.</param>
		/// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> that will be assigned to the new task.</param>
		/// <param name="creationOptions">The <see cref="TaskCreationOptions" /> used to customize the task's behavior.</param>
		/// <exception cref="ObjectDisposedException">The <see cref="T:System.Threading.CancellationTokenSource" /> that created<paramref name=" cancellationToken" /> has already been disposed.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="creationOptions" /> argument specifies an invalid value for <see cref="TaskCreationOptions" />.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="function" /> argument is null.</exception>
		public TaskAdapter([NotNull] Func<TResult> function, CancellationToken cancellationToken, TaskCreationOptions creationOptions)
			: this(new Task<TResult>(function, cancellationToken, creationOptions))
		{
		}

		/// <summary>Initializes a new <see cref="Task{TResult}" /> with the specified function and state.</summary>
		/// <param name="function">The delegate that represents the code to execute in the task. When the function has completed, the task's <see cref="P:System.Threading.Tasks.Task`1.Result" /> property will be set to return the result value of the function.</param>
		/// <param name="state">An object representing data to be used by the action.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="function" /> argument is null.</exception>
		public TaskAdapter([NotNull] Func<object, TResult> function, object state)
			: this(new Task<TResult>(function, state))
		{
		}

		/// <summary>Initializes a new <see cref="Task{TResult}" /> with the specified action, state, and options.</summary>
		/// <param name="function">The delegate that represents the code to execute in the task. When the function has completed, the task's <see cref="P:System.Threading.Tasks.Task`1.Result" /> property will be set to return the result value of the function.</param>
		/// <param name="state">An object representing data to be used by the function.</param>
		/// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> to be assigned to the new task.</param>
		/// <exception cref="ObjectDisposedException">The <see cref="T:System.Threading.CancellationTokenSource" /> that created<paramref name=" cancellationToken" /> has already been disposed.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="function" /> argument is null.</exception>
		public TaskAdapter([NotNull] Func<object, TResult> function, object state, CancellationToken cancellationToken)
			: this(new Task<TResult>(function, state, cancellationToken))
		{
		}

		/// <summary>Initializes a new <see cref="Task{TResult}" /> with the specified action, state, and options.</summary>
		/// <param name="function">The delegate that represents the code to execute in the task. When the function has completed, the task's <see cref="P:System.Threading.Tasks.Task`1.Result" /> property will be set to return the result value of the function.</param>
		/// <param name="state">An object representing data to be used by the function.</param>
		/// <param name="creationOptions">The <see cref="TaskCreationOptions" /> used to customize the task's behavior.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="creationOptions" /> argument specifies an invalid value for <see cref="TaskCreationOptions" />.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="function" /> argument is null.</exception>
		public TaskAdapter([NotNull] Func<object, TResult> function, object state, TaskCreationOptions creationOptions)
			: this(new Task<TResult>(function, state, creationOptions))
		{
		}

		/// <summary>Initializes a new <see cref="Task{TResult}" /> with the specified action, state, and options.</summary>
		/// <param name="function">The delegate that represents the code to execute in the task. When the function has completed, the task's <see cref="P:System.Threading.Tasks.Task`1.Result" /> property will be set to return the result value of the function.</param>
		/// <param name="state">An object representing data to be used by the function.</param>
		/// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> to be assigned to the new task.</param>
		/// <param name="creationOptions">The <see cref="TaskCreationOptions" /> used to customize the task's behavior.</param>
		/// <exception cref="ObjectDisposedException">The <see cref="T:System.Threading.CancellationTokenSource" /> that created<paramref name=" cancellationToken" /> has already been disposed.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="creationOptions" /> argument specifies an invalid value for <see cref="TaskCreationOptions" />.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="function" /> argument is null.</exception>
		public TaskAdapter([NotNull] Func<object, TResult> function, object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions)
			: this(new Task<TResult>(function, state, cancellationToken, creationOptions))
		{
		}

		/// <summary>Gets an awaiter used to await this <see cref="Task{TResult}" />.</summary>
		/// <returns>An awaiter instance.</returns>
		public new TaskAwaiter<TResult> GetAwaiter()
		{
			return Implementation.GetAwaiter();
		}

		/// <summary>Configures an awaiter used to await this <see cref="Task{TResult}" />.</summary>
		/// <param name="continueOnCapturedContext">true to attempt to marshal the continuation back to the original context captured; otherwise, false.</param>
		/// <returns>An object used to await this task.</returns>
		public new ConfiguredTaskAwaitable<TResult> ConfigureAwait(bool continueOnCapturedContext)
		{
			return Implementation.ConfigureAwait(continueOnCapturedContext);
		}

		/// <summary>Creates a continuation that executes asynchronously when the target task completes. </summary>
		/// <param name="continuationAction">An action to run when the antecedent <see cref="Task{TResult}" /> completes. When run, the delegate will be passed the completed task as an argument.</param>
		/// <returns>A new continuation task. </returns>
		/// <exception cref="ObjectDisposedException">The <see cref="Task{TResult}" /> has been disposed. </exception>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationAction" /> argument is null. </exception>
		public ITask ContinueWith(Action<ITask<TResult>> continuationAction)
		{
			return Implementation.ContinueWith(Convert(continuationAction)).ToInterface();
		}

		/// <summary>Creates a cancelable continuation that executes asynchronously when the target <see cref="Task{TResult}" /> completes.</summary>
		/// <param name="continuationAction">An action to run when the <see cref="Task{TResult}" /> completes. When run, the delegate is passed the completed task as an argument. </param>
		/// <param name="cancellationToken">The cancellation token that is passed to the new continuation task. </param>
		/// <returns>A new continuation task. </returns>
		/// <exception cref="ObjectDisposedException">The <see cref="Task{TResult}" /> has been disposed.-or-The <see cref="T:System.Threading.CancellationTokenSource" /> that created <paramref name="cancellationToken" /> has been disposed. </exception>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationAction" /> argument is null. </exception>
		public ITask ContinueWith(Action<ITask<TResult>> continuationAction, CancellationToken cancellationToken)
		{
			return Implementation.ContinueWith(Convert(continuationAction), cancellationToken).ToInterface();
		}

		/// <summary>Creates a continuation that executes asynchronously when the target <see cref="Task{TResult}" /> completes.</summary>
		/// <param name="continuationAction">An action to run when the <see cref="Task{TResult}" /> completes. When run, the delegate will be passed the completed task as an argument.</param>
		/// <param name="scheduler">The <see cref="TaskScheduler" /> to associate with the continuation task and to use for its execution.</param>
		/// <returns>A new continuation <see cref="Task" />.</returns>
		/// <exception cref="ObjectDisposedException">The <see cref="Task{TResult}" /> has been disposed.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationAction" /> argument is null.-or-The <paramref name="scheduler" /> argument is null.</exception>
		public ITask ContinueWith(Action<ITask<TResult>> continuationAction, TaskScheduler scheduler)
		{
			return Implementation.ContinueWith(Convert(continuationAction), scheduler).ToInterface();
		}

		/// <summary>Creates a continuation that executes according the condition specified in <paramref name="continuationOptions" />.</summary>
		/// <param name="continuationAction">An action to according the condition specified in <paramref name="continuationOptions" />. When run, the delegate will be passed the completed task as an argument.</param>
		/// <param name="continuationOptions">Options for when the continuation is scheduled and how it behaves. This includes criteria, such as <see cref="F:System.Threading.Tasks.TaskContinuationOptions.OnlyOnCanceled" />, as well as execution options, such as <see cref="F:System.Threading.Tasks.TaskContinuationOptions.ExecuteSynchronously" />.</param>
		/// <returns>A new continuation <see cref="Task" />.</returns>
		/// <exception cref="ObjectDisposedException">The <see cref="Task{TResult}" /> has been disposed.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationAction" /> argument is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="continuationOptions" /> argument specifies an invalid value for <see cref="TaskContinuationOptions" />.</exception>
		public ITask ContinueWith(Action<ITask<TResult>> continuationAction, TaskContinuationOptions continuationOptions)
		{
			return Implementation.ContinueWith(Convert(continuationAction), continuationOptions).ToInterface();
		}

		/// <summary>Creates a continuation that executes according the condition specified in <paramref name="continuationOptions" />.</summary>
		/// <param name="continuationAction">An action to run according the condition specified in <paramref name="continuationOptions" />. When run, the delegate will be passed the completed task as an argument.</param>
		/// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> that will be assigned to the new continuation task.</param>
		/// <param name="continuationOptions">Options for when the continuation is scheduled and how it behaves. This includes criteria, such as <see cref="F:System.Threading.Tasks.TaskContinuationOptions.OnlyOnCanceled" />, as well as execution options, such as <see cref="F:System.Threading.Tasks.TaskContinuationOptions.ExecuteSynchronously" />.</param>
		/// <param name="scheduler">The <see cref="TaskScheduler" /> to associate with the continuation task and to use for its execution.</param>
		/// <returns>A new continuation <see cref="Task" />.</returns>
		/// <exception cref="ObjectDisposedException">The <see cref="Task{TResult}" /> has been disposed.-or-The <see cref="T:System.Threading.CancellationTokenSource" /> that created <paramref name="cancellationToken" /> has already been disposed.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationAction" /> argument is null.-or-The <paramref name="scheduler" /> argument is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="continuationOptions" /> argument specifies an invalid value for <see cref="TaskContinuationOptions" />.</exception>
		public ITask ContinueWith(Action<ITask<TResult>> continuationAction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
		{
			return Implementation.ContinueWith(Convert(continuationAction), cancellationToken, continuationOptions, scheduler).ToInterface();
		}

		/// <summary>Creates a continuation that that is passed state information and that executes when the target <see cref="Task{TResult}" /> completes. </summary>
		/// <param name="continuationAction">An action to run when the <see cref="Task{TResult}" /> completes. When run, the delegate is   passed the completed task and the caller-supplied state object as arguments. </param>
		/// <param name="state">An object representing data to be used by the continuation action. </param>
		/// <returns>A new continuation <see cref="Task" />.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationAction" /> argument is null.</exception>
		public ITask ContinueWith(Action<ITask<TResult>, object> continuationAction, object state)
		{
			return Implementation.ContinueWith(Convert(continuationAction), state).ToInterface();
		}

		/// <summary>Creates a continuation that executes when the target <see cref="Task{TResult}" /> completes.</summary>
		/// <param name="continuationAction">An action to run when the <see cref="Task{TResult}" /> completes. When run, the delegate will be  passed the completed task and the caller-supplied state object as arguments.</param>
		/// <param name="state">An object representing data to be used by the continuation action.</param>
		/// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> that will be assigned to the new continuation task.</param>
		/// <returns>A new continuation <see cref="Task" />.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationAction" /> argument is null.</exception>
		/// <exception cref="ObjectDisposedException">The provided <see cref="T:System.Threading.CancellationToken" /> has already been disposed.</exception>
		public ITask ContinueWith(Action<ITask<TResult>, object> continuationAction, object state, CancellationToken cancellationToken)
		{
			return Implementation.ContinueWith(Convert(continuationAction), state, cancellationToken).ToInterface();
		}

		/// <summary>Creates a continuation that executes when the target <see cref="Task{TResult}" /> completes.</summary>
		/// <param name="continuationAction">An action to run when the <see cref="Task{TResult}" /> completes. When run, the delegate will be passed the completed task and the caller-supplied state object as arguments.</param>
		/// <param name="state">An object representing data to be used by the continuation action.</param>
		/// <param name="scheduler">The <see cref="TaskScheduler" /> to associate with the continuation task and to use for its execution.</param>
		/// <returns>A new continuation <see cref="Task" />.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationAction" /> argument is null. </exception>
		/// <exception cref="ArgumentNullException">The <paramref name="scheduler" /> argument is null. </exception>
		public ITask ContinueWith(Action<ITask<TResult>, object> continuationAction, object state, TaskScheduler scheduler)
		{
			return Implementation.ContinueWith(Convert(continuationAction), state, scheduler).ToInterface();
		}

		/// <summary>Creates a continuation that executes when the target <see cref="Task{TResult}" /> completes.</summary>
		/// <param name="continuationAction">An action to run when the <see cref="Task{TResult}" /> completes. When run, the delegate will be  passed the completed task and the caller-supplied state object as arguments.</param>
		/// <param name="state">An object representing data to be used by the continuation action.</param>
		/// <param name="continuationOptions">Options for when the continuation is scheduled and how it behaves. This includes criteria, such  as <see cref="F:System.Threading.Tasks.TaskContinuationOptions.OnlyOnCanceled" />, as well as execution options, such as <see cref="F:System.Threading.Tasks.TaskContinuationOptions.ExecuteSynchronously" />.</param>
		/// <returns>A new continuation <see cref="Task" />.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationAction" /> argument is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="continuationOptions" /> argument specifies an invalid value for <see cref="TaskContinuationOptions" />.</exception>
		public ITask ContinueWith(Action<ITask<TResult>, object> continuationAction, object state, TaskContinuationOptions continuationOptions)
		{
			return Implementation.ContinueWith(Convert(continuationAction), state, continuationOptions).ToInterface();
		}

		/// <summary>Creates a continuation that executes when the target <see cref="Task{TResult}" /> completes.</summary>
		/// <param name="continuationAction">An action to run when the <see cref="Task{TResult}" /> completes. When run, the delegate will be  passed the completed task and the caller-supplied state object as arguments.</param>
		/// <param name="state">An object representing data to be used by the continuation action.</param>
		/// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> that will be assigned to the new continuation task.</param>
		/// <param name="continuationOptions">Options for when the continuation is scheduled and how it behaves. This includes criteria, such as <see cref="F:System.Threading.Tasks.TaskContinuationOptions.OnlyOnCanceled" />, as  well as execution options, such as <see cref="F:System.Threading.Tasks.TaskContinuationOptions.ExecuteSynchronously" />.</param>
		/// <param name="scheduler">The <see cref="TaskScheduler" /> to associate with the continuation task and to use for its  execution.</param>
		/// <returns>A new continuation <see cref="Task" />.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationAction" /> argument is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="continuationOptions" /> argument specifies an invalid value for <see cref="TaskContinuationOptions" />.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="scheduler" /> argument is null.</exception>
		/// <exception cref="ObjectDisposedException">The provided <see cref="T:System.Threading.CancellationToken" /> has already been disposed.</exception>
		public ITask ContinueWith(Action<ITask<TResult>, object> continuationAction, object state, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
		{
			return Implementation.ContinueWith(Convert(continuationAction), state, cancellationToken, continuationOptions, scheduler).ToInterface();
		}

		/// <summary>Creates a continuation that executes asynchronously when the target <see cref="Task{TResult}" /> completes.</summary>
		/// <param name="continuationFunction">A function to run when the <see cref="Task{TResult}" /> completes. When run, the delegate will be passed the completed task as an argument.</param>
		/// <typeparam name="TNewResult"> The type of the result produced by the continuation.</typeparam>
		/// <returns>A new continuation <see cref="Task{TResult}" />.</returns>
		/// <exception cref="ObjectDisposedException">The <see cref="Task{TResult}" /> has been disposed.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationFunction" /> argument is null.</exception>
		public ITask<TNewResult> ContinueWith<TNewResult>(Func<ITask<TResult>, TNewResult> continuationFunction)
		{
			return Implementation.ContinueWith(Convert(continuationFunction)).ToInterface();
		}

		/// <summary>Creates a continuation that executes asynchronously when the target <see cref="Task{TResult}" /> completes.</summary>
		/// <param name="continuationFunction">A function to run when the <see cref="Task{TResult}" /> completes. When run, the delegate will be passed the completed task as an argument.</param>
		/// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> that will be assigned to the new task.</param>
		/// <typeparam name="TNewResult"> The type of the result produced by the continuation.</typeparam>
		/// <returns>A new continuation <see cref="Task{TResult}" />.</returns>
		/// <exception cref="ObjectDisposedException">The <see cref="Task{TResult}" /> has been disposed.-or-The <see cref="T:System.Threading.CancellationTokenSource" /> that created<paramref name=" cancellationToken" /> has already been disposed.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationFunction" /> argument is null.</exception>
		public ITask<TNewResult> ContinueWith<TNewResult>(Func<ITask<TResult>, TNewResult> continuationFunction, CancellationToken cancellationToken)
		{
			return Implementation.ContinueWith(Convert(continuationFunction), cancellationToken).ToInterface();
		}

		/// <summary>Creates a continuation that executes asynchronously when the target <see cref="Task{TResult}" /> completes.</summary>
		/// <param name="continuationFunction">A function to run when the <see cref="Task{TResult}" /> completes. When run, the delegate will be passed the completed task as an argument.</param>
		/// <param name="scheduler">The <see cref="TaskScheduler" /> to associate with the continuation task and to use for its execution.</param>
		/// <typeparam name="TNewResult"> The type of the result produced by the continuation.</typeparam>
		/// <returns>A new continuation <see cref="Task{TResult}" />.</returns>
		/// <exception cref="ObjectDisposedException">The <see cref="Task{TResult}" /> has been disposed.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationFunction" /> argument is null.-or-The <paramref name="scheduler" /> argument is null.</exception>
		public ITask<TNewResult> ContinueWith<TNewResult>(Func<ITask<TResult>, TNewResult> continuationFunction, TaskScheduler scheduler)
		{
			return Implementation.ContinueWith(Convert(continuationFunction), scheduler).ToInterface();
		}

		/// <summary>Creates a continuation that executes according the condition specified in <paramref name="continuationOptions" />.</summary>
		/// <param name="continuationFunction">A function to run according the condition specified in <paramref name="continuationOptions" />.When run, the delegate will be passed the completed task as an argument.</param>
		/// <param name="continuationOptions">Options for when the continuation is scheduled and how it behaves. This includes criteria, such as <see cref="F:System.Threading.Tasks.TaskContinuationOptions.OnlyOnCanceled" />, as well as execution options, such as <see cref="F:System.Threading.Tasks.TaskContinuationOptions.ExecuteSynchronously" />.</param>
		/// <typeparam name="TNewResult"> The type of the result produced by the continuation.</typeparam>
		/// <returns>A new continuation <see cref="Task{TResult}" />.</returns>
		/// <exception cref="ObjectDisposedException">The <see cref="Task{TResult}" /> has been disposed.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationFunction" /> argument is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="continuationOptions" /> argument specifies an invalid value for <see cref="TaskContinuationOptions" />.</exception>
		public ITask<TNewResult> ContinueWith<TNewResult>(Func<ITask<TResult>, TNewResult> continuationFunction, TaskContinuationOptions continuationOptions)
		{
			return Implementation.ContinueWith(Convert(continuationFunction), continuationOptions).ToInterface();
		}

		/// <summary>Creates a continuation that executes according the condition specified in <paramref name="continuationOptions" />.</summary>
		/// <param name="continuationFunction">A function to run according the condition specified in <paramref name="continuationOptions" />.When run, the delegate will be passed as an argument this completed task.</param>
		/// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> that will be assigned to the new task.</param>
		/// <param name="continuationOptions">Options for when the continuation is scheduled and how it behaves. This includes criteria, such as <see cref="F:System.Threading.Tasks.TaskContinuationOptions.OnlyOnCanceled" />, as well as execution options, such as <see cref="F:System.Threading.Tasks.TaskContinuationOptions.ExecuteSynchronously" />.</param>
		/// <param name="scheduler">The <see cref="TaskScheduler" /> to associate with the continuation task and to use for its execution.</param>
		/// <typeparam name="TNewResult"> The type of the result produced by the continuation.</typeparam>
		/// <returns>A new continuation <see cref="Task{TResult}" />.</returns>
		/// <exception cref="ObjectDisposedException">The <see cref="Task{TResult}" /> has been disposed.-or-The <see cref="T:System.Threading.CancellationTokenSource" /> that created<paramref name=" cancellationToken" /> has already been disposed.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationFunction" /> argument is null.-or-The <paramref name="scheduler" /> argument is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="continuationOptions" /> argument specifies an invalid value for <see cref="TaskContinuationOptions" />.</exception>
		public ITask<TNewResult> ContinueWith<TNewResult>(Func<ITask<TResult>, TNewResult> continuationFunction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
		{
			return Implementation.ContinueWith(Convert(continuationFunction), cancellationToken, continuationOptions, scheduler).ToInterface();
		}

		/// <summary>Creates a continuation that executes when the target <see cref="Task{TResult}" /> completes.</summary>
		/// <param name="continuationFunction">A function to run when the <see cref="Task{TResult}" /> completes. When run, the delegate will be passed the completed task and the caller-supplied state object as arguments.</param>
		/// <param name="state">An object representing data to be used by the continuation function.</param>
		/// <typeparam name="TNewResult">The type of the result produced by the continuation.</typeparam>
		/// <returns>A new continuation <see cref="Task{TResult}" />.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationFunction" /> argument is null.</exception>
		public ITask<TNewResult> ContinueWith<TNewResult>(Func<ITask<TResult>, object, TNewResult> continuationFunction, object state)
		{
			return Implementation.ContinueWith(Convert(continuationFunction), state).ToInterface();
		}

		/// <summary>Creates a continuation that executes when the target <see cref="Task{TResult}" /> completes.</summary>
		/// <param name="continuationFunction">A function to run when the <see cref="Task{TResult}" /> completes. When run, the delegate will be passed the completed task and the caller-supplied state object as arguments.</param>
		/// <param name="state">An object representing data to be used by the continuation function.</param>
		/// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> that will be assigned to the new task.</param>
		/// <typeparam name="TNewResult">The type of the result produced by the continuation.</typeparam>
		/// <returns>A new continuation <see cref="Task{TResult}" />.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationFunction" /> argument is null.</exception>
		/// <exception cref="ObjectDisposedException">The provided <see cref="T:System.Threading.CancellationToken" /> has already been disposed.</exception>
		public ITask<TNewResult> ContinueWith<TNewResult>(Func<ITask<TResult>, object, TNewResult> continuationFunction, object state, CancellationToken cancellationToken)
		{
			return Implementation.ContinueWith(Convert(continuationFunction), state, cancellationToken).ToInterface();
		}

		/// <summary>Creates a continuation that executes when the target <see cref="Task{TResult}" /> completes.</summary>
		/// <param name="continuationFunction">A function to run when the <see cref="Task{TResult}" /> completes. When run, the delegate will be passed the completed task and the caller-supplied state object as arguments.</param>
		/// <param name="state">An object representing data to be used by the continuation function.</param>
		/// <param name="scheduler">The <see cref="TaskScheduler" /> to associate with the continuation task and to use for its execution.</param>
		/// <typeparam name="TNewResult">The type of the result produced by the continuation.</typeparam>
		/// <returns>A new continuation <see cref="Task{TResult}" />.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationFunction" /> argument is null.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="scheduler" /> argument is null.</exception>
		public ITask<TNewResult> ContinueWith<TNewResult>(Func<ITask<TResult>, object, TNewResult> continuationFunction, object state, TaskScheduler scheduler)
		{
			return Implementation.ContinueWith(Convert(continuationFunction), state, scheduler).ToInterface();
		}

		/// <summary>Creates a continuation that executes when the target <see cref="Task{TResult}" /> completes.</summary>
		/// <param name="continuationFunction">A function to run when the <see cref="Task{TResult}" /> completes. When run, the delegate will be  passed the completed task and the caller-supplied state object as arguments.</param>
		/// <param name="state">An object representing data to be used by the continuation function.</param>
		/// <param name="continuationOptions">Options for when the continuation is scheduled and how it behaves. This includes criteria, such as <see cref="F:System.Threading.Tasks.TaskContinuationOptions.OnlyOnCanceled" />, as well as execution options, such as <see cref="F:System.Threading.Tasks.TaskContinuationOptions.ExecuteSynchronously" />.</param>
		/// <typeparam name="TNewResult">The type of the result produced by the continuation.</typeparam>
		/// <returns>A new continuation <see cref="Task{TResult}" />.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationFunction" /> argument is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="continuationOptions" /> argument specifies an invalid value for <see cref="TaskContinuationOptions" />.</exception>
		public ITask<TNewResult> ContinueWith<TNewResult>(Func<ITask<TResult>, object, TNewResult> continuationFunction, object state, TaskContinuationOptions continuationOptions)
		{
			return Implementation.ContinueWith(Convert(continuationFunction), state, continuationOptions).ToInterface();
		}

		/// <summary>Creates a continuation that executes when the target <see cref="Task{TResult}" /> completes.</summary>
		/// <param name="continuationFunction">A function to run when the <see cref="Task{TResult}" /> completes. When run, the delegate will be  passed the completed task and the caller-supplied state object as arguments.</param>
		/// <param name="state">An object representing data to be used by the continuation function.</param>
		/// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> that will be assigned to the new task.</param>
		/// <param name="continuationOptions">Options for when the continuation is scheduled and how it behaves. This includes criteria, such as <see cref="F:System.Threading.Tasks.TaskContinuationOptions.OnlyOnCanceled" />, as well as execution options, such as <see cref="F:System.Threading.Tasks.TaskContinuationOptions.ExecuteSynchronously" />.</param>
		/// <param name="scheduler">The <see cref="TaskScheduler" /> to associate with the continuation task and to use for its execution.</param>
		/// <typeparam name="TNewResult">The type of the result produced by the continuation.</typeparam>
		/// <returns>A new continuation <see cref="Task{TResult}" />.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="continuationFunction" /> argument is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The  <paramref name="continuationOptions" />  argument specifies an invalid value for <see cref="TaskContinuationOptions" />.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="scheduler" /> argument is null.</exception>
		/// <exception cref="ObjectDisposedException">The provided <see cref="T:System.Threading.CancellationToken" /> has already been disposed.</exception>
		public ITask<TNewResult> ContinueWith<TNewResult>(Func<ITask<TResult>, object, TNewResult> continuationFunction, object state, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
		{
			return Implementation.ContinueWith(Convert(continuationFunction), state, cancellationToken, continuationOptions, scheduler).ToInterface();
		}

		[CanBeNull]
		private static Action<Task<TResult>> Convert([CanBeNull] Action<ITask<TResult>> continuationAction)
		{
			return continuationAction == null ? (Action<Task<TResult>>)null : t => continuationAction(t.ToInterface());
		}

		[CanBeNull]
		private static Action<Task<TResult>, object> Convert([CanBeNull] Action<ITask<TResult>, object> action)
		{
			return action == null ? (Action<Task<TResult>, object>)null : (t, s) => action(t.ToInterface(), s);
		}

		[CanBeNull]
		private static Func<Task<TResult>, TNewResult> Convert<TNewResult>([CanBeNull] Func<ITask<TResult>, TNewResult> func)
		{
			return func == null ? (Func<Task<TResult>, TNewResult>)null : t => func(t.ToInterface());
		}

		[CanBeNull]
		private static Func<Task<TResult>, object, TNewResult> Convert<TNewResult>([CanBeNull] Func<ITask<TResult>, object, TNewResult> func)
		{
			return func == null ? (Func<Task<TResult>, object, TNewResult>)null : (t, s) => func(t.ToInterface(), s);
		}
	}
}
