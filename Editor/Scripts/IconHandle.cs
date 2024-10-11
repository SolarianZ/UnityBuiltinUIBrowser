using System;
using UnityEditor;
using UnityEngine;

namespace GBG.EditorIconsOverview.Editor
{
    [Serializable]
    public class IconHandle
    {
        public string IconName;
        public bool HasSkin;
        public bool Has2x;


        public IconHandle(string iconName, bool hasSkin, bool has2x)
        {
            IconName = iconName;
            HasSkin = hasSkin;
            Has2x = has2x;
        }

        public string GetName(bool isProSkin, bool is2x)
        {
            string finalIconName;
            if (HasSkin && isProSkin && !IconName.StartsWith("d_", StringComparison.OrdinalIgnoreCase))
            {
                if (Has2x && is2x && !IconName.EndsWith("@2x", StringComparison.OrdinalIgnoreCase))
                {
                    finalIconName = $"d_{IconName}@2x";
                }
                else
                {
                    finalIconName = $"d_{IconName}";
                }
            }
            else
            {
                if (Has2x && is2x && !IconName.EndsWith("@2x", StringComparison.OrdinalIgnoreCase))
                {
                    finalIconName = $"{IconName}@2x";
                }
                else
                {
                    finalIconName = IconName;
                }
            }

            return finalIconName;
        }

        public Texture2D GetTexture2D(bool isProSkin, bool is2x)
        {
            string finalIconName = GetName(isProSkin, is2x);
            //Texture2D icon = (Texture2D)EditorGUIUtility.LoadRequired(finalIconName); // 加载到的Icon模糊
            GUIContent iconContent = EditorGUIUtility.IconContent(finalIconName);
            Texture2D icon = (Texture2D)iconContent.image;

            return icon;
        }
    }
}