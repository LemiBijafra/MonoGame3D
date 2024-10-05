# MonoGame3D
3D extensions and examples for the awesome MonoGame framework.

## Introduction ##
The goal of this project is to provide useful additions for doing 3D work with the MonoGame framework, in the form of sample code and tutorials for:
* Dynamic loading of 3D scenes.
* Phong shader.
* Importing scenes from Blender.
* Integrating custom shaders (especially from ShaderToy).

## Example Project ##
A dystopian scene using a number of modified publically available 3D models (see the credits section).
The scene is created in Blender:
![alt text](https://github.com/LemiBijafra/MonoGame3D/blob/main/Screenshot_Blender.png?raw=true)
Current rendering in the demo app:

![alt text](https://github.com/LemiBijafra/MonoGame3D/blob/main/Screenshot.png?raw=true)

## Building ##
The sample project was done using Visual Studio 2022 on Windows, targeting Desktop GL (i.e. platform agnostic). In case you get a build error mentioning `./config/dotnet-tools.json` file, open it in Explorer and check the "Unblock" option. Please chime in in case of other issues.
In case the build fails because of the missing AssimpNet reference, invoke `InstallAssimpNet.bat`.

## TODO ##
* Fix the issue with textures.
* Finalize the Phong shader, currently just in the beginning.
* Finalize the `Material` class.
* Texture manager for dynamically loaded textures.
* Investigate F# scripting possibilites.
* Blender camera import.
* Animation.

## Contact ##
Pull requests are more than welcome!

## Credits ##
Apart MonoGame v3.8.2, the AssimpNet library v4.1.0 is used for loading 3D scenes.

For artwork credits, please refer to `ARTWORK_CREDITS.txt`.