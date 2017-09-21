using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Projekt2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private ObservableCollection<MyProcess> processesCollection;
        public Process[] processes;
        public int processId{get;set;}
        Thread restartThread;

        public MainWindow()
        {
            InitializeComponent();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(Timer_RefreshProcessesList);
            timer.Interval = new TimeSpan(0, 0, 0, 1);
            timer.Start();

            Button_RefreshProcessesList(new object(), new RoutedEventArgs());
        }

        private void restart()
        {
            ProcessStartInfo info = null;
            //int id = Process.GetProcessesByName("notepad").First().Id;
            Process pr;
            string filename = Process.GetProcessById(processId).Modules[0].FileName;
            while (true)
            {
                var procs = Process.GetProcesses();
                pr = null;
                foreach (Process p in procs)
                {
                    if (p.Id == processId)
                    {
                        pr = p;
                        info = pr.StartInfo;
                        break;
                    }
                }
                if (pr == null)
                {
                    pr = Process.Start(filename);
                    pr.StartInfo = info;
                    processId = pr.Id;
                }
            }
        }

        private void refresh()
        {
            try
            {
                while (true)
                {
                    var processesCollection = ((App)Application.Current).Processes;
                    int index;
                    int? id = null;
                    index = App.Current.Dispatcher.Invoke(delegate
                    {
                        index = processesList.SelectedIndex;
                        return index;
                    });
                    if (index != -1)
                        id = processesCollection.ElementAt(index).Id;
                    List<Process> processes = Process.GetProcesses().ToList();
                    processes.Sort(Comparer<Process>.Create((x, y) => x.ProcessName.CompareTo(y.ProcessName)));
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        processesCollection.Clear();
                    });
                    foreach (var p in processes)
                    {
                        App.Current.Dispatcher.Invoke((Action)delegate
                        {
                            processesCollection.Add(p);
                        });
                    }
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        processGrid.DataContext = processesCollection;
                    });
                    if (index != -1)
                        for (int i = 0; i < processesCollection.Count; i++)
                        {
                            if (processesCollection[i].Id == id.Value)
                            {
                                index = i;
                                break;
                            }
                        }
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        processesList.SelectedIndex = index;
                        processesList.ScrollIntoView(processesList.SelectedItem);
                    });
                    Thread.Sleep(1000);
                }
            }
            catch (Exception) { }
        }

        private void Button_RefreshProcessesList(object sender, RoutedEventArgs e)
        {
            Timer_RefreshProcessesList(sender, e);
        }

        private void Timer_RefreshProcessesList(object sender, EventArgs e)
        {
            //((ObjectDataProvider)FindResource("odpProcesses")).Refresh();
            var processesCollection = ((App)Application.Current).Processes;
            int index = processesList.SelectedIndex;
            int? id = null;
            if (index != -1)
                id = processesCollection.ElementAt(index).Id;
            List<Process> processes = Process.GetProcesses().ToList();
            processes.Sort(Comparer<Process>.Create((x, y) => x.ProcessName.CompareTo(y.ProcessName)));
            processesCollection.Clear();
            foreach (var p in processes)
            {
                processesCollection.Add(p);
            }
            processGrid.DataContext = processesCollection;

            if (index != -1)
                for (int i = 0; i < processesCollection.Count; i++)
                {
                    if (processesCollection[i].Id == id.Value)
                    {
                        index = i;
                        break;
                    }
                }
            processesList.SelectedIndex = index;
            keepAliveTB.Text = processId.ToString();
        }

        private void Button_KillProcess(object sender, RoutedEventArgs e)
        {
            //((Process[])((ObjectDataProvider)FindResource("odpProcesses")).Data)[(int)processesList.SelectedItem].Kill();
            var processesCollection = ((App)Application.Current).Processes;
            int index = processesList.SelectedIndex;
            int? id = null;
            if (index == -1)
                return;
            else
                id = processesCollection.ElementAt(index - 1).Id;
            //Process tmp = processesCollection[index].Process;
            processesCollection.ElementAt(index).Kill();
            processesCollection.RemoveAt(index);
            //tmp.Kill();
            index--;
            List<Process> processes = Process.GetProcesses().ToList();
            processes.Sort(Comparer<Process>.Create((x, y) => x.ProcessName.CompareTo(y.ProcessName)));
            processesCollection.Clear();
            foreach (var p in processes)
            {
                processesCollection.Add(p);
            }
            var tempCollection = ((App)Application.Current).Processes;
            processGrid.DataContext = tempCollection;

            if (index != -1)
                for (int i = 0; i < processesCollection.Count; i++)
                {
                    if (processesCollection[i].Id == id.Value)
                    {
                        index = i;
                        break;
                    }
                }
            if (((string)keepAliveButton.Content) != "Keep alive")
            {
                index++;
            }
            processesList.SelectedIndex = index;
            Timer_RefreshProcessesList(sender, e);
        }

        private void processesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var processesCollection = ((App)Application.Current).Processes;
            int index = processesList.SelectedIndex;
            if (index == -1)
                return;
            Process p;
            string details;
            List<string> threads = new List<string>();
            List<string> modules = new List<string>();
            string format = "{0,-30}{1}\n";
            string format2 = "{0,-30}{1}K\n";
            p = processesCollection[index];
            try
            {
                details =
                    String.Format(format, "Main window title:         ", p.MainWindowTitle) +
                    String.Format(format, "Procesor affinity:         ", p.ProcessorAffinity) +
                    String.Format(format, "Privileged processor time: ", p.PrivilegedProcessorTime) +
                    String.Format(format, "Threads:                   ", p.Threads.Count) +
                    String.Format(format, "Modules:                   ", p.Modules.Count) +
                    String.Format(format, "Base priority:             ", p.BasePriority) +
                    String.Format(format2, "Working set:               ", p.WorkingSet64 / 1024) +
                    String.Format(format2, "Paged memory size:         ", p.PagedMemorySize64 / 1024) +
                    String.Format(format2, "Virtual memory size:       ", p.VirtualMemorySize64 / 1024) +
                    String.Format(format, "Start time:                ", p.StartTime);

                detailsTextBox.Text = details;
            }
            catch (Exception ex)
            {
                detailsTextBox.Text = ex.Message;
            }
            try
            {
                var it = p.Threads.GetEnumerator();
                while (it.MoveNext())
                {
                    threads.Add(((ProcessThread)it.Current).Id.ToString());
                }
            }
            catch (Exception ex)
            {
                threads.Add(ex.Message);
            }
            finally
            {
                threadsListBox.ItemsSource = threads;
            }
            try
            {
                var it = p.Modules.GetEnumerator();
                while (it.MoveNext())
                {
                    modules.Add(((ProcessModule)it.Current).ModuleName);
                }
            }
            catch (Exception ex)
            {
                modules.Add(ex.Message);
            }
            finally
            {
                modulesListBox.ItemsSource = modules;
            }
        }

        private void Button_KeepAliveProcess(object sender, RoutedEventArgs e)
        {
            if (((string)keepAliveButton.Content)=="Keep alive")
            {
                keepAliveTB.IsEnabled = false;
                keepAliveButton.Content = "Disable";
                int index = processesList.SelectedIndex;
                var procs=Process.GetProcesses().ToList();
                procs.Sort(Comparer<Process>.Create((x, y) => x.ProcessName.CompareTo(y.ProcessName)));
                int id=procs[index].Id;
                keepAliveTB.Text = id.ToString();
                processId = id;
                restartThread = new Thread(new ThreadStart(() => restart()));
                restartThread.IsBackground = true;
                restartThread.Start();
            }
            else
            {
                keepAliveTB.IsEnabled = true;
                processId = 0;
                keepAliveButton.Content = "Keep alive";
                restartThread.Abort();
            }

        }
    }
}
