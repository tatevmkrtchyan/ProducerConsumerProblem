using System;
using System.Collections.Generic;
using System.Threading;

namespace ProducerConsumerProblem
{
    class ProducerConsumer
    {
        static Random Rand = new Random();
        static int CountOfProducers = 9;
        static int CountOfConsumer = 7;
        static Queue<int> Queue = new Queue<int>();
        Thread[] ProduceThreads = new Thread[CountOfProducers];
        Thread[] CustomThreads = new Thread[CountOfConsumer];
        int MaxCountOfItemsInQueue = 20;
        private static readonly object LockObject = new object();

        public ProducerConsumer()
        {
            for (int i = 0; i < ProduceThreads.Length; i++)
                ProduceThreads[i] = new Thread(Produce);

            for (int i = 0; i < CustomThreads.Length; i++)
                CustomThreads[i] = new Thread(Consume);
        }

        void Produce()
        {
            while (true)
            {
                lock (LockObject)
                {
                    int RandNum = RandomNumber();

                    Thread.Sleep(RandNum);

                    if (Queue.Count < MaxCountOfItemsInQueue)
                    {
                        Console.WriteLine($"Enqueue  {RandNum}");
                        Queue.Enqueue(RandNum);
                    }
                    else
                    {
                        Monitor.PulseAll(LockObject);
                        Monitor.Wait(LockObject);
                    }
                }
            }
        }

        void Consume()
        {
            while (true)
            {
                lock (LockObject)
                {
                    Thread.Sleep(RandomNumber());

                    if (Queue.Count > 0)
                    {
                        Console.WriteLine($"Dequeue                 {Queue.Peek()}");
                        Queue.Dequeue();
                    }
                    else
                    {
                        Monitor.PulseAll(LockObject);
                        Monitor.Wait(LockObject);
                    }
                }
            }
        }

        public void StartThreads()
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(myHandler);

            for (int i = 0; i < CountOfProducers; i++)
                ProduceThreads[i].Start();

            for (int i = 0; i < CountOfConsumer; i++)
                CustomThreads[i].Start();
        }

        static int RandomNumber()
        {
            return Rand.Next(0, 100);
        }

        private static void myHandler(object sender, ConsoleCancelEventArgs e)
        {
            while (Queue.Count > 0)
            {
                Console.WriteLine($"DequeueFromHandler                {Queue.Peek()}");
                Queue.Dequeue();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ProducerConsumer PC = new ProducerConsumer();

            PC.StartThreads();
        }
    }
}