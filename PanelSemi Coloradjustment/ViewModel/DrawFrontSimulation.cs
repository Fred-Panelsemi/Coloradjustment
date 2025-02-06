using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PanelSemi_Coloradjustment.Properties;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace PanelSemi_Coloradjustment
{
    internal partial class MainProcess
    {

        

        /// <summary>
        /// 
        /// </summary>
        private void FrontSimulation_Action()
        {
            DrawBackSimulation_Action();
            if (HDMI_SelectedItem != null & Microusb_SelectedItem != null)
            {
                initRegBoxMark(ref MainSender, 1, ColumnCount * RowCount);
                fillRegBoxMark(ref MainSender);
            }
               
            //ConnecterSwitcher(ref MicroUSBCount, ref HDMICount);
            //initRegBoxMark(ref MainSender, 1, ColumnCount * RowCount);
            Bitmap _bitmap = new Bitmap(ColumnCount * 255, RowCount * 155);
            int OnceWidth = 0;
            int OnceHeight = 0;
            OnceWidth = 200;
            OnceHeight = 112;
            int svpL = (int)(OnceHeight * 0.2);
            System.Drawing.Pen pL = new System.Drawing.Pen(System.Drawing.Color.FromArgb(255, 255, 255), svpL / 2);
            using (Graphics gra1 = Graphics.FromImage(_bitmap))
            {
                for (int SvC = 0; SvC < ColumnCount; SvC++)
                {
                    for (int SvR = 0; SvR < RowCount; SvR++)
                    {
                        int DrawX = SvC * OnceWidth + 2;
                        int DrawY = SvR * OnceHeight + 2;
                        int DrawW = OnceWidth - 4;
                        int DrawH = OnceHeight - 4;
                        int DrawX2 = SvC * OnceWidth + OnceWidth;

                        if (HDMI_SelectedItem != null)
                        {
                            // 畫HDMI的位置
                            switch (HDMI_SelectedItem.Num)
                            {
                                case 1:
                                    if (SvR == RowCount - 1)
                                    {
                                        gra1.FillEllipse(System.Drawing.Brushes.Blue, (int)(DrawX + DrawW / 3 * 2), (int)(DrawY + DrawH / 4 * 3), (int)(DrawW / 10), (int)(DrawW / 10));
                                        gra1.FillEllipse(System.Drawing.Brushes.White, (int)(DrawX + DrawW / 3 * 2 + (int)(DrawW / 10 / 4)), (int)(DrawY + DrawH / 4 * 3 + (int)(DrawW / 10 / 4)), (int)(DrawW / 10 - 2 * (int)(DrawW / 10 / 4)), (int)(DrawW / 10 - 2 * (int)(DrawW / 10 / 4)));
                                    }
                                    break;
                                case 2:
                                    if (SvC == ColumnCount - 1)
                                    {
                                        gra1.FillEllipse(System.Drawing.Brushes.Blue, (int)(DrawX + DrawW / 3 * 2), (int)(DrawY + DrawH / 4 * 3), (int)(DrawW / 10), (int)(DrawW / 10));
                                        gra1.FillEllipse(System.Drawing.Brushes.White, (int)(DrawX + DrawW / 3 * 2 + (int)(DrawW / 10 / 4)), (int)(DrawY + DrawH / 4 * 3 + (int)(DrawW / 10 / 4)), (int)(DrawW / 10 - 2 * (int)(DrawW / 10 / 4)), (int)(DrawW / 10 - 2 * (int)(DrawW / 10 / 4)));

                                    }
                                    break;
                                case 3:
                                    if (SvR == 0)
                                    {
                                        gra1.FillEllipse(System.Drawing.Brushes.Blue, (int)(DrawX + DrawW / 3 * 2), (int)(DrawY + DrawH / 4 * 3), (int)(DrawW / 10), (int)(DrawW / 10));
                                        gra1.FillEllipse(System.Drawing.Brushes.White, (int)(DrawX + DrawW / 3 * 2 + (int)(DrawW / 10 / 4)), (int)(DrawY + DrawH / 4 * 3 + (int)(DrawW / 10 / 4)), (int)(DrawW / 10 - 2 * (int)(DrawW / 10 / 4)), (int)(DrawW / 10 - 2 * (int)(DrawW / 10 / 4)));

                                    }
                                    break;
                                case 4:
                                    if (SvC == 0)
                                    {
                                        gra1.FillEllipse(System.Drawing.Brushes.Blue, (int)(DrawX + DrawW / 3 * 2), (int)(DrawY + DrawH / 4 * 3), (int)(DrawW / 10), (int)(DrawW / 10));
                                        gra1.FillEllipse(System.Drawing.Brushes.White, (int)(DrawX + DrawW / 3 * 2 + (int)(DrawW / 10 / 4)), (int)(DrawY + DrawH / 4 * 3 + (int)(DrawW / 10 / 4)), (int)(DrawW / 10 - 2 * (int)(DrawW / 10 / 4)), (int)(DrawW / 10 - 2 * (int)(DrawW / 10 / 4)));

                                    }
                                    break;
                            }
                        }
                        if (Microusb_SelectedItem != null)
                        {
                            // 畫Microusb的位置
                            switch (Microusb_SelectedItem.Num)
                            {
                                case 1:
                                    if (SvR == RowCount - 1 && SvC == 0)
                                    {
                                        gra1.FillEllipse(System.Drawing.Brushes.Red, (int)(DrawX + DrawW / 5 * 4), (int)(DrawY + DrawH / 4 * 3), (int)(DrawW / 10), (int)(DrawW / 10));
                                        gra1.FillEllipse(System.Drawing.Brushes.White, (int)(DrawX + DrawW / 5 * 4 + (int)(DrawW / 10 / 4)), (int)(DrawY + DrawH / 4 * 3 + (int)(DrawW / 10 / 4)), (int)(DrawW / 10 - 2 * (int)(DrawW / 10 / 4)), (int)(DrawW / 10 - 2 * (int)(DrawW / 10 / 4)));
                                    }
                                    break;
                                case 2:
                                    if (SvR == 0 && SvC == 0)
                                    {
                                        gra1.FillEllipse(System.Drawing.Brushes.Red, (int)(DrawX + DrawW / 5 * 4), (int)(DrawY + DrawH / 4 * 3), (int)(DrawW / 10), (int)(DrawW / 10));
                                        gra1.FillEllipse(System.Drawing.Brushes.White, (int)(DrawX + DrawW / 5 * 4 + (int)(DrawW / 10 / 4)), (int)(DrawY + DrawH / 4 * 3 + (int)(DrawW / 10 / 4)), (int)(DrawW / 10 - 2 * (int)(DrawW / 10 / 4)), (int)(DrawW / 10 - 2 * (int)(DrawW / 10 / 4)));
                                    }
                                    break;
                                case 3:
                                    if (SvR == 0 && SvC == ColumnCount - 1)
                                    {
                                        gra1.FillEllipse(System.Drawing.Brushes.Red, (int)(DrawX + DrawW / 5 * 4), (int)(DrawY + DrawH / 4 * 3), (int)(DrawW / 10), (int)(DrawW / 10));
                                        gra1.FillEllipse(System.Drawing.Brushes.White, (int)(DrawX + DrawW / 5 * 4 + (int)(DrawW / 10 / 4)), (int)(DrawY + DrawH / 4 * 3 + (int)(DrawW / 10 / 4)), (int)(DrawW / 10 - 2 * (int)(DrawW / 10 / 4)), (int)(DrawW / 10 - 2 * (int)(DrawW / 10 / 4)));
                                    }
                                    break;
                                case 4:
                                    if (SvR == RowCount - 1 && SvC == ColumnCount - 1)
                                    {
                                        gra1.FillEllipse(System.Drawing.Brushes.Red, (int)(DrawX + DrawW / 5 * 4), (int)(DrawY + DrawH / 4 * 3), (int)(DrawW / 10), (int)(DrawW / 10));
                                        gra1.FillEllipse(System.Drawing.Brushes.White, (int)(DrawX + DrawW / 5 * 4 + (int)(DrawW / 10 / 4)), (int)(DrawY + DrawH / 4 * 3 + (int)(DrawW / 10 / 4)), (int)(DrawW / 10 - 2 * (int)(DrawW / 10 / 4)), (int)(DrawW / 10 - 2 * (int)(DrawW / 10 / 4)));
                                    }
                                    break;
                            }
                        }

                        //gra1.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.FromArgb(255, 255, 255)), 2, 2, 198, 98);
                        gra1.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.FromArgb(255, 255, 255), 1), DrawX, DrawY, DrawW, DrawH);
                        gra1.DrawLine(pL, DrawX, DrawY + (int)(svpL / 4), DrawX2 - 2, DrawY + (int)(svpL / 4));
                        Image img = Resources.Panelsemi_blue;
                        int imgFitwidth = (int)(img.Width * ((float)DrawW / 1000) / 2);
                        int imgFitheight = (int)(img.Height * ((float)DrawH / 900) / 2);
                        Rectangle compress = new Rectangle(DrawX + DrawW / 2 - imgFitwidth, DrawY + DrawH / 2 - imgFitheight, (int)(img.Width * ((float)DrawW / 1000)), (int)(img.Height * ((float)DrawH / 700)));
                        //Rectangle compress = new Rectangle(DrawX , DrawY , (int)(img.Width * ((float)DrawW / 1000)), (int)(img.Height * ((float)DrawH / 950)));

                        gra1.DrawImage(img, compress);
                        img.Dispose();
                    }
                }
            }
            //_bitmap.Save("551.jpg");
            pL.Dispose();
            using (MemoryStream stream = new MemoryStream())
            {
                _bitmap.Save(stream, ImageFormat.Png); // 將 Bitmap 保存到內存流
                stream.Seek(0, SeekOrigin.Begin);

                // 將 Bitmap 轉換為 BitmapImage
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = stream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                FrontSimulation = bitmapImage;
                OnPropertyChanged(nameof(FrontSimulation));
            }
        }

        // 方法：將 Bitmap 轉換為 ImageSource
        private ImageSource ConvertToImageSource(Bitmap bitmap)
        {
            var hBitmap = bitmap.GetHbitmap(); // 獲取 HBitmap
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(hBitmap); // 釋放 HBitmap
            }
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
    }
}
