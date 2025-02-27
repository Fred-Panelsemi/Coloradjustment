using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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
    // TotalProcess 本體
    // 1. 【Init】
    //    => TotalProcess()
    // 2. 【Void】
    //    => USB_ComPort_FindandOpen()
    // 3. 【Void】
    //    => funSendMessageTo()
    // 4. 【Void】
    //    => CommandSwitches(string cmdName, byte[] ReadDataBuffer,string WriteStatus)
    // 5. 【Void】
    //    => WriteReadMcuTask(byte[] OUTBuffer, ref uint BytesWritten, byte[] INBuffer, ref uint BytesRead)
    // 6. 【Void】
    //    => WriteReadMcuTask(byte[] OUTBuffer, byte[] INBuffer)

    //================================================================================================================================================================================
    // Partial C >> TotalProcess
    internal partial class TotalProcess
    {
        //
        #region  Device ID
        public string[] deviceAll = { "C12A", "H5512A", "Customize_UI", "C12B", "Primary", "TV130" };
        public string[] deviceIDall = { "0200", "0300", "0400", "0200", "0500", "0600" };
        public string deviceID = "";
        public string deviceName = "";
        #endregion

        #region Command & Finish Info Array

        public string lblCmd;
        public string lblCompose;
        public string[] lCmd;
        public string[] lGet;
        public int lCount;
        public int lCounts;      //累計單一命令需要由RS485接收到的計數

        #endregion


        // projectName
        public string projectName = "";

        public Byte ReturnOK = 0x03;

        public string[] CommArray; // 利用findcommport的函式得到的 port name 並回丟在此
        public bool flgSendmessage = false; // 原程式碼對應海康相機 Commport關不掉的bug 未來合併時 看是否需要移除

        public int MCU_APP_VERSION_ADDR = 0x0007FFE0;
        public int MCU_INFO_ADDR = 0x7FF00;
        public int MCU_BOOT_INFO_ADDR = 0xFF00;
        public int MCU_BOOT_VERSION_ADDR = 0xFFF0;

        private System.IO.Ports.SerialPort serialPort1;

        public string isUSBCconnect = "";
        public bool isUSBOpen = false;

        /// <summary>
        /// Ffunction Initial
        /// </summary>
        public TotalProcess()
        {
            markreset(3,false);
            for (int j = 0; j < uiregadr_default_p.Length; j++)
            {
                svuiregadr.RemoveAt(j);
                svuiregadr.Insert(j, uiregadr_default_p[j].Split(',')[2]);
                svuiregadr.RemoveAt(j + (int)(GAMMA_SIZE / 8));   //x062000共存2048個參數(FPFA A與FPGA B各半)每個參數占用4個bytes=2048*4=8192 bytes=mvars.GAMMA_SIZE;
                svuiregadr.Insert(j + (int)(GAMMA_SIZE / 8), uiregadr_default_p[j].Split(',')[2]);
            }
            
            // 連線 Init
            sp1 = new SerialPort();

            isUSBCconnect = FindComPort();
            deviceID = "0500";
            if (isUSBCconnect != "") { string isConnectInfo = Sp1open(CommArray[0], out isUSBOpen); }
            
        }

        public void USB_ComPort_FindandOpen()
        {
            isUSBCconnect = FindComPort();
            deviceID = "0500";
            if (isUSBCconnect != "") { string isConnectInfo = Sp1open(CommArray[0], out isUSBOpen); }
        }

        /// <summary>
        /// 
        /// </summary>
        public void funSendMessageTo()
        {
            string strTemp = "";
            int Svi;
            Byte[] tmp = null;

            Wait(0);    //1227 重新啟用
            FillOutBuffer(RS485_WriteDataBuffer);

            uint BytesWritten = 0, BytesRead = 0;
            Svi = 0;
            flgSend = true; flgReceived = false;
            string WriteStatus = "";
            WriteStatus = WriteReadMcuTask(RS485_WriteDataBuffer, ref BytesWritten, ReadDataBuffer, ref BytesRead);
            CommandSwitches(lblCmd, ReadDataBuffer, WriteStatus);
            //Console.WriteLine($"Fred test : {WriteStatus}");
        }

        public void CommandSwitches(string cmdName, byte[] ReadDataBuffer,string WriteStatus)
        {
            string strTemp = "";
            string Svs = "-1";
            int Svi;
            string resultInfo = "";
            Byte[] tmp = null;
            if (WriteStatus.IndexOf("true", 0) > -1)
            {
                switch (cmdName)
                {
                    case "MCU_VERSION":

                        #region MCU Version

                        string svdeviceID = Byte2HexString(ReadDataBuffer[1]) + Byte2HexString(ReadDataBuffer[2]);
                        if (svdeviceID == deviceID)
                        {
                            tmp = new byte[16];
                            Copy(ReadDataBuffer, 6 + 1, tmp, 0, 16);
                            strTemp = System.Text.Encoding.ASCII.GetString(tmp);
                            verMCU = strTemp;
                            resultInfo = $"DONE,{cmdName},{strTemp}";
                            Svs = resultInfo;
                        }
                        else
                        {
                            Svs = "-1";
                            Console.WriteLine("Device not match");
                        }
                        #endregion

                        break;
                    case "READ_MCUAPPVER":

                        break;
                    case "READ_MCUBOOTVER":

                        break;
                    case "PRIID_AUTOID":
                        // 沒有動作
                        resultInfo = $"Done,{cmdName},{strTemp}";
                        break;
                    case "PRIID_SERIESMODE":
                        // 沒有動作
                        resultInfo = $"Done,{cmdName},{strTemp}";
                        break;
                    case "MCU_INFORMATION":
                        projectName = "UnKnow";
                        UInt16 MCU_VerLen = (UInt16)(Convert.ToUInt16(ReadDataBuffer[3 + 1] * 256 + ReadDataBuffer[4 + 1]) - 10);
                        tmp = new byte[MCU_VerLen];
                        Copy(ReadDataBuffer, 6 + 1, tmp, 0, MCU_VerLen);
                        string MCU_Ver = System.Text.Encoding.ASCII.GetString(tmp);
                        MCU_Ver = ContinueAsciiFilter(MCU_Ver, "\0");
                        string[] split = MCU_Ver.Split(new Char[] { '\0', '\t' });
                        Svs = split[3] + "," + split[0];
                        projectName = split[3];

                        for (byte sva = 0; sva < deviceAll.Length; sva++)
                        {
                            if (deviceAll[sva] == projectName)
                            {
                                deviceID = deviceIDall[sva];
                                deviceName = projectName;
                                break;
                            }
                        }
                        resultInfo = Svs;

                        break;
                    case "MCU_FLASH_R60000":
                        #region MCU_FLASH_R60000
                        int i = 0;
                        strR60K = "";
                        do
                        {
                            Svs = (ReadDataBuffer[6 + (i * 4) + 1] * 256 + ReadDataBuffer[7 + (i * 4) + 1]).ToString() + "," + (ReadDataBuffer[8 + (i * 4) + 1] * 256 + ReadDataBuffer[9 + (i * 4) + 1]).ToString();
                            if (Svs == "65535,65535" || (i == 0 && Svs == "0,0")) break;
                            //if (i == 0 && Svs == "0,0") break;
                            strR60K += "~" + Svs;
                            i++;
                        }
                        while (i < FPGA_MTP_SIZE / 4);
                        if (strR60K.Length > 1 && strR60K.Substring(0, 1) == "~") strR60K = strR60K.Substring(1, strR60K.Length - 1);
                        #endregion
                        break;
                    case "MCU_FLASH_R62000":
                        #region MCU_FLASH_R62000
                        i = 0;
                        strR62K = "";
                        do
                        {
                            Svs = (ReadDataBuffer[6 + (i * 4) + 1] * 256 + ReadDataBuffer[7 + (i * 4) + 1]).ToString() + "," + (ReadDataBuffer[8 + (i * 4) + 1] * 256 + ReadDataBuffer[9 + (i * 4) + 1]).ToString();
                            if (Svs.IndexOf(",65535") != -1 || (i == 0 && Svs == "0,0")) break;
                            strR62K += "~" + Svs;
                            i++;
                        }
                        while (i < GAMMA_MTP_SIZE / 4);
                        if (strR62K.Length > 1 && strR62K.Substring(0, 1) == "~") strR62K = strR62K.Substring(1, strR62K.Length - 1);
                        #endregion
                        break;
                    case "MCU_FLASH_R64000":
                        #region MCU_FLASH_R64000
                        strR64K = "";
                        i = 0;
                        do
                        {
                            Svs = (ReadDataBuffer[6 + (i * 4) + 1] * 256 + ReadDataBuffer[7 + (i * 4) + 1]).ToString() + "," + (ReadDataBuffer[8 + (i * 4) + 1] * 256 + ReadDataBuffer[9 + (i * 4) + 1]).ToString();
                            if (Svs.IndexOf(",65535") != -1 || (i == 0 && Svs == "0,0")) break;
                            strR64K += "~" + Svs;
                            i++;
                        }
                        while (i < GAMMA_MTP_SIZE / 4);
                        if (strR64K.Length > 1 && strR64K.Substring(0, 1) == "~") strR64K = strR64K.Substring(1, strR64K.Length - 1);
                        #endregion
                        break;
                    case "MCU_FLASH_R66000":
                        #region MCU_FLASH_R66000
                        strR66K = "";
                        i = 0;
                        do
                        {
                            Svs = (ReadDataBuffer[6 + (i * 4) + 1] * 256 + ReadDataBuffer[7 + (i * 4) + 1]).ToString() + "," + (ReadDataBuffer[8 + (i * 4) + 1] * 256 + ReadDataBuffer[9 + (i * 4) + 1]).ToString();
                            if (Svs == "65535,65535" || (i == 4 && Svs == "0,0")) break;
                            strR66K += "~" + Svs;
                            i++;
                        }
                        while (i < GAMMA_MTP_SIZE / 4);
                        if (strR66K.Length > 1 && strR66K.Substring(0, 1) == "~") strR66K = strR66K.Substring(1, strR66K.Length - 1);
                        #endregion
                        break;
                    case "UIREGRAD_READ":
                        #region UIREGARD_READ
                        Svs = "DONE";
                        #endregion
                        break;
                    case "FPGA_SPI_R":

                        #region FPGA Version

                        if (pvindex == 0)
                        {
                            verFPGAm = new string[2];
                            verFPGA = "-1";
                            strTemp = DecToHex(Convert.ToInt16(ReadDataBuffer[6 + 1]), 2) + "." + DecToHex(Convert.ToInt16(ReadDataBuffer[7 + 1]), 2);
                            verFPGAm[0] = DecToHex(Convert.ToInt16(ReadDataBuffer[6 + 1]), 2) + DecToHex(Convert.ToInt16(ReadDataBuffer[7 + 1]), 2);
                            verFPGA = strTemp;
                            strTemp = DecToHex(Convert.ToInt16(ReadDataBuffer[8 + 1]), 2) + "." + DecToHex(Convert.ToInt16(ReadDataBuffer[9 + 1]), 2);
                            verFPGAm[1] = DecToHex(Convert.ToInt16(ReadDataBuffer[8 + 1]), 2) + DecToHex(Convert.ToInt16(ReadDataBuffer[9 + 1]), 2);
                            strTemp = verFPGA + "-" + strTemp;
                        }
                        else if (pvindex == 1)
                        {
                            //將數字前面補0，補足設定的長度
                            strTemp = Convert.ToString(ReadDataBuffer[7 + 1], 2).PadLeft(8, '0') + "-" + Convert.ToString(ReadDataBuffer[9 + 1], 2).PadLeft(8, '0');
                        }
                        else
                        {
                            strTemp = (ReadDataBuffer[6 + 1] * 256 + ReadDataBuffer[7 + 1]) + "-" + (ReadDataBuffer[8 + 1] * 256 + ReadDataBuffer[9 + 1]);
                        }
                        Svs = "R" + pvindex.ToString("000") + "," + strTemp;
                        verFPGA = Svs;
                        resultInfo = Svs;
                        //Console.WriteLine(Svs);
                        #endregion

                        break;
                    case "FPGA_SPI_W":
                        resultInfo = $"Done,{cmdName},{strTemp}";
                        break;
                    case "LT8668_Bin_WrRd":
                        for (Svi = 0; Svi < LT8668rd_arr.Length; Svi++)
                            Svs += "," + Byte2HexString(ReadDataBuffer[6 + 1 + Svi]);
                        Svs = Svs.Substring(1, Svs.Length - 1);
                        resultInfo = Svs;
                        break;
                    case "LT8668VERSION":
                        resultInfo = $"Done,{cmdName},{strTemp}";
                        break;
                    case "EnableI2C_Wr":
                        resultInfo = $"Done,{cmdName},{strTemp}";
                        break;
                    case "Block_Erase_Bin_Wr":
                        resultInfo = $"Done,{cmdName},{strTemp}";
                        break;
                    case "LT8668_Bin_Wr":
                        resultInfo = $"Done,{cmdName},{strTemp}";
                        break;
                    case "WrDis_Bin_Wr":
                        resultInfo = $"Done,{cmdName},{strTemp}";
                        break;
                    case "Scaler_Bin_Wr":
                        resultInfo = $"Done,{cmdName},{strTemp}";
                        break;
                    case "LT8668_SCALE ON":
                        resultInfo = $"Done,{cmdName},{strTemp}";
                        break;
                    case "LT8668_SCALE OFF":
                        resultInfo = $"Done,{cmdName},{strTemp}";
                        break;
                    default:
                        Svs = "1";
                        Console.WriteLine("Not Much Classify");
                        break;

                }
            }
            else
            {

            }
            if (Svs == "-1" || Svs.IndexOf("-1,") != -1)
            {
                lGet[lCount] = lCmd[lCount] + "ERROR," + "," + Svs;
            }
            else
            {
                if (Svs.IndexOf("DONE", 0) != -1) lGet[lCount] = lCmd[lCount] + "DONE";
                else lGet[lCount] = lCmd[lCount] + "DONE," + "," + Svs;
            }
            lCount++;
        }

        #region WriteReadMcuTask

        public string WriteReadMcuTask(byte[] OUTBuffer, ref uint BytesWritten, byte[] INBuffer, ref uint BytesRead)
        {
            Console.WriteLine(OUTBuffer);
            uint PacketSize = Convert.ToUInt16(OUTBuffer[3 + 1] * 256 + OUTBuffer[4 + 1]);
            try
            {
                if (flgSendmessage == false)
                {
                    sp1.Close();
                    if (!sp1.IsOpen) { sp1.Open(); }
                }

                if (PacketSize > 512)
                {
                    byte[] arr = new byte[513];
                    arr[0] = 0; //Report ID
                    for (UInt16 i = 0; i < (PacketSize / 512 + 1); i++)
                    {
                        Copy(OUTBuffer, 512 * i + 1, arr, 1, 512);
                        sp1.Write(arr, 1, 512);
                    }
                }
                else
                {
                    Console.WriteLine(sp1.BytesToRead);
                    sp1.Write(OUTBuffer, 1, 512);
                    Console.WriteLine(sp1.BytesToRead);
                }
            }
            catch (Exception ex)
            {
                CommClose();
                return "false," + ex.Message;
            }
            if (lblCmd == "MCU_RESET") { string svs = CommClose4SWRESET(); return "true," + svs; }
            else   //Read From MCU
            {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Reset();
                sw.Start();
                int Length;
                int ReceiveSize = 1;
                while (sw.Elapsed.TotalMilliseconds < 1000)
                {
                    if (sp1.BytesToRead > 0)
                    {

                        if (sp1.BytesToRead > INBuffer.Length) { CommClose(); return "false,BytesToRead > INBuffer length"; }

                        Length = sp1.Read(INBuffer, ReceiveSize, sp1.BytesToRead);
                        ReceiveSize += Length;
                        PacketSize = Convert.ToUInt16(INBuffer[3 + 1] * 256 + INBuffer[4 + 1]);
                        if ((ReceiveSize - 1) == PacketSize)
                            break;
                    }
                }

                if (sw.Elapsed.TotalMilliseconds >= 1000)
                {
                    CommClose(); return "false,TimeOut";
                }
                else
                {
                    if (ChecksumCheck(INBuffer) == true)
                    {
                        if (INBuffer[5 + 1] == ReturnOK) { CommClose(); return "true"; }
                        else
                        {
                            CommClose();
                            return "false,Inpub Buffer Return NG";
                        }
                    }
                    else
                    {
                        CommClose();
                        return "false,Inpub Buffer Checksum Error";
                    }
                }
            }
        }

        public string WriteReadMcuTask(byte[] OUTBuffer, byte[] INBuffer)
        {
            uint PacketSize = Convert.ToUInt16(OUTBuffer[3 + 1] * 256 + OUTBuffer[4 + 1]);

            try
            {
                if (flgSendmessage == false)
                {
                    sp1.Close();
                    if (!sp1.IsOpen) { sp1.Open(); }
                }

                if (PacketSize > 64)
                {
                    byte[] arr = new byte[65];
                    arr[0] = 0;
#if false
                    for (UInt16 i = 0; i < (PacketSize / 64 + 1); i++)
                    {
                        Copy(OUTBuffer, 64 * i + 1, arr, 1, 64);
                        sp.Write(arr, 1, 64);
                    }
#else
                    UInt16 i;
                    for (i = 0; i < (PacketSize / 64); i++)
                    {
                        Copy(OUTBuffer, 64 * i + 1, arr, 1, 64);
                        sp1.Write(arr, 1, 64);
                    }
                    Copy(OUTBuffer, 64 * i + 1, arr, 1, 64);
                    sp1.Write(arr, 1, (int)PacketSize % 64);
#endif
                }
                else
#if false
                    mvars.sp1.Write(OUTBuffer, 1, 64);
#else
                    sp1.Write(OUTBuffer, 1, (int)PacketSize);
#endif
            }
            catch (Exception ex)
            {
                CommClose();
                return "false," + ex.Message;
            }

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Reset();
            sw.Start();
            int Length;
            int ReceiveSize = 1;
            while (sw.Elapsed.TotalMilliseconds < 1000)
            {
                try
                {
                    if (sp1.BytesToRead > 0)
                    {
                        Length = sp1.Read(INBuffer, ReceiveSize, sp1.BytesToRead);
                        ReceiveSize += Length;
                        PacketSize = Convert.ToUInt16(INBuffer[3 + 1] * 256 + INBuffer[4 + 1]);
                        if ((ReceiveSize - 1) == PacketSize)
                            break;
                    }
                }
                catch (Exception ex)
                {
                    CommClose();
                    return "false," + ex.Message;
                }
            }
            if (sw.Elapsed.TotalMilliseconds >= 1000)
            {
                CommClose(); return "false,TimeOut";
            }
            else
            {
                if (ChecksumCheck(INBuffer) == true)
                {
                    if (INBuffer[5 + 1] == ReturnOK) { return "true"; }
                    else
                    {
                        CommClose();
                        Byte[] tmp = { INBuffer[6 + 1], INBuffer[7 + 1] };
                        string sErrCode = BitConverter.ToString(tmp).Replace("-", "");
                        return "false," + USB_Error_Handler(sErrCode);
                    }
                }
                else { CommClose(); return "false,Inpub Buffer Checksum Error"; }
            }
        }


        #endregion

    }
}
