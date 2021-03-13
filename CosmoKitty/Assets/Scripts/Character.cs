using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Footsteps))]

public class Character : Unit
{
    [SerializeField] 
    private int lives = 5; //count of characters lives

    private LivesBar livesBar;

    [SerializeField] //Чтобы отображалась в юнити
    private float speed = 3.0F; //speed of character
    [SerializeField]
    private int jumpForce = 15; //force of jump
    public float jumpDistance = 0.75f; // расстояние от центра объекта, до поверхности (определяется вручную в зависимости от размеров спрайта)

    private bool isGround = false; // variable that determines whether a character is on the ground or not

    new private Rigidbody2D rigidbody; //Rigibody2D - the sprite will be subject to gravity and can be controlled from scripts using forces.
    private Animator animator;

    public string ladderTag = "GameController"; // тег лестниц

    private int layerMask;
    private Rigidbody2D body;
    private Vector3 upLadder, downLadder;
    private bool isLadder;
    private Footsteps foot;
    private float curT, curStepTimer;
    private string tagName;
    private bool fall;
    public float stepTimer = 0.8f;

    public Camera mainCamera;
    public bool facingRight = true; // на старте, персонаж смотрит вправо?
    private Vector3 theScale;
    private Vector3 pos;
    private float k = 0;

    void Start()
    {
        foot = GetComponent<Footsteps>();
        body = GetComponent<Rigidbody2D>();
        theScale = transform.localScale;
        body.freezeRotation = true;
        layerMask = 1 << gameObject.layer | 1 << 2;
        layerMask = ~layerMask;
    }
    private void Awake() //initialization of variables and state of the game before starting the game.
    {
        livesBar = FindObjectOfType<LivesBar>();

        rigidbody = GetComponent<Rigidbody2D>(); //Returns a component of type Rigidbody2
        animator = GetComponent<Animator>(); //Returns a component of type Animator

    }

    private void FixedUpdate() //the event can be triggered multiple times in a frame
    {
        CheckGround();
    }

    private void Update() //the event can be triggered one times in a frame
    {
        // переносим позицию из мировых координат в экранные
        pos = mainCamera.WorldToScreenPoint(transform.position);

        if (k == 0) LookAtCursor();
        if (k > 0 && !facingRight) Flip(); else if (k < 0 && facingRight) Flip();
        else LookAtCursor();
        

        if (Input.GetKey(KeyCode.W)) State = CharState.Climb;
        else if (isGround) State = CharState.Idle;  //if character is on ground then start animation Idle
                                                    // if (Input.GetButtonDown("Fire1")) Shout();
        if (Input.GetButton("Horizontal"))
        {
            if (Input.GetKeyDown(KeyCode.D) && !facingRight && !Input.GetMouseButton(0)) Flip();
            if (Input.GetKeyDown(KeyCode.A) && facingRight && !Input.GetMouseButton(0)) Flip();
            Run();
        }
        if (isGround && Input.GetButtonDown("Jump")) Jump();

        Debug.DrawRay(transform.position, Vector3.down * jumpDistance, Color.red); // подсветка, для визуальной настройки jumpDistance

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && GetJump() || Input.GetKeyDown(KeyCode.Space) && isLadder && v == 0)
        {
            body.isKinematic = false;
        }

        if (isLadder) LadderMode(v);

        curStepTimer = stepTimer;

        if (GetJump())
        {
            Steps();
            Falling();
        }
        else
        {
            curT = 0;
            fall = true;
        }
    }
    void LookAtCursor()
    {
        if (Input.mousePosition.x < pos.x && facingRight && Input.GetMouseButton(0)) Flip();
        else if (Input.mousePosition.x > pos.x && !facingRight && Input.GetMouseButton(0)) Flip();
    }
    void Flip() // отразить по горизонтали
    {
        facingRight = !facingRight;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Unit unit = collision.gameObject.GetComponent<Unit>();

        if (unit) ReceiveDamage();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Bullet bullet = collision.GetComponent<Bullet>();

        if (bullet)
        {
            ReceiveDamage();
        }

        if (collision.tag.Equals("Coin"))
        {
            Coins_collect.crystallCount += 1;
            Destroy(collision.gameObject);
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == ladderTag && !isLadder)
        {
            Ladder ladder = other.GetComponent<Ladder>();
            upLadder = ladder.up.position;
            downLadder = ladder.down.position;
            isLadder = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == ladderTag)
        {
            isLadder = false;
            body.isKinematic = false;
        }
    }
    bool GetJump() // проверяем, есть ли коллайдер под ногами
    {
        bool result = false;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down, jumpDistance, layerMask);
        if (hit.collider)
        {
            result = true;
            tagName = hit.transform.tag; // берем тег поверхности
        }
        else tagName = string.Empty;
        return result;
    }

    void Falling() // падение на что-нибудь
    {
        if (fall)
        {
            fall = false;
            curT = 0;
            GetStep();
        }
    }

    void Steps()
    {
        // округляем текущее значение скорости по оси X и Z, до сотых
        // чтобы исключить те, которые близкие к нулю, например: 0.000001805331f
        // в противном случаи, функция будет срабатывать, даже если персонаж не движется
       // float velocityZ = RoundTo(Mathf.Abs(body.velocity.z), 100);
        float velocityX = RoundTo(Mathf.Abs(body.velocity.x), 100);

        if (Input.GetButton("Horizontal")) // если персонаж движется
        {
            curT += Time.deltaTime;

            if (curT > curStepTimer)
            {
                curT = 0;
                GetStep();
            }
        }
        else
        {
            curT = 1000;
        }
    }

    void GetStep() // фильтр по тегу
    {
        switch (tagName)
        {
            case "Stone":
                foot.PlayStep(Footsteps.StepsOn.Beton, 1);
                break;
            case "Ground":
                foot.PlayStep(Footsteps.StepsOn.Ground, 1);
                break;
        }
    }

    float RoundTo(float f, int to) // округлить до, указанного значения
    {
        return ((int)(f * to)) / (float)to;
    }

void LadderMode(float vertical)
    {
        if (transform.position.y < upLadder.y && vertical > 0)
        {
            body.isKinematic = true;
        }
        else if (transform.position.y > downLadder.y && vertical < 0 && transform.position.y > upLadder.y)
        {
            body.isKinematic = true;
        }
        else if (vertical < 0 && GetJump() && transform.position.y < upLadder.y)
        {
            body.isKinematic = false;
        }

        if (body.isKinematic)
        {
            transform.Translate(new Vector2(0, speed * vertical * Time.fixedDeltaTime)); // движение по лестнице
        }
    }

    private CharState State 
    {
        get { return (CharState)animator.GetInteger("State"); }
        set { animator.SetInteger("State", (int) value); }
    }

    private void Run()
    {
        Vector3 direction = transform.right * Input.GetAxis("Horizontal"); //determines the direction of the character depending on the pressed button

        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime); //character movement

        //sprite.flipX = direction.x < 0.0F; //character rotation

        //float move = Input.GetAxis("Horizontal");

       // rigidbody.velocity = new Vector2(move * speed, rigidbody.velocity.y);

      //  if (move > 0 && !isFacingRight) Flip();
      //  else if (move < 0 && isFacingRight) Flip();

       if (Input.GetKey(KeyCode.W)) State = CharState.Climb;
        else if (isGround) State = CharState.Run;
    }

    private void Jump()
    {
        rigidbody.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);

        State = CharState.Jump;
    }

    public override void ReceiveDamage()
    {
        Lives--;

        rigidbody.velocity = Vector3.zero;
        rigidbody.AddForce(transform.up * 10.0F, ForceMode2D.Impulse);

        Debug.Log(lives);
    }

    private void CheckGround()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.3F);

        isGround = colliders.Length > 1;

        if (!isGround) State = CharState.Jump;
    }

    public int Lives
    {
        get { return lives; }
        set
        {
            if (value < 5) lives = value;
            livesBar.Refresh();
        }
    }
}



public enum CharState //character state class 
{
    Idle,
    Run,
    Jump,
    Climb
}
