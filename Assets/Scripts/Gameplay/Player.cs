using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

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
    public float throwableSpeed;

    public GameObject dialogObject;
    public TMP_Text dialogText;
    public GameObject pabtc;

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
    private bool didLandOnBouncePad;
    private bool didLandOnRotatedBouncePad;

    private bool isDragging = false;
    private GameObject currentThrowable;
    Vector3 currentPos;
    public GameObject throwable;
    private LineRenderer lr;

    void Start()
    {
        mainRB = GetComponent<Rigidbody2D>();
        groundCheckDistance = (GetComponent<BoxCollider2D>().bounds.size.y / 2) + .05f;
        groundCheckLayerMask = ~(LayerMask.GetMask("Player"));
        groundCheckRCOffset = new Vector2(GetComponent<BoxCollider2D>().bounds.size.x / 2, 0);
        sideCheckRCOffset = new Vector2(0, GetComponent<BoxCollider2D>().bounds.size.y / 2);
        lr = GetComponent<LineRenderer>();
        StartCoroutine(loadEffect());
    }

    void FixedUpdate()
    {
        if (!controlEnabled) return;
        if (Keyboard.current.dKey.isPressed 
            && Physics2D.Raycast(transform.position - sideCheckRCOffset, Vector2.right, groundCheckDistance, groundCheckLayerMask).collider == null
            && !didLandOnRotatedBouncePad)
        {
            mainRB.velocity = new Vector2(speed * Time.deltaTime, mainRB.velocity.y);
        }
        if (Keyboard.current.aKey.isPressed
            && Physics2D.Raycast(transform.position - sideCheckRCOffset, Vector2.left, groundCheckDistance, groundCheckLayerMask).collider == null
            && !didLandOnRotatedBouncePad)
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
        if (!Keyboard.current.spaceKey.isPressed && mainRB.velocity.y > 0 && !didLandOnBouncePad)
        {
            mainRB.velocity = new Vector2(mainRB.velocity.x, mainRB.velocity.y * .5f);
        }
        if (
            Physics2D.Raycast(transform.position + groundCheckRCOffset, Vector2.down, groundCheckDistance, groundCheckLayerMask).collider != null
            || Physics2D.Raycast(transform.position - groundCheckRCOffset, Vector2.down, groundCheckDistance, groundCheckLayerMask).collider != null)
        {
            grounded = !didLandOnBouncePad; //If true, then grounded will be false, so player will not be able to jump whil launched by a bounce pad.
        } else
        {
            grounded = false;
        }

        if (Mouse.current.leftButton.isPressed && !isDragging)
        {
            //Mouse is down for the first time (OnMouseDown)
            isDragging = true;
            Mouse.current.WarpCursorPosition(Camera.main.WorldToScreenPoint(transform.position));
            //currentThrowable = Instantiate(throwable, transform.position, transform.rotation);
        }
        if (Mouse.current.leftButton.isPressed && isDragging)
        {
            //Called when mouse is dragging (OnMouseDrag)
            currentPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            List<Vector3> points = Plot(throwable.GetComponent<Rigidbody2D>(), transform.position, -((currentPos - transform.position) * throwableSpeed), 500);
            lr.positionCount = points.Count;
            lr.SetPositions(points.ToArray());
        }
        if (!Mouse.current.leftButton.isPressed && isDragging)
        {
            //Called when mouse is released (OnMouseUp)
            isDragging = false;
            currentPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 velocity = (currentPos - transform.position)*throwableSpeed;
            currentThrowable = Instantiate(throwable, transform.position, transform.rotation);
            currentThrowable.GetComponent<Rigidbody2D>().velocity = -velocity;
            currentThrowable.GetComponent<ThrowableJumpPad>().setThrower(this);
            Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), currentThrowable.GetComponent<BoxCollider2D>(), true);
            lr.positionCount = 0;
            lr.SetPositions(new Vector3[0]);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Death check
        if (collision.gameObject.CompareTag("Killer") && !isDying)
        {
            StartCoroutine(die(collision));
        }

        //Jump pad interaction
        if (collision.gameObject.CompareTag("BouncePad"))
        {
            //if it is a 90 degree pad and the hit was 90 degrees, then a land event happened
            if (collision.gameObject.GetComponent<ThrowableJumpPad>().isThisRotated()
                && isApproximately(Vector2.Angle(collision.GetContact(0).normal, Vector2.up), 90, 0.01f))
            {
                didLandOnRotatedBouncePad = true;
                didLandOnBouncePad = false;
            }
            //Same as above but for flat pads
            if (!collision.gameObject.GetComponent<ThrowableJumpPad>().isThisRotated()
                && isApproximately(Vector2.Angle(collision.GetContact(0).normal, Vector2.up), 0, 0.01f))
            {
                didLandOnBouncePad = true;
                didLandOnRotatedBouncePad = false;
            }
        } else
        {
            didLandOnBouncePad = false;
            didLandOnRotatedBouncePad = false;
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

    private IEnumerator loadEffect()
    {
        controlEnabled = false;
        transform.position = spawnPoint.transform.position;
        deathPanel.color = new Color(0, 0, 0, 255);
        for (int i = 100; i > 0; i--)
        {
            deathPanel.color = new Color(0, 0, 0, i * .01f);
            yield return new WaitForSeconds(0.01f);
        }
        controlEnabled = true;
    }

    private bool isApproximately(float a, float b, float tolerance)
    {
        return (Mathf.Abs(a - b) < tolerance);
    }

    public void returnPad(GameObject pad)
    {
        Destroy(pad);
    }

    public List<Vector3> Plot(Rigidbody2D rigidbody, Vector2 pos, Vector2 velocity, int steps)
    {
        List<Vector3> results = new List<Vector3>();

        float timestep = Time.fixedDeltaTime / Physics2D.velocityIterations;
        Vector2 gravityAccel = Physics2D.gravity * rigidbody.gravityScale * timestep * timestep;
        float drag = 1f - timestep * rigidbody.drag;
        Vector2 moveStep = velocity * timestep;

        while (true)
        {
            moveStep += gravityAccel;
            moveStep *= drag;
            pos += moveStep;
            if (Physics2D.CircleCast(pos, .1f, Vector2.zero, 0, groundCheckLayerMask).collider != null)
            {
                break; //stops line before collision
            }
            results.Add(pos);
        }

        return results;
    }

    public IEnumerator sayDialog(string whatToSay, bool canMove, float speedBetweenLetters)
    {
        controlEnabled = canMove;
        dialogObject.SetActive(true);
        pabtc.SetActive(false) ;
        foreach (char letter in whatToSay) {
            dialogText.text = dialogText.text + letter;
            yield return new WaitForSeconds(speedBetweenLetters);
        }
        controlEnabled = true;
        pabtc.SetActive(!canMove);
        if (!canMove)
        {
            yield return new WaitUntil(() =>
            {
                return Keyboard.current.anyKey.isPressed;
            });
        } else
        {
            yield return new WaitForSeconds(1.75f);
        }
        dialogObject.SetActive(false);
        dialogText.text = "";
        if (!canMove) yield return new WaitForSeconds(.1f);
        controlEnabled = true;
    }
}
