using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
// Panel Semi C#/UI Standard Rules
// 最上層必須敘述該CS作用
// VM 的部分只有與介面綁定的物件可以使用Publish
// 需要標明 該Class 是屬於獨立還是 Partial，若是Partical 需標明是MVVM的哪層
// 每個Void皆需要有Summary
// Publish 物件需遵守大駝峰命名法   => Publish String PanelSemi
// Private 物件前綴需要有m 開頭大寫 => Private String mPanelSemi
// 方法內物件須遵守小駝峰式命名法     => int panelSemi
//================================================================================================================================================================================
namespace PanelSemi_Coloradjustment
{
    // PaneladjustSwitch 本體
    // 1. 【Init】
    //    => PaneladjustSwitch()
    // 2. 【Void】
    //    => PanelSwitch_update(string tileFlag, int fpgaCel, int xbNum, int tileNum,int colorGamut, string rgbFlag, int valueR, int valueG, int valueB)
    // 3. 【Void】
    //    => HandleTotalTile(int xbNum, int colorGamut, string rgbFlag, int valueR, int valueG, int valueB)
    // 4. 【Void】
    //    => Handle1x4Tile(int fpgaCel, int xbNum, int colorGamut, string rgbFlag, int valueR, int valueG, int valueB)
    // 5. 【Void】
    //    => HandleSingleTile(int fpgaCel, int xbNum, int tileNum, int colorGamut,string rgbFlag, int valueR, int valueG, int valueB)
    // 6. 【Void】
    //    => AssignFPGAValue(ref Dictionary<int, List<int>> fpga, int xbNum, int index, int value)
    // 7. 【Void】
    //    => AssignFPGAWhite(Dictionary<int, List<int>> fpga, int xbNum, int tileIndex,int colorGamut, int valueR, int valueG, int valueB)
    // 8. 【Dictionary<string, int>】
    //    => ColorOffsetMap 
    // 9. 【Void】
    //    => Init_Dictionary()
    // 10. 【Void】
    //     => Fill_Defaultvalue()


    //================================================================================================================================================================================
    // Class  >> PaneladjustSwitch
    internal class PaneladjustSwitch: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // 正面看控制左半邊的FPGA
        public Dictionary<int, ObservableCollection<int>> FPGA_B = new Dictionary<int, ObservableCollection<int>>();

        // 正面看控制右半邊的FPGA
        public Dictionary<int, ObservableCollection<int>> FPGA_A = new Dictionary<int, ObservableCollection<int>>();
        // 色域旗標
        public string ColorspaceFlag = "";
        public string ColorFalg = "";
        public int ColorspaceFlagnum = 0;
        public int SelectXbNum = 0;
        public int SelectTile = 0;
        public int SelectFPGA = 0;
        private bool mIsChangeMode = false;
        public PaneladjustSwitch()
        {
            
            Fill_Defaultvalue();
        }


        public delegate void FPGA_Dictionary_2UI(Dictionary<int, ObservableCollection<int>> FPGA_B, Dictionary<int, ObservableCollection<int>> FPGA_A);
        public event FPGA_Dictionary_2UI FPGA_Dictionary_2UIChangeEvent;
        public  void FPGA_Dictionary_2UIChange(Dictionary<int, ObservableCollection<int>> FPGA_B, Dictionary<int, ObservableCollection<int>> FPGA_A) => FPGA_Dictionary_2UIChangeEvent(FPGA_B, FPGA_A);



        /// <summary>
        /// 
        /// </summary>
        /// <param name="tileFlag">Total Tile,1x4 Tile,Single Tile</param>
        /// <param name="fpgaCel">0,1</param>
        /// <param name="xbNum">1,2,3,4</param>
        /// <param name="tileNum">0,1,2,3</param>
        /// <param name="colorGamut">512,1024,2048,4096</param>
        /// <param name="rgbFlag"></param>
        /// <param name="valueR"></param>
        /// <param name="valueG"></param>
        /// <param name="valueB"></param>
        public void PanelSwitch_update(string tileFlag, int fpgaCel, int xbNum, int tileNum,int colorGamut, string rgbFlag, int valueR, int valueG, int valueB)
        {
            if(valueR == 0 & valueG == 0 & valueB == 0)
            {
                mIsChangeMode = true;
            }
            else
            {
                mIsChangeMode = false;
            }
            switch (colorGamut)
            {
                case 512:
                    ColorspaceFlag = $"128{rgbFlag}";
                    colorGamut = 0;
                    break;
                case 1024:
                    ColorspaceFlag = $"256{rgbFlag}";
                    colorGamut = 1;
                    break;
                case 2048:
                    ColorspaceFlag = $"512{rgbFlag}";
                    colorGamut = 2;
                    break;
                case 4096:
                    ColorspaceFlag = $"1024{rgbFlag}";
                    colorGamut = 3;
                    break;
            }
            // 規範化 colorGamut
            //colorGamut = (colorGamut / 512) ;

            switch (tileFlag)
            {
                case "Total Tile":
                    HandleTotalTile(xbNum, colorGamut, rgbFlag, valueR, valueG, valueB);
                    break;

                case "1x4 Tile":
                    Handle1x4Tile(fpgaCel, xbNum, colorGamut, rgbFlag, valueR, valueG, valueB);
                    break;

                case "Single Tile":
                    HandleSingleTile(fpgaCel, xbNum, tileNum, colorGamut, rgbFlag, valueR, valueG, valueB);
                    break;
            }

            // 事件委派 將 FPGA_B & FPGA_A 的值傳入 Mainprocess 並更新畫布
            FPGA_Dictionary_2UIChange(FPGA_B,FPGA_A);


        }


        private void HandleTotalTile(int xbNum, int colorGamut, string rgbFlag, int valueR, int valueG, int valueB)
        {
            for (int j = 1; j < 5; j++)
            {
                if (rgbFlag == "W")
                {
                    for (int i = 0; i < 4; i++)
                    {
                        AssignFPGAWhite(FPGA_B, j, i, colorGamut, valueR, valueG, valueB);
                        AssignFPGAWhite(FPGA_A, j, i, colorGamut, valueR, valueG, valueB);
                    }
                }
                else if (ColorOffsetMap.ContainsKey(rgbFlag))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        AssignFPGAValue(ref FPGA_B, j, i * 12 + ColorOffsetMap[rgbFlag] + colorGamut, rgbFlag == "R" ? valueR : rgbFlag == "G" ? valueG : valueB);
                        AssignFPGAValue(ref FPGA_A, j, i * 12 + ColorOffsetMap[rgbFlag] + colorGamut, rgbFlag == "R" ? valueR : rgbFlag == "G" ? valueG : valueB);
                    }
                }
            }
        }

        private void Handle1x4Tile(int fpgaCel, int xbNum, int colorGamut, string rgbFlag, int valueR, int valueG, int valueB)
        {
            var targetFPGA = fpgaCel == 0 ? FPGA_B : FPGA_A;

            if (rgbFlag == "W")
            {
                for (int i = 0; i < 4; i++)
                {
                    AssignFPGAWhite(targetFPGA, xbNum, i, colorGamut, valueR, valueG, valueB);
                }
            }
            else if (ColorOffsetMap.ContainsKey(rgbFlag))
            {
                for (int i = 0; i < 4; i++)
                {
                    AssignFPGAValue(ref targetFPGA, xbNum, i * 12 + ColorOffsetMap[rgbFlag] + colorGamut, rgbFlag == "R" ? valueR : rgbFlag == "G" ? valueG : valueB);
                }
            }
        }

        private void HandleSingleTile(
            int fpgaCel, int xbNum, int tileNum, int colorGamut,
            string rgbFlag, int valueR, int valueG, int valueB)
        {
            var targetFPGA = fpgaCel == 0 ? FPGA_B : FPGA_A;

            if (rgbFlag == "W")
            {
                AssignFPGAWhite(targetFPGA, xbNum, tileNum, colorGamut, valueR, valueG, valueB);
            }
            else if (ColorOffsetMap.ContainsKey(rgbFlag))
            {
                AssignFPGAValue(ref targetFPGA, xbNum, tileNum * 12 + ColorOffsetMap[rgbFlag] + colorGamut,
                    rgbFlag == "R" ? valueR : rgbFlag == "G" ? valueG : valueB);
            }
        }

        private void AssignFPGAValue(ref Dictionary<int, ObservableCollection<int>> fpga, int xbNum, int index, int value)
        {
            if (mIsChangeMode)
            {
                fpga[xbNum][index] = fpga[xbNum][index];
            }
            else
            {
                if(fpga[xbNum][index] + value >= 4080)
                {
                    fpga[xbNum][index] = 4080;
                }
                else
                {
                    fpga[xbNum][index] = fpga[xbNum][index] + value;
                }
            }
        }

        private void AssignFPGAWhite(
            Dictionary<int, ObservableCollection<int>> fpga, int xbNum, int tileIndex,
            int colorGamut, int valueR, int valueG, int valueB)
        {
            AssignFPGAValue(ref fpga, xbNum, tileIndex * 12 + 0 + colorGamut, valueR); // R
            AssignFPGAValue(ref fpga, xbNum, tileIndex * 12 + 4 + colorGamut, valueG); // G
            AssignFPGAValue(ref fpga, xbNum, tileIndex * 12 + 8 + colorGamut, valueB); // B
        }

        // RGB 對應偏移量
        private readonly Dictionary<string, int> ColorOffsetMap = new Dictionary<string, int>
        {
            { "R", 0 },
            { "G", 4 },
            { "B", 8 }
        };

        /// <summary>
        /// 
        /// </summary>
        public void Init_Dictionary()
        {
            for (int i = 1; i < 5; i++)
            {
                FPGA_B.Add(i, new ObservableCollection<int>(defaultvalue_List_B));
                FPGA_A.Add(i, new ObservableCollection<int>( defaultvalue_List_A));
            }
        }
        /// <summary>
        /// 填充Default值
        /// </summary>
        private void Fill_Defaultvalue()
        {
            for (int i = 0; i < Colordefaulvalue_A.Length; i++)
            {
                Colordefaulvalue_A[i] = defaultvalue_A[i % 4]; // 按照 4 個值循環填入
                Colordefaulvalue_B[i] = defaultvalue_B[i % 4]; // 按照 4 個值循環填入
            }
            for (int i = 0; i < 48; i++)
            {
                defaultvalue_List_A.Add(new int[4] { 481, 963, 1927, 3840 }[i % 4]); // 按照 4 個值循環填入
                defaultvalue_List_B.Add(new int[4] { 481, 963, 1927, 3840 }[i % 4]); // 按照 4 個值循環填入
            }
            Init_Dictionary();
        }

        // X1_1 RRRR GGGG BBBB
        // ......
        // X4_4 RRRR GGGG BBBB
        // 1個FPGA 16*12 = 192 個數值 
        public static int[] Colordefaulvalue_A = new int[192];
        public static int[] defaultvalue_A = new int[4] { 481, 963, 1927, 3840 };
        public static List<int> defaultvalue_List_A = new List<int>();
        public static int[] Colordefaulvalue_B = new int[192];
        public static int[] defaultvalue_B = new int[4] { 481, 963, 1927, 3840 };
        public static List<int> defaultvalue_List_B = new List<int>();

    }
}
