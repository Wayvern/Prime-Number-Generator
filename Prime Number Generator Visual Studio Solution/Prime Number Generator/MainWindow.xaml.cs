using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Windows.Threading;
using static Prime_Number_Generator.NumberManager;

namespace Prime_Number_Generator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variables

        List<WorkerHandler> workerHandlers = new List<WorkerHandler>();
        List<TaskHandler> taskHandlers = new List<TaskHandler>();

        bool  Running = false, WorkerBasedMultithreading = true;
     
        public float ProgressPercentage = 0f, progressIncreaseRatio; //Progressbar Updates
        private short NumberofThreads = 4;
        public long IndexForPrintingFoundPrimeNumbers = 1;

        DispatcherTimer ScreenPrintInterval;
        Stopwatch TimeElapsed = new Stopwatch();

        #endregion

        public MainWindow()
        {
            AmountToGenerateSliderLabel = new TextBox();
            AmountToGenerateSlider = new Slider();
            NumberofThreadsTextBox = new TextBox();
            ScreenPrintInterval = new DispatcherTimer();
            ScreenPrintInterval.Interval = TimeSpan.FromSeconds(0.01);
            ScreenPrintInterval.Stop();
            
            ScreenPrintInterval.Tick += UpdateFoundPrimeNumberList;

            InitializePrimeNumberList(); //Adds six Numbers to List of Numbers to Check
            UpdateThreadCount();         //Fills TaskHandler and WorkerHandler Lists
            InitializeComponent();
        }

        #region Basic Functions

        void SetUIActive(bool active)
        {
            NumberofThreadsTextBox.IsEnabled = active;
            NumberofThreadsSlider.IsEnabled = active;
            AmountToGenerateSlider.IsEnabled = active;
            BenchmarkButton.IsEnabled = active;
            ClearButton.IsEnabled = active;
            SearchButton.IsEnabled = active;
            AmountToGenerateSliderLabel.IsEnabled = active;
            BackGroundWorkerRadioButton.IsEnabled = active;
            TaskRadioButton.IsEnabled = active;
        }

        //Starts Task Based MultiThreading
        private void StartTasks(long max)
        {
            Running = true;
            CurrentPrimeNumberIndex = IndexForPrintingFoundPrimeNumbers;
            progressIncreaseRatio = 100f / max;
            max += CurrentPrimeNumberIndex;
            maxPrimeNumberIndex = max;
            foreach (TaskHandler taskHandler in taskHandlers)
                taskHandler.RunTask();
        }

        //Starts Worker Based MultiThreading
        private void GeneratePrimeNumbersWithWorkers(long max)
        {
            Running = true;
            CurrentPrimeNumberIndex = IndexForPrintingFoundPrimeNumbers;
            progressIncreaseRatio = 100f / max;
            max += CurrentPrimeNumberIndex;
            maxPrimeNumberIndex = max;
            foreach (WorkerHandler workerHandler in workerHandlers)
                workerHandler.StartWorker();
        }

        //Checks if Input Text is a number
        private bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9]");
            return regex.IsMatch(text);
        }

        //Prints Found Prime numbesrs to Screen
        private void UpdateFoundPrimeNumberList()
        {
            this.Dispatcher.Invoke(() =>
            {
                if (ConfirmedPrimeNumbers.Count > 0)
                {
                    foreach (long CPN in GetConfirmedPrimeNumbers())
                    {
                        ProgressPercentage += progressIncreaseRatio;
                        PrimeNumberIndexTextbox.AppendText(IndexForPrintingFoundPrimeNumbers.ToString("0,0") + ". " + CPN.ToString("0,0") + "\n");
                        ProgressBar.Value = ProgressPercentage;
                        IndexForPrintingFoundPrimeNumbers++;
                    }

                }
            });
            PrimeNumberIndexTextbox.ScrollToEnd();
        }
        
        private void UpdateFoundPrimeNumberList(Object Obj, EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (ConfirmedPrimeNumbers.Count > 0)
                {
                    foreach (long CPN in GetConfirmedPrimeNumbers())
                    {
                        ProgressPercentage += progressIncreaseRatio;
                        PrimeNumberIndexTextbox.AppendText(IndexForPrintingFoundPrimeNumbers.ToString("0,0") + ". " + CPN.ToString("0,0") + "\n");
                        ProgressBar.Value = ProgressPercentage;
                        IndexForPrintingFoundPrimeNumbers++;
                    }
                }
            });
            PrimeNumberIndexTextbox.ScrollToEnd();
        }

        //Refills worker, and Task lists to have the correct number for newly target input thread count
        private void UpdateThreadCount()
        {
            if (!Running)
            {
                if (NumberofThreads != workerHandlers.Count)
                {
                    workerHandlers.Clear();
                    for (int i = 0; i < NumberofThreads; i++)
                    {
                        workerHandlers.Add(new WorkerHandler());
                        workerHandlers[i].FinishedGeneration += StopGenerating;
                    }
                    
                }
                if(NumberofThreads != taskHandlers.Count)
                {
                    taskHandlers.Clear();
                    for (int i = 0; i < NumberofThreads; i++)
                    {
                        taskHandlers.Add(new TaskHandler());
                        taskHandlers[i].FinishedGeneration += StopGenerating;
                    }
                }

            }
        }

        //Disables Select UI and Begins Generating Prime Numbers
        private void StartGenerating()
        {
            TimeElapsed.Reset();
            TimeElapsed.Start();
            ProgressPercentage = 0.0f;
            SetUIActive(false);            
            maxPrimeNumberIndex = CurrentPrimeNumberIndex;
            
            if (WorkerBasedMultithreading)
            {
                GeneratePrimeNumbersWithWorkers((int)AmountToGenerateSlider.Value);
            }
            else
            {
                StartTasks((long)AmountToGenerateSlider.Value);
            }
            
            StartButton.Content = "Stop";
            ScreenPrintInterval.Start();
        }

        //Prints Time elapsed and reenables UI
        private void StopGenerating()
        {
            SetUIActive(true);
            Running = false;
            TimeElapsed.Stop();
            TimeElapsedLabel.Content = "Time Elapsed: " + (int)TimeElapsed.Elapsed.Minutes + " Minutes " + (int)TimeElapsed.Elapsed.Seconds + " Seconds " + (int)TimeElapsed.Elapsed.Milliseconds + " Milliseconds";
            StartButton.Content = "Start";
            ScreenPrintInterval.Stop();
            UpdateFoundPrimeNumberList();
        }

        //Event Triggered Overload to StopGenerating
        private void StopGenerating(Object Obj, EventArgs Event)
        {
            this.Dispatcher.Invoke(() =>
            {
                SetUIActive(true);
                TimeElapsedLabel.Content = "Time Elapsed: " + (int)TimeElapsed.Elapsed.Minutes + " Minutes " + (int)TimeElapsed.Elapsed.Seconds + " Seconds " + (int)TimeElapsed.Elapsed.Milliseconds + " Milliseconds";
                StartButton.Content = "Start";
                Running = false;
                TimeElapsed.Stop();
                ScreenPrintInterval.Stop();
                UpdateFoundPrimeNumberList();
            });

        } 

        //Clears all Generated Numbers and Resets the Current Number
        private void ClearGeneratedPrimeNumbers()
        {
            PrimeNumberIndexTextbox.Clear();
            ClearNumbers();
            IndexForPrintingFoundPrimeNumbers = 1;
        }

        //Starts at the 20,000th prime number and generates 20,000 more prime numbers
        private void Benchmark()
        {
            AmountToGenerateSlider.Value = 20000;
            IndexForPrintingFoundPrimeNumbers = 20000;
            PrepBenchmark();
            StartGenerating();
        }

        #endregion

        #region Input Triggered Functions

        //Starts and stops Prime number generation
        private void cmd_PlayPause(object sender, RoutedEventArgs e)
        {
            if (!Running)
            {
                StartGenerating();
            }
            else
            {
                StopGenerating();
                maxPrimeNumberIndex = CurrentPrimeNumberIndex;
            }
        }

        //Updates called when UI slider value Changes
        private void cmd_UpdateLabel(object sender,RoutedEventArgs e)
        {
            int newValue =(int) AmountToGenerateSlider.Value;
            AmountToGenerateSliderLabel.Text = newValue.ToString();
            NumberofThreadsTextBox.Text = NumberofThreadsSlider.Value.ToString();
            NumberofThreads =(short) NumberofThreadsSlider.Value;
            UpdateThreadCount();
        }

        //Updates the slider position when a value is input manually
        private void cmd_manualValueUpdate(object sender,RoutedEventArgs e)
        {
            if (AmountToGenerateSliderLabel.Text != null&&AmountToGenerateSliderLabel.Text!= "")
                AmountToGenerateSlider.Value = Int32.Parse(AmountToGenerateSliderLabel.Text);
            else
                AmountToGenerateSliderLabel.Text = "1"; 
           
        }

        //Checks if manually input Text is Numercial
        private void cmd_CheckTextInput(object sender,TextCompositionEventArgs e)
        {
            e.Handled = IsTextAllowed(e.Text);
        }

        //Updates UI and Worker/Task Lists
        private void cmd_UpdateThreadCount(object sender, RoutedEventArgs e)
        {
            if (NumberofThreadsTextBox.Text != null && NumberofThreadsTextBox.Text != "")
            {
                NumberofThreads = (short)Int32.Parse(NumberofThreadsTextBox.Text);
            }
            else
            {
                NumberofThreads = 1;
                NumberofThreadsTextBox.Text = "1";
            }

            if(NumberofThreads>7)
            {
                NumberofThreads = 7;
                NumberofThreadsTextBox.Text = "7";
            }
            NumberofThreadsSlider.Value = NumberofThreads;
            UpdateThreadCount();
        }

        //Searches List of Generated Number for certain Prime Number Index
        private void cmd_SearchForPrimeNumber(object sender, RoutedEventArgs e)
        {
            int SearchNumberValue = Int32.Parse(SearchNumber.Text);
            SearchNumberValue -= (SearchNumberValue / 10000)+1;

            if (SearchNumberValue > CurrentPrimeNumberIndex)
            {
                PrimeNumberIndexTextbox.ScrollToEnd();
            }
            else
            {
                PrimeNumberIndexTextbox.ScrollToLine(SearchNumberValue);
            }
        }

        //Checks Whether Background workers, or Task based Multithreading should be used
        private void cmd_RadioButtonCheck(object sender, RoutedEventArgs e)
        {
            if((bool)BackGroundWorkerRadioButton.IsChecked)
            {
                WorkerBasedMultithreading = true;
            }
            else
            {
                WorkerBasedMultithreading = false;
            }
        }

        //Clears Generated Prime Number List
        private void cmd_ClearList(object sender, RoutedEventArgs e)
        {           
            if(MessageBox.Show("This will delete all generated prime numbers!","Proceed?",MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                ClearGeneratedPrimeNumbers();
            }
        }

        //Runs Benchmark
        private void cmd_RunBenchmark(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("This will delete all generated prime numbers!", "Proceed?", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                ClearGeneratedPrimeNumbers();
                Benchmark();
            }
        }

        #endregion

    }
}
 
