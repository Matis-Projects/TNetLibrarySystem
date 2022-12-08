<div>

# TNetLibrarySystem [![GitHub](https://img.shields.io/github/license/Matis-Projects/TNetLibrarySystem?color=blue&label=License&style=flat)](https://github.com/Matis-Projects/TNetLibrarySystem/blob/main/LICENSE) [![GitHub Repo stars](https://img.shields.io/github/stars/Matis-Projects/TNetLibrarySystem?style=flat&label=Stars)](https://github.com/Matis-Projects/TNetLibrarySystem/stargazers) [![GitHub all releases](https://img.shields.io/github/downloads/Matis-Projects/TNetLibrarySystem/total?color=blue&label=Downloads&style=flat)](https://github.com/Matis-Projects/TNetLibrarySystem/releases) [![GitHub tag (latest SemVer)](https://img.shields.io/github/v/tag/Matis-Projects/TNetLibrarySystem?color=blue&label=Release&sort=semver&style=flat)](https://github.com/Matis-Projects/TNetLibrarySystem/releases/latest)

</div>

### A package for make a Networking system easy to use. 

---

TNetLibrarySystem is a packages for make your Networking scripts more readable and support parameters!

---

# How to use TNetLibrarySystem

### Preparation of the world

1. Put the prefabs named `TNLS Manager` in your world

### Preparation of the script

1. Your script Syncronization Method need to be in MANUAL in **all** your NetworkedScript.
2. Declare under your `UdonSharpBehaviour` class the TNLS: `[SerializeField] public TNLSManager TNLSManager;`
    * In case you have a script link to all your scripts, you can declare it into the this one and not re-declare it all times.
3. Declare the ScriptName or the ScriptId:
    1.  You need to set the value when the script Start! 
        *   You can set to a custom string using `TNLSManager.AddANamedNetworkedScript(<the name>, this);`
        *   You can set to a custom number using : `TNLSManager.AddAIdNetworkedScript("<the number>", this);`
    *  ***WARNING** It's one name/id per instance of script.*

### Call a void

*   
    *   In case of you want use the Id of the script: `TNLSManager.CallNetworkedScript("<Your Void Name>", <The ScriptId destination>, <parameters>);`
    *   In case of you want use the name of the script: `TNLSManager.CallNamedNetworkedScript("<Your Void Name>", "<The ScriptName destination>", <parameters>);`
*   **WARNING**
    * Parameters can be `null` or a array of `object[]`!
    * You have at maximum 25 parameters!
    * Some type aren't compatible! Check the list.

### Receive Parameters

* Create a object array and set the value with the method `TNLSManager.GetParameters()`.
    * `object[] parameters = TNLSManager.GetParameters();`

### All types compatibles
*Some types aren't compatible for now, they will be compatible after.*

* ~~String (Don't use any special Ascii character)~~ ***NOT RECOMMENDED** Use it ONLY if it's required!*
* Int16 with array support.
* Int32 with array support.
* Int64 with array support.
* Bool  with array support.
* Byte  with array support.
* VRCPlayerApi

# How to install TNetLibrarySystem

### Prerequisites:

* [VRCSDK3 - Base](https://vrchat.com/home/download)
* [VRCSDK3 - UdonSharp](https://vrchat.com/home/download)

<details><summary>

### Import with [VRChat Creator Companion](https://vcc.docs.vrchat.com/vpm/packages#user-packages):</summary>

> 1. Download `fr.tismatis.tnetlibrarysystem.zip` from [here](https://github.com/Matis-Projects/TNetLibrarySystem/releases/latest)
> 2. Unpack the .zip somewhere
> 3. In VRChat Creator Companion, navigate to `Settings` > `User Packages` > `Add`
> 4. Navigate to the unpacked folder, `fr.tismatis.tnetlibrarysystem` and click `Select Folder`
> 5. `TNetLibrarySystem` should now be visible under `Local User Packages` in the project view in VRChat Creator Companion
> 6. Click `Add`

</details><details><summary>

### Import with [Unity Package Manager (git)](https://docs.unity3d.com/2019.4/Documentation/Manual/upm-ui-giturl.html):</summary>

> 1. In the Unity toolbar, select `Window` > `Package Manager` > `[+]` > `Add package from git URL...` 
> 2. Paste the following link: `https://github.com/Matis-Projects/TNetLibrarySystem.git`

</details>
