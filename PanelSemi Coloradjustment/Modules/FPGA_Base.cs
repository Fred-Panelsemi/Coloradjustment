using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanelSemi_Coloradjustment
{
    internal partial class TotalProcess
    {
        public string verFPGA;
        public string[] verFPGAm;
        public int pvindex = 0;
        public byte FPGAsel = 0;

        public UInt32 FPGA_START_ADDR = 0x60000;
        public UInt32 GAMMA_START_ADDR = 0x62000;

        /// <summary>
        /// FPGA Version
        /// </summary>
        public void mhFPGASPIREAD()                                          // 0x10 2022版公用程序加入 
        {
            pvindex = 0;
            // line 4890有使用問題 svverFPGA = mvars.verFPGA.Split('-')[mvars.flashselQ-1];  為什麼使用mvars.flashselQ
            // line 5015有使用問題 
            /*
            if (MultiLanguage.DefaultLanguage == "en-US")
                lst_get1.Items.Add(" -> " + svtitle +
                " write OK but Version check : " + svverFPGA + " >> " + mvars.verFPGA.Split('-')[svflashQ - 1]);
            else if (MultiLanguage.DefaultLanguage == "zh-CHT")
                lst_get1.Items.Add(" -> " + svtitle +
                " 寫入完成後版本確認 : " + svverFPGA + " >> " + mvars.verFPGA.Split('-')[svflashQ - 1]);
            else if (MultiLanguage.DefaultLanguage == "zh-CN")
                lst_get1.Items.Add(" -> " + svtitle +
                " 写入完成后版本确认 : " + svverFPGA + " >> " + mvars.verFPGA.Split('-')[svflashQ - 1]);
            */
            #region 2023版公用程序
            byte svns = 2;
            if (svnova)
            {
                Delaymillisec = 5; NumBytesToRead = 28;
                Array.Resize(ref RS485_WriteDataBuffer, svns + 0x0B);
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

            RS485_WriteDataBuffer[2 + svns] = 0x10;   ///Cmd
            RS485_WriteDataBuffer[svns + 3] = 0x00;   ///Size
            RS485_WriteDataBuffer[svns + 4] = 0x0B;   ///Size    
            RS485_WriteDataBuffer[5 + svns] = (byte)((pvindex / 256) | 0x80); //Reg Address
            RS485_WriteDataBuffer[6 + svns] = (byte)(pvindex % 256);          //Reg Address
            funSendMessageTo();
        }

        public void mhFPGASPIWRITE(byte svFsel, int svData)                  // 0x11 0500 相容 
        {

            #region 2023版公用程序
            byte svns = 2;
            if (svnova)
            {
                Delaymillisec = 5; NumBytesToRead = 28;
                if (deviceID.Substring(0, 2) == "05") Array.Resize(ref RS485_WriteDataBuffer, svns + 0x0E);
                else Array.Resize(ref RS485_WriteDataBuffer, svns + 0x0D);
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


            //if (mvars.deviceID.Substring(0, 2) == "05" || mvars.deviceID.Substring(0, 2) == "06")
            //{
            RS485_WriteDataBuffer[2 + svns] = 0x11;                           ///Cmd
            RS485_WriteDataBuffer[3 + svns] = 0x00;                           ///Size
            RS485_WriteDataBuffer[4 + svns] = 0x0E;                           ///Size
                                                                              ///byte[] WriteSizeArr = { mvars.RS485_WriteDataBuffer[4 + 1], mvars.RS485_WriteDataBuffer[3 + 1] };
                                                                              ///UInt16 WriteSize = BitConverter.ToUInt16(WriteSizeArr, 0);
            RS485_WriteDataBuffer[5 + svns] = svFsel;                         ///0:ABCD 1:EFGH 2:Boardcast
            RS485_WriteDataBuffer[6 + svns] = (byte)(pvindex / 256);    ///Reg Address
            RS485_WriteDataBuffer[7 + svns] = (byte)(pvindex % 256);    ///Reg Address
            RS485_WriteDataBuffer[8 + svns] = (byte)(svData / 256);           ///Reg Data
            RS485_WriteDataBuffer[9 + svns] = (byte)(svData % 256);           ///Reg Data
            //}
            //else
            //{
            //    mvars.RS485_WriteDataBuffer[2 + svns] = 0x11;          //Cmd
            //    mvars.RS485_WriteDataBuffer[3 + svns] = 0x00;          //Size
            //    mvars.RS485_WriteDataBuffer[4 + svns] = 0x0D;          //Size
            //                                                           //byte[] WriteSizeArr = { mvars.RS485_WriteDataBuffer[4 + 1], mvars.RS485_WriteDataBuffer[3 + 1] };
            //                                                           //UInt16 WriteSize = BitConverter.ToUInt16(WriteSizeArr, 0);
            //    mvars.RS485_WriteDataBuffer[5 + svns] = (byte)(Form1.pvindex / 256);            //Reg Address
            //    mvars.RS485_WriteDataBuffer[6 + svns] = (byte)(Form1.pvindex % 256);            //Reg Address
            //    mvars.RS485_WriteDataBuffer[7 + svns] = (byte)(svData / 256);  //Reg Data
            //    mvars.RS485_WriteDataBuffer[8 + svns] = (byte)(svData % 256);  //Reg Data
            //}

            funSendMessageTo();
        }

        public void mhFPGASPIWRITE(byte svFsel, int[] svreg, int[] svdata)   // 0x11 0500 相容 
        {
            #region 2023版公用程序
            byte svns = 2;
            if (svnova)
            {
                Delaymillisec = 5; NumBytesToRead = 28;
                if (deviceID.Substring(0, 2) == "05") Array.Resize(ref RS485_WriteDataBuffer, svns + (svreg.Length * 5 + 10));
                else Array.Resize(ref RS485_WriteDataBuffer, svns + (svreg.Length * 4 + 9));
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

            if (deviceID.Substring(0, 2) == "05" || deviceID.Substring(0, 2) == "06")
            {
                RS485_WriteDataBuffer[2 + svns] = 0x11;                                           ///Cmd
                RS485_WriteDataBuffer[3 + svns] = Convert.ToByte((svreg.Length * 5 + 10) / 256);  ///Size
                RS485_WriteDataBuffer[4 + svns] = Convert.ToByte((svreg.Length * 5 + 10) % 256);  ///Size
                for (int j = 0; j < svreg.Length; j++)
                {
                    RS485_WriteDataBuffer[5 + (j * 5) + svns] = svFsel;                                               ///0:ABCD 1:EFGH 2:Boardcast
                    RS485_WriteDataBuffer[6 + (j * 5) + svns] = Convert.ToByte(svreg[j] / 256);                       ///Addr
                    RS485_WriteDataBuffer[7 + (j * 5) + svns] = Convert.ToByte(svreg[j] % 256);                       ///Addr
                    RS485_WriteDataBuffer[8 + (j * 5) + svns] = Convert.ToByte(Convert.ToInt32(svdata[j]) / 256);     ///Data
                    RS485_WriteDataBuffer[9 + (j * 5) + svns] = Convert.ToByte(Convert.ToInt32(svdata[j]) % 256);     ///Data
                }
            }
            else
            {
                RS485_WriteDataBuffer[2 + svns] = 0x11;                                           //Cmd
                RS485_WriteDataBuffer[3 + svns] = Convert.ToByte((svreg.Length * 4 + 9) / 256);   //Size
                RS485_WriteDataBuffer[4 + svns] = Convert.ToByte((svreg.Length * 4 + 9) % 256);   //Size
                                                                                                        //byte[] WriteSizeArr = { mvars.RS485_WriteDataBuffer[4 + 1], mvars.RS485_WriteDataBuffer[3 + 1] };
                                                                                                        //UInt16 WriteSize = BitConverter.ToUInt16(WriteSizeArr, 0);
                for (int j = 0; j < svreg.Length; j++)
                {
                    RS485_WriteDataBuffer[5 + (j * 4) + svns] = Convert.ToByte(svreg[j] / 256);                       //Addr
                    RS485_WriteDataBuffer[6 + (j * 4) + svns] = Convert.ToByte(svreg[j] % 256);                       //Addr
                    RS485_WriteDataBuffer[7 + (j * 4) + svns] = Convert.ToByte(Convert.ToInt32(svdata[j]) / 256);     //Data
                    RS485_WriteDataBuffer[8 + (j * 4) + svns] = Convert.ToByte(Convert.ToInt32(svdata[j]) % 256);     //Data
                }
            }



            funSendMessageTo();
        }

        public void mpFPGAUIREGWarr(string[] RegDec, string[] DataDec)       //0x11 multi register
        {
            #region 2023版公用程序(w/o novastar，帶 PacketSize)
            byte svns = 1;
            int PacketSize = 9 + RegDec.Length * 5 + 1;
            Array.Resize(ref RS485_WriteDataBuffer, 8192);
            Array.Resize(ref ReadDataBuffer, 513);
            Array.Clear(ReadDataBuffer, 0, ReadDataBuffer.Length);
            Array.Clear(RS485_WriteDataBuffer, 0, RS485_WriteDataBuffer.Length);
            #endregion

            RS485_WriteDataBuffer[2 + svns] = 0x13;       //Cmd
            byte[] bytes = BitConverter.GetBytes(PacketSize);
            RS485_WriteDataBuffer[3 + svns] = bytes[1];   //Size
            RS485_WriteDataBuffer[4 + svns] = bytes[0];   //Size
            //DataDec[98] = "2400";
            
            for (uint i = 0; i < RegDec.Length; i++)
            {
                RS485_WriteDataBuffer[5 + svns + i * 5] = FPGAsel;                                      //FPGA SELECT
                RS485_WriteDataBuffer[6 + svns + i * 5] = Convert.ToByte(Convert.ToInt32(RegDec[i]) / 256);   //Reg Address
                RS485_WriteDataBuffer[7 + svns + i * 5] = Convert.ToByte(Convert.ToInt32(RegDec[i]) % 256);   //Reg Address
                RS485_WriteDataBuffer[8 + svns + i * 5] = Convert.ToByte(Convert.ToInt32(DataDec[i]) / 256);  //Reg Data
                RS485_WriteDataBuffer[9 + svns + i * 5] = Convert.ToByte(Convert.ToInt32(DataDec[i]) % 256);  //Reg Data
            }
            funSendMessageTo();
        }
    }
}
