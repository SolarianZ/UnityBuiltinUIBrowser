using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.EditorIconsOverview.Editor
{
    public class IconElement : VisualElement
    {
        public Image Image { get; }
        public Label NameLabel { get; }
        public Label SizeLabel { get; }

        public IconHandle IconHandle { get; private set; }

        private readonly VisualElement _imageContainer;
        private bool _isImageBgInProSkin;


        public IconElement()
        {
            style.flexDirection = FlexDirection.Row;


            #region Image

            _imageContainer = new VisualElement
            {
                name = "icon-element__image-container",
                style =
                {
                    width = 100,
                    minWidth = 100,
                    maxWidth = 100,
                    flexShrink = 0,
                    overflow = Overflow.Hidden,
                }
            };
            SetImageBackgroundSkin(EditorGUIUtility.isProSkin);
            Add(_imageContainer);

            // Image
            Image = new Image
            {
                name = "icon-element__image",
            };
            _imageContainer.Add(Image);

            #endregion


            #region Labels

            VisualElement labelContainer = new VisualElement
            {
                name = "icon-element__label-container",
                style =
                {
                    flexGrow = 1,
                }
            };
            Add(labelContainer);

            // Name Label
            NameLabel = new Label
            {
                name = "icon-element__name-label",
            };
            labelContainer.Add(NameLabel);

            // Size Label

            VisualElement sizeLabelContainer = new VisualElement
            {
                name = "icon-element__label-container__size",
                style =
                {
                    flexGrow = 1,
                    flexDirection = FlexDirection.Row,
                }
            };
            labelContainer.Add(sizeLabelContainer);

            SizeLabel = new Label
            {
                name = "icon-element__size-label",
                style =
                {
                    flexGrow = 1,
                }
            };
            sizeLabelContainer.Add(SizeLabel);

            Button switchSizeButton = new Button()
            {
                text = "2x",
                name = "icon-element__button__switch-size",
            };
            sizeLabelContainer.Add(switchSizeButton);

            #endregion


            #region Buttons

            VisualElement buttonContainer = new VisualElement
            {
                name = "icon-element__button-container",
                style =
                {
                    flexGrow = 1,
                }
            };
            Add(buttonContainer);

            // Copy Icon Name Button
            Button copyIconNameButton = new Button(CopyIconNameToClipboard)
            {
                text = "N",
            };
            buttonContainer.Add(copyIconNameButton);

            // Copy IconContent Code Button
            Button copyIconContentCodeButton = new Button(CopyIconContentCodeToClipboard)
            {
                text = "C",
            };
            buttonContainer.Add(copyIconContentCodeButton);

            #endregion
        }

        public void SetIconHandle(IconHandle iconHandle)
        {
            IconHandle = iconHandle;
        }

        public void SwitchImageBackgroundSkin()
        {
            SetImageBackgroundSkin(!_isImageBgInProSkin);
        }

        public void SetImageBackgroundSkin(bool isProSkin)
        {
            _isImageBgInProSkin = isProSkin;

            // TODO: 切换 _imageContainer Skin
            Debug.LogError("TODO: 切换 _imageContainer Skin");
        }

        public void CopyIconNameToClipboard()
        {
            //GUIUtility.systemCopyBuffer = 
        }

        public void CopyIconContentCodeToClipboard()
        {
            //EditorGUIUtility.IconContent("", "|");
            //GUIUtility.systemCopyBuffer = 
        }
    }
}