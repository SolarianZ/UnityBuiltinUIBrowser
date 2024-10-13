using UnityEngine;
using UnityEngine.UIElements;

namespace GBG.EditorIconsOverview.Editor
{
    internal class IMGUIImageElement : VisualElement
    {
        public Texture Image
        {
            get => _image;
            set
            {
                _image = value;
                _imageDrawerSizeDirty = true;
            }
        }
        private Texture _image;
        private bool _imageDrawerSizeDirty = true;

        private readonly IMGUIContainer _imageDrawer;

        public IMGUIImageElement()
        {
            style.flexGrow = 1;
            style.flexShrink = 0;
            style.alignSelf = Align.Stretch;

            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);

            _imageDrawer = new IMGUIContainer(DrawImage)
            {
                style =
                {
                    alignSelf = Align.Center,
                }
            };
            Add(_imageDrawer);
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            CalcImageDrawerSize();
        }

        private void CalcImageDrawerSize()
        {
            Vector2 containerSize = localBound.size;
            if (!Image || Image.height == 0 || containerSize.y == 0)
                return;

            float imageWidth = Image.width;
            float imageHeight = Image.height;
            float aspect = imageWidth / imageHeight;
            float maxWidth = Mathf.Min(imageWidth, containerSize.x);
            float maxHeight = Mathf.Min(imageHeight, containerSize.y);
            float tempAspect = maxWidth / maxHeight;
            if (tempAspect > 1 + 1E-3F)
            {
                maxWidth = aspect * maxHeight;
            }
            else if (tempAspect < 1 - 1E-3F)
            {
                maxHeight = maxWidth / aspect;
            }
            _imageDrawer.style.minWidth = _imageDrawer.style.maxWidth = maxWidth;
            _imageDrawer.style.minHeight = _imageDrawer.style.maxHeight = maxHeight;
        }

        private void DrawImage()
        {
            if (!Image)
                return;

            if (_imageDrawerSizeDirty)
            {
                CalcImageDrawerSize();
                _imageDrawerSizeDirty = false;
            }

            GUI.DrawTexture(_imageDrawer.contentRect, Image);
        }
    }
}