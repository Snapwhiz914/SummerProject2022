using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    public int speed;
    public bool canJump = true;
    private Rigidbody2D rb;
    public float jumpForce;
    public int groundLayerNumber;
    public Image deathPanel;
    private bool controlEnabled = true;
    public GameObject spawnPoint;

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
        if (!controlEnabled) return;
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
        if (ch == ' ' && canJump && controlEnabled)
        {
            Debug.Log("iia true");
            rb.AddForce(new Vector2(0f, jumpForce));
            canJump = false;
        }
        Debug.Log(ch);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Death check
        if (collision.gameObject.CompareTag("Killer"))
        {
            StartCoroutine(die(collision));
        }
    }

    private IEnumerator die(Collision2D collision)
    {
        controlEnabled = false;
        for (int i = 0; i < 100; i++)
        {
            deathPanel.color = new Color(0, 0, 0, i*.01f);
            yield return new WaitForSeconds(0.01f);
        }
        transform.position = spawnPoint.transform.position;
        for (int i = 100; i > 0; i--)
        {
            deathPanel.color = new Color(0, 0, 0, i*.01f);
            yield return new WaitForSeconds(0.01f);
        }
        controlEnabled = true;
    }
}
