using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GBG.EditorIconsOverview.Editor
{
    public class IconHandle
    {
        public string IconName { get; }
        public HashSet<string> IconNameSet { get; }


        public IconHandle(string iconName, HashSet<string> iconNameSet)
        {
            IconName = iconName;
            IconNameSet = iconNameSet;
        }

        public string GetCharacterlessName()
        {
            string characterlessName = IconName;
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

        public Texture2D GetTexture2D()
        {
            //Texture2D icon = (Texture2D)EditorGUIUtility.LoadRequired(IconName); // 加载到的Icon模糊
            GUIContent iconContent = EditorGUIUtility.IconContent(IconName);
            Texture2D icon = (Texture2D)iconContent.image;

            return icon;
        }

        public bool HasSkin()
        {
            string anotherSkinName = GetAnotherSkinName(out _);
            return IconNameSet.Contains(anotherSkinName);
        }

        public string GetAnotherSkinName(out bool anotherIsProSkin)
        {
            string anotherSkinName;
            if (IconName.StartsWith("d_", StringComparison.OrdinalIgnoreCase))
            {
                anotherSkinName = IconName.Substring(2);
                anotherIsProSkin = false;
            }
            else
            {
                anotherSkinName = "d_" + IconName;
                anotherIsProSkin = true;
            }

            return anotherSkinName;
        }

        public string GetNameCodeWithSkin()
        {
            string anotherSkinName = GetAnotherSkinName(out bool anotherIsProSkin);
            if (!IconNameSet.Contains(anotherSkinName))
            {
                return $"\"{IconName}\";";
            }

            if (anotherIsProSkin)
            {
                return $"EditorGUIUtility.isProSkin ? \"{anotherSkinName}\" : \"{IconName}\";";
            }

            return $"EditorGUIUtility.isProSkin ? \"{IconName}\" : \"{anotherSkinName}\";";
        }

        public string GetIconContentCodeWithSkin()
        {
            string anotherSkinName = GetAnotherSkinName(out bool anotherIsProSkin);
            if (!IconNameSet.Contains(anotherSkinName))
            {
                return $"EditorGUIUtility.IconContent(\"{IconName}\", \"|\"); // tips: \"text|tooltip\"";
            }

            if (anotherIsProSkin)
            {
                return $"EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? \"{anotherSkinName}\" : \"{IconName}\", \"|\"); // tips: \"text|tooltip\"";
            }

            return $"EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? \"{IconName}\" : \"{anotherSkinName}\", \"|\"); // tips: \"text|tooltip\"";
        }

        public string GetIconContentCode()
        {
            return $"EditorGUIUtility.IconContent(\"{IconName}\", \"|\"); // tips: \"text|tooltip\"";
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

            if (a.IconName.StartsWith("d_", StringComparison.OrdinalIgnoreCase) &&
                !b.IconName.StartsWith("d_", StringComparison.OrdinalIgnoreCase))
            {
                return EditorGUIUtility.isProSkin ? 1 : -1;
            }

            if (!a.IconName.StartsWith("d_", StringComparison.OrdinalIgnoreCase) &&
                b.IconName.StartsWith("d_", StringComparison.OrdinalIgnoreCase))
            {
                return EditorGUIUtility.isProSkin ? -1 : 1;
            }

            if (a.IconName.EndsWith("@2x", StringComparison.OrdinalIgnoreCase) &&
                !b.IconName.EndsWith("@2x", StringComparison.OrdinalIgnoreCase))
            {
                return 1;
            }

            if (!a.IconName.EndsWith("@2x", StringComparison.OrdinalIgnoreCase) &&
                b.IconName.EndsWith("@2x", StringComparison.OrdinalIgnoreCase))
            {
                return -1;
            }

            return 0;
        }
    }
}