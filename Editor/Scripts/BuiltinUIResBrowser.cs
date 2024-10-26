using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.EditorIconsOverview.Editor
{
    internal enum UIResType
    {
        Icon,
        StyleSheet,
        VisualTree,
    }

    public class BuiltinUIResBrowser : EditorWindow, IHasCustomMenu
    {
        [MenuItem("Tools/Bamboo/Built-in UI Res Browser")]
        public static void Open()
        {
            GetWindow<BuiltinUIResBrowser>().Focus();
        }


        private const UIResType _DefaultResType = UIResType.Icon;
        private EnumField _resTypeField;
        private ToolbarSearchField _searchField;
        private ListView _assetListView;
        private List<BuiltinAssetHandle> _allAssetHandles;
        private List<BuiltinAssetHandle> _filteredAssetHandles;


        #region Unity Messages

        private void ShowButton(Rect pos)
        {
            if (GUI.Button(pos, EditorGUIUtility.IconContent("_Help"), GUI.skin.FindStyle("IconButton")))
            {
                Application.OpenURL("https://github.com/SolarianZ/UnityBuiltinUIResBrowser");
            }
        }

        private void OnEnable()
        {
            titleContent = new GUIContent("Built-in UI Res Browser");
            minSize = new Vector2(300, 150);
        }

        private void CreateGUI()
        {
            // Toolbar
            Toolbar toolbar = new Toolbar();
            rootVisualElement.Add(toolbar);

            // Type Field
            _resTypeField = new EnumField(_DefaultResType)
            {
                style =
                {
                    width = 90,
                    flexShrink = 0,
                }
            };
            _resTypeField.RegisterValueChangedCallback(OnResTypeChanged);
            toolbar.Add(_resTypeField);

            // Search Field
            _searchField = new ToolbarSearchField
            {
                style =
                {
                    flexGrow = 1,
                    flexShrink = 1,
                }
            };
            _searchField.RegisterValueChangedCallback(OnSearchContentChanged);
            toolbar.Add(_searchField);

            // ListView
            _assetListView = new ListView
            {
                makeItem = MakeItem,
                bindItem = BindItem,
#if UNITY_2021_3_OR_NEWER
                showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly,
#else
                style =
                {
                    flexGrow = 1,
                },
#endif
            };
            rootVisualElement.Add(_assetListView);

            // Refresh
            UpdateAssetHandles();
        }

        #endregion


        private void UpdateAssetHandles()
        {
            UIResType _resType = _resTypeField != null ? (UIResType)_resTypeField.value : _DefaultResType;
            float listItemHeight;
            switch (_resType)
            {
                case UIResType.Icon:
                    List<string> iconNames = BuiltinUIResUtility.GetBuiltinIconNames();
                    _allAssetHandles = BuiltinIconHandle.CreateHandles(iconNames)
                        .Select(handle => (BuiltinAssetHandle)handle)
                        .ToList();
                    listItemHeight = BuiltinIconElement.MinHeight;
                    break;

                case UIResType.StyleSheet:
                    List<string> ussNames = BuiltinUIResUtility.GetBuiltinUssNames();
                    _allAssetHandles = BuiltinUssHandle.CreateHandles(ussNames)
                        .Select(handle => (BuiltinAssetHandle)handle)
                        .ToList();
                    listItemHeight = BuiltinAssetElement.MinHeight;
                    break;

                case UIResType.VisualTree:
                    List<string> uxmlNames = BuiltinUIResUtility.GetBuiltinUxmlNames();
                    _allAssetHandles = BuiltinUxmlHandle.CreateHandles(uxmlNames)
                        .Select(handle => (BuiltinAssetHandle)handle)
                        .ToList();
                    listItemHeight = BuiltinAssetElement.MinHeight;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_resType), _resType, null);
            }


            string searchContent = _searchField?.value ?? string.Empty;
            _filteredAssetHandles = string.IsNullOrEmpty(searchContent)
                ? new List<BuiltinAssetHandle>(_allAssetHandles)
                : _allAssetHandles
#if UNITY_2021_3_OR_NEWER
                    .Where(handle => handle.AssetName.Contains(searchContent, StringComparison.OrdinalIgnoreCase))
#else
                    .Where(handle => handle.AssetName.ToUpperInvariant().Contains(searchContent.ToUpperInvariant()))
#endif
                    .ToList();

            if (_assetListView != null)
            {
                _assetListView.itemsSource = _filteredAssetHandles;
#if UNITY_2021_3_OR_NEWER
                _assetListView.fixedItemHeight = listItemHeight;
                _assetListView.Rebuild();
#else
                _assetListView.itemHeight = (int)listItemHeight;
                _assetListView.Refresh();
#endif
            }
        }

        private void OnResTypeChanged(ChangeEvent<Enum> evt)
        {
            UpdateAssetHandles();
        }

        private void OnSearchContentChanged(ChangeEvent<string> evt)
        {
            object selection = _assetListView.selectedItem;

            string searchContent = evt.newValue;
            _filteredAssetHandles = _allAssetHandles
#if UNITY_2021_3_OR_NEWER
                .Where(handle => handle.AssetName.Contains(searchContent, StringComparison.OrdinalIgnoreCase))
#else
                .Where(handle => handle.AssetName.ToUpperInvariant().Contains(searchContent.ToUpperInvariant()))
#endif
                .ToList();
            _assetListView.itemsSource = _filteredAssetHandles;

            int newSelectionIndex = -1;
            if (selection != null)
            {
                newSelectionIndex = _filteredAssetHandles.FindIndex(item => item == selection);
            }

#if UNITY_2021_3_OR_NEWER
            _assetListView.Rebuild();
#else
            _assetListView.Refresh();
#endif

            if (newSelectionIndex > -1)
            {
                _assetListView.selectedIndex = newSelectionIndex;
                _assetListView.ScrollToItem(newSelectionIndex);
            }
        }

        private VisualElement MakeItem()
        {
            UIResType _resType = (UIResType)_resTypeField.value;
            switch (_resType)
            {
                case UIResType.Icon:
                    return new BuiltinIconElement();

                case UIResType.StyleSheet:
                case UIResType.VisualTree:
                    return new BuiltinAssetElement();

                default:
                    throw new ArgumentOutOfRangeException(nameof(_resType), _resType, null);
            }
        }

        private void BindItem(VisualElement element, int index)
        {
            UIResType _resType = (UIResType)_resTypeField.value;
            switch (_resType)
            {
                case UIResType.Icon:
                    BuiltinIconElement iconElement = element as BuiltinIconElement;
                    BuiltinIconHandle iconHandle = (BuiltinIconHandle)_assetListView.itemsSource[index];
                    iconElement?.SetIconHandle(iconHandle, _allAssetHandles.IndexOf(iconHandle));
                    break;

                case UIResType.StyleSheet:
                case UIResType.VisualTree:
                    BuiltinAssetElement assetElement = element as BuiltinAssetElement;
                    BuiltinAssetHandle assetHandle = (BuiltinAssetHandle)_assetListView.itemsSource[index];
                    assetElement?.SetAssetHandle(assetHandle, _allAssetHandles.IndexOf(assetHandle));
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(_resType), _resType, null);
            }
        }


        #region ContextMenu

        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
            // Print All Asset Names
            menu.AddItem(new GUIContent("Print All Asset Names"), false, () =>
            {
                string[] allAssetNames = BuiltinUIResUtility.GetEditorAssetBundle().GetAllAssetNames();
                foreach (string assetName in allAssetNames)
                {
                    Debug.Log(assetName);
                }
            });

            // Source Code
            menu.AddItem(new GUIContent("Source Code"), false, () =>
            {
                Application.OpenURL("https://github.com/SolarianZ/UnityBuiltinUIResBrowser");
            });
        }

        #endregion
    }
}
