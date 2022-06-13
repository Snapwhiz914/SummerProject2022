using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableJumpPad : MonoBehaviour
{
    private Player thrower;
    private float distanceToMoveIfRotated;
    private bool hasLanded = false;
    private bool isRotated = false;

    private bool isApproximately(float a, float b, float tolerance)
    {
        return (Mathf.Abs(a - b) < tolerance);
    }

    public void setThrower(Player t)
    {
        thrower = t;
        distanceToMoveIfRotated = GetComponent<BoxCollider2D>().bounds.extents.x - GetComponent<BoxCollider2D>().bounds.extents.y;
        Debug.Log(distanceToMoveIfRotated);
    }

    public bool isThisRotated()
    {
        return isRotated;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("BouncePad") && !hasLanded)
        {
            returnThisPadToPlayer();
            return;
        }

        if (!collision.gameObject.CompareTag("Player") && !hasLanded) //Landing
        {
            float angle = Vector2.Angle(collision.GetContact(0).normal, Vector2.up);
            Debug.Log(angle);
            if (isApproximately(angle, 90, 0.01f))
            {
                Debug.Log("90 triggered");
                isRotated = true;
                float offset = distanceToMoveIfRotated;
                if (collision.GetContact(0).point.x < transform.position.x) //Left, right is default
                {
                    offset = -distanceToMoveIfRotated;
                }
                transform.rotation = Quaternion.Euler(0, 0, 90);
                transform.position = new Vector3(transform.position.x + offset, transform.position.y);
            }

            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

            if (isApproximately(angle, 180, 1f) || isApproximately(angle, 0, 1f))
            {
                transform.rotation = Quaternion.Euler(0, 0, 0); //Prevents from wierd rotated edge cases
            }

            Physics2D.IgnoreCollision(thrower.GetComponent<BoxCollider2D>(), GetComponent<BoxCollider2D>(), false);

            hasLanded = true;
        }
        
        if (collision.gameObject.CompareTag("Player")) //Player interaction
        {
            Rigidbody2D otherRB = collision.gameObject.GetComponent<Rigidbody2D>();
            if (isRotated) //90 degree version
            {
                otherRB.velocity = new Vector2(-collision.relativeVelocity.x, thrower.jumpForce);
                Debug.Log(otherRB.velocity);
                Debug.Log(new Vector2(-collision.relativeVelocity.x, collision.relativeVelocity.y));
            } else
            {
                otherRB.velocity = new Vector2(collision.relativeVelocity.x, -collision.relativeVelocity.y);
                Debug.Log(otherRB.velocity);
                Debug.Log(new Vector2(collision.relativeVelocity.x, -collision.relativeVelocity.y));
            }
        }
    }
    
    private void returnThisPadToPlayer()
    {
        thrower.returnPad(gameObject);
    }
}
