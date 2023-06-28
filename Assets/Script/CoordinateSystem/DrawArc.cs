using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawArc : MonoBehaviour
{
    //public Transform[] points;
    LineRenderer lineRenderer;
    List<Vector3> arcPoints;
    public float startAngle=0, endAngle=180, radius=1;
    public int segments=50;

    void createArc (int segments, float startAngle, float endAngle, float radius) {
        //float y = 0;
        arcPoints = new List<Vector3>();
        arcPoints.Add(new Vector3(0, 0, 0));
        float angle = startAngle;
        float arcLength = endAngle - startAngle;
        for (int i = 0; i <= segments; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            float z = -Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            arcPoints.Add(new Vector3(x,0, z));

            angle += (arcLength / segments);
        }
        arcPoints.Add (new Vector3(0, 0, 0));
    }    
    
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        createArc (segments, startAngle, endAngle, radius);
        lineRenderer.positionCount = arcPoints.Count;
        for (int i =0; i < arcPoints.Count; i++) {
            lineRenderer.SetPosition (i, arcPoints[i]);
        }
    }
}
