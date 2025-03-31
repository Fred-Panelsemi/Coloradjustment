﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CtLib.Library;
using CtLib.Library.Wpf;
using MahApps.Metro.Controls;

namespace PanelSemi_Coloradjustment
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        MainProcess mMainProcess = new MainProcess();

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = mMainProcess;
            
            this.Topmost = true;
            LoadingWindowLibrary.LoadingWindowHelper.Close();

        }

        private void NumericUpDown_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }


        private void MetroWindow_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mMainProcess.WindowClose_Action();
        }

        private void MetroWindow_Activated(object sender, EventArgs e)
        {
            this.Topmost = false;
        }

        
    }
}
