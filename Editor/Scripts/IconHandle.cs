using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GBG.EditorIconsOverview.Editor
{
    public class IconHandle
    {
        public string RawIconName { get; }
        public HashSet<string> IconNameSet { get; }


        public IconHandle(string rawIconName, HashSet<string> rawIconNameSet)
        {
            RawIconName = rawIconName;
            IconNameSet = rawIconNameSet;
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

        public Texture2D LoadIcon()
        {
            return (Texture2D)EditorGUIUtility.LoadRequired(RawIconName);
        }


        public static List<IconHandle> CreateHandles(IReadOnlyList<string> iconNames)
        {
            HashSet<string> iconNameSet = new HashSet<string>(iconNames);
            List<IconHandle> handles = new List<IconHandle>();
            for (int i = 0; i < iconNames.Count; i++)
            {
                string iconName = iconNames[i];
                IconHandle handle = new IconHandle(iconName, iconNameSet);
                handles.Add(handle);
            }

            handles.Sort(IconHandleComparison);
            Debug.Log($"{handles.Count} editor built-in icons found.");

            return handles;
        }

        public static int IconHandleComparison(IconHandle a, IconHandle b)
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