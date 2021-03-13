using UnityEngine;

public class WeaponBullet : MonoBehaviour {

    [Header("Наносимый урон:")]
    public float damage = 15;

	void Start()
	{
		Destroy(gameObject, 10);
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if(!coll.isTrigger)
		{
            Enemy target = coll.GetComponent<Enemy>();
			if(target != null) target.AdjustHP(-damage);
            Destroy(gameObject);
        }
	}
}
