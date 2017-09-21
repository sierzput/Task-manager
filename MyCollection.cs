using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Projekt2
{
    public class MyCollection : ObservableCollection<MyProcess>
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public new void Clear()
        {
            base.Clear();
            RaisePropertyChanged("Clear");
        }

    }
}
