using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoResizeRect : MonoBehaviour
{
    public bool vertical = false;
    public bool horizon = false;
    // Start is called before the first frame update
    void Start()
    {
        GridLayoutGroup grid = this.gameObject.GetComponent<GridLayoutGroup>();
        float child = this.transform.childCount;
        //Debug.Log("Child : " + child);
        Vector2 spacing = grid.spacing;
        Vector2 cellsize = grid.cellSize;
        if (vertical == true)
            this.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (cellsize.y + spacing.y) * child);
        if (horizon == true)
            this.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (cellsize.x + spacing.x) * child);
    }
}
