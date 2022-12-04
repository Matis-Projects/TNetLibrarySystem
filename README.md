<div>

# TNetLibrarySystem [![GitHub](https://img.shields.io/github/license/Matis-Projects/TNetLibrarySystem?color=blue&label=License&style=flat)](https://github.com/Matis-Projects/TNetLibrarySystem/blob/main/LICENSE) [![GitHub Repo stars](https://img.shields.io/github/stars/Matis-Projects/TNetLibrarySystem?style=flat&label=Stars)](https://github.com/Matis-Projects/TNetLibrarySystem/stargazers) [![GitHub all releases](https://img.shields.io/github/downloads/Matis-Projects/TNetLibrarySystem/total?color=blue&label=Downloads&style=flat)](https://github.com/Matis-Projects/TNetLibrarySystem/releases) [![GitHub tag (latest SemVer)](https://img.shields.io/github/v/tag/Matis-Projects/TNetLibrarySystem?color=blue&label=Release&sort=semver&style=flat)](https://github.com/Matis-Projects/TNetLibrarySystem/releases/latest)

</div>

### A package for make a Networking system easy to use. 

---

TNetLibrarySystem is a packages for make your Networking scripts more readable and support parameters!

---

# How to use TNetLibrarySystem

### Preparation of the world

1. Put the prefabs named `TNetLibrarySystem` in your world

### Preparation of the script

1. Your script Syncronization Method need to be in MANUAL in **all** your NetworkedScript.
2. Declare under your `UdonSharpBehaviour` class the TNLS: `[SerializeField] public TNLS TNLS;`
    * In case you have a script link to all your scripts, you can declare it into the this one and not re-declare it all times.
3. Declare the ScriptId:
    *   You can set a custom number the `ScriptId` using `TNLS.DeclareNewNetworkingScript(this, <your number>);` **only when your class is used ONE TIME in your world.**
    *   In case you don't want that, you can just add this line to your `public void Start()`: `ScriptId = TNLS.DeclareNewDynamicNetworkingScript(this);`

### Call a void

* Use the method `net.SendNetwork("<Your Void Name>", <The ScriptId destination>, <parameters>);`
    * This can be the same ScriptId if it's for call a 
    * Parameters can be `null` or a array of `object[]`!
     ***WARNING** You have at maximum 25 parameters!*
     ***WARNING** Some type aren't compatible! Check the list under.*

### Receive Parameters

* Create a object array and set the value with the method `TNLS.GetParameters()`.
    `object[] parameters = TNLS.GetParameters();`

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

</details><details><summary>

### Import from [Unitypackage](https://docs.unity3d.com/2019.4/Documentation/Manual/AssetPackagesImport.html):</summary>

> 1. Download latest `TNetLibrarySystem` from [here](https://github.com/Matis-Projects/TNetLibrarySystem/releases/latest)
> 2. Import the downloaded .unitypackage into your Unity project

</details>
