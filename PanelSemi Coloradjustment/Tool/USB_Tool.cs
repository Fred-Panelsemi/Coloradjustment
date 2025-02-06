using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
    internal partial class TotalProcess
    {
        public SerialPort sp1 = null;

        /// <summary>
        /// 開起COM Port
        /// </summary>
        /// <param name="svcom">COM Port Name</param>
        /// <returns></returns>
        public string Sp1open(string svcom,out bool portIsconnect)
        {
            portIsconnect = false;
            try
            {
                if (sp1.IsOpen) {sp1.Close(); }
                sp1.PortName = svcom;
                sp1.RtsEnable = true;
                if (!sp1.IsOpen) { sp1.Open(); }
                Console.WriteLine("SUCCESS : "+ sp1.PortName + " ： Open");
                portIsconnect = true;
                return sp1.PortName + "：Open";
                
            }
            catch (Exception ex)
            {
                portIsconnect = false;
                return "false," + ex.Message;
            }
        }

        public void CommClose()
        {
            if (flgSendmessage == false)
            {
                try { sp1.Close(); }
                catch (Exception ex)
                {
                    // 未來需要控件補上 error code
                    Console.WriteLine(ex);
                }
            }
        }

        /// <summary>
        /// 找尋ComPort
        /// </summary>
        /// <returns></returns>
        public string FindComPort()
        {
            try
            {
                if (sp1.IsOpen) sp1.Close();
            }
            catch (Exception ex)
            {
                //Form1.lstget1.Items.Add(ex.Message);
                Console.WriteLine(ex.Message);
                return "";
            }
            // 查詢COM Port
            List<string> comports = ComPortNames("04D8", "000A");
            if (comports.Count == 0)
            {
                
                return "";
            }
            else
            {
                Array.Resize(ref CommArray, comports.Count);
                string usbCom_infor = $"已搜尋到USB COM {comports.Count} 個 : ";
                for (int i = 0; i < comports.Count; i++)
                {
                    usbCom_infor = usbCom_infor + $"{CommArray[i]} ";
                }
                
                for (int i = 0; i < comports.Count; i++) CommArray[i] = comports[i];
                return comports[0];
            }
        }

        // 此區COM Port 的查找依賴於 Window 的註冊表中查詢 
        // 然而 此方法在通訊傳輸上比較沒關係
        // 如果要通傳輸上的應用 需要 呼叫 system.io.port的外部API
        // 外部API 在資料傳輸上以及開啟port口上會較為方便
        public static List<string> ComPortNames(String VID, String PID)
        {
            // 創建一個用於存儲 COM 埠名稱的列表
            List<string> comports = new List<string>();
            try
            {
                // 定義一個正則表達式模式，用於匹配 VID 和 PID
                String pattern = String.Format("^VID_{0}.PID_{1}", VID, PID);
                Regex _rx = new Regex(pattern, RegexOptions.IgnoreCase);

                // 打開 Windows 註冊表中的 LocalMachine 鍵
                RegistryKey rk1 = Registry.LocalMachine;
                // 打開 SYSTEM\CurrentControlSet\Enum 鍵
                RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");

                // 遍歷 Enum 鍵中的所有子鍵
                foreach (String s3 in rk2.GetSubKeyNames())
                {
                    RegistryKey rk3 = rk2.OpenSubKey(s3);
                    foreach (String s in rk3.GetSubKeyNames())
                    {
                        // 如果子鍵名稱匹配正則表達式，則進一步查找
                        if (_rx.Match(s).Success)
                        {

                            RegistryKey rk4 = rk3.OpenSubKey(s);
                            foreach (String s2 in rk4.GetSubKeyNames())
                            {
                                try
                                {
                                    // 打開 Device Parameters 鍵，並獲取 PortName 值
                                    RegistryKey rk5 = rk4.OpenSubKey(s2);
                                    string location = (string)rk5.GetValue("LocationInformation");
                                    RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
                                    string portName = (string)rk6.GetValue("PortName");
                                    // 如果 PortName 不為空，且在系統中存在，則添加到列表中
                                    if (!String.IsNullOrEmpty(portName) && SerialPort.GetPortNames().Contains(portName))
                                        comports.Add((string)rk6.GetValue("PortName"));
                                }
                                catch
                                {
                                    // 忽略任何異常，繼續處理下一個子鍵
                                    continue;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Form1.lstget1.Items.Add(ex.Message);
            }
            return comports;
        }

        public string USB_Error_Handler(string sErrCode)
        {
            switch (sErrCode)
            {
                //
                case "00A2": return "ERROR_TX_PACKET_TIMEOUT";
                //FPGA
                case "1001": return "FPGA DONE";
                //I2C
                case "2004": return "ERROR_I2C_TIMEOUT";
                case "2005": return "ERROR_I2C_SDA_SCL_LOW";
                case "2006": return "ERROR_I2C_NACK";
                case "2007": return "ERROR_I2C_ERR_BUS";
                case "2008": return "ERROR_I2C_CLKHOLD";
                case "2010": return "ERROR_I2C_ADDR_NACK";
                case "2011": return "ERROR_I2C_DATA_NACK";
                case "2012": return "ERROR_I2C_SCL_LOW";
                case "2013": return "ERROR_I2C_SDA_LOW";
                case "2014": return "ERROR_I2C_BUS_LOW";
                //SPI
                case "3010": return "SPI_FLASH_STATUS_BUSY";
                case "3011": return "SPI_FLASH_STATUS_ERROR_UNKNOWN";
                case "3012": return "SPI_FLASH_WE_ERROR_UNKNOWN";
                case "3013": return "SPI_FLASH_256_ERROR";
                case "3014": return "SPI_FLASH_WR_CHK_TIMEOUT";
                case "301F": return "SPI_FLASH_ERROR";
                case "3020": return "SPI_ERROR_CODE_BUSY";
                case "3021": return "SPI_ERROR_CODE_256";
                case "3022": return "SPI_ERROR_CODE_WR_TIMEOUT";
                case "3023": return "SPI_ERROR_CODE_WE";
                case "3024": return "SPI_ERROR_CODE_SECTOR_ERASE";
                case "3025": return "SPI_ERROR_CODE_BUSY_TIMEOUT";
                case "3026": return "SPI_ERROR_CODE_MEM_CMP";
                //QSPI
                case "3100": return "QSPI_ERROR_CODE_BUSY";
                case "3101": return "QSPI_ERROR_CODE_256";
                case "3102": return "QSPI_ERROR_CODE_WR_TIMEOUT";
                case "3103": return "QSPI_ERROR_CODE_WE";
                case "3104": return "QSPI_ERROR_CODE_SECTOR_ERASE";
                case "3105": return "QSPI_ERROR_CODE_BUSY_TIMEOUT";
                case "3106": return "QSPI_ERROR_CODE_MEM_CMP";
                case "31FF": return "QSPI_ERROR_CODE_UNKNOWN";
                //MCU Flash
                case "4001": return "MCU_FLASH_ADDR_ERROR";
                case "4002": return "MCU_FLASH_CMP_ERROR";

                default: return "Inpub Buffer Return NG";
            }
        }
    }
}
