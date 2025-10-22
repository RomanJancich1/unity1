using UnityEngine;
public class LockYHeight : MonoBehaviour
{
    public float lockedY = 0f;   // výška podlahy v tvojej scéne
    void LateUpdate()
    {
        var p = transform.position;
        transform.position = new Vector3(p.x, lockedY, p.z);
    }
}
