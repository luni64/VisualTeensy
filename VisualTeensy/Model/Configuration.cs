using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace VisualTeensy.Model
{
    public class Configuration
    {
        public SetupTypes setupType { get; set; }

      //  public string path { get; set; } = string.Empty;
       

        // boards.txt --------------------------------
        public string boardTxtPath { get; set; }
        public string boardTxtPathError => (!String.IsNullOrWhiteSpace(boardTxtPath) && File.Exists(boardTxtPath)) ? null : "Error";
        public bool copyBoardTxt { get; set; }

        // compilerBase ------------------------------
        public string compilerBase { get; set; }
        public string compilerPathError
        {
            get
            {
                if (!String.IsNullOrEmpty(compilerBase) && (Directory.Exists(compilerBase)))
                {
                    string gcc = Path.Combine(compilerBase, @"bin\arm-none-eabi-gcc.exe");
                    if (File.Exists(gcc))
                    {
                        return null;
                    }
                    return @".\bin\arm-none-eabi-gcc.exe not found in the specified directory. Please select a valid arm-none-eabi gcc folder";
                }
                return "Folder doesn't exist";
            }
        }

        // core --------------------------------------
        public string coreBase { get; set; }
        public string corePathError
        {
            get
            {
                if (!String.IsNullOrEmpty(coreBase) && (Directory.Exists(coreBase)))
                {
                    string uploader = Path.Combine(coreBase, "Arduino.h");
                    if (File.Exists(uploader))
                    {
                        return null;
                    }
                    return "Arduino.h not found in the specified folder. Doesn't seem to be valid arduino core";
                }
                return "Folder doesn't exist";
            }
        }
        public bool copyCore { get; set; }

        // makefile extension ------------------------
        public string makefileExtension { get; set; }
        
        // libraries ---------------------------------
        public List<Library> sharedLibs { get; set; }
        public List<Library> localLibs { get; set; }



        public List<Board> boards { get; private set; }
        public Board selectedBoard { get; set; }

        public Configuration(SetupData settings = null)
        {
          //  this.setup = settings;
            boards = new List<Board>();
            sharedLibs = new List<Library>();
            localLibs = new List<Library>();
        }



        //static public Configuration open(string projectPath, SetupData setup)
        //{
        //    log.Info($"open project {projectPath}");

        //    var p = new Configuration(setup);

        //    var configFile = Path.Combine(projectPath, ".vscode", "visual_teensy.json");
        //    if (!File.Exists(configFile))
        //    {
        //        log.Warn($"config file {configFile} does not exist");
        //        return null;
        //    }
        //    try
        //    {
        //        string jsonString = File.ReadAllText(configFile);
        //        log.Debug("config file content:\n" + jsonString);

        //        var transferData = JsonConvert.DeserializeObject<vtTransferData>(jsonString);
        //        log.Debug("Deserialize OK");

        //        if (transferData != null)
        //        {
        //            p.path = projectPath;

        //            p.setupType = transferData.setupType;
        //            p.compilerBase = transferData.configurations[0].compilerBase;

        //            p.boardTxtPath = transferData.configurations[0].boardTxtPath.StartsWith("\\") ? Path.Combine(projectPath, transferData.configurations[0].boardTxtPath.Substring(1)) : transferData.configurations[0].boardTxtPath;
        //            p.coreBase = transferData.configurations[0].coreBase.StartsWith("\\") ? Path.Combine(projectPath, transferData.configurations[0].coreBase.Substring(1)) : transferData.configurations[0].coreBase;

        //            p.makefileExtension = transferData.configurations[0].makefileExtension;

        //            p.parseBoardsTxt();

        //            p.selectedBoard = p.boards?.FirstOrDefault(b => b.name == transferData.configurations[0].board.name);
        //            if (p.selectedBoard != null)
        //            {
        //                foreach (var option in transferData.configurations[0].board.options)
        //                {
        //                    var optionSet = p.selectedBoard.optionSets.FirstOrDefault(x => x.name == option.Key);
        //                    if (optionSet != null)
        //                    {
        //                        optionSet.selectedOption = optionSet.options.FirstOrDefault(x => x.name == option.Value);
        //                    }
        //                }
        //            }


        //           // model.libManager.open(transferData.configurations[0].repositories);

        //            //var x = transferData.configurations[0].repositories;


        //            //var tlibs = transferData.configurations[0].repositories.FirstOrDefault(l => l.name == "shared")?.libraries;

        //            //foreach (var lib in tlibs)
        //            //{
        //            //    //var selectedLib = p.sharedLibs.libraries.FirstOrDefault(l => l.name.ToUpper() == lib.ToUpper());
        //            //    //if (selectedLib != null) selectedLib.isSelected = true;
        //            //}

        //            log.Info($"{configFile} read sucessfully");
        //            p.logProject();
        //        }
        //        return p;
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("error opening project", ex);
        //        return null;
        //    }
        //}

        public void logProject()
        {
            var sb = new StringBuilder();
            sb.Append("Data:\n");
            sb.Append($"setupType:\t{setupType}\n");           
            sb.Append($"boardTxtPath:\t{boardTxtPath}\n");
            sb.Append($"compilerBase:\t{compilerBase}\n");
            sb.Append($"coreBase:\t{coreBase}\n");
            sb.Append($"selectedBoard:\t{selectedBoard?.name}");
            log.Debug(sb.ToString());
        }

        public static Configuration getDefault(SetupData setupData)
        {
            log.Info("enter");
            var pd = new Configuration(setupData);

            pd.setupType = SetupTypes.quick;

            // Project Path -------------------------------------
            //int i = 1;
           // pd.path = Path.Combine(setupData.projectBaseDefault, $"newProject");
           // while (Directory.Exists(pd.path)) { pd.path = Path.Combine(setupData.projectBaseDefault, $"newProject({i++})"); }

            pd.boardTxtPath = setupData.arduinoBoardsTxt;
            pd.coreBase = setupData.arduinoCore;
            pd.compilerBase = setupData.arduinoCompiler;

            pd.boards = new List<Board>();
            pd.parseBoardsTxt(setupData.arduinoBoardsTxt);

            pd.logProject();

            return pd;
        }

        public void parseBoardsTxt(string bt)
        {
            log.Info("enter");

            projectTransferData.vtBoard tmp = new projectTransferData.vtBoard(selectedBoard);

           // string boardsTxt = setupType == SetupTypes.quick ? setup.arduinoBoardsTxt : boardTxtPath;
            boards = FileContent.parse(bt ?? boardTxtPath).Where(b => b.core == "teensy3").ToList();

            setBoardOptions(tmp);
        }

        public string generateMakefile(SetupData setup, string path, string name, LibManager libManager)
        {
            var options = selectedBoard.getAllOptions();

            log.Debug("enter");
            StringBuilder mf = new StringBuilder();

            mf.Append("#******************************************************************************\n");
            mf.Append("# Generated by VisualTeensy (https://github.com/luni64/VisualTeensy)\n");
            mf.Append("#\n");
            mf.Append($"# {"Board",-18} {selectedBoard.name}\n");
            selectedBoard.optionSets.ForEach(o => mf.Append($"# {o.name,-18} {o.selectedOption?.name}\n"));
            mf.Append("#\n");
            mf.Append($"# {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}\n");
            mf.Append("#******************************************************************************\n");

            mf.Append($"SHELL            := cmd.exe\nexport SHELL\n\n");

            mf.Append($"TARGET_NAME      := {name?.Replace(" ", "_")}\n");

            mf.Append(makeEntry("BOARD_ID         := ", "build.board", options) + "\n\n");

            mf.Append($"LIBS_SHARED_BASE := {Helpers.getShortPath(libManager.sharedRepositoryPath)}\n");
            mf.Append($"LIBS_SHARED      := ");
            foreach (var lib in sharedLibs)
            {
                mf.Append($"{lib.path ?? lib.name} "); //hack, improve library to distinguish between libraries to download and loacal libs
            }
            mf.Append("\n\n");

            mf.Append($"LIBS_LOCAL_BASE  := lib\n");
            mf.Append($"LIBS_LOCAL       := ");
            foreach (var lib in localLibs)
            {
                mf.Append($"{lib.path ?? lib.name} "); //hack, improve library to distinguish between libraries to download and loacal libs
            }
            mf.Append("\n\n");

            if (setupType == SetupTypes.quick)
            {
                mf.Append($"CORE_BASE        := {Helpers.getShortPath(setup.arduinoCore)}\n");
                mf.Append($"GCC_BASE         := {Helpers.getShortPath(setup.arduinoCompiler)}\n");
                mf.Append($"UPL_PJRC_B       := {Helpers.getShortPath(setup.arduinoTools)}\n");
            }
            else
            {
                mf.Append($"CORE_BASE        := {((copyCore || (Path.GetDirectoryName(coreBase) == path)) ? "core" : Helpers.getShortPath(coreBase))}\n");
                mf.Append($"GCC_BASE         := {Helpers.getShortPath(compilerBase)}\n");
                mf.Append($"UPL_PJRC_B       := {Helpers.getShortPath(setup.uplPjrcBase)}\n");
            }
            mf.Append($"UPL_TYCMD_B      := {Helpers.getShortPath(setup.uplTyBase)}\n");
            mf.Append($"UPL_CLICMD_B     := {Helpers.getShortPath(setup.uplCLIBase)}\n\n");

            mf.Append(makeEntry("MCU   := ", "build.mcu", options) + "\n\n");

            mf.Append(makeEntry("FLAGS_CPU   := ", "build.flags.cpu", options) + "\n");
            mf.Append(makeEntry("FLAGS_OPT   := ", "build.flags.optimize", options) + "\n");
            mf.Append(makeEntry("FLAGS_COM   := ", "build.flags.common", options) + makeEntry(" ", "build.flags.dep", options) + "\n");
            mf.Append(makeEntry("FLAGS_LSP   := ", "build.flags.ldspecs", options) + "\n");

            mf.Append("\n");
            mf.Append(makeEntry("FLAGS_CPP   := ", "build.flags.cpp", options) + "\n");
            mf.Append(makeEntry("FLAGS_C     := ", "build.flags.c", options) + "\n");
            mf.Append(makeEntry("FLAGS_S     := ", "build.flags.S", options) + "\n");
            mf.Append(makeEntry("FLAGS_LD    := ", "build.flags.ld", options) + "\n");

            mf.Append("\n");
            mf.Append(makeEntry("LIBS        := ", "build.flags.libs", options) + "\n");
            mf.Append(makeEntry("LD_SCRIPT   := ", "build.mcu", options) + ".ld\n");

            mf.Append("\n");
            mf.Append(makeEntry("DEFINES     := ", "build.flags.defs", options) + " -DARDUINO=10807\n");
            mf.Append("DEFINES     += ");
            mf.Append(makeEntry("-DF_CPU=", "build.fcpu", options) + " " + makeEntry("-D", "build.usbtype", options) + " " + makeEntry("-DLAYOUT_", "build.keylayout", options) + "\n");

            mf.Append($"\n");
            mf.Append("CPP_FLAGS   := $(FLAGS_CPU) $(FLAGS_OPT) $(FLAGS_COM) $(DEFINES) $(FLAGS_CPP)\n");
            mf.Append("C_FLAGS     := $(FLAGS_CPU) $(FLAGS_OPT) $(FLAGS_COM) $(DEFINES) $(FLAGS_C)\n");
            mf.Append("S_FLAGS     := $(FLAGS_CPU) $(FLAGS_OPT) $(FLAGS_COM) $(DEFINES) $(FLAGS_S)\n");
            mf.Append("LD_FLAGS    := $(FLAGS_CPU) $(FLAGS_OPT) $(FLAGS_LSP) $(FLAGS_LD)\n");
            mf.Append("AR_FLAGS    := rcs\n");

            if (setupType == SetupTypes.expert && !String.IsNullOrWhiteSpace(makefileExtension))
            {
                mf.Append("\n");
                mf.Append(makefileExtension);
                mf.Append("\n");
            }

            mf.Append(setup.makefile_fixed);

            return mf.ToString();
        }

        void setBoardOptions(projectTransferData.vtBoard boardInfo)
        {
            selectedBoard = boards?.FirstOrDefault(b => b.name == boardInfo.name) ?? boards?.FirstOrDefault();
            if (selectedBoard != null)
            {
                if (boardInfo.options != null)
                {
                    foreach (var option in boardInfo.options)
                    {
                        var optionSet = selectedBoard.optionSets.FirstOrDefault(x => x.name == option.Key);
                        if (optionSet != null)
                        {
                            optionSet.selectedOption = optionSet.options.FirstOrDefault(x => x.name == option.Value);
                        }
                    }
                }
            }
            else
            {

            };
        }

        //  SetupData setup;
        private string makeEntry(String txt, String key, Dictionary<String, String> options)
        {
            if (options.ContainsKey(key))
            {
                return $"{txt}{options[key]}";
            }
            else
            {
                return "";
            }
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}

