using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UObject = UnityEngine.Object;

namespace GBG.EditorIconsOverview.Editor
{
    public class BuiltinUssHandle : BuiltinAssetHandle
    {
        public BuiltinUssHandle(string ussName) : base(ussName) { }

        public StyleSheet LoadStyleSheet()
        {
            UObject loaded = LoadAsset();
            StyleSheet styleSheet = loaded as StyleSheet;
            if (!styleSheet)
                Debug.LogError($"Invalid StyleSheet: {loaded}.", loaded);

            return styleSheet;
        }


        public static List<BuiltinUssHandle> CreateHandles(IReadOnlyList<string> ussNames)
        {
            List<BuiltinUssHandle> handles = new List<BuiltinUssHandle>();
            for (int i = 0; i < ussNames.Count; i++)
            {
                string ussName = ussNames[i];
                BuiltinUssHandle handle = new BuiltinUssHandle(ussName);
                handles.Add(handle);
            }

            Debug.Log($"{handles.Count} editor built-in uss found.");

            return handles;
        }
    }
}