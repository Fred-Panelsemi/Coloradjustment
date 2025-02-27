using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CtLib.Library.Wpf;
using PanelSemi_Coloradjustment.ViewModel;

namespace PanelSemi_Coloradjustment
{
    internal partial class MainProcess
    {
        // Declare the event
        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public string PanelSemi_SplicingVersion { get; }

        #region 固定數值
        public static int srcW = 960;
        public static int srcH = 540;
        #endregion


        #region UI object binding
        // 使用者選用拼接樣式
        private int mRowCount = 1;
        public int RowCount { get => mRowCount; set { mRowCount = value; FrontSimulation_Action(); OnPropertyChanged(); } }

        private int mColumnCount = 1;
        public int ColumnCount { get => mColumnCount; set { mColumnCount = value; FrontSimulation_Action(); OnPropertyChanged(); } }

        // 前後示意圖
        private BitmapImage mFrontSimulation;
        public BitmapImage FrontSimulation { get => mFrontSimulation; set { mFrontSimulation = value; OnPropertyChanged(); } }

        private BitmapImage mBackSimulation;
        public BitmapImage BackSimulation { get => mBackSimulation; set { mBackSimulation = value; OnPropertyChanged(); } }

        // USB HDMI combox 綁定物件
        public ObservableCollection<ComboBoxItemModel> Microusb_Items { get; set; }
        private ComboBoxItemModel mMicrousb_SelectedItem;
        public ComboBoxItemModel Microusb_SelectedItem { get => mMicrousb_SelectedItem; set { mMicrousb_SelectedItem = value; FrontSimulation_Action(); OnPropertyChanged(); } } // 綁定到選中的項目
        public ObservableCollection<ComboBoxItemModel> HDMI_Items { get; set; }
        private ComboBoxItemModel mHDMI_SelectedItem;
        public ComboBoxItemModel HDMI_SelectedItem { get => mHDMI_SelectedItem; set { mHDMI_SelectedItem = value; FrontSimulation_Action(); OnPropertyChanged(); } } // 綁定到選中的項目

        public ObservableCollection<string> Mode_Item { get; set; }
        private string mMode_SelectedItem;
        public string Mode_SelectedItem // 綁定到選中的項目
        {
            get => mMode_SelectedItem;
            set
            {
                mMode_SelectedItem = value;
                if (Mode_SelectedItem == "Total Tile")
                {
                    Is1x4 = false;
                    IsSingle = false;
                    if(Ix4checkBoxStates != null & SinglecheckBoxStates!= null)
                    {
                        for (int i = 0; i < Ix4checkBoxStates.Count; i++)
                        {
                            Ix4checkBoxStates[i] = false;
                        }
                        for (int i = 0; i < SinglecheckBoxStates.Count; i++)
                        {
                            SinglecheckBoxStates[i] = false;
                        }
                    }
                    
                }
                else if (Mode_SelectedItem == "1x4 Tile")
                {
                    Is1x4 = true;
                    IsSingle = false;
                    if (SinglecheckBoxStates != null)
                    {
                        for (int i = 0; i < SinglecheckBoxStates.Count; i++)
                        {
                            SinglecheckBoxStates[i] = false;
                        }
                    }
                    

                }
                else if (Mode_SelectedItem == "Single Tile")
                {
                    Is1x4 = true;
                    IsSingle = true;
                }
                OnPropertyChanged();
            }
        }

        // 顯示要RGB要調整色域的數值
        private int mAdjuistment_R_Value_Show = 0;
        public int Adjuistment_R_Value_Show
        {
            get => mAdjuistment_R_Value_Show;
            set
            {
                mAdjuistment_R_Value_Show = value;
                OnPropertyChanged();
            }
        }

        private int mAdjuistment_G_Value_Show = 0;
        public int Adjuistment_G_Value_Show
        {
            get => mAdjuistment_G_Value_Show;
            set
            {
                mAdjuistment_G_Value_Show = value;
                OnPropertyChanged();
            }
        }

        private int mAdjuistment_B_Value_Show = 0;
        public int Adjuistment_B_Value_Show
        {
            get => mAdjuistment_B_Value_Show;
            set
            {
                mAdjuistment_B_Value_Show = value;
                OnPropertyChanged();
            }
        }


        // 進入/離開色差模式
        private string mEnterOrExistColorMoode = "進入\r\n色差調節模式";
        public  string EnterOrExistColorMoode
        {
            get => mEnterOrExistColorMoode;
            set
            {
                mEnterOrExistColorMoode = value;
                OnPropertyChanged(nameof(EnterOrExistColorMoode));
            }
        }

        private Visibility mIsEnterColorAdjustmentMode = Visibility.Visible;
        public Visibility IsEnterColorAdjustmentMode
        {
            get => mIsEnterColorAdjustmentMode;
            set
            {
                mIsEnterColorAdjustmentMode = value;
                OnPropertyChanged(nameof(IsEnterColorAdjustmentMode));
            }
        }


        // 色域的Flag
        private int mColorspaceFlag = 4096;
        public int ColorspaceFlag
        {
            get => mColorspaceFlag;
            set
            {
                mColorspaceFlag = value;
                OnPropertyChanged();
            }
        }

        // 色域Checkbox溝選用
        private ObservableCollection<bool> mColorSpacecheckBoxStates;

        public ObservableCollection<bool> ColorSpaceCheckBoxStates
        {
            get => mColorSpacecheckBoxStates;
            set
            {
                mColorSpacecheckBoxStates = value;
                OnPropertyChanged(nameof(ColorSpaceCheckBoxStates));
            }
        }

        private ObservableCollection<bool> m1x4checkBoxStates;

        public ObservableCollection<bool> Ix4checkBoxStates
        {
            get => m1x4checkBoxStates;
            set
            {
                m1x4checkBoxStates = value;
                OnPropertyChanged(nameof(Ix4checkBoxStates));
            }
        }

        private ObservableCollection<bool> mSinglecheckBoxStates;

        public ObservableCollection<bool> SinglecheckBoxStates
        {
            get => mSinglecheckBoxStates;
            set
            {
                mSinglecheckBoxStates = value;
                OnPropertyChanged(nameof(SinglecheckBoxStates));
            }
        }

        // 顯示/隱藏 RGB 數值
        private Visibility mSpace4096 = Visibility.Hidden;
        public Visibility Space4096
        {
            get => mSpace4096;
            set
            {
                mSpace4096 = value;
                OnPropertyChanged(nameof(Space4096));
            }
        }
        // 顯示/隱藏 RGB 數值
        private Visibility mSpace2048 = Visibility.Hidden;
        public Visibility Space2048
        {
            get => mSpace2048;
            set
            {
                mSpace2048 = value;
                OnPropertyChanged(nameof(Space2048));
            }
        }
        // 顯示/隱藏 RGB 數值
        private Visibility mSpace1024 = Visibility.Hidden;
        public Visibility Space1024
        {
            get => mSpace1024;
            set
            {
                mSpace1024 = value;
                OnPropertyChanged(nameof(Space1024));
            }
        }
        // 顯示/隱藏 RGB 數值
        private Visibility mSpace512 = Visibility.Hidden;
        public Visibility Space512
        {
            get => mSpace512;
            set
            {
                mSpace512 = value;
                OnPropertyChanged(nameof(Space512));
            }
        }
        // 綁定在介面上顯示的 FPGA_B List
        private Dictionary<int, ObservableCollection<int>> mDataB;
        public Dictionary<int, ObservableCollection<int>> DataB
        {
            get => mDataB;
            set
            {
                mDataB = value;
                OnPropertyChanged(nameof(DataB));
            }
        }
        // 綁定在介面上顯示的 FPGA_A List
        private Dictionary<int, ObservableCollection<int>> mDataA;
        public Dictionary<int, ObservableCollection<int>> DataA
        {
            get => mDataA;
            set
            {
                mDataA = value;
                OnPropertyChanged(nameof(DataA));
            }
        }

        // 選擇

        // Mode XB Tile Chose 物件
        private bool mIsTotal = true;
        public bool IsTotal { get => mIsTotal; set { mIsTotal = value; OnPropertyChanged(); } }
        private bool mIs1x4 = false;
        public bool Is1x4 { get => mIs1x4; set { mIs1x4 = value; OnPropertyChanged(); } }
        private bool mIsSingle = false;
        public bool IsSingle { get => mIsSingle; set { mIsSingle = value; OnPropertyChanged(); } }

        // R G B 介面上綁定的數值
        private int mValueR = 1;
        public int ValueR { get => mValueR; set { mValueR = value; OnPropertyChanged(); } }

        private int mValueG = 1;
        public int ValueG { get => mValueG; set { mValueG = value; OnPropertyChanged(); } }

        private int mValueB = 1;
        public int ValueB { get => mValueB; set { mValueB = value; OnPropertyChanged(); } }

        //---
        public int HDMICount = 0;
        public int MicroUSBCount = 0;

        public class ComboBoxItemModel
        {
            public ImageSource Image { get; set; } // 圖片的路徑或 URI
            public string Text { get; set; }  // 文本內容
            public int Num { get; set; }
        }

        public int ChoicePanelID = 0;

        //
        public string[] regbookmark = new string[23] { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0" };


        //
        public typNVSend[] MainSender = new typNVSend[0];


        //
        private string mIsUsbConnect = "not connect";
        public string IsUsbConnect { get => mIsUsbConnect; set { mIsUsbConnect = value; OnPropertyChanged(); } }

        #endregion

        #region UI Command binding
        public ICommand FrontBackSimulation { get; set; }
        public ICommand StartandInit { get; set; }
        public ICommand IDChoice { get; set; }
        public ICommand USB_ComPort_FindandOpen { get; set; }
        public ICommand SaveColorInfo { get; set; }
        public ICommand RecoverDefaultValue { get; set; }
        public ICommand WindowClose { get; set; }
        public ICommand ID_On { get; set; }
        public ICommand ID_Off { get; set; }
        public ICommand MCU_Reset { get; set; }

        public DelegateCommand<string> AdjValueR { get; set; }
        public DelegateCommand<string> AdjValueG { get; set; }
        public DelegateCommand<string> AdjValueB { get; set; }
        #endregion

        #region 自定義控建

        private ObservableCollection<CheckBoxModel> mPanel_ID_CheckBoxes;
        public ObservableCollection<CheckBoxModel> Panel_ID_CheckBoxes { get => mPanel_ID_CheckBoxes; set { mPanel_ID_CheckBoxes = value; OnPropertyChanged(); } }
        #endregion

        #region MyRegion
        SolidColorBrush PanelBABrush = new SolidColorBrush();
        private SolidColorBrush _strRGB = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        public SolidColorBrush strRGB
        {
            get => _strRGB;
            private set
            {
                _strRGB = value;
                OnPropertyChanged(nameof(_strRGB));
            }
        }
        ObservableCollection<SolidColorBrush> mList_fillBrush = new ObservableCollection<SolidColorBrush>();
        public ObservableCollection<SolidColorBrush> List_fillBrush
        {
            get => mList_fillBrush;
            set
            {
                mList_fillBrush = value;
                OnPropertyChanged(nameof(List_fillBrush));
            }
        }
        // FPGA B
        //------------------------------XB1
        private SolidColorBrush _fillBrush_X1T1 = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        public SolidColorBrush FillBrushX1T1
        {
            get => _fillBrush_X1T1;
            private set
            {
                _fillBrush_X1T1 = value;
                OnPropertyChanged(nameof(FillBrushX1T1));
            }
        }
        private SolidColorBrush _fillBrush_X1T2 = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        public SolidColorBrush FillBrushX1T2
        {
            get => _fillBrush_X1T2;
            private set
            {
                _fillBrush_X1T2 = value;
                OnPropertyChanged(nameof(FillBrushX1T2));
            }
        }
        private SolidColorBrush _fillBrush_X1T3 = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        public SolidColorBrush FillBrushX1T3
        {
            get => _fillBrush_X1T3;
            private set
            {
                _fillBrush_X1T3 = value;
                OnPropertyChanged(nameof(FillBrushX1T3));
            }
        }
        private SolidColorBrush _fillBrush_X1T4 = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        public SolidColorBrush FillBrushX1T4
        {
            get => _fillBrush_X1T4;
            private set
            {
                _fillBrush_X1T4 = value;
                OnPropertyChanged(nameof(FillBrushX1T4));
            }
        }
        //---------------------------------XB2
        private SolidColorBrush _fillBrush_X2T1 = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        public SolidColorBrush FillBrushX2T1
        {
            get => _fillBrush_X2T1;
            private set
            {
                _fillBrush_X2T1 = value;
                OnPropertyChanged(nameof(FillBrushX2T1));
            }
        }
        private SolidColorBrush _fillBrush_X2T2 = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        public SolidColorBrush FillBrushX2T2
        {
            get => _fillBrush_X2T2;
            private set
            {
                _fillBrush_X2T2 = value;
                OnPropertyChanged(nameof(FillBrushX2T2));
            }
        }
        private SolidColorBrush _fillBrush_X2T3 = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        public SolidColorBrush FillBrushX2T3
        {
            get => _fillBrush_X2T3;
            private set
            {
                _fillBrush_X2T3 = value;
                OnPropertyChanged(nameof(FillBrushX2T3));
            }
        }
        private SolidColorBrush _fillBrush_X2T4 = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        public SolidColorBrush FillBrushX2T4
        {
            get => _fillBrush_X2T4;
            private set
            {
                _fillBrush_X2T4 = value;
                OnPropertyChanged(nameof(FillBrushX2T4));
            }
        }
        //---------------------------------XB3
        private SolidColorBrush _fillBrush_X3T1 = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        public SolidColorBrush FillBrushX3T1
        {
            get => _fillBrush_X3T1;
            private set
            {
                _fillBrush_X3T1 = value;
                OnPropertyChanged(nameof(FillBrushX3T1));
            }
        }
        private SolidColorBrush _fillBrush_X3T2 = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        public SolidColorBrush FillBrushX3T2
        {
            get => _fillBrush_X3T2;
            private set
            {
                _fillBrush_X3T2 = value;
                OnPropertyChanged(nameof(FillBrushX3T2));
            }
        }
        private SolidColorBrush _fillBrush_X3T3 = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        public SolidColorBrush FillBrushX3T3
        {
            get => _fillBrush_X3T3;
            private set
            {
                _fillBrush_X3T3 = value;
                OnPropertyChanged(nameof(FillBrushX3T3));
            }
        }
        private SolidColorBrush _fillBrush_X3T4 = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        public SolidColorBrush FillBrushX3T4
        {
            get => _fillBrush_X3T4;
            private set
            {
                _fillBrush_X3T4 = value;
                OnPropertyChanged(nameof(FillBrushX3T4));
            }
        }
        //---------------------------------XB4
        private SolidColorBrush _fillBrush_X4T1 = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        public SolidColorBrush FillBrushX4T1
        {
            get => _fillBrush_X4T1;
            private set
            {
                _fillBrush_X4T1 = value;
                OnPropertyChanged(nameof(FillBrushX4T1));
            }
        }
        private SolidColorBrush _fillBrush_X4T2 = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        public SolidColorBrush FillBrushX4T2
        {
            get => _fillBrush_X4T2;
            private set
            {
                _fillBrush_X4T2 = value;
                OnPropertyChanged(nameof(FillBrushX4T2));
            }
        }
        private SolidColorBrush _fillBrush_X4T3 = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        public SolidColorBrush FillBrushX4T3
        {
            get => _fillBrush_X4T3;
            private set
            {
                _fillBrush_X4T3 = value;
                OnPropertyChanged(nameof(FillBrushX4T3));
            }
        }
        private SolidColorBrush _fillBrush_X4T4 = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        public SolidColorBrush FillBrushX4T4
        {
            get => _fillBrush_X4T4;
            private set
            {
                _fillBrush_X4T4 = value;
                OnPropertyChanged(nameof(FillBrushX4T4));
            }
        }
        //===================
        // FPGA A 
        //---------------------------------XB1
        private SolidColorBrush A_fillBrush_X1T1 = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        public SolidColorBrush AFillBrushX1T1
        {
            get => A_fillBrush_X1T1;
            private set
            {
                A_fillBrush_X1T1 = value;
                OnPropertyChanged(nameof(AFillBrushX1T1));
            }
        }
        private SolidColorBrush A_fillBrush_X1T2 = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        public SolidColorBrush AFillBrushX1T2
        {
            get => A_fillBrush_X1T2;
            private set
            {
                A_fillBrush_X1T2 = value;
                OnPropertyChanged(nameof(AFillBrushX1T2));
            }
        }
        private SolidColorBrush A_fillBrush_X1T3 = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        public SolidColorBrush AFillBrushX1T3
        {
            get => A_fillBrush_X1T3;
            private set
            {
                A_fillBrush_X1T3 = value;
                OnPropertyChanged(nameof(AFillBrushX1T3));
            }
        }
        private SolidColorBrush A_fillBrush_X1T4 = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        public SolidColorBrush AFillBrushX1T4
        {
            get => A_fillBrush_X1T4;
            private set
            {
                A_fillBrush_X1T4 = value;
                OnPropertyChanged(nameof(AFillBrushX1T4));
            }
        }
        //---------------------------------XB2
        private SolidColorBrush A_fillBrush_X2T1 = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        public SolidColorBrush AFillBrushX2T1
        {
            get => A_fillBrush_X2T1;
            private set
            {
                A_fillBrush_X2T1 = value;
                OnPropertyChanged(nameof(AFillBrushX2T1));
            }
        }
        private SolidColorBrush A_fillBrush_X2T2 = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        public SolidColorBrush AFillBrushX2T2
        {
            get => A_fillBrush_X2T2;
            private set
            {
                A_fillBrush_X2T2 = value;
                OnPropertyChanged(nameof(AFillBrushX2T2));
            }
        }
        private SolidColorBrush A_fillBrush_X2T3 = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        public SolidColorBrush AFillBrushX2T3
        {
            get => A_fillBrush_X2T3;
            private set
            {
                A_fillBrush_X2T3 = value;
                OnPropertyChanged(nameof(AFillBrushX2T3));
            }
        }
        private SolidColorBrush A_fillBrush_X2T4 = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        public SolidColorBrush AFillBrushX2T4
        {
            get => A_fillBrush_X2T4;
            private set
            {
                A_fillBrush_X2T4 = value;
                OnPropertyChanged(nameof(AFillBrushX2T4));
            }
        }
        //---------------------------------XB3
        //
        private SolidColorBrush A_fillBrush_X3T1 = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        public SolidColorBrush AFillBrushX3T1
        {
            get => A_fillBrush_X3T1;
            private set
            {
                A_fillBrush_X3T1 = value;
                OnPropertyChanged(nameof(AFillBrushX3T1));
            }
        }
        private SolidColorBrush A_fillBrush_X3T2 = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        public SolidColorBrush AFillBrushX3T2
        {
            get => A_fillBrush_X3T2;
            private set
            {
                A_fillBrush_X3T2 = value;
                OnPropertyChanged(nameof(AFillBrushX3T2));
            }
        }
        private SolidColorBrush A_fillBrush_X3T3 = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        public SolidColorBrush AFillBrushX3T3
        {
            get => A_fillBrush_X3T3;
            private set
            {
                A_fillBrush_X3T3 = value;
                OnPropertyChanged(nameof(AFillBrushX3T3));
            }
        }
        private SolidColorBrush A_fillBrush_X3T4 = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        public SolidColorBrush AFillBrushX3T4
        {
            get => A_fillBrush_X3T4;
            private set
            {
                _fillBrush_X3T4 = value;
                OnPropertyChanged(nameof(AFillBrushX3T4));
            }
        }
        //---------------------------------XB4
        private SolidColorBrush A_fillBrush_X4T1 = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        public SolidColorBrush AFillBrushX4T1
        {
            get => A_fillBrush_X4T1;
            private set
            {
                A_fillBrush_X4T1 = value;
                OnPropertyChanged(nameof(AFillBrushX4T1));
            }
        }
        private SolidColorBrush A_fillBrush_X4T2 = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        public SolidColorBrush AFillBrushX4T2
        {
            get => A_fillBrush_X4T2;
            private set
            {
                A_fillBrush_X4T2 = value;
                OnPropertyChanged(nameof(AFillBrushX4T2));
            }
        }
        private SolidColorBrush A_fillBrush_X4T3 = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        public SolidColorBrush AFillBrushX4T3
        {
            get => A_fillBrush_X4T3;
            private set
            {
                A_fillBrush_X4T3 = value;
                OnPropertyChanged(nameof(AFillBrushX4T3));
            }
        }
        private SolidColorBrush A_fillBrush_X4T4 = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        public SolidColorBrush AFillBrushX4T4
        {
            get => A_fillBrush_X4T4;
            private set
            {
                A_fillBrush_X4T4 = value;
                OnPropertyChanged(nameof(AFillBrushX4T4));
            }
        }
        #endregion

        public struct typNVSend
        {
            #region regBoxMark 說明
            /*
            C12A  [0] 0未選,1from,2to,3end
                  [1] 發送卡 [2] 網口 [3] 接收卡 [4] 座標X [5] 座標Y [6] W [7] H [8] DrawX [9]DrawY [10]三角形的p1X [11]三角形的p1Y [12]三角形的p2X [13]三角形的p2Y [14]三角形的p3X [15]三角形的p3Y [16]BOXGAP [17]mcu ver [18]fpga ver
            */
            //public string[,] regBoxMark;      //[svpo, svcard] C12B
            //public ushort[] regPoCards;       //16個Port的連線點數
            /*
            Primary [0]
                    [1]HDMI主編號(由1起算)
                    [2]各組HDMI序號(由1起算) 
                    [3]1=Start,2=body,99=Tail,100=single 
                    [4]背面走線右側起算座標X 
                    [5]由下往上座標Y 
                    [6] W 
                    [7] H 
                    [8]正面走線左側起算座標X 
                    [9]由上往下座標Y 
                    [10]解析度W 
                    [11]解析度H 
                    [12]每個HDMI輸出內的X位置(背面) 
                    [13]每個HDMI輸出內的Y位置(背面) 
                    [14] 
                    [15]HDMI R 
                    [16]HDMI G 
                    [17]HDMI B
                    [18]verMCU 
                    [19]verFPGA
                    [20]verEDID
                    [21]in485
                    [22]inHDMI                                                                        背面走線座標X(從右到左走線)
            */
            public string[][] regBoxMark;     //[svpo] svpo=deviceID  svcard不使用
            #endregion regBoxMark
            public ushort regPoCards;
        }
    }
}
