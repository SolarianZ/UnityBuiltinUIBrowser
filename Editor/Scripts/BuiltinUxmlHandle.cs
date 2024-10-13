using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UObject = UnityEngine.Object;

namespace GBG.EditorIconsOverview.Editor
{
    public class BuiltinUxmlHandle : BuiltinAssetHandle
    {
        public BuiltinUxmlHandle(string ussName) : base(ussName) { }

        public VisualTreeAsset LoadVisualTree()
        {
            UObject loaded = LoadAsset();
            VisualTreeAsset visualTree = loaded as VisualTreeAsset;
            if (!visualTree)
                Debug.LogError($"Invalid VisualTreeAsset: {loaded}.", loaded);

            return visualTree;
        }


        public static List<BuiltinUxmlHandle> CreateHandles(IReadOnlyList<string> uxmlNames)
        {
            List<BuiltinUxmlHandle> handles = new List<BuiltinUxmlHandle>();
            for (int i = 0; i < uxmlNames.Count; i++)
            {
                string uxmlName = uxmlNames[i];
                BuiltinUxmlHandle handle = new BuiltinUxmlHandle(uxmlName);
                handles.Add(handle);
            }

            return handles;
        }
    }
}