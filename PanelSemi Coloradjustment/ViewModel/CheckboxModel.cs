using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanelSemi_Coloradjustment.ViewModel
{
    public class CheckBoxModel : INotifyPropertyChanged
    {
        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
                OnCheckedChanged();
                OnCheckedChanged_2UIChange(Content,IsChecked);
            }
        }


        public string Content { get; set; } // 顯示文字

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }

        public delegate void OnCheckedChanged_2UI(string Content,bool IsChecked);
        public event OnCheckedChanged_2UI OnCheckedChanged_2UIChangeEvent;
        public void OnCheckedChanged_2UIChange(string Content, bool IsChecked) => OnCheckedChanged_2UIChangeEvent(Content, IsChecked);



        public void OnCheckedChanged()
        {
            
        }
    }
}
