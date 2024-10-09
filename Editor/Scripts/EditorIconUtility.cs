using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.Assertions;
using UObject = UnityEngine.Object;

namespace GBG.EditorIconsOverview.Editor
{
    public static class EditorIconUtility
    {
        public static AssetBundle GetEditorAssetBundle()
        {
            MethodInfo getEditorAssetBundle = typeof(EditorGUIUtility).GetMethod(
                "GetEditorAssetBundle", BindingFlags.NonPublic | BindingFlags.Static);
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

                if (assetName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) == false &&
                    assetName.EndsWith(".asset", StringComparison.OrdinalIgnoreCase) == false)
                    continue;

                yield return assetName;
            }
        }

        public static void ExportEditorIcons()
        {
            EditorUtility.DisplayProgressBar("Export Editor Icons", "Exporting...", 0.0f);
            try
            {
                AssetBundle editorAssetBundle = GetEditorAssetBundle();
                int count = 0;
                string iconsPath = EditorResources.iconsPath;
                string iconExportFolder = GetIconExportFolder();
                string anyIconPath = null;
                foreach (string iconName in EnumerateEditorIconNames(editorAssetBundle))
                {
                    Texture2D icon = editorAssetBundle.LoadAsset<Texture2D>(iconName);
                    if (!icon)
                        continue;

                    Texture2D readableTexture = new Texture2D(icon.width, icon.height, icon.format, icon.mipmapCount > 1);
                    Graphics.CopyTexture(icon, readableTexture);

                    string folderPath = Path.GetDirectoryName(Path.Combine(iconExportFolder, iconName.Substring(iconsPath.Length)));
                    if (Directory.Exists(folderPath) == false)
                        Directory.CreateDirectory(folderPath);

                    string iconPath = Path.Combine(folderPath, icon.name + ".png");
                    File.WriteAllBytes(iconPath, readableTexture.EncodeToPNG());

                    count++;
                    if (string.IsNullOrEmpty(anyIconPath))
                        anyIconPath = iconPath;
                }

                Debug.Log($"{count} editor icons has been exported: {iconExportFolder}");

                EditorUtility.RevealInFinder(anyIconPath ?? iconExportFolder);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private static string GetIconExportFolder()
        {
            string projectFolder = Application.dataPath.Remove(Application.dataPath.Length - "Assets".Length);
            string thisFilePath = Path.Combine(projectFolder, GetThisFilePath());
            string thisFileFolder = Path.GetDirectoryName(thisFilePath);
            string iconExportFolder = Path.Combine(thisFileFolder, "../../Documents~/icons/");
            Assert.IsTrue(Directory.Exists(iconExportFolder));

            return iconExportFolder;
        }

        private static string GetThisFilePath([CallerFilePath] string callerFilePath = "") => callerFilePath;

        public static long GetObjectLocalFileId(this UObject obj)
        {
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out string guid, out long localId);
            return localId;
        }

        public static void GenerateEditorIconMarkdown(string outputPath = null)
        {
            if (string.IsNullOrEmpty(outputPath))
                outputPath = EditorUtility.SaveFilePanel("Editor Icon Markdown", $"{Application.dataPath}/..", "UnityEditorIcons", "md");
            if (string.IsNullOrEmpty(outputPath))
                return;

            if (!outputPath.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
                outputPath += ".md";

            string outputFolder = Path.GetDirectoryName(outputPath);
            if (!Directory.Exists(outputFolder))
                Directory.CreateDirectory(outputFolder);

            EditorUtility.DisplayProgressBar("Generate README.md", "Generating...", 0.0f);
            try
            {
                AssetBundle editorAssetBundle = GetEditorAssetBundle();
                string iconsPath = EditorResources.iconsPath;
                StringBuilder readmeContents = new StringBuilder();

                readmeContents.AppendLine($"Unity Editor Built-in Icons");
                readmeContents.AppendLine($"==============================");
                readmeContents.AppendLine($"Unity version: {Application.unityVersion}");
                readmeContents.AppendLine($"Icons what can load using `EditorGUIUtility.IconContent`");
                readmeContents.AppendLine();
                readmeContents.AppendLine($"File ID");
                readmeContents.AppendLine($"-------------");
                readmeContents.AppendLine($"You can change script icon by file id");
                readmeContents.AppendLine($"1. Open `*.cs.meta` in Text Editor");
                readmeContents.AppendLine($"2. Modify line `icon: {{instanceID: 0}}` to `icon: {{fileID: <FILE ID>, guid: 0000000000000000d000000000000000, type: 0}}`");
                readmeContents.AppendLine($"3. Save and focus Unity Editor");
                readmeContents.AppendLine();
                readmeContents.AppendLine($"| Icon | Name | File ID |");
                readmeContents.AppendLine($"|------|------|---------|");

                string[] assetNames = EnumerateEditorIconNames(editorAssetBundle).ToArray();
                for (int i = 0; i < assetNames.Length; i++)
                {
                    string iconName = assetNames[i];
                    Texture2D icon = editorAssetBundle.LoadAsset<Texture2D>(iconName);
                    if (icon == null)
                        continue;

                    EditorUtility.DisplayProgressBar("Generate README.md", $"Generating... ({i + 1}/{assetNames.Length})", (float)i / assetNames.Length);

                    Texture2D readableTexture = new Texture2D(icon.width, icon.height, icon.format, icon.mipmapCount > 1);
                    Graphics.CopyTexture(icon, readableTexture);

                    string folderPath = Path.GetDirectoryName(Path.Combine(outputFolder, iconName.Substring(iconsPath.Length)));
                    if (Directory.Exists(folderPath) == false)
                        Directory.CreateDirectory(folderPath);

                    string iconPath = Path.Combine(folderPath, icon.name + ".png");
                    File.WriteAllBytes(iconPath, readableTexture.EncodeToPNG());

                    long iconFileId = icon.GetObjectLocalFileId();
                    string escapedUrl = iconPath.Replace(outputFolder, ".").Replace(" ", "%20").Replace('\\', '/');
                    readmeContents.AppendLine($"| ![]({escapedUrl}) | `{icon.name}` | `{iconFileId}` |");
                }

                File.WriteAllText(outputPath, readmeContents.ToString(), Encoding.UTF8);

                Debug.Log($"Editor icon markdown has been generated: {outputPath}");
                EditorUtility.RevealInFinder(outputPath);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}