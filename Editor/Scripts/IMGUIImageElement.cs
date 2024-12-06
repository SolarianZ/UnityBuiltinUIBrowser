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
            style.justifyContent = Justify.Center;

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
            if (float.IsNaN(containerSize.x) || float.IsNaN(containerSize.y))
                return;

            if (!Image || Image.height == 0 || containerSize.y == 0)
                return;

            float imageWidth = Image.width;
            float imageHeight = Image.height;
            float containerWidth = containerSize.x;
            float containerHeight = containerSize.y;
            if (imageWidth <= containerWidth && imageHeight <= containerHeight)
            {
                _imageDrawer.style.minWidth = _imageDrawer.style.maxWidth = imageWidth;
                _imageDrawer.style.minHeight = _imageDrawer.style.maxHeight = imageHeight;
                return;
            }

            float imageAspect = imageWidth / imageHeight;
            float containerAspect = containerWidth / containerHeight;
            if (imageAspect >= containerAspect)
            {
                // scale image width to container width
                imageWidth = containerWidth;
                imageHeight = imageWidth / imageAspect;
            }
            else
            {
                // scale image height to container height
                imageHeight = containerHeight;
                imageWidth = imageHeight * imageAspect;
            }

            _imageDrawer.style.minWidth = _imageDrawer.style.maxWidth = imageWidth;
            _imageDrawer.style.minHeight = _imageDrawer.style.maxHeight = imageHeight;
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