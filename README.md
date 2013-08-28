GLIF
====

### Overview
Using external OpenGL libraries in Visual Studio C++ projects can be a difficult, tedious process. It involves digging through project properties, putting files in your win32 folder, etc. The kicker is, while project properties follow you from computer to computer, those win32 files don't. GLIF solves these problems.

GLIF is a unified installer for GLUT, freeglut, GLEW, GLFW, and SOIL. With just a few clicks, it gives you all of the files you require, tells Visual Studio where to look for them, and sets up a copy script so your output folder is always up to date. This allows you to setup a project once, and no matter what computer you try to run it on, it will never be missing any required files or be looking in nonexistent folders for important files.

### Use
Using GLIF is a very simple 4 step process.

  1. Browse to and select your Visual Studio solution file
  2. Select the project you are editing
  3. Select the libraries you would like to "install"
  4. Click the big go button

Once this is complete, there is one more step you must complete. You must tell VS to look for the installed .lib files during the linking phase. You may do that one of 2 ways.

  1. Somewhere in your project (probably your main file), paste the following code: `#pragma comment( lib, "LIB_NAME.lib" )`. You must do this for each of the lib files in your "GL\x86\lib" folder.
  2. In your project settings, navigate to Linker -> Input -> Additional Dependencies -> Edit, and enter the name of each lib file in your "GL\x86\lib" folder, seperated by a new line.

### Notes
Using the installer for multiple projects in the same solution may cause weirdness. Avoid it if possible.
