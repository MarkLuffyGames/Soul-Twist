using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Interact interact;

    [SerializeField] GameObject collectButton; //Boton que se muestra al acercarte a un objeto que puedes recoger.
    [SerializeField] Image collectButtonChargue; //Imagen de barra de carga del boton de recoger.
    [SerializeField] GameObject collectableItemGameObject; //Menu que muestra los objetos que se pueden recoger.
    [SerializeField] CollectableItemUI collectableItemUI; //Clase que controla el menu de seleccion de objetos recogibles.

    [SerializeField] GameObject openCloseButton;

    private void Start()
    {
        //Oculta los menus al iniciar el objeto.
        collectButton.SetActive(false);
        openCloseButton.SetActive(false);
        collectableItemGameObject.SetActive(false);

        InvokeRepeating("FrameCount", 1, 1);
    }

    public TextMeshProUGUI fps;
    float frameRate;
    float frameCount;
    void FrameCount()
    {
        frameRate = Time.frameCount - frameCount;
        frameCount = Time.frameCount;
        fps.text = frameRate.ToString();
    }

    /// <summary>
    /// Muestra el boton de abrir o cerrar.
    /// </summary>
    public void ShowOpenCloseButton()
    {
        openCloseButton.SetActive(true);
    }

    /// <summary>
    /// Oculta el boton de abrir o cerrar.
    /// </summary>
    public void HideOpenCloseButton()
    {
        openCloseButton.SetActive(false);
    }

    /// <summary>
    /// Comprueba si el boton de abrir y cerrar esta activo.
    /// </summary>
    /// <returns></returns>
    public bool IsActiveOpenCloseButton()
    {
        return openCloseButton.activeInHierarchy;
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

    public bool isCanceled = false;
    public IEnumerator FillBar()
    {
        collectButtonChargue.fillAmount = 0;
        while (!isCanceled)
        {
            collectButtonChargue.fillAmount += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        isCanceled = false;
        collectButtonChargue.fillAmount = 0;
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
