using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace Projekt2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ObservableCollection<Process> processes = new ObservableCollection<Process>(Process.GetProcesses());
        //public Thread refreshThread { get; set; }

        public ObservableCollection<Process> Processes
        {
            get { return this.processes; }
            set { this.processes = value; }
        }

        public App()
        {
            //refreshThread = new Thread(new ThreadStart(() => refresh()));
            //refreshThread.Start();
        }

        //private void refresh()
        //{
        //    Process[] proc={};
        //    while (true)
        //    {
        //        //foreach (var p in proc)
        //        //{
        //        //    Processes.Remove(p);
        //        //}
        //        proc = Process.GetProcesses();
        //        List<MyProcess> procs = new List<MyProcess>();
        //        foreach (var p in proc)
        //        {
        //            procs.Add(new MyProcess(p));
        //        }
        //        Processes = new ObservableCollection<MyProcess>(procs);
        //        Processes[0].RaisePropertyChanged("Name");
        //        //processGrid.DataContext = processesCollection;
        //        Thread.Sleep(1000);
        //    }
        //}
    }
}
