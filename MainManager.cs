using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using CodeStage.AntiCheat.ObscuredTypes; 

public class MainManager : MonoBehaviour
{
    private static MainManager _instance;
    public static MainManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(MainManager)) as MainManager;

                if (_instance == null)
                {
                    //Debug.LogError("No Active!");
                }
            }
            return _instance;
        }
    }

    public GameObject buttonPause;
    public Options options;

    [SerializeField]
    private ObscuredInt myCoins = 100;
    private ObscuredInt myScore = 0;
    private ObscuredInt myBestScore = 0;

    [SerializeField]
    private ObscuredFloat baseMapSpeed = 1.5f;

    private DisplayPlayer player;
    private ShopManager.ItemInfo gamePlayerGoodsInfo;
    private Sprite[] playerSprites;


    private Text textMyCoin;
    private Text textMyScore;

    void Awake()
    {
        if (_instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this);
        _instance = this;

        
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            DisableSystemUI.DisableNavUI();
        }
    }

    public void ReLoad(string sceneName)
    {
        StartCoroutine(CorReLoad(sceneName));
    }
    IEnumerator CorReLoad(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
        {
            yield return null;
        }

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<DisplayPlayer>();
        textMyCoin = GameObject.Find("TextMyCoins").GetComponent<Text>();
        textMyScore = GameObject.Find("TextMyScore").GetComponent<Text>();

        bool load = SaveData.Instance.LoadUserData();
        //Debug.Log("SAVE : " + load);
        if (load == true)
        {
            myBestScore = SaveData.Instance.LoadBestScore();
            myCoins = SaveData.Instance.LoadCoins();
        }
        else
        {
            myBestScore = 0;
            myCoins = 200;
            SaveData.Instance.SaveUserData(myCoins, myBestScore);
        }

        textMyScore.text = "" + myBestScore;

        AdManager.Instance.NewAwake();
        options.SetOptionMode(false);
    }
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<DisplayPlayer>();        
        textMyCoin = GameObject.Find("TextMyCoins").GetComponent<Text>();
        textMyScore = GameObject.Find("TextMyScore").GetComponent<Text>();

        bool load = SaveData.Instance.LoadUserData();
        //Debug.Log("SAVE : " + load);
        if (load == true)
        {
            myBestScore = SaveData.Instance.LoadBestScore();
            myCoins = SaveData.Instance.LoadCoins();
        }
        else
        {
            myBestScore = 0;
            myCoins = 200;
        }

        textMyScore.text = "" + myBestScore;

        InvokeRepeating("RandomizeKeys", 1, 1);
    }
    private void RandomizeKeys()
    {
        myCoins.RandomizeCryptoKey();
        myBestScore.RandomizeCryptoKey();
        myScore.RandomizeCryptoKey();
    }

    public void SetActiveText(bool active)
    {
        if (textMyCoin == null) textMyCoin = GameObject.Find("TextMyCoins").GetComponent<Text>();
        if (textMyScore == null) textMyScore = GameObject.Find("TextMyScore").GetComponent<Text>();
        textMyScore.transform.parent.gameObject.SetActive(active);
        textMyCoin.transform.parent.gameObject.SetActive(active);
    }
    public ref ObscuredInt GetMyCoin()
    {
        return ref myCoins;
    }
    public void SetMyCoin(ObscuredInt value)
    {
        myCoins = value;
    }

    public void SetDataFromCloudSave(ref SaveData.CloudGameData data)
    {
        if (textMyCoin == null) textMyCoin = GameObject.Find("TextMyCoins").GetComponent<Text>();
        if (textMyScore == null) textMyScore = GameObject.Find("TextMyScore").GetComponent<Text>();

        myCoins = data.userData.coins;
        myScore = data.userData.bestScore;
        textMyCoin.text = "" + myCoins;
        textMyScore.text = "" + myScore;
    }

    public bool ApplyGameResult(ObscuredInt earnCoins, ObscuredInt score)
    {
        if (earnCoins < 0 || score < 0) return false;

        myCoins += earnCoins;
        if (score > myBestScore)
        {
            myBestScore = score;
        }

        SaveData.Instance.SaveUserData(myCoins, myBestScore);

        return true;
    }

    public void ChangeSceneGamePlay()
    {
        gamePlayerGoodsInfo = ShopManager.Instance.GetSelectedPlayerInfo();
        LoadSceneManager.LoadScene(LoadSceneManager.sceneNames.gamePlay, playerSprites);
        AdManager.Instance.GameStartAndStopCoroutine();
        //SceneManager.LoadScene("GamePlay", LoadSceneMode.Single);
    }

    public void ActivePauseButton()
    {
        if (buttonPause.activeSelf == false)
            buttonPause.SetActive(true);
        SetOptions(true);
    }
    public void SetOptions(bool isGamePlay)
    {
        buttonPause.SetActive(isGamePlay);
        options.SetOptionMode(isGamePlay);
    }

    public void Pause(float value)
    {
        if (value > 1) value = 1;
        Time.timeScale = value;
    }

    public float GetBaseMapSpeed()
    {
        return baseMapSpeed;
    }

    public void ChangePlayer(Sprite[] sprites, ShopManager.ItemInfo itemInfo)
    {
        gamePlayerGoodsInfo = itemInfo;
        playerSprites = sprites;
        player.SetSprites(sprites);
    }
    public void ChangePlayerSprite(Sprite[] sprite)
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").GetComponent<DisplayPlayer>();
        playerSprites = sprite;
        player.SetSprites(sprite);
    }

    public void ReloadSettings()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<DisplayPlayer>();
        textMyCoin = GameObject.Find("TextMyCoins").GetComponent<Text>();
        textMyScore = GameObject.Find("TextMyScore").GetComponent<Text>();

        bool load = SaveData.Instance.LoadUserData();
        //Debug.Log("SAVE : " + load);
        if (load == true)
        {
            myBestScore = SaveData.Instance.LoadBestScore();
            myCoins = SaveData.Instance.LoadCoins();
        }
        else
        {
            myBestScore = 0;
            myCoins = 200;
            SaveData.Instance.SaveUserData(myCoins, myBestScore);
        }

        textMyScore.text = "" + myBestScore;

        AdManager.Instance.NewAwake();
        options.SetOptionMode(false);
    }
    public void LoadSceneMainMenu()
    {
        LoadSceneManager.LoadScene(LoadSceneManager.sceneNames.mainMenu,null,true);
    }



    public ShopManager.ItemInfo GetPlayerInfo()
    {
        return gamePlayerGoodsInfo;
    }
    public void SetPlayerSprites(Sprite[] sprites)
    {
        playerSprites = sprites;
    }
    public Sprite[] GetPlayerSprite()
    {
        return playerSprites;
    }
}
