using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using CustomListView;

public class ListItem : ListItemBase
{
    [SerializeField]
    private Button _button;
    [SerializeField]
    private InputField _inputField;
    private event Action<int> OnDelButtonClicked;
    private event Action<int, string> OnValueChanged; 

    private void Awake()
    {
        base.Awake();
        _button.onClick.AddListener(() =>
        {
            OnDelButtonClicked?.Invoke(itemIndex);
        });
        
        _inputField.onEndEdit.AddListener((newValue) =>
        {
            OnValueChanged?.Invoke(itemIndex, newValue);
        });
    }

    public void SetItem(int index, string item, Action<int> delAction, Action<int, string> updateAction)
    {
        itemIndex = index;
        _inputField.text = item;
        OnDelButtonClicked = null;
        OnDelButtonClicked += delAction;
        OnValueChanged = null;
        OnValueChanged += updateAction;
    }
}
