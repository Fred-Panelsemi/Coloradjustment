using ControlzEx.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PanelSemi_Coloradjustment
{
    internal partial class TotalProcess
    {
        public int Delaymillisec;
        public int NumBytesToRead;
        public bool svnova = false;
        public bool nvBoardcast = false;
        public string strR60K;
        public string strR62K;
        public string strR64K;
        public string strR66K;

        public string verMCU;

        #region DataBuffer
        public byte[] RS485_WriteDataBuffer;
        public byte[] ReadDataBuffer;
        #endregion

        /// <summary>
        /// MCU Version
        /// </summary>
        public void mhVersion()
        {
            // 掛上Command label
            
            #region 2023版公用程序
            byte svns = 2;
            if (svnova)
            {
                Delaymillisec = 5;
                NumBytesToRead = 28;
                Array.Resize(ref RS485_WriteDataBuffer, svns + 0x09);
            }
            else
            {
                svns = 1;
                Array.Resize(ref RS485_WriteDataBuffer, 513);
            }
            Array.Resize(ref ReadDataBuffer, 65);
            Array.Clear(ReadDataBuffer, 0, ReadDataBuffer.Length);
            Array.Clear(RS485_WriteDataBuffer, 0, RS485_WriteDataBuffer.Length);
            #endregion

            RS485_WriteDataBuffer[svns + 2] = 0x01;   ///Cmd
            RS485_WriteDataBuffer[svns + 3] = 0x00;   ///Size
            RS485_WriteDataBuffer[svns + 4] = 0x09;   ///Size    [svns + 4] + ip + checksum = 9筆data
            funSendMessageTo();
        }

        /// <summary>
        /// Series Mode
        /// </summary>
        public void mIDSERIESMODE()
        {
            // 掛上Command label
            #region 2023版公用程序
            byte svns = 1;
            Array.Resize(ref RS485_WriteDataBuffer, 513);
            Array.Resize(ref ReadDataBuffer, 513);
            Array.Clear(ReadDataBuffer, 0, ReadDataBuffer.Length);
            Array.Clear(RS485_WriteDataBuffer, 0, RS485_WriteDataBuffer.Length);
            #endregion
            RS485_WriteDataBuffer[2 + svns] = 0x86;       //Cmd
            RS485_WriteDataBuffer[3 + svns] = 0x00;       //Size
            RS485_WriteDataBuffer[4 + svns] = 12;         //Size
            RS485_WriteDataBuffer[5 + svns] = 1;          //Display On
            RS485_WriteDataBuffer[6 + svns] = 0x12;       //Serial Mode Key
            RS485_WriteDataBuffer[7 + svns] = 0x34;       //Serial Mode Key
            funSendMessageTo();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="svAutoID"></param>
        public void mAUTOID(string svAutoID)     //0x87
        {
            #region 2023版公用程序
            byte svns = 1;
            Array.Resize(ref RS485_WriteDataBuffer, 513);
            Array.Resize(ref ReadDataBuffer, 513);
            Array.Clear(ReadDataBuffer, 0, ReadDataBuffer.Length);
            Array.Clear(RS485_WriteDataBuffer, 0, RS485_WriteDataBuffer.Length);
            #endregion
            RS485_WriteDataBuffer[2 + svns] = 0x87;                       //Cmd
            RS485_WriteDataBuffer[3 + svns] = 0x00;                       //Size
            RS485_WriteDataBuffer[4 + svns] = 10;                         //Size
            RS485_WriteDataBuffer[5 + svns] = Convert.ToByte(svAutoID);   //ID
            funSendMessageTo();
        }

        /// <summary>
        /// 
        /// </summary>
        public void mWRGETDEVID()     //0x8A
        {
            
            #region 2023版公用程序
            byte svns = 1;
            Array.Resize(ref RS485_WriteDataBuffer, 513);
            Array.Resize(ref ReadDataBuffer, 513);
            Array.Clear(ReadDataBuffer, 0, ReadDataBuffer.Length);
            Array.Clear(RS485_WriteDataBuffer, 0, RS485_WriteDataBuffer.Length);
            #endregion
            RS485_WriteDataBuffer[2 + svns] = 0x8A;   //Cmd
            RS485_WriteDataBuffer[3 + svns] = 0x00;   //Size
            RS485_WriteDataBuffer[4 + svns] = 0x0C;   //Size
            RS485_WriteDataBuffer[5 + svns] = 0xA1;   //Wr mcu parameter cmd.
            RS485_WriteDataBuffer[6 + svns] = 0x56;   //Key
            RS485_WriteDataBuffer[7 + svns] = 0xA0;   //Length
            funSendMessageTo();
        }

        /// <summary>
        /// MCU Flash Read
        /// </summary>
        /// <param name="svtxt44"></param>
        /// <param name="ReadSize"></param>
        public void mhMCUFLASHREAD(string svtxt44, int ReadSize)             // 0x14 C12B
        {
            //UInt16 ReadSize = (UInt16)mvars.MCU_60000_SIZE; //1024,2048,4096,8192,16384 (需與cFLASHWRITE_C12ACB 設定值相同)
            //UInt16 ReadSize = 1024; //1024,2048,4096,8192,16384 (需與cFLASHWRITE_C12ACB 設定值相同)
            byte[] bytes = BitConverter.GetBytes(ReadSize);
            byte[] flash_addr_arr = StringToByteArray(svtxt44);

            #region Novastar Setup
            byte svns = 2;
            Delaymillisec = 2; NumBytesToRead = ReadSize + 65;
            Array.Resize(ref RS485_WriteDataBuffer, svns + 0x0F);
            #endregion Novastar
            if (svnova == false)
            {
                svns = 1;
                Array.Resize(ref RS485_WriteDataBuffer, 513);
                Array.Resize(ref ReadDataBuffer, 65 + 8192);
            }
            Array.Clear(RS485_WriteDataBuffer, 0, RS485_WriteDataBuffer.Length);
            Array.Clear(ReadDataBuffer, 0, ReadDataBuffer.Length);

            RS485_WriteDataBuffer[2 + svns] = 0x14;                   //Cmd
            RS485_WriteDataBuffer[3 + svns] = 0x00;                   //Size
            RS485_WriteDataBuffer[4 + svns] = 0x0F;                   //Size
            RS485_WriteDataBuffer[5 + svns] = flash_addr_arr[0];      //QSPI address
            RS485_WriteDataBuffer[6 + svns] = flash_addr_arr[1];      //QSPI address
            RS485_WriteDataBuffer[7 + svns] = flash_addr_arr[2];      //QSPI address
            RS485_WriteDataBuffer[8 + svns] = flash_addr_arr[3];      //QSPI address.
            RS485_WriteDataBuffer[9 + svns] = bytes[1];               //Read Size
            RS485_WriteDataBuffer[10 + svns] = bytes[0];              //Read Size
            funSendMessageTo();
        }

        /// <summary>
        /// 
        /// </summary>
        public void mhMCUinf()                                              // 0x03 
        {
            #region 2023版公用程序
            byte svns = 2;
            if (svnova)
            {
                Delaymillisec = 5; NumBytesToRead = 28;
                Array.Resize(ref RS485_WriteDataBuffer, svns + 0x09);
            }
            else
            {
                svns = 1;
                Array.Resize(ref RS485_WriteDataBuffer, 513);
            }
            Array.Resize(ref ReadDataBuffer, 513);
            Array.Clear(ReadDataBuffer, 0, ReadDataBuffer.Length);
            Array.Clear(RS485_WriteDataBuffer, 0, RS485_WriteDataBuffer.Length);
            #endregion

            RS485_WriteDataBuffer[svns + 2] = 0x03;   ///Cmd
            RS485_WriteDataBuffer[svns + 3] = 0x00;   ///Size
            RS485_WriteDataBuffer[svns + 4] = 0x09;   ///Size    [svns + 4] + ip + checksum = 9筆data
            funSendMessageTo();
        }
        /// <summary>
        /// 
        /// </summary>
        public void mhMCU_FLASH_R60000()
        {
            mhMCUFLASHREAD("60000".PadLeft(8, '0'), 8192);
        }
        /// <summary>
        /// 
        /// </summary>
        public void mhMCUReadBootVer()
        {
            mhMCUFLASHREAD(MCU_BOOT_VERSION_ADDR.ToString("X8"), 16);
        }

        /// <summary>
        /// 
        /// </summary>
        public void mhMCUReadAppVer()
        {
            mhMCUFLASHREAD(MCU_APP_VERSION_ADDR.ToString("X8"), 16);
        }

        /// <summary>
        /// 
        /// </summary>
        public void McuSW_Reset()   //0xFF
        {
            
            #region 2023版公用程序 (無 Nova 參數)
            byte svns = 1;
            Array.Resize(ref RS485_WriteDataBuffer, 513);
            Array.Resize(ref ReadDataBuffer, 513);
            Array.Clear(ReadDataBuffer, 0, ReadDataBuffer.Length);
            Array.Clear(RS485_WriteDataBuffer, 0, RS485_WriteDataBuffer.Length);
            #endregion
            RS485_WriteDataBuffer[2 + svns] = 0xFF;            //Cmd
            RS485_WriteDataBuffer[3 + svns] = 0x00;            //Size
            RS485_WriteDataBuffer[4 + svns] = 0x0B;            //Size
            RS485_WriteDataBuffer[5 + svns] = 0x07;            //Delay 1s
            RS485_WriteDataBuffer[6 + svns] = 0xD0;            //Delay 1s
            funSendMessageTo();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="svOnOff"></param>
        public void mpIDONOFF(byte svOnOff)          //0x86
        {
            
            #region 2023版公用程序
            byte svns = 1;
            Array.Resize(ref RS485_WriteDataBuffer, 513);
            Array.Resize(ref ReadDataBuffer, 513);
            Array.Clear(ReadDataBuffer, 0, ReadDataBuffer.Length);
            Array.Clear(RS485_WriteDataBuffer, 0, RS485_WriteDataBuffer.Length);
            #endregion
            RS485_WriteDataBuffer[2 + svns] = 0x86;       //Cmd
            RS485_WriteDataBuffer[3 + svns] = 0x00;       //Size
            RS485_WriteDataBuffer[4 + svns] = 10;         //Size
            RS485_WriteDataBuffer[5 + svns] = svOnOff;    //Display On
            funSendMessageTo();
        }

        public void mhMCUFLASHWRITE(string sFlashAddr, ref byte[] Arr)   // 0x15 Primary 
        {
            #region 2023版公用程序(w/o novastar，帶 PacketSize)
            byte svns = 1;
            int PacketSize = Arr.Length + 0x0F;
            Array.Resize(ref RS485_WriteDataBuffer, Arr.Length + 513);
            Array.Resize(ref ReadDataBuffer, 513);
            Array.Clear(ReadDataBuffer, 0, ReadDataBuffer.Length);
            Array.Clear(RS485_WriteDataBuffer, 0, RS485_WriteDataBuffer.Length);
            #endregion

            RS485_WriteDataBuffer[2 + svns] = 0x15;               //Cmd
            byte[] bytes = BitConverter.GetBytes(PacketSize);
            RS485_WriteDataBuffer[3 + svns] = bytes[1];           //Size
            RS485_WriteDataBuffer[4 + svns] = bytes[0];           //Size
            byte[] flash_addr_arr = StringToByteArray(sFlashAddr.PadLeft(8, '0'));
            RS485_WriteDataBuffer[5 + svns] = 0x00;               //cmd for IntFlash.
            RS485_WriteDataBuffer[6 + svns] = flash_addr_arr[1];  //address
            RS485_WriteDataBuffer[7 + svns] = flash_addr_arr[2];  //address
            RS485_WriteDataBuffer[8 + svns] = flash_addr_arr[3];  //address
            byte[] WrArr = BitConverter.GetBytes(Arr.Length);
            RS485_WriteDataBuffer[9 + svns] = WrArr[1];           //Write Size
            RS485_WriteDataBuffer[10 + svns] = WrArr[0];          //Write Size
                                                                  //byte[] DataArr = File.ReadAllBytes(textBox41.Text);
            //RS485_WriteDataBuffer[94] = Convert.ToByte(4000);
            //RS485_WriteDataBuffer[98] = Convert.ToByte(2400);
            //RS485_WriteDataBuffer[102] = Convert.ToByte(2400);

            for (UInt16 i = 0; i < Arr.Length; i++)
                RS485_WriteDataBuffer[11 + svns + i] = Arr[i];    //Data
            funSendMessageTo();
        }

        /// <summary>
        /// 色差初始化Step1
        /// </summary>
        /// <param name="panelCount">計算屏幕的數量</param>
        public void cBOXREAD(ref int panelCount)
        {
            lblCompose = "BOXREAD";
            lCounts = 9999;
            lCount = 0;
            Array.Resize(ref lCmd, lCounts); Array.Clear(lCmd, 0, lCmd.Length);
            Array.Resize(ref lGet, lCounts); Array.Clear(lGet, 0, lGet.Length);
            lCount = 0;
            errCode = "0";
            flgDelFB = true;
            Break = false;
            //lstget1.Items.Clear();
            DateTime t1 = DateTime.Now;
            bool svnvBoardcast = nvBoardcast;
            nvBoardcast = false;
            string svdeviceID = deviceID;

            lstm.Items.Clear();
            // 迴圈讀取 MCU Version 如果沒讀到跳出迴圈
            for (byte svscr = 1; svscr <= 32; svscr++)
            {
                deviceID = deviceID.Substring(0, 2) + DecToHex(Convert.ToInt16(svscr), 2);

                lblCmd = "MCU_VERSION";
                mhVersion();
                
                if (lGet[lCount - 1].IndexOf("DONE", 0) != -1)
                {
                    if (verMCU.Substring(verMCU.Length - 5, 1) == "P")
                    {
                        lstm.Items.Add(deviceID);
                        panelCount = (int)svscr;
                    }
                }
                else
                {

                    break;
                }
                
            }
            if (lstm.Items.Count > 0)
            {
                List<string> mcuR66000 = new List<string>();
                for (int i = 0; i < lstm.Items.Count; i++)
                {
                    deviceID = deviceID.Substring(0, 2) + lstm.Items[i].ToString().Substring(2, 2);

                    pvindex = 0;         //Ver 
                    lblCmd = "FPGA_SPI_R";
                    mhFPGASPIREAD();
                    string svX = "-1";
                    string svY = "-1";
                    string svW = "-1";
                    string svH = "-1";
                    //if (lGet[lCount - 1].IndexOf("DONE", 0) > -1)
                    //{
                        
                    //}
                    #region 0x66000 Read (-..8/.9)
                    mcuR66000.Clear();
                    lblCmd = "MCU_FLASH_R66000";
                    mhMCUFLASHREAD("00066000", 8192);
                    Console.WriteLine(ReadDataBuffer);
                    if (strR66K == "")
                    {
                        errCode = "-3";
                        //mvars.strReceive = "ERROR,AIO_UI v" + mvars.UImajor + @" need MCUFLASH_0X66K content,please check AIO version or redo ""Setting Screen""";
                    }
                    else
                    {
                        svX = strR66K.Split('~')[8].Split(',')[1];
                        svY = strR66K.Split('~')[5].Split(',')[1];
                        svW = "960";
                        svH = "540";
                    }
                    #endregion 0x66000 Read
                }
            }
            else
            {
                errCode = "-1";
            }

            if (svnova == false && sp1.IsOpen) { CommClose(); }
            nvBoardcast = svnvBoardcast;
            deviceID = svdeviceID;
            //mvars.FPGAsel = svFPGAsel;
            //lstget1.Items.Insert(0, MultiLanguage.DefaultLanguage.Split('-')[1]);
            flgDelFB = false;
            lCounts = lCount + 1;
            lblCmd = "EndcCMD"; lCmd[lCount] = lblCmd + ",";
            flgSend = true; flgReceived = false;
            if (errCode == "0")
            {
                //if (mvars.actFunc != "" && mvars.flgSelf == true) lstget1.Items.Insert(0, "↓BoxRead，" + (DateTime.Now - t1).TotalSeconds.ToString("0.0") + "s");
            }
            //lstget1.TopIndex = lstget1.Items.Count - 1;
            //mvars.tmeRSIn.Enabled = true;
            flgReceived = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string CommClose4SWRESET()
        {
            try { sp1.Close(); return "DONE"; }
            catch (Exception ex) { return "ERROR," + ex.Message; }
        }

    }
}
