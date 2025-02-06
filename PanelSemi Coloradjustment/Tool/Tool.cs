using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Windows.Forms;

namespace PanelSemi_Coloradjustment
{
    internal partial class TotalProcess
    {

        public bool flgSend = false;
        public bool flgReceived = false;
        public bool Break = false;

        /// <summary>
        /// 等待【非同步 讓UI還可以更新】
        /// </summary>
        /// <param name="SvCountMax"></param>
        public  void Wait(int SvCountMax)
        {
            int Svi;
            // 限制 SvCountMax 的範圍為 0 到 32766，如果超過或小於則重置為 32766
            if (SvCountMax > 32766 || SvCountMax < 0) { SvCountMax = 32766; }
            // 初始化 Svi 為 1
            Svi = 1;
            do
            {
                // 每次循環，Svi 加 1
                Svi++; 
                // 設置計時器精度為 1 毫秒，讓系統支持更精確的計時
                timeBeginPeriod(1);
                // 紀錄當前時間
                uint start = timeGetTime(); 
                do
                {
                    // 如果已經超過 20 毫秒，則跳出內部循環
                    if ((timeGetTime() - start) >= 20) { break; }
                    // 允許系統處理當前所有掛起的事件，以避免 UI 凍結
                    System.Windows.Forms.Application.DoEvents();
                } while ((timeGetTime() - start) < 20);  // 確保內循環至少持續 20 毫秒

                // 恢復計時器精度，避免持續的高精度對系統性能的影響
                timeEndPeriod(1);
                // 再次允許系統處理所有掛起的事件
                System.Windows.Forms.Application.DoEvents();
                // 如果 flgSend 和 flgReceived 都是 false，則跳出外層循環
                if (flgSend == false && flgReceived == false) { break; }
                // 如果 mvars.Break 為 true，則跳出外層循環
                if (Break) { break; }
            } while (Svi < SvCountMax);  // 當 Svi 小於 SvCountMax 時重複外層循環
            // 結束後將 flgSend 和 flgReceived 設置為 false
            flgSend = false; flgReceived = false;
        }

        /// <summary>
        /// 旗標重製
        /// </summary>
        /// <param name="svtotalcounts"></param>
        /// <param name="svdelfb"></param>
        public void markreset(int svtotalcounts, bool svdelfb)
        {
            flgSend = false;
            flgReceived = false;
            //flgSelf = selfrun;
            Array.Resize(ref lCmd, svtotalcounts); Array.Clear(lCmd, 0, lCmd.Length);
            Array.Resize(ref lGet, svtotalcounts); Array.Clear(lGet, 0, lGet.Length);
            flgDelFB = svdelfb;
            flgSend = false;
            flgReceived = false;
        }

        /// <summary>
        /// 等待XX秒並在 Listview 顯示倒數
        /// </summary>
        /// <param name="svWaitSec">需自行填入需要等待的秒數</param>
        /// <param name="ProcessList">與 UI 控鍵相互綁定的 ObservableCollection 或是 List</param>
        public void WaitSec(int svWaitSec, ObservableCollection<string> ProcessList)
        {
            do
            {
                // 控件倒數秒數
                ProcessList.Insert(1, $"{svWaitSec}秒");
                // 讓UI介面解除回圈的卡住問題
                Application.DoEvents();
                // 1秒1秒等
                doDelayms(1000);
                // 控件移除前一秒的顯示
                ProcessList.RemoveAt(1);
                // 輸入秒數減少
                svWaitSec--;
            } while (svWaitSec > 0);
        }
        /// <summary>
        /// 實作等待XX秒
        /// </summary>
        /// <param name="Svms">需要Delay的秒數</param>
        public void doDelayms(int Svms)
        {
            timeBeginPeriod(1);
            uint start = timeGetTime();
            do
            {
                if ((timeGetTime() - start) >= Svms) { break; }
                Application.DoEvents();
            } while ((timeGetTime() - start) < Svms);
            timeEndPeriod(1);
        }

        public void SaveBinFile(string sPathFileName, byte[] BinArr)
        {
            string sPath = Path.GetDirectoryName(sPathFileName);
            if (!Directory.Exists(sPath))
                Directory.CreateDirectory(sPath);
            File.WriteAllBytes(sPathFileName, BinArr);
        }

        public int HexToDec(string SvVa)
        {
            int SvintVal = 0;
            for (int svi = SvVa.Length; svi > 0; svi--)
            {
                string Svtemp = SvVa.Substring(SvVa.Length - svi, 1);
                if (Svtemp == "A" || Svtemp == "a")
                {
                    SvintVal += 10 * (int)Math.Pow(16, (svi - 1));
                }
                else if (Svtemp == "B" || Svtemp == "b")
                {
                    SvintVal += 11 * (int)Math.Pow(16, (svi - 1));
                }
                else if (Svtemp == "C" || Svtemp == "c")
                {
                    SvintVal += 12 * (int)Math.Pow(16, (svi - 1));
                }
                else if (Svtemp == "D" || Svtemp == "d")
                {
                    SvintVal += 13 * (int)Math.Pow(16, (svi - 1));
                }
                else if (Svtemp == "E" || Svtemp == "e")
                {
                    SvintVal += 14 * (int)Math.Pow(16, (svi - 1));
                }
                else if (Svtemp == "F" || Svtemp == "f")
                {
                    SvintVal += 15 * (int)Math.Pow(16, (svi - 1));
                }
                else
                {
                    SvintVal += Convert.ToInt32(Svtemp) * (int)Math.Pow(16, (svi - 1));
                }
            }
            return SvintVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Buffer"></param>
        public void FillOutBuffer(byte[] Buffer)
        {
            Buffer[0] = 0x01;                //Report ID

            Buffer[0 + 1] = Convert.ToByte(deviceID.Substring(0, 2), 16);    //Device ID 
            Buffer[1 + 1] = Convert.ToByte(deviceID.Substring(2, 2), 16);    //Device ID
            byte[] WriteSizeArr = { Buffer[4 + 1], Buffer[3 + 1] };
            UInt16 WriteSize = BitConverter.ToUInt16(WriteSizeArr, 0);
            //Byte[] IP = StringToByteArray(mvars.sIP);
            Buffer[WriteSize - 3] = 0x00;
            Buffer[WriteSize - 2] = 0x01;
            byte[] arr = BitConverter.GetBytes(CalChecksum(Buffer, 1, WriteSize));
            Buffer[WriteSize - 1] = arr[1];  //Checksum
            Buffer[WriteSize] = arr[0];      //Checksum
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static UInt16 CalChecksum(byte[] arr, UInt16 index, UInt16 size)
        {
            UInt32 Checksum = 0;
            UInt16 i;
            for (i = index; i < (size + 1); i++)
            {
                Checksum += (UInt32)(arr[i]);
                if (Checksum > 65535) Checksum %= 65536;
            }
            return (UInt16)Checksum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Buffer"></param>
        /// <returns></returns>
        public static bool ChecksumCheck(byte[] Buffer)
        {
            byte[] ReadSizeArr = { Buffer[4 + 1], Buffer[3 + 1] };
            UInt16 ReadSize = BitConverter.ToUInt16(ReadSizeArr, 0);
            byte[] ChecksumArr = { Buffer[ReadSize], Buffer[ReadSize - 1] };
            UInt16 Checksum = BitConverter.ToUInt16(ChecksumArr, 0);

            if (CalChecksum(Buffer, 1, (UInt16)(ReadSize - 2)) == Checksum)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sStr"></param>
        /// <param name="sAscii"></param>
        /// <returns></returns>
        public string ContinueAsciiFilter(string sStr, string sAscii)
        {
            for (int i = 0; i < (sStr.Length - 1); i++)
            {
                if (sStr.Substring(i, 1) == sAscii && sStr.Substring(i + 1, 1) == sAscii)
                    sStr = sStr.Remove(i--, 1);
            }
            return sStr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="IndexStart"></param>
        /// <param name="IndexEnd"></param>
        /// <returns></returns>
        public UInt16 CalChecksumIndex(byte[] arr, UInt32 IndexStart, UInt32 IndexEnd)
        {
            UInt32 Checksum = 0;
            UInt32 i;
            for (i = IndexStart; i <= IndexEnd; i++)
            {
                Checksum += (UInt32)(arr[i]);
                if (Checksum > 65535)
                {
                    Checksum %= 65536;
                }
            }
            return (UInt16)Checksum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourceOffset"></param>
        /// <param name="target"></param>
        /// <param name="targetOffset"></param>
        /// <param name="count"></param>
        /// <exception cref="System.ArgumentException"></exception>
        public static unsafe void Copy(byte[] source, int sourceOffset, byte[] target, int targetOffset, int count)
        {
            if ((source == null) || (target == null))
            {
                throw new System.ArgumentException();
            }
            if ((sourceOffset < 0) || (targetOffset < 0) || (count < 0))
            {
                throw new System.ArgumentException();
            }
            if ((source.Length - sourceOffset < count) ||
                (target.Length - targetOffset < count))
            {
                throw new System.ArgumentException();
            }
            fixed (byte* pSource = source, pTarget = target)
            {
                for (int i = 0; i < count; i++)
                {
                    pTarget[targetOffset + i] = pSource[sourceOffset + i];
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tmp"></param>
        /// <returns></returns>
        public string Byte2HexString(byte tmp)
        {
            byte[] arr = { tmp };
            string str = BitConverter.ToString(arr);
            return str;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SvVa"></param>
        /// <param name="SvBits"></param>
        /// <returns></returns>
        public string DecToHex(int SvVa, int SvBits)
        {
            string os = "";
            string[] temp = new String[4];
            int i;
            //int intl = SvVa.ToString().Length - 1;
            int Svj = SvVa;

            for (i = 0; i <= 3; i++)
            {
                temp[i] = (Svj % 16).ToString();
                //Svj = Svj / 16;
                Svj /= 16;
                switch (temp[i])
                {
                    case "10":
                        temp[i] = "A"; break;
                    case "11":
                        temp[i] = "B"; break;
                    case "12":
                        temp[i] = "C"; break;
                    case "13":
                        temp[i] = "D"; break;
                    case "14":
                        temp[i] = "E"; break;
                    case "15":
                        temp[i] = "F"; break;
                }
                os = temp[i] + os;
            }
            if (SvBits < 4)
            {
                os = os.Substring(4 - SvBits, SvBits);  //=Right()
            }
            return os;
        }

        #region Dll Import
        [DllImport("winmm")]
        public static extern void timeBeginPeriod(int t);
        [DllImport("winmm")]
        static extern uint timeGetTime();
        [DllImport("winmm")]
        public static extern void timeEndPeriod(int t);
        #endregion

    }
}
