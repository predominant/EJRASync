# EJRASync

This is a console application and library that is designed to automatically synchronize content for Friday Night Racing (FNR) for [EJ_SA](https://www.twitch.tv/ej_sa) community.

## Basic Usage

Download the [latest release](https://github.com/predominant/EJRASync/releases/latest) and extract the `EJRASync.CLI.exe` file to a convenient location.

Run the `EJRASync.CLI.exe` file by double-clicking on it.

## Advanced Usage

If your Assetto Corsa installation is not detected for some reason, you can override the auto-detection by specifying the path to your AC install.

Open a command prompt (Start > Run > cmd) and change to the directory where you extracted your `EJRASync.CLI.exe` file, example:

```ps
C:\Users\predominant> cd Downloads
```

Once in the directory, execute the file, and supply your assetto corsa installation directory as a parameter, example:

```ps
C:\Users\predominant\Downloads> EJRASync.CLI.exe E:\games\assettocorsa
```

If you need to find your assetto corsa installation, you can right click on the game in your Steam library, and select Manage > Browse local files
