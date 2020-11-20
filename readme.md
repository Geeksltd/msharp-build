# M# Build Tool

To install this tool, in `cmd` run the following:

```
C:\> dotnet tool install --global msharp-build
```

## Create a new M# project
To create a new M# project, run the following command:
```
C:\Projects\> msharp-build /new /n:"ProjectName" [/t:https://github.com/Geeksltd/Olive.MvcTemplate/archive/master.zip] 
```
At this point, the template repository will be downloaded [from here](https://github.com/Geeksltd/Olive.MvcTemplate), and the placeholders will be replaced with the name provided in the `/n:` parameter. 

The `/t:...` parameter is optional, and defaults to the above. Alternatively, you can provide your own project template repository (which must be publically accessible).


## Build an existing project
To build an existing M# project, run the following command:
```
C:\Projects\MyProject\> msharp-build
```

## Install build tools (prepare your environment)
You need to do this only once, to ensure your development environment is prepared, with all necessary tools installed:
```js
C:\>msharp-build /tools
```

## Update nuget packages
There are many Olive components which can have interdependencies. Unfortunately Visual Studio can sometimes not figure out the right way to update the packages when you want to update your project nugets. To solve this problem, you can run the following command:
```js
C:\>msharp-build /update-nuget
```

It will query the nuget.org server to find the latest version of all Olive and msharp nuget packages used in any of your projects (Model, UI, Domain, and Website). It will then directly replace the `csproj` files to use the latest versions. This will bypass the dependency checks. When you build the project, the packages will be restored automatically.
