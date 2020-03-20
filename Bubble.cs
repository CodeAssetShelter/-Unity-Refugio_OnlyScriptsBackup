using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public Rigidbody2D rigid2D;

    public const float maxJumpTimer = 5.0f;

    private bool isJumping = false;
    private bool isCreated = false;

    public class MoveValues
    {
        public GameObject start, end;
        public float speed = 0;
        public float maxSpeed = 0;
        public float jumpPower = 0;
        public float maxJumpPower = 0;

        public void Set(GameObject start, GameObject end, float speed, float jumpPower)
        {
            this.start = start;
            this.end = end;
            this.speed = maxSpeed = speed;
            this.jumpPower = maxJumpPower = jumpPower;

            SetNewValues();
        }

        public void SetNewValues()
        {
            speed = Random.Range(1.0f, maxSpeed);
            jumpPower = Random.Range(1.0f, maxJumpPower);
        }
    }
    MoveValues moveValues = new MoveValues();
    Vector2 gravity;
    Vector2 myGravity = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        gravity = Physics2D.gravity;
        gravity *= 0.3f;


        transform.eulerAngles = transform.parent.eulerAngles;

        isCreated = true;
    }

    //private void OnEnable()
    //{
    //    if (isCreated == true)
    //    {
    //        moveValues.SetNewValues();
    //        StartCoroutine(CMove());
    //        StartCoroutine(CJump());
    //    }
    //}

    public void InitBubble(GameObject start, GameObject end, float speed, float jumpPower, Vector3 gravityVector)
    {
        myGravity = gravityVector;
        moveValues.Set(start, end, speed, jumpPower);
        StartCoroutine(CMove());
        StartCoroutine(CJump());
    }

    IEnumerator CMove()
    {
        float direction = 1;
        while (true)
        {
            if (transform.localPosition.x <= moveValues.start.transform.localPosition.x)
            {
                direction = 1;
            }
            if (transform.localPosition.x >= moveValues.end.transform.localPosition.x)
            {
                direction = -1;
            }
            transform.Translate(Time.fixedDeltaTime * moveValues.speed * direction, 0, 0, Space.Self);
            rigid2D.AddForce(gravity * myGravity * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator CJump()
    {
        float timer = 1.0f;
        timer = Random.Range(1.0f, maxJumpTimer);
        moveValues.jumpPower = Random.Range(1.0f, moveValues.maxJumpPower);
        yield return new WaitForSeconds(timer);

        while (true)
        {
            if (isJumping == false)
            {
                timer = Random.Range(1.0f, maxJumpTimer);
                moveValues.jumpPower = Random.Range(1.0f, moveValues.maxJumpPower);

                rigid2D.AddForce(new Vector2(1.0f, moveValues.jumpPower) * myGravity, ForceMode2D.Force);
                isJumping = true;
                yield return new WaitForSeconds(timer);        
            }
            yield return new WaitForSeconds(1.0f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Map") == true)
        {
            isJumping = false;
        }   
    }
}
