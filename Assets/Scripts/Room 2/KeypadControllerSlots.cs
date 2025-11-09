using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class KeypadController : MonoBehaviour
{
    [Header("UI (4 samostatné TMP)")]
    public TMP_Text[] SlotTMPs = new TMP_Text[4];

    [Header("Logika")]
    public int[] Digits = new int[4] { 0, 0, 0, 0 };
    public int[] Target = new int[4] { 1, 2, 3, 4 };

    [Header("Tlačidlá na prebliknutie")]
    public KeypadButton[] Buttons;     
    public float GroupFlashTime = 0.25f;

    [Header("Udalosti")]
    public UnityEvent OnSolved;
    public UnityEvent OnWrong;

    void Start() => UpdateUI();

    public void IncrementSlot(int slot)
    {
        if (slot < 0 || slot > 3) return;
        Digits[slot] = (Digits[slot] + 1) % 10;
        UpdateUI();
    }

    public void Clear()
    {
        for (int i = 0; i < 4; i++) Digits[i] = 0;
        UpdateUI();
    }

    public void Submit()
    {
        bool ok = true;
        for (int i = 0; i < 4; i++) if (Digits[i] != Target[i]) { ok = false; break; }

        if (Buttons != null)
            foreach (var b in Buttons)
                if (b != null)
                    if (ok) b.FlashSuccess(GroupFlashTime);
                    else b.FlashFail(GroupFlashTime);

        if (ok) OnSolved?.Invoke();
        else OnWrong?.Invoke();
    }

    void UpdateUI()
    {
        for (int i = 0; i < 4; i++)
            if (i < SlotTMPs.Length && SlotTMPs[i] != null)
                SlotTMPs[i].text = Digits[i].ToString();
    }
}
