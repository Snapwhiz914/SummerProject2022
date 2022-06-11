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
    public bool doubleJumpEnabled;
    [Range(0f, Mathf.Infinity)]
    public float airControlFactor;

    private Rigidbody2D mainRB;
    private float groundCheckDistance;
    private int groundCheckLayerMask;
    private Vector3 groundCheckRCOffset;
    private Vector3 sideCheckRCOffset;
    private bool grounded = true;
    private bool controlEnabled = true;
    private bool canJumpAgain;
    private bool spaceKeyDebounce;
    private bool isDying;

    private bool isDragging = false;
    private Vector2 startDragPos;
    private GameObject currentThrowable;
    public GameObject throwable;

    void Start()
    {
        mainRB = GetComponent<Rigidbody2D>();
        groundCheckDistance = (GetComponent<BoxCollider2D>().bounds.size.y / 2) + .05f;
        groundCheckLayerMask = ~(LayerMask.GetMask("Player"));
        groundCheckRCOffset = new Vector2(GetComponent<BoxCollider2D>().bounds.size.x / 2, 0);
        sideCheckRCOffset = new Vector2(0, GetComponent<BoxCollider2D>().bounds.size.y / 2);
    }

    void FixedUpdate()
    {
        if (!controlEnabled) return;
        if (Keyboard.current.dKey.isPressed && Physics2D.Raycast(transform.position - sideCheckRCOffset, Vector2.right, groundCheckDistance, groundCheckLayerMask).collider == null)
        {
            mainRB.velocity = new Vector2(speed * Time.deltaTime, mainRB.velocity.y);
        }
        if (Keyboard.current.aKey.isPressed && Physics2D.Raycast(transform.position - sideCheckRCOffset, Vector2.left, groundCheckDistance, groundCheckLayerMask).collider == null)
        {
            mainRB.velocity = new Vector2(-speed * Time.deltaTime, mainRB.velocity.y);
        }
        if (!Keyboard.current.aKey.isPressed && !Keyboard.current.dKey.isPressed)
        {
            mainRB.velocity = new Vector2(0, mainRB.velocity.y);
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
                    spaceKeyDebounce = false;
                }
                else if (canJumpAgain && spaceKeyDebounce)
                {
                    mainRB.velocity = new Vector2(mainRB.velocity.x, jumpForce);
                    canJumpAgain = false;
                    spaceKeyDebounce = false;
                }
            }
        }
        if (!Keyboard.current.spaceKey.isPressed && !spaceKeyDebounce)
        {
            spaceKeyDebounce = true;
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

        if (Mouse.current.leftButton.isPressed && !isDragging)
        {
            //Mouse is down for the first time (OnMouseDown)
            isDragging = true;
            Mouse.current.WarpCursorPosition(Camera.main.WorldToScreenPoint(transform.position));
            startDragPos = transform.position;
            //currentThrowable = Instantiate(throwable, transform.position, transform.rotation);
        }
        //if (Mouse.current.leftButton.isPressed && isDragging)
        //{
            //Called when mouse is dragging (OnMouseDrag)
        //}
        if (!Mouse.current.leftButton.isPressed && isDragging)
        {
            //Called when mouse is released (OnMouseUp)
            isDragging = false;
            Vector2 dragEndPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 velocity = (dragEndPos - startDragPos)*2;
            currentThrowable = Instantiate(throwable, transform.position, transform.rotation);
            currentThrowable.GetComponent<Rigidbody2D>().velocity = -velocity;
            currentThrowable.GetComponent<ThrowableManager>().setThrower(this);
            Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), currentThrowable.GetComponent<BoxCollider2D>(), true);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Death check
        if (collision.gameObject.CompareTag("Killer") && !isDying)
        {
            StartCoroutine(die(collision));
        }
    }

    private IEnumerator die(Collision2D collision)
    {
        controlEnabled = false;
        mainRB.velocity = Vector2.zero;
        isDying = true;
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
        isDying = false;
        controlEnabled = true;
    }
}
