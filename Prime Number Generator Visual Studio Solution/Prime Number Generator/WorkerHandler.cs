using System;
using System.ComponentModel;
using static Prime_Number_Generator.NumberManager;

namespace Prime_Number_Generator
{
    class WorkerHandler
    {
        public delegate void FinishedGenerationHandler(Object sender, EventArgs Event);
        public event FinishedGenerationHandler FinishedGeneration;

        #region Worker Setup and Start Logic

        private readonly BackgroundWorker worker = new BackgroundWorker
        {
            WorkerReportsProgress = true
        };

        public WorkerHandler()
        {
            worker.DoWork += GeneratePrimeNumbers;
        }

        public void StartWorker()
        {
            worker.RunWorkerAsync();
        }

        #endregion

        #region WorkerLogic

        private void GeneratePrimeNumbers(object sender, DoWorkEventArgs e)
        {
            bool isPrime = true;
            while (CurrentPrimeNumberIndex < maxPrimeNumberIndex) 
            {
                long NumberToCheck = GetNumberToCheck();  //Gets a number from the List of numbers to check in Number Manager
                if (NumberToCheck != 4)
                {
                    for (double i = 2; i < NumberToCheck / 2; i++)//Logic to check if number is prime
                    {
                        double val1 = NumberToCheck / i;
                        long val2 = (long)val1;
                        if (val1 == val2)
                        {
                            isPrime = false;
                            break;
                        }
                    }
                    if (isPrime)
                    {
                        AddConfirmedPrimeNumber(NumberToCheck);
                    }
                }
                isPrime = true;
                NumberToCheck++;
                
            }
            OnGenerationFinish();
        }
                
        protected virtual void OnGenerationFinish()
        {
            if(FinishedGeneration!=null)
            {
                FinishedGeneration(this, EventArgs.Empty); //Calls to StopGenerating to Reactivate UI elements
            }
        }

        #endregion
    }
}
