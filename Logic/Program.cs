using Logic;
using TaskStatus = System.Threading.Tasks.TaskStatus;

static class Program
{
    static void Main(string[] args)
    {
        var taskExecutor = new TaskExecutor();

        taskExecutor.ExecuteTaskAsync(null);
        Task.Run(() => taskExecutor.ExecuteTaskAsync("First"));
        Task.Run(() => taskExecutor.ExecuteTaskAsync("Second"));
        Thread.Sleep(100000);
    }
}
