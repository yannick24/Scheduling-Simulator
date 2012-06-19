using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SchedulingSimulator
{
    class Scheduling
    {
        private string fileName;

        /// <summary>
        /// Construtor for Scheduling class
        /// </summary>
        /// <param name="name">name of the filename of the text file</param>
        public Scheduling(string name)
        {
            this.fileName = name;
        }

        /// <summary>
        /// To Execute First-Come First-Serve algorithm
        /// </summary>
        /// <returns>return job processed stored in a queue</returns>
        public Queue<Job> ExecuteFCFS()
        {
            Queue<Job> jobQueue = new Queue<Job>();
            Queue<Job> jobDoneQueue = new Queue<Job>();
            jobQueue = GetData(fileName);

            int jobExecution = 0;
            int time = 0;

            while (jobQueue.Count() != 0)
            {
                Job job = jobQueue.Dequeue();
                jobExecution = job.executionTime;
                time += jobExecution;
                job.processedTime = jobExecution;

                job.turnaroundTime += jobExecution;
                job.executionTime -= jobExecution;

                jobDoneQueue.Enqueue(job);

                foreach (Job jobElement in jobQueue)
                {
                    if (jobElement.arrivalTime <= time)
                    {
                        jobElement.waitTime = time - jobElement.arrivalTime;
                        jobElement.turnaroundTime = time - jobElement.arrivalTime;
                    }
                }
            }

            PrintResult(jobDoneQueue, "FCFS");

            return jobDoneQueue;
        }

        /// <summary>
        /// To execute Round-Robin algorithm
        /// </summary>
        /// <returns>return job processed stored in a queue</returns>
        public Queue<Job> ExecuteRoundRobin()   // q = 4 (quanta)
        {
            Queue<Job> jobQueue = new Queue<Job>();
            jobQueue = GetData(fileName);

            int quanta = 4;
            int jobExecution = 0;
            int time = 0;

            do
            {
                foreach (Job jobElement in jobQueue)
                {
                    if (jobElement.executionTime != 0 && jobElement.arrivalTime <= time)
                    {
                        if (jobElement.executionTime >= quanta)
                        {
                            time += quanta;
                            jobElement.turnaroundTime += quanta;
                            jobElement.executionTime -= quanta;
                            jobElement.processedTime += quanta;

                            foreach (Job otherJob in jobQueue)
                            {
                                if (otherJob.jobName == jobElement.jobName)
                                    continue;

                                if (otherJob.arrivalTime <= time && otherJob.executionTime != 0)
                                {
                                    otherJob.waitTime = time - (otherJob.arrivalTime + otherJob.processedTime);
                                    otherJob.turnaroundTime = time - otherJob.arrivalTime;
                                }
                            }
                        }
                        else
                        {
                            jobExecution = jobElement.executionTime;
                            time += jobExecution;

                            jobElement.turnaroundTime += jobExecution;
                            jobElement.executionTime -= jobExecution;
                            jobElement.processedTime += jobExecution;

                            foreach (Job otherJob in jobQueue)
                            {
                                if (otherJob.jobName == jobElement.jobName)
                                    continue;

                                if (otherJob.arrivalTime <= time && otherJob.executionTime != 0)
                                {
                                    otherJob.waitTime = time - (otherJob.arrivalTime + otherJob.processedTime);
                                    otherJob.turnaroundTime = time - otherJob.arrivalTime;
                                }
                            }

                        }

                    } // end of job that still needs to be processed
                    
                } // end of outer foreach statement

            } while (!Done(jobQueue)) ;

            PrintResult(jobQueue, "RoundRobin");

            return jobQueue;
        }


        /// <summary>
        /// To execute Shortest Process Next algorithm
        /// </summary>
        /// <returns>return job processed stored in a queue</returns>
        public Queue<Job> ExecuteSPN()
        {
            Queue<Job> jobQueue = new Queue<Job>();
            Queue<Job> jobDoneQueue = new Queue<Job>();
            jobQueue = GetData(fileName);

            int jobExecution = 0;
            int time = 0;

            do
            {
                Job job = NextShortestProcess(jobQueue, time);
                jobExecution = job.executionTime;
                time += jobExecution;

                job.turnaroundTime += jobExecution;
                job.executionTime -= jobExecution;
                job.processedTime = jobExecution;

                jobDoneQueue.Enqueue(job);

                foreach (Job otherJob in jobQueue)
                {
                    if (otherJob.jobName == job.jobName)
                        continue;

                    if (otherJob.arrivalTime <= time && otherJob.executionTime != 0)
                    {
                        otherJob.waitTime = time - (otherJob.arrivalTime + otherJob.processedTime);
                        otherJob.turnaroundTime = time - otherJob.arrivalTime;
                    }
                }

            } while (!Done(jobQueue));

            PrintResult(jobDoneQueue, "SPN");

            return jobDoneQueue;
        }


        /// <summary>
        /// To execute Shortest Remaining Time
        /// </summary>
        /// <returns>return job processed stored in a queue</returns>
        public Queue<Job> ExecuteSRN()
        {
            Queue<Job> jobQueue = new Queue<Job>();
            jobQueue = GetData(fileName);

            int time = 0;

            do
            {
                Job job = NextShortestProcess(jobQueue, time);
                time += 1;

                job.turnaroundTime += 1;
                job.executionTime -= 1;
                job.processedTime += 1;


                foreach (Job otherJob in jobQueue)
                {
                    if (otherJob.jobName == job.jobName)
                        continue;

                    if (otherJob.arrivalTime <= time && otherJob.executionTime != 0)
                    {
                        otherJob.waitTime = time - (otherJob.arrivalTime + otherJob.processedTime);
                        otherJob.turnaroundTime = time - otherJob.arrivalTime;
                    }
                }

            } while (!Done(jobQueue));

            PrintResult(jobQueue, "SRN");

            return jobQueue;
        }
        

        /// <summary>
        /// To get next process process that shoulb be processed
        /// based on length of execution, shortest first
        /// </summary>
        /// <param name="jobQueue">job queue (jobs that need to be processed)</param>
        /// <param name="time">time used by dispatcher</param>
        /// <returns>return shortest process</returns>
        private Job NextShortestProcess(Queue<Job> jobQueue, int time)
        {
            Job job = new Job();
            int shortest = int.MaxValue;

            foreach (Job jobElt in jobQueue)
            {
                if (jobElt.arrivalTime <= time)
                {
                    if (jobElt.executionTime < shortest && jobElt.executionTime != 0)
                    {
                        job = jobElt;
                        shortest = jobElt.executionTime;
                    }
                }
            }

            return job;
        }


        /// <summary>
        /// Determine if there is no job left to be processed
        /// </summary>
        /// <param name="jobQueue">job queue</param>
        /// <returns>true if there is no job waiting anymore, false if some jobs are still waiting to be processed</returns>
        private bool Done(Queue<Job> jobQueue)
        {
            foreach (Job jobElement in jobQueue)
            {
                if (jobElement.executionTime != 0)
                    return false;
            }

            return true;
        }


        /// <summary>
        /// To get data from text file and populate job queue with those data
        /// </summary>
        /// <param name="filename">file name of text file</param>
        /// <returns>job queue populated by jobs from text file</returns>
        private Queue<Job> GetData(string filename)
        {
            Queue<Job> jobQueue = new Queue<Job>();

            if (File.Exists(filename))
            {
                FileStream inFile = new FileStream(filename, FileMode.Open, FileAccess.Read);
                StreamReader reader = new StreamReader(inFile);

                string line = reader.ReadLine();
                string[] tag = null;
                int counter = 1;
                char[] delim = { '!', '\t', '@', ' '};

                while (line != null)
                {
                    if (line == "")
                    {
                        line = reader.ReadLine();
                        continue;
                    }

                    tag = line.Split(delim, StringSplitOptions.RemoveEmptyEntries);

                    if (tag[0].StartsWith("#") || tag.Count() == 0)
                    {
                        line = reader.ReadLine();
                        continue;
                    }

                    Job job = new Job("Job " + counter, int.Parse(tag[0]), int.Parse(tag[1]));
                    jobQueue.Enqueue(job);

                    line = reader.ReadLine();
                    counter++;
                }

                reader.Close();
                inFile.Close();
            }
            else
                Console.WriteLine("File is not found...");

            return jobQueue;
        }


        /// <summary>
        /// To compute average wait time of all jobs
        /// </summary>
        /// <param name="jobQueue">job queue</param>
        /// <returns>the average wait time</returns>
        private double ComputeAVGWait(Queue<Job> jobQueue)
        {
            int[] waitArray = GetWaitTimeArray(jobQueue);
            return Math.Round((double) waitArray.Sum() / waitArray.Count(), 2);
        }


        /// <summary>
        /// To compute average turnaround time of all jobs
        /// </summary>
        /// <param name="jobQueue">job queue</param>
        /// <returns>the average turnaround time</returns>
        private double ComputeAVGTurn(Queue<Job> jobQueue)
        {
            int[] turnArray = GetTurnTimeArray(jobQueue);
            return Math.Round((double) turnArray.Sum() / turnArray.Count(), 2);
        }


        /// <summary>
        /// To compute standard deviation of wait times of all jobs
        /// </summary>
        /// <param name="jobQueue">job queue</param>
        /// <returns>the standard deviation of wait times</returns>
        private double WaitSTD(Queue<Job> jobQueue)
        {
            int[] waitArray = GetWaitTimeArray(jobQueue);
            double mean = ComputeAVGWait(jobQueue);
            double[] deviation = new double[waitArray.Count()];
            for (int i = 0; i < deviation.Count(); i++)
            {
                deviation[i] = waitArray[i] - mean;
                deviation[i] = deviation[i] * deviation[i];
            }

            double sumOfDeviation = deviation.Sum();
            double std = sumOfDeviation / (deviation.Count() - 1);
            std = Math.Sqrt(std);

            return Math.Round(std, 2);
        }


        /// <summary>
        /// To compute standard deviation of turnaround times of all jobs
        /// </summary>
        /// <param name="jobQueue">job queue</param>
        /// <returns>the standard deviation of turnaround times</returns>
        private double TurnSTD(Queue<Job> jobQueue)
        {
            int[] turnArray = GetTurnTimeArray(jobQueue);
            double mean = ComputeAVGTurn(jobQueue);
            double[] deviation = new double[turnArray.Count()];
            for (int i = 0; i < deviation.Count(); i++)
            {
                deviation[i] = turnArray[i] - mean;
                deviation[i] = deviation[i] * deviation[i];
            }

            double sumOfDeviation = deviation.Sum();
            double std = sumOfDeviation / (deviation.Count() - 1);
            std = Math.Sqrt(std);

            return Math.Round(std, 2);
        }


        /// <summary>
        /// To Get Wait time of all jobs stored in an array
        /// </summary>
        /// <param name="jobQueue">job queue</param>
        /// <returns>array of wait time of all jobs</returns>
        private int[] GetWaitTimeArray(Queue<Job> jobQueue)
        {
            int[] waitArray = new int[jobQueue.Count];
            int i = 0;
            foreach (Job job in jobQueue)
            {
                waitArray[i] = job.waitTime;
                i++;
            }
            
            return waitArray;
        }
        


        /// <summary>
        /// To Get turnaround time of all jobs stored in an array
        /// </summary>
        /// <param name="jobQueue">job queue</param>
        /// <returns>array of turnaround time of all jobs</returns>
        private int[] GetTurnTimeArray(Queue<Job> jobQueue)
        {
            int[] turnArray = new int[jobQueue.Count];
            int i = 0;
            foreach (Job job in jobQueue)
            {
                turnArray[i] = job.turnaroundTime;
                i++;
            }

            return turnArray;
        }


        /// <summary>
        /// To print Scheduling simulation on the console and write it on a text file
        /// </summary>
        /// <param name="processedQueue">processed job queue</param>
        /// <param name="algorithm">algorithm to be used</param>
        public void PrintResult(Queue<Job> processedQueue, string algorithm)
        {
            FileStream outFile = new FileStream(algorithm + ".txt.", FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(outFile);

            writer.WriteLine("Process-Scheduling Simulator using " + algorithm + ":");
            writer.WriteLine();

            Console.WriteLine("Process-Scheduling Simulator using " + algorithm + ":");
            Console.WriteLine();

            writer.WriteLine("{0, -15} {1, -5} {2, 20}", "Job name", "Wait Time", "Turnaround Time");
            Console.WriteLine("{0, -15} {1, -5} {2, 20}", "Job name", "Wait Time", "Turnaround Time");

            foreach (Job job in processedQueue)
            {
                writer.WriteLine("{0, -15} {1, -5} ms  {2, 10} ms", job.jobName, job.waitTime, job.turnaroundTime);
                Console.WriteLine("{0, -15} {1, -5} ms  {2, 10} ms", job.jobName, job.waitTime, job.turnaroundTime);
            }

            writer.WriteLine();
            writer.WriteLine("Average Wait Time: " + ComputeAVGWait(processedQueue));
            writer.WriteLine("Average Turnaround Time: " + ComputeAVGTurn(processedQueue));

            Console.WriteLine();
            Console.WriteLine("Average Wait Time: " + ComputeAVGWait(processedQueue));
            Console.WriteLine("Average Turnaround Time: " + ComputeAVGTurn(processedQueue));

            writer.WriteLine();
            writer.WriteLine("Standard deviation Wait Time: " + WaitSTD(processedQueue));
            writer.WriteLine("Standard deviation Turnaround Time: " + TurnSTD(processedQueue));

            Console.WriteLine();
            Console.WriteLine("Standard deviation Wait Time: " + WaitSTD(processedQueue));
            Console.WriteLine("Standard deviation Turnaround Time: " + TurnSTD(processedQueue));

            writer.WriteLine();
            writer.WriteLine();

            Console.WriteLine();
            Console.WriteLine();


            writer.Close();
            outFile.Close();
        }
    }
}
