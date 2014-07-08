using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;

namespace BriefFiniteElementNet.BenchmarkApplication
{
    public class MemoryMonitor
    {
        PerformanceCounter perfCounter=new PerformanceCounter();
        public MemoryMonitor(double refreshInterVal)
        {
            this.timer = new Timer(refreshInterVal);
            timer.Elapsed+=TimerOnElapsed;
            timer.Start();

        }

        public double MaxMemoryUsage;

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            var currentProcess = Process.GetCurrentProcess();

            var size = 0.0;

            try
            {
                size= currentProcess.PrivateMemorySize64 / 1048576.0;
            }
            catch (Exception)
            {
                size = -1;
            }

            if (size > MaxMemoryUsage)
                MaxMemoryUsage = size;
        }

        public void Reset()
        {
            MaxMemoryUsage = 0;
        }

        private Timer timer;
    }
}
