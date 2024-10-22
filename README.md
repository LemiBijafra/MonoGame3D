# MonoGame3D
3D extensions and examples for the awesome MonoGame framework.

## Introduction ##
The goal of this project is to provide useful additions for doing 3D work on top of the MonoGame framework, in the form of sample code and tutorials for:
* Dynamic loading of 3D scenes.
* Phong shader.
* Importing scenes from Blender.
* Integrating custom shaders (especially from ShaderToy).

No attempt to change the MonoGame sources will be made.

## Example Project ##
A dystopian scene using a number of modified publically available 3D models (see the credits section).

Use `WASD` to move around the scene (WIP).

### Blender Scene ###
![alt text](https://github.com/LemiBijafra/MonoGame3D/blob/main/Screenshot_Blender.png?raw=true)

### Current Rendering With The Test App ###
![alt text](https://github.com/LemiBijafra/MonoGame3D/blob/main/Screenshot.png?raw=true)

## Building ##
The sample project was done using Visual Studio 2022 on Windows, targeting Desktop GL (i.e. platform agnostic). In case you get a build error mentioning `./config/dotnet-tools.json` file, open it in Explorer and check the "Unblock" option. Please chime in in case of other issues.
In case the build fails because of the missing AssimpNet reference, invoke `InstallAssimpNet.bat`.

## TODO ##
* Improve the Phong shader (directional and spot lights, transparency), better Blender WYSIWYG.
* Finalize the `Material` class.
* Texture manager for dynamically loaded textures.
* Investigate F# scripting possibilites.
* Blender camera import.
* Animation.

## Contact ##
Since this project is in active development, in case you want to contribute and to avoid work duplication, please read the TODOs and open a discussion item with the changes you'd like to add.

## Credits ##
Apart from MonoGame v3.8.2, the AssimpNet library v4.1.0 is used for loading 3D scenes.

For artwork credits, please refer to `ARTWORK_CREDITS.txt`.