using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine.EventSystems;

public class ControllerPlayer : MonoBehaviour
{
    // Events
    public delegate void StateChange();
    public static event StateChange onGameOver;

    [Header ("- Player Components")]
    public new Rigidbody2D rigidbody2D;
    public SpriteRenderer spriteBodyRenderer;
    public CircleCollider2D body;
    public GameObject shield;
    public GameObject deadAnimation;

    [Header ("- Audio")]
    public AudioSource audioSource;
    public AudioClip audioClipInvincible;
    public AudioClip audioClipGetDamaged;
    public AudioClip audioClipMoveUp;

    [Header("- Player UI chiled scripts")]
    public PlayerStatusManager playerStatusManager;
    public TimeBar playerTimeBar;

    // Triggers
    private Cinemachine.CinemachineBasicMultiChannelPerlin virtualCamera;

    bool shield_invinclble = false;
    bool gameOver = false;
    public Sprite[] playerSprites;

    //[Header("- Speed")]
    //public ObscuredFloat upSpeed = 1;
    //public ObscuredFloat fallingLimit = 1;
    //public ObscuredFloat revision = 1.0f;

    //[Header("- Shield")]
    //public ObscuredFloat shieldDuration = 0.2f;
    //public ObscuredBool invincible = false;

    [System.Serializable]
    public class PlayerStatus
    {
        public ControllerPlayer player;

        [Header("- Speed")]
        public float upSpeed = 1;
        public float fallingLimit = 1;
        public float rivision = 1;

        [Header("- Shield")]
        public float shieldDuration = 0.2f;
        public bool invincible = false;
        public float minimalizeScale = 1.0f;
    }
    public PlayerStatus playerStatus;

    private Vector2 mySpeed = Vector2.zero;
    Coroutine coActiveShield;
    Coroutine coShieldInvincible;

    // Triggers
    bool isShieldActivated = false;


    // Start is called before the first frame update
    void Awake()
    {
        playerStatus.player = this;
        playerStatus.fallingLimit *= Physics2D.gravity.y;
        //onGameOver = null;

        virtualCamera = FindObjectOfType<Cinemachine.CinemachineBasicMultiChannelPerlin>();

        shield.SetActive(false);
    }

    private void OnEnable()
    {
        StartCoroutine(CorMoveUpDown());
        StartCoroutine(CorUpdate());
    }
    // 물리 연산이 포함되면 전부 여기다가
    IEnumerator CorMoveUpDown()
    {
        WaitForFixedUpdate wait = new WaitForFixedUpdate();
        while (true)
        {
            //this.transform.Translate(Time.fixedDeltaTime * 2.0f, 0, 0, Space.Self);
            if (rigidbody2D.velocity.y >= playerStatus.upSpeed)
            {
                Vector2 newSpeed = Vector2.zero;
                newSpeed.y = playerStatus.upSpeed;
                rigidbody2D.velocity = newSpeed;
            }
            if (rigidbody2D.velocity.y <= playerStatus.fallingLimit)
            {
                Vector2 newSpeed = Vector2.zero;
                newSpeed.y = playerStatus.fallingLimit;
                rigidbody2D.velocity = newSpeed;
            }

            if (gameOver == false)
            {
                // 버튼유지
                if (Input.GetMouseButton(0))
                {
                    if (EventSystem.current.IsPointerOverGameObject() == true)
                    {

                    }
                    else
                    {
                    }
                    rigidbody2D.AddForce(Vector2.up * Physics2D.gravity * -2f, ForceMode2D.Force);
                }
            }
            yield return wait;
        }
    }
    // 물리 연산이 없는 조작체계는 전부 여기다가
    IEnumerator CorUpdate()
    {
        while (true)
        {

            // 누르기
            if (Input.GetMouseButtonDown(0))
            {
                //Debug.Log("Down Call");
                if (coShieldInvincible == null)
                {
                    if (isShieldActivated == false)
                    {
                        if (coActiveShield != null)
                        {
                            StopCoroutine(coActiveShield);
                            ActiveShield(false);
                            isShieldActivated = false;
                        }
                        coActiveShield = StartCoroutine(CorActiveShield(playerStatus.shieldDuration));
                        isShieldActivated = true;
                    }
                }
                PlaySound(audioClipMoveUp);
            }
            // 떼기
            if (Input.GetMouseButtonUp(0))
            {
                isShieldActivated = false;
            }
            yield return null;
        }
    }

    IEnumerator CorActiveShield(float shieldDuration)
    {
        float timer = 0;
        ActiveShield(true);

        //Debug.Log("Shield : " + shieldDuration);
        while (timer < shieldDuration)
        {
            timer += Time.deltaTime;
            yield return Time.deltaTime;
        }
        ActiveShield(false);
        isShieldActivated = false;
    }
    public void ActiveShield(bool active)
    {
        //Debug.Log("Shield is : " + active);
        shield.SetActive(active);
    }
    public void ShieldCollideBonus()
    {
        GameManager.Instance.ScoreAdd(10);
        if (coShieldInvincible != null) StopCoroutine(coShieldInvincible);
        coShieldInvincible = StartCoroutine(CorShieldInvincible());
    }
    IEnumerator CorShieldInvincible()
    {
        float timer = 0;
        shield_invinclble = true;
        //Debug.Log("Invincible : " + !body.enabled);
        Color[] colorSwitch = new Color[2] { new Color(1, 1, 1, 0), Color.white };
        int colorSwitcherIndex = 0;

        spriteBodyRenderer.color = Color.white;
        while (timer < 1.0f)
        {
            spriteBodyRenderer.color = colorSwitch[colorSwitcherIndex++ % colorSwitch.Length];
            timer += Time.deltaTime;
            yield return Time.deltaTime;
        }
        shield_invinclble = false;
        spriteBodyRenderer.color = Color.white;

        coShieldInvincible = null;
        yield break;
        //Debug.Log("Invincible : " + !body.enabled);
    }
    Coroutine coItemInvincible;
    public void ActiveItemInvincible(float time)
    {
        if (coItemInvincible != null) StopCoroutine(coItemInvincible);
        coItemInvincible = StartCoroutine(CorItemInvinclble(time));
    }
    IEnumerator CorItemInvinclble(float time)
    {
        playerStatus.invincible = true;
        float timer = 0;
        Color color = Color.white;
        while (timer < time)
        {
            timer += Time.deltaTime;
            color.r = Random.Range(0, 1.0f);
            color.g = Random.Range(0, 1.0f);
            color.b = Random.Range(0, 1.0f);
            spriteBodyRenderer.color = color;
            yield return Time.deltaTime;
        }
        spriteBodyRenderer.color = Color.white;
        playerStatus.invincible = false;
    }

    public void GetDamaged()
    {
        if (gameOver == false)
        {
            // 무적이 아닌 경우
            // 데미지 처리
            if (shield_invinclble == false && playerStatus.invincible == false)
            {
                if (coShieldInvincible == null)
                    coShieldInvincible = StartCoroutine(CorShieldInvincible());
                PlaySound(audioClipGetDamaged);
                StartCoroutine(CorGetDamageShakeCamera());
            }
            // 무적 상태인 경우
            else
            {
                // 실드로 인한 무적을 공유함
                if (coShieldInvincible == null) //StopCoroutine(coShieldInvincible);
                coShieldInvincible = StartCoroutine(CorShieldInvincible());
            }
        }
        else
        {
            if (deadAnimation.activeSelf == false)
            {
                StopAllCoroutines();
                spriteBodyRenderer.color = new Color(1, 1, 1, 0);
                deadAnimation.SetActive(true);
            }
        }
    }
    IEnumerator CorGetDamageShakeCamera(float shakeValue = 1)
    {
        float timer = 0;
        while(timer < 0.2f)
        {
            timer += Time.deltaTime;
            virtualCamera.m_AmplitudeGain = shakeValue;
            virtualCamera.m_FrequencyGain = shakeValue;
            yield return Time.deltaTime;
        }

        virtualCamera.m_AmplitudeGain = 0;
        virtualCamera.m_FrequencyGain = 0;
    }

    public void SetPlayerInfo(ObscuredFloat shieldDuration, Sprite[] sprite, ObscuredInt slotAmount, ObscuredFloat lifeTime)
    {
        //Debug.Log("Get Amounts : " + shieldDuration + ", " + slotAmount + ", " + lifeTime);
        shieldDuration = (shieldDuration < 0) ? 0 : shieldDuration;
        slotAmount = (slotAmount < 0) ? 0 : (int)slotAmount;
        lifeTime = (lifeTime < 1) ? 1 : lifeTime;
        //Debug.Log("Get Amounts : " + shieldDuration + ", " + slotAmount + ", " + lifeTime);

        playerStatus.shieldDuration = shieldDuration;

        spriteBodyRenderer.sprite = sprite[0];

        playerSprites = sprite;
        playerStatusManager.CreateItemSlots(slotAmount);
        playerTimeBar.InitTimeBar(lifeTime, OnGameOver);
    }

    public void SetItem(Sprite sprite, int itemType)
    {
        playerStatusManager.SetItemSlots(sprite, ref itemType);        
    }

    private void PlaySound(AudioClip clip, float rivision = 1)
    {
        float effectVolume;
        effectVolume = (SoundManager.Instance == null) ? 1 : SoundManager.Instance.effectVolume * rivision;
        audioSource.PlayOneShot(clip, effectVolume);
    }


    public void OnGameOver()
    {
        //Debug.Log("Called : " + onGameOver.GetInvocationList().Length);
        gameOver = true;
        ActiveShield(false);

        Invoke("OnGameOverEffects", 1.0f);
    }
    public void OnGameOverEffects()
    {
        spriteBodyRenderer.color = new Color(1, 1, 1, 0);
        GameManager.Instance.GameOver();
        onGameOver();
    }
    public void SetAnimIndex(int i)
    {
        if (playerSprites == null)
        {
            Debug.Log("is null");
            return;
        }
        spriteBodyRenderer.sprite = playerSprites[i];
    }
}
