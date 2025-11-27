using UnityEngine;


public class GridCells : MonoBehaviour
{
    [Header("Grid position")]
    public int row;
    public int column;

    [Header("Visual plate under the cube")]
    public Renderer plateRenderer;

    UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket;
    GridSumController controller;

    void Awake()
    {
        socket = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();
    }

    void Start()
    {
        controller = GetComponentInParent<GridSumController>();

        if (socket == null)
        {
            Debug.LogError("Cell missing XRSocketInteractor!", this);
            return;
        }

        socket.selectEntered.AddListener(_ => controller.Recalculate());
        socket.selectExited.AddListener(_ => controller.Recalculate());
    }
}
