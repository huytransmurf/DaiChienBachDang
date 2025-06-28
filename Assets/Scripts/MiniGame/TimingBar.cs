using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimingBar : MonoBehaviour
{
    public RectTransform movingBar;
    public RectTransform successZone;
    public float speed = 200f;
    private bool goingRight = true;
    private bool gameEnded = false;

    public List<GameObject> spikesToActivate; // Gán trong Inspector
    public int spikesPerWin = 2;
    private int currentWinCount = 0;
    public RectTransform dangerZone;

    public GameObject dialogBox;
    public TextMeshProUGUI dialogText;

    public GameObject minigame;

    private float initialSpeed;
    private float initialSuccessWidth;
    private float initialDangerWidth;

    void Start()
    {
        dialogBox.SetActive(false);

        foreach (var spike in spikesToActivate)
        {
            spike.SetActive(false);
        }
        initialSpeed = speed;
        initialSuccessWidth = successZone.rect.width;
        initialDangerWidth = dangerZone.rect.width;
        RandomizeSuccessZone();
    }

    void Update()
    {
        if (gameEnded) return;

        float moveAmount = speed * Time.unscaledDeltaTime;
        if (!goingRight) moveAmount = -moveAmount;

        movingBar.anchoredPosition += new Vector2(moveAmount, 0);

        float halfBarWidth = ((RectTransform)transform).rect.width / 2;
        float limit = halfBarWidth - movingBar.rect.width / 2;

        if (movingBar.anchoredPosition.x > limit)
        {
            goingRight = false;
            movingBar.anchoredPosition = new Vector2(limit, 0);
        }
        else if (movingBar.anchoredPosition.x < -limit)
        {
            goingRight = true;
            movingBar.anchoredPosition = new Vector2(-limit, 0);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckResult();
        }
    }

    void CheckResult()
    {
        gameEnded = true;

        float barPos = movingBar.anchoredPosition.x;

        float dangerMin = dangerZone.anchoredPosition.x - dangerZone.rect.width / 2;
        float dangerMax = dangerZone.anchoredPosition.x + dangerZone.rect.width / 2;

        if (barPos >= dangerMin && barPos <= dangerMax)
        {
            currentWinCount = 0;
            speed = initialSpeed;

            successZone.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, initialSuccessWidth);
            dangerZone.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, initialDangerWidth);

            ShowStageDialog(" Chạm vùng nguy hiểm! Làm lại từ đầu!");

            foreach (var spike in spikesToActivate)
            {
                spike.SetActive(false);
            }

            StartCoroutine(ResetGameAfterDelay(1f));
            return;
        }

        float zoneMin = successZone.anchoredPosition.x - successZone.rect.width / 2;
        float zoneMax = successZone.anchoredPosition.x + successZone.rect.width / 2;

        if (barPos >= zoneMin && barPos <= zoneMax)
        {
            currentWinCount++;
            ShowStageDialog($" Mốc {currentWinCount}/10 hoàn thành!");

            ActivateSpikes(spikesPerWin);
            IncreaseDifficulty();

            if (currentWinCount < 10)
            {
                StartCoroutine(ResetGameAfterDelay(1f));
            }
            else
            {
                ShowStageDialog(" Hoàn thành minigame!");
                StartCoroutine(EndMinigameAfterDelay(2f));
            }
        }
        else
        {
            ShowStageDialog(" Sai vị trí! Thử lại mốc này");
            StartCoroutine(ResetGameAfterDelay(1f));
        }
    }

    IEnumerator EndMinigameAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        Time.timeScale = 1; 

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }
        gameObject.SetActive(false);    
        minigame.SetActive(false);      

        
    }


    void IncreaseDifficulty()
    {
        speed += 20f;

        float newSuccessWidth = Mathf.Max(20f, successZone.rect.width - 5f);
        successZone.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newSuccessWidth);

        float newDangerWidth = Mathf.Max(20f, dangerZone.rect.width + 6f);
        dangerZone.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newDangerWidth);
    }

    IEnumerator ResetGameAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        ResetGame();
    }

    void ResetGame()
    {
        gameEnded = false;
        movingBar.anchoredPosition = new Vector2(-((RectTransform)transform).rect.width / 2 + movingBar.rect.width / 2, 0);
        RandomizeSuccessZone();
    }

    void ActivateSpikes(int count)
    {
        int activated = 0;

        foreach (var spike in spikesToActivate)
        {
            if (!spike.activeSelf)
            {
                spike.SetActive(true);
                activated++;
            }

            if (activated >= count)
                break;
        }
    }

    void RandomizeSuccessZone()
    {
        RectTransform barRect = (RectTransform)transform;
        float barWidth = barRect.rect.width;
        float successWidth = successZone.rect.width;
        float dangerWidth = dangerZone.rect.width;

        float totalZoneWidth = successWidth + dangerWidth;
        float maxOffset = (barWidth - totalZoneWidth) / 2f;

        float randomOffset = Random.Range(-maxOffset, maxOffset);
        bool dangerOnRight = Random.value > 0.5f;

        if (dangerOnRight)
        {
            successZone.anchoredPosition = new Vector2(randomOffset, 0);
            dangerZone.anchoredPosition = new Vector2(randomOffset + (successWidth + dangerWidth) / 2f, 0);
        }
        else
        {
            successZone.anchoredPosition = new Vector2(randomOffset, 0);
            dangerZone.anchoredPosition = new Vector2(randomOffset - (successWidth + dangerWidth) / 2f, 0);
        }
    }

    void ShowStageDialog(string message)
    {
        if (dialogBox != null && dialogText != null)
        {
            dialogText.text = message;
            dialogBox.SetActive(true);

            StopAllCoroutines(); 
            StartCoroutine(HideDialogAfterDelay(1.5f));
        }
    }

    IEnumerator HideDialogAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        HideStageDialog();
    }

    void HideStageDialog()
    {
        if (dialogBox != null)
        {
            dialogBox.SetActive(false);
        }
    }
}
