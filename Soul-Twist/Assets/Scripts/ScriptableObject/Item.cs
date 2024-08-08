using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Create Item")]
public class Item : ScriptableObject
{
    [SerializeField] private int id;
    [SerializeField] private string objectName;
    [TextArea][SerializeField] private string description;
    [SerializeField] private GameObject objectPrefab;
    [SerializeField] private Sprite icon;
    [SerializeField] private Vector3 objectPosition;
    [SerializeField] private Vector3 objectRotation;
    [SerializeField] private ObjectType objectType;
    [SerializeField] private AttackType attackType;

    //Stats
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    [SerializeField] private int spAttack;
    [SerializeField] private int spDefense;

    public int ID => id;
    public string ObjectName => objectName;
    public string Description => description;
    public GameObject ObjectPrefab => objectPrefab;
    public Sprite Icon => icon;
    public Vector3 ObjectPosition => objectPosition;
    public Vector3 ObjectRotation => objectRotation;
    public ObjectType ObjectType => objectType;
    public AttackType AttackType => attackType;
    public int Attack => attack;
    public int Defense => defense;
    public int SpAttack => spAttack;
    public int SpDefense => spDefense;

}

public enum ObjectType
{
    Sword, Shield
}

public enum AttackType
{
    None, Sword_1H, Sword_2H
}

