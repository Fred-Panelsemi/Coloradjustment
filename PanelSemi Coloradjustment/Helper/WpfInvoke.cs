using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Input;

namespace PanelSemi_Coloradjustment.Helper
{
    public static class WpfInvoke
    {
        public static void TryInvoke<TDisp>(this TDisp dispatcher, Action<TDisp> action) where TDisp : DispatcherObject
        {
            if (dispatcher.Dispatcher.CheckAccess())
            {
                action(dispatcher);
                return;
            }

            dispatcher.Dispatcher.Invoke(action, dispatcher);
        }

        public static TResult TryInvoke<TDisp, TResult>(this TDisp dispatcher, Func<TDisp, TResult> action) where TDisp : DispatcherObject
        {
            if (dispatcher.Dispatcher.CheckAccess())
            {
                return action(dispatcher);
            }

            return (TResult)dispatcher.Dispatcher.Invoke(action, dispatcher);
        }

        public static DispatcherOperation TryInvokeAsync<TDisp>(this TDisp dispatcher, Action<TDisp> action, DispatcherPriority priority = DispatcherPriority.Normal) where TDisp : DispatcherObject
        {
            return dispatcher.Dispatcher.InvokeAsync(delegate
            {
                action(dispatcher);
            }, priority);
        }

        public static DispatcherOperation<TResult> TryInvokeAsync<TDisp, TResult>(this TDisp dispatcher, Func<TDisp, TResult> action, DispatcherPriority priority = DispatcherPriority.Normal) where TDisp : DispatcherObject
        {
            return dispatcher.Dispatcher.InvokeAsync(() => action(dispatcher), priority);
        }

        public static void InvokeBackground(this Control control, Color color)
        {
            control.TryInvoke((Control ctrl) => ctrl.Background = new SolidColorBrush(color));
        }

        public static Color InvokeBackground(this Control control)
        {
            return control.TryInvoke(delegate (Control ctrl)
            {
                if (ctrl.Background is SolidColorBrush solidColorBrush)
                {
                    return solidColorBrush.Color;
                }

                throw new TypeAccessException("Couldn't get color from '" + ctrl.Background.GetType().Name + "'");
            });
        }

        public static Cursor InvokeCursor(this FrameworkElement frameElmt)
        {
            return frameElmt.TryInvoke((FrameworkElement ctrl) => ctrl.Cursor);
        }

        public static void InvokeEnabled(this UIElement uiElmt, bool enabled)
        {
            uiElmt.TryInvoke((UIElement ctrl) => ctrl.IsEnabled = enabled);
        }

        public static bool InvokeEnabled(this UIElement uiElmt)
        {
            return uiElmt.TryInvoke((UIElement ctrl) => ctrl.IsEnabled);
        }

        public static void InvokeFocus(this UIElement uiElmt)
        {
            uiElmt.TryInvoke((UIElement ctrl) => ctrl.Focus());
        }

        public static bool InvokeFocused(this UIElement uiElmt)
        {
            return uiElmt.TryInvoke((UIElement ctrl) => ctrl.IsFocused);
        }

        public static void InvokeFont(this Control control, FontFamily font)
        {
            control.TryInvoke((Control ctrl) => ctrl.FontFamily = font);
        }

        public static FontFamily InvokeFont(this Control control)
        {
            return control.TryInvoke((Control ctrl) => ctrl.FontFamily);
        }

        public static void InvokeForeground(this Control control, Color color)
        {
            control.TryInvoke((Control ctrl) => ctrl.Foreground = new SolidColorBrush(color));
        }

        public static Color InvokeForeColor(this Control control)
        {
            return control.TryInvoke(delegate (Control ctrl)
            {
                if (ctrl.Foreground is SolidColorBrush solidColorBrush)
                {
                    return solidColorBrush.Color;
                }

                throw new TypeAccessException("Couldn't get color from '" + ctrl.Foreground.GetType().Name + "'");
            });
        }

        public static void InvokeHeight(this FrameworkElement frameElmt, double height)
        {
            frameElmt.TryInvoke((FrameworkElement ctrl) => ctrl.Height = height);
        }

        public static double InvokeHeight(this Control frameElmt)
        {
            return frameElmt.TryInvoke((Control ctrl) => ctrl.Height);
        }

        public static void InvokeHide(this UIElement uiElmt)
        {
            uiElmt.TryInvoke((UIElement ctrl) => ctrl.Visibility = Visibility.Collapsed);
        }

        public static void InvokeMargin(this FrameworkElement frameElmt, Thickness margin)
        {
            frameElmt.TryInvoke((FrameworkElement ctrl) => ctrl.Margin = margin);
        }

        public static void InvokeMargin(this FrameworkElement frameElmt, double? left, double? top, double? right, double? bottom)
        {
            frameElmt.TryInvoke(delegate (FrameworkElement ctrl)
            {
                Thickness margin = new Thickness(left ?? ctrl.Margin.Left, top ?? ctrl.Margin.Top, right ?? ctrl.Margin.Right, bottom ?? ctrl.Margin.Bottom);
                ctrl.Margin = margin;
            });
        }

        public static Thickness InvokeLocation(this Control frameElmt)
        {
            return frameElmt.TryInvoke((Control ctrl) => ctrl.Margin);
        }

        public static void InvokeSize(this FrameworkElement frameElmt, Size size)
        {
            frameElmt.TryInvoke(delegate (FrameworkElement ctrl)
            {
                ctrl.Height = size.Height;
                ctrl.Width = size.Width;
            });
        }

        public static void InvokeSize(this FrameworkElement frameElmt, double width, double height)
        {
            frameElmt.TryInvoke(delegate (FrameworkElement ctrl)
            {
                ctrl.Height = height;
                ctrl.Width = width;
            });
        }

        public static Size InvokeSize(this FrameworkElement frameElmt)
        {
            return frameElmt.TryInvoke((FrameworkElement ctrl) => new Size(ctrl.Width, ctrl.Height));
        }

        public static void InvokeShow(this UIElement uiElmt)
        {
            uiElmt.TryInvoke((UIElement ctrl) => ctrl.Visibility = Visibility.Visible);
        }

        public static void InvokeTag(this FrameworkElement frameElmt, object tag)
        {
            frameElmt.TryInvoke((FrameworkElement ctrl) => ctrl.Tag = tag);
        }

        public static object InvokeTag(this FrameworkElement frameElmt)
        {
            return frameElmt.TryInvoke((FrameworkElement ctrl) => ctrl.Tag);
        }

        public static void InvokeVisible(this UIElement uiElmt, Visibility visibility)
        {
            uiElmt.TryInvoke((UIElement ctrl) => ctrl.Visibility = visibility);
        }

        public static Visibility InvokeVisible(this UIElement uiElmt)
        {
            return uiElmt.TryInvoke((UIElement ctrl) => ctrl.Visibility);
        }

        public static void InvokeWidth(this FrameworkElement frameElmt, int width)
        {
            frameElmt.TryInvoke((FrameworkElement ctrl) => ctrl.Width = width);
        }

        public static double InvokeWidth(this FrameworkElement frameElmt)
        {
            return frameElmt.TryInvoke((FrameworkElement ctrl) => ctrl.Width);
        }

        public static void InvokeContent(this ContentControl control, object content)
        {
            control.TryInvoke((ContentControl ctrl) => ctrl.Content = content);
        }

        public static object InvokeContent(this ContentControl control)
        {
            return control.TryInvoke((ContentControl ctrl) => ctrl.Content);
        }

        public static void InvokeHeader(this HeaderedItemsControl header, object content)
        {
            header.TryInvoke((HeaderedItemsControl ctrl) => ctrl.Header = content);
        }

        public static object InvokeHeader(this HeaderedItemsControl header)
        {
            return header.TryInvoke((HeaderedItemsControl ctrl) => ctrl.Header);
        }

        public static void InvokeChecked(this ToggleButton button, bool? value)
        {
            button.TryInvoke((ToggleButton btn) => btn.IsChecked = value);
        }

        public static bool? InvokeChecked(this ToggleButton button)
        {
            return button.TryInvoke((ToggleButton chk) => chk.IsChecked);
        }

        public static void InvokeClose(this Window window)
        {
            window.TryInvoke(delegate (Window wind)
            {
                wind.Close();
            });
        }

        public static void InvokeAdd(this Panel panel, UIElement uiElmt)
        {
            panel.TryInvoke((Panel ctrl) => ctrl.Children.Add(uiElmt));
        }

        public static void InvokeImage(this Image image, ImageSource img)
        {
            image.TryInvoke((Image ctrl) => ctrl.Source = img);
        }

        public static ImageSource InvokeImage(this Image picbox)
        {
            return picbox.TryInvoke((Image ctrl) => ctrl.Source);
        }

        public static void InvokeValue(this RangeBase range, int value)
        {
            range.TryInvoke((RangeBase ctrl) => ctrl.Value = value);
        }

        public static double InvokeValue(this RangeBase range)
        {
            return range.TryInvoke((RangeBase ctrl) => ctrl.Value);
        }

        public static void InvokeMaximum(this RangeBase range, int max)
        {
            range.TryInvoke((RangeBase ctrl) => ctrl.Maximum = max);
        }

        public static double InvokeMaximum(this RangeBase range)
        {
            return range.TryInvoke((RangeBase ctrl) => ctrl.Maximum);
        }

        public static void InvokeMinimum(this RangeBase range, int min)
        {
            range.TryInvoke((RangeBase ctrl) => ctrl.Minimum = min);
        }

        public static double InvokeMinimum(this RangeBase range)
        {
            return range.TryInvoke((RangeBase ctrl) => ctrl.Minimum);
        }

        public static void InvokeRange(this RangeBase range, double min, double max)
        {
            range.TryInvoke(delegate (RangeBase ctrl)
            {
                ctrl.Minimum = min;
                ctrl.Maximum = max;
            });
        }

        public static double[] InvokeRange(this RangeBase range)
        {
            return range.TryInvoke((RangeBase ctrl) => new double[2] { ctrl.Maximum, ctrl.Minimum });
        }

        public static void InvokeChecked(this MenuItem item, bool check)
        {
            item.TryInvoke((MenuItem ctrl) => item.IsChecked = check);
        }

        public static bool InvokeChecked(this MenuItem item)
        {
            return item.TryInvoke((MenuItem ctrl) => item.IsChecked);
        }

        public static void InvokeSelectedIndex(this Selector selector, int index)
        {
            selector.TryInvoke((Selector cbox) => cbox.SelectedIndex = index);
        }

        public static int InvokeSelectedIndex(this Selector selector)
        {
            return selector.TryInvoke((Selector cbox) => cbox.SelectedIndex);
        }

        public static void InvokeSelectedItem<TItem>(this Selector selector, TItem item)
        {
            selector.TryInvoke((Selector cbox) => cbox.SelectedItem = item);
        }

        public static object InvokeSelectedItem(this Selector selector)
        {
            return selector.TryInvoke((Selector cbox) => cbox.SelectedItem);
        }

        public static void InvokeInsert(this ItemsControl itemsControl, int index, object value, bool delete = false)
        {
            itemsControl.TryInvoke(delegate (ItemsControl ctrl)
            {
                if (delete && ctrl.Items.Count > 200)
                {
                    ctrl.Items.RemoveAt(ctrl.Items.Count - 1);
                }

                ctrl.Items.Insert(index, value);
            });
        }

        public static void InvokeAdd(this ItemsControl itemsControl, object value, bool delete = false)
        {
            itemsControl.TryInvoke(delegate (ItemsControl ctrl)
            {
                if (delete && ctrl.Items.Count > 200)
                {
                    ctrl.Items.RemoveAt(ctrl.Items.Count - 1);
                }

                ctrl.Items.Add(value);
            });
        }

        public static void InvokeAdd<TValue>(this ItemsControl itemsControl, IEnumerable<TValue> value, bool delete = false)
        {
            itemsControl.TryInvoke(delegate (ItemsControl ctrl)
            {
                if (delete && ctrl.Items.Count > 200)
                {
                    ctrl.Items.RemoveAt(ctrl.Items.Count - 1);
                }

                if (value is string)
                {
                    ctrl.Items.Add(value);
                    return;
                }

                foreach (TValue item in value)
                {
                    ctrl.Items.Add(item);
                }
            });
        }

        public static void InvokeRemove<TItem>(this ItemsControl itemsControl, TItem value)
        {
            itemsControl.TryInvoke(delegate (ItemsControl ctrl)
            {
                ctrl.Items.Remove(value);
            });
        }

        public static void InvokeRemove(this ItemsControl itemsControl, int index)
        {
            itemsControl.TryInvoke(delegate (ItemsControl ctrl)
            {
                ctrl.Items.RemoveAt(index);
            });
        }

        public static void InvokeClear(this ItemsControl itemsControl)
        {
            itemsControl.TryInvoke(delegate (ItemsControl ctrl)
            {
                ctrl.Items.Clear();
            });
        }

        public static void InvokeClearAndAdd(this ItemsControl itemsControl, object value)
        {
            itemsControl.TryInvoke(delegate (ItemsControl ctrl)
            {
                ctrl.Items.Clear();
                ctrl.Items.Add(value);
            });
        }

        public static void InvokeClearAndAdd<TValue>(this ItemsControl itemsControl, IEnumerable<TValue> value)
        {
            itemsControl.TryInvoke(delegate (ItemsControl ctrl)
            {
                ctrl.Items.Clear();
                if (value is string)
                {
                    ctrl.Items.Add(value);
                    return;
                }

                foreach (TValue item in value)
                {
                    ctrl.Items.Add(item);
                }
            });
        }
    }
}
