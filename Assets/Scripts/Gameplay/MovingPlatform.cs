using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    private GameObject[] wayPoints;

    [SerializeField]
    private float speed;

    private int currentTargetWaypoint = 0;

    public void setTargetWaypoint(int target)
    {
        currentTargetWaypoint = target;
    }

    void Start()
    {
        currentTargetWaypoint = 0;
    }

    void FixedUpdate()
    {
        if (transform.localPosition == wayPoints[currentTargetWaypoint].transform.localPosition)
        {
            Debug.Log("cw");
            currentTargetWaypoint++;
            if (currentTargetWaypoint >= wayPoints.Length) currentTargetWaypoint = 0;
        } else
        {
            transform.localPosition = Vector2.MoveTowards(transform.localPosition, wayPoints[currentTargetWaypoint].transform.localPosition, speed * Time.deltaTime);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("BouncePad"))
        {
            collision.gameObject.transform.parent = transform;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.parent = null;
        }
    }
}
