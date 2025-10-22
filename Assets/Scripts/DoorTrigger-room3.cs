using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DoorTriggerGate : MonoBehaviour
{
    public DoorController door;                 // dvere, ktoré tento trigger ovláda
    public ButtonPadController puzzle;          // tvoj pad pre danú miestnosť

    Collider triggerCol;

    void Awake()
    {
        triggerCol = GetComponent<Collider>();
        triggerCol.isTrigger = true;            // istota
    }

    void Start()
    {
        // na štarte zakáž trigger a zamkni dvere
        triggerCol.enabled = false;
        if (door) door.Lock();

        // po vyriešení puzzla povol trigger + odomkni dvere
        if (puzzle) puzzle.OnSolved.AddListener(() =>
        {
            triggerCol.enabled = true;
            if (door) door.Unlock();
        });
    }

    // Ak chceš, aby sa dvere otvorili blízkosťou:
    void OnTriggerEnter(Collider other)
    {
        if (!door || door.isLocked) return;
        // tu môžeš spustiť animáciu/otočenie dverí
        if (door.animator) door.animator.SetTrigger("Open");
    }
}
