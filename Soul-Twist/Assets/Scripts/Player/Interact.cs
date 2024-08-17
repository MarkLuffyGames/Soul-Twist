using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using UnityEngine.InputSystem.Interactions;
using Unity.VisualScripting;

public class Interact : NetworkBehaviour
{
    //Lista de objetos que el jugador puede recoger.
    [SerializeField] private List<CollectableItem> collectable = new List<CollectableItem>();
    [SerializeField] private UI uI;// Referencia a la UI de jugador.
    [SerializeField] private Door door;

    private Animator _animator;

    private InputAction interactAction;

    private void Start()
    {
        if (!IsOwner) return;
        interactAction = InputSystem.actions.FindAction("Interact");
        interactAction.started += context => 
        {
            if (context.interaction is HoldInteraction)
            {
                StartCoroutine(uI.FillBar());
            }
        };

        interactAction.canceled += context =>
        {
            if (context.interaction is HoldInteraction)
            {
                uI.isCanceled = true;
            }
        };

        interactAction.performed += InteractAction_performed;
        uI = FindFirstObjectByType<UI>();
        uI.interact = this;
        _animator = GetComponent<Animator>();
    }

    private void InteractAction_performed(InputAction.CallbackContext Context)
    {
        if (Context.interaction is PressInteraction)
        {
            OpenDoor();
        }
        else if(Context.interaction is HoldInteraction)
        {
            PickUpAction();
        }
    }

    /// <summary>
    /// Cuando se preciona el boton de interactuar, intenta abrir o cerrar la puerta.
    /// </summary>
    private void OpenDoor()
    {
        if (uI.IsActiveOpenCloseButton())
        {
            door.ToggleDoorServerRpc();
            door = null;
            uI.HideOpenCloseButton();
        }
    }

    /// <summary>
    /// Cuando se mantiene el boton de interactuar, si esta disponible activa el menu de recoger objetos.
    /// </summary>
    private void PickUpAction()
    {
        if (uI.IsActiveCollectButton())
        {
            uI.HideCollectButton();
            uI.ShowCollectableItemUi(collectable);
            //Animacion de recoger objetos.
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return;
        // Si el objeto que entra en el trigger es un objeto recogible lo agraga ala lista y actualiza el menu
        if (other.TryGetComponent(out CollectableItem collectableItem))
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
        else if(other.TryGetComponent(out Door door))
        {
            this.door = door;
            uI.ShowOpenCloseButton();
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
        else if(other.TryGetComponent(out Door door))
        {
            this.door = null;
            uI.HideOpenCloseButton();
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
