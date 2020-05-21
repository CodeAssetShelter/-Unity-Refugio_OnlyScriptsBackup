using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    public ControllerPlayer player;

    // Detect Enemy Collision
    // Detect Item Collision
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy") == true)
        {
            player.GetDamaged();
        }

        //if(collision.tag == "EditorOnly")
        //{
        //    ItemManager item = collision.GetComponent<ItemManager>();
        //    item.Use();
        //}
        //Debug.Log(" T : " + collision.name);
    }

    // Detect Map Collision
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Map") == true)
        {
            player.GetDamaged();
        }
    }
}
