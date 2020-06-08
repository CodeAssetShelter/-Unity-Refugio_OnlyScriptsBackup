using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviour
{
    public static string nextScene;

    public class SceneNames
    {
        public string mainMenu = "MainMenu";
        public string gamePlay = "GamePlay";
        public string loading = "NowLoading";
    }
    public static SceneNames sceneNames = new SceneNames();

    [SerializeField]
    Image progressBar;

    private DisplayPlayer player;
    private static Sprite[] playerSprites;
    private static Coroutine routine;
    private static bool toMainMenu = false;

    private void Start()
    {
       routine = StartCoroutine(LoadScene());
    }

    public static void LoadScene(string sceneName, Sprite[] sprites = null, bool isGotoMainMenu = false)
    {
        if (sprites != null)
        {
            playerSprites = sprites;
        }
        toMainMenu = isGotoMainMenu;
        nextScene = sceneName;
  
        SceneManager.LoadScene(sceneNames.loading);
    }

    IEnumerator LoadScene()
    {
        yield return null;

        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0.0f;

        Vector3 min, max;
        min = Camera.main.ViewportToWorldPoint(Vector3.zero);
        max = Camera.main.ViewportToWorldPoint(new Vector3(1, 1));
        //Debug.Log(min + " : " + max);


        player = GameObject.FindWithTag("Player").GetComponent<DisplayPlayer>();
        player.SetSprites(playerSprites);


        min.z = min.y = max.y = max.z = 0;
        min *= 1.3f;
        player.transform.position = min;


        yield return new WaitForSeconds(0.5f);

        while (!op.isDone)
        {
            yield return null;
            timer += Time.deltaTime;
            if (op.progress < 0.9f)
            {
                //progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
                player.transform.position = Vector3.Lerp(min, max, timer);
                if (timer >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                //progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);
                player.transform.position = Vector3.Lerp(min, max, timer);
                if (timer >= 1.0f)
                {
                    op.allowSceneActivation = true;

                    if (toMainMenu == true)
                    {
                        MainManager.Instance.ReLoad(nextScene);
                    }
                    yield break;
                }
            }
        }
    }
}
