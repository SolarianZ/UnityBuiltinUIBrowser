using UnityEditor;
using UnityEngine;

namespace GBG.EditorIconsOverview.Editor
{
    public class EditorIconsOverviewWindow : EditorWindow, IHasCustomMenu
    {
        [MenuItem("Tools/Bamboo/Editor Icons Overview")]
        public static void Open()
        {
            GetWindow<EditorIconsOverviewWindow>().Focus();
        }


        #region Icons

        [SerializeField]
        private Texture2D _clipboardIcon;

        #endregion


        private void OnEnable()
        {
            titleContent = new GUIContent("Editor Icons Overview");
        }

        private void CreateGUI()
        {
            IconElement icon = new IconElement();
            icon.Image.image = _clipboardIcon;
            rootVisualElement.Add(icon);
        }


        #region ContextMenu

        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Export Editor Icons"), false, EditorIconUtility.ExportEditorIcons);
            menu.AddItem(new GUIContent("Generate Editor Icon Markdown"), false, () => EditorIconUtility.GenerateEditorIconMarkdown());
        }

        #endregion
    }
}