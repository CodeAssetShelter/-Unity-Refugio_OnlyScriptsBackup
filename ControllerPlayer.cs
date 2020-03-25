using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine.EventSystems;

public class ControllerPlayer : MonoBehaviour
{
    [Header ("- Player Components")]
    public new Rigidbody2D rigidbody2D;

    [Header("- Speed")]
    public ObscuredFloat upSpeed = 1;
    public ObscuredFloat fallingLimit = 1;

    private Vector2 mySpeed = Vector2.zero;
    // Start is called before the first frame update
    void Awake()
    {
        fallingLimit *= Physics2D.gravity.y;

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        //this.transform.Translate(Time.fixedDeltaTime * 2.0f, 0, 0, Space.Self);
        if (rigidbody2D.velocity.y >= upSpeed)
        {
            Vector2 newSpeed = Vector2.zero;
            newSpeed.y = upSpeed;
            rigidbody2D.velocity = newSpeed;
        }
        if (rigidbody2D.velocity.y <= fallingLimit)
        {
            Vector2 newSpeed = Vector2.zero;
            newSpeed.y = fallingLimit;
            rigidbody2D.velocity = newSpeed;
        }

        if (Input.GetMouseButton(0))
        {
            if (EventSystem.current.IsPointerOverGameObject() == true)
            {

            }
            else
            {
                rigidbody2D.AddForce(Vector2.up * Physics2D.gravity * -2f, ForceMode2D.Force);
            }
        }
    }

    // Detect Enemy Collision
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(" T : " + collision.name);
    }

    // Detect Map Collision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(" C : " + collision.collider.name);
    }
}
