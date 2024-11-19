using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Inventory : NetworkBehaviour
{
    [SerializeField] Item[] itemList;
    [SerializeField] GameObject mainHand;
    [SerializeField] GameObject secondHand;

    [SerializeField] Item[] slots = new Item[30];
    [SerializeField] Item mainHandSlot;
    [SerializeField] Item secondHandSlot;
    [SerializeField] UIInventory uIInventory;

    Combat combat;
    PlayerController playerController;

    public Item MainHandSlot => mainHandSlot;
    public Item SecondHandSlot => secondHandSlot;
    public Item[] Slots => slots;


    private InputAction menuAction;

    private void Start()
    {
        combat = GetComponent<Combat>();
        playerController = GetComponent<PlayerController>();
        if(playerController != null)
        {
            if (IsOwner)
            {
                uIInventory = FindFirstObjectByType<UIInventory>();
                uIInventory.playerInventory = this;


                menuAction = InputSystem.actions.FindAction("Menu");
                menuAction.started += MenuAction_started;
            }
            
        }


        if (mainHandSlot != null) SpawnMainWeaponRpc(mainHandSlot.ID);
        if (secondHandSlot != null) SpawnSecondWeaponRpc(secondHandSlot.ID);
    }

    private void MenuAction_started(InputAction.CallbackContext obj)
    {
        uIInventory.ShowAndHideInventory();
    }

    public void GetObject(int obtainedObjectId)
    {
        Item obtainedObject = GetObjectFromId(obtainedObjectId);

        if (mainHandSlot == null && obtainedObject.ObjectType == ObjectType.Sword)
        {
            SpawnMainWeaponRpc(obtainedObject.ID);
        }
        else if(secondHandSlot == null && obtainedObject.ObjectType == ObjectType.Shield)
        {
            SpawnSecondWeaponRpc(obtainedObject.ID);
        }
        else if (mainHandSlot != null && secondHandSlot == null && obtainedObject.ObjectType == ObjectType.Sword)
        {
            SpawnSecondWeaponRpc(obtainedObject.ID);
        }
        else
        {
            int primeraPosicionVacia = -1;
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == null)
                {
                    primeraPosicionVacia = i;
                    break;
                }
            }

            if(primeraPosicionVacia != -1)
            {
                slots[primeraPosicionVacia] = obtainedObject;
            }
        }

        if(IsOwner)uIInventory.UpdateInventory();
    }

    [Rpc(SendTo.Server)]
    private void SpawnMainWeaponRpc(int itemId)
    {
        SetMainWeaponRpc(itemId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void SetMainWeaponRpc(int itemId)
    {
        mainHandSlot = GetObjectFromId(itemId);

        var intance = Instantiate(mainHandSlot.ObjectPrefab,
                    mainHand.transform.position, mainHand.transform.rotation, mainHand.transform);
        intance.transform.localPosition += mainHandSlot.ObjectPosition;
        intance.transform.localRotation = Quaternion.Euler(mainHandSlot.ObjectRotation);

        combat.weaponCollider = intance.GetComponent<BoxCollider>();
        intance.GetComponent<Hit>()._healt = GetComponent<Healt>();
    }

    [Rpc(SendTo.Server)]
    private void SpawnSecondWeaponRpc(int itemId)
    {
        SetSecondWeaponRpc(itemId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SetSecondWeaponRpc(int itemId)
    {
        secondHandSlot = GetObjectFromId(itemId);

        var intance = Instantiate(secondHandSlot.ObjectPrefab,
            secondHand.transform.position, secondHand.transform.rotation, secondHand.transform);
        intance.transform.localPosition += secondHandSlot.ObjectPosition;
        intance.transform.localRotation = Quaternion.Euler(secondHandSlot.ObjectRotation);

        if (secondHandSlot.ObjectType == ObjectType.Sword)
        {
            combat.weaponCollider = intance.GetComponent<BoxCollider>();
            intance.GetComponent<Hit>()._healt = GetComponent<Healt>();
        }
        else if(secondHandSlot.ObjectType == ObjectType.Shield)
        {
            if (playerController != null) playerController.canBlock = true;
        }
    }

    private Item GetObjectFromId(int id)
    {
        foreach (Item item in itemList)
        {
            if(item.ID == id)
            {
                return item;
            }
        }
        Debug.LogError("El objeto no esta agragado a la lista en el inventario");
        return null;
    }

    public AttackType GetAttackType()
    {
        if(MainHandSlot != null)
        {
            return MainHandSlot.AttackType;
        }

        return AttackType.Unarmed;
    }

    public int GetAttackNumber()
    {
        if (MainHandSlot != null)
        {
            return MainHandSlot.AttackNumber;
        }

        return 3;
    }
}
