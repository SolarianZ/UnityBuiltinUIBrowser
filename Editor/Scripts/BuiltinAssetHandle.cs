using System;
using UnityEditor;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace GBG.EditorIconsOverview.Editor
{
    public class BuiltinAssetHandle
    {
        public string AssetName { get; }


        public BuiltinAssetHandle(string assetName)
        {
            AssetName = assetName;
        }

        public string GetLoadingCode(string assetTypeShortName)
        {
            if (string.IsNullOrEmpty(assetTypeShortName))
                return $"EditorGUIUtility.LoadRequired(\"{AssetName}\")";

            return $"({assetTypeShortName})EditorGUIUtility.LoadRequired(\"{AssetName}\")";
        }

        public UObject LoadAsset()
        {
            UObject asset = EditorGUIUtility.LoadRequired(AssetName);
            return asset;
        }

        public void Inspect()
        {
            Selection.activeObject = LoadAsset();
        }

        public void SaveAs()
        {
            UObject asset = LoadAsset();
            if (!asset)
            {
                Debug.LogError($"Cannot load built in asset '{AssetName}'.");
                return;
            }

            string ext = null;
            Type assetType = asset.GetType();
            if (typeof(ScriptableObject).IsAssignableFrom(assetType))
                ext = "asset";

            string savePath = EditorUtility.SaveFilePanelInProject($"Save {AssetName}", AssetName, ext,
                 "Make sure the extension is correct");
            if (string.IsNullOrEmpty(savePath))
                return;

            AssetDatabase.CreateAsset(asset, savePath);
            AssetDatabase.Refresh(); ;
        }
    }
}