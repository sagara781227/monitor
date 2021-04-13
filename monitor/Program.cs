using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using System.Linq;
using System.Threading.Tasks;

namespace monitor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string processName;
            TimeSpan timeAtWork;
            TimeSpan checkInterval;
            if (args.Length > 0)
                processName = args[0];
            else
                processName = "notepad";
            if (args.Length > 1)
                timeAtWork = TimeSpan.FromMinutes(int.Parse(args[1]));
            else
                timeAtWork = TimeSpan.FromMinutes(5);
            if (args.Length > 2)
                checkInterval = TimeSpan.FromMinutes(int.Parse(args[2]));
            else
                checkInterval = TimeSpan.FromMinutes(1);

            List<ProcessStartTime> procesesStartTimes = new List<ProcessStartTime>();

            while (true)
            {
                DateTime currentTime = DateTime.Now;
                Process[] processes = Process.GetProcessesByName(processName);
                
                foreach (Process process in processes)
                {
                    if (procesesStartTimes.Any(x => x.ProcessId == process.Id)) continue;

                    procesesStartTimes.Add(new ProcessStartTime()
                    {
                        ProcessId = process.Id,
                        StartTime = currentTime
                    });
                }

                for (int i = 0; i < procesesStartTimes.Count; i++)
                {
                    ProcessStartTime processStartTime = procesesStartTimes[i];
                    Process process = processes.FirstOrDefault(x => x.Id == processStartTime.ProcessId);

                    if (process is null)
                    {
                        procesesStartTimes.RemoveAt(i);
                        i--;
                    }
                    else if (currentTime - processStartTime.StartTime > timeAtWork)
                    {
                        process.Kill(entireProcessTree: true);
                        Console.WriteLine($"Process {processName} ({processStartTime.ProcessId}) was killed");
                    }
                }

                await Task.Delay(checkInterval);
            }
            
        }

        class ProcessStartTime
        {
            public int ProcessId { get; set; }
            public DateTime StartTime { get; set; }
        }
        
    }
}
