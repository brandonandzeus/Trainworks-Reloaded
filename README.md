# Trainworks-Reloaded
Modding Framework for Monster Train 2

## How to Install

(Note: Trainsworks Reloaded is Built as a BepInEx 5 mod. Install version 5, not 6.)

1. (Install BepInEx into Monster Train 2)[https://docs.bepinex.dev/articles/user_guide/installation/index.html]
2. Install our latest plugin and unzip it into the BepInEx plugins folder.
3. Install other mods similarly.

## How to Build

In order to build the project you will need access to the nuget package registry, run the following with your GITHUB_TOKEN.

`dotnet nuget add source --username $GITHUB_USERNAME --password $GITHUB_TOKEN --name monster-train-packages "https://nuget.pkg.github.com/Monster-Train-2-Modding-Group/index.json"`

From there it is buildable as regular C# project through Visual Studio.