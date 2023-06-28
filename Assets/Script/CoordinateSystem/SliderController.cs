using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderController : MonoBehaviour
{
    public Transform Sphere;
    public Slider Slider;
    public float multiplier;
    public float r_multiplier = 1.0f;
    public float theta_multiplier = Mathf.PI/180.0f;
    public float phi_multiplier = Mathf.PI / 180.0f;
    public TMP_Text text, sphereText; //, text1;
    public TMP_Text tmpText;


    private float v, dv, oldV=0;
    private Vector3 p, d;
    private float disp_theta, disp_phi;


    // Start is called before the first frame update
    void Start()
    {
        //float x = Sphere.localPosition.x, y = Sphere.localPosition.y, z = Sphere.localPosition.z;
    }

    // Update is called once per frame
    public void SliderChange(float slider_value)
    {
        float x = Sphere.localPosition.x, y = Sphere.localPosition.y, z = Sphere.localPosition.z;
        v = slider_value;
        dv = v;
        oldV = v;
        v = 0;
        if (CordManager.CordDirection == 0)
        { // cartesian coordinates
            
            if (Slider.name == "X_Slider")
            {
                Slider.minValue = 0.1f;
                Slider.maxValue = 5;
                x = dv;
                text.text = "X=" + x.ToString("#.##");
            }
            if (Slider.name == "Y_Slider")
            {
                Slider.minValue = 0.1f;
                Slider.maxValue = 5;
                z = dv;
                text.text = "Y=" + z.ToString("#.##");

            }
            if (Slider.name == "Z_Slider")
            {
                Slider.minValue = 0.1f;
                Slider.maxValue = 5;
                y = dv;
                text.text = "Z=" + y.ToString("#.##");
            }
            p = new Vector3(x, y, z);
            Sphere.localPosition = p;
            d = new Vector3(x, z, y);
            sphereText.text = d.ToString("#.##");
        }
        if (CordManager.CordDirection == 1)
        { // sphrical coordinates

            float r = Mathf.Sqrt(x * x + y * y + z * z);
            float r1 = Mathf.Sqrt(x * x + z * z);
            float theta = Mathf.Acos(y / r); // Mathf.Atan (r1/y); // Acos (z/r)
            float phi = Mathf.Acos(x / r1); // Mathf.Atan (z/x); // Acos (x/r1)
            disp_phi = phi * 180.0f / Mathf.PI;
            disp_theta = theta * 180.0f / Mathf.PI;
            if (Slider.name == "X_Slider")
            { // r

                r = dv * r_multiplier;
                Slider.minValue = 10;
                Slider.maxValue = 30;
                y = r * Mathf.Cos(theta);
                r1 = r * Mathf.Sin(theta);
                x = r1 * Mathf.Cos(phi);
                z = r1 * Mathf.Sin(phi);
                text.text = "r=" + r.ToString("#.##");
            }
            if (Slider.name == "Y_Slider")
            { // theta
                Slider.minValue = 0.1f;
                Slider.maxValue = Mathf.PI-0.1f;
                //Debug.Log(theta);
                theta = dv;
                //Debug.Log("theta=" + theta.ToString());
                y = r * Mathf.Cos(theta);
                r1 = r * Mathf.Sin(theta);
                x = r1 * Mathf.Cos(phi);
                z = r1 * Mathf.Sin(phi);
                disp_theta = theta * 180.0f / Mathf.PI;
                text.text = "θ=" + disp_theta.ToString("#.##");
                tmpText.text = r.ToString();

            }
            if (Slider.name == "Z_Slider")
            { // phi
                Slider.minValue = 0;
                Slider.maxValue = 2*Mathf.PI;
                phi = dv;
                //y = r * Mathf.Cos(theta);
                //r1 = r * Mathf.Sin (theta);
                x = r1 * Mathf.Cos(phi);
                z = r1 * Mathf.Sin(phi);
                disp_phi = phi * 180.0f / Mathf.PI;
                text.text = "Φ=" + disp_phi.ToString("#.##");
            }
            p = new Vector3(x, y, z);
            Sphere.localPosition = p;
            d = new Vector3(r, disp_theta, disp_phi);
            sphereText.text = d.ToString("#.##");
        }

    }

        void Update () {
        /*
        float x = Sphere.localPosition.x, y = Sphere.localPosition.y, z = Sphere.localPosition.z;
        //text1.text = "V=" + dv.ToString();
        if (CordManager.CordDirection == 0) { // cartesian coordinates
            if (Slider.name == "X_Slider") {
                //if (x+dv<16 && x+dv>-18) 
                x = x+dv;
                text.text = "X=" + x.ToString("#.##");
            }
            if (Slider.name == "Y_Slider") {
                //if (y+dv<23 && y+dv>5) 
                z = z+dv;
                text.text = "Y=" + z.ToString("#.##");
                
            }
            if (Slider.name == "Z_Slider") {
                //if (z+dv < 15 && z+dv>-12) 
                y+=dv;
                text.text = "Z=" + y.ToString("#.##");
            }
            p = new Vector3 (x, y, z);
            Sphere.localPosition = p;
            d = new Vector3 (x, z, y);
            sphereText.text = d.ToString("#.##");

        }
        if (CordManager.CordDirection == 1) { // sphrical coordinates

            float r = Mathf.Sqrt (x*x + y*y + z*z);
            float r1 = Mathf.Sqrt (x*x + z*z);
            float theta = Mathf.Acos (y/r); // Mathf.Atan (r1/y); // Acos (z/r)
            float phi = Mathf.Acos (x/r1); // Mathf.Atan (z/x); // Acos (x/r1)
            disp_phi = phi * 180.0f / Mathf.PI;
            disp_theta = theta * 180.0f / Mathf.PI;
            if (Slider.name == "X_Slider") { // r
                
                r = r + dv * r_multiplier;

                Slider.value = r;
                Slider.minValue = 10;
                Slider.maxValue = 30;
                Debug.Log(r);
                y = r * Mathf.Cos(theta);
                r1 = r * Mathf.Sin (theta);
                x = r1 * Mathf.Cos (phi);
                z = r1 * Mathf.Sin (phi);
                text.text = "r=" + r.ToString("#.##");
            }
            if (Slider.name == "Y_Slider") { // theta
                Slider.value = theta;
                Slider.minValue = 0;
                Slider.maxValue = 179;
                theta = theta + dv * theta_multiplier;
                y = r * Mathf.Cos(theta);
                r1 = r * Mathf.Sin (theta);
                x = r1 * Mathf.Cos (phi);
                z = r1 * Mathf.Sin (phi);
                disp_theta = theta * 180.0f / Mathf.PI;
                text.text = "θ=" + disp_theta.ToString("#.##");
                
            }
            if (Slider.name == "Z_Slider") { // phi
                Slider.value = phi;
                Slider.minValue = 0;
                Slider.maxValue = 359;
                phi = phi + dv * phi_multiplier;
                //y = r * Mathf.Cos(theta);
                //r1 = r * Mathf.Sin (theta);
                x = r1 * Mathf.Cos (phi);
                z = r1 * Mathf.Sin (phi);
                disp_phi = phi * 180.0f / Mathf.PI;
                text.text = "Φ=" + disp_phi.ToString("#.##");
            }

            p = new Vector3 (x, y, z);
            Sphere.localPosition = p;
            d = new Vector3 (r, disp_theta, disp_phi);
            sphereText.text = d.ToString("#.##");

        }
        if (CordManager.CordDirection == 2) { // cylinder cordinates
        
            //float r = Mathf.Sqrt (x*x + y*y + z*z);
            float r1 = Mathf.Sqrt (x*x + z*z);
            float theta = Mathf.Acos (x/r1); // Mathf.Atan (z/x); // Acos (z/r)
            
            if (Slider.name == "X_Slider") { // r
                r1 = r1 + dv * r_multiplier;
                x = r1 * Mathf.Cos (theta);
                z = r1 * Mathf.Sin (theta);
                text.text = "r=" + r1.ToString("#.##");
            }
            if (Slider.name == "Y_Slider") { // theta
                Slider.minValue = 0;
                Slider.maxValue = 179;
                theta = theta + dv * theta_multiplier;
                x = r1 * Mathf.Cos (theta);
                z = r1 * Mathf.Sin (theta);
                disp_theta = theta * 180.0f / Mathf.PI;
                text.text = "θ=" + disp_theta.ToString("#.##");
                
            }
            if (Slider.name == "Z_Slider") { // phi
                y = y + dv *multiplier;
                //x = r1 * Mathf.Cos (theta);
                //z = r1 * Mathf.Sin (theta);
                text.text = "Z=" + y.ToString("#.##");
            }
            
            p = new Vector3 (x, y, z);
            Sphere.localPosition = p;
            d = new Vector3 (r1, disp_theta, y);
            sphereText.text = d.ToString("#.##");
            
        }
        dv=0.0f;
        */
    }
}
