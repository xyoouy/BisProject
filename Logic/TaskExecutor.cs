namespace Logic;

public class TaskExecutor
{
    private readonly object lockObject = new();
    private Task<int> executionTask;
        
    private TaskCompletionSource<int> tcs;
        
    public async Task<int> ExecuteTaskAsync(object data)
    {
        lock (lockObject)
        {
            if (executionTask is {Status: TaskStatus.Running})
            {
                throw new InvalidOperationException("Task is already running");
            }
            Console.WriteLine(Thread.GetCurrentProcessorId());

            executionTask = Task.Run(async () =>
            {
                await Task.Delay(5000);
                if (data == null)
                    throw new ArgumentNullException(nameof(data), "Data cannot be null");
                    
                return data.GetHashCode();
            });
               
            tcs = new TaskCompletionSource<int>();
                
            executionTask.ContinueWith(task =>
            {
                if (task.Exception != null)
                    tcs.SetException(task.Exception.InnerExceptions);
                    
            }, TaskContinuationOptions.OnlyOnFaulted);
        }
            
        try
        {
            var result = await executionTask;
            tcs.SetResult(result);
        }
        catch (Exception ex)
        {
            tcs.SetException(ex);
        }
            
        return await tcs.Task;
    }

    public TaskStatus GetTaskStatus() => executionTask.Status;

    public int GetTaskResult()
    {
        if (executionTask.Status != TaskStatus.RanToCompletion)
            throw new InvalidOperationException("Task is not yet completed");
            
        return executionTask.Result;
    }
}