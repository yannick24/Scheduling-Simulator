using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchedulingSimulator
{
    class Job
    {
        public string jobName;
        public int arrivalTime;
        public int executionTime;
        public int waitTime;
        public int turnaroundTime;
        public int processedTime;


        /// <summary>
        /// Constructor for Job class
        /// </summary>
        /// <param name="name">name of the job</param>
        /// <param name="arrival">arrival of the job</param>
        /// <param name="duration">duration of the job</param>
        public Job(string name, int arrival, int duration)
        {
            this.jobName = name;
            this.arrivalTime = arrival;
            this.executionTime = duration;
            this.waitTime = 0;
            this.turnaroundTime = 0;
            this.processedTime = 0;
        }

        /// <summary>
        /// Constructor for Job class
        /// </summary>
        public Job()
        {
            this.jobName = string.Empty;
            this.arrivalTime = 0;
            this.executionTime = 0;
            this.waitTime = 0;
            this.turnaroundTime = 0;
            this.processedTime = 0;
        }
    }
}
