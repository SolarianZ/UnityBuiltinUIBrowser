using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.EditorIconsOverview.Editor
{
    public class BuiltinIconElement : VisualElement
    {
        public const float MinHeight = 40;

#if UNITY_2021_3_OR_NEWER
        public Image Image { get; }
#else
        internal IMGUIImageElement Image { get; }
#endif
        public Label NameLabel { get; }
        public Label SizeLabel { get; }
        public Label IndexLabel { get; }

        public BuiltinIconHandle IconHandle { get; private set; }


        public BuiltinIconElement()
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
#if UNITY_2021_3_OR_NEWER
            Image = new Image();
#else
            Image = new IMGUIImageElement();
#endif
            imageContainer.Add(Image);

            #endregion


            #region Labels

            VisualElement labelContainer = new VisualElement
            {
                style =
                {
                    flexGrow = 1,
                    flexShrink = 1,
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

            // Size Label
            SizeLabel = new Label
            {
                style =
                {
                    flexGrow = 1,
                    flexShrink = 0,
                    minHeight = 11,
                    maxHeight = 14,
                    unityTextAlign = TextAnchor.MiddleLeft,
                    unityFontStyleAndWeight = FontStyle.Italic,
                    fontSize = 11,
                }
            };
            labelContainer.Add(SizeLabel);

            #endregion


            #region Index

            // Index Label
            IndexLabel = new Label
            {
                style =
                {
                    flexGrow = 0,
                    flexShrink = 0,
                    unityTextAlign = TextAnchor.LowerRight,
                    fontSize = 11,
                }
            };
            Add(IndexLabel);

            #endregion
        }

        public void SetIconHandle(BuiltinIconHandle iconHandle, int index)
        {
            IconHandle = iconHandle;
            Texture texture = IconHandle.LoadTexture();
#if UNITY_2021_3_OR_NEWER
            Image.image = texture;
#else
            Image.Image = texture;
#endif
            NameLabel.text = IconHandle.RawIconName;
            if (texture)
            {
                SizeLabel.text = $"{texture.width}x{texture.height}";
            }
            else
            {
#if UNITY_2021_3_OR_NEWER
                SizeLabel.enableRichText = true;
                SizeLabel.text = "<color=red>INVALID TEXTURE</color>";
#else
                SizeLabel.text = "INVALID TEXTURE";
#endif
            }

            IndexLabel.text = (index + 1).ToString();
        }


#if UNITY_2021_3_OR_NEWER
        private void OnClick(ClickEvent evt)
#else
        private void OnClick(PointerDownEvent evt)
#endif
        {
            if (evt.clickCount == 2)
            {
                IconHandle.Inspect();
            }
        }

        private void OnContextClick(ContextClickEvent evt)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Copy IconContent Code"), false, CopyIconContentCodeToClipboard);
            menu.AddItem(new GUIContent("Copy Name without 'd_' Prefix",
                "Unity will automatically append 'd_' prefix based on the Editor theme."),
                false, CopyIconNameToClipboard);
            menu.AddItem(new GUIContent("Copy Name"), false, CopyRawIconNameToClipboard);
            menu.AddItem(new GUIContent("Copy File ID"), false, CopyIconFileIdToClipboard);
            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Inspect"), false, IconHandle.Inspect);
            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Save as"), false, IconHandle.SaveAs);

            menu.ShowAsContext();
        }

        public void CopyIconContentCodeToClipboard()
        {
            GUIUtility.systemCopyBuffer = IconHandle.GetIconContentCode();
        }

        public void CopyIconNameToClipboard()
        {
            GUIUtility.systemCopyBuffer = IconHandle.GetIconName();
        }

        public void CopyRawIconNameToClipboard()
        {
            GUIUtility.systemCopyBuffer = IconHandle.RawIconName;
        }

        public void CopyIconFileIdToClipboard()
        {
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(IconHandle.LoadAsset(), out string guid, out long localId);
            GUIUtility.systemCopyBuffer = localId.ToString();
        }
    }
}