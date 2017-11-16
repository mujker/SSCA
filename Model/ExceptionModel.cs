using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Windows.Controls;

namespace SSCA.Model
{
    public class ExceptionModel : ViewModelBase
    {
        private DateTime _exTime;
        private string _exLevel;
        private string _exMessage;

        public DateTime ExTime
        {
            get { return _exTime; }

            set
            {
                _exTime = value;
                OnPropertyChanged("ExTime");
            }
        }

        public string ExLevel
        {
            get { return _exLevel; }

            set
            {
                _exLevel = value;
                OnPropertyChanged("ExLevel");
            }
        }

        public string ExMessage
        {
            get { return _exMessage; }

            set
            {
                _exMessage = value;
                OnPropertyChanged("ExMessage");
            }
        }
    }

    public enum ExEnum
    {
        Infor,
        Error,
    }
}