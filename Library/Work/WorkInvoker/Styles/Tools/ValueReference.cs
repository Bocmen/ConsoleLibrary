using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WorkInvoker.Styles.Tools
{
    public class ValueReference<T> : INotifyPropertyChanged
    {
        private T _value = default;

        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static implicit operator T(ValueReference<T> colorReference) => colorReference._value;
    }
}
