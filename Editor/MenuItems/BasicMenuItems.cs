
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Tismatis.TNetLibrarySystem.Editor
{
    public class BasicMenuItems : MonoBehaviour
    {
        static string n = "\r\n";

        [MenuItem("TNetLibrarySystem/Import prefab in the scene/1 Line", priority = 1)]
        static void BuildPrefabInSceneOne()
        {
            BuildPrefabInTheScene(1);
        }

        [MenuItem("TNetLibrarySystem/Import prefab in the scene/2 Lines", priority = 2)]
        static void BuildPrefabInSceneTwo()
        {
            BuildPrefabInTheScene(2);
        }

        [MenuItem("TNetLibrarySystem/Import prefab in the scene/4 Lines", priority = 3)]
        static void BuildPrefabInSceneFour()
        {
            BuildPrefabInTheScene(4);
        }

        [MenuItem("TNetLibrarySystem/Import prefab in the scene/8 Lines", priority = 4)]
        static void BuildPrefabInSceneEight()
        {
            BuildPrefabInTheScene(8);
        }

        [MenuItem("TNetLibrarySystem/Import prefab in the scene/16 Lines", priority = 5)]
        static void BuildPrefabInSceneSixteen()
        {
            BuildPrefabInTheScene(16);
        }

        [MenuItem("TNetLibrarySystem/Import prefab in the scene/32 Lines", priority = 6)]
        static void BuildPrefabInSceneThirtyTwo()
        {
            BuildPrefabInTheScene(32);
        }

        [MenuItem("TNetLibrarySystem/Repair the Prefab in the scene.", priority = 2)]
        static void RepairPrefabInScene()
        {
            if (Selection.activeGameObject != null)
            {
                var TNLSManager = Selection.activeGameObject.GetComponent<TNLSManager>();
                if (TNLSManager != null)
                {
                    if(EditorUtility.DisplayDialog(
                        "TNetLibrarySystem ~ Fixing Tool",
                        $"We starting to fix the current setup of TNLS, please don't touch anything.\r\nWarning: This tool don't fix the corrumption of the package TNLS himself (like file destroyed).\r\nYou can still refuse to use that function by pressing CANCEL.",
                        "Ok",
                        "Cancel")
                    )
                    {
                        ushort count = 0;

                        if (TNLSManager.TNLSLinePool == null)
                        {
                            EditorUtility.DisplayDialog(
                                "TNetLibrarySystem ~ Fixing Tool",
                                $"We gonna setup a Line Pool with 8 lines.\r\nIf you want more lines, duplicate them in the line pool with the same syntax after the end of the fixing work.",
                                "OK"
                            );

                            GameObject linepool = new GameObject();
                            linepool.name = "TNLS Line Pool";
                            linepool.transform.SetParent(TNLSManager.transform);
                            TNLSLinePool TNLSLinePool = linepool.AddComponent<TNLSLinePool>();
                            TNLSManager.TNLSLinePool = TNLSLinePool;
                            TNLSLinePool.TNLSManager = TNLSManager;

                            for (int i = 0; i < 8; i++)
                            {
                                GameObject line = new GameObject();
                                line.name = $"NetworkLine ({i})";
                                line.transform.SetParent(linepool.transform);
                                line.AddComponent<TNLSLine>();
                            }
                            count++;
                        }
                        if (TNLSManager.TNLSLinePool.transform.childCount == 0)
                        {
                            EditorUtility.DisplayDialog(
                                "TNetLibrarySystem ~ Fixing Tool",
                                $"We gonna setup a Line Pool with 8 lines.\r\nIf you want more lines, duplicate them in the line pool with the same syntax after the end of the fixing work.",
                                "OK"
                            );

                            for (int i = 0; i < 8; i++)
                            {
                                GameObject line = new GameObject();
                                line.name = $"NetworkLine ({i})";
                                line.transform.SetParent(TNLSManager.TNLSLinePool.transform);
                                line.AddComponent<TNLSLine>();
                            }
                            count++;
                        }
                        if (TNLSManager.TNLSLinePool.TNLSManager != TNLSManager)
                        {
                            TNLSManager.TNLSLinePool.TNLSManager = TNLSManager; count++;
                        }

                        if (TNLSManager.TNLSLogingSystem == null)
                        {
                            GameObject logingsystem = new GameObject();
                            logingsystem.name = "TNLS Loging System";
                            logingsystem.transform.SetParent(TNLSManager.transform);
                            TNLSLogingSystem TNLSLogingSystem = logingsystem.AddComponent<TNLSLogingSystem>();
                            TNLSManager.TNLSLogingSystem = TNLSLogingSystem;
                            TNLSLogingSystem.TNLSManager = TNLSManager;
                            count++;
                        }
                        if (TNLSManager.TNLSLogingSystem.TNLSManager != TNLSManager)
                        {
                            TNLSManager.TNLSLogingSystem.TNLSManager = TNLSManager; count++;
                        }

                        if (TNLSManager.TNLSSerialization == null)
                        {
                            GameObject serialization = new GameObject();
                            serialization.name = "TNLS Serialization";
                            serialization.transform.SetParent(TNLSManager.transform);
                            TNLSSerialization TNLSSerialization = serialization.AddComponent<TNLSSerialization>();
                            TNLSManager.TNLSSerialization = TNLSSerialization;
                            TNLSSerialization.TNLSManager = TNLSManager;
                            count++;
                        }
                        if (TNLSManager.TNLSSerialization.TNLSManager != TNLSManager)
                        {
                            TNLSManager.TNLSSerialization.TNLSManager = TNLSManager; count++;
                        }

                        if (TNLSManager.TNLSScriptManager == null)
                        {
                            GameObject scriptManager = new GameObject();
                            scriptManager.name = "TNLS Script Manager";
                            scriptManager.transform.SetParent(TNLSManager.transform);
                            TNLSScriptManager TNLSScriptManager = scriptManager.AddComponent<TNLSScriptManager>();
                            TNLSManager.TNLSScriptManager = TNLSScriptManager;
                            TNLSScriptManager.TNLSManager = TNLSManager;
                            count++;
                        }
                        if (TNLSManager.TNLSScriptManager.TNLSManager != TNLSManager)
                        {
                            TNLSManager.TNLSScriptManager.TNLSManager = TNLSManager; count++;
                        }

                        if (TNLSManager.TNLSSettings == null)
                        {
                            GameObject settings = new GameObject();
                            settings.name = "TNLS Settings";
                            settings.transform.SetParent(TNLSManager.transform);
                            TNLSSettings TNLSSettings = settings.AddComponent<TNLSSettings>();
                            TNLSManager.TNLSSettings = TNLSSettings;
                            TNLSSettings.TNLSManager = TNLSManager;
                            count++;
                        }
                        if (TNLSManager.TNLSSettings.TNLSManager != TNLSManager)
                        {
                            TNLSManager.TNLSSettings.TNLSManager = TNLSManager; count++;
                        }

                        if (TNLSManager.TNLSQueue == null)
                        {
                            GameObject queue = new GameObject();
                            queue.name = "TNLS Queue";
                            queue.transform.SetParent(TNLSManager.transform);
                            TNLSQueue TNLSQueue = queue.AddComponent<TNLSQueue>();
                            TNLSManager.TNLSQueue = TNLSQueue;
                            TNLSQueue.TNLSManager = TNLSManager;
                            count++;
                        }
                        if (TNLSManager.TNLSQueue.TNLSManager != TNLSManager)
                        {
                            TNLSManager.TNLSQueue.TNLSManager = TNLSManager; count++;
                        }

                        if (TNLSManager.TNLSConfirmPool == null)
                        {
                            GameObject confirmPool = new GameObject();
                            confirmPool.name = "TNLS Confirm Pool";
                            confirmPool.transform.SetParent(TNLSManager.transform);
                            TNLSConfirmPool TNLSConfirmPool = confirmPool.AddComponent<TNLSConfirmPool>();
                            TNLSManager.TNLSConfirmPool = TNLSConfirmPool;
                            TNLSConfirmPool.TNLSManager = TNLSManager;

                            for (int i = 0; i < 82; i++)
                            {
                                GameObject playerConfirmReceived = new GameObject();
                                playerConfirmReceived.name = $"PlayerConfirmReceived ({i})";
                                playerConfirmReceived.transform.SetParent(confirmPool.transform);
                                playerConfirmReceived.AddComponent<PlayerConfirmReceived>();
                            }
                            count++;
                        }
                        if (TNLSManager.TNLSConfirmPool.transform.childCount == 0)
                        {
                            for (int i = 0; i < 82; i++)
                            {
                                GameObject playerConfirmReceived = new GameObject();
                                playerConfirmReceived.name = $"PlayerConfirmReceived ({i})";
                                playerConfirmReceived.transform.SetParent(TNLSManager.TNLSConfirmPool.transform);
                                playerConfirmReceived.AddComponent<PlayerConfirmReceived>();
                            }
                            count++;
                        }
                        if (TNLSManager.TNLSConfirmPool.TNLSManager != TNLSManager)
                        {
                            TNLSManager.TNLSConfirmPool.TNLSManager = TNLSManager; count++;
                        }

                        if (TNLSManager.TNLSOthers == null)
                        {
                            GameObject others = new GameObject();
                            others.name = "TNLS Others";
                            others.transform.SetParent(TNLSManager.transform);
                            TNLSOthers TNLSOthers = others.AddComponent<TNLSOthers>();
                            TNLSManager.TNLSOthers = TNLSOthers;
                            TNLSOthers.TNLSManager = TNLSManager;
                            count++;
                        }
                        if (TNLSManager.TNLSOthers.TNLSManager != TNLSManager)
                        {
                            TNLSManager.TNLSOthers.TNLSManager = TNLSManager; count++;
                        }

                        EditorUtility.DisplayDialog(
                            "TNetLibrarySystem ~ Fixing Tool",
                            $"All TNLSManager childs has been reset.\r\nFound {count} errors.",
                            "OK"
                        );
                    }
                    else
                    {
                        EditorUtility.DisplayDialog(
                            "TNetLibrarySystem ~ Fixing Tool",
                            $"You abort the fix. No change has been made to your scene.",
                            "OK"
                        );
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog(
                    "TNetLibrarySystem ~ Fixing Tool",
                        $"Please select your TNLSManager gameobject before doing that action.",
                        "OK"
                    );
                }
            }
            else
            {
                EditorUtility.DisplayDialog(
                "TNetLibrarySystem ~ Fixing Tool",
                    $"Please select your TNLSManager gameobject before doing that action.",
                    "OK"
                );
            }
        }

        [MenuItem("TNetLibrarySystem/Rebuild all networked scripts", priority = 20)]
        static void rebuildAll()
        {
            if(!Editor.BuildFiles.RebuildAllFiles())
            {
                EditorUtility.DisplayDialog(
                "TNetLibrarySystem ~ Fixing Tool",
                    $"Something gone wrong, please check your console.",
                    "OK"
                );
            }
        }

        [MenuItem("TNetLibrarySystem/Credits", priority = 40)]
        static void ShowVersion()
        {
            EditorUtility.DisplayDialog(
                "TNetLibrarySystem",
                   $"Version: {TNLSVersion.getBranch()}_{TNLSVersion.getVersion()}" +
                $"{n}Changelog: {TNLSVersion.getDescription()}" +
                $"{n}" +
                $"{n}TNLS, a networking package wrote by Tismatis for vrchat.",
                "OK"
            );
        }

        #region Utility

        static void BuildPrefabInTheScene(int NumberOfLine)
        {
            GameObject manager = new GameObject();
            manager.name = "TNLSManager";
            SetIcon(manager, (Texture2D)AssetDatabase.LoadAssetAtPath("Packages/fr.tismatis.tnetlibrarysystem/Resources/tnls.new.png", typeof(Texture2D)));
            TNLSManager TNLSManager = manager.AddComponent<TNLSManager>();

            GameObject linepool = new GameObject();
            linepool.name = "TNLS Line Pool";
            linepool.transform.SetParent(manager.transform);
            TNLSLinePool TNLSLinePool = linepool.AddComponent<TNLSLinePool>();
            TNLSManager.TNLSLinePool = TNLSLinePool;
            TNLSLinePool.TNLSManager = TNLSManager;

            for (int i = 0; i < NumberOfLine; i++)
            {
                GameObject line = new GameObject();
                line.name = $"NetworkLine ({i})";
                line.transform.SetParent(linepool.transform);
                line.AddComponent<TNLSLine>();
            }

            GameObject logingsystem = new GameObject();
            logingsystem.name = "TNLS Loging System";
            logingsystem.transform.SetParent(manager.transform);
            TNLSLogingSystem TNLSLogingSystem = logingsystem.AddComponent<TNLSLogingSystem>();
            TNLSManager.TNLSLogingSystem = TNLSLogingSystem;
            TNLSLogingSystem.TNLSManager = TNLSManager;

            GameObject serialization = new GameObject();
            serialization.name = "TNLS Serialization";
            serialization.transform.SetParent(manager.transform);
            TNLSSerialization TNLSSerialization = serialization.AddComponent<TNLSSerialization>();
            TNLSManager.TNLSSerialization = TNLSSerialization;
            TNLSSerialization.TNLSManager = TNLSManager;

            GameObject scriptManager = new GameObject();
            scriptManager.name = "TNLS Script Manager";
            scriptManager.transform.SetParent(manager.transform);
            TNLSScriptManager TNLSScriptManager = scriptManager.AddComponent<TNLSScriptManager>();
            TNLSManager.TNLSScriptManager = TNLSScriptManager;
            TNLSScriptManager.TNLSManager = TNLSManager;

            GameObject settings = new GameObject();
            settings.name = "TNLS Settings";
            settings.transform.SetParent(manager.transform);
            TNLSSettings TNLSSettings = settings.AddComponent<TNLSSettings>();
            TNLSManager.TNLSSettings = TNLSSettings;
            TNLSSettings.TNLSManager = TNLSManager;

            GameObject queue = new GameObject();
            queue.name = "TNLS Queue";
            queue.transform.SetParent(manager.transform);
            TNLSQueue TNLSQueue = queue.AddComponent<TNLSQueue>();
            TNLSManager.TNLSQueue = TNLSQueue;
            TNLSQueue.TNLSManager = TNLSManager;

            GameObject confirmPool = new GameObject();
            confirmPool.name = "TNLS Confirm Pool";
            confirmPool.transform.SetParent(manager.transform);
            TNLSConfirmPool TNLSConfirmPool = confirmPool.AddComponent<TNLSConfirmPool>();
            TNLSManager.TNLSConfirmPool = TNLSConfirmPool;
            TNLSConfirmPool.TNLSManager = TNLSManager;

            for (int i = 0; i < 82; i++)
            {
                GameObject playerConfirmReceived = new GameObject();
                playerConfirmReceived.name = $"PlayerConfirmReceived ({i})";
                playerConfirmReceived.transform.SetParent(confirmPool.transform);
                playerConfirmReceived.AddComponent<PlayerConfirmReceived>();
            }

            GameObject others = new GameObject();
            others.name = "TNLS Others";
            others.transform.SetParent(manager.transform);
            TNLSOthers TNLSOthers = others.AddComponent<TNLSOthers>();
            TNLSManager.TNLSOthers = TNLSOthers;
            TNLSOthers.TNLSManager = TNLSManager;

            manager.transform.SetSiblingIndex(0);
        }

        private static void SetIcon(GameObject gObj, Texture2D texture)
        {
            var ty = typeof(EditorGUIUtility);
            var mi = ty.GetMethod("SetIconForObject", BindingFlags.NonPublic | BindingFlags.Static);
            mi.Invoke(null, new object[] { gObj, texture });
        }

        #endregion Utility
    }
}