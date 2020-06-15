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
        public GameObject parent;
        public TileMapInfo(GameObject gridRoot)
        {
            parent = gridRoot;
            maps = gridRoot.GetComponentsInChildren<MapInfo>(true);
        }
    }

    [Header ("- Objects")]
    public GameObject[] gridRoots;
    public TileMapInfo[] tileMapInfos;
    public MapBridge mapBridge;

    [Header("- Map Musics")]
    public AudioClip[] musicBGMs;

    [Space(20)]
    public SpriteRenderer scoreItemPrefab;
    private List<SpriteRenderer> scoreItems = new List<SpriteRenderer>();
    private Vector3 objectPoolingPos = new Vector3(0, 99, 10);

    // 스코어 아이템 스프라이트
    private Sprite scoreSprite;

    [Header("- Object Pools")]
    [Range(300, 500)]
    public int scoreItemAmount = 500;

    [Header("- Speed")]
    public float speed = 2.0f;
    public bool isStop = false;
    private float defaultSpeed = 1.5f;


    // MoveMap values
    MapInfo currMap, nextMap, tempMap;
    Transform currMapTransform, nextMapTransform;

    // Speed Rivision value
    // 맵 한개를 지날때마다 0.02f 증가
    // 추가예정
    private float speedRivision = 1.0f;
    private float speedRivisionAddValue = 0.01f;
    private float speedMaxRivision = 2.0f;
    private float patternRivision = 1.0f;
    private float patternRivisionMin = 0.5f;

    // Score Upgrade Value
    // 맵 x 개를 지났을 때 1 단계 업그레이드
    [Header("- Score Upgrade Value")]
    public int scoreItemLevelPerPassedMap = 1;

    // Index storage
    List<int> nextMapIndex = new List<int>();
    List<int> gridMapIndex = new List<int>();
    int selected = 0;
    int selectedMap = 0;
    int prevSelectedMap = 0;
    int selectedNextMap = 0;
    int usedMap = 0;

    bool isFinalMap = false;
    Coroutine coMoveMap;
    bool gameOver = false;

    SoundManager soundManager;

    //public Tilemap[] tileMaps;
    // Start is called before the first frame update
    private void Awake()
    {
        for (int i = 0; i < 300; i++)
        {
            SpriteRenderer item =
                Instantiate(scoreItemPrefab, objectPoolingPos, Quaternion.identity, this.transform);
            scoreItems.Add(item);
            item.gameObject.SetActive(false);
        }

        if (MainManager.Instance != null)
            speed = MainManager.Instance.GetBaseMapSpeed();
        else
        {
            speed = 1;
        }

        scoreSprite = GameManager.Instance.GetCurrentScoreSprite();
        defaultSpeed = speed;

        soundManager = FindObjectOfType<SoundManager>();

        StartCoroutine(CoroutineMakeObjects());
    }

    // 오브젝트 풀링을 위한 스코어 아이템 미리 생성
    IEnumerator CoroutineMakeObjects()
    {
        int i = 0;
        while (true)
        {
            if (i >= scoreItemAmount)
            {
                //Debug.Log("Break");
                yield break;
            }
            SpriteRenderer item =
                Instantiate(scoreItemPrefab, objectPoolingPos, Quaternion.identity, this.transform);
            scoreItems.Add(item);
            item.gameObject.SetActive(false);
            i++;

            yield return null;
        }
    }

    void Start()
    {
        tileMapInfos = new TileMapInfo[gridRoots.Length];

        // 타일맵 인포 삽입작업
        for (int i = 0; i < gridRoots.Length; i++)
        {
            tileMapInfos[i] = new TileMapInfo(gridRoots[i]);
            gridMapIndex.Add(i);
        }

        selected = gridMapIndex[Random.Range(0, gridRoots.Length)];
        // Debug
        // 레트로, 자연, 흑목, 수중, 동굴
        //selected = 0;
        prevSelectedMap = selected;

        // selected 된 맵 인덱스의 BGM 을 재생한다
        PlayMusic(selected);


        Vector3 newPos;

        // Ver 1.1
        //selectedMap = Random.Range(0, tileMapInfos[selected].maps.Length);
        //MapInfo info = tileMapInfos[selected].maps[selectedMap];

        // Ver 1.2
        selectedMap = Random.Range(0, tileMapInfos[selected].maps.Length);
        MapInfo info = mapBridge.ActiveMapBridge(EntranceType.MIDDLE, EntranceType.MIDDLE, tileMapInfos[selected].parent.tag);

        // 추가
        tileMapInfos[selected].parent.SetActive(true);

        info.transform.localPosition = new Vector3(3.5f, 0, 0);
        info.gameObject.SetActive(true);
        info.SetItemLocate(scoreItems, scoreSprite);
        //usedMap++;

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
        nextMap.SetItemLocate(scoreItems, scoreSprite);
        nextMapTransform = nextMap.tileMap.transform;
        usedMap++;

        if (!isStop)
            coMoveMap = StartCoroutine(CoroutineMoveMap());
    }

    IEnumerator CoroutineMoveMap()
    {
        WaitForFixedUpdate wait = new WaitForFixedUpdate();
        int allPassedMap = 0;
        while (true)
        {
            if (gameOver == false)
            {
                if (currMap.tileMap.transform.localPosition.x < -currMap.tileMap.size.x * 1.1f)
                {
                    usedMap++;
                    allPassedMap++;
                    speedRivision = (speedRivision > speedMaxRivision) ? speedMaxRivision : speedRivision += speedRivisionAddValue;
                    speed = defaultSpeed * speedRivision;

                    patternRivision = (patternRivision < patternRivisionMin) ? patternRivisionMin : patternRivision -= speedRivisionAddValue;

                    if (allPassedMap % scoreItemLevelPerPassedMap == 0)
                    {
                        GameManager.Instance.ScoreUpgrade();
                        scoreSprite = GameManager.Instance.GetCurrentScoreSprite();
                    }


                    // 마지막 맵이 사라지면서 부모 오브젝트도 비활성화 함
                    // 추가
                    currMap.gameObject.SetActive(false);

                    // 맵 전환 분기
                    if (gridRoots.Length == 1)
                    {
                        // Debug.Log("Map Ea is 1");
                    }
                    if (usedMap > tileMapInfos[selected].maps.Length - 1 && gridRoots.Length > 1)
                    {
                        //Debug.Log("Map Change !");
                        usedMap = -1;
                        int usedIndex = selected;
                        prevSelectedMap = usedIndex;
                        selected = gridMapIndex[Random.Range(0, gridRoots.Length)];
                        ///Debug.Log("New Map index : " + selected);
                        gridMapIndex.Remove(selected);
                        gridMapIndex.Add(usedIndex);

                        //StartCoroutine(CorDisablePrevMap(nextMap.gameObject));

                        // 추가
                        isFinalMap = true;
                        // 추가
                        gridRoots[selected].SetActive(true);

                        // selected 된 맵 인덱스의 BGM 을 재생한다
                        PlayMusic(selected);
                    }

                    Vector3 newPos;

                    newPos = nextMap.tileMap.transform.localPosition + nextMap.tileMap.size;
                    newPos.y = newPos.z = 0;

                    // 새 맵 활성화
                    // 마지막 맵이 아닌 경우
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

                    if (isFinalMap == false)
                    {
                        nextMap = tileMapInfos[selected].maps[nextMapIndex[Random.Range(0, nextMapIndex.Count)]];

                        nextMap.transform.localPosition = newPos;
                        nextMap.gameObject.SetActive(true);
                        nextMap.SetItemLocate(scoreItems, scoreSprite);

                        currMapTransform = currMap.tileMap.transform;
                        nextMapTransform = nextMap.tileMap.transform;
                    }
                    else
                    {
                        nextMap = mapBridge.ActiveMapBridge(currMap.GetEndType(), currMap.GetEndType(), gridRoots[selected].tag);
                        nextMap.transform.localPosition = newPos;
                        nextMap.gameObject.SetActive(true);

                        currMapTransform = currMap.tileMap.transform;
                        nextMapTransform = nextMap.transform;

                        isFinalMap = false;

                        if (prevSelectedMap != selected)
                        {
                            //Debug.Log("NotSame : " + prevSelectedMap + " : " + selected);
                            StartCoroutine(CorDisablePrevMap(currMap.gameObject));
                            StartCoroutine(CorDisablePrevMap(nextMap.gameObject));
                        }
                        else
                        {
                            //Debug.Log("Same : " + prevSelectedMap + " : " + selected);
                        }
                    }
                }
                currMapTransform.Translate(Time.fixedDeltaTime * -1 * speed * speedRivision, 0, 0, Space.Self);

                nextMapTransform.Translate(Time.fixedDeltaTime * -1 * speed * speedRivision, 0, 0, Space.Self);

                //currMap.tileMap.gameObject.transform.
                //    Translate(Time.fixedDeltaTime * -1 * speed, 0, 0, Space.Self);

                //nextMap.tileMap.gameObject.transform.
                //    Translate(Time.fixedDeltaTime * -1 * speed, 0, 0, Space.Self);
            }
            yield return wait;
        }
    }

    // 추가
    IEnumerator CorDisablePrevMap(GameObject mapObject)
    {
        WaitForSeconds wait = new WaitForSeconds(1.0f);
        while (true)
        {
            if(mapObject.activeSelf == false)
            {
                //Debug.Log("name : " + mapObject.name);
                mapObject.transform.parent.gameObject.SetActive(false);
                yield break;
            }
            yield return wait;
        }
    }


    public float GetPatternRivision()
    {
        return patternRivision;
    }


    public void GameOver()
    {
        gameOver = true;
        StopCoroutine(coMoveMap);
    }

    private void PlayMusic(int musicIndex)
    {
        if (soundManager == null || musicIndex > musicBGMs.Length - 1) return;
        else
        {
            soundManager.ActiveBGM(musicBGMs[musicIndex]);
        }
    }
}
