using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBridge : MonoBehaviour
{
    MapManager.EntranceType entranceType;

    public Transform[] entrances;
    [Space(20)]
    public GameObject startTileMap;
    public Transform[] startPortals;
    public Transform[] endPortals;


    List<Vector3>portalLocalPostions = new List<Vector3>();
    List<Transform> BackGrounds = new List<Transform>();
    Vector3 backGroundsLocalPosition;
    public void Awake()
    {
        for (int i = 0; i < startPortals.Length; i++)
        {
            portalLocalPostions.Add(startPortals[i].transform.localPosition);
        }
        for (int i = 0; i < endPortals.Length; i++)
        {
            portalLocalPostions.Add(endPortals[i].transform.localPosition);
        }
        for (int i = 0; i < entrances.Length; i++)
        {
            BackGrounds.Add(entrances[i].Find("BackGround"));
        }
        backGroundsLocalPosition = BackGrounds[0].localPosition;
    }
    public MapInfo ActiveMapBridge(MapManager.EntranceType currMapEndType, MapManager.EntranceType endType, string parentMapTag)
    {
        for (int i = 0; i < entrances.Length; i++)
        {
            if (entrances[i].CompareTag(parentMapTag) == true)
            {
                int selected = Random.Range(0, 3);
                MapInfo sender = entrances[i].GetChild(selected).GetComponent<MapInfo>();
                sender.transform.parent.gameObject.SetActive(true);

                // entrances[i].Find("BackGround").SetParent(sender.transform);
                //entrances[i].GetChild(entrances[i].childCount - 1).SetParent(sender.transform);
                BackGrounds[i].SetParent(sender.transform);
                BackGrounds[i].localPosition = backGroundsLocalPosition;

                int index = (int)currMapEndType;
                startPortals[index].SetParent(sender.transform);
                startPortals[index].transform.localPosition = portalLocalPostions[index];
                startPortals[index].gameObject.SetActive(true);

                index = (int)sender.GetEndType();
                endPortals[index].SetParent(sender.transform);
                endPortals[index].transform.localPosition = portalLocalPostions[startPortals.Length + index];
                endPortals[index].gameObject.SetActive(true);

                startTileMap.transform.SetParent(sender.transform);
                startTileMap.transform.position = sender.transform.position;
                startTileMap.SetActive(true);

                return sender;
            }
        }
        return null;
    }

    //public Transform nextEntrances;
    //public Transform enterPortals;
    //public Transform exitPortals;
    //public GameObject startTileMap;

    //public MapInfo ActiveMapBridge(MapManager.EntranceType startType, MapManager.EntranceType endType, string parentMapTag)
    //{

    //    Debug.Log(startType.ToString() + " // " + endType.ToString() + " // " + parentMapTag);
    //    // MapNature, MapDarkwood, MapUnderWater, MapDriedDepth, MapMemoryRetro
    //    for(int i = 0; i < nextEntrances.childCount; i++)
    //    {
    //        Transform entrance = nextEntrances.GetChild(i);
    //        Debug.Log(entrance.name);
    //        if (entrance.CompareTag(parentMapTag) == true)
    //        {
    //            entrance.gameObject.SetActive(true);
    //            entrance.GetChild((int)startType).gameObject.SetActive(true);
    //            exitPortals.GetChild((int)startType).gameObject.SetActive(true);
    //            enterPortals.GetChild((int)endType).gameObject.SetActive(true);

    //            startTileMap.SetActive(true);
    //            Debug.Log(entrance.GetChild((int)startType).name);
    //            return entrance.GetChild((int)startType).GetComponent<MapInfo>();
    //        }
    //    }
    //    return null;
    //}
}
