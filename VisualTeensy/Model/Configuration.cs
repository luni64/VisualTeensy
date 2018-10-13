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

        public string name { get; set; }
             
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

        // makefile  ------------------------
        public string makefile { get; set; }
        public string makefileExtension { get; set; }
        
        // libraries ---------------------------------
        public List<Library> sharedLibs { get; set; }
        public List<Library> localLibs { get; set; }
        
        // boards
        public List<Board> boards { get; private set; }
        public Board selectedBoard { get; set; }

        public Configuration(SetupData settings = null)
        {
          //  this.setup = settings;
            boards = new List<Board>();
            sharedLibs = new List<Library>();
            localLibs = new List<Library>();
        }
   
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
            pd.name = "default";            

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
                
        

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}

