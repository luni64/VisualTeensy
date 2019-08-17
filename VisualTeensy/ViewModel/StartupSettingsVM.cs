using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vtCore;

namespace ViewModel
{
    public class StartupSettingsVM : BaseViewModel, IDataErrorInfo
    {
        public string ArduinoFolder
        {
            get => setup.arduinoBase;
            set
            {
                setup.arduinoBase = value;
                OnPropertyChanged("");
            }
        }

        public bool isValid => String.IsNullOrWhiteSpace(ArduinoFolder) ? true : setup.arduinoBaseError == null;
        public string errorString => isValid? null : setup.arduinoBaseError;



        public string Error => "Error";

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "ArduinoFolder":
                        return isValid ? null : setup.arduinoBaseError;
                    default:
                        return "Unknown Field";
                }
            }
        }

        public StartupSettingsVM(SetupData setup)
        {
            this.setup = setup;
        }

        SetupData setup;
    }
}
