using UnityEngine;
using Unity.XR.CoreUtils; // dôležité!

public class XRSpawnAt : MonoBehaviour
{
    public XROrigin xrOrigin;
    public Transform spawnPoint;

    void Start()
    {
        if (xrOrigin && spawnPoint)
        {
            xrOrigin.MoveCameraToWorldLocation(spawnPoint.position);

            // zarovnaj smer pohľadu na spawn forward (ignoruje náklon)
            Vector3 fwd = spawnPoint.forward; fwd.y = 0f;
            if (fwd.sqrMagnitude > 0.0001f)
                xrOrigin.MatchOriginUpCameraForward(Vector3.up, fwd.normalized);
        }
    }
}
