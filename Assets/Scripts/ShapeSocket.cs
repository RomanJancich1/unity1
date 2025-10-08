using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor))]
public class ShapeSocket : MonoBehaviour
{
    public ShapeType accepts;
    UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket;

    void Awake() => socket = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();

    public void OnSelectEntered(SelectEnterEventArgs args)
    {
        var shape = args.interactableObject.transform.GetComponentInParent<Shape>();
        if (!shape || shape.type != accepts)
        {
            // nespr�vny tvar � okam�ite ho vypust�
            socket.interactionManager.SelectExit(socket, args.interactableObject);
        }
    }
}
