using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
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


        private ListView _iconListView;
        private List<IconHandle> _allIconHandles;
        private List<IconHandle> _filteredIconHandles;


        #region Unity Messages

        private void OnEnable()
        {
            titleContent = new GUIContent("Editor Icons Overview");

            string[] iconNames = EditorIconUtility.EnumerateEditorIconNames().ToArray();
            _allIconHandles = IconHandle.CreateHandles(iconNames);
            _filteredIconHandles = new List<IconHandle>(_allIconHandles);
        }

        private void ShowButton(Rect pos)
        {
            if (GUI.Button(pos, EditorGUIUtility.IconContent("_Help"), GUI.skin.FindStyle("IconButton")))
            {
                Application.OpenURL("https://github.com/SolarianZ/UnityEditorIconsOverview");
            }
        }

        private void CreateGUI()
        {
            // Toolbar
            Toolbar toolbar = new Toolbar();
            rootVisualElement.Add(toolbar);

            // Icon Name Search Field
            ToolbarSearchField iconSearchField = new ToolbarSearchField
            {
                style =
                {
                    flexGrow = 1
                }
            };
            iconSearchField.RegisterValueChangedCallback(OnIconSearchContentChanged);
            toolbar.Add(iconSearchField);

            // Icon ListView
            _iconListView = new ListView(_filteredIconHandles, IconElement.MinHeight, MakeItem, BindItem)
            {
                showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly,
            };
            rootVisualElement.Add(_iconListView);
        }

        #endregion


        private void OnIconSearchContentChanged(ChangeEvent<string> evt)
        {
            string searchContent = evt.newValue;
            _filteredIconHandles = _allIconHandles
                .Where(handle => handle.RawIconName.Contains(searchContent, StringComparison.OrdinalIgnoreCase))
                .ToList();
            _iconListView.itemsSource = _filteredIconHandles;
            _iconListView.Rebuild();
        }

        private VisualElement MakeItem()
        {
            return new IconElement();
        }

        private void BindItem(VisualElement element, int index)
        {
            IconElement iconElement = (IconElement)element;
            IconHandle iconHandle = (IconHandle)_iconListView.itemsSource[index];
            iconElement.SetIconHandle(iconHandle);
        }


        #region ContextMenu

        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
            // Source Code
            menu.AddItem(new GUIContent("Source Code"), false, () =>
            {
                Application.OpenURL("https://github.com/SolarianZ/UnityEditorIconsOverview");
            });
        }

        #endregion
    }
}