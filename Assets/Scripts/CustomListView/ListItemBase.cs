using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CustomListView
{
    public abstract class ListItemBase : MonoBehaviour
    {
        public int itemIndex { get; protected set; }
        public event Action<int> OnHide;
        public bool CullState;
        private RectTransform _rectTransform;

        protected void Awake()
        {
            CullState = false;
            _rectTransform = GetComponent<RectTransform>();
            GetComponent<MaskableGraphic>().onCullStateChanged.AddListener((state) =>
            {
                if(state) OnHide?.Invoke(itemIndex);
                CullState = state;
            });
        }

        private void OnBecameInvisible()
        {
            
        }

        public void SetYPosition(float y)
        {
            Vector2 newPos = _rectTransform.anchoredPosition;
            newPos.y = y;
            _rectTransform.anchoredPosition = newPos;
        }
    }
}
