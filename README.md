# Under Construction!

# VisualTeensy
[VisualCode](https://code.visualstudio.com/) is a modern, open source code editor which can be used to compile and upload sketches for the [PJRC Teensy Boards](https://www.pjrc.com/). 
VisualTeensy is a small Windows utility which fully automates the generation of Teensy projects for VisualCode. 

## Prerequisites
- VisualTeensy uses GNU Make to build your sketches. You can download a standalone version for Windows from e.g. here: [http://www.equation.com/servlet/equation.cmd?fa=make](http://www.equation.com/servlet/equation.cmd?fa=make) (copy the downloaded file to some convenient location, no installation required).
- While not strictly necessary, for a quick start it is convenient to have Arduino and Teensiduino installated on your system. 

## Usage
### Quick Start
- Enter the path to GNU Make in the setup tab. You can leave the uploader settings empty for the time being. 
![Setup](/media/setup.PNG)

- Open the Project tab and select Quick Setup.Enter a project name (this will define the name for the output files), and a path to the project folder. 

- Enter the path to your Arduino Installation. (Make sure that you have Teensyduino installed)

- Select the used Teensy board and the corresponding settings from the dropdown lists. 
![Quickstart](/media/guickSetupExample.PNG)

- Press the "Generate / Upload Project" Button. This will open a summary of the actions and allows for generating the project. 
![Generate](/media/generateDialog.PNG)

- Open the project folder with VisualCode (File|Open Folder)
![Generate](/media/folderView.PNG)


### Expert Settings
TBD

## Compiling
For compiling you can use the Build Tasks (Terminal | Run Build Task or CTRL+SHIFT+B)
![Build](/media/build.PNG)

# To be continued