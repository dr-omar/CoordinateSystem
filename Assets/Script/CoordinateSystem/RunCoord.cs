using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RunCord : MonoBehaviour
{
    public TMP_Text text1, text2, text3;
    ToggleGroup toggleGroup;
    public Transform Sphere;
    void Start()
    {
        toggleGroup = GetComponent<ToggleGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void togglePressed ()
    {
        //Toggle[] toggles = GetComponentInChildren<
        //foreach (var t in toggles)
        Toggle toggle = toggleGroup.GetFirstActiveToggle();
        string t = toggle.GetComponentInChildren<Text>().text;
        //Debug.Log("toggle.name="+toggle.name + "  t=" + t+"ddddddddd");
        float x = Sphere.localPosition.x, y = Sphere.localPosition.y, z = Sphere.localPosition.z;
        if (t == "Cartesian Coordinate") {
            CordManager.CordDirection = 0;
            text1.text = "X=" + x.ToString("#.##");
            text2.text = "Y=" + y.ToString("#.##");
            text3.text = "Z=" + z.ToString("#.##");
        }
        if (t == "Sphirical Coordinate") {
            CordManager.CordDirection = 1;
            float r = Mathf.Sqrt (x*x + y*y + z*z);
            float r1 = Mathf.Sqrt (x*x + z*z);
            float theta = 180.0f / Mathf.PI * Mathf.Acos (y/r); // Mathf.Atan (r1/y); // Acos (z/r)
            float phi = 180.0f / Mathf.PI * Mathf.Acos (x/r1); // Mathf.Atan (z/x); // Acos (x/r1)
            text1.text = "r=" + r.ToString("#.##");
            text2.text = "θ=" + theta.ToString("#.##");
            text3.text = "Φ=" + phi.ToString ("#.##");
        }
        if (t == "Cylindrical Coordinate") {
            CordManager.CordDirection = 2;
            //float r = Mathf.Sqrt (x*x + y*y + z*z);
            float r1 = Mathf.Sqrt (x*x + z*z);
            float theta = 180.0f / Mathf.PI * Mathf.Acos (x/r1); // Mathf.Atan (r1/y); // Acos (z/r)
            text1.text = "r=" + r1.ToString("#.##");
            text2.text = "θ=" + theta.ToString("#.##");
            text3.text = "Z=" + y.ToString("#.##");
        }
    }
}
