using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MediaCenter.UI;
using Microsoft.MediaCenter;
using OMLEngine.Settings;
using System.Collections;

namespace Library.Code.V3
{
    public class FirstRun : ModelItem
    {
        public bool CancelCommandEnabled { get; set; }
        private Command cancelCommand;
        public Command CancelCommand
        {
            get { return cancelCommand; }
            set
            {
                if (cancelCommand != value)
                {
                    cancelCommand = value;
                    FirePropertyChanged("CancelCommand");
                }
            }
        }

        private Command backCommand;
        public Command BackCommand
        {
            get { return backCommand; }
            set
            {
                if (backCommand != value)
                {
                    backCommand = value;
                    FirePropertyChanged("BackCommand");
                }
            }
        }

        private Command nextCommand;
        public Command NextCommand
        {
            get { return nextCommand; }
            set
            {
                if (nextCommand != value)
                {
                    nextCommand = value;
                    FirePropertyChanged("NextCommand");
                }
            }
        }

        private Command finishCommand;
        public Command FinishCommand
        {
            get { return finishCommand; }
            set
            {
                if (finishCommand != value)
                {
                    finishCommand = value;
                    FirePropertyChanged("FinishCommand");
                }
            }
        }
        public FirstRun()
        {
            this.finishCommand = new Command();
        }
    }
}
