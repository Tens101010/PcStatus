//Please note: This application is purely for my own education, to run through coding 
//examples by following tutorials, and to just tinker around with coding.  
//I know it’s bad practice to heavily comment code (code smell), but comments in all of my 
//exercises will largely be left intact as this serves me 2 purposes:
//    I want to retain what my original thought process was at the time
//    I want to be able to look back in 1..5..10 years to see how far I’ve come
//    And I enjoy commenting on things, however redundant this may be . . . 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
// Added a reference to the project by RC on "Reference" in Solution Explorer
// Added the reference for speech recognition
using System.Speech.Synthesis;

namespace Jarvis
{
    class Program
    {
        // static used here to be able to use synth everywhere without being intitialized
        private static SpeechSynthesizer synth = new SpeechSynthesizer();

        // Entry point
        static void Main(string[] args)
        {
            // List of messages that will selected at random when the CPU is hammered
            List<string> cpuMaxedOutMessages = new List<string>();
            cpuMaxedOutMessages.Add("WARNING!!! Holy $#!7!! The CPU is about to catch FIRE!!!");
            cpuMaxedOutMessages.Add("WARNING!!! Hardware is about to fry!");
            cpuMaxedOutMessages.Add("WARNING!!! Overload!");
            cpuMaxedOutMessages.Add("WARNING!!! Muy Caliente");
            cpuMaxedOutMessages.Add("Nice knowing you");

            // The dice!
            Random rand = new Random();

            // This will greet the user in the specified voice
            synth.SelectVoiceByHints(VoiceGender.Male);
            synth.Speak("Initialized");

            #region My Performance Counters
            // This will pull the current CPU load in percentage
            PerformanceCounter perfCpuCount = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
            // NextValue returns a counter sample and returns the calculated value for it
            perfCpuCount.NextValue();

            // This will pull the currnt available memory in Megabytes
            PerformanceCounter perfMemCount = new PerformanceCounter("Memory", "Available MBytes");
            perfMemCount.NextValue();

            // this will return system uptime in seconds
            PerformanceCounter perfUptimeCount = new PerformanceCounter("System", "System Up Time");
            perfUptimeCount.NextValue();
            #endregion

            // Get a value for system uptime and have the system speak it
            TimeSpan upTimeSpan = TimeSpan.FromSeconds(perfUptimeCount.NextValue());
            string systemUptimeMessage = string.Format("The current system up time is {0} days {1} hours {2} minutes {3} seconds",
                (int)upTimeSpan.TotalDays,
                (int)upTimeSpan.Hours,
                (int)upTimeSpan.Minutes,
                (int)upTimeSpan.Seconds);
            TenSpeak(systemUptimeMessage, VoiceGender.Male, 2);

            int speechSpeed = 1;

            bool isChromeOpenAlready = false;

            // Infinite While loop
            #region While Loop Logic
            while (true)
            {
                // Get the current performance counter values
                int currentCpuPercentage = (int)perfCpuCount.NextValue();
                int currentAvailableMemory = (int)perfMemCount.NextValue();
                int currentSystemUptime = (int)perfUptimeCount.NextValue();

                // will print the CPU % load every 1 second
                Console.WriteLine("CPU load        : {0}%", currentCpuPercentage);
                Console.WriteLine("Available Memory: {0} MB", currentAvailableMemory);
                Console.WriteLine("System Uptime   : {0} seconds", currentSystemUptime);

                // Only tell us vocally if CPU drops below 80% or warn us when at 100%
                if (currentCpuPercentage > 80)
                {
                    if (currentCpuPercentage == 100)
                    {
                        // Dont let speech get to fast.  Will go from 1 to 5 max
                        if (speechSpeed < 5)
                        {
                            speechSpeed++;
                        }
                        // Speech will tell the user these values
                        string cpuLoadVocalMessage = cpuMaxedOutMessages[rand.Next(5)];
                        // Initializing the speech output
                        TenSpeak(cpuLoadVocalMessage, VoiceGender.Male, speechSpeed);

                        if (isChromeOpenAlready == false)
                        {
                            OpenWebSite("http://www.nfl.com");
                            isChromeOpenAlready = true;
                        }
                    }
                    else
                    {
                        string cpuLoadVocalMessage = String.Format("The current CPU load is {0} percent", currentCpuPercentage);
                        TenSpeak(cpuLoadVocalMessage, VoiceGender.Male, 2);
                    }
                }

                // Only tell us if we have less than 1GB of memory
                if (currentAvailableMemory < 1024)
                {
                    string memAvailableVocalMessage = String.Format("You currently have {0} megabytes of memory available", currentAvailableMemory);
                    TenSpeak(memAvailableVocalMessage, VoiceGender.Male, 3);
                }

                Thread.Sleep(1000);
            } // End of loop
            #endregion
        }

        // Speaks with a selected voice
        public static void TenSpeak(string message, VoiceGender voiceGender)
        {
            synth.SelectVoiceByHints(voiceGender);
            synth.Speak(message);
        }

        // speaks with the selected voice at a declared speed
        public static void TenSpeak(string message, VoiceGender voiceGender, int rate)
        {
            synth.Rate = rate;
            TenSpeak(message, voiceGender);
        }

        // Opens a program
        public static void OpenWebSite(string URL)
        {
            Process p1 = new Process();
            p1.StartInfo.FileName = "chrome.exe";
            p1.StartInfo.Arguments = URL;
            p1.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            p1.Start();
        }

    }
}
