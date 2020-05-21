using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
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

    //public Planes gamePlanes;
    private List<ShopManager.ItemInfo>purchasedData;
    private UserData userData;


    // Save Purchased Data
    public void SavePurchasedShopData(List<ShopManager.ItemInfo>itemInfoList)
    {
        BinaryFormatter binary = new BinaryFormatter();
        FileStream file;
        file = File.Create(Application.persistentDataPath + "/RefugioPurchasedAndShop.dat");

        binary.Serialize(file, itemInfoList);

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
            return output;
        }

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

        //saveData = data;
        file.Close();
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
            return true;
        }

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
        SoundManager.Instance.SetVolume(PlayerPrefs.GetFloat("Volume_Bgm"), PlayerPrefs.GetFloat("Volume_Effect"));
    }


    public int LoadBestScore()
    {
       return userData.bestScore;
    }

    public int LoadCoins()
    {
        return userData.coins;
    }
}
