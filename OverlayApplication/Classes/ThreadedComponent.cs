using System;
using System.Threading;

namespace OverlayApplication
{
    public abstract class ThreadedComponent : IDisposable
    {
        protected virtual string ThreadName => nameof(ThreadedComponent);

        protected virtual int ThreadTimeout { get; set; } = 3000;

        protected virtual int ThreadFrameSleep { get; set; } = 500;

        private Thread Thread { get; set; }

        protected ThreadedComponent()
        {
            Thread = new Thread(ThreadStart)
            {
                Name = ThreadName
            };
        }

        public virtual void Dispose()
        {
            Thread.Interrupt();

            if (!Thread.Join(ThreadTimeout))
            {
                Thread.Abort();
            }

            Thread = default;
        }

        public void Start()
        {
            Thread.Start();
        }

        private void ThreadStart()
        {
            try
            {
                while (true)
                {
                    FrameAction();
                    Thread.Sleep(ThreadFrameSleep);
                }
            }
            catch (ThreadInterruptedException)
            {

            }
        }

        protected abstract void FrameAction();
    }
}
