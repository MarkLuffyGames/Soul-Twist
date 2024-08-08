using Unity.Netcode;
using UnityEngine;

public class CollectableItem : NetworkBehaviour
{
    public NetworkVariable<bool> IsCollected = new NetworkVariable<bool>(false);
    public Item item;
}
