using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatusManager : MonoBehaviour
{
    public Transform slotGrid;
    public ItemSlot itemSlotPrefab;
    public AudioSource audioSource;

    private List<ItemSlot> itemSlots;



    private void Start()
    {
        //itemSlots = new List<ItemSlot>();
        //for (int i = 0; i < 3; i++)
        //{
        //    ItemSlot slot = Instantiate(itemSlotPrefab, slotGrid);
        //    slot.SetSlotInfo(i);
        //    itemSlots.Add(slot);
        //}
    }

    public void CreateItemSlots(int amount)
    {
        itemSlots = new List<ItemSlot>();
        itemSlots.Clear();
        //Debug.Log("Amount : " + amount);
        for (int i = 0; i < amount; i++)
        {
            itemSlots.Add(Instantiate(itemSlotPrefab, slotGrid));
        }
    }

    public void SetItemSlots(Sprite sprite, ref int itemInfoIndex)
    {
        //Debug.Log("Slot Amount : " + itemSlots.Count);
        ItemSlot emptySlot = itemSlots.Find(obj => obj.button.interactable == false);
        if (emptySlot == null)
        {
            //Debug.Log("SetItem num no: " + itemInfoIndex);
            return;
        }
        //Debug.Log("SetItem num : " + itemInfoIndex);
        float effectVolume;
        effectVolume = (SoundManager.Instance == null) ? 1 : SoundManager.Instance.effectVolume;
        audioSource.PlayOneShot(audioSource.clip, effectVolume);

        emptySlot.SetItemInfo(ref itemInfoIndex, sprite);
        //itemSlots[0].SetItemInfo(ref itemInfoIndex, sprite);
    }
}
