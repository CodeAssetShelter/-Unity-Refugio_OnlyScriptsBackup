using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumper : MonoBehaviour
{
    public GameObject gravity;

    // Test values
    public GameObject destroyAnim;
    // end Test values


    private Animator animator;
    private Rigidbody2D rigid2D;


    [Header("- Move values")]
    [Range(1.0f, 3.0f)]
    public float maxMoveSpeed = 3.0f;
    private float moveSpeed = 1.0f;

    [Range(1.0f, 5.0f)]
    public float maxJumpPower = 5.0f;
    private float jumpPower = 2.0f;

    [Range(0.5f, 1.5f)]
    public float maxJumpDelay = 0.5f;
    private float jumpDelay = 0.5f;

    private Vector3 myConstPosition;
    private Vector3 myGravity = Vector3.down;
    private Vector2 globalGravity;
    private Vector3 myConstPositionOnGround;
    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        rigid2D = GetComponent<Rigidbody2D>();
        globalGravity = Physics2D.gravity;
        myGravity = transform.position - gravity.transform.position;
        myGravity.Normalize();
        myConstPosition = transform.localPosition;
    }

    private void OnEnable()
    {
        destroyAnim.SetActive(false);

        transform.localPosition = myConstPosition;

        StartCoroutine(CorJump());
        StartCoroutine(CorActiveGravity());
    }

    IEnumerator CorActiveGravity()
    {
        while (true)
        {
            rigid2D.AddForce(myGravity * globalGravity * Time.fixedDeltaTime);

            if (isJumping == false)
            {
                transform.localPosition = myConstPositionOnGround;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    bool isJumping = true;
    IEnumerator CorJump()
    {
        float timer = 0.5f;
        Vector2 direction = Vector2.zero;
        while (true)
        {
            if (isJumping == false)
            {
                direction.x = Random.Range(moveSpeed, maxMoveSpeed);
                direction.x = (Random.Range(0, 2) == 0) ? direction.x *= -1 : direction.x *= 1;
                timer = Random.Range(1.0f, maxJumpDelay);
                jumpPower = Random.Range(2.0f, maxJumpPower);

                direction.y = jumpPower;
                direction.y *= myGravity.y;
                rigid2D.AddForce(direction, ForceMode2D.Force);
                isJumping = true;

                animator.SetBool("Jump", true);


                yield return new WaitForSeconds(timer);
            }
            else
            {
                Invoke("SetIsJumping", 2.0f);
            }
            yield return new WaitForSeconds(1.0f);
        }
    }
    private void SetIsJumping()
    {
        isJumping = false;
        myConstPositionOnGround = transform.localPosition;
        destroyAnim.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Map") == true)
        {
            isJumping = false;
            animator.SetBool("Jump", false);
            destroyAnim.SetActive(true);
            myConstPositionOnGround = transform.localPosition;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Map") == true)
        {
            isJumping = true;
            destroyAnim.SetActive(false);
        }
    }
}
