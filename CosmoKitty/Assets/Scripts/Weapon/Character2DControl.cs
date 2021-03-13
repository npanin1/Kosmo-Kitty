/**************************************************************************************************/
/** 	© 2017 NULLcode Studio. License: https://creativecommons.org/publicdomain/zero/1.0/deed.ru
/** 	Разработано в рамках проекта: http://null-code.ru/
/**                       ******   Внимание! Проекту нужна Ваша помощь!   ******
/** 	WebMoney: R209469863836, Z126797238132, E274925448496, U157628274347
/** 	Яндекс.Деньги: 410011769316504
/**************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class Character2DControl : MonoBehaviour {

	[SerializeField] private float speed = 1.5f; // скорость движения
	[SerializeField] private float acceleration = 100; // ускорение
	[SerializeField] private float jumpForce = 5; // сила прыжка
	[SerializeField] private float jumpDistance = 0.75f; // расстояние от центра объекта, до поверхности (определяется вручную в зависимости от размеров спрайта)
	[SerializeField] private bool facingRight = true; // в какую сторону смотрит персонаж на старте?
	[SerializeField] private KeyCode jumpButton = KeyCode.Space; // клавиша для прыжка

	private Vector3 direction;
	private int layerMask;
	private Rigidbody2D body;

	void Start () 
	{
		body = GetComponent<Rigidbody2D>();
		body.freezeRotation = true;
		layerMask = 1 << gameObject.layer | 1 << 2;
		layerMask = ~layerMask;
	}

	bool GetJump() // проверяем, есть ли коллайдер под ногами
	{
		RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down, jumpDistance, layerMask);
		if(hit.collider) return true;
		return false;
	}

	void FixedUpdate()
	{
		body.AddForce(direction * body.mass * speed * acceleration);

		if(Mathf.Abs(body.velocity.x) > speed)
		{
			body.velocity = new Vector2(Mathf.Sign(body.velocity.x) * speed, body.velocity.y);
		}
	}

	void Flip() // отражение по горизонтали
	{
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawRay(transform.position, Vector3.down * jumpDistance);
	}

	void Update () 
	{
		if(Input.GetKeyDown(jumpButton) && GetJump())
		{
			body.velocity = new Vector2(0, jumpForce);
		}

		float h = Input.GetAxis("Horizontal");

		direction = new Vector2(h, 0); 

		if(h > 0 && !facingRight) Flip(); else if(h < 0 && facingRight) Flip();
	}
}
