using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class sphereScript : MonoBehaviour
{
    public TMP_Text xText, yText, zText; //, text2, text3, text4;
    //public TMP_Text xSliderText, ySliderText, zSliderText; //, text2, text3, text4;
    public GameObject center;

    public Transform startPoint;
    public Transform endPoint;
    public Color color = Color.white;
    public float lineWidth = 0.1f;

    private LineRenderer lineRenderer;

    Vector3 p;
    Vector3 xyPlane, xzPlane, yzPlane;
    //public Vector3 centerPosition;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        //xyPlane = new GameObject("xyPlane");
        //xzPlane = new GameObject("xzPlane");
        //yzPlane = new GameObject("yzPlane");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3[] positions = new Vector3[8];
        int posLength = 8;
        //centerPosition = endPoint.position;
        //p = gameObject.transform.position - center.transform.position;
        p =  gameObject.transform.localPosition;
        int direction = CordManager.CordDirection;
        //Debug.Log (direction);
        if (direction == 0) {
            //p = new Vector3 (p.x, p.z, p.y);
            /*
            text1.text = p.ToString("#.##");
            xSliderText.text = p.x.ToString("#.##");
            ySliderText.text = p.z.ToString("#.##");
            zSliderText.text = p.y.ToString("#.##");

            */
            xText.text = "X";
            yText.text = "Y";
            zText.text = "Z";

        
            p = gameObject.transform.position;
            xyPlane = new Vector3 (p.x, endPoint.position.y, p.z);

            xzPlane = new Vector3 (xyPlane.x, xyPlane.y, endPoint.position.z); // 815f);
            yzPlane = new Vector3 (endPoint.position.x, xyPlane.y, xyPlane.z);

            positions[0] = startPoint.position;
            positions[1] = xyPlane;

            positions[2] = xyPlane;
            positions[3] = xzPlane;

            positions[4] = xyPlane;
            positions[5] = yzPlane;
            posLength = 6;
        }
        if (direction == 1) {
            //xText.text = "r";
            //yText.text = "θ";
            //zText.text = "Φ";
            float r = p.magnitude;

            p = gameObject.transform.position;
            xyPlane = new Vector3(p.x, endPoint.position.y, p.z);
            float r_xyPlane = xyPlane.magnitude;
            float theta = Mathf.Acos (p.y / r_xyPlane); // Mathf.Atan (r_xyPlane/p.z);

            float phi = Mathf.Acos (p.x/r);     //Mathf.Atan (xyPlane.z / xyPlane.x);

            p = new Vector3 (r, theta * 180.0f/Mathf.PI, phi * 180.0f/Mathf.PI);

            //xzPlane = new Vector3(xyPlane.x, xyPlane.y, endPoint.position.z);
            //yzPlane = new Vector3(endPoint.position.x, xyPlane.y, xyPlane.z);

            positions[0] = startPoint.position;
            positions[1] = endPoint.position;

            positions[2] = xyPlane;
            positions[3] = endPoint.position; //center.transform.position;

            positions[4] = startPoint.position;
            positions[5] = xyPlane;

            /*
            text1.text = p.ToString();
            xSliderText.text = p.x.ToString("#.##");
            ySliderText.text = p.z.ToString("#.##");
            zSliderText.text = p.y.ToString("#.##");
            */

            posLength = 6;
        }
        if (direction == 2) {
            //xText.text = "r";
            //yText.text = "θ";
            //zText.text = "Z";
            float r = p.magnitude;

            p = gameObject.transform.position;
            xyPlane = new Vector3(p.x, endPoint.position.y, p.z);
            float r_xyPlane = xyPlane.magnitude;

            float theta = Mathf.Acos (p.x / r_xyPlane); // Mathf.Atan (xyPlane.z / xyPlane.x);

            p = new Vector3 (r_xyPlane, theta*180.0f/Mathf.PI, endPoint.position.y);

            //xzPlane = new Vector3(xyPlane.x, xyPlane.y, endPoint.position.z);
            //yzPlane = new Vector3(endPoint.position.x, xyPlane.y, xyPlane.z);

            //positions[0] = startPoint.position;
            //positions[1] = endPoint.position;


            positions[0] = startPoint.position;
            positions[1] = xyPlane;

            positions[2] = xyPlane;
            positions[3] = endPoint.position;
            /*
            text1.text = p.ToString();
            xSliderText.text = p.x.ToString("#.##");
            ySliderText.text = p.z.ToString("#.##");
            zSliderText.text = p.y.ToString("#.##");
            */
            posLength = 4;
        }
        lineRenderer.positionCount = posLength;
        lineRenderer.SetPositions (positions);

        //lineRenderer.SetPosition(0, startPoint.position);
        //lineRenderer.SetPosition(1, endPoint.position);

        

        //lineRenderer.SetPosition (2, startPoint.position);
        //lineRenderer.SetPosition (3, xyPlane.transform.position);
    }
}
