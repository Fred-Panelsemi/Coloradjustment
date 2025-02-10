using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanelSemi_Coloradjustment.ViewModel
{
    public class DictionaryViewModel : INotifyPropertyChanged
    {
        private int _key;
        public int Key
        {
            get => _key;
            set
            {
                _key = value;
                OnPropertyChanged(nameof(Key));
            }
        }

        private ObservableCollection<int> _values;
        public ObservableCollection<int> Values
        {
            get => _values;
            set
            {
                if (_values != null)
                    _values.CollectionChanged -= Values_CollectionChanged;

                _values = value;

                if (_values != null)
                    _values.CollectionChanged += Values_CollectionChanged;

                OnPropertyChanged(nameof(Values));
                //OnPropertyChanged(nameof(ValuesAsString)); // 更新顯示
            }
        }
        // 當 List 內部變更時，通知 UI
        private void Values_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Values));
            //OnPropertyChanged(nameof(ValuesAsString));
        }



        // 轉換為顯示用的字串
        //public string ValuesAsString => string.Join(", ", Values);

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
