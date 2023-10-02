
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tismatis.TNetLibrarySystem;
using Tismatis.TNetLibrarySystem.Coding;
using UdonSharp;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tismatis.TNetLibrarySystem.Editor
{
    public class BuildFiles
    {
        private static string root = $"{Directory.GetParent(UnityEngine.Application.dataPath)}/TNLStemp";

        private static string t = "    ";
        private static string n = "\r\n";

        public static bool DetectUncompatibleComponent()
        {
            var anstn = SceneManager.GetActiveScene().GetRootGameObjects().Where(go => go.GetComponent<AssignNewScriptToNetwork>() != null).ToArray();

            if(anstn.Length > 0 )
            {
                TNLSLog.SendLog(messageType.defaultError, logAuthorList.buildFileDetectUncompatibleComponent, "Can't process, uncompatible components found!");
                TNLSLog.SendLog(messageType.defaultError, logAuthorList.buildFileDetectUncompatibleComponent, "Please delete your TNLS folder in Packages and unzip the new version.");
                return false;
            }
            else
            {
                TNLSLog.SendLog(messageType.defaultSuccess, logAuthorList.buildFileDetectUncompatibleComponent, "No blacklisted component found.");
                return true;
            }
        }

        public static bool AutoSelectManager()
        {
            var manager = SceneManager.GetActiveScene().GetRootGameObjects().Where(go => go.GetComponent<TNLSManager>() != null).ToArray();

            TNLSLog.SendLog(messageType.defaultSuccess, logAuthorList.buildFileAutoSelectManager, "Attaching all TNLSManager where is needed.");

            if (manager.Length == 1)
            {
                foreach (NetworkedClass udb in Resources.FindObjectsOfTypeAll<NetworkedClass>())
                {
                    if (!EditorUtility.IsPersistent(udb.transform.root.gameObject) && !(udb.hideFlags == HideFlags.NotEditable || udb.hideFlags == HideFlags.HideAndDontSave))
                    {
                        if (udb.TNLSManager == null)
                        {
                            udb.TNLSManager = manager[0].GetComponent<TNLSManager>();
                        }
                    }
                }
                foreach (TNLSCustomButtonSender udb in Resources.FindObjectsOfTypeAll<TNLSCustomButtonSender>())
                {
                    if (!EditorUtility.IsPersistent(udb.transform.root.gameObject) && !(udb.hideFlags == HideFlags.NotEditable || udb.hideFlags == HideFlags.HideAndDontSave))
                    {
                        if (udb.TNLSManager == null)
                        {
                            udb.TNLSManager = manager[0].GetComponent<TNLSManager>();
                        }
                    }
                }
                TNLSLog.SendLog(messageType.defaultSuccess, logAuthorList.buildFileAutoSelectManager, "We attached all TNLSManager where is needed.");
                return true;
            }
            else
            {
                TNLSLog.SendLog(messageType.defaultWarn, logAuthorList.buildFileAutoSelectManager, (manager.Length == 0) ? "No TNLS Manager has been detected, please check if TNLS is correctly setup." : "We found multiple instance of TNLS Manager, you just need one instance per scene.");
                return false;
            }
        }

        public static bool RebuildAllFiles()
        {
            TNLSLog.SendLog(messageType.defaultInfo, logAuthorList.buildFileRebuildAllFiles, "Starting the building of the TNLS dynamic layer.");

            if (Directory.Exists(root))
            {
                Directory.Delete(root, true);
            }
            Directory.CreateDirectory(root);

            UdonSharpBehaviour[] udbList = new UdonSharpBehaviour[0];

            foreach (UdonSharpBehaviour udb in Resources.FindObjectsOfTypeAll<NetworkedClass>())
            {
                if (!EditorUtility.IsPersistent(udb.transform.root.gameObject) && !(udb.hideFlags == HideFlags.NotEditable || udb.hideFlags == HideFlags.HideAndDontSave))
                {
                    udbList = udbList.Add(udb);
                }
            }

            foreach (UdonSharpBehaviour udb in udbList)
            {
                string res = Directory.GetFiles(UnityEngine.Application.dataPath, $"{udb.GetUdonTypeName()}.cs", SearchOption.AllDirectories)[0];
                string newLink = $"{root}/{res.Substring(res.IndexOf("Assets\\") + 7)}";

                if (!Directory.Exists(Path.GetDirectoryName(newLink)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(newLink));
                }

                try
                {
                    string contentFile = ExemptFile(res, udb.GetUdonTypeName());
                    File.WriteAllText(newLink, contentFile);

                    File.WriteAllText(res, ImplementFile(udb, contentFile));
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    return false;
                }
            }

            TNLSLog.SendLog(messageType.defaultInfo, logAuthorList.buildFileRebuildAllFiles, "Finished, compiling all script.");

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

            TNLSLog.SendLog(messageType.defaultSuccess, logAuthorList.buildFileRebuildAllFiles, "Compiled!");

            return true;
        }

        private static string ImplementFile(UdonSharpBehaviour script, string content)
        {
            int startWritingPoint = content.Substring(content.IndexOf(script.GetUdonTypeName())).IndexOf("{") + content.IndexOf(script.GetUdonTypeName()) + 1;

            int testGetTab = 0;
            string tmp = "private class";
            if (content.IndexOf("private class") != -1)
            {
                testGetTab = content.IndexOf("private class") + 1;
            }
            else
            {
                testGetTab = content.IndexOf("public class") + 1;
                tmp = "public class";
            }

            int tmpp = content.Substring(0, testGetTab).LastIndexOf("\r\n") + 2;
            string tmppp = content.Substring(tmpp, content.IndexOf(tmp) - tmpp);
            int numberOfTab = (tmppp.Length > 0) ? tmppp.Length / 4 : 0;

            string newString = $"{n}{tab(1 + numberOfTab)}public override void TNLScallMethod(string methodName, object[] parameters){n}{tab(1 + numberOfTab)}{{{n}{tab(2 + numberOfTab)}switch(methodName){n}{tab(2 + numberOfTab)}{{";

            MethodInfo[] methods = script.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.OptionalParamBinding).Where(m => m.GetCustomAttributes(typeof(NetworkedMethod), true).Length > 0).ToArray();
            foreach (MethodInfo method in methods)
            {
                if (method.GetParameters().Length == 1 && method.GetParameters()[0].ParameterType.Name == "Object[]")
                {
                    newString += $"{n}{tab(3 + numberOfTab)}case \"{method.Name}\":{n}{tab(4 + numberOfTab)}{method.Name}(parameters);{n}{tab(4 + numberOfTab)}break;";
                }
                else if (method.GetParameters().Length == 0)
                {
                    newString += $"{n}{tab(3 + numberOfTab)}case \"{method.Name}\":{n}{tab(4 + numberOfTab)}{method.Name}();{n}{tab(4 + numberOfTab)}break;";
                }
                else
                {
                    TNLSLog.SendLog(messageType.defaultWarn, logAuthorList.buildFileImplementFile, $"An NetworkedMethod hasn't the good architecture! Please check {method.Name} in {script.GetUdonTypeName()}.");
                }
            }

            newString += $"{n}{tab(3 + numberOfTab)}default:{n}{tab(4 + numberOfTab)}Debug.Log(\"[TNLS-Dynamic] We can't find the method requested.\");{n}{tab(4 + numberOfTab)}break;{n}{tab(2 + numberOfTab)}}}{n}{tab(1 + numberOfTab)}}}// EndOfWork";

            return content.Substring(0, startWritingPoint) + newString + content.Substring(startWritingPoint);
        }

        public static string tab(int y)
        {
            string str = "";
            for (int i = 0; i < y; i++)
            {
                str += t;
            }
            return str;
        }

        private static string ExemptFile(string url, string className)
        {
            string content = File.ReadAllText(url);


            int testGetTab = 0;
            string tmp = "private class";
            if (content.IndexOf("private class") != -1)
            {
                testGetTab = content.IndexOf("private class") + 1;
            }
            else
            {
                testGetTab = content.IndexOf("public class") + 1;
                tmp = "public class";
            }

            int tmpp = content.Substring(0, testGetTab).LastIndexOf("\r\n") + 2;
            string tmppp = content.Substring(tmpp, content.IndexOf(tmp) - tmpp);
            int numberOfTab = (tmppp.Length > 0) ? tmppp.Length / 4 : 0;


            int startWritingPoint = content.IndexOf($"{n}{tab(1 + numberOfTab)}public override void TNLScallMethod(string methodName, object[] parameters)");

            if (content.IndexOf($"{n}{tab(1 + numberOfTab)}public override void TNLScallMethod(string methodName, object[] parameters)") != -1)
            {
                int endWritingPoint = content.IndexOf($"}}// EndOfWork") + $"}}// EndOfWork".Length;
                content = $"{content.Substring(0, startWritingPoint)}{content.Substring(endWritingPoint)}";
            }

            return content;
        }
    }
}
