using UnityEngine;
using System;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System.Text;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using Newtonsoft.Json;

public class GoogleCloudManager : MonoBehaviour
{
    private static GoogleCloudManager instance;
    public static GoogleCloudManager Instance    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GoogleCloudManager>();
                if (instance == null)
                {
                    instance = new GameObject("PlayGameCloudData").AddComponent<GoogleCloudManager>();
                }
            }
            return instance;
        }
    }

    public bool isProcessing
    {
        get;
        private set;
    }

    public string loadedData
    {
        get;
        private set;
    }

    private const string m_saveFileName = "game_save_data";

    public GPGSConfirmWindow gpgs;
    public GameObject loadingBar;
    private bool isApplyToLocal = true;

    public bool isAuthenticated
    {
        get
        {
            return Social.localUser.authenticated;
        }
    }

    private void ActiveWaitScreen(float time = 3.0f)
    {
        loadingBar.SetActive(true);
        if (time > 1)
            Invoke("DeactiveWaitScreen", time);
    }
    private void DeactiveWaitScreen()
    {
        loadingBar.SetActive(false);
    }

    private void InitiatePlayGames()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        // enables saving game progress.
        .EnableSavedGames()
        .Build();

        PlayGamesPlatform.InitializeInstance(config);
        // recommended for debugging:
        PlayGamesPlatform.DebugLogEnabled = true;
        // Activate the Google Play Games platform
        PlayGamesPlatform.Activate();
    }

    private void Awake()
    {
        InitiatePlayGames();

    }

    public void PrintData(SaveData.CloudGameData data)
    {
        Debug.Log("Score " + data.userData.bestScore);
        Debug.Log("Coin " + data.userData.coins);

        foreach (var p in data.purchasedData)
        {
            string detail = 
                string.Format(
                    " Name : {0} \n life : {1} \n" +
                    " Slot : {2} \n Shield : {3} \n" +
                    " Price : {4} \n Purchased : {5} \n" +
                    " Selected : {6}", 
                    p.name, p.lifeTime, p.itemSlot, p.shieldDuration,
                    p.price, p.purchased, p.selected);
            Debug.Log(detail);
        }
    }

    public void PrintData(System.Collections.Generic.List<ShopManager.ItemInfo> data)
    {

        foreach (var p in data)
        {
            string detail =
                string.Format(
                    " Name : {0} \n life : {1} \n" +
                    " Slot : {2} \n Shield : {3} \n" +
                    " Price : {4} \n Purchased : {5} \n" +
                    " Selected : {6}",
                    p.name, p.lifeTime, p.itemSlot, p.shieldDuration,
                    p.price, p.purchased, p.selected);
            Debug.Log(detail);
        }
    }

    public void Login()
    {
        ActiveWaitScreen(-1);
        Social.localUser.Authenticate((bool success) =>
        {
            isApplyToLocal = false;
            LoadFromCloud();
            if (!success)
            {
                //Debug.Log("Fail Login");
            }            
        });
    }

    public void Logout()
    {
        ActiveWaitScreen();
        if (Social.localUser.authenticated)
        {
            ((GooglePlayGames.PlayGamesPlatform)Social.Active).SignOut();
        }
    }

    private void ProcessCloudData(byte[] cloudData)
    {
        if (cloudData == null)
        {
            //Debug.Log("No Data saved to the cloud");
            return;
        }

        string progress = BytesToString(cloudData);
        loadedData = progress;
    }

    private void SetCloudDataToLocal(string data)
    {
        SaveData.CloudGameData saveData = new SaveData.CloudGameData();
        saveData = JsonUtility.FromJson<SaveData.CloudGameData>(data);

        //Debug.Log("SetCloudDataToLocal");
        //PrintData(saveData);
        SaveData.Instance.SetCloudSaveDataToLocalData(saveData, isApplyToLocal);
        isApplyToLocal = true;
    }

    public void Load()
    {
        if (isAuthenticated && !isProcessing)
        {
            //Debug.Log("Debug : Load");
            gpgs.gameObject.SetActive(true);
            gpgs.SetDetails(false, LoadFromCloud);
        }
        else
        {
            Login();
        }
    }
    public void LoadFromCloud()
    {
        if (isAuthenticated && !isProcessing)
        {
            //Debug.Log("Debug : LoadFromCloud");
            ActiveWaitScreen();
            StartCoroutine(LoadFromCloudRoutin(SetCloudDataToLocal));
        }
        else
        {
            Login();
        }
    }


    private IEnumerator LoadFromCloudRoutin(Action<string> loadAction)
    {
        isProcessing = true;
        //Debug.Log("Loading game progress from the cloud.");

        ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution(
            m_saveFileName, //name of file.
            DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime,
            OnFileOpenToLoad);

        while (isProcessing)
        {
            yield return null;
        }

        loadAction.Invoke(loadedData);
    }

    public void SaveButton()
    {
        if (isAuthenticated)
        {
            gpgs.gameObject.SetActive(true);
            gpgs.SetDetails(true, SaveToCloud);
            //Debug.Log("Debug : SaveButton");
        }
        else
        {
            Login();
        }
    }
    public void SaveToCloud()
    {
        SaveData.CloudGameData temp = new SaveData.CloudGameData();
        temp.purchasedData = SaveData.Instance.GetLocalPurchasedData();
        temp.userData = SaveData.Instance.GetLocalUserData();
        //string dataToSave = SaveData.Instance.GetCloudSaveData();

        string dataToSave = JsonUtility.ToJson(temp);
        //Debug.Log("Debug : Save To Cloud");
        //PrintData(JsonUtility.FromJson<SaveData.CloudGameData>(dataToSave));

        if (dataToSave == null)
        {
            //Debug.Log("dataToSave is null");
        }
        if (isAuthenticated)
        {
            ActiveWaitScreen();
            loadedData = dataToSave;
            isProcessing = true;
            ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution(m_saveFileName, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnFileOpenToSave);
            SaveData.Instance.SetCloudSaveDataToLocalData(temp);
        }
        else
        {
            Login();
        }
    }

    private void OnFileOpenToSave(SavedGameRequestStatus status, ISavedGameMetadata metaData)
    {
        if (status == SavedGameRequestStatus.Success)
        {

            byte[] data = StringToBytes(loadedData);

            SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();

            SavedGameMetadataUpdate updatedMetadata = builder.Build();

            ((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(metaData, updatedMetadata, data, OnGameSave);


        }
        else
        {
            Debug.LogWarning("Error opening Saved Game" + status);
        }
    }


    private void OnFileOpenToLoad(SavedGameRequestStatus status, ISavedGameMetadata metaData)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            ((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(metaData, OnGameLoad);
        }
        else
        {
            Debug.LogWarning("Error opening Saved Game" + status);
        }
    }


    private void OnGameLoad(SavedGameRequestStatus status, byte[] bytes)
    {
        if (status != SavedGameRequestStatus.Success)
        {
            Debug.LogWarning("Error Saving" + status);
        }
        else
        {
            ProcessCloudData(bytes);
        }

        isProcessing = false;
    }

    private void OnGameSave(SavedGameRequestStatus status, ISavedGameMetadata metaData)
    {
        if (status != SavedGameRequestStatus.Success)
        {
            Debug.LogWarning("Error Saving" + status);
        }

        isProcessing = false;
    }

    private byte[] StringToBytes(string stringToConvert)
    {
        return Encoding.UTF8.GetBytes(stringToConvert);
    }

    private string BytesToString(byte[] bytes)
    {
        return Encoding.UTF8.GetString(bytes);
    }
}