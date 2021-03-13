using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; //for metod All

public class Pig : Monster
{
    [SerializeField]
    private float speed = 2.0F;

    private SpriteRenderer sprite;

    private Vector3 direction;

    public bool facingRight = true;

    protected override void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    protected override void Start()
    {
        direction = transform.right;
    }

    protected override void Update()
    {
        Move();
    }

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        Unit unit = collider.GetComponent<Unit>();

        if (unit && unit is Character)
        {
            if (Mathf.Abs(unit.transform.position.x - transform.position.x) < 0.3) ReceiveDamage(); //if collided with a character (their distance between colliders is <0.3) then the monster will receive damage
            else unit.ReceiveDamage(); //otherwise the character takes damage
        }
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        facingRight = !facingRight;
    }

    private void Move()
    {
        sprite.flipX = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + transform.up * 0.5F + transform.right * direction.x * 0.7F, 0.05F); //checks if there is a collider in front of it

        if (colliders.Length > 0 && colliders.All(x => !x.GetComponent<Character>()))
        {
            direction *= -1; //if there is a collider turn around
            Flip();
        }

        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);
    }
}
