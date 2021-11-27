﻿using UnityEngine;

namespace EntityLogic.AI
{
    public static class AILogs
    {
        private static string MainLog { get; set; }
        private static string SecondaryLog { get; set;  }

        public static void AddMainLogEndl(string log)
        {
            MainLog += $"{log}\n";
        }

        public static void AddMainLog(string log)
        {
            MainLog += $"{log} ";

        }

        public static void AddSecondaryLogEndl(string log)
        {
            SecondaryLog += $"{log}\n";
        }
        
        public static void AddSecondaryLog(string log)
        {
            SecondaryLog += $"{log} ";
        }

        public static void Log()
        {
            Debug.Log(MainLog + SecondaryLog);
            MainLog = "";
            SecondaryLog = "";
        }
    }
}