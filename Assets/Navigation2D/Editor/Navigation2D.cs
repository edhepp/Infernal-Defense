// Navigation2D Script (c) noobtuts.com
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

public class Navigation2D : EditorWindow
{
    // options
    float navmeshExtends = 1;
    static int drawMode = 0; // 0 = wireframe, 1 = full

    static bool IsValidCollider(Collider2D co)
    {
        // usable for navmesh generation if not trigger and if navigation static
        bool navstatic = GameObjectUtility.AreStaticEditorFlagsSet(co.gameObject, StaticEditorFlags.NavigationStatic);
        return navstatic && co.enabled && !co.isTrigger;
    }

    // set area as not walkable so that huge objects dont get walkable areas
    // inside of them
    static void MakeUnwalkable(GameObject go)
    {
        int layer = GameObjectUtility.GetNavMeshAreaFromName("Not Walkable");
        GameObjectUtility.SetNavMeshArea(go, layer);
    }

    // thanks to Collider2D.CreateMesh(), we can build 3D colliders the same way
    // for every collider type.
    //
    // NOTE: at RUNTIME, PolygonCollider would be significantly slower than
    //       Box/SphereCollider primitives. BUT we are only baking an Edit Time
    //       mesh and then destroying the helper GameObjects, so it's fine!
    //
    // IMPORTANT: we don't just add all collider2Ds because Tilemaps/Edge need
    //            special handling.
    internal static void AddCollider2Ds<T>(Transform parent)
        where T : Collider2D
    {
        // find all valid colliders, add them to projection
        T[] colliders = FindObjectsOfType<T>();
        List<T> filtered = colliders.Where(IsValidCollider).ToList();
        foreach (T collider in filtered)
        {
            // note: creating a primitive is necessary in order for it to bake properly
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.isStatic = true;
            // copy name for easier debugging
            go.name = $"{collider.name}_3D";
            go.transform.parent = parent;

            // remove box collider. note that baking uses the meshfilter, so
            // the collider doesn't really matter anyway.
            DestroyImmediate(go.GetComponent<BoxCollider>());

            // Collider2D has a CreateMesh() function that we can use.
            // it uses world position. no need to set position/rotation/scale.
            Mesh mesh2D = collider.CreateMesh(true, true);

            // create 3D mesh by projecting 2D points to 3D.
            // triangles remain the same, the mesh was 3D already just on 2D plane.
            Mesh mesh3D = new Mesh();
            mesh3D.vertices = mesh2D.vertices.Select(v => new Vector3(v.x, 0, v.y)).ToArray();
            mesh3D.triangles = mesh2D.triangles;
            //mesh.RecalculateNormals();
            mesh3D.RecalculateBounds();

            // according to Collider2D.CreateMesh() docs, we need to destroy the
            // created Mesh to avoid memory leaks.
            DestroyImmediate(mesh2D);

            // assign it to the mesh filter
            go.GetComponent<MeshFilter>().sharedMesh = mesh3D;

            MakeUnwalkable(go);
        }
    }

    internal static void AddBoxCollider2Ds(Transform parent) =>
        AddCollider2Ds<BoxCollider2D>(parent);

    static void AddCircleCollider2Ds(Transform parent) =>
        AddCollider2Ds<CircleCollider2D>(parent);

    internal static void AddPolygonCollider2Ds(Transform parent) =>
        AddCollider2Ds<PolygonCollider2D>(parent);

    // IMPORTANT: supposedly EdgeCollider2D.CreateMesh doesn't always work,
    //            so let's keep the old method for now
    internal static void AddEdgeCollider2Ds(Transform parent)
    {
        // find all valid colliders, add them to projection
        EdgeCollider2D[] colliders = FindObjectsOfType<EdgeCollider2D>();
        List<EdgeCollider2D> filtered = colliders.Where(IsValidCollider).ToList();
        foreach (EdgeCollider2D collider in filtered)
        {
            // note: creating a primitive is necessary in order for it to bake properly
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.isStatic = true;
            // copy name for easier debugging
            go.name = $"{collider.name}_3D";
            go.transform.parent = parent;

            // position via offset and transformpoint
            Vector3 localPos = new Vector3(collider.offset.x, collider.offset.y, 0);
            Vector3 worldPos = collider.transform.TransformPoint(localPos);
            // position in 3D with Y aligned so feet are at y=0
            go.transform.position = new Vector3(worldPos.x, NavMesh2D.ProjectedObjectY, worldPos.y);
            // scale depending on scale * collider size (circle=radius/box=size/...)
            go.transform.localScale = NavMeshUtils2D.ScaleFromEdgeCollider2D(collider);
            // rotation
            go.transform.rotation = Quaternion.Euler(NavMeshUtils2D.RotationTo3D(collider.transform.eulerAngles));

            // remove box collider. note that baking uses the meshfilter, so
            // the collider doesn't really matter anyway.
            DestroyImmediate(go.GetComponent<BoxCollider>());

            // create mesh from edgecollider2D by stepping through each point
            // and creating a triangle with point, point-1 and point-1 with y=1
            List<Vector3> vertices = new List<Vector3>();
            List<int> indices = new List<int>();

            // start at 2nd point so we can use the first one in our triangle,
            for (int i = 1; i < collider.points.Length; ++i)
            {
                Vector2 a2D = collider.points[i-1];
                Vector2 b2D = collider.points[i];

                // convert to 3D
                //   point A
                //   point B
                //   point C := A with y+1
                Vector3 a3D = new Vector3(a2D.x, 0, a2D.y);
                Vector3 b3D = new Vector3(b2D.x, 0, b2D.y);
                Vector3 c3D = new Vector3(a2D.x, 1, a2D.y);

                // add 3 vertices
                vertices.Add(a3D);
                vertices.Add(b3D);
                vertices.Add(c3D);

                // add last 3 vertices as indices
                indices.Add(vertices.Count - 1);
                indices.Add(vertices.Count - 2);
                indices.Add(vertices.Count - 3);
            }

            // create mesh
            Mesh mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = indices.ToArray();
            //mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            // assign it to the mesh filter
            go.GetComponent<MeshFilter>().sharedMesh = mesh;

            MakeUnwalkable(go);
        }
    }

    static void AddTilemapCollider2Ds(Transform parent)
    {
        // find all grids
        Grid[] grids = FindObjectsOfType<Grid>();
        foreach (Grid grid in grids)
        {
            // find tilemaps (we only care about those that have colliders)
            List<Tilemap> tilemaps = grid.GetComponentsInChildren<Tilemap>().Where(
                tm => tm.GetComponent<TilemapCollider2D>()
            ).ToList();

            foreach (Tilemap tilemap in tilemaps)
            {
                // go through each cell
                BoundsInt bounds = tilemap.cellBounds;
                for (int y = bounds.position.y; y < bounds.size.y; ++y)
                {
                    for (int x = bounds.position.x; x < bounds.size.x; ++x)
                    {
                        // find out if it has a collider
                        Vector3Int cellPosition = new Vector3Int(x, y, 0);
                        if (tilemap.GetColliderType(cellPosition) != Tile.ColliderType.None)
                        {
                            // convert to world space
                            Vector3 worldPosition = tilemap.GetCellCenterWorld(cellPosition);

                            // note: creating a primitive is necessary in order for it to bake properly
                            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            go.isStatic = true;
                            // copy name for easier debugging
                            go.name = $"{tilemap.name}_3D";
                            go.transform.parent = parent;
                            // position in 3D with Y aligned so feet are at y=0
                            go.transform.position = new Vector3(worldPosition.x, NavMesh2D.ProjectedObjectY, worldPosition.y);
                            // scale depending on scale * collider size (circle=radius/box=size/...)
                            go.transform.localScale = NavMeshUtils2D.ScaleTo3D(tilemap.transform.localScale);
                            // rotation
                            go.transform.rotation = Quaternion.Euler(NavMeshUtils2D.RotationTo3D(tilemap.transform.eulerAngles));

                            MakeUnwalkable(go);
                        }
                    }
                }
            }
        }
    }

    void BakeNavMesh2D()
    {
        // create a temporary parent GameObject
        GameObject obj = new GameObject();
        // set name for easier debugging
        obj.name = "3D Projection";

        // find all static box colliders, add them to projection
        AddBoxCollider2Ds(obj.transform);
        // find all static circle colliders, add them to projection
        AddCircleCollider2Ds(obj.transform);
        // find all static polygon colliders, add them to projection
        AddPolygonCollider2Ds(obj.transform);
        // find all edge polygon colliders, add them to projection
        AddEdgeCollider2Ds(obj.transform);
        // find all tilemap colliders, add them to projection
        AddTilemapCollider2Ds(obj.transform);

        // min and max point from 2D colliders projected to 3D.
        // (scanning through 3D colliders doesn't work well because the polygon
        //  GameObjects are pure meshes without colliders)
        Collider2D[] cols = FindObjectsOfType<Collider2D>();
        if (cols.Length > 0)
        {
            Vector2 min = new Vector2(Mathf.Infinity, Mathf.Infinity);
            Vector2 max = -min;
            foreach (Collider2D c in cols)
            {
                Vector2[] minmax = NavMeshUtils2D.AdjustMinMax(c, min, max);
                min = minmax[0];
                max = minmax[1];
            }

            // create ground (cube instead of plane because it has unit size)
            // (pos between min and max; scaled to fit min and max * scale)
            // note: scale.y=0 so that *navmeshExtends doesn't make it too high
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "Ground"; // for debugging
            go.isStatic = true;
            go.transform.parent = obj.transform;
            float w = max.x - min.x;
            float h = max.y - min.y;
            // IMPORTANT ground is now positioned at y=0 so that baked navmesh
            // is also at y=0. fixes a bug where sampling required a minimum
            // distance of 0.5 to make up for the difference between projection
            // (at y=0) and navmesh (at y=-0.5)!
            go.transform.position = new Vector3(min.x + w/2, 0, min.y + h/2);
            go.transform.localScale = new Vector3(w, 0, h) * navmeshExtends;
        }


        // bake navmesh asynchronously, clear mesh
        UnityEditor.AI.NavMeshBuilder.BuildNavMeshAsync(); // Async causes weird results
        if (gizmesh != null) gizmesh.Clear();
        needsRebuild = true; // rebuild as soon as async baking is finished

        // delete the gameobjects now that the path was created
        DestroyImmediate(obj);
    }

    // editor window ///////////////////////////////////////////////////////////
    [MenuItem("Window/Navigation2D")]
    public static void ShowWindow()
    {
        // Show existing window instance. If one doesn't exist, make one.
        GetWindow(typeof(Navigation2D));
    }

    static Texture2D logo =>
        (Texture2D)EditorGUIUtility.Load("Assets/Navigation2D/logo.png");

    void OnGUI()
    {
        GUILayout.BeginVertical();

        // colored logo
        //var backup = GUI.backgroundColor;
        //GUI.backgroundColor = Color.white;
        //GUILayout.BeginVertical("box");
        GUILayout.Label(logo, new GUIStyle(GUI.skin.label){alignment=TextAnchor.MiddleCenter});
        //GUILayout.EndVertical();
        //GUI.backgroundColor = backup;
        GUILayout.Label("<b>by vis2k</b>", new GUIStyle(GUI.skin.label){richText=true,alignment=TextAnchor.MiddleCenter});

        // instructions
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Instructions", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(@"1. Make your 2D Colliders Static
2. Press Bake and wait until it's done
3. Add NavMeshAgent2D to agents", MessageType.Info);
        EditorGUILayout.Space();

        // get access to original navigation settings
        // (that's how Unity's Navigation window does it too)
        SerializedObject settings = new SerializedObject(UnityEditor.AI.NavMeshBuilder.navMeshSettingsObject);
        SerializedProperty agentRadius = settings.FindProperty("m_BuildSettings.agentRadius");
        SerializedProperty agentHeight = settings.FindProperty("m_BuildSettings.agentHeight");

        // settings header
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);

        // diagram
        Rect controlRect = EditorGUILayout.GetControlRect(false, 120f, new GUILayoutOption[0]);
        NavMeshEditorHelpers.DrawAgentDiagram(controlRect, agentRadius.floatValue, agentHeight.floatValue, 0, 0);

        // agent radius
        agentRadius.floatValue = EditorGUILayout.FloatField(new GUIContent("Agent Radius", "Modifies the built in Agent Radius from Window->Navigation"), agentRadius.floatValue);
        settings.ApplyModifiedProperties();

        // ground extends
        navmeshExtends = EditorGUILayout.Slider(new GUIContent("NavMesh Extends", "Can be used to cover the outside of your scene with a NavMesh"), navmeshExtends, 1, 100);

        // visibility
        drawMode = EditorGUILayout.IntPopup("Mode", drawMode, new string[]{"Wireframe", "Full"}, new int[]{0, 1, 2});

        // repaint scene if drawMode option changed
        if (GUI.changed)
            SceneView.RepaintAll();

        // buttons
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Clear", GUILayout.Width(95f)))
        {
            UnityEditor.AI.NavMeshBuilder.ClearAllNavMeshes();
            if (gizmesh != null) gizmesh.Clear();
        }
        if (UnityEditor.AI.NavMeshBuilder.isRunning)
        {
            if (GUILayout.Button("Cancel", GUILayout.Width(95f)))
                UnityEditor.AI.NavMeshBuilder.Cancel();
        }
        else
        {
            if (GUILayout.Button("Bake", GUILayout.Width(95f)))
                BakeNavMesh2D();
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    // save & load editor settings /////////////////////////////////////////////
    void LoadSettings()
    {
        if (EditorPrefs.HasKey("Navigation2D_navmeshExtends"))
            navmeshExtends = EditorPrefs.GetFloat("Navigation2D_navmeshExtends");

        if (EditorPrefs.HasKey("Navigation2D_drawMode"))
            drawMode = EditorPrefs.GetInt("Navigation2D_drawMode");
    }

    void SaveSettings()
    {
        EditorPrefs.SetFloat("Navigation2D_navmeshExtends", navmeshExtends);
        EditorPrefs.SetInt("Navigation2D_drawMode", drawMode);
    }

    void OnFocus() => LoadSettings();
    void OnLostFocus() => SaveSettings();

    void OnDestroy() => SaveSettings();

    // visibility //////////////////////////////////////////////////////////////
    // simple helper to find out if the window is currently visible or not
    static bool visible = false;

    public void OnBecameVisible()
    {
        // repaint scene view to refresh gizmo when we show the window
        // otherwise it's only refreshed when clicking into the scene view again
        SceneView.RepaintAll();
        visible = true;
    }

    public void OnBecameInvisible()
    {
        // repaint scene view to refresh gizmo when we hide the window
        // otherwise it's only refreshed when clicking into the scene view again
        SceneView.RepaintAll();
        visible = false;
    }

    // gizmo ///////////////////////////////////////////////////////////////////
    static bool needsRebuild = false;
    static Mesh gizmesh;
    static void RebuildGizmesh(NavMeshTriangulation nm)
    {
        // the mesh is cleared after stopping the game, rebuild if necessary
        if (!gizmesh) gizmesh = new Mesh();
        gizmesh.vertices = nm.vertices.Select(v => new Vector3(v.x, v.z, 0)).ToArray(); // TODO utils?
        gizmesh.triangles = nm.indices;
        gizmesh.normals = gizmesh.vertices.Select(_ => new Vector3(0, 0, -1)).ToArray();
        needsRebuild = false;
    }

    [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
    static void OnGizmo(Transform tf, GizmoType gt)
    {
        // only show navmesh while the window is visible
        // Gizmos.Draw is slow, so only do it when the user really needs it.
        // otherwise bigger games would be slow all the time.
        if (!visible) return;

        // rebuild if necessary
        if (!gizmesh || needsRebuild)
            if (!UnityEditor.AI.NavMeshBuilder.isRunning)
                RebuildGizmesh(NavMesh.CalculateTriangulation());

        // draw if not empty
        if (gizmesh.vertices.Length > 0)
        {
            Gizmos.color = Color.cyan;
            if (drawMode == 0)
            {
                Gizmos.DrawWireMesh(gizmesh);
            }
            else if (drawMode == 1)
            {
                Gizmos.DrawMesh(gizmesh);
                Gizmos.DrawWireMesh(gizmesh);
            }
        }
    }
}
#endif
