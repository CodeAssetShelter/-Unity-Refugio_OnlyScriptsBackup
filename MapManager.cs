using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance;
    public static MapManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(MapManager)) as MapManager;

                if (_instance == null)
                {
                    //Debug.LogError("No Active GameManager!");
                }
            }

            return _instance;
        }
    }

    public enum EntranceType { TOP = 0, MIDDLE, BOTTOM }

    [System.Serializable]
    public class TileMapInfo
    {
        public MapInfo[] maps;

        public TileMapInfo(GameObject gridRoot)
        {
            maps = gridRoot.GetComponentsInChildren<MapInfo>(true);
        }
    }

    [Header ("- Objects")]
    public GameObject[] gridRoots;
    public TileMapInfo[] tileMapInfos;
    public GameObject scoreItemPrefab;
    private List<GameObject> scoreItems = new List<GameObject>();
    private Vector3 objectPoolingPos = new Vector3(0, 99, 10);

    [Header("- Object Pools")]
    [Range(300, 500)]
    public int scoreItemAmount = 500;

    [Header("- Speed")]
    public float speed = 2.0f;

    // MoveMap values
    MapInfo currMap, nextMap, tempMap;
    Transform currMapTransform, nextMapTransform;

    // Index storage
    List<int> nextMapIndex = new List<int>();
    List<int> gridMapIndex = new List<int>();
    int selected = 0;
    int selectedMap = 0;
    int selectedNextMap = 0;
    int usedMap = 0;

    //public Tilemap[] tileMaps;
    // Start is called before the first frame update
    private void Awake()
    {
        for (int i = 0; i < 300; i++)
        {
            GameObject item =
                Instantiate(scoreItemPrefab, objectPoolingPos, Quaternion.identity, this.transform);
            scoreItems.Add(item);
            item.SetActive(false);
        }
        StartCoroutine(CoroutineMakeObjects());
    }
    IEnumerator CoroutineMakeObjects()
    {
        int i = 0;
        while (true)
        {
            if (i >= scoreItemAmount)
            {
                Debug.Log("Break");
                StopCoroutine(CoroutineMakeObjects());
                yield break;
            }
            GameObject item =
                Instantiate(scoreItemPrefab, objectPoolingPos, Quaternion.identity, this.transform);
            scoreItems.Add(item);
            item.SetActive(false);
            i++;

            yield return null;
        }
    }

    void Start()
    {
        //for (int i = 0; i < 300; i++)
        //{
        //    GameObject item =
        //        Instantiate(scoreItemPrefab, objectPoolingPos, Quaternion.identity, this.transform);
        //    scoreItems.Add(item);
        //    item.SetActive(false);
        //}

        tileMapInfos = new TileMapInfo[gridRoots.Length];

        // 타일맵 인포 삽입작업
        for (int i = 0; i < gridRoots.Length; i++)
        {
            tileMapInfos[i] = new TileMapInfo(gridRoots[i]);
            gridMapIndex.Add(i);
        }

        selected = gridMapIndex[Random.Range(0, gridRoots.Length)];


        Vector3 newPos;

        // Ver 1.1
        selectedMap = Random.Range(0, tileMapInfos[selected].maps.Length);
        MapInfo info = tileMapInfos[selected].maps[selectedMap];
        info.transform.position = new Vector3(0, 0, 0);
        info.gameObject.SetActive(true);
        info.SetItemLocate(scoreItems);
        usedMap++;

        currMap = info;
        currMapTransform = currMap.tileMap.transform;

        // 현재 맵 endType 추출
        MapManager.EntranceType mapEndType = info.GetEndType();
        nextMapIndex.Clear();
        for (int i = 0; i < tileMapInfos[selected].maps.Length; i++)
        {
            if (tileMapInfos[selected].maps[i].GetStartType() == mapEndType)
            {
                // 현재 진행 중인 맵 인덱스는 빼버림
                if (i != selectedMap)
                    nextMapIndex.Add(i);
            }
        }

        int selectedNextMap = nextMapIndex[Random.Range(0, nextMapIndex.Count)];



        newPos = tileMapInfos[selected].maps[selectedMap].tileMap.transform.localPosition +
            tileMapInfos[selected].maps[selectedNextMap].tileMap.size;
        newPos.y = newPos.z = 0;

        tileMapInfos[selected].maps[selectedNextMap].tileMap.transform.localPosition = newPos;
        tileMapInfos[selected].maps[selectedNextMap].tileMap.gameObject.SetActive(true);
        nextMap = tileMapInfos[selected].maps[selectedNextMap];
        nextMap.SetItemLocate(scoreItems);
        nextMapTransform = nextMap.tileMap.transform;
        usedMap++;



        // Ver 1.0
        //for (int i = 0; i < tileMapInfos[selected].maps.Length; i++)
        //{
        //    int newIndex = i;
        //    if (i == 0)
        //    {
        //        newIndex = tileMapInfos[selected].maps.Length;
        //        //Debug.Log("-1" + " // " + tileMapInfos[selected].maps[i].tileMap.size);
        //    }
        //    else
        //    {
        //        newPos = tileMapInfos[selected].maps[newIndex - 1].tileMap.transform.localPosition + 
        //            tileMapInfos[selected].maps[i].tileMap.size;
        //        newPos.y = newPos.z = 0;

        //        tileMapInfos[selected].maps[i].tileMap.transform.localPosition = newPos;
        //        if (i <= 1)
        //            tileMapInfos[selected].maps[i].tileMap.gameObject.SetActive(true);
        //        else
        //        {
        //            tileMapInfos[selected].maps[i].tileMap.gameObject.SetActive(false);
        //        }
        //    }
        //}

        StartCoroutine(CoroutineMoveMap());
    }

    IEnumerator CoroutineMoveMap()
    {
        while (true)
        {
            if (currMap.tileMap.transform.localPosition.x < -currMap.tileMap.size.x * 1.1f)
            {
                usedMap++;
                currMap.gameObject.SetActive(false);

                // 맵 전환 분기
                if (gridRoots.Length == 1)
                {
                   // Debug.Log("Map Ea is 1");
                }
                if (usedMap > tileMapInfos[selected].maps.Length - 1 && gridRoots.Length > 1)
                {
                    Debug.Log("Map Change !");
                    usedMap = 0;
                    int usedIndex = selected;
                    selected = gridMapIndex[Random.Range(0, gridRoots.Length)];
                    gridMapIndex.Remove(selected);
                    gridMapIndex.Add(usedIndex);
                }

                Vector3 newPos;

                newPos = nextMap.tileMap.transform.localPosition + nextMap.tileMap.size;
                newPos.y = newPos.z = 0;

                // 새 맵 활성화
                // 현재 맵 endType 추출
                MapManager.EntranceType mapEndType = nextMap.GetEndType();
                nextMapIndex.Clear();
                for (int i = 0; i < tileMapInfos[selected].maps.Length; i++)
                {
                    if (tileMapInfos[selected].maps[i].GetStartType() == mapEndType)
                    {
                        // 현재 진행 중인 맵 인덱스는 빼버림
                        if (tileMapInfos[selected].maps[i] != nextMap)
                        {
                            //Debug.Log("IN : " + i);
                            nextMapIndex.Add(i);
                        }
                        else
                        {
                            //Debug.Log("OUT : " + i);
                        }
                    }
                }

                currMap = nextMap;
                nextMap = tileMapInfos[selected].maps[nextMapIndex[Random.Range(0, nextMapIndex.Count)]];
                nextMap.transform.localPosition = newPos;
                nextMap.gameObject.SetActive(true);
                nextMap.SetItemLocate(scoreItems);

                currMapTransform = currMap.tileMap.transform;
                nextMapTransform = nextMap.tileMap.transform;
            }

            currMapTransform.Translate(Time.fixedDeltaTime * -1 * speed, 0, 0, Space.Self);

            nextMapTransform.Translate(Time.fixedDeltaTime * -1 * speed, 0, 0, Space.Self);

            //currMap.tileMap.gameObject.transform.
            //    Translate(Time.fixedDeltaTime * -1 * speed, 0, 0, Space.Self);

            //nextMap.tileMap.gameObject.transform.
            //    Translate(Time.fixedDeltaTime * -1 * speed, 0, 0, Space.Self);

            yield return new WaitForFixedUpdate();

            // Ver 1.0
            //for (int i = 0; i < tileMapInfos[selected].maps.Length; i++)
            //{
            //    if (tileMapInfos[selected].maps[i].tileMap.transform.localPosition.x <
            //        -tileMapInfos[selected].maps[i].tileMap.size.x * 1.1f)
            //    {
            //        int newIndex = i;
            //        Vector3 newPos;
            //        if (i == 0)
            //        {
            //            newIndex = tileMapInfos[selected].maps.Length;
            //        }

            //        newPos = tileMapInfos[selected].maps[newIndex - 1].tileMap.transform.localPosition +
            //            tileMapInfos[selected].maps[i].tileMap.size;
            //        newPos.y = newPos.z = 0;
            //        tileMapInfos[selected].maps[i].tileMap.transform.localPosition = newPos;

            //        if (i + 2 >= tileMapInfos[selected].maps.Length)
            //        {
            //            int next = i + 2 - tileMapInfos[selected].maps.Length;
            //            tileMapInfos[selected].maps[next].tileMap.gameObject.SetActive(true);
            //        }
            //        else
            //        {
            //            tileMapInfos[selected].maps[i + 2].tileMap.gameObject.SetActive(true);
            //        }
            //        tileMapInfos[selected].maps[i].tileMap.gameObject.SetActive(false);
            //    }

            //    tileMapInfos[selected].maps[i].tileMap.gameObject.transform.
            //        Translate(Time.fixedDeltaTime * -1 * speed, 0, 0, Space.Self);
            //}
        }
        //while (true)
        //{
        //    for (int i = 0; i < tileMaps.Length; i++)
        //    {
        //        if (tileMaps[i].transform.localPosition.x < -tileMaps[i].size.x * 1.1f)
        //        {
        //            int newIndex = i;
        //            Vector3 newPos;
        //            if (i == 0)
        //            {
        //                newIndex = tileMaps.Length;
        //            }

        //            newPos = tileMaps[newIndex - 1].transform.localPosition + tileMaps[i].size;
        //            newPos.y = newPos.z = 0;
        //            tileMaps[i].transform.localPosition = newPos;

        //            if (i + 2 >= tileMaps.Length)
        //            {
        //                Debug.Log(i);
        //                int next = i + 2 - tileMaps.Length;
        //                tileMaps[next].gameObject.SetActive(true);
        //            }
        //            else
        //            {
        //                tileMaps[i + 2].gameObject.SetActive(true);
        //            }
        //            tileMaps[i].gameObject.SetActive(false);
        //        }

        //        tileMaps[i].transform.Translate(Time.fixedDeltaTime * -2.0f, 0, 0, Space.Self);
        //    }
        //    yield return new WaitForFixedUpdate();
        //}
    }
}
