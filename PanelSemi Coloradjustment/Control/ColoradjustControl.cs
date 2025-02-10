using CtLib.Library;
using CtLib.Module.ChainDa.Dimmer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    // ColorControl 本體
    // 1. 【Init】 ColorControl 結構 Init
    //    => ColorControl(TotalProcess TLP, PaneladjustSwitch PAS, ColorControl CLT)
    // 2. 【Void】 開始進行調整色差時的初始化
    //    => ColorCtrInit()
    // 3. 【Void】 讀取選擇屏的色差資訊
    //    => ColorInfoRead()
    // 4. 【Void】 讀取當前有幾個屏幕需要調整色差
    //    => UpdateNumofPanel()
    // 5. 【Void】 變換色域 因為PG Mode 下 960 960 960 會直接覆蓋調色差 須先將色差資訊讀出來，再暫時寫入
    //    => ColrSpaceChange(int ColorspaceFlagnum, string ColorFalg)
    // 6. 【Void】 色差值暫時儲存
    //    => ColorTempSave()
    // 7. 【Void】 色差值寫進 MCU FLASH
    //    => ColorSave()
    // 8. 【Void】 調整紅色
    //    => AdjustR()
    // 9. 【Void】 調整綠色
    //    => AdjustG()
    // 10.【Void】 調整藍色
    //    => AdjustB() 
    // 11.【Void】 讀取全部屏的色差資訊
    //    => PGMODE_Value_Read()
    // 12.【Void】 填充Default值
    //    => Fill_Defaultvalue()

    //================================================================================================================================================================================
    // Partial C >> ColorControl
    internal partial class ColorControl
    {
        // Class TotalProcess 宣告
        private TotalProcess mTotalProcess;
        // Class TotalProcess 宣告
        private PaneladjustSwitch mPaneladjustSwitch;
        // 屏幕數量
        public int PanelConut = 0;
        // 選擇的ID
        public int ChoicePanelID = 0;
        // 是否進入色差調整模式
        public bool IsColorAdjustmentMode = false;
        // 讀取所有屏色差值的容器 FPGA_B Dictionary<屏幕 , IDDictionary<XB ID, List<色差資訊>>>
        Dictionary<int, Dictionary<int, ObservableCollection<int>>> FLASH_B_List;
        // 讀取所有屏色差值的容器 FPGA_A Dictionary<屏幕 , IDDictionary<XB ID, List<色差資訊>>>
        Dictionary<int, Dictionary<int, ObservableCollection<int>>> FLASH_A_List;

        /// <summary>
        /// class ColorControl 結構初始化
        /// </summary>
        /// <param name="TLP"></param>
        /// <param name="PAS"></param>
        /// <param name="CLT"></param>
        public ColorControl(TotalProcess TLP, PaneladjustSwitch PAS, ColorControl CLT)
        {
            mTotalProcess = TLP;
            mPaneladjustSwitch = PAS;
            Fill_Defaultvalue();
        }

        /// <summary>
        /// 開始進行調整色差時的初始化
        /// </summary>
        public void ColorCtrInit()
        {
            /* 讀取全部屏的色差資訊 */
            PGMODE_Value_Read();
            /* ENG_GAM ON */
            mTotalProcess.cENGGMAONWRITEp(0, 0, 160, Convert.ToByte(1), Convert.ToByte(2), mPaneladjustSwitch.FPGA_B, mPaneladjustSwitch.FPGA_A);
            /* 進入PG MODE */
            mTotalProcess.cPGRGB10BITp(255, 0, 0, 0, 0, "-1", "-1", "-1");
            /* 讀取當前選定ID屏的色差資訊 */
            ColorInfoRead();
            /* 送PG Mode 1020 白 */
            ColrSpaceChange(4096, "W");

        }

        /// <summary>
        /// 讀取選擇屏的色差資訊
        /// </summary>
        public void ColorInfoRead()
        {
            /* 設置需要讀取屏幕的ID */
            string svdeviceID = mTotalProcess.deviceID;
            mTotalProcess.deviceID = mTotalProcess.deviceID.Substring(0, 2) + mTotalProcess.DecToHex(ChoicePanelID, 2);
            /* 將 lb 改為 MCU_FLASH_R62000 */
            mTotalProcess.lblCmd = "MCU_FLASH_R62000";
            /* 呼叫MCU Flash 讀取函數 */
            mTotalProcess.mhMCUFLASHREAD("00062000", 8192);
            /* 宣告旗標 */
            int coUnt = 0;
            int coUntXb = 1;
            /* 迴圈讀取 ReadDataBuffer中的數值 372~1137 為 FPGA A 的色差數值 */
            for (int i = 372; i < 1137; i = i + 4)
            {
                mPaneladjustSwitch.FPGA_A[coUntXb][coUnt] = mTotalProcess.ReadDataBuffer[i + 1] * 256 + mTotalProcess.ReadDataBuffer[i + 2];
                /* 一塊Tile的色差值是 12個，一片XB有四片Tile 所以 色差數值有 48 個 >> 所以位置為 0~47*/
                /* 所以47的位置填完後 要將 coUntXb 的值+1，coUnt 歸0 */
                if (coUnt == 47)     /* RRRR */
                {                    /* GGGG */
                    coUntXb++;       /* BBBB */  
                    coUnt = 0;
                }
                else
                {
                    coUnt++;
                }
            }
            /* 旗標重製 */
            coUnt = 0;
            coUntXb = 1;
            /* 迴圈讀取 ReadDataBuffer中的數值 372~1137 為 FPGA B 的色差數值 */
            for (int i = 4468; i < 5233; i = i + 4)
            {
                mPaneladjustSwitch.FPGA_B[coUntXb][coUnt] = mTotalProcess.ReadDataBuffer[i + 1] * 256 + mTotalProcess.ReadDataBuffer[i + 2];
                /* 一塊Tile的色差值是 12個，一片XB有四片Tile 所以 色差數值有 48 個 >> 所以位置為 0~47*/
                /* 所以47的位置填完後 要將 coUntXb 的值+1，coUnt 歸0 */
                if (coUnt == 47)
                {
                    coUntXb++;
                    coUnt = 0;
                }
                else
                {
                    coUnt++;
                }
            }
        }

        /// <summary>
        /// 讀取當前有幾個屏幕需要調整色差
        /// </summary>
        public void UpdateNumofPanel()
        {
            mTotalProcess.cBOXREAD(ref PanelConut);
        }

        /// <summary>
        /// 變換色域 因為PG Mode 下 960 960 960 會直接覆蓋調色差 須先將色差資訊讀出來
        /// 再暫時寫入
        /// </summary>
        public void ColrSpaceChange(int ColorspaceFlagnum, string ColorFalg)
        {
            if (ColorspaceFlagnum == 4096) { ColorspaceFlagnum = 4080; }
            int valueR = 0;
            int valueG = 0;
            int valueB = 0;
            switch (ColorFalg)
            {
                case "R":
                    valueR = ColorspaceFlagnum / 4;
                    break;
                case "G":
                    valueG = ColorspaceFlagnum / 4;
                    break;
                case "B":
                    valueB = ColorspaceFlagnum / 4;
                    break;
                case "W":
                    valueR = ColorspaceFlagnum / 4;
                    valueG = ColorspaceFlagnum / 4;
                    valueB = ColorspaceFlagnum / 4;
                    break;
            }
            if(IsColorAdjustmentMode == true)
            {
                for (int i = 0; i < PanelConut; i++)
                {
                    /* 送白畫面 1020 階 */
                    mTotalProcess.cPGRGB10BITp(2, 0, 0, Convert.ToByte(i + 1), 2, valueR.ToString(), valueG.ToString(), valueB.ToString());
                    /* 因為送PG Mode 白畫面 會覆蓋色差 先將原本的色差在寫入 */
                    mTotalProcess.cENGGMAONWRITEp(1, 0, 160, Convert.ToByte(i + 1), Convert.ToByte(2), FLASH_B_List[i + 1], FLASH_A_List[i + 1]);
                }
            }
        }

        /// <summary>
        /// 色差值暫時儲存
        /// </summary>
        public void ColorTempSave()
        {
            /* 這個是暫時存 */
            mTotalProcess.cENGGMAONWRITEp(1, 0, 160, Convert.ToByte(ChoicePanelID), Convert.ToByte(2), mPaneladjustSwitch.FPGA_B, mPaneladjustSwitch.FPGA_A);
        }

        /// <summary>
        /// 色差值寫進 MCU FLASH
        /// </summary>
        public void ColorSave()
        {
            /* 這個是寫死進去 */
            mTotalProcess.cENGGMAONWRITEp(2, 0, 160, Convert.ToByte(ChoicePanelID), Convert.ToByte(2), mPaneladjustSwitch.FPGA_B, mPaneladjustSwitch.FPGA_A);
        }

        /// <summary>
        /// 調整紅色
        /// </summary>
        /// <param name="paRa">看是 up 還是 dwn</param>
        /// <param name="Mode">是整屏?條屏?單Tile?</param>
        /// <param name="adjValueR">調整的數值</param>
        public void AdjustR(string paRa, string Mode, int adjValueR)
        {
            switch (paRa)
            {
                case "up":
                    mPaneladjustSwitch.PanelSwitch_update(Mode, mPaneladjustSwitch.SelectFPGA, mPaneladjustSwitch.SelectXbNum, mPaneladjustSwitch.SelectTile, mPaneladjustSwitch.ColorspaceFlagnum, mPaneladjustSwitch.ColorFalg,
                        adjValueR, 0, 0);
                    break;
                case "dwn":
                    mPaneladjustSwitch.PanelSwitch_update(Mode, mPaneladjustSwitch.SelectFPGA, mPaneladjustSwitch.SelectXbNum, mPaneladjustSwitch.SelectTile, mPaneladjustSwitch.ColorspaceFlagnum, mPaneladjustSwitch.ColorFalg,
                      (-adjValueR), 0, 0);
                    break;
            }
            /* 這個是暫時存 */
            ColorTempSave();
        }

        /// <summary>
        /// 調整綠色
        /// </summary>
        /// <param name="paRa">看是 up 還是 dwn</param>
        /// <param name="Mode">是整屏?條屏?單Tile?</param>
        /// <param name="adjValueG">調整的數值</param>
        public void AdjustG(string paRa, string Mode, int adjValueG)
        {
            switch (paRa)
            {
                case "up":
                    mPaneladjustSwitch.PanelSwitch_update(Mode, mPaneladjustSwitch.SelectFPGA, mPaneladjustSwitch.SelectXbNum, mPaneladjustSwitch.SelectTile, mPaneladjustSwitch.ColorspaceFlagnum, mPaneladjustSwitch.ColorFalg,
                        0, adjValueG, 0);
                    break;
                case "dwn":
                    mPaneladjustSwitch.PanelSwitch_update(Mode, mPaneladjustSwitch.SelectFPGA, mPaneladjustSwitch.SelectXbNum, mPaneladjustSwitch.SelectTile, mPaneladjustSwitch.ColorspaceFlagnum, mPaneladjustSwitch.ColorFalg,
                        0, (-adjValueG), 0);
                    break;
            }
            /* 這個是暫時存 */
            ColorTempSave();
        }

        /// <summary>
        /// 調整藍色
        /// </summary>
        /// <param name="paRa">看是 up 還是 dwn</param>
        /// <param name="Mode">是整屏?條屏?單Tile?</param>
        /// <param name="adjValueB">調整的數值</param>
        public void AdjustB(string paRa, string Mode, int adjValueB)
        {
            switch (paRa)
            {
                case "up":
                    mPaneladjustSwitch.PanelSwitch_update(Mode, mPaneladjustSwitch.SelectFPGA, mPaneladjustSwitch.SelectXbNum, mPaneladjustSwitch.SelectTile, mPaneladjustSwitch.ColorspaceFlagnum, mPaneladjustSwitch.ColorFalg,
                        0, 0, adjValueB);
                    break;
                case "dwn":
                    mPaneladjustSwitch.PanelSwitch_update(Mode, mPaneladjustSwitch.SelectFPGA, mPaneladjustSwitch.SelectXbNum, mPaneladjustSwitch.SelectTile, mPaneladjustSwitch.ColorspaceFlagnum, mPaneladjustSwitch.ColorFalg,
                        0, 0, (-adjValueB));
                    break;
            }
            /* 這個是暫時存 */
            ColorTempSave();
        }

        
        /// <summary>
        /// 讀取全部屏的色差資訊
        /// </summary>
        public void PGMODE_Value_Read()
        {
            /* 重製 */
            FLASH_B_List = new Dictionary<int, Dictionary<int, ObservableCollection<int>>>();
            FLASH_A_List = new Dictionary<int, Dictionary<int, ObservableCollection<int>>>();
            /* 初始化字典Step1 */
            for (int i = 0; i < PanelConut; i++)
            {
                FLASH_B_List.Add(i + 1, new Dictionary<int, ObservableCollection<int>>());
                FLASH_A_List.Add(i + 1, new Dictionary<int, ObservableCollection<int>>());
            }
            /* 初始化字典Step2 */
            for (int i = 0; i < PanelConut; i++)
            {
                for (int j = 1; j < 5; j++)
                {
                    FLASH_B_List[i + 1].Add(j, new ObservableCollection<int>(defaultvalue_List_B));
                    FLASH_A_List[i + 1].Add(j, new ObservableCollection<int>(defaultvalue_List_A));
                }
            }
            /* 宣告旗標 */
            int coUnt = 0;
            int coUntXb = 1;
            /* 讀取 FLASH 中的資料 */
            for (int i = 0; i < PanelConut; i++)
            {
                /* 設置需要讀取屏幕的ID */
                string svdeviceID = mTotalProcess.deviceID;
                mTotalProcess.deviceID = mTotalProcess.deviceID.Substring(0, 2) + mTotalProcess.DecToHex((i + 1), 2);
                /* 將 lb 改為 MCU_FLASH_R62000 */
                mTotalProcess.lblCmd = "MCU_FLASH_R62000";
                /* 呼叫MCU Flash 讀取函數 */
                mTotalProcess.mhMCUFLASHREAD("00062000", 8192);
                /* 旗標重製 */
                coUnt = 0;
                coUntXb = 1;
                /* 迴圈讀取 ReadDataBuffer中的數值 372~1137 為 FPGA A 的色差數值 */
                for (int j = 372; j < 1137; j = j + 4)
                {
                    FLASH_A_List[i + 1][coUntXb][coUnt] = (mTotalProcess.ReadDataBuffer[j + 1] * 256 + mTotalProcess.ReadDataBuffer[j + 2]);
                    /* 一塊Tile的色差值是 12個，一片XB有四片Tile 所以 色差數值有 48 個 >> 所以位置為 0~47*/
                    /* 所以47的位置填完後 要將 coUntXb 的值+1，coUnt 歸0 */
                    if (coUnt == 47)
                    {
                        coUntXb++;
                        coUnt = 0;
                    }
                    else
                    {
                        coUnt++;
                    }
                }
                /* 旗標重製 */
                coUnt = 0;
                coUntXb = 1;
                /* 迴圈讀取 ReadDataBuffer中的數值 372~1137 為 FPGA B 的色差數值 */
                for (int k = 4468; k < 5233; k = k + 4)
                {
                    FLASH_B_List[i + 1][coUntXb][coUnt] = (mTotalProcess.ReadDataBuffer[k + 1] * 256 + mTotalProcess.ReadDataBuffer[k + 2]);
                    /* 一塊Tile的色差值是 12個，一片XB有四片Tile 所以 色差數值有 48 個 >> 所以位置為 0~47*/
                    /* 所以47的位置填完後 要將 coUntXb 的值+1，coUnt 歸0 */
                    if (coUnt == 47)
                    {
                        coUntXb++;
                        coUnt = 0;
                    }
                    else
                    {
                        coUnt++;
                    }
                }
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
                defaultvalue_List_A.Add(new int[4] { 512, 1024, 2048, 4080 }[i % 4]); // 按照 4 個值循環填入
                defaultvalue_List_B.Add(new int[4] { 512, 1024, 2048, 4080 }[i % 4]); // 按照 4 個值循環填入
            }

        }

        // X1_1 RRRR GGGG BBBB
        // ......
        // X4_4 RRRR GGGG BBBB
        // 1個FPGA 16*12 = 192 個數值 
        public static int[] Colordefaulvalue_A = new int[192];
        public static int[] defaultvalue_A = new int[4] { 512, 1024, 2048, 4080 };
        public static List<int> defaultvalue_List_A = new List<int>();
        public static int[] Colordefaulvalue_B = new int[192];
        public static int[] defaultvalue_B = new int[4] { 512, 1024, 2048, 4080 };
        public static List<int> defaultvalue_List_B = new List<int>();



    }
}
