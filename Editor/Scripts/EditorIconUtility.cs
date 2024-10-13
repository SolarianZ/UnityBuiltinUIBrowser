using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace GBG.EditorIconsOverview.Editor
{
    public static class EditorIconUtility
    {
        public static AssetBundle GetEditorAssetBundle()
        {
            MethodInfo getEditorAssetBundle = typeof(EditorGUIUtility).GetMethod("GetEditorAssetBundle",
                BindingFlags.NonPublic | BindingFlags.Static);
            return (AssetBundle)getEditorAssetBundle.Invoke(null, null);
        }

        public static IEnumerable<string> EnumerateEditorIconNames(AssetBundle editorAssetBundle = null)
        {
            string iconsPath = EditorResources.iconsPath;
            if (!editorAssetBundle)
                editorAssetBundle = GetEditorAssetBundle();

            foreach (string assetName in editorAssetBundle.GetAllAssetNames())
            {
                if (assetName.StartsWith(iconsPath, StringComparison.OrdinalIgnoreCase) == false)
                    continue;

                if (!assetName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) &&
                    !assetName.EndsWith(".asset", StringComparison.OrdinalIgnoreCase))
                    continue;

                string shortName = Path.GetFileNameWithoutExtension(assetName);
                yield return shortName;
            }
        }

        public static long GetObjectLocalFileId(this UObject obj)
        {
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out string guid, out long localId);
            return localId;
        }

        //private static string GetThisFilePath([CallerFilePath] string callerFilePath = "") => callerFilePath;
    }
}