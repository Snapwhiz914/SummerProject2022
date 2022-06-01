using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

    public GameObject target;
    public Transform background;
    private float yStart;
    private float cameraSize;

    void Start()
    {
        cameraSize = GetComponent<Camera>().orthographicSize;
        yStart = transform.position.y - cameraSize;

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        float newY = target.transform.position.y;
        if (target.transform.position.y - cameraSize < yStart) newY = 0;
        transform.position = new Vector3(target.transform.position.x, newY, -10);
        background.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
}
