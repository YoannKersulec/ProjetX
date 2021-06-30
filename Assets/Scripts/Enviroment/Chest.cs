using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Sprite openSprite, closedSprite;

    private bool isOpen;

    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private List<Item> items;

    [SerializeField]
    private BagScript bag;

    [SerializeField]
    private List<Item> debugItems;

    bool FirstOpened = false;


    public List<Item> MyItems
    {
        get
        {
            return items;
        }

        set
        {
            items = value;
        }
    }

    public BagScript MyBag
    {
        get
        {
            return bag;
        }

        set
        {
            bag = value;
        }
    }

    private void Awake()
    {
        items = new List<Item>();

    }

    public void Interact()
    {
        if (isOpen)
        {
            StopInteract();
        }
        else
        {
            
            AddItems();
            if (!FirstOpened)
            {
                FirstOpened = true;
                for (int i = 0; i < 4; i++)
                {
                    GoldNugget nugget = (GoldNugget)Instantiate(debugItems[0]);
                    MyBag.AddItem(nugget);
                }
            }
            isOpen = true;
            spriteRenderer.sprite = openSprite;
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
        }

    }

    public void StopInteract()
    {
        if (isOpen)
        {
            StoreItems();
            MyBag.Clear();
            isOpen = false;
            spriteRenderer.sprite = closedSprite;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0;
        }


    }

    public void AddItems()
    {
        if (MyItems != null)
        {
            foreach (Item item in MyItems)
            {
                item.MySlot.AddItem(item);
            }
        }
    }

    public void StoreItems()
    {
        MyItems = MyBag.GetItems();
    }

}
