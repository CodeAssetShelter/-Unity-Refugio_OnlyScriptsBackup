using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinBox : MonoBehaviour
{
    public Text coinAmount;
    public Animator animation;

    public void SetDisplayEarnCoin(int amount)
    {
        gameObject.SetActive(true);
        coinAmount.text = "" + amount;
    }
    public void SetDeactive()
    {
        gameObject.SetActive(false);
    }
}
