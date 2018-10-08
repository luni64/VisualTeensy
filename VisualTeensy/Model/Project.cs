using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace VisualTeensy.Model
{
    public class ProjectData
    {
        public SetupTypes setupType { get; set; }

        public string path { get; set; } = string.Empty;
        public string pathError
        {
            get
            {
                try
                {
                    Path.GetFullPath(path);
                    return null;
                }
                catch { return "Path to the project folder not valid"; }
            }
        }
        public string name => Path.GetFileName(path ?? "ERROR");

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
              
        public List<Board> boards { get; private set; }
        public Board selectedBoard { get; set; }

        public ProjectData(SetupData settings)
        {
            this.setup = settings;
           
            //sharedLibs = new RepositoryIndexJson("Shared", settings.libBase);
            //localLibs = new RepositoryIndexJson("Local", "lib");
        }

        static public ProjectData open(string projectPath, SetupData setup)
        {
            log.Info($"open project {projectPath}");

            var p = new ProjectData(setup);

            var configFile = Path.Combine(projectPath, ".vscode", "visual_teensy.json");
            if (!File.Exists(configFile))
            {
                log.Warn($"config file {configFile} does not exist");
                return null;
            }
            try
            {
                string jsonString = File.ReadAllText(configFile);
                log.Debug("config file content:\n" + jsonString);

                var transferData = JsonConvert.DeserializeObject<vtTransferData>(jsonString);
                log.Debug("Deserialize OK");

                if (transferData != null)
                {
                    p.path = projectPath;

                    p.setupType = transferData.setupType;
                    p.compilerBase = transferData.configurations[0].compilerBase;

                    p.boardTxtPath = transferData.configurations[0].boardTxtPath.StartsWith("\\") ? Path.Combine(projectPath, transferData.configurations[0].boardTxtPath.Substring(1)) : transferData.configurations[0].boardTxtPath;
                    p.coreBase = transferData.configurations[0].coreBase.StartsWith("\\") ? Path.Combine(projectPath, transferData.configurations[0].coreBase.Substring(1)) : transferData.configurations[0].coreBase;

                    p.makefileExtension = transferData.configurations[0].makefileExtension;

                    p.parseBoardsTxt();

                    p.selectedBoard = p.boards?.FirstOrDefault(b => b.name == transferData.configurations[0].board.name);
                    if (p.selectedBoard != null)
                    {
                        foreach (var option in transferData.configurations[0].board.options)
                        {
                            var optionSet = p.selectedBoard.optionSets.FirstOrDefault(x => x.name == option.Key);
                            if (optionSet != null)
                            {
                                optionSet.selectedOption = optionSet.options.FirstOrDefault(x => x.name == option.Value);
                            }
                        }
                    }


                   // model.libManager.open(transferData.configurations[0].repositories);

                    //var x = transferData.configurations[0].repositories;


                    //var tlibs = transferData.configurations[0].repositories.FirstOrDefault(l => l.name == "shared")?.libraries;

                    //foreach (var lib in tlibs)
                    //{
                    //    //var selectedLib = p.sharedLibs.libraries.FirstOrDefault(l => l.name.ToUpper() == lib.ToUpper());
                    //    //if (selectedLib != null) selectedLib.isSelected = true;
                    //}

                    log.Info($"{configFile} read sucessfully");
                    p.logProject();
                }
                return p;
            }
            catch (Exception ex)
            {
                log.Error("error opening project", ex);
                return null;
            }
        }

        public void logProject()
        {
            var sb = new StringBuilder();
            sb.Append("Data:\n");
            sb.Append($"setupType:\t{setupType}\n");
            sb.Append($"path:\t\t{path}\n");
            sb.Append($"boardTxtPath:\t{boardTxtPath}\n");
            sb.Append($"compilerBase:\t{compilerBase}\n");
            sb.Append($"coreBase:\t{coreBase}\n");
            sb.Append($"selectedBoard:\t{selectedBoard?.name}");
            log.Debug(sb.ToString());
        }

        public static ProjectData getDefault(SetupData setupData)
        {
            log.Info("enter");
            var pd = new ProjectData(setupData);

            pd.setupType = SetupTypes.quick;

            // Project Path -------------------------------------
            int i = 1;
            pd.path = Path.Combine(setupData.projectBaseDefault, $"newProject");
            while (Directory.Exists(pd.path)) { pd.path = Path.Combine(setupData.projectBaseDefault, $"newProject({i++})"); }

            pd.boardTxtPath = setupData.arduinoBoardsTxt;
            pd.coreBase = setupData.arduinoCore;
            pd.compilerBase = setupData.arduinoCompiler;

            pd.boards = new List<Board>();
            pd.parseBoardsTxt();

            pd.logProject();

            return pd;
        }

        public void parseBoardsTxt()
        {
            log.Info("enter");

            vtTransferData.vtBoard tmp = new vtTransferData.vtBoard(selectedBoard);

            string boardsTxt = setupType == SetupTypes.quick ? setup.arduinoBoardsTxt : boardTxtPath;
            boards = FileContent.parse(boardsTxt).Where(b => b.core == "teensy3").ToList();

            setBoardOptions(tmp);
        }


        void setBoardOptions(vtTransferData.vtBoard boardInfo)
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

        SetupData setup;

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}

