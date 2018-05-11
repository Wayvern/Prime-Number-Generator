using System;
using System.Threading.Tasks;
using static Prime_Number_Generator.NumberManager;

namespace Prime_Number_Generator
{
    class TaskHandler
    {
        public delegate void FinishedGenerationHandler(Object sender, EventArgs Event);
        public event FinishedGenerationHandler FinishedGeneration;

        Task MainTask;

        public void RunTask()
        {
            MainTask = new Task(GeneratePrimeNumbers);
            MainTask.Start();
        }

        //Logic to Find if Number is prime
        private void GeneratePrimeNumbers()
        {
            bool isPrime = true;
            while (CurrentPrimeNumberIndex < maxPrimeNumberIndex)
            {
                long NumberToCheck = GetNumberToCheck();
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
    }
}
