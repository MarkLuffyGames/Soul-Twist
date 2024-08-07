using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Create Item")]
public class Item : ScriptableObject
{
    [SerializeField] private new string name;
    [SerializeField] private Sprite icon;

    public string Name => name;
    public Sprite Icon => icon;
}
