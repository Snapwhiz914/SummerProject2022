using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTriggerOnCollision : MonoBehaviour
{

    public string whatToSay;
    public float timeBetweenLetters;
    public bool controlEnabled;

    private bool hasBeenTriggered = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !hasBeenTriggered)
        {
            StartCoroutine(collision.gameObject.GetComponent<Player>().sayDialog(whatToSay, controlEnabled, timeBetweenLetters));
            hasBeenTriggered = true;
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
