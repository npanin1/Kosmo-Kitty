using UnityEngine;
[RequireComponent(typeof(CircleCollider2D))]
public class Ammo : MonoBehaviour
{
    [Header("Префаб оружия и количество патронов:")]
    public Weapon weaponPrefab;
    public int patrons = 42;

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
            WeaponManager.Add(weaponPrefab, patrons, 2);
            Destroy(gameObject);
        }
    }
}


