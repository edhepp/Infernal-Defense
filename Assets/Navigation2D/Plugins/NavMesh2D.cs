// NavMesh.Raycast etc. ported to 2D
using UnityEngine;
using UnityEngine.AI;

public static class NavMesh2D
{
    public const int AllAreas = -1;

    // baked 3D navmesh is perfectly positioned at y=0.
    // 3D objects should be positioned at y=0.5 so that their feet are at y=0.
    // => let's have a const so we don't need to hardcode the value everywhere.
    public const float ProjectedObjectY = 0.5f;

    // when sampling positions, the 2D position is projected to 3D.
    // in 3D, the baked Navmesh is never exactly at Y=0.
    // it seems to at around 0.05 above the ground.
    // so include that distance for sampling.
    public const float DistanceAboveGroundIn3D = 0.1f;

    // raycast to see if there's anything between source and target.
    // returns true if hit something, false if hit nothing.
    // based on: https://docs.unity3d.com/ScriptReference/AI.NavMesh.Raycast.html
    public static bool Raycast(Vector2 sourcePosition, Vector2 targetPosition, out NavMeshHit2D hit, int areaMask)
    {
        NavMeshHit hit3D;
        if (NavMesh.Raycast(NavMeshUtils2D.ProjectPointTo3D(sourcePosition),
                            NavMeshUtils2D.ProjectPointTo3D(targetPosition),
                            out hit3D,
                            areaMask))
        {
            hit = new NavMeshHit2D{position = NavMeshUtils2D.ProjectTo2D(hit3D.position),
                                   normal = NavMeshUtils2D.ProjectTo2D(hit3D.normal),
                                   distance = hit3D.distance,
                                   mask = hit3D.mask,
                                   hit = hit3D.hit};
            return true;
        }
        hit = new NavMeshHit2D();
        return false;
    }

    // check if a position is on navmesh.
    // based on: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
    // NOTE: distance=0 always returns false even on navmesh. use at least 0.01f
    public static bool SamplePosition(Vector2 sourcePosition, out NavMeshHit2D hit, float maxDistance, int areaMask)
    {
        // when sampling positions, the 2D position is projected to 3D.
        // in 3D, the baked Navmesh is never exactly at Y=0.
        // it seems to at around 0.05 above the ground.
        // so include that distance for sampling.
        // => see Test: SamplePosition_OnNavMesh_Distance0
        maxDistance += DistanceAboveGroundIn3D;

        NavMeshHit hit3D;
        if (NavMesh.SamplePosition(NavMeshUtils2D.ProjectPointTo3D(sourcePosition), out hit3D, maxDistance, areaMask))
        {
            hit = new NavMeshHit2D{position = NavMeshUtils2D.ProjectTo2D(hit3D.position),
                                   normal = NavMeshUtils2D.ProjectTo2D(hit3D.normal),
                                   distance = hit3D.distance,
                                   mask = hit3D.mask,
                                   hit = hit3D.hit};
            return true;
        }
        hit = new NavMeshHit2D();
        return false;
    }
}