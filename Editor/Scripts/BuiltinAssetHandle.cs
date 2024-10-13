using System.IO;
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
#if UNITY_2022_3_OR_NEWER
            Selection.activeObject = LoadAsset();
#else
            UObject asset = LoadAsset();
            asset.hideFlags &= ~HideFlags.HideInInspector;
            Selection.activeObject = asset;
#endif
        }

        public virtual void SaveAs()
        {
            UObject asset = LoadAsset();
            if (!asset)
            {
                Debug.LogError($"Cannot load built in asset '{AssetName}'.");
                return;
            }

            string ext = null;
            if (asset is ScriptableObject)
                ext = "asset";
            else if (asset is Texture)
                ext = "png";

            string savePath = EditorUtility.SaveFilePanelInProject($"Save {AssetName}", AssetName, ext,
                 "Make sure the extension is correct");
            if (string.IsNullOrEmpty(savePath))
                return;

            // ScriptableObject
            if (asset is ScriptableObject scriptableObject)
            {
                ScriptableObject newInstance = UObject.Instantiate(scriptableObject);
                AssetDatabase.CreateAsset(newInstance, savePath);
                AssetDatabase.Refresh();
                Debug.Log($"Export asset '{AssetName}' to {savePath}", newInstance);
                return;
            }

            // Texture2D
            if (asset is Texture2D texture)
            {
                Texture2D readableTexture = new Texture2D(texture.width, texture.height, texture.format, texture.mipmapCount > 1);
                Graphics.CopyTexture(texture, readableTexture);
                File.WriteAllBytes(savePath, readableTexture.EncodeToPNG());
                AssetDatabase.Refresh();
                Debug.Log($"Export asset '{AssetName}' to {savePath}", AssetDatabase.LoadAssetAtPath<UObject>(savePath));
                return;
            }

            Debug.LogError($"The 'Save as' function for objects of type '{asset.GetType().FullName}' is not implemented.", asset);
        }
    }
}