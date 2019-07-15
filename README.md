##Currently under development
Support for debugging with Cortex-Debug (use branch 'develop' for testing)

# VisualTeensy
[VisualCode](https://code.visualstudio.com/) is a modern, open source code editor which can easily be used to compile and upload sketches for the [PJRC Teensy Boards](https://www.pjrc.com/) using VisualTeensy. 
VisualTeensy is a small Windows utility which fully automates the generation of Teensy projects for VisualCode. 
Precompiled binaries can be downloaded [here](https://github.com/luni64/VisualTeensy/releases).

## Prerequisites
- Obviously you'll need **VisualCode** installed. If you want to take advantage of intellisense you should also install the standard <em>"C/C++ IntelliSense, debugging, and code browsing"</em> extension. 

- **GNU Make:** VisualTeensy sets up vsCode to use GNU Make to build your sketches. You can download a full setup of GNU Make from here [http://gnuwin32.sourceforge.net/packages/make.htm]. However, for the purposes of VisualStudio a standalone version is perfectly sufficent. I added a 64bit version to the release section [https://github.com/luni64/VisualTeensy/releases]. 

- While it is not strictly necessary, I recommend to have a working installation of  **Arduino and Teensyduino** installated on your system. On the first startup VisualTeensy tries to locate an Arduino/Teensyduino installation in the usual places and sets the default settings to use this installation for a quick start. 

## Usage
### Quickstart Guide

Here a short video showing the setup of VisualTeensy and the generation of a new project using default settings. 

<a href="http://www.youtube.com/watch?feature=player_embedded&v=leQS2GS_BmE
" target="_blank"><img src="http://img.youtube.com/vi/leQS2GS_BmE/0.jpg" 
alt="IMAGE ALT TEXT HERE" width="480" height="360" border="10" /></a>

- Download VisualTeensy.exe and make.exe from the release section [https://github.com/luni64/VisualTeensy/releases] and store it in some convenient folder. You don't need to install anything, just double click on VisualTeensy to start it. 

- If VisualTeensy finds an Arduino installation, it will use it for generating the project without further configuration. The project folder defaults to "%HOMEPATH%\source\new_project and the path to make.exe will default to the VisualTeensy directory. Of course, all default settings can be changed later to fit your needs.

- Select the used Teensy board  and the corresponding settings from the dropdown lists.

![Quickstart](/media/quickSetupExample.PNG)

- Click on "Generate / Upload Project" Button. This will open a summary of the actions and allows for generating the project. 

![Generate](/media/generateDialog.PNG)

- After succesfull generation of the project VisualTeensy will start vsCode, load the project folder, open the generated main.cpp and closes itself. After that you are ready to compile the sketch and upload it to a Teensy board. 

![Generate](/media/folderView.PNG)

- For compiling you can use the menu entry *Terminal | Run Build Tasks* or the shortcut *CTRL+SHIFT+B* The following options are available: 
    - **Build** Standard build procedure of the project. Only changed or new files will be compiled and linked. Similar to the Arduino         Sketch|verify compile action. 
    - **Rebuild user files** rebuilds all user files but keeps the Teensiduino core libraries. 
    - **Clean** Removes all generated binaries
    - **Upload (Teensy Uploader)** uploads to the connected board using the standard PJRC uploader. Builds the project first if               necessary
    - **Upload (TyCommander)** uploads using TyCommander from the TyTools. Builds the project first if necessary

![Build](/media/build.PNG)

### Expert Settings
TBD







# To be continued
