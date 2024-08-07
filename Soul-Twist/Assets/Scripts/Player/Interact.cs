using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interact : MonoBehaviour
{
    //Lista de objetos que el jugador puede recoger.
    [SerializeField] private List<CollectableItem> collectable = new List<CollectableItem>();
    [SerializeField] private UI uI;// Referencia a la UI de jugador.

    [SerializeField] CollectableItem itemObtained;

    private InputAction pickUpAction;

    private void Start()
    {
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
        // Si el objeto que entra en el trigger es un objeto recogible lo agraga ala lista y actualiza el menu
        if(other.TryGetComponent<CollectableItem>(out CollectableItem collectableItem))
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
        // Si el objeto que sale del trigger es uno recogible lo elimina de la lista actualiza la misma.
        if(other.TryGetComponent<CollectableItem>(out CollectableItem collectibleItem))
        {
            collectable.Remove(collectibleItem);
            if(collectable.Count == 0)
            {
                uI.HideCollectButton();
                uI.HideCollectableItemUi();
            }
            else
            {
                uI.UpdateCollectableItemUi(collectable);
            }
        }
    }

    public void TryGetItem(CollectableItem item)
    {
        itemObtained = item;
        //Comprobar
    }
}
