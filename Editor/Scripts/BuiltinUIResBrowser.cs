using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.EditorIconsOverview.Editor
{
    public class BuiltinUIResBrowser : EditorWindow, IHasCustomMenu
    {
        [MenuItem("Tools/Bamboo/Built-in UI Res Browser")]
        public static void Open()
        {
            GetWindow<BuiltinUIResBrowser>().Focus();
        }


        private ListView _iconListView;
        private List<BuiltinIconHandle> _allIconHandles;
        private List<BuiltinIconHandle> _filteredIconHandles;


        #region Unity Messages

        private void OnEnable()
        {
            titleContent = new GUIContent("Built-in UI Res Browser");

            List<string> iconNames = BuiltinUIResUtility.GetBuiltinIconNames();
            _allIconHandles = BuiltinIconHandle.CreateHandles(iconNames);
            _filteredIconHandles = new List<BuiltinIconHandle>(_allIconHandles);
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
            _iconListView = new ListView(_filteredIconHandles, BuiltinIconElement.MinHeight, MakeItem, BindItem)
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
            return new BuiltinIconElement();
        }

        private void BindItem(VisualElement element, int index)
        {
            BuiltinIconElement iconElement = (BuiltinIconElement)element;
            BuiltinIconHandle iconHandle = (BuiltinIconHandle)_iconListView.itemsSource[index];
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