# VisualTeensy
[vsCode](https://code.visualstudio.com/) is a modern, open source code editor which can easily be used to compile and upload sketches for the [PJRC Teensy Boards](https://www.pjrc.com/). **VisualTeensy** is a light weight Windows utility which fully automates the generation of Teensy projects for VisualCode. Precompiled binaries of VisualTeensy can be downloaded from here: [https://github.com/luni64/VisualTeensy/releases](https://github.com/luni64/VisualTeensy/releases).

Here a quick video showing how to use VisualTeensy:

[![Watch the video](https://user-images.githubusercontent.com/12611497/63101306-170a2680-bf79-11e9-9e8e-38b3f9a8d6be.png)](https://youtu.be/g5YXzBwtecg)


## Prerequisites
- Obviously you'll need **vsCode** installed. If you want to take advantage of intellisense (which I strongly suggest) you should also install the ["C/C++ IntelliSense, debugging, and code browsing](https://code.visualstudio.com/docs/languages/cpp) extension from Microsoft.
- While it is not strictly necessary, I recommend to have a working installation of  **Arduino and Teensyduino** installated on your system. On the first startup VisualTeensy will ask you for this Arduino installation and sets the default settings accordingly. (If you want, you can change those settings at any time later)

## Installation
- Download the VisualTeensy zip file from the releases section of the gitHub repository ([https://github.com/luni64/VisualTeensy/releases](https://github.com/luni64/VisualTeensy/releases)) and unzip it to any convenient folder. You don't need to install anything, just double click on VisualTeensy to start it. 
- Alternatively you can download the sources and build VisualTeensy with a current version of VisualStudio. The free Community editon works nicely for that. 

## Usage
VisualTeensy does not touch any global or user settings in vsCode and does not install any extension to vsCode. All it does is generating a makefile as well as some json control files (tasks.json, c_cpp_properties.json, launch.json) and copying them to the .vscode and .vsteensy subfolders of your project. You can then use vsCode's native commands to build your programm and upload it to the board. 

- Open an empty or already existing project folder (File | Open Project)
- Select the used Teensy board  and the corresponding settings from the dropdown lists.
 
![Quickstart](/media/quickStart.jpg)
- Use File | Save to generate or update the project. This will open a summary of the actions and allows for generating the project. 

![Generate](/media/generateDialog.jpg)

- After succesfull generation of the project VisualTeensy will start vsCode, load the project folder, open the generated main.cpp and closes itself. After that you are ready to compile the sketch and upload it to a Teensy board. 

![Generate](/media/folderView.PNG)

- For compiling you can use the menu entry *Terminal | Run Build Tasks* or the shortcut *CTRL+SHIFT+B* The following options are available: 
    - **Build** Standard build procedure of the project. Only changed or new files will be compiled and linked. Similar to the Arduino Sketch|verify compile action. 
    - **Clean** Removes all generated binaries
    - **Upload (Teensy Uploader)** uploads to the connected board using the standard PJRC uploader. Builds the project first if necessary
    - **Upload (TyCommander)** uploads using TyCommander from the TyTools. Builds the project first if necessary
   
![Build](/media/build.PNG)

### Expert Settings
TBD







# To be continued
