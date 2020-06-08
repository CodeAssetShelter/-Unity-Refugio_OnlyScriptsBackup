using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(UnityEngine.Tilemaps.Tilemap))]
public class MapInfo : MonoBehaviour
{
   
    [SerializeField]
    private MapManager.EntranceType startType;
    [SerializeField]
    private MapManager.EntranceType endType;

    public Tilemap tileMap;
    public GameObject itemBatchContainer;
    public ItemLocator[] itemLocator;

    [HideInInspector]
    public bool isFinalMap = false;

    Transform container;
    private void Awake()
    {
        if (itemBatchContainer == null) return;

        container = itemBatchContainer.transform;
        itemLocator = new ItemLocator[container.childCount];
        for(int i = 0; i < container.childCount; i++)
        {
            itemLocator[i] = container.GetChild(i).GetComponent<ItemLocator>();
        }
    }

    public MapManager.EntranceType GetStartType()
    {
        return startType;
    }

    public MapManager.EntranceType GetEndType()
    {
        return endType;
    }

    public void SetItemLocate(List<SpriteRenderer> scoreItems, Sprite sprite)
    {
        if (itemBatchContainer == null) return;

        container = itemBatchContainer.transform;

        if (container.childCount == 0 || container == null) return;
        for (int i = 0; i < container.childCount; i++)
        {
            if (itemLocator[i] != null)
                itemLocator[i].SetItem(scoreItems, sprite);
        }
    }
}
