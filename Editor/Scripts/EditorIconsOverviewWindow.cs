using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.EditorIconsOverview.Editor
{
    public class EditorIconsOverviewWindow : EditorWindow, IHasCustomMenu
    {
        [MenuItem("Tools/Bamboo/Editor Icons Overview")]
        public static void Open()
        {
            GetWindow<EditorIconsOverviewWindow>().Focus();
        }


        private ListView _listView;


        private void OnEnable()
        {
            titleContent = new GUIContent("Editor Icons Overview");
        }

        private void CreateGUI()
        {
            string[] editorIconNames = EditorIconUtility.EnumerateEditorIconNames().ToArray();
            List<IconHandle> iconHandles = IconHandle.CreateHandles(editorIconNames);

            _listView = new ListView(iconHandles, IconElement.MinHeight, MakeItem, BindItem)
            {
                showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly,
            };
            rootVisualElement.Add(_listView);
        }

        private VisualElement MakeItem()
        {
            return new IconElement();
        }

        private void BindItem(VisualElement element, int index)
        {
            IconElement iconElement = (IconElement)element;
            IconHandle iconHandle = (IconHandle)_listView.itemsSource[index];
            iconElement.SetIconHandle(iconHandle);
        }


        #region ContextMenu

        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
        }

        #endregion
    }
}