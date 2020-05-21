using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectiveItem : MonoBehaviour
{
    public GameManager.ItemType itemType;
    //public AudioClip audioClip;
    //public AudioSource audioSource;

    [SerializeField]
    private int constItemType = -1;
    [SerializeField]
    private bool isConstType = false;
    public ParticleSystem particle;

    private void OnEnable()
    {
        if (isConstType == false) constItemType = -1;
        else
        {
            if (itemType == GameManager.ItemType.StateCount)
            {
                constItemType = -1;
                isConstType = false;
            }
            else
            {
                constItemType = (int)itemType;
            }
        }
        SetItemType(GameManager.Instance.GetItemType(ref constItemType, particle));
    }

    private void SetItemType(Sprite sprite)
    {
        GetComponent<SpriteRenderer>().sprite = sprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") == true)
        {
            // 임시
            //if (itemType != GameManager.ItemType.CoinBox && itemType != GameManager.ItemType.Coins)
            //{
            //    float effectVolume;
            //    effectVolume = (SoundManager.Instance == null) ? 1 : SoundManager.Instance.effectVolume;
            //    audioSource.PlayOneShot(audioClip, effectVolume);
            //    //audioSource.PlayOneShot(audioSource.clip, SoundManager.Instance.effectVolume);
            //}
            //float effectVolume;
            //effectVolume = (SoundManager.Instance == null) ? 1 : SoundManager.Instance.effectVolume;
            //audioSource.PlayOneShot(audioClip, 1);
            GameManager.Instance.SetItemSlot(constItemType);
            gameObject.SetActive(false);
        }        
    }
}
