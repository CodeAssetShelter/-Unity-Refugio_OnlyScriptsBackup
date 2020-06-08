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

    public bool isAuthenticated
    {
        get
        {
            return Social.localUser.authenticated;
        }
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

    private void PrintData(SaveData.CloudGameData data)
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

    public void Login()
    {
        Social.localUser.Authenticate((bool success) =>
        {
            if (!success)
            {
                Debug.Log("Fail Login");
            }
        });
    }

    public void Logout()
    {
        if (Social.localUser.authenticated)
        {
            ((GooglePlayGames.PlayGamesPlatform)Social.Active).SignOut();
        }
    }

    private void ProcessCloudData(byte[] cloudData)
    {
        if (cloudData == null)
        {
            Debug.Log("No Data saved to the cloud");
            return;
        }

        string progress = BytesToString(cloudData);
        loadedData = progress;
    }

    private void SetCloudDataToLocal(string data)
    {
        SaveData.CloudGameData saveData = new SaveData.CloudGameData();
        saveData = JsonUtility.FromJson<SaveData.CloudGameData>(data);

        Debug.Log("SetCloudDataToLocal");
        PrintData(saveData);
        SaveData.Instance.SetCloudSaveDataToLocalData(saveData);
    }

    public void LoadFromCloud()
    {
        if (isAuthenticated && !isProcessing)
        {
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
        Debug.Log("Loading game progress from the cloud.");

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

    public void SaveToCloud()
    {
        string dataToSave = SaveData.Instance.GetCloudSaveData();
        Debug.Log("Save To Cloud");
        PrintData(JsonUtility.FromJson<SaveData.CloudGameData>(dataToSave));

        if (dataToSave == null)
        {
            Debug.Log("dataToSave is null");
        }
        if (isAuthenticated)
        {
            loadedData = dataToSave;
            isProcessing = true;
            ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution(m_saveFileName, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnFileOpenToSave);
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