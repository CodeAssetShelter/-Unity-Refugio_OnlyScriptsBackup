using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDownBlocks : MonoBehaviour
{
    [Header("- Original Prefab")]
    public Block blockPrefab;

    [Header("- Block spawn sector")]
    public GameObject start;
    public GameObject end;

    [Header("- Sprite")]
    public Sprite[] sprites;

    [Header("- Setting values")]
    public int blocks = 10;
    public float fallDownMaxSpeed = 3.0f;

    // private
    List<Block> blockList;

    // Start is called before the first frame update
    void Start()
    {
        blockList = new List<Block>();
    }

    private void OnEnable()
    {
        StartCoroutine(CInitFallDownBlocks());
    }

    IEnumerator CInitFallDownBlocks()
    {
        int index = 0;

        Vector3 spawnPosition = Vector3.zero;
        Block block = null;
        Sprite sprite;

        spawnPosition.y = this.transform.position.y;
        // Create blocks
        while (true)
        {
            if(index > blocks)
            {
                yield break;
            }

            spawnPosition.x = Random.Range(start.transform.position.x, end.transform.position.x);

            block = Instantiate(blockPrefab, spawnPosition, Quaternion.identity, this.transform);

            sprite = sprites[Random.Range(0, sprites.Length)];
            block.Init(start.transform.localPosition.x, end.transform.localPosition.x, fallDownMaxSpeed, sprite);
            //blockList.Add(block);
            index++;
            yield return new WaitForSeconds(0.8f);
        }
    }
}
