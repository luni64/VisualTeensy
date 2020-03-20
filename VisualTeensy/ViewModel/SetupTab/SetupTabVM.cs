using System;
using System.ComponentModel;
using System.Drawing;
using vtCore;
using vtCore.Interfaces;

namespace ViewModel
{
    public class SetupTabVM : BaseViewModel, IDataErrorInfo
    {     
        public string Error => "ERROR";
        public string this[string columnName]
        {
            get
            {
                string error;

                switch (columnName)
                {
                    case "arduinoBase":
                        error = project.selectedConfiguration.setupType == SetupTypes.quick ? setup.arduinoBaseError : null;
                        break;

                    case "projectPathDefault":
                        error = project.pathError;
                        break;

                    case "makePath":
                        error = setup.makeExeBase.error;
                        break;

                    case "uploadTyPath":
                        error = setup.uplTyBase.error;
                        break;

                    case "uploadPjrcPath":
                        error = setup.uplPjrcBase.error;
                        break;

                    case "uploadCLIPath":
                        error = setup.uplCLIBase.error;
                        break;

                    case "uploadJLinkPath":
                        error = setup.uplJLinkBase.error;
                        break;

                    default:
                        error = null;
                        break;
                }
                return error;
            }
        }

        // Folders
        public String arduinoBase
        {
            get => setup.arduinoBase;
            set
            {
                if (value != setup.arduinoBase)
                {
                    setup.arduinoBase = value;
                    OnPropertyChanged("");
                }
            }
        }
        public String makePath
        {
            get => setup.makeExeBase.path;
            set
            {
                if (value != setup.makeExeBase.path)
                {
                    setup.makeExeBase.path = value.Trim();
                    OnPropertyChanged();
                }
            }
        }
        public String uploadTyPath
        {
            get => setup.uplTyBase.path;
            set
            {
                if (value != setup.uplTyBase.path)
                {
                    setup.uplTyBase.path = value;
                    OnPropertyChanged();
                }
            }
        }
        public String uploadJLinkPath
        {
            get => setup.uplJLinkBase.path;
            set
            {
                if (value != setup.uplJLinkBase.path)
                {
                    setup.uplJLinkBase.path = value;
                    OnPropertyChanged();
                }
            }
        }
        public String uploadPjrcPath
        {
            get => setup.uplPjrcBase.path;
            set
            {
                if (value != setup.uplPjrcBase.path)
                {
                    setup.uplPjrcBase.path = value;
                    OnPropertyChanged();
                }
            }
        }
        public String uploadCLIPath
        {
            get => setup.uplCLIBase.path;
            set
            {
                if (value != setup.uplCLIBase.path)
                {
                    setup.uplCLIBase.path = value;
                    OnPropertyChanged();
                }
            }
        }
        public String projectBaseDefault
        {
            get => setup.projectBaseDefault;
            set
            {
                if (value != setup.projectBaseDefault)
                {
                    setup.projectBaseDefault = value;
                    OnPropertyChanged();
                }
            }
        }

        // Targets
        public bool isTargetvsCode
        {
            get => project.target == Target.vsCode;
            set
            {
                if (value == true) project.target = Target.vsCode;
                OnPropertyChanged();
            }
        }
        public bool isTargetATOM
        {
            get => project.target == Target.atom;
            set
            {
                if (value == true) project.target = Target.atom;
                OnPropertyChanged();
            }
        }

        // Build System
        public bool isMakefileBuild
        {
            get => project.buildSystem == BuildSystem.makefile;
            set
            {
                if (value == true) project.buildSystem = BuildSystem.makefile;
                OnPropertyChanged("");
            }
        }
        public bool isArduinoBuild
        {
            get => project.buildSystem == BuildSystem.arduino;
            set
            {
                if (value == true) project.buildSystem = BuildSystem.arduino;
                OnPropertyChanged();
            }
        }
       

        // Debugging
        public bool hasDebugSupport
        {
            get => setup.debugSupportDefault;
            set
            {
                if (setup.debugSupportDefault != value)
                {
                    setup.debugSupportDefault = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool isDebugEnabled
        {
            get => project.debugSupport == DebugSupport.cortex_debug;
            set
            {
                project.debugSupport = value == true ? DebugSupport.cortex_debug : DebugSupport.none;
                OnPropertyChanged();
            }
        }

        // Colored Output
        public bool isColorEnabled
        {
            get => setup.isColoredOutput;
            set
            {
                setup.isColoredOutput = value;
                OnPropertyChanged("");
            }
        }
        public Color colorCore
        {
            get => isColorEnabled && isMakefileBuild ? setup.colorCore : Color.LightGray;
            set
            {
                setup.colorCore = value;
                OnPropertyChanged();
            }
        }
        public Color colorLib
        {
            get => isColorEnabled && isMakefileBuild ? setup.colorUserLib : Color.LightGray;
            set
            {
                setup.colorUserLib = value;
                OnPropertyChanged();
            }
        }
        public Color colorSrc
        {
            get => isColorEnabled && isMakefileBuild ? setup.colorUserSrc : Color.LightGray;
            set
            {
                setup.colorUserSrc = value;
                OnPropertyChanged();
            }
        }
        public Color colorLink
        {
            get => isColorEnabled && isMakefileBuild ? setup.colorLink : Color.LightGray;
            set
            {
                setup.colorLink = value;
                OnPropertyChanged();
            }
        }
        public Color colorOk
        {
            get => isColorEnabled && isMakefileBuild ? setup.colorOk : Color.LightGray;
            set
            {
                setup.colorOk = value;
                OnPropertyChanged();
            }
        }
        public Color colorErr
        {
            get => isColorEnabled && isMakefileBuild ? setup.colorErr : Color.LightGray;
            set
            {
                setup.colorErr = value;
                OnPropertyChanged();
            }
        }
        
        public SetupTabVM(IProject project, SetupData setup)
        {
            this.project = project;
            this.setup = setup;
        }

        private readonly SetupData setup;
        private readonly IProject project;
    }
}

