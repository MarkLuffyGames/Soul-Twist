using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    public Inventory playerInventory;
    [SerializeField] Image[] icon = new Image[30];
    [SerializeField] Image mainHandIcon;
    [SerializeField] Image secondHandIcon;
    [SerializeField] GameObject inventoryUI;

    private void Start()
    {
    }

    public void UpdateInventory()
    {
        for (int i = 0; i < icon.Length; i++)
        {
            if (playerInventory.Slots[i] != null)
            {
                icon[i].sprite = playerInventory.Slots[i].Icon;
            }
        }

        if (playerInventory.MainHandSlot != null)
        {
            mainHandIcon.sprite = playerInventory.MainHandSlot.Icon;
        }
        if (playerInventory.SecondHandSlot != null)
        {
            secondHandIcon.sprite = playerInventory.SecondHandSlot.Icon;
        }
       

    }
}
