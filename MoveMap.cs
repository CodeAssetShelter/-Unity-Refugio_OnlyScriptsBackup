using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MoveMap : MonoBehaviour
{
    Tilemap tilemap;
    public TileBase tile;
    // Start is called before the first frame update
    void Start()
    {
        tilemap = this.GetComponent<Tilemap>();
        //tile = tilemap.GetComponent<TileBase>();
        Vector3Int test = tilemap.size;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    }
}
