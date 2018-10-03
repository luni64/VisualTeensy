using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using log4net;
using Newtonsoft.Json;

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
              
        // boards.txt ---------------------------
        public string boardTxtPath { get; set; }
        public string boardTxtPathError => (!String.IsNullOrWhiteSpace(boardTxtPath) && File.Exists(boardTxtPath)) ? null : "Error";
        public bool copyBoardTxt { get; set; }

        // compilerBase ---------------------------
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
        public string compilerBaseShort => (compilerBase != null && compilerBase.Contains(" ")) ? Helpers.getShortPath(compilerBase) : compilerBase;

        // core -------------------------------------
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
        public string coreBaseShort => coreBase.Contains(" ") ? Helpers.getShortPath(coreBase) : coreBase;

        // libraries ---------------------------------
        //p//ublic List<string> sharedLibs { get; }

        public vtTransferData.LibraryRepositiory sharedLibraries { get; }
        public vtTransferData.LibraryRepositiory localLibraries { get; }

        public List<Board> boards { get; set; }
        public Board selectedBoard { get; set; }

        public ProjectData(SetupData settings)
        {
            this.setup = settings;
            //sharedLibs = new List<string>();
            //libraries = new List<vtTransferData.LibraryRepositiory>();
            sharedLibraries = new vtTransferData.LibraryRepositiory();
            localLibraries = new vtTransferData.LibraryRepositiory();
        }

        public bool open(string filename)
        {
            log.Info($"open project {filename}");

            var configFile = Path.Combine(filename, ".vscode", "visual_teensy.json");
            if (!File.Exists(configFile))
            {
                log.Warn($"config file {configFile} does not exist");
                return false;
            }
            try
            {
                string jsonString = File.ReadAllText(configFile);
                log.Debug("config file content:\n" + jsonString);

                var transferData = JsonConvert.DeserializeObject<vtTransferData>(jsonString);

                if (transferData != null)
                {
                    path = filename;

                    setupType = transferData.setupType;
                    compilerBase = transferData.compilerBase;

                    boardTxtPath = transferData.boardTxtPath.StartsWith("\\") ? Path.Combine(path, transferData.boardTxtPath.Substring(1)) : transferData.boardTxtPath;
                    coreBase = transferData.coreBase.StartsWith("\\") ? Path.Combine(path, transferData.coreBase.Substring(1)) : transferData.coreBase;

                    parseBoardsTxt();

                    selectedBoard = boards?.FirstOrDefault(b => b.name == transferData.board.name);
                    if (selectedBoard != null)
                    {
                        foreach (var option in transferData.board.options)
                        {
                            var optionSet = selectedBoard.optionSets.FirstOrDefault(x => x.name == option.Key);
                            if (optionSet != null)
                            {
                                optionSet.selectedOption = optionSet.options.FirstOrDefault(x => x.name == option.Value);
                            }
                        }
                    }

                    //var libs = libManager.repositories[0].libraries;

                    //foreach (string libName in transferData.libraries[0].libraries)
                    //{
                    //    var lib = libs.FirstOrDefault(l => l.name == libName);
                    //    if (lib != null) project.libraries.Add(lib);
                    //}

                    sharedLibraries.libraries.Clear();

                    sharedLibraries.libraries.AddRange(transferData.libraries);
                    sharedLibraries = transferData.libraries.FirstOrDefault(l => l.repository == "Shared");


                    log.Info($"{configFile} read sucessfully");
                    logProject();
                }
                return true;
            }
            catch (Exception ex)
            {
                log.Error("error opening project", ex);
                return false;
            }
        }

        public void logProject()
        {
            var sb = new StringBuilder();
            sb.Append("Project Information\n");
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
            var pd = new ProjectData(setupData);

            pd.setupType = SetupTypes.quick;

            // Project Path ----------------------------------------------------------------------------------------------------
            int i = 1;
            pd.path = Path.Combine(setupData.projectBaseDefault, $"newProject");
            while (Directory.Exists(pd.path)) { pd.path = Path.Combine(setupData.projectBaseDefault, $"newProject({i++})"); }

            pd.boardTxtPath = setupData.getBoardFromArduino();
            pd.coreBase = setupData.getCoreFromArduino();
            pd.compilerBase = setupData.getCompilerFromArduino();

            pd.boards = new List<Board>();
            pd.parseBoardsTxt();

            log.Info("Generated default project");
            pd.logProject();

            return pd;
        }

        public void parseBoardsTxt()
        {
            log.Info("parse boards.txt");

            vtTransferData.vsBoard tmp = new vtTransferData.vsBoard(selectedBoard);

            string boardsTxt = setupType == SetupTypes.quick ? setup.getBoardFromArduino() : boardTxtPath;
            boards = FileContent.parse(boardsTxt).Where(b => b.core == "teensy3").ToList();

            setBoardOptions(tmp);
        }


        void setBoardOptions(vtTransferData.vsBoard boardInfo)
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

