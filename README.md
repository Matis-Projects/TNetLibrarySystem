<div>

# TNetLibrarySystem [![GitHub](https://img.shields.io/github/license/Matis-Projects/TNetLibrarySystem?color=blue&label=License&style=flat)](https://github.com/Matis-Projects/TNetLibrarySystem/blob/main/LICENSE) [![GitHub Repo stars](https://img.shields.io/github/stars/Matis-Projects/TNetLibrarySystem?style=flat&label=Stars)](https://github.com/Matis-Projects/TNetLibrarySystem/stargazers) [![GitHub all releases](https://img.shields.io/github/downloads/Matis-Projects/TNetLibrarySystem/total?color=blue&label=Downloads&style=flat)](https://github.com/Matis-Projects/TNetLibrarySystem/releases) [![GitHub tag (latest SemVer)](https://img.shields.io/github/v/tag/Matis-Projects/TNetLibrarySystem?color=blue&label=Release&sort=semver&style=flat)](https://github.com/Matis-Projects/TNetLibrarySystem/releases/latest)

</div>

### A package for make a Networking system easy to use. 

---

TNetLibrarySystem is a packages for make your Networking scripts more readable and support parameters!

---

---

# Warning! The current version is not stable and the latest one!
## This readme file need to be reviewed, this is an very old version and some stuff aren't updated today!

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
<!--
|Fonctionnality|UNet|NetworkedEventCaller|T.N.L.S
|--------------|-------|-|-|
|Can call method with arguments|<center>✔️</center>|<center>✔️</center>|<center>✔️</center>
|Complex target (All/Only Master/Local)|<center>✔️</center>|<center>✔️</center>|<center>✔️</center>
|Support all basics types of c# *(string,ushort,byte)*|<center>⚠️¹</center>|<center>✔️</center>|<center>✔️</center>
|Support all basics types of Unity *(Quaternion/Vector)*|<center>⚠️¹</center>|<center>⚠️²</center>|<center>✔️</center>
|De-centralized networked script file|<center>❌</center>|<center>❌</center>|<center>✔️</center>
|Easy to setup & use|<center>❌</center>|<center>✔️</center>|<center>✔️</center>
|With a queue system|<center>❌</center>|<center>✔️</center>|<center>✔️</center>

* *¹ → you need to transform it into byte before send it by the network*
* *² → not every types*
-->
* TODO: Remake this part

# How to use TNetLibrarySystem

### Preparation of the world

1. Select in the Menu Bar `TNetLibrarySystem`.
2. Click on `Import prefab in the scene` and select the line count.
* We recommends to use 16 lines.

### Coding with TNLS.

*   **Warning**, all old method for adding the script automaticly to TNLS is outdated, that include `AssignNewScriptToNetwork` due to the new update.
1. Import `Tismatis.TNetLibrarySystem.Coding` in the class.
2. Just put the inherent class as `NetworkedClass` and not `UdonSharpBehaviour` by default.
3. Set by the `Inspector` in Unity to modify the `scriptName` value.
* *You can set `scriptName` by overriding `void Start()` **but you need to change the value before the `base.Start();`**.*
* When you use the `void Start()`, make sure you override the one of `NetworkedClass` and you added `base.Start();` at the first line.

### Call a method

*   In the case the script where you start the call is in a NetworkedClass:
    * You call the script himself: `SendNetworkedEvent("All/Local", "<Your Void Name>", "<The ScriptName destination>", <parameters>);`
    * You call another script than the one you use to call: `SendExternalNetworkedEvent("All/Local", "<Your Void Name>", "<The ScriptName destination>", <parameters>);`
*   In the case the script where you start the call is not in a NetworkedClass: `TNLSManager.CallNamedNetworkedScript("All/Local", "<Your Void Name>", "<The ScriptName destination>", <parameters>);`

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

* Now you need to declare ddirectly in yours parameters an array of object like this: `void MyNetworkedMethod(object[] parameters)`.

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

* Now you need to enable `enableLog` in `Debug Mode`!
* To put in a text:
    1. Create a 2D Text
    2. Assign this 2D Text to the `TNLS Logging System`*, a child of `TNLS Manager`.*
    3. Activate debug mode in `TNLS Manager`.
* To activate the debug mode: *(for get more detail)*
    1. Activate the `Debug Mode` in the `TNLS Settings`*, a child of `TNLS Manager`.*
