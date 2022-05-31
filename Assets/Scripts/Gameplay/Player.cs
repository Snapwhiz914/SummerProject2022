using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    public int speed;
    public float jumpForce;
    public int groundLayerNumber;
    public Image deathPanel;
    public GameObject spawnPoint;

    private Rigidbody2D mainRB;
    private float groundCheckDistance;
    private int groundCheckLayerMask;
    private Vector3 groundCheckRCOffset;
    public bool grounded = true;
    private bool controlEnabled = true;
    public bool doubleJumpEnabled;
    private bool canJumpAgain;

    void Start()
    {
        mainRB = GetComponent<Rigidbody2D>();
        groundCheckDistance = (GetComponent<BoxCollider2D>().bounds.size.y / 2) + .1f;
        groundCheckLayerMask = ~(LayerMask.GetMask("Player"));
        groundCheckRCOffset = new Vector2(GetComponent<BoxCollider2D>().bounds.size.x / 2, 0);
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

        if (Keyboard.current.spaceKey.isPressed)
        {
            if (!doubleJumpEnabled && grounded) mainRB.velocity = new Vector2(mainRB.velocity.x, jumpForce);
            if (doubleJumpEnabled && mainRB.velocity.y <= 0)
            {
                if (grounded)
                {
                    mainRB.velocity = new Vector2(mainRB.velocity.x, jumpForce);
                    canJumpAgain = true;
                }
                else if (canJumpAgain)
                {
                    mainRB.velocity = new Vector2(mainRB.velocity.x, jumpForce);
                    canJumpAgain = false;
                }
            }
        }
        if (!Keyboard.current.spaceKey.isPressed && mainRB.velocity.y > 0)
        {
            mainRB.velocity = new Vector2(mainRB.velocity.x, mainRB.velocity.y * .5f);
        }

        if (
            Physics2D.Raycast(transform.position + groundCheckRCOffset, Vector2.down, groundCheckDistance, groundCheckLayerMask).collider != null
            || Physics2D.Raycast(transform.position - groundCheckRCOffset, Vector2.down, groundCheckDistance, groundCheckLayerMask).collider != null)
        {
            grounded = true;
        } else
        {
            grounded = false;
        }
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
