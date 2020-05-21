using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayPlayer : MonoBehaviour
{
    public Sprite[] sprites;
    public SpriteRenderer spriteRenderer;

    public void SetSprites(Sprite[] sprites)
    {
        //spriteRenderer = GetComponent<SpriteRenderer>();
        this.sprites = sprites;
    }

    public void SetAnimIndex(int idx)
    {
        spriteRenderer.sprite = sprites[idx];
    }
}
