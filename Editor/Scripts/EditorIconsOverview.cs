using UnityEditor;
using UnityEngine;

namespace GBG.EditorIconsOverview.Editor
{
    public class EditorIconsOverviewWindow : EditorWindow
    {
        [MenuItem("Tools/Bamboo/Editor Icons Overview")]
        public static void Open()
        {
            GetWindow<EditorIconsOverviewWindow>().Focus();
        }

        private void OnEnable()
        {
            titleContent = new GUIContent("Editor Icons Overview");
        }
    }
}