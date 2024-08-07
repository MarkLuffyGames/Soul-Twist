using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    public Interact interact;

    [SerializeField] GameObject collectButton; //Boton que se muestra al acercarte a un objeto que puedes recoger.
    [SerializeField] GameObject collectableItemGameObject; //Menu que muestra los objetos que se pueden recoger.

    [SerializeField] CollectableItemUI collectableItemUI; //Clase que controla el menu de seleccion de objetos recogibles.

    private void Start()
    {
        //Oculta los menus al iniciar el objeto.
        collectButton.SetActive(false);
        collectableItemGameObject.SetActive(false);
    }

    /// <summary>
    /// Muestra el boton de recoger objetos.
    /// </summary>
    public void ShowCollectButton()
    {
        collectButton.SetActive(true);
    }

    /// <summary>
    /// Oculta el boton de recoger objetos.
    /// </summary>
    public void HideCollectButton()
    {
        collectButton.SetActive(false);
    }

    /// <summary>
    /// Comprueba si el boton de recoger objetos esta activo.
    /// </summary>
    /// <returns></returns>
    public bool IsActiveCollectButton()
    {
        return collectButton.activeInHierarchy;
    }

    /// <summary>
    /// Muestra el menu de recoger objetos.
    /// </summary>
    /// <param name="collectable">Lista de objetos que se pueden recoger.</param>
    public void ShowCollectableItemUi(List<CollectableItem> collectable)
    {
        collectableItemGameObject.SetActive(true);
        UpdateCollectableItemUi(collectable);
    }

    /// <summary>
    /// Oculta el menu de recoger objetos.
    /// </summary>
    public void HideCollectableItemUi()
    {
        collectableItemGameObject.SetActive(false);
    }

    /// <summary>
    /// Retorna si el menu de recoger objetos esta activo o no.
    /// </summary>
    /// <returns></returns>
    public bool IsActiveCollectableItemUi()
    {
        return collectableItemGameObject.activeInHierarchy;
    }

    /// <summary>
    /// Actualiza los objetos disponibles para recoger segun la lista que se pase por parametro.
    /// </summary>
    /// <param name="collectable">Lista de objetos recogibles.</param>
    public void UpdateCollectableItemUi(List<CollectableItem> collectable)
    {
        collectableItemUI.UpdateContent(collectable);
    }

    /// <summary>
    /// Le envia a la clase de interaccion el objeto que se quiere recoger.
    /// </summary>
    /// <param name="item"></param>
    public void GetItem(CollectableItem item)
    {
        interact.TryGetItem(item);
    }
}
