using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance { get; private set; }

    [Header("Time settings")]
    public float totalTimeSeconds = 600f;  

    [Header("UI")]
    public TMP_Text timerText;

    [Header("End game UI")]
    public GameEndUI endUI;

    float _timeLeft;
    bool _running = false;
    bool _finished = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _timeLeft = totalTimeSeconds;

        if (timerText)
        {
            timerText.gameObject.SetActive(false);   
            UpdateTimerText();
        }
    }

    void Update()
    {
        if (!_running || _finished) return;

        _timeLeft -= Time.deltaTime;
        if (_timeLeft < 0f) _timeLeft = 0f;

        UpdateTimerText();

        if (_timeLeft <= 0f)
        {
            _running = false;
            HandleLose();
        }
    }

    void UpdateTimerText()
    {
        if (!timerText) return;

        int seconds = Mathf.CeilToInt(_timeLeft);
        int minutes = seconds / 60;
        int secs = seconds % 60;

        timerText.text = $"{minutes:0}:{secs:00}";
    }

    public void StartTimer()
    {
        _finished = false;
        _timeLeft = totalTimeSeconds;
        _running = true;

        if (timerText)
            timerText.gameObject.SetActive(true);
    }

    public void NotifyPlayerFinished()
    {
        if (_finished) return;
        _finished = true;
        _running = false;

        if (endUI) endUI.ShowWin();
    }

    void HandleLose()
    {
        if (_finished) return;
        _finished = true;

        if (endUI) endUI.ShowLose();
    }
}
