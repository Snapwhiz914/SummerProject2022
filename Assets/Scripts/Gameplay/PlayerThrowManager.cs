using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerThrowManager : MonoBehaviour
{

    private bool isDragging;
    private Player attachedPlayer;
    private Vector2 startDrag;
    private LineRenderer lr;
    private new Camera camera;
    public GameObject disk;

    private float vel; //Initial Velocity, calculated via V = Force / Mass * fixedTime (0.02)
    private float _gravity;
    private float _collisionCheckRadius = 0.1f;
    private float force;
    private float mass;
    public int forcemultiplier;

    void Start()
    {
        //Cursor.visible = false;
        attachedPlayer = GetComponentInParent<Player>();
        disk.GetComponent<BoxCollider2D>().enabled = false;
        lr = GetComponent<LineRenderer>();
        mass = GetComponent<Rigidbody2D>().mass;
        camera = Camera.main;
        lr.enabled = false;
    }

    private List<Vector2> SimulateArc(Vector2 directionVector, Vector2 launchPosition) //A method happening via this List
    {
        List<Vector2> lineRendererPoints = new List<Vector2>(); //Reset LineRenderer List for new calculation

        float maxDuration = 5f; //INPUT amount of total time for simulation
        float timeStepInterval = 0.1f; //INPUT amount of time between each position check
        int maxSteps = (int)(maxDuration / timeStepInterval);//Calculates amount of steps simulation will iterate for

        vel = force / mass * Time.fixedDeltaTime; //Initial Velocity, or Velocity Modifier, with which to calculate Vector Velocity

        for (int i = 0; i < maxSteps; ++i)
        {
            //Remember f(t) = (x0 + x*t, y0 + y*t - 9.81t²/2)
            //calculatedPosition = Origin + (transform.up * (speed * which step * the length of a step);
            Vector2 calculatedPosition = launchPosition + directionVector * vel * i * timeStepInterval; //Move both X and Y at a constant speed per Interval
            calculatedPosition.y += Physics2D.gravity.y / 2 * Mathf.Pow(i * timeStepInterval, 2); //Subtract Gravity from Y

            lineRendererPoints.Add(calculatedPosition); //Add this to the next entry on the list

            if (CheckForCollision(calculatedPosition)) //if you hit something, stop adding positions
            {
                break; //stop adding positions
            }
        }
        return lineRendererPoints;
    }

    private bool CheckForCollision(Vector2 position)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, _collisionCheckRadius); //Measure collision via a small circle at the latest position, dont continue simulating Arc if hit
        if (hits.Length > 0) //Return true if something is hit, stopping Arc simulation
        {
            return true;
        }
        return false;
    }

    void Update()
    {
        if (Mouse.current.leftButton.isPressed && !isDragging)
        {
            //Mouse is down for the first time (OnMouseDown)
            isDragging = true;
            Mouse.current.WarpCursorPosition(transform.position); //sets cursor position
            //Cursor.visible = true;
            startDrag = camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            disk.GetComponent<BoxCollider2D>().enabled = false;
            disk.transform.position = startDrag;
            lr.enabled = true;
        }
        SpringJoint2D sj2d = new SpringJoint2D();
        //sj2d.for
        if (Mouse.current.leftButton.isPressed && isDragging)
        {
            //Called when mouse is dragging (OnMouseDrag)
            Vector2 currentMousePos = camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            disk.transform.position = currentMousePos;

            force = Vector2.Distance(currentMousePos, startDrag) * forcemultiplier;
            List<Vector2> arc = SimulateArc((currentMousePos - startDrag).normalized, currentMousePos);
            lr.positionCount = arc.Count;
            for (int i = 0; i < lr.positionCount; i++)
            {
                lr.SetPosition(i, arc[i]); 
            }
        }
        if (!Mouse.current.leftButton.isPressed && isDragging)
        {
            //Called when mouse is released (OnMouseUp)
            //Cursor.visible = false;
            lr.enabled = false;
        }
    }
}
