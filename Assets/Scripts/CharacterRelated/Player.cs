using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This is the player script, it contains functionality that is specific to the Player
/// </summary>
public class Player : Character
{
    private static Player instance;

    public static Player MyInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Player>();
            }

            return instance;
        }
    }

    private List<Enemy> attackers = new List<Enemy>();

    /// <summary>
    /// The player's mana
    /// </summary>
    [SerializeField]
    private Stat mana;

    [SerializeField]
    private Stat xpStat;

    [SerializeField]
    private Text levelText;

    /// <summary>
    /// The player's initial mana
    /// </summary>
    private float initMana = 50;

    /// <summary>
    /// An array of blocks used for blocking the player's sight
    /// </summary>
    [SerializeField]
    private Block[] blocks;

    /// <summary>
    /// Exit points for the spells
    /// </summary>
    [SerializeField]
    private Transform[] exitPoints;

    [SerializeField]
    private Animator ding;

    [SerializeField]
    private Transform minimapIcon;

    /// <summary>
    /// Index that keeps track of which exit point to use, 2 is default down
    /// </summary>
    private int exitIndex = 2;


    private List<IInteractable> interactables = new List<IInteractable>();

    private Vector3 min, max;

    [SerializeField]
    private GearSocket[] gearSockets;

    public int MyGold { get; set; }

    public List<IInteractable> MyInteractables
    {
        get
        {
            return interactables;
        }

        set
        {
            interactables = value;
        }
    }

    public Stat MyXp
    {
        get
        {
            return xpStat;
        }

        set
        {
            xpStat = value;
        }
    }

    public Stat MyMana
    {
        get
        {
            return mana;
        }

        set
        {
            mana = value;
        }
    }

    public List<Enemy> MyAttackers
    {
        get
        {
            return attackers;
        }

        set
        {
            attackers = value;
        }
    }

    /// <summary>
    /// We are overriding the characters update function, so that we can execute our own functions
    /// </summary>
    protected override void Update()
    {
        //Executes the GetInput function
        GetInput();

        //Clamps the player inside the tilemap
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, min.x, max.x), 
            Mathf.Clamp(transform.position.y, min.y, max.y), 
            transform.position.z);

        base.Update();
    }

    public void SetDefaultValues()
    {
        MyGold = 1000;
        health.Initialize(initHealth, initHealth);
        MyMana.Initialize(initMana, initMana);
        MyXp.Initialize(0, Mathf.Floor(100 * MyLevel * Mathf.Pow(MyLevel, 0.5f)));
        levelText.text = MyLevel.ToString();
    }

    /// <summary>
    /// Listen's to the players input
    /// </summary>
    private void GetInput()
    {
        Direction = Vector2.zero;

        ///THIS IS USED FOR DEBUGGING ONLY
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            health.MyCurrentValue -= 10;
            MyMana.MyCurrentValue -= 10;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            GainXP(600);
        }
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            health.MyCurrentValue += 10;
            MyMana.MyCurrentValue += 10;
        }

        if (Input.GetKey(KeybindManager.MyInstance.Keybinds["UP"])) //Moves up
        {
            exitIndex = 0;
            Direction += Vector2.up;
            minimapIcon.eulerAngles = new Vector3(0, 0, 0);
        }
        if (Input.GetKey(KeybindManager.MyInstance.Keybinds["LEFT"])) //Moves left
        {
            exitIndex = 3;
            Direction += Vector2.left;
            if (Direction.y == 0)
            {
                minimapIcon.eulerAngles = new Vector3(0, 0, 90);
            }
          
        }
        if (Input.GetKey(KeybindManager.MyInstance.Keybinds["DOWN"]))
        {
            exitIndex = 2;
            Direction += Vector2.down;

            minimapIcon.eulerAngles = new Vector3(0, 0, 180);
        }
        if (Input.GetKey(KeybindManager.MyInstance.Keybinds["RIGHT"])) //Moves right
        {
            exitIndex = 1;
            Direction += Vector2.right;
            if (Direction.y == 0)
            {
                minimapIcon.eulerAngles = new Vector3(0, 0, 270);
            }

        }
        if (IsMoving)
        {
            StopAttack();
        }

        foreach (string action in KeybindManager.MyInstance.ActionBinds.Keys)
        {
            if (Input.GetKeyDown(KeybindManager.MyInstance.ActionBinds[action]))
            {
                UIManager.MyInstance.ClickActionButton(action);

            }
        }


    }

    /// <summary>
    /// Set's the player's limits so that he can't leave the game world
    /// </summary>
    /// <param name="min">The minimum position of the player</param>
    /// <param name="max">The maximum postion of the player</param>
    public void SetLimits(Vector3 min, Vector3 max)
    {
        this.min = min;
        this.max = max;
    }

    /// <summary>
    /// A co routine for attacking
    /// </summary>
    /// <returns></returns>
    private IEnumerator Attack(string spellName)
    {
        Transform currentTarget = MyTarget;

        //Creates a new spell, so that we can use the information form it to cast it in the game
        Spell newSpell = SpellBook.MyInstance.CastSpell(spellName);

        IsAttacking = true; //Indicates if we are attacking

        MyAnimator.SetBool("attack", IsAttacking); //Starts the attack animation

        foreach (GearSocket g in gearSockets)
        {
            g.MyAnimator.SetBool("attack", IsAttacking);
        }

        yield return new WaitForSeconds(newSpell.MyCastTime); //This is a hardcoded cast time, for debugging

        if (currentTarget != null && InLineOfSight())
        {
            SpellScript s = Instantiate(newSpell.MySpellPrefab, exitPoints[exitIndex].position, Quaternion.identity).GetComponent<SpellScript>();

            s.Initialize(currentTarget, newSpell.MyDamage, transform);
        }

        StopAttack(); //Ends the attack
    }

    private IEnumerator GatherRoutine(string skillName, List<Drop> items)
    {
        Transform currentTarget = MyTarget;

        //Creates a new spell, so that we can use the information form it to cast it in the game
        Spell newSpell = SpellBook.MyInstance.CastSpell(skillName);

        IsAttacking = true; //Indicates if we are attacking

        MyAnimator.SetBool("attack", IsAttacking); //Starts the attack animation

        foreach (GearSocket g in gearSockets)
        {
            g.MyAnimator.SetBool("attack", IsAttacking);
        }

        yield return new WaitForSeconds(newSpell.MyCastTime); //This is a hardcoded cast time, for debugging

        StopAttack(); //Ends the attack

        LootWindow.MyInstance.CreatePages(items);
    }

    /// <summary>
    /// Casts a spell
    /// </summary>
    public void CastSpell(string spellName)
    {
        Block();

        if (MyTarget != null && MyTarget.GetComponentInParent<Character>().IsAlive &&!IsAttacking && !IsMoving && InLineOfSight()) //Chcks if we are able to attack
        {
            actionRoutine = StartCoroutine(Attack(spellName));
        }
    }

    public void Gather(string skillName, List<Drop> items)
    {
        if (!IsAttacking)
        {
            actionRoutine = StartCoroutine(GatherRoutine(skillName, items));
        }
    }

    /// <summary>
    /// Checks if the target is in line of sight
    /// </summary>
    /// <returns></returns>
    private bool InLineOfSight()
    {
        if (MyTarget != null)
        {
            //Calculates the target's direction
            Vector3 targetDirection = (MyTarget.transform.position - transform.position).normalized;

            //Thorws a raycast in the direction of the target
            RaycastHit2D hit = Physics2D.Raycast(transform.position, targetDirection, Vector2.Distance(transform.position, MyTarget.transform.position), 256);

            //If we didn't hit the block, then we can cast a spell
            if (hit.collider == null)
            {
                return true;
            }

        }

        //If we hit the block we can't cast a spell
        return false;
    }

    /// <summary>
    /// Changes the blocks based on the players direction
    /// </summary>
    private void Block()
    {
        foreach (Block b in blocks)
        {
            b.Deactivate();
        }

        blocks[exitIndex].Activate();
    }

    /// <summary>
    /// Stops the attack
    /// </summary>
    public void StopAttack()
    {
        //Stop the spellbook from casting
        SpellBook.MyInstance.StopCating();

        IsAttacking = false; //Makes sure that we are not attacking

        MyAnimator.SetBool("attack", IsAttacking); //Stops the attack animation

        foreach (GearSocket g in gearSockets)
        {
            g.MyAnimator.SetBool("attack", IsAttacking);
        }


        if (actionRoutine != null) //Checks if we have a reference to an co routine
        {
            StopCoroutine(actionRoutine);
        }
    }

    public override void HandleLayers()
    {
        base.HandleLayers();

        if (IsMoving)
        {
            foreach (GearSocket g in gearSockets)
            {
                g.SetXAndY(Direction.x, Direction.y);
            }
        }
    }

    public override void ActivateLayer(string layerName)
    {
        base.ActivateLayer(layerName);

        foreach (GearSocket g in gearSockets)
        {
            g.ActivateLayer(layerName);
        }
    }


    public void GainXP(int xp)
    {
        MyXp.MyCurrentValue += xp;
        CombatTextManager.MyInstance.CreateText(transform.position, xp.ToString(), SCTTYPE.XP, false);

        if (MyXp.MyCurrentValue >= MyXp.MyMaxValue)
        {
            StartCoroutine(Ding());
        }
    }

    public void AddAttacker(Enemy enemy)
    {
        if (!MyAttackers.Contains(enemy))
        {
            MyAttackers.Add(enemy);
        }
    }

    private IEnumerator Ding()
    {
        while (!MyXp.IsFull)
        {
            yield return null;
        }

        MyLevel++;
        ding.SetTrigger("Ding");
        levelText.text = MyLevel.ToString();
        MyXp.MyMaxValue = 100 * MyLevel * Mathf.Pow(MyLevel, 0.5f);
        MyXp.MyMaxValue = Mathf.Floor(MyXp.MyMaxValue);
        MyXp.MyCurrentValue = MyXp.MyOverflow;
        MyXp.Reset();

        if (MyXp.MyCurrentValue >= MyXp.MyMaxValue)
        {
            StartCoroutine(Ding());
        }

    }

    public void UpdateLevel()
    {
        levelText.text = MyLevel.ToString();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" ||collision.tag== "Interactable")
        {
            IInteractable interactable = collision.GetComponent<IInteractable>();

            if (!MyInteractables.Contains(interactable))
            {
                MyInteractables.Add(interactable);
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" || collision.tag == "Interactable")
        {
            if (MyInteractables.Count > 0)
            {
                IInteractable interactable = MyInteractables.Find(x => x == collision.GetComponent<IInteractable>());

                if (interactable != null)
                {
                    interactable.StopInteract();
                }

                MyInteractables.Remove(interactable);
            }

           
  
        }
    }
}
