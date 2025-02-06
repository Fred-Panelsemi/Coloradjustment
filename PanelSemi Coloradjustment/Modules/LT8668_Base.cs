using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PanelSemi_Coloradjustment
{
    internal partial class TotalProcess
    {
        public byte[] LT8668rd_arr = null;
        public string verEDID;

        public void LT8668_Bin_WrRd(Byte Addr, UInt16 WrSize, byte[] arr, UInt16 RdSize, ref byte[] rd_arr)   //0x61
        {

            #region 2023版公用程序 (無 Nova 參數)
            //byte svns = 1;
            Array.Resize(ref RS485_WriteDataBuffer, 513);
            Array.Resize(ref ReadDataBuffer, 513);
            Array.Clear(ReadDataBuffer, 0, ReadDataBuffer.Length);
            Array.Clear(RS485_WriteDataBuffer, 0, RS485_WriteDataBuffer.Length);
            #endregion

            UInt16 Size = (UInt16)(0x0E + WrSize);
            string sShow = "0x" + Byte2HexString(Addr) + ",";
            RS485_WriteDataBuffer[2 + 1] = 0x61;                            //Cmd
            RS485_WriteDataBuffer[3 + 1] = (byte)(Size / 256);              //Size
            RS485_WriteDataBuffer[4 + 1] = (byte)(Size % 256);              //Size
            RS485_WriteDataBuffer[5 + 1] = (byte)(Addr / 2);                //Device Address
            RS485_WriteDataBuffer[6 + 1] = (byte)(WrSize / 256);            //Write Size
            RS485_WriteDataBuffer[7 + 1] = (byte)(WrSize % 256);            //Write Size
            for (UInt16 i = 0; i < WrSize; i++)
            {
                RS485_WriteDataBuffer[8 + 1 + i] = arr[i];
                sShow += "0x" + Byte2HexString(arr[i]) + ",";
            }
            sShow += "0x" + Byte2HexString((byte)(Addr + 1)) + ",";
            RS485_WriteDataBuffer[8 + 1 + WrSize] = (byte)(RdSize / 256);   //Read Size
            RS485_WriteDataBuffer[9 + 1 + WrSize] = (byte)(RdSize % 256);   //Read Size
            funSendMessageTo();
        }

        public void mLt8668Version()
        {
            Byte[] arr = new byte[2]; // EDID Version 1.28 // 因為撈一次沒有辦法把整個版本號撈回來所以需要撈兩次
            LT8668rd_arr = new byte[1];
            byte VerHi = 0, VerLo = 0;
            arr[0] = 0x82;
            LT8668_Bin_WrRd(0x86, 1, arr, 1, ref LT8668rd_arr);
            VerHi = ReadDataBuffer[7];
            arr[0] = 0x83;
            LT8668_Bin_WrRd(0x86, 1, arr, 1, ref LT8668rd_arr);
            VerLo = ReadDataBuffer[7];
            verEDID = $"{VerHi},{VerLo}";
        }

        public void LT8668_Bin_Wr(Byte Addr, UInt16 WrSize, byte[] arr, string cmdName)   //0x60
        {
            #region 2023版公用程序 (無 Nova 參數)
            byte svns = 1;
            Array.Resize(ref RS485_WriteDataBuffer, 513);
            Array.Resize(ref ReadDataBuffer, 513);
            Array.Clear(ReadDataBuffer, 0, ReadDataBuffer.Length);
            Array.Clear(RS485_WriteDataBuffer, 0, RS485_WriteDataBuffer.Length);
            #endregion

            UInt16 Size = (UInt16)(0x0C + WrSize);

            RS485_WriteDataBuffer[2 + svns] = 0x60;                      //Cmd
            RS485_WriteDataBuffer[3 + svns] = (byte)(Size / 256);        //Size
            RS485_WriteDataBuffer[4 + svns] = (byte)(Size % 256);        //Size
            RS485_WriteDataBuffer[5 + svns] = (Byte)(Addr / 2);          //Device Address
            RS485_WriteDataBuffer[6 + svns] = (byte)(WrSize / 256);      //Write Size
            RS485_WriteDataBuffer[7 + svns] = (byte)(WrSize % 256);      //Write Size
            for (UInt16 i = 0; i < WrSize; i++)
            {
                //sShow += "0x" + Byte2HexString(arr[i]) + ",";
                RS485_WriteDataBuffer[8 + svns + i] = arr[i];
            }
            funSendMessageTo();
        }


        public void LT8668_Bin_Wr_Loop(string OffsetAddrText, byte[] gBinArr, string cmdName)     // 0x62
        {
            #region 2023版公用程序 (無 Nova 參數)
            byte svns = 1;
            Array.Resize(ref RS485_WriteDataBuffer, 8193);
            Array.Resize(ref ReadDataBuffer, 513);
            Array.Clear(ReadDataBuffer, 0, ReadDataBuffer.Length);
            Array.Clear(RS485_WriteDataBuffer, 0, RS485_WriteDataBuffer.Length);
            #endregion
            //EDID
            int PacketSize = 256;
            byte[] BinArr = new byte[PacketSize];
            Array.Resize(ref BinArr, PacketSize);
            UInt16 Count = (UInt16)(BinArr.Length / PacketSize);
            for (int i = 0; i < Count; i++)
            {
                string FlashAddrText = (i * PacketSize + Convert.ToUInt32(OffsetAddrText, 16)).ToString("X6");
                byte[] flash_addr_arr = StringToByteArray(FlashAddrText);
                RS485_WriteDataBuffer[2 + svns] = 0x62;                                //Cmd
                RS485_WriteDataBuffer[3 + svns] = (byte)((0x0E + PacketSize) / 256);   //Size
                RS485_WriteDataBuffer[4 + svns] = (byte)((0x0E + PacketSize) % 256);   //Size
                RS485_WriteDataBuffer[5 + svns] = flash_addr_arr[0];                   //Address
                RS485_WriteDataBuffer[6 + svns] = flash_addr_arr[1];                   //Address
                RS485_WriteDataBuffer[7 + svns] = flash_addr_arr[2];                   //Address
                RS485_WriteDataBuffer[8 + svns] = (byte)(PacketSize / 256);            //Packet Size
                RS485_WriteDataBuffer[9 + svns] = (byte)(PacketSize % 256);            //Packet Size
                for (UInt16 j = 0; j < PacketSize; j++)
                    //mvars.RS485_WriteDataBuffer[10 + 1 + j] = Convert.ToByte((string)Bin_Wr_dGV.Rows[i * (PacketSize / 16) + j / 16 + 1].Cells[(j % 16) + 1].Value, 16);
                    RS485_WriteDataBuffer[10 + 1 + j] = gBinArr[j];
                funSendMessageTo();

            }
        }


        public void LT8668_ScaleONOPFF(Byte ScalerOff, string cmdName)   //0x8A
        {
            #region 2023版公用程序 (無 Nova 參數)
            byte svns = 1;
            Array.Resize(ref RS485_WriteDataBuffer, 513);
            Array.Resize(ref ReadDataBuffer, 513);
            Array.Clear(ReadDataBuffer, 0, ReadDataBuffer.Length);
            Array.Clear(RS485_WriteDataBuffer, 0, RS485_WriteDataBuffer.Length);
            #endregion
            RS485_WriteDataBuffer[2 + svns] = 0x8A;              //Cmd
            RS485_WriteDataBuffer[3 + svns] = 0x00;              //Size
            RS485_WriteDataBuffer[4 + svns] = 0x0D;              //Size
            RS485_WriteDataBuffer[5 + svns] = 0xB1;              //Wr mcu parameter cmd.
            RS485_WriteDataBuffer[6 + svns] = 0x67;              //Key
            RS485_WriteDataBuffer[7 + svns] = 0x01;              //Length
            RS485_WriteDataBuffer[8 + svns] = ScalerOff;         //On/Off
            funSendMessageTo();
        }


        public void LT8668_Reset_L(string cmdName)   //0x83
        {
            #region 2023版公用程序 (無 Nova 參數)
            byte svns = 1;
            Array.Resize(ref RS485_WriteDataBuffer, 513);
            Array.Resize(ref ReadDataBuffer, 513);
            Array.Clear(ReadDataBuffer, 0, ReadDataBuffer.Length);
            Array.Clear(RS485_WriteDataBuffer, 0, RS485_WriteDataBuffer.Length);
            #endregion
            RS485_WriteDataBuffer[2 + svns] = 0x83;                                            //Cmd
            RS485_WriteDataBuffer[3 + svns] = 0x00;                                            //Size
            RS485_WriteDataBuffer[4 + svns] = 0x0B;                                            //Size
            RS485_WriteDataBuffer[5 + svns] = 0x01;                                            //Reset IO
            RS485_WriteDataBuffer[6 + svns] = 0x00;                                            //L
            funSendMessageTo();
        }

        public void LT8668_Reset_H(string cmdName)   //0x83
        {
            #region 2023版公用程序 (無 Nova 參數)
            byte svns = 1;
            Array.Resize(ref RS485_WriteDataBuffer, 513);
            Array.Resize(ref ReadDataBuffer, 513);
            Array.Clear(ReadDataBuffer, 0, ReadDataBuffer.Length);
            Array.Clear(RS485_WriteDataBuffer, 0, RS485_WriteDataBuffer.Length);
            #endregion
            RS485_WriteDataBuffer[2 + svns] = 0x83;                                            //Cmd
            RS485_WriteDataBuffer[3 + svns] = 0x00;                                            //Size
            RS485_WriteDataBuffer[4 + svns] = 0x0B;                                            //Size
            RS485_WriteDataBuffer[5 + svns] = 0x01;                                            //Reset IO
            RS485_WriteDataBuffer[6 + svns] = 0x01;                                            //H            funSendMessageTo();
            funSendMessageTo();
        }
    }
}
