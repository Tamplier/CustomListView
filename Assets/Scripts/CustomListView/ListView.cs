using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CustomListView
{
    public abstract class ListView<T, U> : MonoBehaviour where U : ListItemBase
    {
        [SerializeField]
        private int _spacing;
        [SerializeField]
        private int _maxElementsOnScreen;
        [SerializeField]
        private GameObject _elementPrefab;
        [SerializeField]
        private InputField _itemSelector;
        
        private IListViewContentProvider<T, U> _contentProvider;
        private ScrollRect _scrollRect;
        private List<float> _elementsPositions;
        private U[] _items;
        private float _prevScrollRectValue;
        private int _spacingHeight;
        private float _itemHeight;
        private int _prevCount;

        protected abstract IListViewContentProvider<T, U> GetContentProvider();

        private void Init()
        {
            _contentProvider = GetContentProvider();
            _contentProvider.OnItemRemoved += UpdateElements;
            _scrollRect = GetComponent<ScrollRect>();
            
            //Calc height of items
            _spacingHeight = (_spacing) * (_maxElementsOnScreen + 1);
            _itemHeight = (_scrollRect.GetComponent<RectTransform>().rect.height - _spacingHeight) / _maxElementsOnScreen;
            
            UpdateContentHeight();
            int itemsCount = _contentProvider.GetItemsCount();
            CalcElementsPositions(itemsCount, _itemHeight);
            InstantiateItems(Mathf.Min(_maxElementsOnScreen+2, itemsCount), _itemHeight);
            _scrollRect.verticalNormalizedPosition = 1;
            
            _prevScrollRectValue = 0;
        }

        private void UpdateContentHeight()
        {
            int itemsCount = _contentProvider.GetItemsCount();
            float contentHeight = itemsCount * _itemHeight + (itemsCount + 1) * _spacing;
            Vector2 newSize = _scrollRect.content.sizeDelta;
            newSize.y = contentHeight;
            _scrollRect.content.sizeDelta = newSize;
        }

        private void CalcElementsPositions(int count, float itemsHeight)
        {
            _elementsPositions = new List<float>(count);
            float currentY = -_spacing;
            for (int i = 0; i < count; i++)
            {
                _elementsPositions.Add(currentY);
                currentY -= itemsHeight + _spacing;
            }
        }

        private void InstantiateItems(int count, float height)
        {
            _items = new U[count];
            for (int i = 0; i < count; i++)
            {
                GameObject element = Instantiate(_elementPrefab, _scrollRect.content, false);
                RectTransform rt = element.GetComponent<RectTransform>();
                Vector2 newSize = rt.sizeDelta;
                newSize.y = height;
                rt.sizeDelta = newSize;
                U elementBase = element.GetComponent<U>();
                elementBase.OnHide += UpdateElements;
                _contentProvider.InitItem(i, elementBase);
                _items[i] = elementBase;
            }

            UpdateElementsPosition();
        }

        private void UpdateElements(int removedElement)
        {
            int minIndex = _items.Where(i => !i.CullState).Min(i => i.itemIndex);
            int maxIndex = _items.Where(i => !i.CullState).Max(i => i.itemIndex);
            int minIndexCull = _items.Min(i => i.itemIndex);
            int maxIndexCull = _items.Max(i => i.itemIndex);
            bool wasOnStart = false;
            bool wasOnEnd = false;

            if (minIndex == 0) wasOnStart = true;
            if (maxIndex == _contentProvider.GetLastIndex()) wasOnEnd = true;

            int newIndex = removedElement;
            if (removedElement == minIndex && maxIndexCull < _contentProvider.GetLastIndex())
            {
                if (!wasOnStart)
                {
                    removedElement = minIndexCull;
                    minIndexCull++;
                    maxIndexCull++;
                    newIndex = maxIndexCull;
                }
                else newIndex = minIndexCull;
            }
            else if (removedElement == maxIndex && minIndexCull > 0)
            {
                if (!wasOnEnd)
                {
                    removedElement = maxIndexCull;
                    minIndexCull--;
                    maxIndexCull--;
                    newIndex = minIndexCull;
                }
                else newIndex = maxIndexCull;
            }

            bool outOfRange = maxIndexCull > _contentProvider.GetLastIndex();

            U removedObject = _items.FirstOrDefault(i => i.itemIndex == removedElement);
            
            for (int i = 0; i < _items.Length; i++)
            {
                if(outOfRange)
                    _contentProvider.InitItem(_contentProvider.GetLastIndex() - i, _items[i]);
                else if (_items[i].itemIndex == removedElement)
                    _contentProvider.InitItem(newIndex, removedObject);
                else
                    _contentProvider.InitItem(_items[i].itemIndex, _items[i]);

            }

            UpdateElementsPosition();
            if(_prevCount != _contentProvider.GetItemsCount()) UpdateContentHeight();
            _prevCount = _contentProvider.GetItemsCount();
        }

        private void UpdateElementsPosition()
        {
            for (int i = 0; i < _items.Length; i++)
            {
                _items[i].SetYPosition(_elementsPositions[_items[i].itemIndex]);
            }
        }

        private void SetItem(int index)
        {
            int startIndex = Math.Max(0, index-1);
            int endIndex = startIndex + _maxElementsOnScreen + 2;
            if (endIndex > _contentProvider.GetLastIndex())
            {
                endIndex = _contentProvider.GetLastIndex();
                startIndex = endIndex - _maxElementsOnScreen - 2;
            }
            
            for (int i = 0; i < _items.Length; i++)
            {
                _contentProvider.InitItem(startIndex + i, _items[i]);
            }

            UpdateElementsPosition();
            _scrollRect.verticalNormalizedPosition = 1 - index / ((float)_contentProvider.GetLastIndex());
        }

        // Start is called before the first frame update
        void Start()
        {
            Init();
            if (_itemSelector != null)
            {
                _itemSelector.onEndEdit.AddListener((val) =>
                {
                    int index = Mathf.Clamp(Convert.ToInt32(val), 0, _contentProvider.GetLastIndex());
                    SetItem(index);
                    _itemSelector.text = index.ToString();
                });
            }
        }
    }
}
