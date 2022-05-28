using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomDeathBarrier : MonoBehaviour
{
    private IEnumerator allowFallThrough()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        yield return new WaitForSeconds(1);
        GetComponent<BoxCollider2D>().enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(allowFallThrough());
        }
    }
}
