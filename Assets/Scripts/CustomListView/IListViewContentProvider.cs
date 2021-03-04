using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomListView
{
    public interface IListViewContentProvider<T, U>
    {
        event Action<int> OnItemRemoved;  
        int GetItemsCount();
        int GetLastIndex();
        void InitItem(int i, U view);
        void RemoveItem(int i);
        void UpdateItem(int i, T newValue);
    }
}
