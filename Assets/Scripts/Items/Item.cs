using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : ScriptableObject, IMoveable, IDescribable
{

    [SerializeField]
    private Sprite icon;


    [SerializeField]
    private int stackSize;

    [SerializeField]
    private string titel;

    [SerializeField]
    private Quality quality;

    private SlotScript slot;

    private CharButton charButton;

    [SerializeField]
    private int price;


    public Sprite MyIcon
    {
        get
        {
            return icon;
        }
    }

    public int MyStackSize
    {
        get
        {
            return stackSize;
        }
    }

    public SlotScript MySlot
    {
        get
        {
            return slot;
        }

        set
        {
            slot = value;
        }
    }

    public Quality MyQuality
    {
        get
        {
            return quality;
        }
    }

    public string MyTitle
    {
        get
        {
            return titel;
        }
    }

    public CharButton MyCharButton
    {
        get
        {
            return charButton;
        }

        set
        {
            MySlot = null;
            charButton = value;
        }
    }

    public int MyPrice
    {
        get
        {
            return price;
        }
    }

    public virtual string GetDescription()
    {
        return string.Format("<color={0}>{1}</color>", QualityColor.MyColors[MyQuality], MyTitle);
    }

    public void Remove()
    {
        if (MySlot != null)
        {
            MySlot.RemoveItem(this);

        }
    }
}

