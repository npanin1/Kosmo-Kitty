using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootableMonster : Monster
{
    [SerializeField]
    private float rate = 2.0F;

    [SerializeField]
    private Color bulletCollor = Color.white; //Bullet color

    private Bullet bullet;

    private Animator animator;

    protected override void Awake()
    {
        bullet = Resources.Load<Bullet>("Bullet");
        animator = GetComponent<Animator>();

        //ShootEnemyState = ShootEnemyState.Shoot;

    }

    protected override void Start()
    {
        InvokeRepeating("Shoot", rate, rate); //repeat shoot
    }

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        Unit unit = collider.GetComponent<Unit>();
        Bullet bullet = collider.GetComponent<Bullet>();

        if (bullet)
        {
            ReceiveDamage();
        }

        if (unit && unit is Character)
        {
            unit.ReceiveDamage();
        }
    }

    private void Shoot()
    {
       // ShootEnemyState = ShootEnemyState.Shoot;

        Vector3 position = transform.position; //position bullet
        position.y += 0.7F;
        position.x -= 0.5F;

        Bullet newBullet = Instantiate(bullet, position, bullet.transform.rotation) as Bullet; //creating bullet

        newBullet.Parent = gameObject; //who create bullet

        newBullet.Direction = -newBullet.transform.right; // bullet move to right

        newBullet.Color = bulletCollor;
    }

    /*private ShootEnemyState ShootEnemyState
    {
        get { return (ShootEnemyState)animator.GetInteger("State"); }
        set { animator.SetInteger("State", (int)value); }
    }*/
}

public enum ShootEnemyState //character state class 
{
    Idle,
    Shoot
}


