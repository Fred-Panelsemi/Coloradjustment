using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ControlzEx.Standard;
using MahApps.Metro.Controls;
using PanelSemi_Coloradjustment.Helper;
using PanelSemi_Coloradjustment.Properties;
using PanelSemi_Coloradjustment.ViewModel;
using static System.Net.Mime.MediaTypeNames;
// Panel Semi C#/UI Standard Rules
// 最上層必須敘述該CS作用
// VM 的部分只有與介面綁定的物件可以使用Publish
// 需要標明 該Class 是屬於獨立還是 Partial，若是Partical 需標明是MVVM的哪層
// 每個Void皆需要有Summary
// Publish 物件需遵守大駝峰命名法         => Publish String PanelSemi
// Private 物件前綴需要有m的大駝峰命名法  => Private String mPanelSemi
// 方法內物件須遵守小駝峰式命名法           => int panelSemi
//================================================================================================================================================================================
namespace PanelSemi_Coloradjustment
{
    // MainProcess 本體
    // V1.0.0.0
    // 1. 【Init】 Mainprocess 結構Init
    //    => MainProcess()
    // 2. 【事件委派】 用於當 FPGA_B FPGA_A 的數值改變時 通知VM層的介面綁定物件進行刷新
    //    FPGA_Dictionary_2UIChangeEvent()
    // 3. 【Void】 拼接需要的資訊矩陣初始化
    //    => initRegBoxMark()
    // 4. 【Void】 填充拼接需要的資訊矩陣
    //    => fillRegBoxMark()
    // 5. 【Void】 離開視窗觸發之動作
    //    => WindowClose_Action()
    // 6. 【Void】 回復預設色差
    //    => RecoverDefaultValue_Action()
    // 7. 【Void】 開始進行色差調整併進入色差調整模式
    //    => StartandInit_Action()
    // 8. 【Void】 更新目前連接電腦的屏幕數量
    //    => UpdateNumofPanel()
    // 9. 【Void】 重新搜尋USB COM Port
    //    => USB_ComPort_FindandOpen_Action()
    // 10 【Void】 儲存色差資訊
    //    => SaveColorInfo_Action()
    // 11 【Void】 變更紅色數值
    //    => AdjValueR_Action()
    // 12 【Void】 變更綠色數值
    //    => AdjValueG_Action()
    // 13 【Void】 變更藍色數值
    //    => AdjValueB_Action()
    // 14 【事件委派】 當Checkbox發生變動時的事件
    //    => SinglecheckBoxStates_CollectionChanged
    // 15 【事件委派】 當Checkbox發生變動時的事件
    //    => Ix4checkBoxStates_CollectionChanged
    // 16 【事件委派】 當Checkbox發生變動時的事件
    //    => mCheckBoxStates_CollectionChanged
    // 17 【事件委派】 當FPGA_B FPGA_A發生變動時的事件
    //    => FPGA_Dictionary_2UIChangeEvent

    //================================================================================================================================================================================
    // Partial VM >> MainProcess
    internal partial class MainProcess :  INotifyPropertyChanged
    {
        // 測試區 ================================================================================================================================================================================
      
   

        // 測試區 ================================================================================================================================================================================
        
        private PaneladjustSwitch mPaneladjustSwitch;
        private TotalProcess mTotalProcess = new TotalProcess();
        private EasyDebug mEasyDebug;
        private ColorControl mColorControl;

        public MainProcess()
        {
            /* 測試宣告區 */

            /* 撈出版本號　=> 由AssemblyInfo.cs中設定 */
            var asm = Assembly.GetExecutingAssembly();
            PanelSemi_SplicingVersion = $"v{asm.GetName().Version}";
            /**/
            LoadingWindowLibrary.LoadingWindowHelper.Create();
            //LoadingWindow.CreateNew();
            Thread.Sleep(2000);
            /* Command 宣告 */
            FrontBackSimulation = new RelayCommand(FrontSimulation_Action);
            StartandInit = new RelayCommand(StartandInit_Action);
            USB_ComPort_FindandOpen = new RelayCommand(USB_ComPort_FindandOpen_Action);
            SaveColorInfo = new RelayCommand(SaveColorInfo_Action);
            RecoverDefaultValue = new RelayCommand(RecoverDefaultValue_Action);
            WindowClose = new RelayCommand(WindowClose_Action);
            ID_On = new RelayCommand(ID_On_Action);
            ID_Off = new RelayCommand(ID_Off_Action);
            MCU_Reset = new RelayCommand(MCU_Reset_Action);
           

            /* DelegateCommand 宣告 */
            AdjValueR = new DelegateCommand<string>(AdjValueR_Action);
            AdjValueG = new DelegateCommand<string>(AdjValueG_Action);
            AdjValueB = new DelegateCommand<string>(AdjValueB_Action);
            ChangeCulture = new DelegateCommand<string>(ChangeCulture_Action);

            /* Microusb_Items Combox 初始化 */
            Microusb_Items = new ObservableCollection<ComboBoxItemModel>
            {
                new ComboBoxItemModel { Image = ConvertToImageSource(Resources.RDn), Text = "Lower Right",Num =1 },
                new ComboBoxItemModel { Image = ConvertToImageSource(Resources.RUn), Text = "Upper Right",Num =2 },
                new ComboBoxItemModel { Image = ConvertToImageSource(Resources.LUn), Text = "Upper Left", Num =3 },
                new ComboBoxItemModel { Image = ConvertToImageSource(Resources.LDn), Text = "Lower Left", Num =4 }
            };
            Microusb_SelectedItem = Microusb_Items[0];

            /* HDMI_Items Combox 初始化 */
            HDMI_Items = new ObservableCollection<ComboBoxItemModel>
            {
                new ComboBoxItemModel { Image = ConvertToImageSource(Resources.D2U), Text = "Lower Side",Num =1 },
                new ComboBoxItemModel { Image = ConvertToImageSource(Resources.R2L), Text = "Right Side",Num =2 },
                new ComboBoxItemModel { Image = ConvertToImageSource(Resources.U2D), Text = "Upper Side",Num =3 },
                new ComboBoxItemModel { Image = ConvertToImageSource(Resources.L2R), Text = "Left Side" ,Num =4 }
            };
            HDMI_SelectedItem = HDMI_Items[0];

            /* Mode_Item 列表 初始化 */
            Mode_Item = new ObservableCollection<string>
            {
                "Total Tile",
                "1x4 Tile",
                "Single Tile"
            };
            Mode_SelectedItem = Mode_Item[0];

            /* 初始化語系 */
            var cultFs = new Uri[]
            {
                new Uri(@"Views/Cultures/zh-TW.xaml",UriKind.Relative),
                new Uri(@"Views/Cultures/en-US.xaml",UriKind.Relative),
                new Uri(@"Views/Cultures/ja-JP.xaml",UriKind.Relative)
            };
            PanelSemi_Coloradjustment.Helper.CultureHelper.Initial(
                cultFs,
                PanelSemi_Coloradjustment.Properties.Settings.Default.DefaultCulture,
                System.Windows.Application.Current
            );

            /* ColorSpaceCheckBoxStates checkbox 填充 */
            ColorSpaceCheckBoxStates = new ObservableCollection<bool>();
            for(int i = 0; i<16; i++)
            {
                ColorSpaceCheckBoxStates.Add(false);
            }
            ColorSpaceCheckBoxStates.CollectionChanged += mCheckBoxStates_CollectionChanged;

            /* Ix4checkBoxStates checkbox 填充 */
            Ix4checkBoxStates = new ObservableCollection<bool>();
            for(int i = 0; i<8 ; i++)
            {
                Ix4checkBoxStates.Add(false);
            }
            Ix4checkBoxStates.CollectionChanged += Ix4checkBoxStates_CollectionChanged;

            /* SinglecheckBoxStates checkbox 填充 */
            SinglecheckBoxStates = new ObservableCollection<bool>();
            for (int i = 0; i < 4; i++)
            {
                SinglecheckBoxStates.Add(false);
            }
            SinglecheckBoxStates.CollectionChanged += SinglecheckBoxStates_CollectionChanged;

            

            /* 其他 Class 宣告 */
            mPaneladjustSwitch = new PaneladjustSwitch();
            mPaneladjustSwitch.FPGA_Dictionary_2UIChangeEvent += FPGA_Dictionary_2UIChangeEvent;
            mEasyDebug = new EasyDebug(mTotalProcess);
            mColorControl = new ColorControl(mTotalProcess, mPaneladjustSwitch,mColorControl);

            /* List_fillBrush FPGA_B 畫筆顏色填充 */
            List_fillBrush.Add(FillBrushX1T1);
            List_fillBrush.Add(FillBrushX1T2);
            List_fillBrush.Add(FillBrushX1T3);
            List_fillBrush.Add(FillBrushX1T4);
            List_fillBrush.Add(FillBrushX2T1);
            List_fillBrush.Add(FillBrushX2T2);
            List_fillBrush.Add(FillBrushX2T3);
            List_fillBrush.Add(FillBrushX2T4);
            List_fillBrush.Add(FillBrushX3T1);
            List_fillBrush.Add(FillBrushX3T2);
            List_fillBrush.Add(FillBrushX3T3);
            List_fillBrush.Add(FillBrushX3T4);
            List_fillBrush.Add(FillBrushX4T1);
            List_fillBrush.Add(FillBrushX4T2);
            List_fillBrush.Add(FillBrushX4T3);
            List_fillBrush.Add(FillBrushX4T4);

            /* List_fillBrush FPGA_A 畫筆顏色填充 */
            List_fillBrush.Add(AFillBrushX1T1);
            List_fillBrush.Add(AFillBrushX1T2);
            List_fillBrush.Add(AFillBrushX1T3);
            List_fillBrush.Add(AFillBrushX1T4);
            List_fillBrush.Add(AFillBrushX2T1);
            List_fillBrush.Add(AFillBrushX2T2);
            List_fillBrush.Add(AFillBrushX2T3);
            List_fillBrush.Add(AFillBrushX2T4);
            List_fillBrush.Add(AFillBrushX3T1);
            List_fillBrush.Add(AFillBrushX3T2);
            List_fillBrush.Add(AFillBrushX3T3);
            List_fillBrush.Add(AFillBrushX3T4);
            List_fillBrush.Add(AFillBrushX4T1);
            List_fillBrush.Add(AFillBrushX4T2);
            List_fillBrush.Add(AFillBrushX4T3);
            List_fillBrush.Add(AFillBrushX4T4);


            Panel_ID_CheckBoxes = new ObservableCollection<CheckBoxModel>();
            LoadingWindowLibrary.LoadingWindowHelper.Update("建立連線");
            /* 如果有找到USB接口 且 已連線 >>*/
            if (mTotalProcess.isUSBCconnect != "" & mTotalProcess.isUSBOpen == true)
            {
                LoadingWindowLibrary.LoadingWindowHelper.Update("匯入螢幕資訊");
                /*>> Update Panel 數量 */
                UpdateNumofPanel();
                Thread.Sleep(2000);
            }
            else
            {
                LoadingWindowLibrary.LoadingWindowHelper.Update("目前無連接屏幕");
                Thread.Sleep(2000);
            }
      
            DataB = new Dictionary<int, ObservableCollection<int>>();
            DataA = new Dictionary<int, ObservableCollection<int>>();
            DataB = mPaneladjustSwitch.FPGA_B;
            DataA = mPaneladjustSwitch.FPGA_A;
            LoadingWindowLibrary.LoadingWindowHelper.Update("進入視窗");
            LoadingWindowLibrary.LoadingWindowHelper.Close();
        }

        private void ChangeCulture_Action(string cultureCode)
        {
            CultureInfo newCulture = new CultureInfo(cultureCode);
            CultureHelper.ChangeCulture(newCulture);
        }

        /// <summary>
        /// 將屏幕重啟
        /// </summary>
        private void MCU_Reset_Action()
        {
            mTotalProcess.markreset(99, true);
            mTotalProcess.McuSW_Reset();
        }

        /// <summary>
        /// ID On
        /// </summary>
        private void ID_Off_Action()
        {
            mTotalProcess.markreset(99,true);
            mTotalProcess.mpIDONOFF(Convert.ToByte("0"));
        }

        /// <summary>
        /// ID Off
        /// </summary>
        private void ID_On_Action()
        {
            mTotalProcess.markreset(99, true);
            mTotalProcess.mpIDONOFF(Convert.ToByte("1"));
        }

        /// <summary>
        /// 關閉視窗動作
        /// </summary>
        public void WindowClose_Action()
        {
            // 這裡執行關閉應用程式前的動作
            MessageBoxResult result = System.Windows.MessageBox.Show("確定要關閉應用程式嗎?", "確認", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.No)
            {
                return;
            }
            else
            {
                // 這裡可以做一些釋放資源或存檔的動作
                if (mColorControl.IsColorAdjustmentMode == true)
                {
                    IsEnterColorAdjustmentMode = Visibility.Visible;
                    mColorControl.IsColorAdjustmentMode = false;
                    EnterOrExistColorMoode = "進入 色差調節模式";
                    mColorControl.ExistColorMode();
                }
            }
            System.Windows.Application.Current.Shutdown();
        }

        /// <summary>
        /// 回復預設色差值
        /// </summary>
        private void RecoverDefaultValue_Action()
        {
            mColorControl.RecoverDefault();
        }

        /// <summary>
        /// 事件委派
        /// </summary>
        /// <param name="Content"></param>
        /// <param name="IsChecked"></param>
        private void MainProcess_OnCheckedChanged_2UIChangeEvent(string Content, bool IsChecked)
        {
            if(IsChecked == true)
            {
                for (int i = 0; i < Panel_ID_CheckBoxes.Count; i++)
                {
                    if($"ID {i+1}" != Content)
                    {
                        Panel_ID_CheckBoxes[i].IsChecked = false;
                    }
                    else
                    {
                        ChoicePanelID = i+1;
                        mColorControl.ChoicePanelID = ChoicePanelID;
                    }
                }
            }
            /* 更換ID時 要去重讀 Flah的色差值 */
            if (mColorControl.IsColorAdjustmentMode == true) { mColorControl.ColorInfoRead(); }
            /* 刷新模擬器的顏色 */
            mPaneladjustSwitch.PanelSwitch_update("Total Tile", 99, 99, 99, mPaneladjustSwitch.ColorspaceFlagnum, mPaneladjustSwitch.ColorFalg, 0, 0, 0);
            /* 並在Panel再刷上原本的數值 因為有調整後沒儲存更換ID再換回來 Panel 上的色塊不會不見 */
            if (mColorControl.IsColorAdjustmentMode == true) { mColorControl.ColorTempSave(); }
         
        }
            
        /// <summary>
        /// 選擇ID後並開始調整色差
        /// 1. 會先用 Serialmode 分配ID
        /// 2. 進行色差Init
        /// 3. 勾選 1020 階W
        /// </summary>
        private void StartandInit_Action()
        {
            LoadingWindowLibrary.LoadingWindowHelper.Create();
            Thread.Sleep(500);
            if(mColorControl.IsColorAdjustmentMode == false)
            {
                if (mColorControl.ChoicePanelID == 0)
                {
                    System.Windows.MessageBox.Show("請選擇ID再開始");
                    return;
                }
                mColorControl.IsColorAdjustmentMode = true;
                //mEasyDebug.ID_Serialmode();
                Thread.Sleep(3000);
                mColorControl.ColorCtrInit();
                /* 勾選1020階白畫面 */
                ColorSpaceCheckBoxStates[3] = true;
                
                EnterOrExistColorMoode = "Exist\r\nAdjustment Mode";
                IsEnterColorAdjustmentMode = Visibility.Hidden;
                //CtLog.Warning("進入 色差調節模式");
            }
            else
            {
                IsEnterColorAdjustmentMode = Visibility.Visible;
                mColorControl.IsColorAdjustmentMode = false;
                EnterOrExistColorMoode = "Enter\r\nAdjustment Mode";
                mColorControl.ExistColorMode();
                //CtLog.Warning("離開 色差調節模式");
            }
            LoadingWindowLibrary.LoadingWindowHelper.Close();
        }

        /// <summary>
        /// Update Panel 數量
        /// </summary>
        private void UpdateNumofPanel()
        {
            /* ID 控建內容清除 */
            Panel_ID_CheckBoxes.Clear();
            /* 呼叫 Coloradjustment 中的  UpdateNumofPanel 來搜尋目前有幾個Panel連接 */
            mColorControl.UpdateNumofPanel();
            /* 將搜尋到的ID數量加入 Panel_ID_CheckBoxes 中 */
            for (int i = 0; i<mColorControl.PanelConut; i++)
            {
                Panel_ID_CheckBoxes.Add(new CheckBoxModel { Content = $"ID {i+1}" });
            }
            for (int i = 0; i < Panel_ID_CheckBoxes.Count; i++)
            {
                Panel_ID_CheckBoxes[i].OnCheckedChanged_2UIChangeEvent += MainProcess_OnCheckedChanged_2UIChangeEvent;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void USB_ComPort_FindandOpen_Action()
        {
            /* 呼叫 TotalProcess 中的 USB_ComPort_FindandOpen 找USB的COM Port並連接 */
            //mTotalProcess.USB_ComPort_FindandOpen();
            ///* 如果有找到USB接口 且 已連線 >>*/
            //if (mTotalProcess.isUSBCconnect != "" & mTotalProcess.isUSBOpen == true)
            //{
            //    /*>> Update Panel 數量 */
            //    UpdateNumofPanel();
            //}

            /* 測試 */
            mTotalProcess.cPGRGB10BITp(3,0,0,1,2,"99","99","99");
        }

        /// <summary>
        /// 
        /// </summary>
        private void SaveColorInfo_Action()
        {
            LoadingWindowLibrary.LoadingWindowHelper.Create();
            Thread.Sleep(2000);
            LoadingWindowLibrary.LoadingWindowHelper.Update("寫入色差中");
            
            mColorControl.ColorSave();
            Thread.Sleep(4000);
            LoadingWindowLibrary.LoadingWindowHelper.Close();
        }

        /// <summary>
        /// 調節紅色色差值
        /// </summary>
        /// <param name="paRa">由介面的Parameter發出看是 up 還是 dwn</param>
        private void AdjValueR_Action(string paRa)
        {
            mColorControl.AdjustR(paRa, Mode_SelectedItem, ValueR);
        
        }

        /// <summary>
        /// 調節綠色色差值
        /// </summary>
        /// <param name="paRa">由介面的Parameter發出看是 up 還是 dwn</param>
        private void AdjValueG_Action(string paRa)
        {
            mColorControl.AdjustG(paRa, Mode_SelectedItem, ValueG);

        }

        /// <summary>
        /// 調節藍色色差值
        /// </summary>
        /// <param name="paRa">由介面的Parameter發出看是 up 還是 dwn</param>
        private void AdjValueB_Action(string paRa)
        {
            mColorControl.AdjustB(paRa, Mode_SelectedItem, ValueB);
        }

        /// <summary>
        /// 
        /// </summary>
        int ix4ChangeInt = 0;
        /// <summary>
        /// 當Checkbox發生變動時的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SinglecheckBoxStates_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if ((bool)e.NewItems[0] == true)
            {
                ix4ChangeInt = e.NewStartingIndex;
                for (int i = 0; i < SinglecheckBoxStates.Count; i++)
                {
                    if (i != ix4ChangeInt)
                    {
                        SinglecheckBoxStates[i] = false;
                    }
                    else
                    {
                        mPaneladjustSwitch.SelectTile = e.NewStartingIndex;
                    }
                }
            }
        }

        /// <summary>
        /// 當Checkbox發生變動時的事件
        /// </summary>
        int singleChangeInt = 0;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ix4checkBoxStates_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if ((bool)e.NewItems[0] == true)
            {
                singleChangeInt = e.NewStartingIndex;
                for (int i = 0; i < Ix4checkBoxStates.Count; i++)
                {
                    if (i != singleChangeInt)
                    {
                        Ix4checkBoxStates[i] = false;
                    }
                    else
                    {
                        if (e.NewStartingIndex < 4)
                        {
                            mPaneladjustSwitch.SelectFPGA = 0;
                            mPaneladjustSwitch.SelectXbNum = e.NewStartingIndex+1;
                        }
                        else
                        {
                            mPaneladjustSwitch.SelectFPGA = 1;
                            mPaneladjustSwitch.SelectXbNum = e.NewStartingIndex-4+1;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        int colorSpaceChangeInt = 0;
        /// <summary>
        /// 當Checkbox發生變動時的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mCheckBoxStates_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if ((bool)e.NewItems[0] == true)
            {
                colorSpaceChangeInt = e.NewStartingIndex;
                for (int i = 0; i < ColorSpaceCheckBoxStates.Count; i++)
                {
                    if (i != colorSpaceChangeInt)
                    {
                        ColorSpaceCheckBoxStates[i] = false;
                    }
                    else
                    {
                        if(i == 0 | i ==1 | i == 2 | i == 3)
                        {
                            mPaneladjustSwitch.ColorspaceFlagnum = 4096;
                            ColorspaceFlag = 4096;
                            Space4096 = Visibility.Visible;
                            Space2048 = Visibility.Hidden;
                            Space1024 = Visibility.Hidden;
                            Space512 = Visibility.Hidden;
                        }
                        else if(i == 4 | i == 5 | i == 6 | i == 7)
                        {
                            mPaneladjustSwitch.ColorspaceFlagnum = 2048;
                            ColorspaceFlag = 2048;
                            Space4096 = Visibility.Hidden;
                            Space2048 = Visibility.Visible;
                            Space1024 = Visibility.Hidden;
                            Space512 = Visibility.Hidden;
                        }
                        else if (i == 8 | i == 9 | i == 10 | i == 11)
                        {
                            mPaneladjustSwitch.ColorspaceFlagnum = 1024;
                            ColorspaceFlag = 1024;
                            Space4096 = Visibility.Hidden;
                            Space2048 = Visibility.Hidden;
                            Space1024 = Visibility.Visible;
                            Space512 = Visibility.Hidden;
                        }
                        else if (i == 12 | i == 13 | i == 14 | i == 15)
                        {
                            mPaneladjustSwitch.ColorspaceFlagnum = 512;
                            ColorspaceFlag = 512;
                            Space4096 = Visibility.Hidden;
                            Space2048 = Visibility.Hidden;
                            Space1024 = Visibility.Hidden;
                            Space512 = Visibility.Visible;
                        }

                        if (i == 0 | i == 4 | i == 8 | i == 12)
                        {
                            mPaneladjustSwitch.ColorFalg = "R";
                        }
                        else if (i == 1 | i == 5 | i == 9 | i == 13)
                        {
                            mPaneladjustSwitch.ColorFalg = "G";
                        }
                        else if (i == 2 | i == 6 | i == 10 | i == 14)
                        {
                            mPaneladjustSwitch.ColorFalg = "B";
                        }
                        else if (i == 3 | i == 7 | i == 11 | i == 15)
                        {
                            mPaneladjustSwitch.ColorFalg = "W";
                        }
                        // 切換色域時將當前色域的數值畫上去上模擬機上 Mode為整屏
                        mPaneladjustSwitch.PanelSwitch_update("Total Tile", 99, 99, 99, mPaneladjustSwitch.ColorspaceFlagnum, mPaneladjustSwitch.ColorFalg, 0, 0, 0);
                        mColorControl.ColrSpaceChange(mPaneladjustSwitch.ColorspaceFlagnum,mPaneladjustSwitch.ColorFalg);
                    }
                }
                /* 介面互動 */
            }
        }

        // 將 Dictionary FPGA_B FPGA_A 的值 傳遞來給 Mainprocess 並更新筆刷 將介面的示意圖刷新
        // 3 15 27 39
        /// <summary>
        /// 
        /// </summary>
        /// <param name="FPGA_B"></param>
        /// <param name="FPGA_A"></param>
        private void FPGA_Dictionary_2UIChangeEvent(Dictionary<int, ObservableCollection<int>> FPGA_B, Dictionary<int, ObservableCollection<int>> FPGA_A)
        {
            switch (mPaneladjustSwitch.ColorspaceFlag)
            {
                case "1024R":
                    UpdateBrushColors("R",1024, List_fillBrush, FPGA_B); // 更新左半邊
                    UpdateBrushColors("R", 1024, List_fillBrush, FPGA_A,16);
                    break;
                case "1024G":
                    UpdateBrushColors("G", 1024, List_fillBrush, FPGA_B); // 更新左半邊
                    UpdateBrushColors("G", 1024, List_fillBrush, FPGA_A, 16);
                    break;
                case "1024B":
                    UpdateBrushColors("B", 1024, List_fillBrush, FPGA_B); // 更新左半邊
                    UpdateBrushColors("B", 1024, List_fillBrush, FPGA_A, 16);
                    break;
                case "1024W":
                    UpdateBrushColors("W", 1024, List_fillBrush, FPGA_B); // 更新左半邊
                    UpdateBrushColors("W", 1024, List_fillBrush, FPGA_A, 16);
                    break;
                case "512R":
                    UpdateBrushColors("R", 512, List_fillBrush, FPGA_B); // 更新左半邊
                    UpdateBrushColors("R", 512, List_fillBrush, FPGA_A, 16);
                    break;
                case "512G":
                    UpdateBrushColors("G", 512, List_fillBrush, FPGA_B); // 更新左半邊
                    UpdateBrushColors("G", 512, List_fillBrush, FPGA_A, 16);
                    break;
                case "512B":
                    UpdateBrushColors("B", 512, List_fillBrush, FPGA_B); // 更新左半邊
                    UpdateBrushColors("B", 512, List_fillBrush, FPGA_A, 16);
                    break;
                case "512W":
                    UpdateBrushColors("W", 512, List_fillBrush, FPGA_B); // 更新左半邊
                    UpdateBrushColors("W", 512, List_fillBrush, FPGA_A, 16);
                    break;
                case "256R":
                    UpdateBrushColors("R", 256, List_fillBrush, FPGA_B); // 更新左半邊
                    UpdateBrushColors("R", 256, List_fillBrush, FPGA_A, 16);
                    break;
                case "256G":
                    UpdateBrushColors("G", 256, List_fillBrush, FPGA_B); // 更新左半邊
                    UpdateBrushColors("G", 256, List_fillBrush, FPGA_A, 16);
                    break;
                case "256B":
                    UpdateBrushColors("B", 256, List_fillBrush, FPGA_B); // 更新左半邊
                    UpdateBrushColors("B", 256, List_fillBrush, FPGA_A, 16);
                    break;
                case "256W":
                    UpdateBrushColors("W", 256, List_fillBrush, FPGA_B); // 更新左半邊
                    UpdateBrushColors("W", 256, List_fillBrush, FPGA_A, 16);
                    break;
                case "128R":
                    UpdateBrushColors("R", 128, List_fillBrush, FPGA_B); // 更新左半邊
                    UpdateBrushColors("R", 128, List_fillBrush, FPGA_A, 16);
                    break;
                case "128G":
                    UpdateBrushColors("G", 128, List_fillBrush, FPGA_B); // 更新左半邊
                    UpdateBrushColors("G", 128, List_fillBrush, FPGA_A, 16);
                    break;
                case "128B":
                    UpdateBrushColors("B", 128, List_fillBrush, FPGA_B); // 更新左半邊
                    UpdateBrushColors("B", 128, List_fillBrush, FPGA_A, 16);
                    break;
                case "128W":
                    UpdateBrushColors("W", 128, List_fillBrush, FPGA_B); // 更新左半邊
                    UpdateBrushColors("W", 128, List_fillBrush, FPGA_A, 16);
                    break;
            }
        }
        

        //---------------------------- 以下色差程式無用到---------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="svsender"></param>
        /// <param name="NumOfSplicing"></param>
        /// <param name="NumOfPanel"></param>
        private void initRegBoxMark(ref typNVSend[] svsender, byte NumOfSplicing, int NumOfPanel)
        {
            Array.Resize(ref svsender, NumOfSplicing);

            for (int svsen = 0; svsen < NumOfSplicing; svsen++)
            {
                svsender[svsen].regBoxMark = new string[NumOfPanel][];
                svsender[svsen].regPoCards = 0;
                for (int svpo = 0; svpo < NumOfPanel; svpo++)
                {
                    // 將Regbox 拓展 並填入初始矩陣
                    svsender[svsen].regBoxMark[svpo] = new string[23] { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0" };
                    for (int svpo2 = 0; svpo2 < 23; svpo2++)
                    {
                        // 將 Microusb 與 HDMI 的出入方向匯入至 Regbox中
                        switch (svpo2)
                        {
                            case 21:
                                svsender[svsen].regBoxMark[svpo][svpo2] = Microusb_SelectedItem.Num.ToString();
                                break;
                            case 22:
                                svsender[svsen].regBoxMark[svpo][svpo2] = HDMI_SelectedItem.Num.ToString();
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="svsender"></param>
        private void fillRegBoxMark(ref typNVSend[] svsender)
        {
            // MicroUSB 由右下進入
            // 第一片的X位置
            // column-1 * 960
            // 第一欄最高的ID
            // column * row /column
            // 每一欄中每個屏的Y
            // 1*540、2*540....Loop
            int count = 0;
            // 先填入 MicroUsb 所代表的值
            switch (Microusb_SelectedItem.Num)
            {
                // 【背面看】microUsb 從右下接入 
                case 1:
                    for (int i = 0; i < ColumnCount; i++)
                    {
                        for (int j = 0; j < RowCount; j++)
                        {
                            // 背面 X 座標 > 對應矩陣位置 5
                            svsender[0].regBoxMark[count][4] = (Math.Abs(ColumnCount - 1 - i) * srcW).ToString();
                            // 正面 X 座標 > 對應矩陣位置 9
                            svsender[0].regBoxMark[count][8] = (Math.Abs(i) * srcW).ToString();
                            // (i + 1) % 2 == 0 & i!=0 => 表示 當符合條件時USB由上到下串 else 是由下到上串
                            if ((i + 1) % 2 == 0 & i != 0)
                            {
                                // 由下往上座標Y > 對應矩陣位置 6
                                svsender[0].regBoxMark[count][5] = (Math.Abs(j) * srcH).ToString();
                                // 由下往上座標Y > 對應矩陣位置 10
                                svsender[0].regBoxMark[count][9] = (Math.Abs(RowCount - 1 - j) * srcH).ToString();

                            }
                            else
                            {
                                // 由下往上座標Y > 對應矩陣位置 6
                                svsender[0].regBoxMark[count][5] = (Math.Abs(RowCount - 1 - j) * srcH).ToString();
                                // 由下往上座標Y > 對應矩陣位置 10
                                svsender[0].regBoxMark[count][9] = (Math.Abs(j) * srcH).ToString();
                            }
                            // 單屏 W > 對應矩陣位置 7 
                            svsender[0].regBoxMark[count][6] = srcW.ToString();
                            // 單屏 H > 對應矩陣位置 8 
                            svsender[0].regBoxMark[count][7] = srcH.ToString();
                            count++;
                        }
                    }

                    break;
                // 【背面看】microUsb 從右上接入 
                case 2:
                    for (int i = 0; i < ColumnCount; i++)
                    {
                        for (int j = 0; j < RowCount; j++)
                        {
                            // 背面 X 座標 > 對應矩陣位置 5
                            svsender[0].regBoxMark[count][4] = (Math.Abs(ColumnCount - 1 - i) * srcW).ToString();
                            // 正面 X 座標 > 對應矩陣位置 9
                            svsender[0].regBoxMark[count][8] = (Math.Abs(i) * srcW).ToString();
                            // (i + 1) % 2 == 0 & i!=0 => 表示 當符合條件時USB由上到下串 else 是由下到上串
                            if ((i + 1) % 2 == 0 & i != 0)
                            {
                                // 由下往上座標Y > 對應矩陣位置 6
                                svsender[0].regBoxMark[count][5] = (Math.Abs(RowCount - 1 - j) * srcH).ToString();
                                // 由下往上座標Y > 對應矩陣位置 10
                                svsender[0].regBoxMark[count][9] = (Math.Abs(j) * srcH).ToString();
                            }
                            else
                            {
                                // 由下往上座標Y > 對應矩陣位置 6
                                svsender[0].regBoxMark[count][5] = (Math.Abs(j) * srcH).ToString();
                                // 由下往上座標Y > 對應矩陣位置 10
                                svsender[0].regBoxMark[count][9] = (Math.Abs(RowCount - 1 - j) * srcH).ToString();
                            }
                            // 單屏 W > 對應矩陣位置 7 
                            svsender[0].regBoxMark[count][6] = srcW.ToString();
                            // 單屏 H > 對應矩陣位置 8 
                            svsender[0].regBoxMark[count][7] = srcH.ToString();
                            count++;
                        }
                    }

                    break;
                // 【背面看】microUsb 從左上接入 
                case 3:
                    for (int i = 0; i < ColumnCount; i++)
                    {
                        for (int j = 0; j < RowCount; j++)
                        {
                            // 背面 X 座標 > 對應矩陣位置 5
                            svsender[0].regBoxMark[count][4] = (Math.Abs(i) * srcW).ToString();
                            // 正面 X 座標 > 對應矩陣位置 9
                            svsender[0].regBoxMark[count][8] = (Math.Abs(ColumnCount - 1 - i) * srcW).ToString();
                            // (i + 1) % 2 == 0 & i!=0 => 表示 當符合條件時USB由上到下串 else 是由下到上串
                            if ((i + 1) % 2 == 0 & i != 0)
                            {
                                // 由下往上座標Y > 對應矩陣位置 6
                                svsender[0].regBoxMark[count][5] = (Math.Abs(RowCount - 1 - j) * srcH).ToString();
                                // 由下往上座標Y > 對應矩陣位置 10
                                svsender[0].regBoxMark[count][9] = (Math.Abs(j) * srcH).ToString();
                            }
                            else
                            {
                                // 由下往上座標Y > 對應矩陣位置 6
                                svsender[0].regBoxMark[count][5] = (Math.Abs(j) * srcH).ToString();
                                // 由下往上座標Y > 對應矩陣位置 10
                                svsender[0].regBoxMark[count][9] = (Math.Abs(RowCount - 1 - j) * srcH).ToString();
                            }
                            // 單屏 W > 對應矩陣位置 7 
                            svsender[0].regBoxMark[count][6] = srcW.ToString();
                            // 單屏 H > 對應矩陣位置 8 
                            svsender[0].regBoxMark[count][7] = srcH.ToString();
                            count++;
                        }
                    }

                    break;
                // 【背面看】microUsb 從左下接入 
                case 4:
                    for (int i = 0; i < ColumnCount; i++)
                    {
                        for (int j = 0; j < RowCount; j++)
                        {
                            // 背面 X 座標 > 對應矩陣位置 5
                            svsender[0].regBoxMark[count][4] = (Math.Abs(i) * srcW).ToString();
                            // 正面 X 座標 > 對應矩陣位置 9
                            svsender[0].regBoxMark[count][8] = (Math.Abs(ColumnCount - 1 - i) * srcW).ToString();
                            // (i + 1) % 2 == 0 & i!=0 => 表示 當符合條件時USB由上到下串 else 是由下到上串
                            if ((i + 1) % 2 == 0 & i != 0)
                            {
                                // 由下往上座標Y > 對應矩陣位置 6
                                svsender[0].regBoxMark[count][5] = (Math.Abs(j) * srcH).ToString();
                                // 由下往上座標Y > 對應矩陣位置 10
                                svsender[0].regBoxMark[count][9] = (Math.Abs(RowCount - 1 - j) * srcH).ToString();
                            }
                            else
                            {
                                // 由下往上座標Y > 對應矩陣位置 6
                                svsender[0].regBoxMark[count][5] = (Math.Abs(RowCount - 1 - j) * srcH).ToString();
                                // 由下往上座標Y > 對應矩陣位置 10
                                svsender[0].regBoxMark[count][9] = (Math.Abs(j) * srcH).ToString();
                            }
                            // 單屏 W > 對應矩陣位置 7 
                            svsender[0].regBoxMark[count][6] = srcW.ToString();
                            // 單屏 H > 對應矩陣位置 8 
                            svsender[0].regBoxMark[count][7] = srcH.ToString();
                            count++;
                        }
                    }

                    break;
            }
            count = 0;
            int HDMI_MajNum = 1;
            int HDMI_MinNum = 1;
            int HDMI_Min_Sequence_number = 1;
            switch (HDMI_SelectedItem.Num)
            {
                // HDMI由下往上串
                case 1:
                    List<string> HDMI_Maj_Info = new List<string>();
                    for (int i = 0; i < ColumnCount; i++)
                    {
                        int rowCount = RowCount - 1;
                        for (int j = 0; j < RowCount; j++)

                        {
                            // HDMI 數量 > 對應矩陣位置 1
                            int factor = (RowCount > 4) ? ((RowCount + 3) / 4) : 1; // 計算倍數
                            svsender[0].regBoxMark[count][0] = (ColumnCount * factor).ToString();

                            if ((i + 1) % 2 == 0 & i != 0)
                            {
                                // HDMI 主編號 > 對應矩陣位置 2 // 每 4 個 Row 切換值\
                                // 因為是倒過來 所以 HDMI Maj 所以先將大的數字給值
                                svsender[0].regBoxMark[count + rowCount][1] = HDMI_MajNum.ToString();
                                // HDMI 主編號下的次編號 > 對應矩陣位置 3 
                                svsender[0].regBoxMark[count + rowCount][2] = HDMI_MinNum.ToString();

                                // HDMI 次編號順序 > 對應矩陣位置 4
                                svsender[0].regBoxMark[count + rowCount][3] = 2.ToString();
                                if ((j + 1) % 4 == 0)
                                {
                                    // HDMI 次編號順序 > 對應矩陣位置 4
                                    svsender[0].regBoxMark[count + rowCount][3] = 99.ToString();
                                }
                                if (j == RowCount - 1)
                                {
                                    // HDMI 次編號順序 > 對應矩陣位置 4
                                    svsender[0].regBoxMark[count + rowCount][3] = 99.ToString();
                                }
                                if (svsender[0].regBoxMark[count + rowCount][2] == "1")
                                {
                                    // HDMI 次編號順序 > 對應矩陣位置 4
                                    svsender[0].regBoxMark[count + rowCount][3] = 1.ToString();
                                }
                                HDMI_MinNum++;
                                rowCount = rowCount - 2;
                                if ((j + 1) % 4 == 0 & (j + 1) != RowCount)
                                {
                                    HDMI_MajNum++;
                                    HDMI_MinNum = 1;

                                }
                            }
                            else
                            {
                                // HDMI 主編號 > 對應矩陣位置 2 // 每 4 個 Row 切換值
                                svsender[0].regBoxMark[count][1] = HDMI_MajNum.ToString();
                                // HDMI 主編號下的次編號 > 對應矩陣位置 3 
                                svsender[0].regBoxMark[count][2] = HDMI_MinNum.ToString();

                                // HDMI 次編號順序 > 對應矩陣位置 4
                                svsender[0].regBoxMark[count][3] = 2.ToString();
                                if ((j + 1) % 4 == 0)
                                {
                                    svsender[0].regBoxMark[count][3] = 99.ToString();
                                }
                                if (j == RowCount - 1)
                                {
                                    // HDMI 次編號順序 > 對應矩陣位置 4
                                    svsender[0].regBoxMark[count][3] = 99.ToString();
                                }
                                if (svsender[0].regBoxMark[count][2] == "1")
                                {
                                    // HDMI 次編號順序 > 對應矩陣位置 4
                                    svsender[0].regBoxMark[count][3] = 1.ToString();
                                }
                                HDMI_MinNum++;
                                if ((j + 1) % 4 == 0 & (j + 1) != RowCount)
                                {
                                    HDMI_MajNum++;
                                    HDMI_MinNum = 1;

                                }

                            }
                            // 拼接屏 解析度 W > 對應矩陣位置 11 
                            svsender[0].regBoxMark[count][10] = (srcW).ToString();

                            count++;
                        }
                        HDMI_MajNum++;
                        HDMI_MinNum = 1;
                        HDMI_Min_Sequence_number = 1;
                    }
                    // 預計 RowCount * ColumnCount
                    int totalCount = RowCount * ColumnCount;

                    for (int b = 0; b < totalCount; b++)
                    {
                        HDMI_Maj_Info.Add(svsender[0].regBoxMark[b][1]);
                    }

                    // 使用 LINQ 分組並轉換為字典。 List 中各個數字的數量
                    var groupedNumbers = HDMI_Maj_Info
                        .GroupBy(n => n)
                        .ToDictionary(group => group.Key, group => group.Count() * 540);

                    // 將 解析度 H 的訊息填入 > 對應矩陣位置 12
                    for (int b = 0; b < totalCount; b++)
                    {
                        svsender[0].regBoxMark[b][11] = groupedNumbers[svsender[0].regBoxMark[b][1]].ToString();
                    }

                    // 秀出 矩陣內容
                    for (int k = 0; k < RowCount * ColumnCount; k++)
                    {
                        string showmsg = "";
                        foreach (var aa in svsender[0].regBoxMark[k])
                        {
                            showmsg = showmsg + "_" + aa;
                        }
                        Console.WriteLine(showmsg);

                    }
                    Console.WriteLine("=================================================================");

                    break;
                case 2:
                    for (int i = 0; i < ColumnCount; i++)
                    {
                        for (int j = 0; j < RowCount; j++)
                        {
                            // 拼接屏 解析度 W > 對應矩陣位置 11 
                            svsender[0].regBoxMark[count][10] = (srcW * ColumnCount).ToString();
                            // 拼接屏 解析度 H > 對應矩陣位置 12 
                            svsender[0].regBoxMark[count][11] = srcH.ToString();
                            count++;
                        }
                    }
                    break;
                case 3:
                    for (int i = 0; i < ColumnCount; i++)
                    {
                        for (int j = 0; j < RowCount; j++)
                        {
                            // 拼接屏 解析度 W > 對應矩陣位置 11 
                            svsender[0].regBoxMark[count][10] = (srcW).ToString();
                            // 拼接屏 解析度 H > 對應矩陣位置 12 
                            svsender[0].regBoxMark[count][11] = (srcH * RowCount).ToString();
                            count++;
                        }
                    }
                    break;
                case 4:
                    for (int i = 0; i < ColumnCount; i++)
                    {
                        for (int j = 0; j < RowCount; j++)
                        {
                            // 拼接屏 解析度 W > 對應矩陣位置 11 
                            svsender[0].regBoxMark[count][10] = (srcW * ColumnCount).ToString();
                            // 拼接屏 解析度 H > 對應矩陣位置 12 
                            svsender[0].regBoxMark[count][11] = srcH.ToString();
                            count++;
                        }
                    }
                    break;
            }
        }
    }
}
