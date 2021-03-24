using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField]
    private Stat mana;

    private float initMana = 50;

    [SerializeField]
    private Block[] blocks;

    [SerializeField]
    private Transform[] exitPoints;

    private int exitIndex = 2;

    private IInteractable interactable;

    private Vector3 min, max;

    [SerializeField]
    private GearSocket[] gearSockets;

    protected override void Start()
    {      
        mana.Initialize(initMana, initMana);

        base.Start();
    }
    protected override void Update()
    {
        GetInput();

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, min.x, max.x), 
            Mathf.Clamp(transform.position.y, min.y, max.y), 
            transform.position.z);

        base.Update();
    }

    private void GetInput()
    {
        Direction = Vector2.zero;

        if (Input.GetKeyDown(KeyCode.I))
        {
            health.MyCurrentValue -= 10;
            mana.MyCurrentValue -= 10;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            health.MyCurrentValue += 10;
            mana.MyCurrentValue += 10;
        }

        if (Input.GetKey(KeybindManager.MyInstance.Keybinds["UP"])) 
        {
            exitIndex = 0;
            Direction += Vector2.up;
        }
        if (Input.GetKey(KeybindManager.MyInstance.Keybinds["LEFT"])) 
        {
            exitIndex = 3;
            Direction += Vector2.left; 
        }
        if (Input.GetKey(KeybindManager.MyInstance.Keybinds["DOWN"]))
        {
            exitIndex = 2;
            Direction += Vector2.down;
        }
        if (Input.GetKey(KeybindManager.MyInstance.Keybinds["RIGHT"])) 
        {
            exitIndex = 1;
            Direction += Vector2.right;
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

    public void SetLimits(Vector3 min, Vector3 max)
    {
        this.min = min;
        this.max = max;
    }

    private IEnumerator Attack(string spellName)
    {
        Transform currentTarget = MyTarget;

        Spell newSpell = SpellBook.MyInstance.CastSpell(spellName);

        IsAttacking = true; 

        MyAnimator.SetBool("attack", IsAttacking);

        foreach (GearSocket g in gearSockets)
        {
            g.MyAnimator.SetBool("attack", IsAttacking);
        }

        yield return new WaitForSeconds(newSpell.MyCastTime); 

        if (currentTarget != null && InLineOfSight())
        {
            SpellScript s = Instantiate(newSpell.MySpellPrefab, exitPoints[exitIndex].position, Quaternion.identity).GetComponent<SpellScript>();

            s.Initialize(currentTarget, newSpell.MyDamage, transform);
        }

        StopAttack(); 
    }

    public void CastSpell(string spellName)
    {
        Block();

        if (MyTarget != null && MyTarget.GetComponentInParent<Character>().IsAlive &&!IsAttacking && !IsMoving && InLineOfSight()) 
        {
            attackRoutine = StartCoroutine(Attack(spellName));
        }
    }

    
    private bool InLineOfSight()
    {
        if (MyTarget != null)
        {
           
            Vector3 targetDirection = (MyTarget.transform.position - transform.position).normalized;

          
            RaycastHit2D hit = Physics2D.Raycast(transform.position, targetDirection, Vector2.Distance(transform.position, MyTarget.transform.position), 256);

           
            if (hit.collider == null)
            {
                return true;
            }

        }

        return false;
    }

    private void Block()
    {
        foreach (Block b in blocks)
        {
            b.Deactivate();
        }

        blocks[exitIndex].Activate();
    }

    public void StopAttack()
    {
        SpellBook.MyInstance.StopCating();

        IsAttacking = false; 

        MyAnimator.SetBool("attack", IsAttacking); 

        foreach (GearSocket g in gearSockets)
        {
            g.MyAnimator.SetBool("attack", IsAttacking);
        }


        if (attackRoutine != null) 
        {
            StopCoroutine(attackRoutine);
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

    public void Interact()
    {
        if (interactable != null)
        {
            interactable.Interact();
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" ||collision.tag== "Interactable")
        {
            interactable = collision.GetComponent<IInteractable>();
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" || collision.tag == "Interactable")
        {
            if (interactable != null)
            {
                interactable.StopInteract();
                interactable = null;
            }
  
        }
    }
}
