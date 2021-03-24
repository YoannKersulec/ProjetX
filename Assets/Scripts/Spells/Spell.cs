using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class Spell : IUseable, IMoveable, IDescribable
{
    [SerializeField]
    private string name;

    [SerializeField]
    private int damage;

    [SerializeField]
    private Sprite icon;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float castTime;

    [SerializeField]
    private GameObject spellPrefab;

    [SerializeField]
    private string description;

    [SerializeField]
    private Color barColor;

    public string MyName
    {
        get
        {
            return name;
        }
    }

    public int MyDamage
    {
        get
        {
            return damage;
        }

    }

    public Sprite MyIcon
    {
        get
        {
            return icon;
        }
    }

    public float MySpeed
    {
        get
        {
            return speed;
        }
    }

    public float MyCastTime
    {
        get
        {
            return castTime;
        }
    }

    public GameObject MySpellPrefab
    {
        get
        {
            return spellPrefab;
        }
    }

    public Color MyBarColor
    {
        get
        {
            return barColor;
        }
    }

    public string GetDescription()
    {
        return string.Format("{0}\nCast time: {1} second(s)\n<color=#ffd111>{2}\nthat causes {3} damage</color>", name, castTime,description, damage);
    }

    public void Use()
    {
        Player.MyInstance.CastSpell(MyName);
    }
}
