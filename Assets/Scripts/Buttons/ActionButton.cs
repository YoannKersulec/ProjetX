using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour, IPointerClickHandler, IClickable,IPointerEnterHandler, IPointerExitHandler
{
    public IUseable MyUseable { get; set; }

    [SerializeField]
    private Text stackSize;

    private Stack<IUseable> useables = new Stack<IUseable>();

    private int count;

    public Button MyButton { get; private set; }

    public Image MyIcon
    {
        get
        {
            return icon;
        }

        set
        {
            icon = value;
        }
    }

    public int MyCount
    {
        get
        {
            return count;
        }
    }


    public Text MyStackText
    {
        get
        {
            return stackSize;
        }
    }

    [SerializeField]
    private Image icon;

    void Start ()
    {
        MyButton = GetComponent<Button>();
        MyButton.onClick.AddListener(OnClick);
        InventoryScript.MyInstance.itemCountChangedEvent += new ItemCountChanged(UpdateItemCount);

	}
	
	void Update () {
		
	}

    public void OnClick()
    {
        if (HandScript.MyInstance.MyMoveable == null)
        {
            if (MyUseable != null)
            {
                MyUseable.Use();
            }
            if (useables != null && useables.Count > 0)
            {
                useables.Peek().Use();
            }
        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (HandScript.MyInstance.MyMoveable != null && HandScript.MyInstance.MyMoveable is IUseable)
            {
                SetUseable(HandScript.MyInstance.MyMoveable as IUseable); 
            }
        }
    }

    public void SetUseable(IUseable useable)
    {
        if (useable is Item)
        {
            useables = InventoryScript.MyInstance.GetUseables(useable);
           
            InventoryScript.MyInstance.FromSlot.MyIcon.color = Color.white;
            InventoryScript.MyInstance.FromSlot = null;
        }
        else
        {
            useables.Clear();
            this.MyUseable = useable;
        }

        count = useables.Count;
        UpdateVisual();
    }

    public void UpdateVisual() 
    {
        MyIcon.sprite = HandScript.MyInstance.Put().MyIcon;
        MyIcon.color = Color.white;

        if (count > 1)
        {
            UIManager.MyInstance.UpdateStackSize(this);
        }
        else if (MyUseable is Spell)
        {
            UIManager.MyInstance.ClearStackCount(this);
        }
    }

    public void UpdateItemCount(Item item)
    {
        if (item is IUseable && useables.Count > 0)
        {
            if (useables.Peek().GetType() == item.GetType())
            {
                useables = InventoryScript.MyInstance.GetUseables(item as IUseable);

                count = useables.Count;

                UIManager.MyInstance.UpdateStackSize(this);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        IDescribable tmp = null;

        if (MyUseable !=null && MyUseable is IDescribable)
        {
            tmp = (IDescribable)MyUseable;
        }
        else if (useables.Count > 0)
        {
        }
        if (tmp != null)
        {
            UIManager.MyInstance.ShowToolitip(new Vector2(1,0), transform.position, tmp);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.MyInstance.HideTooltip();
    }
}
