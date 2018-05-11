using System;
using System.Collections.Generic;
using System.Linq;

namespace Prime_Number_Generator
{
   public static class NumberManager
    {
        public static long CurrentPrimeNumberIndex = 1, CurrentNumber = 3, maxPrimeNumberIndex;

        public static List<long> NumbersToCheck = new List<long>(), ConfirmedPrimeNumbers = new List<long>();

        static object _lock = new object();

        public static void InitializePrimeNumberList()
        {
            for(int i = 0;i<6;i++)
            {
                NumbersToCheck.Add(CurrentNumber);
                CurrentNumber++;
            }
        }

        //Provides Numbers to Workers/Tasks
        public static long GetNumberToCheck()
        {
            lock (_lock)
            {
                NumbersToCheck.Add(CurrentNumber);
                CurrentNumber++;
                long numberToReturn = NumbersToCheck[0];
                NumbersToCheck.RemoveAt(0);
                return numberToReturn;
            }
        }

        public static void AddConfirmedPrimeNumber(long num)
        {
            lock(_lock)
            {
                CurrentPrimeNumberIndex++;
                ConfirmedPrimeNumbers.Add(num);
            }
        }
        public static List<long> GetConfirmedPrimeNumbers()
        {
            lock(_lock)
            {
                ConfirmedPrimeNumbers.Sort();
                List<long> newList = ConfirmedPrimeNumbers.ToList();
                ConfirmedPrimeNumbers.Clear();
                return newList;
            }
        }

        public static void ClearNumbers()
        {
            CurrentPrimeNumberIndex = 1;
            CurrentNumber = 3;

            NumbersToCheck.Clear();
            ConfirmedPrimeNumbers.Clear();
            InitializePrimeNumberList();
        }

        //Ensures that benchmarks start on the same number
        public static void PrepBenchmark()
        {
            NumbersToCheck.Clear();
            CurrentNumber = 224742;
            for (int i = 0; i<10;i++)
            {
                NumbersToCheck.Add(224743+i);
            }
            CurrentPrimeNumberIndex = 20000;
        }

    }
}
