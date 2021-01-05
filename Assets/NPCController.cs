using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public float speed;

    public List<Transform> waypoints = new List<Transform>();

    private void Awake()
    {
        List<Vector3> points = new List<Vector3>();
        waypoints.ForEach((v) => { points.Add(v.position); });
        StartCoroutine(FollowWaypoints(points));
    }

    IEnumerator MoveToPoint (Vector3 point)
    {
        while (Vector3.Distance(transform.position, point) >= 0.1f)
        {
            transform.position += Vector3.Normalize(point - transform.position)*Time.deltaTime*speed;
            yield return null;
        }
    }

    IEnumerator FollowWaypoints (List<Vector3> points)
    {
        while (true)
        {
            for (int i = 0; i < points.Count; i++)
            {
                yield return StartCoroutine(MoveToPoint(points[i]));
            }

            yield return null;
        }
    }
}
