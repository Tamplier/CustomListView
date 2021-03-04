using System;
using System.Collections;
using System.Collections.Generic;
using CustomListView;
using UnityEngine;

public class MyContentProvider : IListViewContentProvider<string, ListItem>
{
    public event Action<int> OnItemRemoved;
    private List<string> _stringList;
    public MyContentProvider(int count)
    {
        _stringList = new List<string>(count);
        for (int i = 0; i < count; i++)
        {
            _stringList.Add((i + 1).ToString());
        }
    }

    public int GetItemsCount()
    {
        return _stringList.Count;
    }

    public int GetLastIndex()
    {
        return GetItemsCount() - 1;
    }

    public void InitItem(int i, ListItem view)
    {
        view.SetItem(i, _stringList[i], RemoveItem, UpdateItem);
    }
    

    public void RemoveItem(int i)
    {
        _stringList.RemoveAt(i);
        OnItemRemoved?.Invoke(i);
    }

    public void UpdateItem(int i, string newValue)
    {
        _stringList[i] = newValue;
    }
}
