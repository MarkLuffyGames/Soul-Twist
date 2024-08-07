using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CollectableItemUI : MonoBehaviour
{
    public UI ui;

    [SerializeField] private List<GameObject> itemsList = new List<GameObject>();
    [SerializeField] private RectTransform viewport;
    [SerializeField] private RectTransform content;

    [SerializeField] private GameObject itemTemplate;

    [SerializeField] private int selectedButton = -1;

    private InputAction uiUpAction;
    private InputAction uiDownAction;

    private void Awake()
    {
        itemTemplate.SetActive(false);
    }

    private void Start()
    {
        uiUpAction = InputSystem.actions.FindAction("Up");
        uiDownAction = InputSystem.actions.FindAction("Down");

        uiUpAction.started += UiUpAction_started;
        uiDownAction.started += UiDownAction_started;
    }

    private void UiUpAction_started(InputAction.CallbackContext obj)
    {
        if (selectedButton > 0) selectedButton--;

        itemsList[selectedButton].GetComponent<Button>().Select();

        AdjustContentPosition();
    }

    private void UiDownAction_started(InputAction.CallbackContext obj)
    {
        if (selectedButton < itemsList.Count - 1) selectedButton++;

        itemsList[selectedButton].GetComponent<Button>().Select();

        AdjustContentPosition();
    }

    public void UpdateContent(List<CollectableItem> items)
    {
        foreach (var item in itemsList)
        {
            Destroy(item);
        }
        itemsList.Clear();

        if(items.Count >= 5)
        {
            AdjustRectTransformSize(viewport, 150);
        }
        else
        {
            AdjustRectTransformSize(viewport, items.Count * 30);
        }

        content.sizeDelta = new Vector2(content.sizeDelta.x, items.Count * 30);

        for (int i = 0; i < items.Count; i++)
        {
            var template = Instantiate(itemTemplate, content.transform);
            template.SetActive(true);
            template.GetComponent<CollectableItemIconUI>().SetData(items[i], ui);
            itemsList.Add(template);
        }
        if(selectedButton == -1)
        {
            selectedButton = 0;
        }
        else if(selectedButton >= itemsList.Count)
        {
            selectedButton = itemsList.Count - 1;
        }
        itemsList[selectedButton].GetComponent<Button>().Select();

        AdjustContentPosition();
    }

    private void AdjustRectTransformSize(RectTransform rectTransform, int size)
    {
        rectTransform.sizeDelta = new Vector2 (rectTransform.sizeDelta.x, size);
    }

    private void AdjustContentPosition()
    {
        
        if(content.anchoredPosition.y < 30 * selectedButton - 120)
        {
            content.anchoredPosition = new Vector2(content.anchoredPosition.x,30 * selectedButton -120);
        }
        else if(content.anchoredPosition.y > 30 * selectedButton)
        {
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, 30 * selectedButton);
        }
    }
}
