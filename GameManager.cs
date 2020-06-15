using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using TMPro;
using CodeStage.AntiCheat.ObscuredTypes;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(GameManager)) as GameManager;

                if (_instance == null)
                {
                    //Debug.LogError("No Active GameManager!");
                }
            }

            return _instance;
        }
    }

    public enum ItemType { Coins = 0, SpeedDown, CoinBox, Invincible, Minimalize, SlowMotion, StateCount, LifePotion }

    // 지금은 GM 에 다 때려넣지만
    // 차후에는 Interface 이용해서 관리자 없이 효과를 낼 수 있게 제작
    [System.Serializable]
    public class ItemInfo
    {
        public string name;
        public ItemType type;
        [Range(0, 100)]
        public int chance;
        public float duration = 0.5f;
        public float effectValue = 1.0f;

        public AudioClip audioClip;

        public bool useImmediately = false;
        public bool isParticling = false;

        public Coroutine corUsingItem { get; set; } = null;
        public Coroutine corUsingItemDisplay { get; set; } = null;
    }

    [Header("- Camera effect for UseItem")]
    public PostProcessVolume postProcessVolume;

    [Header("- Item Displayer")]
    public Transform usingItemDisplayer;
    public Image usingItemDisplayHolderPrefab;
    Dictionary<ItemType, Image> usingItemDisplayHolders;

    /* Smoothing Item Using Effect values (Not use)
    private LensDistortion ppLensDistortion;
    private ColorGrading ppColorGrading;
    private float ppColorGradingSaturationValue = -100f;
    private Vignette ppVignette;
    private float ppVignetteActiveValue = 0.2f;
    */

    [Header("- Item Effects & UI")]
    public CoinBox coinBoxPrefab;
    public Transform coinPacker;
    private List<CoinBox> coinBoxes;
    public Text earnedCoinText;
    public GameObject screenBlackFog;

    [Header("- Item Chance & Sprites")]
    public ItemInfo[] itemInfos;
    public Sprite[] itemSprite;


    [Header("- TimeScale")]
    public ObscuredFloat timeScale = 1.0f;
    private ObscuredInt scoreItemValue = 10;

    // Score Value
    [Header("- Score & Coin Values")]
    public Text scoreText;
    public Sprite[] scoreSprites;
    public int currentScoreIndex = 0;
    private ObscuredInt myScore = 0;
    private ObscuredInt myEarnedCoin = 1;
    private ObscuredInt myDisplayCoin = 1;


    [Header("- Sound")]
    public AudioSource audioSource;
    public AudioClip audioClipGetItem;

    ShopManager.ItemInfo playerInfo;

    // Start is called before the first frame update
    void OnEnable()
    {
        if (MainManager.Instance != null)
            MainManager.Instance.ActivePauseButton();
        scoreText.text = "" + 0;
        
        /* Smoothing Item Using Effect Values (Not Use)
        ppLensDistortion = postProcessVolume.profile.GetSetting<LensDistortion>();
        ppVignette = postProcessVolume.profile.GetSetting<Vignette>();
        ppColorGrading = postProcessVolume.profile.GetSetting<ColorGrading>();
        */

        // playerInfo = MainManager.Instance.GetPlayerInfo();

        //SetPlaySettings();

        // 임시
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<ControllerPlayer>();

        InvokeRepeating("RandomizeKeys", 1, 1);

        coinBoxes = new List<CoinBox>();
        usingItemDisplayHolders = new Dictionary<ItemType, Image>();
        earnedCoinText.text = "" + 0;

        SetPlaySettings();

        StartCoroutine(CorPassiveAddScore());
        StartCoroutine(CorEarnCoinBoard());
    }


    int refreshMultifly = 1;
    IEnumerator CorEarnCoinBoard()
    {
        WaitForFixedUpdate wait = new WaitForFixedUpdate();
        WaitForSeconds waitLong = new WaitForSeconds(1.0f);

        StartCoroutine(CorRefreshMultifly());
        while (true)
        {
            if (myEarnedCoin != myDisplayCoin)
            {                
                myDisplayCoin += 11 * refreshMultifly;

                if (myDisplayCoin >= myEarnedCoin)
                {
                    myDisplayCoin = myEarnedCoin;
                }

                earnedCoinText.text = "" + myDisplayCoin;
                yield return wait;
            }
            else
                yield return waitLong;
        }
    }
    IEnumerator CorRefreshMultifly()
    {
        WaitForSeconds waitLong = new WaitForSeconds(1.5f);
        float myE = 0, myD = 0;
        while (true)
        {
            myE = Mathf.Log10((float)myEarnedCoin);
            myD = Mathf.Log10((float)myDisplayCoin);

            refreshMultifly = (int)myE - (int)myD + 1;
            yield return waitLong;
        }
    }

    IEnumerator CorPassiveAddScore()
    {
        ObscuredFloat timer = 0.5f;
        WaitForSeconds wait = new WaitForSeconds(timer);
        while (true)
        {
            myScore += 1;
            scoreText.text = "" + myScore;
            yield return wait;
        }
    }

    private void RandomizeKeys()
    {
        scoreItemValue.RandomizeCryptoKey();
        timeScale.RandomizeCryptoKey();
        myScore.RandomizeCryptoKey();
        myEarnedCoin.RandomizeCryptoKey();
    }

    public Sprite GetCurrentScoreSprite()
    {
        return scoreSprites[currentScoreIndex];
    }


    // 플레이 중인 플레이어 정보를 새로이 전달함
    //public ItemSlotManager itemSlotManager;
    //public void SetItemSlot(int itemType)
    //{
    //    itemSlotManager.SetItemSlots(itemSprite[itemType], ref itemType);
    //}
    //public ItemSlotManager itemSlotManager;
    ControllerPlayer player;
    public void SetPlaySettings()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<ControllerPlayer>();
        if (MainManager.Instance != null)
        {
            playerInfo = MainManager.Instance.GetPlayerInfo();
            player.SetPlayerInfo(playerInfo.shieldDuration, MainManager.Instance.GetPlayerSprite(), playerInfo.itemSlot, playerInfo.lifeTime);
        }
        else
        {
            Sprite mySpr = Sprite.Create(Texture2D.whiteTexture, new Rect(Vector2.zero, new Vector2(1, 1)), Vector2.zero);
            Sprite[] spr = { mySpr, mySpr, mySpr };
            playerInfo = new ShopManager.ItemInfo();
            playerInfo.lifeTime = 600;
            player.SetPlayerInfo(playerInfo.shieldDuration, spr, playerInfo.itemSlot, playerInfo.lifeTime);
        }
    }

    public void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }

    //public void OnGUI()
    //{
    //    GUIStyle style = new GUIStyle();

    //    Rect rect = new Rect(0, 0, Screen.width, Screen.height);
    //    style.fontSize = (int)(Screen.height * 0.06);
    //    style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    //    //float fps = 1.0f / Time.deltaTime;
    //    float fps = Time.realtimeSinceStartup;
    //    string text = string.Format("{0:0.}", fps);
    //    GUI.Label(rect, text, style);
    //}

    public void ScoreAdd(int rivision = 1)
    {
        if (rivision == 1)
        {
            float effectVolume;
            effectVolume = (SoundManager.Instance == null) ? 1 : SoundManager.Instance.effectVolume * 0.2f;
            audioSource.PlayOneShot(audioClipGetItem, effectVolume);
        }
        // Add Time Bonus
        if (rivision == -1)
        {
            float effectVolume;
            effectVolume = (SoundManager.Instance == null) ? 1 : SoundManager.Instance.effectVolume * 0.2f;
            audioSource.PlayOneShot(audioClipGetItem, effectVolume);
            rivision = 1;

            if (player != null)
            {
                player.playerTimeBar.AddTimerBonus(2);
            }
        }
        myScore += scoreItemValue * rivision;
        scoreText.text = "" + myScore;
    }

    public void ScoreUpgrade()
    {
        scoreItemValue++;
        if (currentScoreIndex >= scoreSprites.Length - 1)
        {
            currentScoreIndex = scoreSprites.Length - 1;
        }
        else
            currentScoreIndex++;
    }


    //                                                  //
    //                                                  //
    //                                                  //
    //                                                  //
    //                                                  //
    //   Item Methods                                   //
    //                                                  //
    //                                                  //
    //                                                  //
    //                                                  //

    public Sprite GetItemType(ref int typeNumber, ParticleSystem particle)
    {
        if (itemSprite == null) return null;

        // Not const typeNumber
        if (typeNumber <= -1)
        {
            int output = Random.Range(0, 100);
            int addPrevChance = 0;
            for (int i = 0; i < (int)ItemType.StateCount; i++)
            {
                if (output < itemInfos[i].chance + addPrevChance)
                {
                    typeNumber = i;
                    if (itemInfos[i].isParticling == true) particle.Play();
                    else particle.Stop();

                    return itemSprite[i];
                }
                addPrevChance += itemInfos[i].chance;
            }
        }
        else
        {
            // Const Type
            if (typeNumber < (int)ItemType.StateCount)
            {
                if (itemInfos[typeNumber].isParticling == true) particle.Play();
                else particle.Stop();
                return itemSprite[typeNumber];
            }
            else
            {
                if (itemInfos[typeNumber - 1].isParticling == true) particle.Play();
                else particle.Stop();
                return itemSprite[typeNumber - 1];
            }
        }

        return itemSprite[0];
    }

    public void SetItemSlot(int itemType)
    {
        if (player != null)
            player.playerTimeBar.AddTimerBonus(2);

        if (itemType > (int)ItemType.StateCount) itemType -= 1;
        if (itemInfos[itemType].useImmediately == true)
        {
            itemInfos[itemType].corUsingItem = StartCoroutine(CorUsingItem(itemInfos[itemType]));
        }
        else
        {
            player.SetItem(itemSprite[itemType], itemType);            
        }
    }

    public void UseItem(int itemType)
    {
        if (itemInfos[itemType].corUsingItem != null)
        {
            StopCoroutine(itemInfos[itemType].corUsingItem);
        }
        itemInfos[itemType].corUsingItem = StartCoroutine(CorUsingItem(itemInfos[itemType]));
    }

    public void GameOverItemAllStop()
    {
        for(int i = 0; i < itemInfos.Length; i++)
        {
            if (itemInfos[i].corUsingItem != null)
            {
                StopCoroutine(itemInfos[i].corUsingItem);
                itemInfos[i].corUsingItem = StartCoroutine(CorUsingItem(itemInfos[i], true));
            }
        }
    }

    float origValue = 1.0f;
    IEnumerator CorUsingItem(ItemInfo info, bool allStop = false)
    {
        //Debug.Log("Use : " + info.name);
        float timer = 0;


        if (allStop == false)
        {
            float effectVolume;
            effectVolume = (SoundManager.Instance == null) ? 1 : SoundManager.Instance.effectVolume;
            audioSource.PlayOneShot(info.audioClip, effectVolume);

            // Active
            switch (info.type)
            {
                // 즉발
                default:
                case ItemType.Coins:
                    myEarnedCoin += (int)info.effectValue;
                    yield break;

                case ItemType.CoinBox:
                    SetEarnedCoinBox((int)info.duration, (int)info.effectValue);
                    yield break;

                case ItemType.LifePotion:
                    player.playerTimeBar.AddTimerBonus(info.effectValue);
                    yield break;

                // 이하 액티브
                case ItemType.Invincible:
                    player.ActiveItemInvincible(info.duration);
                    break;

                case ItemType.Minimalize:
                    player.spriteBodyRenderer.transform.localScale = new Vector3(info.effectValue, info.effectValue, 1);
                    break;

                case ItemType.SpeedDown:
                    origValue = MapManager.Instance.speed;
                    MapManager.Instance.speed *= info.effectValue;
                    break;

                case ItemType.SlowMotion:
                    Time.timeScale = info.effectValue;
                    ItemPostProcess(true);
                    break;
            }

            Image holder;
            if (usingItemDisplayHolders.ContainsKey(info.type) == false)
            {
                holder = Instantiate(usingItemDisplayHolderPrefab, usingItemDisplayer);
                holder.sprite = itemSprite[(int)info.type];
                usingItemDisplayHolders.Add(info.type, holder);
            }
            else
            {
                holder = usingItemDisplayHolders[info.type];
            }
            holder.fillAmount = 1;
            holder.gameObject.SetActive(true);


            // Timer
            while (timer < info.duration)
            {
                timer += Time.deltaTime;
                holder.fillAmount -= Time.deltaTime / info.duration;

                // 유지해야만 하는 아이템의 경우 여기에 넣어서 유지한다
                switch (info.type)
                {
                    case ItemType.SlowMotion:
                        if (Time.timeScale != 0)
                            Time.timeScale = info.effectValue;
                        break;
                }

                yield return Time.deltaTime;
            }
            holder.fillAmount = 0;

            //Debug.Log(Time.deltaTime);
            //Debug.Log(timer);

            holder.gameObject.SetActive(false);
        }
        else
        {
            usingItemDisplayHolders.Clear();
        }
        // Deavtive
        switch (info.type)
        {
            // 이하 액티브
            case ItemType.Invincible:
                player.playerStatus.invincible = false;
                break;

            case ItemType.Minimalize:
                player.spriteBodyRenderer.transform.localScale = new Vector3(1, 1, 1);
                break;

            case ItemType.SpeedDown:
                MapManager.Instance.speed = origValue;
                break;

            case ItemType.SlowMotion:
                Time.timeScale = 1;
                ItemPostProcess(false);
                break;
        }
        //Debug.Log("END : " + info.name);
    }


    private void SetEarnedCoinBox(int value, int effectValue)
    {
        CoinBox box;
        int earnCoin = value * Random.Range(1, effectValue + 1);
        if (coinBoxes.Count == 0)
        {
            box = Instantiate(coinBoxPrefab, coinPacker);
            box.SetDisplayEarnCoin(earnCoin);
            myEarnedCoin += earnCoin;
            coinBoxes.Add(box);
        }
        else
        {
            box = coinBoxes.Find(obj => obj.gameObject.activeSelf == false);
            // Found any deactive object
            if (box != null)
            {
                box.SetDisplayEarnCoin(earnCoin);
                myEarnedCoin += earnCoin;
            }
            else
            {
                box = Instantiate(coinBoxPrefab, coinPacker);
                box.SetDisplayEarnCoin(earnCoin);
                coinBoxes.Add(box);
                myEarnedCoin += earnCoin;
            }
        }
    }

    private void ItemPostProcess(bool active)
    {
        for (int i = 0; i < postProcessVolume.profile.settings.Count; i++)
            postProcessVolume.profile.settings[i].active = active;
    }


    ////////////////////////////////
    ////////////////////////////////
    ////////////////////////////////
    ////////////////////////////////
    //          Trigger Methods   //
    /////////////////////////////////
    ////////////////////////////////
    //////////////////////////////////
    ///////////////////////////////
    ////////////////////////////////
    ///

    public void GameOver()
    {

        MapManager.Instance.GameOver();
        GameOverItemAllStop();
        screenBlackFog.SetActive(true);

        if (MainManager.Instance != null)
        {
            MainManager.Instance.ApplyGameResult(myEarnedCoin, myScore);
            MainManager.Instance.SetOptions(false);
            MainManager.Instance.LoadSceneMainMenu();
        }
    }
}
