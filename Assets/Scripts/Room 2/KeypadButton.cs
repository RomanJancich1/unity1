using UnityEngine;

public class KeypadButton : MonoBehaviour
{
    public enum ActionType { IncrementSlot, Clear, Submit }

    [Header("Binding")]
    public ActionType Action = ActionType.IncrementSlot;
    public int SlotIndex = 0;
    public KeypadController Controller;

    [Header("Feedback")]
    public Renderer Visual;            
    public Color FlashColor = Color.yellow;
    public float FlashTime = 0.12f;

    Color _origColor;
    Material _matInstance;

    void Awake()
    {
        if (Visual != null)
        {
            _matInstance = Visual.material;
            _origColor = _matInstance.color;
        }
    }

    public void Press()
    {
        if (Controller == null) return;

        switch (Action)
        {
            case ActionType.IncrementSlot: Controller.IncrementSlot(Mathf.Clamp(SlotIndex, 0, 3)); break;
            case ActionType.Clear: Controller.Clear(); break;
            case ActionType.Submit: Controller.Submit(); break;
        }

        FlashCustom(FlashColor, FlashTime);
    }

    public void FlashSuccess(float time = 0.2f) => FlashCustom(Color.green, time);
    public void FlashFail(float time = 0.2f) => FlashCustom(Color.red, time);

    public void FlashCustom(Color c, float t)
    {
        if (_matInstance == null) return;
        StopAllCoroutines();
        StartCoroutine(FlashRoutine(c, t));
    }

    System.Collections.IEnumerator FlashRoutine(Color c, float t)
    {
        _matInstance.color = c;
        yield return new WaitForSeconds(t);
        _matInstance.color = _origColor;
    }
}
