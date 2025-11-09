using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SnapSocket : MonoBehaviour
{
    public NumberTile current;
    public Transform snapPoint;
    public float snapDistance = 0.12f;

    void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
        if (!snapPoint) snapPoint = transform;
    }

    void OnTriggerStay(Collider other)
    {
        if (current) return;

        var tile = other.GetComponentInParent<NumberTile>();
        if (!tile) return;

        if (Vector3.Distance(tile.transform.position, snapPoint.position) <= snapDistance)
        {
            var rb = tile.GetComponent<Rigidbody>();
            if (rb) { rb.isKinematic = true; rb.useGravity = false; }
            var grab = tile.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            if (grab) grab.enabled = false;

            tile.transform.SetPositionAndRotation(snapPoint.position, snapPoint.rotation);
            current = tile;
            var ctrl = GetComponentInParent<GridSumController>();
            if (ctrl) ctrl.CountPlacement();
        }
    }

    public void Clear()
    {
        if (!current) return;
        var rb = current.GetComponent<Rigidbody>();
        if (rb) { rb.isKinematic = false; rb.useGravity = true; }
        var grab = current.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab) grab.enabled = true;
        current = null;
    }
}
