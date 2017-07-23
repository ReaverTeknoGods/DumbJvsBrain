using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace DumbJVSManager
{
    public class XInputRemapViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Visibility ExtrasVisibility => ExtrasVisible ? Visibility.Visible : Visibility.Collapsed;

        private bool _extrasVisible = false;

        public bool ExtrasVisible
        {
            get
            {
                return _extrasVisible;
            }

            set
            {
                _extrasVisible = value;
                OnPropertyChanged(nameof(ExtrasVisible));
            }
        }
    }
}
