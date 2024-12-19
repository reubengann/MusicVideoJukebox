using Prism.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

//namespace MusicVideoJukebox.Core
//{
public class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false; // No change, no notification needed
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true; // Property changed
    }

    protected bool SetUnderlyingProperty<T>(T currentValue, T newValue, Action<T> setValue, [CallerMemberName] string propertyName = null!)
    {
        if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
            return false;

        setValue(newValue); // Update the value in the wrapped object
        OnPropertyChanged(propertyName); // Notify UI
        return true;
    }
}

public abstract class AsyncInitializeableViewModel : BaseViewModel
{
    public abstract Task Initialize();
}
