using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(UIManager)) as UIManager;

                if (_instance == null)
                {
                    //Debug.LogError("No Active GameManager!");
                }
            }

            return _instance;
        }
    }

    Stack<GameObject> rootObject;

    public UnityEngine.UI.Text earnCoin;

    void Awake()
    {
        rootObject = new Stack<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

            if (hit.collider != null && hit.collider.transform == this.transform)
            {
                Debug.Log(hit.collider.name);
                // raycast hit this gameobject
            }
            //if (EventSystem.current.IsPointerOverGameObject() == false)
            //{
            //    PopUIObject();
            //    //Debug.Log("11");
            //}
            //else
            //{
            //    if (EventSystem.current.currentSelectedGameObject.CompareTag("Exit") == true)
            //    {
            //        PopUIObject();
            //    }
            //    //Debug.Log("22");
            //}
        }
    }

    //public void PushUIObject(GameObject gameObject)
    //{
    //    if (rootObject.Contains(gameObject) == false)
    //    {
    //        rootObject.Push(gameObject);
    //    }
    //}
    //public void PopUIObject()
    //{
    //    if (rootObject.Count != 0)
    //    {
    //        rootObject.Pop().SetActive(false);
    //    }
    //    else
    //    {
    //    }
    //}

    public void CheckActiveCoinText()
    {
        MainManager.Instance.SetActiveText(true);
        RefreshCoin();
    }

    public void RefreshCoin()
    {
        earnCoin.text = "" + MainManager.Instance.GetMyCoin();
    }
}
