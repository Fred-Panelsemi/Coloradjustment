using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    // Mainprocess 的 Partial 分身
    // 1. 【Void】 用於刷新 色差模擬器中 Rectangle Fill 筆刷
    //    => Void UpdateBrushColors()
    // 2. 【Void】 用於刷新 色差模擬器中 Rectangle Stroke 筆刷

    // FPGA_B FPGA_A 內容
    // 128    256    512    1024
    //  0      1      2       3
    //  R  --  R  --  R  --   R
    //  4      5      6       7
    //  G  --  G  --  G  --   G
    //  8      9     10      11
    //  B  --  B  --  B  --   B

    //================================================================================================================================================================================
    // Partial VM >> MainProcess
    internal partial class MainProcess
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="colorSpace"></param>
        /// <param name="listFillBrush"></param>
        /// <param name="fpgaData"></param>
        /// <param name="offset"></param>
        private void UpdateBrushColors(string channel, int colorSpace, ObservableCollection<SolidColorBrush> listFillBrush, Dictionary<int, ObservableCollection<int>> fpgaData, int offset = 0)
        {
            int loopCount = 0;
            int colorPt = 0;

            // 設定 colorPt 根據 colorSpace
            switch (colorSpace)
            {
                case 128:
                    colorPt = 0;
                    break;
                case 256:
                    colorPt = 1;
                    break;
                case 512:
                    colorPt = 2;
                    break;
                case 1024:
                    colorPt = 3;
                    break;
            }

            for (int i = 0; i < listFillBrush.Count / 2; i++)
            {
                // 每 4 個元素重新計算 loopCount 和 colorPt
                if (i % 4 == 0)
                {
                    loopCount++;
                    switch (colorSpace)
                    {
                        case 128:
                            colorPt = (colorSpace == 128) ? 0 : colorPt; // colorSpace 128 時保留 colorPt
                            break;
                        case 256:
                            colorPt = (colorSpace == 256) ? 1 : colorPt; // colorSpace 256 時保留 colorPt
                            break;
                        case 512:
                            colorPt = (colorSpace == 512) ? 2 : colorPt; // colorSpace 512 時保留 colorPt
                            break;
                        case 1024:
                            colorPt = (colorSpace == 1024) ? 3 : colorPt; // colorSpace 1024 時保留 colorPt
                            break;
                    }
                   
                }
                byte r = 0, g = 0, b = 0;

                // 設定 RGB 值
                switch (channel)
                {
                    case "R":
                        r = (byte)(fpgaData[loopCount][colorPt] /4 * 255 / 1024 );
                        break;
                    case "G":
                        
                        g = (byte)(fpgaData[loopCount][colorPt + 4] / 4 * 255 / 1024 );
                        break;
                    case "B":
                        b = (byte)(fpgaData[loopCount][colorPt + 8] / 4 * 255 / 1024 );
                        break;
                    case "W":
                        r = (byte)(fpgaData[loopCount][colorPt] / 4 * 255 / 1024 );
                        g = (byte)(fpgaData[loopCount][colorPt + 4] / 4 * 255 / 1024 );
                        b = (byte)(fpgaData[loopCount][colorPt + 8] / 4 * 255 / 1024 );
                        break;
                }

                // 更新顏色
                listFillBrush[i + offset].Color = System.Windows.Media.Color.FromRgb(r, g, b);

                // 增加 colorPt 偏移量
                colorPt += 12;
            }
        }
    }
}
