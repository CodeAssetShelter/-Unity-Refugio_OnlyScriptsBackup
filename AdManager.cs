using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.UI;
using System;

// Example script showing how to invoke the Google Mobile Ads Unity plugin.
public class AdManager : MonoBehaviour
{
    private static AdManager _instance;
    public static AdManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(AdManager)) as AdManager;

                if (_instance == null)
                {
                    //Debug.LogError("No Active GameManager!");
                }
            }

            return _instance;
        }
    }

    private BannerView bannerView;
    private InterstitialAd interstitial;
    private RewardedAd rewardedAd;
    // private float deltaTime = 0.0f;
    private static string outputMessage = string.Empty;
    // private bool isFirstReward = true;


    private Button rewardButton;
    Coroutine rewardTimer;

    public static string OutputMessage
    {
        set { outputMessage = value; }
    }

    private void Awake()
    {
        rewardButton = GameObject.FindWithTag("UIShop").transform.GetChild(0).Find("Reward Button").GetComponent<Button>();
        rewardButton.onClick.AddListener(() => ShowRewardedAd());

        //GameObject.Find("Button (0)").GetComponent<Button>().onClick.AddListener(() => ShowRewardedAd());
        CreateAndLoadRewardedAd();
        RequestInterstitial();
    }

    public void NewAwake()
    {
        rewardButton = GameObject.FindWithTag("UIShop").transform.GetChild(0).Find("Reward Button").GetComponent<Button>();
        rewardButton.onClick.AddListener(() => ShowRewardedAd());

        if (UnityEngine.Random.Range(0, 100) < 75)
        {
            CreateAndLoadRewardedAd();
            //RequestInterstitial();
            ShowInterstitial();
        }
        rewardTimer = StartCoroutine(RewardTimer());
    }

    public void Start()
    {

#if UNITY_ANDROID
        string appId = "ca-app-pub-9184116991089390~7276697203";
        //string appId = "ca-app-pub-3940256099942544~3347511713";
#elif UNITY_IPHONE
        string appId = "ca-app-pub-3940256099942544~1458002511";
#elif UNITY_EDITOR
        string appId = "ca-app-pub-9184116991089390~7276697203";
#else
        string appId = "unexpected_platform";
#endif

        MobileAds.SetiOSAppPauseOnBackground(true);
        // Initialize the Google Mobile Ads SDK.
        //MobileAds.Initialize(InitializationStatus => { });
        MobileAds.Initialize(appId);
        //rewardTimer = StartCoroutine(RewardTimer());
        //StartCoroutine(IntersitialTimer());
        //Invoke("ShowRewardedAd", 10);

    }
    //System.Collections.IEnumerator IntersitialTimer()
    //{
    //    while (true)
    //    {
    //        if (this.interstitial.IsLoaded() == false)
    //        {
    //            RequestInterstitial();
    //        }
    //        yield return new WaitForSeconds(10.0f);
    //    }
    //}
    //bool createOnce = false;
    System.Collections.IEnumerator RewardTimer()
    {
        if (this.rewardedAd.IsLoaded() == true)
        {
            rewardButton.gameObject.SetActive(true);
        }
        while (true)
        {
            if (rewardButton == null)
            {
                rewardTimer = null;
                yield break;
            }

            if (this.rewardedAd.IsLoaded() == true)
            {
                rewardButton.gameObject.SetActive(true);
                //state = "isLoaded in RewardTimer";
            }
            yield return new WaitForSeconds(1.0f);
        }
        //while (true)
        //{
        //    if (this.rewardedAd.IsLoaded() == true)
        //    {
        //        UICanvas.Instance.ActivateRewardButton(true);
        //        createOnce = false;
        //    }
        //    else
        //    {
        //        if (createOnce == false && this.rewardedAd.IsLoaded() == false)
        //        {
        //            CreateAndLoadRewardedAd();
        //            createOnce = true;
        //        }
        //    }
        //    yield return new WaitForSeconds(2.0f);
        //}
    }
    public void GameStartAndStopCoroutine()
    {
        //StopCoroutine(rewardTimer);
        rewardTimer = null;
    }

    //string state = "none";
    //string state2 = "none";
    //public void OnGUI()
    //{
    //    GUIStyle style = new GUIStyle();

    //    Rect rect = new Rect(0, 0, Screen.width, Screen.height);
    //    style.fontSize = (int)(Screen.height * 0.03);
    //    style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    //    //float fps = 1.0f / this.deltaTime;
    //    string text = string.Format("{0:0.} status", state);
    //    GUI.Label(rect, text, style);

    //    Rect rect2 = new Rect(0, Screen.height * 0.06f, Screen.width, Screen.height);
    //    style.fontSize = (int)(Screen.height * 0.03);
    //    style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    //    //float fps = 1.0f / this.deltaTime;
    //    string text2 = string.Format("{0:0.} status", state2);
    //    GUI.Label(rect2, text2, style);
    //}

    //public Text st1, st2;
    //string state = "0", state2 = "0";
    //public void Update()
    //{
    //    // Calculate simple moving average for time to render screen. 0.1 factor used as smoothing
    //    // value.
    //    st1.text = state;
    //    st2.text = state2;
    //    //this.deltaTime += (Time.deltaTime - this.deltaTime) * 0.1f;
    //}

    //public void OnGUI()
    //{
    //    GUIStyle style = new GUIStyle();

    //    Rect rect = new Rect(0, 0, Screen.width, Screen.height);r
    //        style.fontSize = (int) (Screen.height* 0.06);
    //        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
    //    float fps = 1.0f / this.deltaTime;
    //    string text = string.Format("{0:0.} fps", fps);
    //    GUI.Label(rect, text, style);

    //// Puts some basic buttons onto the screen.
    //GUI.skin.button.fontSize = (int)(0.035f * Screen.width);
    //float buttonWidth = 0.35f * Screen.width;
    //float buttonHeight = 0.15f * Screen.height;
    //float columnOnePosition = 0.1f * Screen.width;
    //float columnTwoPosition = 0.55f * Screen.width;

    //Rect requestBannerRect = new Rect(
    //    columnOnePosition,
    //    0.05f * Screen.height,
    //    buttonWidth,
    //    buttonHeight);
    //if (GUI.Button(requestBannerRect, "Request\nBanner"))
    //{
    //    this.RequestBanner();
    //}

    //Rect destroyBannerRect = new Rect(
    //    columnOnePosition,
    //    0.225f * Screen.height,
    //    buttonWidth,
    //    buttonHeight);
    //if (GUI.Button(destroyBannerRect, "Destroy\nBanner"))
    //{
    //    this.bannerView.Destroy();
    //}

    //Rect requestInterstitialRect = new Rect(
    //    columnOnePosition,
    //    0.4f * Screen.height,
    //    buttonWidth,
    //    buttonHeight);
    //if (GUI.Button(requestInterstitialRect, "Request\nInterstitial"))
    //{
    //    this.RequestInterstitial();
    //}

    //Rect showInterstitialRect = new Rect(
    //    columnOnePosition,
    //    0.575f * Screen.height,
    //    buttonWidth,
    //    buttonHeight);
    //if (GUI.Button(showInterstitialRect, "Show\nInterstitial"))
    //{
    //    this.ShowInterstitial();
    //}

    //Rect destroyInterstitialRect = new Rect(
    //    columnOnePosition,
    //    0.75f * Screen.height,
    //    buttonWidth,
    //    buttonHeight);
    //if (GUI.Button(destroyInterstitialRect, "Destroy\nInterstitial"))
    //{
    //    this.interstitial.Destroy();
    //}

    //Rect requestRewardedRect = new Rect(
    //    columnTwoPosition,
    //    0.05f * Screen.height,
    //    buttonWidth,
    //    buttonHeight);
    //if (GUI.Button(requestRewardedRect, "Request\nRewarded Ad"))
    //{
    //    this.CreateAndLoadRewardedAd();
    //}

    //Rect showRewardedRect = new Rect(
    //    columnTwoPosition,
    //    0.225f * Screen.height,
    //    buttonWidth,
    //    buttonHeight);
    //if (GUI.Button(showRewardedRect, "Show\nRewarded Ad"))
    //{
    //    this.ShowRewardedAd();
    //}

    //Rect textOutputRect = new Rect(
    //    columnTwoPosition,
    //    0.925f * Screen.height,
    //    buttonWidth,
    //    0.05f * Screen.height);
    //GUI.Label(textOutputRect, outputMessage);
    //}

    // Returns an ad request with custom ad targeting.
    private AdRequest CreateAdRequest()
    {
        //return new AdRequest.Builder().Build();

        return new AdRequest.Builder()
            .AddKeyword("game")
            .AddExtra("color_bg", "9B30FF")
            .Build();
    }

    public void RequestBanner()
    {
        // These ad units are configured to always serve test ads.
#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Clean up banner ad before creating a new one.
        if (this.bannerView != null)
        {
            this.bannerView.Destroy();
        }

        // Create a 320x50 banner at the top of the screen.
        this.bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);

        // Register for ad events.
        this.bannerView.OnAdLoaded += this.HandleAdLoaded;
        this.bannerView.OnAdFailedToLoad += this.HandleAdFailedToLoad;
        this.bannerView.OnAdOpening += this.HandleAdOpened;
        this.bannerView.OnAdClosed += this.HandleAdClosed;
        this.bannerView.OnAdLeavingApplication += this.HandleAdLeftApplication;

        // Load a banner ad.
        this.bannerView.LoadAd(this.CreateAdRequest());
    }
    public void DestroyBannerView()
    {
        if (this.bannerView != null)
        {
            this.bannerView.Destroy();
        }
    }


    // 전면 광고
    public void RequestInterstitial()
    {
        // These ad units are configured to always serve test ads.
#if UNITY_EDITOR
        string adUnitId = "ca-app-pub-9184116991089390/8972728738";
#elif UNITY_ANDROID
        //string adUnitId = "ca-app-pub-9184116991089390/8972728738";
        string adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Clean up interstitial ad before creating a new one.
        if (this.interstitial != null)
        {
            this.interstitial.Destroy();
        }

        // Create an interstitial.
        this.interstitial = new InterstitialAd(adUnitId);

        // Register for ad events.
        this.interstitial.OnAdLoaded += this.HandleInterstitialLoaded;
        this.interstitial.OnAdFailedToLoad += this.HandleInterstitialFailedToLoad;
        this.interstitial.OnAdOpening += this.HandleInterstitialOpened;
        this.interstitial.OnAdClosed += this.HandleInterstitialClosed;
        this.interstitial.OnAdLeavingApplication += this.HandleInterstitialLeftApplication;

        // Load an interstitial ad.
        this.interstitial.LoadAd(this.CreateAdRequest());
    }

    public void CreateAndLoadRewardedAd()
    {
#if UNITY_EDITOR
        string adUnitId = "ca-app-pub-9184116991089390/8781157049";
#elif UNITY_ANDROID
        string adUnitId = "ca-app-pub-9184116991089390/8781157049";
        //string adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
        string adUnitId = "unexpected_platform";
#endif
        // Create new rewarded ad instance.
        this.rewardedAd = new RewardedAd(adUnitId);
        //try
        //{
        //    Debug.Log("reward state : " + this.rewardedAd);
        //}
        //catch (NullReferenceException ie)
        //{
        //    Debug.Log("Error : " + ie);
        //    rewardedAd = new RewardedAd(adUnitId);
        //}

        // Called when an ad request has successfully loaded.
        this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = this.CreateAdRequest();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);
        //state2 = "Create";
    }

    public void ShowInterstitial()
    {
        if (this.interstitial.IsLoaded())
        {
            this.interstitial.Show();
            return;
        }
        else
        {
            RequestInterstitial();
            MonoBehaviour.print("Interstitial is not ready yet");
        }
        StartCoroutine(CorShowInterstitial());
    }

    WaitForSeconds wait = new WaitForSeconds(0.5f);
    System.Collections.IEnumerator CorShowInterstitial()
    {
        while (true)
        {
            if (this.interstitial.IsLoaded())
            {
                this.interstitial.Show();
                yield break;
            }
            yield return wait;
        }
    }

    public void ShowRewardedAd()
    {
        //try
        //{
        //    if (this.rewardedAd == null)
        //    {
        //        Debug.Log("Show's rewardedAd is Null");
        //    }
        //}
        //catch (NullReferenceException ie)
        //{
        //    Debug.Log("Show Error : " + ie);
        //    this.rewardedAd = new RewardedAd("ca-app-pub-3940256099942544/5224354917");
        //    Debug.Log("Create 9");
        //    AdRequest request = this.CreateAdRequest();
        //    // Load the rewarded ad with the request.
        //    Debug.Log("Create 10");
        //    this.rewardedAd.LoadAd(request);
        //}
        if (this.rewardedAd.IsLoaded())
        {
           this.rewardedAd.Show();
           //state2 = "Show";
        }
        else
        {
            MonoBehaviour.print("Rewarded ad is not ready yet");
        }
    }

    #region Banner callback handlers

    public void HandleAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
    }

    public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: " + args.Message);
    }

    public void HandleAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void HandleAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
    }

    public void HandleAdLeftApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeftApplication event received");
    }

    #endregion

    #region Interstitial callback handlers

    public void HandleInterstitialLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleInterstitialLoaded event received");
    }

    public void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print(
            "HandleInterstitialFailedToLoad event received with message: " + args.Message);
    }

    public void HandleInterstitialOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleInterstitialOpened event received");
    }

    public void HandleInterstitialClosed(object sender, EventArgs args)
    {
        RequestInterstitial();
        MonoBehaviour.print("HandleInterstitialClosed event received");
    }

    public void HandleInterstitialLeftApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleInterstitialLeftApplication event received");
    }

    #endregion

    #region RewardedAd callback handlers

    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        rewardButton.gameObject.SetActive(true);
        //state2 = "HandleLoaded";
        //MonoBehaviour.print("HandleRewardedAdLoaded event received");
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
    {
        //state2 = "HandleFailToLoad";
        //MonoBehaviour.print(
        //    "HandleRewardedAdFailedToLoad event received with message: " + args.Message);
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        //state2 = "adOpening";
        //MonoBehaviour.print("HandleRewardedAdOpening event received");
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        //state2 = "FailToShow";
        //MonoBehaviour.print(
        //    "HandleRewardedAdFailedToShow event received with message: " + args.Message);
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        //state2 = "AdClosed";
        CreateAndLoadRewardedAd();
        //MonoBehaviour.print("HandleRewardedAdClosed event received");
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;

        if (type == "Coin")
        {
            MainManager.Instance.SetMyCoin(MainManager.Instance.GetMyCoin() + (int)amount);
            ShopManager shop = GameObject.FindWithTag("UIShop").GetComponent<ShopManager>();
            if (shop != null)
            {
                shop.RefreshCoin();
                shop.resultWindow.InitAdsResult((int)amount);
            }
            //PublicValueStorage.Instance.RefreshCredit((int)amount);
        }

        //state2 = "Earned";
        //MonoBehaviour.print(
        //    "HandleRewardedAdRewarded event received for "
        //                + amount.ToString() + " " + type);
    }

    #endregion
}