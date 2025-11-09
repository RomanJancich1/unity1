using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DoorTriggerGate : MonoBehaviour
{
    public DoorController door;                 
    public ButtonPadController puzzle;          

    Collider triggerCol;

    void Awake()
    {
        triggerCol = GetComponent<Collider>();
        triggerCol.isTrigger = true;            
    }

    void Start()
    {
        triggerCol.enabled = false;
        if (door) door.Lock();

        if (puzzle) puzzle.OnSolved.AddListener(() =>
        {
            triggerCol.enabled = true;
            if (door) door.Unlock();
        });
    }

    void OnTriggerEnter(Collider other)
    {
        if (!door || door.isLocked) return;
        if (door.animator) door.animator.SetTrigger("Open");
    }
}
