using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundChecker : MonoBehaviour
{

    public Player player;

    void Start()
    {
        player = GetComponentInParent<Player>();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == player.groundLayerNumber)
        {
            player.canJump = true;
        }
    }
}
