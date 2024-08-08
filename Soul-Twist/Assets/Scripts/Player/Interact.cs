using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class Interact : NetworkBehaviour
{
    //Lista de objetos que el jugador puede recoger.
    [SerializeField] private List<CollectableItem> collectable = new List<CollectableItem>();
    [SerializeField] private UI uI;// Referencia a la UI de jugador.

    private InputAction pickUpAction;

    private void Start()
    {
        if (!IsOwner) return;
        pickUpAction = InputSystem.actions.FindAction("Interact");
        pickUpAction.started += PickUpAction_started;
        uI = FindFirstObjectByType<UI>();
        uI.interact = this;
    }

    /// <summary>
    /// Cuando se preciona el boton de interactuar, si esta disponible activa el menu de recoger objetos.
    /// </summary>
    /// <param name="obj"></param>
    private void PickUpAction_started(InputAction.CallbackContext obj)
    {
        if (uI.IsActiveCollectButton())
        {
            uI.HideCollectButton();
            uI.ShowCollectableItemUi(collectable);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return;
        // Si el objeto que entra en el trigger es un objeto recogible lo agraga ala lista y actualiza el menu
        if (other.TryGetComponent<CollectableItem>(out CollectableItem collectableItem))
        {
            collectable.Add(collectableItem);
            if (uI.IsActiveCollectableItemUi())
            {
                uI.UpdateCollectableItemUi(collectable);
            }
            else
            {
                uI.ShowCollectButton();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsOwner) return;
        // Si el objeto que sale del trigger es uno recogible lo elimina de la lista actualiza la misma.
        if (other.TryGetComponent(out CollectableItem collectableItem))
        {
            RemoveObjectCollectable(collectableItem);
        }
    }
    public void RemoveObjectCollectable(CollectableItem collectableItem)
    {
        collectable.Remove(collectableItem);
        if (collectable.Count == 0)
        {
            uI.HideCollectButton();
            uI.HideCollectableItemUi();
        }
        else
        {
            uI.UpdateCollectableItemUi(collectable);
        }
    }


    public void TryGetItem(CollectableItem item)
    {
        CollectItemRpc(OwnerClientId, item.NetworkObject);
    }

    [Rpc(SendTo.Server)]
    void CollectItemRpc(ulong clientId, NetworkObjectReference networkObjectReference)
    {
        networkObjectReference.TryGet(out NetworkObject NetworkObject);
        var itemObtained = NetworkObject.GetComponent<CollectableItem>();

        // Lógica para agregar el objeto al inventario del jugador
        var inventory = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<Inventory>();

        if (!itemObtained.IsCollected.Value)
        {
            itemObtained.IsCollected.Value = true;
            inventory.GetObject(itemObtained.item.ID);

            // Destruir o desactivar el objeto en la escena
            RemoveItemFromAllClientsRpc(networkObjectReference);
            itemObtained.NetworkObject.Despawn();
        }
        
    }

    [Rpc(SendTo.ClientsAndHost)]
    void RemoveItemFromAllClientsRpc(NetworkObjectReference networkObjectReference)
    {
        networkObjectReference.TryGet(out NetworkObject NetworkObject);
        var itemObtained = NetworkObject.GetComponent<CollectableItem>();

        NetworkManager.Singleton.LocalClient.
            PlayerObject.GetComponent<Interact>().RemoveObjectCollectable(itemObtained);
    }
}
