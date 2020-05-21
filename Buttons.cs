using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buttons : MonoBehaviour
{
    public Button buttonShop;
    public Button buttonStart;
    public Button buttonOption;

    private void OnEnable()
    {
        ShopManager shopManager = FindObjectOfType<ShopManager>();
        
        buttonShop.onClick.AddListener(delegate { SoundManager.Instance.ActiveUiClickSound(); });
        buttonShop.onClick.AddListener(() => shopManager.ActiveShopGameObject());

        buttonStart.onClick.AddListener(delegate { SoundManager.Instance.ActiveUiClickSound(); });
        buttonStart.onClick.AddListener(delegate { MainManager.Instance.ChangeSceneGamePlay(); });

        buttonOption.onClick.AddListener(delegate { SoundManager.Instance.ActiveUiClickSound(); });
        buttonOption.onClick.AddListener(() => GameObject.FindWithTag("UIOption").transform.GetChild(0).gameObject.SetActive(true));
    }
}
