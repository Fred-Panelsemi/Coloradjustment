using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PanelSemi_Coloradjustment
{
    internal partial class MainProcess
    {
        public void DrawBackSimulation_Action()
        {
            // 1格方格 為  250*141

            Bitmap Drawbp = new Bitmap(ColumnCount * 255, RowCount * 155);
            using (Graphics gra1 = Graphics.FromImage(Drawbp))
            {
                for (int SvC = 0; SvC < ColumnCount; SvC++)
                {
                    for (int SvR = 0; SvR < RowCount; SvR++)
                    {
                        int DrawX = SvC * 200 ;
                        int DrawY = SvR * 112 ;
                        gra1.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.FromArgb(255, 255, 255), 1), DrawX, DrawY, 200, 112);


                    }
                }
            }

            using (MemoryStream stream = new MemoryStream())
            {
                Drawbp.Save(stream, ImageFormat.Png); // 將 Bitmap 保存到內存流
                stream.Seek(0, SeekOrigin.Begin);

                // 將 Bitmap 轉換為 BitmapImage
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = stream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                BackSimulation = bitmapImage;
                OnPropertyChanged(nameof(BackSimulation));
            }
        }
    }
}
