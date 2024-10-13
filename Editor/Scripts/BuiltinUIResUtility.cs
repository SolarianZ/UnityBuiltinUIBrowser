using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEngine;

namespace GBG.EditorIconsOverview.Editor
{
    public static class BuiltinUIResUtility
    {
        public static AssetBundle GetEditorAssetBundle()
        {
            MethodInfo getEditorAssetBundle = typeof(EditorGUIUtility).GetMethod("GetEditorAssetBundle",
                BindingFlags.NonPublic | BindingFlags.Static);
            return (AssetBundle)getEditorAssetBundle.Invoke(null, null);
        }

        public static List<string> GetBuiltinIconNames(AssetBundle editorAssetBundle = null)
        {
            if (!editorAssetBundle)
                editorAssetBundle = GetEditorAssetBundle();

            List<string> shortNames = new List<string>();
            string iconsPath = EditorResources.iconsPath;
            foreach (string assetName in editorAssetBundle.GetAllAssetNames())
            {
                if (!assetName.StartsWith(iconsPath, StringComparison.OrdinalIgnoreCase) &&
                    !assetName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!assetName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) &&
                    !assetName.EndsWith(".asset", StringComparison.OrdinalIgnoreCase)) // *icon.asset
                    continue;

                string shortName = Path.GetFileNameWithoutExtension(assetName);
                shortNames.Add(shortName);
            }

            return shortNames;
        }

        public static List<string> GetBuiltinUssNames(AssetBundle editorAssetBundle = null)
        {
            if (!editorAssetBundle)
                editorAssetBundle = GetEditorAssetBundle();

            List<string> shortNames = new List<string>();
            foreach (string assetName in editorAssetBundle.GetAllAssetNames())
            {
                if (!assetName.EndsWith(".uss", StringComparison.OrdinalIgnoreCase) &&
                    !assetName.EndsWith("uss.asset", StringComparison.OrdinalIgnoreCase)) // *uss.asset
                    continue;

                string shortName = Path.GetFileName(assetName);
                shortNames.Add(shortName);
            }

            return shortNames;
        }

        public static List<string> GetBuiltinUxmlNames(AssetBundle editorAssetBundle = null)
        {
            if (!editorAssetBundle)
                editorAssetBundle = GetEditorAssetBundle();

            List<string> shortNames = new List<string>();
            foreach (string assetName in editorAssetBundle.GetAllAssetNames())
            {
                if (!assetName.EndsWith(".uxml", StringComparison.OrdinalIgnoreCase) &&
                    !assetName.EndsWith("uxml.asset", StringComparison.OrdinalIgnoreCase)) // *uxml.asset
                    continue;

                string shortName = Path.GetFileName(assetName);
                shortNames.Add(shortName);
            }

            return shortNames;
        }

        //private static string GetThisFilePath([CallerFilePath] string callerFilePath = "") => callerFilePath;
    }
}