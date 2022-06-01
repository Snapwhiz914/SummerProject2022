using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

    public GameObject target;
    public Transform background;
    public float backgroundMovementFactor;
    private float yStart;
    private float cameraSize;
    private Vector2 currentBackgroundOffset;
    private Vector3 cameraStart;

    void Start()
    {
        cameraSize = GetComponent<Camera>().orthographicSize;
        yStart = transform.position.y - cameraSize;
        cameraStart = transform.position;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        float newY = target.transform.position.y;
        if (target.transform.position.y - cameraSize < yStart) newY = 0;
        currentBackgroundOffset = (cameraStart - transform.position)/backgroundMovementFactor;
        transform.position = new Vector3(target.transform.position.x, newY, -10);
        background.position = new Vector2(transform.position.x, transform.position.y) - currentBackgroundOffset;
    }
}
