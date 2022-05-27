using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    public int speed;
    public bool isInAir = true;
    private Rigidbody2D rb;
    public float jumpForce;
    public int groundLayerNumber;

    void Start()
    {
        Keyboard.current.onTextInput += OnTextInput;
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnDisable()
    {
        Keyboard.current.onTextInput -= OnTextInput;
    }

    void FixedUpdate()
    {
        if (Keyboard.current.dKey.isPressed)
        {
            transform.position = new Vector2(transform.position.x + speed * Time.deltaTime, transform.position.y);
        }
        if (Keyboard.current.aKey.isPressed)
        {
            transform.position = new Vector2(transform.position.x - speed * Time.deltaTime, transform.position.y);
        }
    }

    private void OnTextInput(char ch)
    {
        if (ch == ' ' && !isInAir)
        {
            Debug.Log("iia true");
            rb.AddForce(new Vector2(0f, jumpForce));
            isInAir = true;
        }
        Debug.Log(ch);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == groundLayerNumber)
        {
            isInAir = false;
            Debug.Log("iia false");
        }
        Debug.Log("oce2");
    }
}
