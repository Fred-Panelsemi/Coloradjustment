using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanelSemi_Coloradjustment
{
    internal partial class EasyDebug
    {
        private TotalProcess mTotalProcess;
        public EasyDebug(TotalProcess TLP) 
        {
            mTotalProcess= TLP;
        }
        public void ID_Serialmode()
        {
            mTotalProcess.deviceID = "0500";
            string svdeviceID = mTotalProcess.deviceID;
            if (mTotalProcess.deviceID.Substring(0, 2) == "05") mTotalProcess.deviceID = "05A0";
            mTotalProcess.lblCmd = "PRIID_SERIESMODE";
            mTotalProcess.mIDSERIESMODE();
            //mTotalProcess.mAUTOID("1");

        }
    }
}
