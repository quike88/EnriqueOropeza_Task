using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("FadeOut Settings")]
    [SerializeField] private Animator fadeAnimator;

    [Header("Restart Settings")]
    [SerializeField] private float holdDuration = 2.0f;
    private float holdTimer = 0f;
    private bool isRestarting = false;
    private bool isHoldingRestart = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (isHoldingRestart && !isRestarting)
        {
            holdTimer += Time.deltaTime;

            if (holdTimer >= holdDuration)
            {
                RestartLevel();
            }
        }
    }

    public void OnRestart(InputAction.CallbackContext context)
    {
        if (isRestarting) return;

        if (context.started)
        {
            isHoldingRestart = true;
        }
        else if (context.canceled)
        {
            isHoldingRestart = false;
            holdTimer = 0f;
        }
    }

    public void RestartLevel()
    {
        if (isRestarting) return;
        PlayerPrefs.DeleteAll();
        StartCoroutine(RestartSequence());
    }

    private IEnumerator RestartSequence()
    {
        isRestarting = true;

        if (fadeAnimator != null)
        {
            fadeAnimator.SetTrigger("Out");
        }

        yield return new WaitForSecondsRealtime(1f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NotifyPlayerFell()
    {
        RestartLevel();
    }
    public void NotifyPlayerDied()
    {
        RestartLevel();
    }

}