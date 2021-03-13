using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Footsteps))]
public class WeaponPickup : MonoBehaviour
{
    [Header("Префаб оружия и количество патронов:")]
    public Weapon weaponPrefab;
    public int patrons = 42;

    int flag = 1;
    private Animator animator;
    private Footsteps foot;

    private void Start()
    {
        foot = GetComponent<Footsteps>();
    }
    private void Awake()
    {
        animator = GetComponent<Animator>(); //Returns a component of type Animator
    }

    void OnValidate()
    {
        Collider2D coll = GetComponent<Collider2D>();
        coll.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.tag.CompareTo("Player") == 0)
        {
            // если делать программное добавление оружия, например, в начале игры
            // то эту функцию можно делать только (!) через void Start()
            if(flag == 1)foot.PlayStep(Footsteps.StepsOn.OpenBox, 1);
            WeaponManager.Add(weaponPrefab, patrons, flag);
            flag = 0;
            BoxState = BoxState.Empty;
            //Destroy(gameObject);
        }
    }

    private BoxState BoxState
    {
        get { return (BoxState)animator.GetInteger("State"); }
        set { animator.SetInteger("State", (int)value); }
    }
}

public enum BoxState //character state class 
{
    Idle,
    Empty
}
