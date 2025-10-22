using UnityEngine;

public class MyDoorController : MonoBehaviour
{
    [Header("Open motion")]
    public float openAngle = 90f;   // +/− podľa smeru
    public float openTime = 0.4f;

    [Header("Gating")]
    public bool handleInput = true;          // E na klávesnici
    public KeyCode interactKey = KeyCode.E;
    public bool requireUnlocked = true;      // musí byť odomknuté?
    public bool requirePlayerInRange = true; // musí byť hráč v triggri?

    [Header("State (runtime)")]
    public bool isLocked = true;      // zamknuté, kým puzzle nesplníš
    public bool playerInRange = false;

    Quaternion rotClosed, rotOpen;
    bool isOpen, isMoving;

    void Awake()
    {
        rotClosed = transform.localRotation;
        rotOpen = rotClosed * Quaternion.Euler(0f, openAngle, 0f);
    }

    void Update()
    {
        if (!handleInput) return;
        if (Input.GetKeyDown(interactKey))
            TryToggleDoor(); // rešpektuje lock a range
    }

    // ▶ Volaj toto (nie ToggleDoor) z triggerov / vstupu
    public void TryToggleDoor()
    {
        if (requireUnlocked && isLocked) return;
        if (requirePlayerInRange && !playerInRange) return;
        ToggleDoor(); // pôvodné správanie
    }

    // ▼ Tvoj pôvodný toggle (ponechaný bezo zmeny)
    public void ToggleDoor()
    {
        if (isMoving) return;
        StopAllCoroutines();
        StartCoroutine(RotateTo(isOpen ? rotClosed : rotOpen));
        isOpen = !isOpen;
    }

    System.Collections.IEnumerator RotateTo(Quaternion target)
    {
        isMoving = true;
        float t = 0f;
        Quaternion start = transform.localRotation;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, openTime);
            float s = t * t * (3f - 2f * t);   // smoothstep
            transform.localRotation = Quaternion.Slerp(start, target, s);
            yield return null;
        }
        transform.localRotation = target;
        isMoving = false;
    }

    // 🔒 API na puzzle/trigger skripty:
    public void Lock() => isLocked = true;
    public void Unlock() => isLocked = false;
}
