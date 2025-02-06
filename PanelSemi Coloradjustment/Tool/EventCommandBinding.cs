using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows;

namespace PanelSemi_Coloradjustment
{
    internal class EventCommandBinding
    {
        public static readonly DependencyProperty ValueChangedCommandProperty =
        DependencyProperty.RegisterAttached(
            "ValueChangedCommand",
            typeof(ICommand),
            typeof(EventCommandBinding),
            new PropertyMetadata(null, OnValueChangedCommandChanged));

        public static ICommand GetValueChangedCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(ValueChangedCommandProperty);
        }

        public static void SetValueChangedCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ValueChangedCommandProperty, value);
        }

        private static void OnValueChangedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RangeBase rangeBase)
            {
                rangeBase.ValueChanged -= OnValueChanged;
                if (e.NewValue != null)
                {
                    rangeBase.ValueChanged += OnValueChanged;
                }
            }
        }

        private static void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sender is DependencyObject d)
            {
                var command = GetValueChangedCommand(d);
                if (command != null && command.CanExecute(e.NewValue))
                {
                    command.Execute(e.NewValue);
                }
            }
        }
    }
}
