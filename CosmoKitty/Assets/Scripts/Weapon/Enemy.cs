using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : Monster
{
    [Header("Родительский трансформ:")]
    public Transform parent;
    [Header("Здоровье юнита на старте:")]
    public float HP = 100;
    [Header("Размеры UI бара:")]
    public BarType barType;
    [Header("Точка отображение UI бара:")]
    public Transform barPoint; // дочерняя точка привязки бара, как правило размещается над головой юнита
    [Header("Событие смерти:")]
    public UnityEvent eventDie; // можно делать привязку к другим скриптам, которые будут на этом юните, чтобы сообщить им момент, когда здоровье юнита достигнет нуля
    private Bar bar;

    public float barDelta { get; private set; }
    public float currentHP { get { return bar != null ? bar.curHP : 0; } }

    public float speed;

    public bool facingRight = false;
    public bool lookRight = false;
    bool moveingRight;

    float idleSpeed;

    public int positionOfPatrol; // расстояние на которое противник сможет патрулировать

    public Transform point;

    Transform player;
    public float stoppingDistance;

    bool idle = false;
    bool angry = false;
    bool goBack = false;
    bool fight = false;

    private int lastX;

    private Material matBlink;
    private Material matDefault;
    private SpriteRenderer sprite;



    // Start is called before the first frame update
    protected override void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        matBlink = Resources.Load("EnemyBlink", typeof(Material)) as Material;
        matDefault = sprite.material;

        player = GameObject.FindGameObjectWithTag("Player").transform;
        idleSpeed = speed;

        barDelta = Vector3.Distance(parent.position, barPoint.position);
        bar = UIManager.AddEnemy(this);
    }

    // Update is called once per frame
    protected override void Update()
    {
        int currentX = Mathf.RoundToInt(transform.position.x);
        if (currentX > lastX) lookRight = true; 
        else if (currentX < lastX) lookRight = false;
        lastX = Mathf.RoundToInt(transform.position.x);

        if (lookRight && !facingRight) Flip();
        if (!lookRight && facingRight) Flip();

        if(fight == true)
        {
            Fight();
        }

        if (Vector2.Distance(transform.position, point.position) < positionOfPatrol && angry == false)
        {
            idle = true;
        }

        if (Vector2.Distance(transform.position, player.position) < stoppingDistance)
        {
            angry = true;
            idle = false;
            goBack = false;
        }


        if (Vector2.Distance(transform.position, player.position) > stoppingDistance)
        {
            goBack = true;
            angry = false;
        }

        if (idle == true)
        {
            Idle();
        }
        else if (angry == true)
        {
            Angry();
        }
        else if (goBack == true)
        {
            GoBack();
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            sprite.material = matBlink;
            Invoke("ResetMaterial", .2F);
        }

      /*  if (collision.CompareTag("Point"))
        {
            Debug.Log(fight);
            fight = true;
        }*/
    }

    void ResetMaterial()
    {
        sprite.material = matDefault;
    }

    void Idle()
    {
        if (transform.position.x > point.position.x + positionOfPatrol)
        {
            moveingRight = false;
        }
        else if (transform.position.x < point.position.x - positionOfPatrol)
        {
            moveingRight = true;
        }

        if (moveingRight)
        {
            lookRight = true;
            transform.position = new Vector2(transform.position.x + speed * Time.deltaTime, transform.position.y);
        }
        else
        {
            lookRight = false;
            transform.position = new Vector2(transform.position.x - speed * Time.deltaTime, transform.position.y);
        }
    }

    void Angry()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        speed = idleSpeed + 1;
    }

    void GoBack()
    {
        transform.position = Vector2.MoveTowards(transform.position, point.position, speed * Time.deltaTime);
        speed = idleSpeed;
    }

    void Fight()
    {
        speed = 0;
        Invoke("Angry", .0F);
    }
    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        facingRight = !facingRight;
    }

    void Die()
    {
        bar.MyDestroy();
        eventDie.Invoke();
    }

    public void AdjustHP(float damage)
    {
        bar.Adjust(damage);

        if (bar.curHP <= 0)
        {
            Die();
        }
    }
}
