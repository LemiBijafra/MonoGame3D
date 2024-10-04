# MonoGame3D
3D extensions and examples for the awesome MonoGame framework.

## Introduction ##
The goal of this project is to provide useful additions for doing 3D work with the MonoGame framework, and providing sample code and tutorials for:
* Dynamic loading of 3D scenes.
* Phong shader.
* Importing scenes from Blender.
* Integrating custom shaders (especially from ShaderToy).

## Example Project ##
![alt text](https://github.com/LemiBijafra/MonoGame3D/blob/main/Screenshot.png?raw=true)

## Building ##
The sample project was done using Visual Studio 2022 on Windows, targeting Desktop GL (i.e. platform agnostic). In case you get a build error mentioning `./config/dotnet-tools.json` file, open it in Explorer and check the "Unblock" option. Please chime in in case of other issues.

## Contact ##
Pull requests are more than welcome!

## Credits ##
The code uses AssimpNet library for loading 3D scenes. For artwork credits, please refer to `MODEL_CREDITS.txt`.