using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace GBG.EditorIconsOverview.Editor
{
    public class BuiltinIconHandle : BuiltinAssetHandle
    {
        public string RawIconName { get; }


        public BuiltinIconHandle(string rawIconName) : base(rawIconName)
        {
            RawIconName = rawIconName;
        }

        public string GetIconName()
        {
            // Unity会根据主题自动追加 d_ 前缀
            if (RawIconName.StartsWith("d_", StringComparison.OrdinalIgnoreCase))
                return RawIconName.Substring(2);

            return RawIconName;
        }

        public string GetIconContentCode()
        {
            return $"EditorGUIUtility.IconContent(\"{GetIconName()}\", \"|\"); // tips: \"text|tooltip\"";
        }

        internal string GetCharacterlessName()
        {
            string characterlessName = RawIconName;
            if (characterlessName.StartsWith("d_", StringComparison.OrdinalIgnoreCase))
            {
                characterlessName = characterlessName.Substring(2);
            }

            if (characterlessName.EndsWith("@2x", StringComparison.OrdinalIgnoreCase))
            {
                characterlessName = characterlessName.Substring(0, characterlessName.Length - 3);
            }

            return characterlessName;
        }

        public Texture LoadTexture()
        {
            UObject loaded = EditorGUIUtility.LoadRequired(RawIconName);
            Texture icon = loaded as Texture;
            if (!icon)
                Debug.LogError($"Invalid Texture: {loaded}.", loaded);

            return icon;
        }


        public static List<BuiltinIconHandle> CreateHandles(IReadOnlyList<string> iconNames)
        {
            List<BuiltinIconHandle> handles = new List<BuiltinIconHandle>();
            for (int i = 0; i < iconNames.Count; i++)
            {
                string iconName = iconNames[i];
                BuiltinIconHandle handle = new BuiltinIconHandle(iconName);
                handles.Add(handle);
            }

            handles.Sort(IconHandleComparison);
            Debug.Log($"{handles.Count} editor built-in icons found.");

            return handles;
        }

        public static int IconHandleComparison(BuiltinIconHandle a, BuiltinIconHandle b)
        {
            string characterlessNameA = a.GetCharacterlessName();
            string characterlessNameB = b.GetCharacterlessName();
            int ret = string.Compare(characterlessNameA, characterlessNameB, StringComparison.OrdinalIgnoreCase);
            if (ret != 0)
            {
                return ret;
            }

            if (a.RawIconName.StartsWith("d_", StringComparison.OrdinalIgnoreCase) &&
                !b.RawIconName.StartsWith("d_", StringComparison.OrdinalIgnoreCase))
            {
                return EditorGUIUtility.isProSkin ? 1 : -1;
            }

            if (!a.RawIconName.StartsWith("d_", StringComparison.OrdinalIgnoreCase) &&
                b.RawIconName.StartsWith("d_", StringComparison.OrdinalIgnoreCase))
            {
                return EditorGUIUtility.isProSkin ? -1 : 1;
            }

            if (a.RawIconName.EndsWith("@2x", StringComparison.OrdinalIgnoreCase) &&
                !b.RawIconName.EndsWith("@2x", StringComparison.OrdinalIgnoreCase))
            {
                return 1;
            }

            if (!a.RawIconName.EndsWith("@2x", StringComparison.OrdinalIgnoreCase) &&
                b.RawIconName.EndsWith("@2x", StringComparison.OrdinalIgnoreCase))
            {
                return -1;
            }

            return 0;
        }
    }
}