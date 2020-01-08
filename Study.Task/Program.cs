using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Study
{
    class Program
    {
        //Thread occupa 1MB in memoria
        //Crearlo e distruggerlo è un casino, molto meglio usare un thread pool
        //Il TAP funziona tramite un thread pool gestito dal sistema (Task-based asynchronous pattern)
        static async Task Main(string[] args)
        {
            //Queue the specified work to run on the thread pool and returns a Task object that represent that work
            List<Task> initialTasks = new List<Task>();
            initialTasks.Add(Task.Run(async () => { await Task.Delay(100); Console.WriteLine("threaded async"); }));
            initialTasks.Add(Task.Run(() => { Console.WriteLine("threaded"); }));
            await Task.WhenAll(initialTasks);

            initialTasks = new List<Task>();
            initialTasks.Add(Task.Run(async () => { await Task.Delay(0); Console.WriteLine("threaded async"); }));
            initialTasks.Add(Task.Run(() => { Console.WriteLine("threaded"); }));
            await Task.WhenAll(initialTasks);

            IHello hello = new Hello();
            int x = await hello.Make();
            Console.WriteLine(x);
            Task<int> tasker = hello.Make();
            Task<int> tasker2 = hello.Make();
            Task<int> tasker3 = hello.Make();
            List<Task<int>> tasks = new List<Task<int>>();
            tasks.Add(tasker);
            tasks.Add(tasker2);
            tasks.Add(tasker3);
            //cjkasdjsalkjdsjalkdlksa
            int[] results = await Task.WhenAll(tasks);
            Task<int> results2 = await Task.WhenAny(tasks);
            Console.WriteLine(string.Join(',', results));
            Console.WriteLine(string.Join(',', results2));

            Sample obj = new Sample();
            for (int i = 0; i < 100; i++)
            {
                obj.Produce(new Jar(i));
            }
            Console.WriteLine("Thread {0}",
            Thread.CurrentThread.GetHashCode()); //{0}
            while (obj.QueueLength != 0)
            {
                Thread.Sleep(1000);
            }
            Console.Read();
        }
    }
    public interface IHello
    {
        Task<int> Make();
    }

    public class Hello : IHello
    {
        //public Task<int> Make()
        //{
        //    Task<int> x = new Task<int>(() => 0);
        //    Console.WriteLine("Wrong Way");
        //    return x;
        //}

        public async Task<int> Make()
        {
            await Task.Delay(0);
            Console.WriteLine("Right Way");
            return 0;
        }
    }
    public class Jar
    {
        public int Id;
        public Jar(int id)
        {
            Id = id;
        }
    }
    class Sample
    {
        public int QueueLength { get; private set; } = 0;
        public void Produce(Jar jar)
        {
            ThreadPool.QueueUserWorkItem(
                new WaitCallback(Consume), jar);
            QueueLength++;
        }
        public void Consume(object obj)
        {
            Console.WriteLine("Thread {0} consumes {1}",
                Thread.CurrentThread.GetHashCode(), //{0}
                ((Jar)obj).Id); //{1}
            Thread.Sleep(100);
            QueueLength--;
        }
    }
}
