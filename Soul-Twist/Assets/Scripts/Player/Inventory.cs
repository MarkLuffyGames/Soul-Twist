using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;
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

    Attack atacar;
    PlayerController playerController;

    public Item MainHandSlot => mainHandSlot;
    public Item SecondHandSlot => secondHandSlot;
    public Item[] Slots => slots;

    private void Start()
    {
        atacar = GetComponent<Attack>();
        playerController = GetComponent<PlayerController>();
        if(playerController != null)
        {
            if (IsOwner)
            {
                uIInventory = FindFirstObjectByType<UIInventory>();
                uIInventory.playerInventory = this;
            }
            
        }
        

        if (mainHandSlot != null)SetMainWeapon(mainHandSlot);
        if(secondHandSlot != null)SetSecondWeapon(secondHandSlot);
    }

    public void GetObject(int obtainedObjectId)
    {
        Item obtainedObject = GetObjectFromId(obtainedObjectId);

        if (mainHandSlot == null && obtainedObject.ObjectType == ObjectType.Sword)
        {
            SetMainWeapon(obtainedObject);
        }
        else if(secondHandSlot == null && obtainedObject.ObjectType == ObjectType.Shield)
        {
            SetSecondWeapon(obtainedObject);
        }
        else if (mainHandSlot != null && secondHandSlot == null && obtainedObject.ObjectType == ObjectType.Sword)
        {
            SetSecondWeapon(obtainedObject);
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

    public void SetMainWeapon(Item weapon)
    {
        mainHandSlot = weapon;
        var intance = Instantiate(weapon.ObjectPrefab, 
            mainHand.transform.position, mainHand.transform.rotation, mainHand.transform);
        intance.transform.localPosition += weapon.ObjectPosition;
        intance.transform.localRotation = Quaternion.Euler(weapon.ObjectRotation);

        //atacar._collider = intance.GetComponent<BoxCollider>();

        if(playerController != null)playerController.canAttack = true;
    }

    public void SetSecondWeapon(Item weapon)
    {
        secondHandSlot = weapon;
        var intance = Instantiate(weapon.ObjectPrefab,
            secondHand.transform.position, secondHand.transform.rotation, secondHand.transform);
        intance.transform.localPosition += weapon.ObjectPosition;

        if (weapon.ObjectType == ObjectType.Sword)
        {
            //atacar._collider = intance.GetComponent<BoxCollider>();
        }

        //if (playerController != null) playerController.canDefend = true;
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
}
