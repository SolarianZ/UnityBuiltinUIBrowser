using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UObject = UnityEngine.Object;

namespace GBG.EditorIconsOverview.Editor
{
    public class BuiltinAssetElement : VisualElement
    {
        public const float MinHeight = 30;

        public Image Image { get; }
        public Label NameLabel { get; }

        public BuiltinAssetHandle AssetHandle { get; private set; }

        private UObject _asset;


        public BuiltinAssetElement()
        {
            style.flexDirection = FlexDirection.Row;
            style.paddingLeft = 2;
            style.paddingRight = 2;
            style.paddingTop = 1;
            style.paddingBottom = 1;
            style.minHeight = MinHeight;

#if UNITY_2021_3_OR_NEWER
            RegisterCallback<ClickEvent>(OnClick);
#else
            RegisterCallback<PointerDownEvent>(OnClick);
#endif
            RegisterCallback<ContextClickEvent>(OnContextClick);


            #region Image

            VisualElement imageContainer = new VisualElement
            {
                style =
                {
                    width = 100,
                    minWidth = 100,
                    maxWidth = 100,
                    flexShrink = 0,
                    alignItems = Align.Center,
                    justifyContent = Justify.Center,
                    paddingLeft = 2,
                    paddingRight = 2,
                    paddingTop = 2,
                    paddingBottom = 2,
                    overflow = Overflow.Hidden,
                }
            };
            Add(imageContainer);

            // Image
            Image = new Image();
            imageContainer.Add(Image);

            #endregion


            #region Labels

            VisualElement labelContainer = new VisualElement
            {
                style =
                {
                    flexGrow = 1,
                    flexShrink = 0,
                    paddingLeft = 2,
                    paddingRight = 2,
                }
            };
            Add(labelContainer);

            // Name Label
            NameLabel = new Label
            {
                //selection =
                //{
                //    isSelectable = true,
                //},
                style =
                {
                    flexGrow = 1,
                    flexShrink = 0,
                    unityTextAlign = TextAnchor.MiddleLeft,
                    fontSize = 15,
                }
            };
            labelContainer.Add(NameLabel);

            #endregion

        }

        public void SetAssetHandle(BuiltinAssetHandle assetHandle)
        {
            AssetHandle = assetHandle;
            _asset = AssetHandle.LoadAsset();

            Image.image = AssetPreview.GetMiniThumbnail(_asset);
            NameLabel.text = AssetHandle.AssetName;
        }


#if UNITY_2021_3_OR_NEWER
        private void OnClick(ClickEvent evt)
#else
        private void OnClick(PointerDownEvent evt)
#endif
        {
            if (evt.clickCount == 2)
            {
                AssetHandle.Inspect();
            }
        } 

        private void OnContextClick(ContextClickEvent evt)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Copy Loading Code"), false, CopyAssetLoadingCodeToClipboard);
            menu.AddItem(new GUIContent("Copy Name"), false, CopyAssetNameToClipboard);
            menu.AddItem(new GUIContent("Copy File ID"), false, CopyAssetFileIdToClipboard);
            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Inspect"), false, AssetHandle.Inspect);
            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Save as"), false, AssetHandle.SaveAs);

            menu.ShowAsContext();
        }

        public void CopyAssetLoadingCodeToClipboard()
        {
            GUIUtility.systemCopyBuffer = null;
            if (!_asset)
            {
                Debug.LogError($"Cannot load built in asset '{AssetHandle.AssetName}'.");
                return;
            }

            GUIUtility.systemCopyBuffer = AssetHandle.GetLoadingCode(_asset.GetType().Name);
        }

        public void CopyAssetNameToClipboard()
        {
            GUIUtility.systemCopyBuffer = AssetHandle.AssetName;
        }

        public void CopyAssetFileIdToClipboard()
        {
            GUIUtility.systemCopyBuffer = null;
            if (!_asset)
            {
                Debug.LogError($"Cannot load built in asset '{AssetHandle.AssetName}'.");
                return;
            }

            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(_asset, out string guid, out long localId);
            GUIUtility.systemCopyBuffer = localId.ToString();
        }
    }
}