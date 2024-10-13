using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.EditorIconsOverview.Editor
{
    public class BuiltinIconElement : VisualElement
    {
        public const float MinHeight = 40;

        public Image Image { get; }
        public Label NameLabel { get; }
        public Label SizeLabel { get; }

        public BuiltinIconHandle IconHandle { get; private set; }

        private readonly VisualElement _imageContainer;


        public BuiltinIconElement()
        {
            style.flexDirection = FlexDirection.Row;
            style.paddingLeft = 2;
            style.paddingRight = 2;
            style.paddingTop = 1;
            style.paddingBottom = 1;
            style.minHeight = MinHeight;

            RegisterCallback<ClickEvent>(OnClick);
            RegisterCallback<ContextClickEvent>(OnContextClick);


            #region Image

            _imageContainer = new VisualElement
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
            Add(_imageContainer);

            // Image
            Image = new Image();
            _imageContainer.Add(Image);

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

        }

        public void SetIconHandle(BuiltinIconHandle iconHandle)
        {
            IconHandle = iconHandle;
            Texture texture = IconHandle.LoadTexture();
            Image.image = texture;
            NameLabel.text = IconHandle.RawIconName;
            if (texture)
            {
                SizeLabel.text = $"{texture.width}x{texture.height}";
            }
            else
            {
                SizeLabel.enableRichText = true;
                SizeLabel.text = "<color=red>INVALID TEXTURE</color>";
            }

            // TODO: Skin Bg
            //_imageContainer.style.backgroundColor = 
        }


        private void OnClick(ClickEvent evt)
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
            menu.AddItem(new GUIContent("Copy Name (No d_ Prefix)",
                "Unity will automatically append d_ prefix based on the Editor theme."),
                false, CopyIconNameToClipboard);
            menu.AddItem(new GUIContent("Copy Raw Name"), false, CopyRawIconNameToClipboard);
            menu.AddItem(new GUIContent("Copy File ID"), false, CopyIconFileIdToClipboard);
            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Inspect"), false, IconHandle.Inspect);

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
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(Image.image, out string guid, out long localId);
            GUIUtility.systemCopyBuffer = localId.ToString();
        }
    }
}