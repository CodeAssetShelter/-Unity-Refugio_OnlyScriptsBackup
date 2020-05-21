using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot : MonoBehaviour
{
    public UnityEngine.UI.Button button;
    public UnityEngine.UI.Image image;
    public Sprite nullSprite;
    private int itemInfoIndex = -1;
    public int mySlotNumber = 0;

    private bool isGameOver = false;
    private bool firstGetItem = false;

    public void SetSlotInfo(int slotIndex)
    {
        image.sprite = nullSprite;
        mySlotNumber = slotIndex;
        button.interactable = false;
    }

    public void SetItemInfo(ref int itemInfoIndex, Sprite sprite = null)
    {
        if (sprite == null)
        {
            image.sprite = nullSprite;
        }
        else
        {
            if (firstGetItem == false)
            {
                ControllerPlayer.onGameOver += OnGameOver;
                firstGetItem = true;
            }

            this.itemInfoIndex = itemInfoIndex;
            SetButtonActive(true);
            image.sprite = sprite;
        }
    }

    public void UseItem()
    {
        if (isGameOver == false)
        {
            // Use Item
            if (itemInfoIndex != -1)
                GameManager.Instance.UseItem(itemInfoIndex);
            image.sprite = nullSprite;
            itemInfoIndex = -1;
        }
        else
        {
            // No Active
        }
        SetButtonActive(false);
    }

    private void OnGameOver()
    {
        isGameOver = true;
        SetButtonActive(false);
        ControllerPlayer.onGameOver -= OnGameOver;
    }

    private void SetButtonActive(bool active)
    {
        button.interactable = active;
    }
}
