
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UdonSharp;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using VRC.SDKBase.Editor.BuildPipeline;

namespace Tismatis.TNetLibrarySystem.Editor
{
    public class BeforeBuild : IVRCSDKBuildRequestedCallback
    {
        public int callbackOrder => -2;

        public bool OnBuildRequested(VRCSDKRequestedBuildType requestedBuildType)
        {
            if (!Editor.BuildFiles.DetectUncompatibleComponent() || !Editor.BuildFiles.AutoSelectManager() || !Editor.BuildFiles.RebuildAllFiles()) {
                EditorUtility.DisplayDialog(
                "TNetLibrarySystem ~ Builder",
                    $"Something gone wrong, please check your console.",
                    "OK"
                );
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}