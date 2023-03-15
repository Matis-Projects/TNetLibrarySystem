<div>

# TNetLibrarySystem [![GitHub](https://img.shields.io/github/license/Matis-Projects/TNetLibrarySystem?color=blue&label=License&style=flat)](https://github.com/Matis-Projects/TNetLibrarySystem/blob/main/LICENSE) [![GitHub Repo stars](https://img.shields.io/github/stars/Matis-Projects/TNetLibrarySystem?style=flat&label=Stars)](https://github.com/Matis-Projects/TNetLibrarySystem/stargazers) [![GitHub all releases](https://img.shields.io/github/downloads/Matis-Projects/TNetLibrarySystem/total?color=blue&label=Downloads&style=flat)](https://github.com/Matis-Projects/TNetLibrarySystem/releases) [![GitHub tag (latest SemVer)](https://img.shields.io/github/v/tag/Matis-Projects/TNetLibrarySystem?color=blue&label=Release&sort=semver&style=flat)](https://github.com/Matis-Projects/TNetLibrarySystem/releases/latest)

</div>

### A package for make a Networking system easy to use. 

---

TNetLibrarySystem is a packages for make your Networking scripts more readable and support parameters!

---

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


# How to use TNetLibrarySystem

### Preparation of the world

1. Put the prefab named `TNLS Manager` in your world
2. **UNPACK THE PREFAB** ! Without unpack, this will just break the entire system.
3. Put the prefab at the top of the world. *That can fix a lot of issues.*

### Preparation of the Networked script

* Automatic mode
    1. Add in the same gameobject a `AssignNewScriptToNetwork` component.
    2. Change the scriptName to what you want.
    3. Assign the `TNLS Manager` and your Networked script in the new component.
    3. Change the syncronization method of your script to `Manual`.
* Legacy mode
    1. Your script Syncronization Method need to be in MANUAL in **all** your NetworkedScript.
    2. Declare under your `UdonSharpBehaviour` class the TNLS: `[SerializeField] private TNLSManager TNLSManager;`
        * In case you have a script link to all your scripts, you can declare it into the this one and not re-declare it all times.
    3. Declare the ScriptName or the ScriptId:
        1.  You need to set the value when the script Start! 
            *   You can set to a custom string using `TNLSManager.AddANamedNetworkedScript(<the name>, this);`
            *   You can set to a custom number using : `TNLSManager.AddAIdNetworkedScript("<the number>", this);`
        *  ***WARNING** It's one name/id per instance of script.*

### Call a method

*   In case of you want use the Id of the script: `TNLSManager.CallNetworkedScript("<Your Void Name>", <The ScriptId destination>, <parameters>);`
*   In case of you want use the name of the script: `TNLSManager.CallNamedNetworkedScript("<Your Void Name>", "<The ScriptName destination>", <parameters>);`

### Call method by UI Button with arguments

* Currently, only the string is supported.
1. Create your button and select it on Unity.
2. Add the component `TNLS Custom Button Sender`.
3. Configuration of the component: 
    1. Insert your `TNLS Manager`.
    2. Write your Target.
    3. Write the original script name.
    4. Write the original Method name.
4. Add an action on the button and insert the `TNLS Custom Button Sender` component.
5. Select the event `UdonBehaviour.SendCustomEvent` and write the event `CallTheCustomEvent`.

### Receive Parameters

* Create a object array and set the value with the method `TNLSManager.GetParameters()`.
    * `object[] parameters = TNLSManager.GetParameters();`

### Parameters

*   Limits of the system
    * Parameters can be `null` or a array of `object[]`!
    * You have at maximum 25 parameters!* *Can be modified in the `TNLS Manager` but not recommended*
    * Some type aren't compatible! Check the list.
*   Supported types
    *Some types aren't compatible for now, they will be compatible after.*

    * String (Don't use any special Ascii character) ***NOT RECOMMENDED** Use it ONLY if it's required!*
    * Int16 with array support.
    * UInt16 with array support.
    * Int32 with array support.
    * UInt32 with array support.
    * Int64 with array support.
    * UInt64 with array support.
    * Single with array support.
    * Double with array support.
    * Bool  with array support.
    * Byte  with array support.
    * SByte  with array support.
    * VRCPlayerApi

### Debug

* It's very easy to see log of TNLS in-game!
1. Create a 2D Text
2. Assign this 2D Text to the `TNLS Logging System`*, a child of `TNLS Manager`.*
3. Activate debug mode in `TNLS Manager`.
