using UnityEngine;

public struct NavMeshHit2D
{
    public Vector2 position;
    public Vector2 normal;
    public float distance;
    public int mask;
    public bool hit;

    // for easier debugging
    public override string ToString() =>
        $"NavMeshHit2D(position={position} normal={normal} distance={distance} mask={mask} hit={hit})";
}
