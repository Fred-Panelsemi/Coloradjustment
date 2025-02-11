using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CtLib.Library.Contracts;
using CtLib.Library;
using System.Windows.Input;
using System.Windows;

namespace PanelSemi_Coloradjustment
{
    internal partial class MainProcess
    {
        public override ICommand CleanAlarm => throw new NotImplementedException();

        protected override ICAMProConfig<Panelsemi> Config => throw new NotImplementedException();

        public override Task SystemDispose()
        {
            
            throw new NotImplementedException();
        }

        public override Task SystemInitial()
        {
            throw new NotImplementedException();
        }

        protected override void ChangeBuzzer(bool state)
        {
            throw new NotImplementedException();
        }

        protected override IEquipError<Panelsemi> CreateUnknown(Panelsemi equip, int code)
        {
            throw new NotImplementedException();
        }

        protected override void CultureChanged(CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        protected override void EquipSttChanged(EquipState stt)
        {
            throw new NotImplementedException();
        }

        protected override string GetSignCulture(bool inOut)
        {
            throw new NotImplementedException();
        }

        protected override void ModeChanged(OperateMode mode)
        {
            throw new NotImplementedException();
        }
    }
}
