//using GooglePlayGames;
//using GooglePlayGames.BasicApi;
//using GooglePlayGames.BasicApi.SavedGame;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveData : MonoBehaviour
{
    private static SaveData _instance;
    public static SaveData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(SaveData)) as SaveData;

                if (_instance == null)
                {
                    //Debug.LogError("No Active SaveData!");
                }
            }

            return _instance;
        }
    }

    // Save Data
    [Serializable]
    public class UserData
    {
        public int coins;
        public int bestScore;
    }

    [Serializable]
    public class CloudGameData
    {
        public CloudGameData()
        {
            userData = new UserData();
            purchasedData = new List<ShopManager.ItemInfo>();
        }
        public UserData userData;
        public List<ShopManager.ItemInfo> purchasedData;
    }

    //public Planes gamePlanes;
    private List<ShopManager.ItemInfo> purchasedData;
    private UserData userData;
    private CloudGameData cloudGameData;

    private void Awake()
    {
        purchasedData = new List<ShopManager.ItemInfo>();
        userData = new UserData();
        cloudGameData = new CloudGameData();
        //Invoke("TEST", 1.0f);
    }

    private void TEST()
    {
        cloudGameData.purchasedData = purchasedData;
        cloudGameData.userData = userData;

        var dat = JsonUtility.ToJson(cloudGameData);
        FileStream file;
        file = new FileStream(Application.persistentDataPath + "/RefugioPurchasedAndShop.bin",
            FileMode.Create);

        byte[] data = Encoding.UTF8.GetBytes(dat);
        file.Write(data, 0, data.Length);
        file.Close();
    }

    // Save Purchased Data
    public void SavePurchasedShopData(List<ShopManager.ItemInfo> itemInfoList)
    {
        BinaryFormatter binary = new BinaryFormatter();
        FileStream file;
        file = File.Create(Application.persistentDataPath + "/RefugioPurchasedAndShop.dat");

        binary.Serialize(file, itemInfoList);

        cloudGameData.purchasedData = itemInfoList;
        this.purchasedData = itemInfoList;
        //saveData = data;
        file.Close();
    }

    // Load Area
    public List<ShopManager.ItemInfo> LoadPurchasedShopData()
    {
        //Debug.Log(Application.persistentDataPath);
        if (File.Exists(Application.persistentDataPath + "/RefugioPurchasedAndShop.dat") == false) return null;

        BinaryFormatter binary = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/RefugioPurchasedAndShop.dat", FileMode.Open);
        List<ShopManager.ItemInfo> output = new List<ShopManager.ItemInfo>();
        if (file != null && file.Length > 0)
        {
            output = new List<ShopManager.ItemInfo>((List<ShopManager.ItemInfo>)binary.Deserialize(file));
            //(List<ShopManager.ItemInfo>())binary.Deserialize(file);
            //(OnePlaneData)binary.Deserialize(file);
            file.Close();
            return output;
        }
        file.Close();
        return null;
    }

    // Save Purchased Data
    public void SaveUserData(int myCoins, int myScore)
    {
        BinaryFormatter binary = new BinaryFormatter();
        FileStream file;
        file = File.Create(Application.persistentDataPath + "/RefugioUserData.dat");

        UserData user = new UserData();
        user.coins = myCoins;
        user.bestScore = myScore;

        userData = user;

        binary.Serialize(file, userData);
        file.Close();

        CloudGameData cloudGameDataTemp = new CloudGameData();
        cloudGameDataTemp.userData.bestScore = myScore;
        cloudGameDataTemp.userData.coins = myCoins;
        //saveData = data;
    }

    // Load Area
    public bool LoadUserData()
    {
        //Debug.Log(Application.persistentDataPath);
        if (File.Exists(Application.persistentDataPath + "/RefugioUserData.dat") == false) return false;

        BinaryFormatter binary = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/RefugioUserData.dat", FileMode.Open);
        //UserData output = new UserData();
        if (file != null && file.Length > 0)
        {
            userData = (UserData)binary.Deserialize(file);
            //(List<ShopManager.ItemInfo>())binary.Deserialize(file);
            //(OnePlaneData)binary.Deserialize(file);
            file.Close();
            return true;
        }
        file.Close();
        return false;
    }



    public void SaveUserDataFromQuitGame(int coin, int score, float bgmVolume, float effectVolume)
    {
        BinaryFormatter binary = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/RefugioUserData.dat");

        UserData data = new UserData();
        //data.goodsInfo = new Planes.GoodsInfo[gamePlanes.GetGoodsEa()];
        //for (int i = 0; i < gamePlanes.GetGoodsEa(); i++)
        //{
        //    data.goodsInfo[i] = gamePlanes.goodsInfo[i];
        //}

        //data.goodsInfo = gamePlanes.goodsInfo;

        //data.coins = coin;
        data.bestScore = score;

        binary.Serialize(file, data);

        userData = data;
        file.Close();
    }


    public void SaveVolumeData()
    {
        PlayerPrefs.SetFloat("Volume_Bgm", SoundManager.Instance.bgmVolume);
        PlayerPrefs.SetFloat("Volume_Effect", SoundManager.Instance.effectVolume);
    }
    public void LoadVolumeData()
    {
        float bgm = 
            (PlayerPrefs.HasKey("Volume_Bgm") == true) ? 
            PlayerPrefs.GetFloat("Volume_Bgm") : 0.5f;
        float effect = 
            (PlayerPrefs.HasKey("Volume_Bgm") == true) ? 
            PlayerPrefs.GetFloat("Volume_Bgm") : 0.5f;

        SoundManager.Instance.SetVolume(bgm, effect);
    }


    public int LoadBestScore()
    {
        return userData.bestScore;
    }

    public int LoadCoins()
    {
        return userData.coins;
    }

    public bool SetCloudSaveDataToLocalData(CloudGameData cloudGameData)
    {
        if (this.cloudGameData == null)
        {
            Debug.Log("cloud data is null");
            return false;
        }
        if (cloudGameData == null)
        {
            Debug.Log("Loaded Cloud Data is not valid");
            return false;
        }
        this.cloudGameData = cloudGameData;
        userData = cloudGameData.userData;
        purchasedData = cloudGameData.purchasedData;

        MainManager.Instance.SetDataFromCloudSave(ref this.cloudGameData);
        FindObjectOfType<ShopManager>().itemInfoList = this.purchasedData;
        return true;
    }
    public string GetCloudSaveData()
    {
        if (userData == null)
        {
            Debug.Log("userData is not valid");
            return null;
        }
        if (purchasedData == null)
        {
            Debug.Log("purchasedData is not valid");
            return null;
        }
        cloudGameData.purchasedData = purchasedData;
        cloudGameData.userData = userData;

        var dat = JsonUtility.ToJson(cloudGameData);
        return dat;
    }
}
