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


# Why use TNetLibrarySystem

|Fonctionnality|UNet|NetworkedEventCaller|T.N.L.S
|--------------|-------|-|-|
|Can call method with arguments|<center>✔️</center>|<center>✔️</center>|<center>✔️</center>
|Complex target (All/Only Master/Local)|<center>✔️</center>|<center>✔️</center>|<center>✔️</center>
|Support all basics types of c# *(string,ushort,byte)*|<center>⚠️¹</center>|<center>✔️</center>|<center>✔️</center>
|Support all basics types of Unity *(Quaternion/Vector)*|<center>⚠️¹</center>|<center>⚠️²</center>|<center>✔️</center>
|De-centralized networked script file|<center>❌</center>|<center>❌</center>|<center>✔️</center>
|Easy to setup & use|<center>❌</center>|<center>✔️</center>|<center>✔️</center>
|With a queue system|<center>❌</center>|<center>✔️</center>|<center>✔️</center>

* *¹ → you need to transform it before send it by the network*
* *² → not every Vector*
* *³ → coming soon*

# How to use TNetLibrarySystem

### Preparation of the world

1. Put the prefab named `TNLS Manager` in your world
2. **UNPACK THE PREFAB** ! Without unpack, this will just break the entire system.
3. Put the prefab at the top of the world. *That can fix a lot of issues.*
4. All settings are in the `TNLS Settings`*, a child of `TNLS Manager`*.

### Preparation of the Networked script

* Automatic mode
    1. Add in the same gameobject a `AssignNewScriptToNetwork` component.
    2. Change the scriptName to what you want.
    3. Assign the `TNLS Manager` and your Networked script in the new component.
    4. The Serialization Method can't be in `None` but put the same value in the `AssignNewScriptToNetwork`.
* Legacy mode
    1. Your script Syncronization Method can't be in `None` or he can't receive any networked method. 
    2. Declare under your `UdonSharpBehaviour` class the TNLS: `[SerializeField] private TNLSManager TNLSManager;` *(can be private or public)*
        * In case you have a script link to all your scripts, you can declare it into the this one and not re-declare it all times.
    3. Declare the ScriptName:
        * Use the following method to transform that script into a networked script: `TNLSManager.AddANamedNetworkedScript("<the name>", this);`
        *  ***WARNING** It's one name per instance of script.*

### Call a method

*   In case of you want use the name of the script: `TNLSManager.CallNamedNetworkedScript("All/Local", "<Your Void Name>", "<The ScriptName destination>", <parameters>);`

### Call method by UI Button with arguments

* Currently, only the string is supported.
1. Create your button and select it on Unity.
2. Add the component `TNLS Custom Button Sender`.
3. Configuration of the component: 
    1. Insert your `TNLS Manager`.
    2. Write your Target.
    3. Write the original ScriptName.
    4. Write the original MethodName.
4. Add an action on the button and insert the `TNLS Custom Button Sender` component.
5. Select the event `UdonBehaviour.SendCustomEvent` and write the event `CallTheCustomEvent`.

### Receive Parameters

* Create a object array and set the value with the method `TNLSManager.GetParameters()`.
    * `object[] parameters = TNLSManager.GetParameters();`

### Parameters

*   Limits of the system
    * Parameters can be `null` or a array of `object[]`!
    * You have at maximum 25 parameters!* *Can be modified in the `TNLS Settings` but not recommended*
*   Supported types
    *Some types aren't compatible for now, they will be compatible after.*
    - If you get any error from one type listed, please create a issue for report it.

    |Keyword|Aliased Type|Base Support|Array Support|Full Support|Note|
    |---------------|---------------|-|-|-|-|
    |`short`        |Int16          |<center>✔️</center>|<center>✔️</center>|<center>✔️</center>|
    |`ushort`       |UInt16         |<center>✔️</center>|<center>✔️</center>|<center>✔️</center>|
    |`int`          |Int32          |<center>✔️</center>|<center>✔️</center>|<center>✔️</center>|
    |`uint`         |UInt32         |<center>✔️</center>|<center>✔️</center>|<center>✔️</center>|
    |`long`         |Int64          |<center>✔️</center>|<center>✔️</center>|<center>✔️</center>|
    |`ulong`        |UInt64         |<center>✔️</center>|<center>✔️</center>|<center>✔️</center>|
    |`float`        |Single         |<center>✔️</center>|<center>✔️</center>|<center>✔️</center>|
    |`double`       |Double         |<center>✔️</center>|<center>✔️</center>|<center>✔️</center>|
    |`bool`         |Boolean        |<center>✔️</center>|<center>✔️</center>|<center>✔️</center>|
    |`byte`         |Byte           |<center>✔️</center>|<center>✔️</center>|<center>✔️</center>|
    |`sbyte`        |SByte          |<center>✔️</center>|<center>✔️</center>|<center>✔️</center>|
    |~              |Color          |<center>✔️</center>|<center>✔️</center>|<center>✔️</center>|
    |~              |Color32        |<center>✔️</center>|<center>✔️</center>|<center>✔️</center>|
    |~              |Quarternion    |<center>✔️</center>|<center>✔️</center>|<center>✔️</center>|
    |~              |Vector2        |<center>✔️</center>|<center>✔️</center>|<center>✔️</center>|
    |~              |Vector2Int     |<center>✔️</center>|<center>✔️</center>|<center>✔️</center>|
    |~              |Vector3        |<center>✔️</center>|<center>✔️</center>|<center>✔️</center>|
    |~              |Vector3Int     |<center>✔️</center>|<center>✔️</center>|<center>✔️</center>|
    |~              |Vector4        |<center>✔️</center>|<center>✔️</center>|<center>✔️</center>|
    |`decimal`      |Decimal        |<center>✔️</center>|<center>✔️</center>|<center>✔️</center>|
    |~              |VRCPlayerApi   |<center>✔️</center>|<center>✔️</center>|<center>✔️</center>|
    |`string`       |String         |<center>✔️</center>|<center>✔️</center>|<center>✔️</center>|
    |`char`         |Char           |<center>✔️</center>|<center>✔️</center>|<center>✔️</center>|

### Debug

* To put in a text:
    1. Create a 2D Text
    2. Assign this 2D Text to the `TNLS Logging System`*, a child of `TNLS Manager`.*
    3. Activate debug mode in `TNLS Manager`.
* To activate the debug mode: *(for get more detail)*
    1. Activate the `Debug Mode` in the `TNLS Settings`*, a child of `TNLS Manager`.*