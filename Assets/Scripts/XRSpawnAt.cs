using UnityEngine;
using Unity.XR.CoreUtils;   // dôležité: kvôli XROrigin metodám

public class XRSpawnAt : MonoBehaviour
{
    public Transform spawnPoint;

    void Start()
    {
        var origin = GetComponent<XROrigin>();
        if (origin == null || spawnPoint == null) return;

        // presuň kameru (hlavu) presne na spawnPoint
        origin.MoveCameraToWorldLocation(spawnPoint.position);

        // natoč horizontálny smer podľa spawnPoint.forward
        Vector3 fwd = spawnPoint.forward;
        fwd.y = 0f;
        if (fwd.sqrMagnitude > 0.0001f)
            origin.MatchOriginUpCameraForward(Vector3.up, fwd.normalized);
    }
}
