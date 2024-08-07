using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectableItemIconUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }
    public void SetData(CollectableItem item ,UI ui)
    {
        button.onClick.AddListener(() =>
        {
            ui.GetItem(item);
        });
        itemName.text = item.name;
    }
}
