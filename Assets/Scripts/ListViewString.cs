using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomListView;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ListViewString : ListView<string, ListItem>
{
    // Start is called before the first frame update
    protected override IListViewContentProvider<string, ListItem> GetContentProvider()
    {
        return new MyContentProvider(10000);
    }
}
