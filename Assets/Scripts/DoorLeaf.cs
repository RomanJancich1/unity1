using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DoorLeafOpener : MonoBehaviour
{
    public MyDoorController door;   

    [Header("Dosah interakcie")]
    public bool requirePlayerInRange = true;
    public float useRange = 2.0f;
    public Transform player;        

    void Awake()
    {
        if (!door) door = GetComponentInParent<MyDoorController>();
        if (!player) { var cam = Camera.main; if (cam) player = cam.transform; }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!door) return;
            if (!requirePlayerInRange || IsPlayerCloseEnough())
                door.TryToggle();
        }
    }

    bool IsPlayerCloseEnough()
    {
        if (!player) return true;
        Vector3 a = player.position; a.y = 0f;
        Vector3 b = transform.position; b.y = 0f;
        return Vector3.Distance(a, b) <= useRange;
    }

    public void Use()
    {
        if (!door) return;
        if (!requirePlayerInRange || IsPlayerCloseEnough())
            door.TryToggle();
    }
}
