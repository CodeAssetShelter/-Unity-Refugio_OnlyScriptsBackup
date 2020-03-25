using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianEye : MonoBehaviour
{
    public class eyePatchPoints
    {
        public Vector3 leftDown, rightUp;
    }
    private eyePatchPoints eyePatchPoint;

    public SpriteRenderer eyePatch;
    public float eyeSpeed;

    [Space(20)]
    public SpriteRenderer spriteObjectShield;
    public GameObject body;
    public float snatchValue = 0.5f;

    [Space(20)]
    private float recognizeDistance;

    private Transform player;
    private Vector3 eyeConstPosition;
    // Start is called before the first frame update
    void Awake()
    {
        eyeConstPosition = transform.localPosition;
        eyePatchPoint = new eyePatchPoints();

        Vector3 t = new Vector3(0, spriteObjectShield.bounds.extents.y, 0);
        recognizeDistance = Vector3.Distance(t, spriteObjectShield.transform.localPosition);
    }

    private void OnEnable()
    {

        // TEMP
        // 싱글턴에서 받아올 예정임
        if (GameObject.FindWithTag("Player") != null)
        {
            player = GameObject.FindWithTag("Player").transform;

            // Vertice index order
            // 0 1
            // 2 3
            eyePatchPoint.leftDown = eyePatch.sprite.vertices[2] * 0.8f;
            eyePatchPoint.rightUp = eyePatch.sprite.vertices[1] * 0.8f;
            StartCoroutine(CorTrackingPlayer());
        }
    }

    bool foundPlayer = false;
    IEnumerator CorTrackingPlayer()
    {
        Vector3 trackingVector;
        Vector3 gravityVector;
        Vector3 borderPositon = Vector3.zero;
        while (true)
        {
            if (player != null)
            {
                if (foundPlayer == true)
                {
                    // 見逃す
                    if (Mathf.Abs(Vector3.Distance(transform.parent.position, player.position)) >= recognizeDistance ||
                        player.gameObject.activeSelf == false)
                    {
                        foundPlayer = false;
                        transform.localPosition = eyeConstPosition;
                    }
                    // 見つける
                    else
                    {
                        trackingVector = player.position - transform.position;
                        gravityVector = transform.parent.position - body.transform.position;
                        transform.Translate(trackingVector.normalized * eyeSpeed * Time.fixedDeltaTime, Space.World);
                        player.Translate(0, gravityVector.normalized.y * Time.fixedDeltaTime * snatchValue * -1, 0, Space.World);
                        // Left
                        if (transform.localPosition.x < eyePatchPoint.leftDown.x)
                        {
                            borderPositon.Set(eyePatchPoint.leftDown.x, transform.localPosition.y, 0);
                            transform.localPosition = borderPositon;
                        }
                        // Down
                        if (transform.localPosition.y < eyePatchPoint.leftDown.y)
                        {
                            borderPositon.Set(transform.localPosition.x, eyePatchPoint.leftDown.y, 0);
                            transform.localPosition = borderPositon;
                        }
                        // Right
                        if (transform.localPosition.x > eyePatchPoint.rightUp.x)
                        {
                            borderPositon.Set(eyePatchPoint.rightUp.x, transform.localPosition.y, 0);
                            transform.localPosition = borderPositon;
                        }
                        // Up
                        if (transform.localPosition.y > eyePatchPoint.rightUp.y)
                        {
                            borderPositon.Set(transform.localPosition.x, eyePatchPoint.rightUp.y, 0);
                            transform.localPosition = borderPositon;
                        }
                    }
                }
                else
                {
                    if (player.gameObject.activeSelf == true)
                    {
                        Vector3.Distance(transform.parent.position, player.position);
                        if (Vector3.Distance(transform.parent.position, player.position) <= recognizeDistance)
                        {
                            foundPlayer = true;
                        }
                    }
                }
            }
            else
            {
                foundPlayer = false;
                transform.localPosition = eyeConstPosition;
            }
            yield return new WaitForFixedUpdate();
        }
    }


}
