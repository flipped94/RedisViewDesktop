using Avalonia.Controls;
using Avalonia.Input;
using Avalonia;
using System.Windows.Input;
using ReactiveUI;
using Avalonia.Interactivity;
using Avalonia.Data;
using RedisViewDesktop.ViewModels;

namespace RedisViewDesktop.Behaviors
{
    public class TappedBehav : AvaloniaObject
    {
        static TappedBehav()
        {
            CommandProperty.Changed.AddClassHandler<Interactive>(HandleCommandChanged);
        }

        public static readonly AttachedProperty<ICommand> CommandProperty = AvaloniaProperty.RegisterAttached<TappedBehav, Interactive, ICommand>(
       "Command", default!, false, BindingMode.OneTime);

        public static readonly AttachedProperty<object> CommandParameterProperty = AvaloniaProperty.RegisterAttached<TappedBehav, Interactive, object>(
        "CommandParameter", default!, false, BindingMode.OneWay, null);

        private static void HandleCommandChanged(Interactive interactElem, AvaloniaPropertyChangedEventArgs args)
        {
            if (args.NewValue is ICommand commandValue)
            {
                // Add non-null value
                interactElem.AddHandler(InputElement.TappedEvent, Handler!);
            }
            else
            {
                // remove prev value
                interactElem.RemoveHandler(InputElement.TappedEvent, Handler!);
            }
            // local handler fcn
            static void Handler(object s, RoutedEventArgs e)
            {
                if (s is Interactive interactElem)
                {
                    // This is how we get the parameter off of the gui element.
                    object commandParameter = interactElem.GetValue(CommandParameterProperty);
                    ICommand commandValue = interactElem.GetValue(CommandProperty);
                    if (commandValue?.CanExecute(commandParameter) == true)
                    {
                        commandValue.Execute(commandParameter);
                    }
                }
            }
        }

        public static void SetCommand(AvaloniaObject element, ICommand commandValue)
        {
            element.SetValue(CommandProperty, commandValue);
        }

        public static ICommand GetCommand(AvaloniaObject element)
        {
            return element.GetValue(CommandProperty);
        }

        public static void SetCommandParameter(AvaloniaObject element, object parameter)
        {
            element.SetValue(CommandParameterProperty, parameter);
        }

        public static object GetCommandParameter(AvaloniaObject element)
        {
            return element.GetValue(CommandParameterProperty);
        }

    }
}
