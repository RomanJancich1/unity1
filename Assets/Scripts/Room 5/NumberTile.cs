using UnityEngine;

[DisallowMultipleComponent]
public class NumberTile : MonoBehaviour
{
    [Min(0)] public int value = 1;

    void OnValidate() { gameObject.name = $"Cube_{value}"; }
}
