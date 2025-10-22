using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonPadController : MonoBehaviour
{
    [Tooltip("Správna sekvencia Id tlačidiel (napr. 2,5,1,3,4)")]
    public List<int> correctSequence = new() { 1, 2, 3, 4, 5 };

    [Tooltip("Všetky tlačidlá patriace k tomuto pultu")]
    public List<ButtonButton> buttons = new();

    [Tooltip("Dvere ktoré sa odomknú po úspechu")]
    public DoorController door;

    [Header("Timings")]
    public float failResetDelay = 0.6f;

    [Header("Events")]
    public UnityEvent OnSolved;
    public UnityEvent OnFailed;

    List<int> _input = new();
    List<ButtonButton> _pressedButtons = new();
    bool _accepting = true;

    void Awake()
    {
        // Automaticky nazbieraj tlačidlá ak nie sú zadané ručne
        if (buttons == null || buttons.Count == 0)
            buttons = new List<ButtonButton>(GetComponentsInChildren<ButtonButton>(true));
        foreach (var b in buttons) if (b) b.pad = this;
    }

    void Start()
    {
        if (door) door.Lock();
        SetAllIdle();
    }

    public void RegisterPress(int id, ButtonButton btn)
    {
        if (!_accepting) return;

        // počas zadávania – zvýrazni práve stlačené tlačidlo na ORANŽOVO a nechaj ho tak
        if (btn) { btn.SetBaseColor(btn.armedColor); _pressedButtons.Add(btn); }
        _input.Add(id);

        // Po naplnení celej dĺžky sekvencie vyhodnoť naraz
        if (_input.Count < correctSequence.Count)
            return;

        _accepting = false;
        bool ok = IsSequenceCorrect();

        if (ok)
        {
            BlinkAll(Color.green, 3);
            if (door) door.Unlock();
            OnSolved?.Invoke();
            // Po krátkej chvíli vráť tlačidlá do idle a resetuj stav (ak chceš ponechať zelené, tento riadok vynechaj)
            Invoke(nameof(ResetAfterSuccess), 0.5f);
        }
        else
        {
            BlinkAll(Color.red, 3);
            OnFailed?.Invoke();
            Invoke(nameof(ResetAttempt), failResetDelay);
        }
    }

    bool IsSequenceCorrect()
    {
        if (_input.Count != correctSequence.Count) return false;
        for (int i = 0; i < correctSequence.Count; i++)
            if (_input[i] != correctSequence[i]) return false;
        return true;
    }

    void ResetAttempt()
    {
        _input.Clear();
        _pressedButtons.Clear();
        SetAllIdle();
        _accepting = true;
    }

    void ResetAfterSuccess()
    {
        _input.Clear();
        _pressedButtons.Clear();
        SetAllIdle(); // alebo nechaj zelené: vyhoď tento riadok, ak chceš „permanentný“ success look
        _accepting = true;
    }

    void SetAllIdle()
    {
        foreach (var b in buttons) if (b) b.SetBaseColor(b.idleColor);
    }

    void BlinkAll(Color c, int times)
    {
        foreach (var b in buttons) if (b) b.Blink(c, times);
    }
}
