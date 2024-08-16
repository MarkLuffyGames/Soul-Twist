using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    public Inventory playerInventory;
    [SerializeField] Image[] icon = new Image[30];
    [SerializeField] Image mainHandIcon;
    [SerializeField] Image secondHandIcon;
    [SerializeField] GameObject inventoryUI;

    private InputActionMap playerActionMap;

    private void Start()
    {
        playerActionMap = InputSystem.actions.FindActionMap("Player");
    }

    public void ShowAndHideInventory()
    {
        if (inventoryUI.activeInHierarchy)
        {
            inventoryUI.SetActive(false);
            playerActionMap.Enable();
        }
        else
        {
            inventoryUI.SetActive(true);
            playerActionMap.Disable();
        }
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
