using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MultiStagePadController : MonoBehaviour
{
    [System.Serializable]
    public class Stage
    {
        [Tooltip("Koľko tlačidiel je v tejto etape aktívnych (1..N).")]
        public int activeButtons = 3;

        [Tooltip("Sekvencia ID tlačidiel (napr. 2,1,3,2). DĹŽKA = počtu bliknutí.")]
        public List<int> sequence = new List<int>() { 1, 2, 3 };

        [Tooltip("Čas (s) svietenia jedného kroku pri prehrávaní sekvencie.")]
        public float showTime = 0.18f;

        [Tooltip("Pauza (s) medzi krokmi sekvencie.")]
        public float gapTime = 0.10f;
    }

    [Header("References")]
    public List<ButtonButton> buttons = new();  
    public DoorController door;                  

    [Header("Stages (3→4→5, podľa zadania)")]
    public List<Stage> stages = new()
    {
        new Stage { activeButtons = 3, sequence = new List<int>{ 1,2,3 } },                  
        new Stage { activeButtons = 4, sequence = new List<int>{ 1,3,2,4,2,1 } },            
        new Stage { activeButtons = 5, sequence = new List<int>{ 5,2,3,1,4,2,5,3 } }         
    };

    [Header("Feedback")]
    public int successBlinks = 3;
    public int failBlinks = 3;
    public float failResetDelay = 0.6f;

    [Header("Events")]
    public UnityEvent OnStageSolved;   
    public UnityEvent OnSolvedAll;     
    public UnityEvent OnFailedAttempt; 

    int stageIndex = 0;
    readonly List<int> input = new();
    bool accepting = true;
    Coroutine playCo;

    void Start()
    {
        if (door) door.Lock();

        if (buttons == null || buttons.Count == 0)
            buttons = new List<ButtonButton>(GetComponentsInChildren<ButtonButton>(true));

        foreach (var b in buttons)
            if (b && b.pad == null) b.pad = this;

        ApplyStageSetup();
        ClearAttempt();
    }

    public void RegisterPress(int id, ButtonButton btn)
    {
        var st = stages[stageIndex];
        if (!accepting) return;
        if (id < 1 || id > st.activeButtons) return; 

        if (btn) btn.SetBaseColor(btn.armedColor);
        input.Add(id);

        if (input.Count >= st.sequence.Count)
        {
            accepting = false;
            bool ok = IsCorrect(st.sequence, input);
            if (ok)
            {
                BlinkAll(Color.green, successBlinks);
                OnStageSolved?.Invoke();

                if (stageIndex == stages.Count - 1)
                {
                    if (door) door.Unlock();
                    OnSolvedAll?.Invoke();
                    Invoke(nameof(ResetForReplay), 0.5f);  
                }
                else
                {
                    stageIndex++;
                    Invoke(nameof(NextStageIdle), 0.35f);
                }
            }
            else
            {
                BlinkAll(Color.red, failBlinks);
                OnFailedAttempt?.Invoke();
                Invoke(nameof(ResetStageOnly), failResetDelay);
            }
        }
    }

    public void PlayCurrent()
    {
        if (playCo != null) StopCoroutine(playCo);
        playCo = StartCoroutine(CoPlay(stages[stageIndex]));
    }

    public void ResetStageOnly()
    {
        ClearAttempt();
        accepting = true;
        SetAllIdle();
    }

    public void ResetAll()
    {
        stageIndex = 0;
        ApplyStageSetup();
        ResetStageOnly();
        if (door) door.Lock();
    }

    void ApplyStageSetup()
    {
        var st = stages[Mathf.Clamp(stageIndex, 0, stages.Count - 1)];
        for (int i = 0; i < buttons.Count; i++)
        {
            bool enable = (i < st.activeButtons);
            if (buttons[i]) buttons[i].SetEnabled(enable);
        }
        SetAllIdle();
    }

    void NextStageIdle()
    {
        ApplyStageSetup();
        ResetStageOnly();
    }

    void ResetForReplay()
    {
        SetAllIdle();
        accepting = true;
    }

    IEnumerator CoPlay(Stage st)
    {
        accepting = false;
        SetAllIdle();
        yield return new WaitForSeconds(0.15f);

        foreach (int id in st.sequence)
        {
            var b = GetById(id);
            if (b)
            {
                b.SetBaseColor(b.armedColor);
                yield return new WaitForSeconds(st.showTime);
                b.SetBaseColor(b.idleColor);
                yield return new WaitForSeconds(st.gapTime);
            }
        }
        ClearAttempt();
        accepting = true;
    }

    bool IsCorrect(List<int> seq, List<int> attempt)
    {
        if (attempt.Count != seq.Count) return false;
        for (int i = 0; i < seq.Count; i++)
            if (attempt[i] != seq[i]) return false;
        return true;
    }

    void ClearAttempt() => input.Clear();

    void SetAllIdle()
    {
        foreach (var b in buttons) if (b) b.SetBaseColor(b.idleColor);
    }

    void BlinkAll(Color c, int times)
    {
        foreach (var b in buttons) if (b) b.Blink(c, times);
    }

    ButtonButton GetById(int id)
    {
        int idx = id - 1;
        if (idx >= 0 && idx < buttons.Count) return buttons[idx];
        foreach (var b in buttons) if (b && b.id == id) return b;
        return null;
    }
}
