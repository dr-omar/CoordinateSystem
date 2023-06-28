using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CoordCanvasManager : MonoBehaviour
{
    // public variables
    public TMP_Text xText, yText, zText;  // slider text
    public Transform Sphere; // sphere point
    public Transform center;
    public TMP_Text SphereText;
    public Slider xSlider, ySlider, zSlider;

    public GameObject xUnitVector, yUnitVector, zUnitVector;

    public GameObject rUnitVector, thetaUnitVector, sPhiUnitVector;
    public GameObject r1UnitVector, cPhiUnitVector, cZUnitVector;
    private bool cZPos = true;
    
    
    public ToggleGroup toggleGroup;
    public ToggleGroup xPlaneToggleGroup, yPlaneToggleGroup, zPlaneToggleGroup;
    //public Toggle planeToggle1;
    [HideInInspector]
    public Vector3 originPosition = Vector3.zero, centerPosition, cartesianPos, currentPosition;
    //public TMP_Text txt, txt1, txt2, txt3, txt4;
    
    Vector3 sphiricalPos, cylindricalPos;
    
    private bool xSliderActive, ySliderActive, zSliderActive;
    
    private float xMax = 8, yMax = 8, zMax = 8;
    private float xMin = -5, yMin = -5, zMin = -5;
    private float rMax, thetaMax = 179.5f, phiMax = 359.5f, r1Max;

    bool onceSphere=true, onceCylind=true;
    //Quaternion rRotOld = Quaternion.Euler (new Vector3 (0, 0, 0));
    Vector3 rRotOld = Vector3.zero, thetaRotOld = Vector3.zero, sPhiRotOld = Vector3.zero;
    Vector3 r1RotOld = Vector3.zero, cPhiRotOld = Vector3.zero, cZRotOld = Vector3.zero;
    
    struct SphericalCoordinate {
        public float r, r1, theta, phi;
    };
    SphericalCoordinate sCoord;
    struct CylindericalCoordinate {
        public float r1, phi, z;
    };
    CylindericalCoordinate cCoord;

    // Cartesian Planes:
    public GameObject xPlane, yPlane, zPlane;
    // Sepherical planes:
    public GameObject rPlane, thetaPlane, phiPlane;
    // Cylindrical coordinates Planes and cylinder
    public GameObject r1Plane, cPhiPlane, cZPlane;
    public GameObject phiLine;

    private int coordChoice; // corrdinate choice: cartesian, sepherical, cylinderical

    LineRenderer cPhiPlaneLineRenderer; // line renderer reference for phi
    List<Vector3> arcPoints; // for drawing arc points

    Toggle planeToggle1, planeToggle2, planeToggle3; // reference to plane toggles
    Text toggle_txt1, toggle_txt2, toggle_txt3; // for changing the plane text toggle

    GameObject phiObj, thetaObj;

    float radianToDegree (float angle) {
        return angle * 180.0f / Mathf.PI;
    }

    float degreeToRadian (float angle) {
        return angle * Mathf.PI / 180.0f;
    }

    void sphericalCompute (Vector3 currentPosition) {
        sCoord.r = Mathf.Sqrt (Mathf.Pow(currentPosition.x, 2) + Mathf.Pow(currentPosition.y, 2) + Mathf.Pow(currentPosition.z, 2));
        sCoord.r1 = Mathf.Sqrt (Mathf.Pow (currentPosition.x, 2) + Mathf.Pow (currentPosition.z, 2));
        sCoord.theta = radianToDegree(Mathf.Acos (currentPosition.y/sCoord.r)); // Mathf.Atan (r1/y); // Acos (z/r)
        sCoord.phi = radianToDegree(Mathf.Acos (currentPosition.x/sCoord.r1)); // Mathf.Atan (z/x); // Acos (x/r1)
        if (currentPosition.z < 0 && currentPosition.x < 0) sCoord.phi = 360-sCoord.phi;
        if (currentPosition.z < 0 && currentPosition.x > 0) sCoord.phi = 360-sCoord.phi;
    }

    void cylindericalCompute (Vector3 currentPosition) {
        cCoord.r1 = Mathf.Sqrt (Mathf.Pow (currentPosition.x, 2) + Mathf.Pow (currentPosition.z, 2));
        cCoord.phi = radianToDegree (Mathf.Acos (currentPosition.x / cCoord.r1));
        cCoord.z = currentPosition.y;
        if (currentPosition.z < 0 && currentPosition.x < 0) cCoord.phi = 360-cCoord.phi;
        if (currentPosition.z < 0 && currentPosition.x > 0) cCoord.phi = 360-cCoord.phi;
    }

    void createThetaLine (Vector3 currentPos) {
        
        arcPoints = new List<Vector3>();
        arcPoints.Add(new Vector3(0, 0, 0));
        arcPoints.Add(new Vector3(0, currentPos.y, 0));
        arcPoints.Add(new Vector3(currentPos.x, currentPos.y, currentPos.z));
    }


    void createArc (int segments, float startAngle, float endAngle, float radius) {
        //float y = 0;
        arcPoints = new List<Vector3>();
        arcPoints.Add(new Vector3(0, 0, 0));
        float angle = startAngle;
        float arcLength = endAngle - startAngle;
        //Debug.Log ("startAngle=" + startAngle.ToString() + ", endAngle=" + endAngle.ToString());
        for (int i = 0; i <= segments; i++)
        {
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            float z = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;

            arcPoints.Add(new Vector3(x,0, z));

            angle += (arcLength / segments);
        }
        //arcPoints.Add (new Vector3(0, 0, 0));
    }

    /*
    void spherePlanes (bool show) {
        rPlane.SetActive(show);
        thetaPlane.SetActive(show);
        phiPlane.SetActive(show);
    }
    void cylinderPlanes (bool show) {
        r1Plane.SetActive(show);
        cPhiPlane.SetActive(show);
        cZPlane.SetActive(show);
    }
    */


    void Start()
    {
        originPosition = Sphere.localPosition;
        centerPosition = center.localPosition;
        cartesianPos = originPosition - centerPosition;
        SphereText.text = cartesianPos.ToString();
        

        xText.text = "X=" + cartesianPos.x.ToString("#.##");
        yText.text = "Y=" + cartesianPos.z.ToString("#.##");
        zText.text = "Z=" + cartesianPos.y.ToString("#.##");

        sphericalCompute (cartesianPos);
        cCoord.r1 = sCoord.r1;
        cCoord.phi = sCoord.phi;
        cCoord.z = currentPosition.z;
        //cylindericalCompute (cartesianPos);

        //float r = Mathf.Sqrt (Mathf.Pow(cartesianPos.x, 2) + Mathf.Pow(cartesianPos.y, 2) + Mathf.Pow(cartesianPos.z, 2));
        //float r1 = Mathf.Sqrt (Mathf.Pow (cartesianPos.x, 2) + Mathf.Pow (cartesianPos.z, 2));
        //float theta = radianToDegree(Mathf.Acos (cartesianPos.y/r)); // Mathf.Atan (r1/y); // Acos (z/r)
        //float phi = radianToDegree(Mathf.Acos (cartesianPos.x/r1)); // Mathf.Atan (z/x); // Acos (x/r1)

        sphiricalPos = new Vector3 (sCoord.r, sCoord.theta, sCoord.phi);
        cylindricalPos = new Vector3 (cCoord.r1, cCoord.phi, cCoord.z);

        //CartUnitVectorDirection();
        SpherUnitVectorDirection(sCoord);
        CylinderUnitVectorDirection (cCoord);

        //cartPlanes (false);
        //spherePlanes (false);
        //cylinderPlanes (false);
        xPlane.SetActive (false);
        rPlane.SetActive (false);
        r1Plane.SetActive (false);

        //txt3.text = sphiricalPos.ToString();

        //txt.text = cartesianPos.x.ToString() + ", "  + cartesianPos.y.ToString() + ", " + cartesianPos.z.ToString();
        //txt1.text = r.ToString() + ", " + theta.ToString() + ", " + phi.ToString();
        
        //x_sliderChange = false; y_sliderChange = false; z_sliderChange = false;
        
        xSlider.minValue = xMin - cartesianPos.x;
        xSlider.maxValue = xMax - cartesianPos.x;
        xSlider.value = 0;
        ySlider.minValue = yMin - cartesianPos.z;
        ySlider.maxValue = yMax - cartesianPos.z;
        ySlider.value = 0;
        zSlider.minValue = zMin - cartesianPos.y;
        zSlider.maxValue = zMax - cartesianPos.y;
        zSlider.value = 0;

        rMax = Mathf.Sqrt (Mathf.Pow(xMax, 2) + Mathf.Pow(yMax, 2) + Mathf.Pow (zMax, 2));
        r1Max = Mathf.Sqrt (Mathf.Pow(xMax, 2) + Mathf.Pow (zMax, 2));

        xSliderActive = true;
        ySliderActive = true;
        zSliderActive = true;
        
        // unit vectors:
        // spherical unit vectors
        DisplaySpherUnitVector (false);
        
        // cylinderical unit vectors
        DisplayCylinderUnitVector (false);

        // line renderer for phi in cylinder coordinate
        cPhiPlaneLineRenderer = phiLine.GetComponent<LineRenderer>();

        // line renderer for phi in sepherical coordinate

        planeToggle1 = xPlaneToggleGroup.transform.GetChild(0).gameObject.GetComponent<Toggle>();
        planeToggle2 = yPlaneToggleGroup.transform.GetChild(0).gameObject.GetComponent<Toggle>();
        planeToggle3 = zPlaneToggleGroup.transform.GetChild(0).gameObject.GetComponent<Toggle>();

        toggle_txt1 = planeToggle1.GetComponentInChildren<Text>();
        toggle_txt2 = planeToggle2.GetComponentInChildren<Text>();
        toggle_txt3 = planeToggle3.GetComponentInChildren<Text>();

    }
    //public GameObject g;
    // Update is called once per frame
    void Update()
    {
        //g.transform.Rotate (0, 0, 10 * Time.deltaTime);
    }
    /*
    void CartUnitVectorDirection () {
        // Unit vector direction:
        if ((currentPosition.x < 0 && xPos) || (currentPosition.x > 0 && !xPos)) {
            xUnitVector.transform.Rotate (0, 0, 180);
            xPos = !xPos;
        }
        if ((currentPosition.y < 0 && yPos) || (currentPosition.y > 0 && !yPos)) {
            yUnitVector.transform.Rotate (0, 0, 180);
            yPos = !yPos;
        }
        if ((currentPosition.z < 0 && zPos) || (currentPosition.z > 0 && !zPos)) {
            zUnitVector.transform.Rotate (180, 0, 0);
            zPos = !zPos;
        }
    }
    */
    void DisplayCartUnitVector (bool show) {
        xUnitVector.SetActive (show);
        yUnitVector.SetActive (show);
        zUnitVector.SetActive (show);
    }

    float wrapAngle (float angle) {
        angle %= 360;
        if (angle < 0 || angle > 180) {
            angle -= 360;
        }
        return angle;
    }

    void SpherUnitVectorDirection (SphericalCoordinate s) {
        
        float rtheta = degreeToRadian (s.theta);
        float rphi = degreeToRadian (s.phi);
        // Unit vector direction:
        Vector3 ur =  new Vector3 (Mathf.Sin(rtheta) * Mathf.Cos(rphi), Mathf.Cos(rtheta), Mathf.Sin(rtheta) * Mathf.Sin(rphi));
        Vector3 ut = new Vector3 (Mathf.Cos(rtheta) * Mathf.Cos(rphi), -Mathf.Sin(rtheta), Mathf.Cos(rtheta) * Mathf.Sin(rphi));
        Vector3 up = new Vector3 (-Mathf.Sin (rphi), 0, Mathf.Cos (rphi));

        //Debug.Log ("ur=" + Mathf.Sqrt (Mathf.Pow (ur.x, 2) + Mathf.Pow (ur.y, 2) + Mathf.Pow (ur.z, 2)));
        //Debug.Log ("ut=" + Mathf.Sqrt (Mathf.Pow (ut.x, 2) + Mathf.Pow (ut.y, 2) + Mathf.Pow (ut.z, 2)));
        //Debug.Log ("up=" + Mathf.Sqrt (Mathf.Pow (up.x, 2) + Mathf.Pow (up.y, 2) + Mathf.Pow (up.z, 2)));
        //Debug.Log ("ut vector=" + ut.ToString());
        //Debug.Log ("up vector=" + up.ToString());
        
        //Debug.Log("eulerAngles before: " + rUnitVector.transform.eulerAngles);
        //rUnitVector.transform.Rotate (-rUnitVector.transform.eulerAngles);
        //Debug.Log("eulerAngles after: " + rUnitVector.transform.eulerAngles);
        Vector3 rRotNew = new Vector3 (0, -s.phi, 90-s.theta);
        Vector3 delta = rRotNew - rRotOld;
        //Debug.Log ("rRotOld = " + rRotOld.ToString());
        //Debug.Log ("rRotNew = " + rRotNew.ToString());
        //Debug.Log ("difference = " + delta_r.ToString());
        rUnitVector.transform.Rotate (delta);
        rRotOld = rRotNew;

        float ydirection = -1, zdirection=-1;
        if (ut.z>0) ydirection = -1;
        else ydirection = 1;
        if (ut.y>0) zdirection = 1;
        else zdirection = -1;

        float yt = Mathf.Acos (ut.x/Mathf.Sqrt(Mathf.Pow(ut.x, 2) + Mathf.Pow(ut.z, 2)));
        float zt = Mathf.Asin (ut.y);
        //Debug.Log ("yt=" + yt.ToString());
        //Debug.Log ("zt=" + zt.ToString());

        Vector3 thetaRotNew = new Vector3 (0, ydirection * radianToDegree (yt), radianToDegree(zt));
        delta = thetaRotNew - thetaRotOld;
        thetaUnitVector.transform.Rotate (delta);
        thetaRotOld = thetaRotNew;

        yt = Mathf.Acos (up.x/(Mathf.Sqrt(Mathf.Pow(up.x, 2) + Mathf.Pow(up.z, 2))));
        zt = Mathf.Asin (up.y);
        if (up.z>0) ydirection = -1;
        else ydirection = 1;

        if (up.y>=0) zdirection = 1;
        else zdirection = -1;

        Vector3 sPhiRotNew = new Vector3 (0, ydirection * radianToDegree(yt), zdirection * radianToDegree(zt));
        //Debug.Log (sPhiRotNew);
        delta = sPhiRotNew - sPhiRotOld;
        sPhiUnitVector.transform.Rotate (delta);
        sPhiRotOld = sPhiRotNew;
        //sPhiUnitVector.Transform.Rotate (0, Mathf.)

        //Debug.Log ("Rotate");
        //Debug.Log("x, y, z: " + rUnitVector.transform.localEulerAngles);

    }

    void DisplaySpherUnitVector (bool show) {
        rUnitVector.SetActive (show);
        thetaUnitVector.SetActive (show);
        sPhiUnitVector.SetActive (show);
    }

    void CylinderUnitVectorDirection (CylindericalCoordinate c) {
        float rphi = degreeToRadian (c.phi);
        // Unit vector direction:
        Vector3 ur =  new Vector3 (Mathf.Cos(rphi), 0, Mathf.Sin(rphi));
        Vector3 up = new Vector3 (-Mathf.Sin(rphi), 0, Mathf.Cos(rphi));
        Vector3 uz = new Vector3 (0, 0, 1);

        //Debug.Log ("ur=" + Mathf.Sqrt (Mathf.Pow (ur.x, 2) + Mathf.Pow (ur.y, 2) + Mathf.Pow (ur.z, 2)));
        //Debug.Log ("up=" + Mathf.Sqrt (Mathf.Pow (up.x, 2) + Mathf.Pow (up.y, 2) + Mathf.Pow (up.z, 2)));
        //Debug.Log ("uz=" + Mathf.Sqrt (Mathf.Pow (uz.x, 2) + Mathf.Pow (uz.y, 2) + Mathf.Pow (uz.z, 2)));
        //Debug.Log ("up vector=" + up.ToString());
        //Debug.Log ("uz vector=" + uz.ToString());
        //Debug.Log ("phi=" + c.phi.ToString());

        Vector3 r1RotNew;
        if (ur.z >= 0) r1RotNew = new Vector3 (0, -c.phi, 0);
        else r1RotNew = new Vector3 (0, c.phi, 0);

        //Debug.Log ("r1RotNew=" + r1RotNew.ToString());
        Vector3 delta = r1RotNew - r1RotOld;
        r1UnitVector.transform.Rotate (delta);

        Vector3 cPhiRotNew;
        
        float direction = -1;
        if (up.z >= 0 ) direction = -1;
        else direction = 1;
        cPhiRotNew = new Vector3(0, radianToDegree (direction * Mathf.Acos (up.x)), radianToDegree (Mathf.Asin (up.y)));
        //Debug.Log ("cPhiRotNew=" + cPhiRotNew.ToString());
        delta = cPhiRotNew - cPhiRotOld;
        cPhiUnitVector.transform.Rotate (delta);

        if ((up.z>0 && !cZPos) || (up.z<0 && cZPos)) {
            cZUnitVector.transform.Rotate (0.0f, 0.0f, 180.0f);
            cZPos = !cZPos;
        }

        r1RotOld = r1RotNew;
        cPhiRotOld = cPhiRotNew;
        //cZRotOld = cZRotNew;

    }

    void DisplayCylinderUnitVector (bool show) {
        r1UnitVector.SetActive (show);
        cPhiUnitVector.SetActive (show);
        cZUnitVector.SetActive (show);
    }

    public void togglePressed ()
    {
        
        Toggle toggle = toggleGroup.GetFirstActiveToggle();
        string t = toggle.GetComponentInChildren<Text>().text;
        currentPosition = Sphere.localPosition - centerPosition;
        //txt.text = currentPosition.ToString();
        //txt2.text = "togglePressed: " + currentPosition.x.ToString("#.##") + "," + currentPosition.y.ToString("#.##") + "," + currentPosition.z.ToString("#.##");
        if (t == "Cartesian Coordinate") {
            coordChoice = 0;
            toggle_txt1.text = "X Plane";
            toggle_txt2.text = "Y Plane";
            toggle_txt3.text = "Z Plane";
            //Debug.Log ("Cartesian Coordinate");
            CordManager.CordDirection = 0;
            xText.text = "X=" + currentPosition.x.ToString("0.##");
            yText.text = "Y=" + currentPosition.z.ToString("0.##");
            zText.text = "Z=" + currentPosition.y.ToString("0.##");
            SphereText.text = currentPosition.ToString();

            xSliderActive = false;
            ySliderActive = false;
            zSliderActive = false;

            xSlider.minValue = xMin - cartesianPos.x;;
            xSlider.maxValue = xMax - cartesianPos.x;
            xSlider.value = currentPosition.x - cartesianPos.x;
            //txt3.text = (currentPosition.x - cartesianPos.x).ToString();

            ySlider.minValue = yMin - cartesianPos.z;
            ySlider.maxValue = yMax - cartesianPos.z;
            ySlider.value = currentPosition.z - cartesianPos.z;

            zSlider.minValue = zMin - cartesianPos.y;
            zSlider.maxValue = zMax - cartesianPos.y;
            zSlider.value = currentPosition.y - cartesianPos.y;

            // Unit vector direction:
            //CartUnitVectorDirection();
            DisplayCartUnitVector(true);
            DisplaySpherUnitVector(false);
            DisplayCylinderUnitVector(false);

            //cartPlanes (false);
            //spherePlanes (false);
            //cylinderPlanes (false);
            xPlane.SetActive (false);
            rPlane.SetActive (false);
            r1Plane.SetActive(false);

            onceSphere=true;
            onceCylind=true;

            xSliderActive = true;
            ySliderActive = true;
            zSliderActive = true;
            //txt1.text = currentPosition.ToString();
            //txt2.text = cartesianPos.ToString();
        }
        if (t == "Spherical Coordinate") {
            coordChoice = 1;
            toggle_txt1.text = "r Plane";
            toggle_txt2.text = "θ Plane";
            toggle_txt3.text = "Φ Plane";
            //Debug.Log ("Sphirical Coordinate");
            CordManager.CordDirection = 1;
            // Compute Sphirical Coordinates r, theta, phi
            float r = Mathf.Sqrt (Mathf.Pow (currentPosition.x, 2) + Mathf.Pow (currentPosition.y, 2) + Mathf.Pow (currentPosition.z, 2));
            float r1 = Mathf.Sqrt (Mathf.Pow  (currentPosition.x, 2) + Mathf.Pow (currentPosition.z, 2));
            float theta = radianToDegree(Mathf.Acos (currentPosition.y/r)); // Mathf.Atan (r1/y); // Acos (z/r)
            float phi = radianToDegree(Mathf.Acos (currentPosition.x/r1)); // Mathf.Atan (z/x); // Acos (x/r1)
            xText.text = "r=" + r.ToString("0.##");
            yText.text = "θ=" + theta.ToString("0.##");
            zText.text = "Φ=" + phi.ToString ("0.##");
            
            // Compute Sphirical position
            Vector3 pos = new Vector3 (r, theta, phi);
            SphereText.text = pos.ToString();

            xSliderActive = false;
            ySliderActive = false;
            zSliderActive = false;
            
            xSlider.minValue = 0.1f - sphiricalPos.x;
            xSlider.maxValue = rMax - sphiricalPos.x;
            xSlider.value = r - sphiricalPos.x;

            //float temp1 = 180.0f - sphiricalPos.y;
            //float temp2 = theta - sphiricalPos.y;
            //float temp3 = -sphiricalPos.y;

            ySlider.minValue = 0.1f - sphiricalPos.y;
            ySlider.maxValue = thetaMax - sphiricalPos.y;
            ySlider.value = theta - sphiricalPos.y;
            //txt3.text = "sphirical: " + temp3.ToString() + ", " + temp1.ToString() + ", " + temp2.ToString();
            //txt3.text = sphiricalPos.ToString();
            
            zSlider.minValue = 0.1f - sphiricalPos.z;
            zSlider.maxValue = phiMax - sphiricalPos.z;
            zSlider.value = phi - sphiricalPos.z;

            DisplayCartUnitVector (false);
            //if (onceSphere) {
                //SpherUnitVectorDirection (r, theta, phi);
                //onceSphere = false;
            //}
            DisplaySpherUnitVector(true);
            DisplayCylinderUnitVector(false);

            onceCylind=true;

            xSliderActive = true;
            ySliderActive = true;
            zSliderActive = true;
            //txt3.text = currentPosition.ToString();
            //txt4.text = cartesianPos.ToString();
            
        }
        if (t == "Cylindrical Coordinate") {
            coordChoice = 2;
            toggle_txt1.text = "r Plane";
            toggle_txt2.text = "Φ Plane";
            toggle_txt3.text = "Z Plane";
            CordManager.CordDirection = 2;
            //Debug.Log ("Cylinderical Coordinate");
            // Computer Cylindrical Coordinates
            float r = Mathf.Sqrt (Mathf.Pow (currentPosition.x, 2) + Mathf.Pow (currentPosition.y, 2) + Mathf.Pow (currentPosition.z, 2));
            float r1 = Mathf.Sqrt (Mathf.Pow (currentPosition.x, 2) + Mathf.Pow (currentPosition.z, 2));
            float phi = radianToDegree(Mathf.Acos (currentPosition.x/r1)); // Mathf.Atan (r1/y); // Acos (z/r)
            xText.text = "ρ=" + r1.ToString("0.##");
            yText.text = "Φ=" + phi.ToString("0.##");
            zText.text = "Z=" + currentPosition.y.ToString("0.##");

            // Compuer Sphere Cylindrical position:
            Vector3 pos = new Vector3 (r1, phi, currentPosition.y);
            SphereText.text = pos.ToString();

            xSliderActive = false;
            ySliderActive = false;
            zSliderActive = false;

            xSlider.minValue = 0.1f - cylindricalPos.x;
            xSlider.maxValue = rMax - cylindricalPos.x;
            xSlider.value = r - cylindricalPos.x;

            ySlider.minValue = 0.1f - cylindricalPos.y;
            ySlider.maxValue = phiMax - cylindricalPos.y;
            ySlider.value = phi - cylindricalPos.y;
            
            zSlider.minValue = zMin - cylindricalPos.z;
            zSlider.maxValue = zMax - cylindricalPos.z;
            zSlider.value = currentPosition.y - cylindricalPos.z;

            DisplayCartUnitVector (false);
            DisplaySpherUnitVector(false);
            //if (onceCylind) {
                //CylinderUnitVectorDirection (r1, phi, currentPosition.y);
                //onceCylind = false;
            //}
            DisplayCylinderUnitVector(true);

            onceSphere=true;

            xSliderActive = true;
            ySliderActive = true;
            zSliderActive = true;
            
        }
        PlaneFunc1 ();
        PlaneFunc2 ();
        PlaneFunc3 ();
        //if (coordChoice == 0) xCartPlane (xToggleVar);
        //if (coordChoice == 1) spherePlanes (xToggleVar);
        //if (coordChoice == 2) cylinderPlanes (xToggleVar);

    }

    void showPlane (GameObject plane, ref bool showVar) {
        showVar = !showVar;
        plane.SetActive (showVar);
    }

    private void FillClosedLoop(Color fillColor)
    {
        if (arcPoints == null || arcPoints.Count < 3)
            return;

        // Convert the array of points to a Vector3 array
        
        Vector3[] vertices = new Vector3[arcPoints.Count];
        for (int i = 0; i < arcPoints.Count; i++)
        {
            vertices[i] = new Vector3(arcPoints[i].x, arcPoints[i].y, arcPoints[i].z);
        }
        System.Array.Reverse (vertices);
        // Create triangles to form the closed loop shape
        int[] triangles = new int[3 * (arcPoints.Count - 2)];
        for (int i = 0; i < arcPoints.Count - 2; i++)
        {
            triangles[3 * i] = 0;
            triangles[3 * i + 1] = i + 1;
            triangles[3 * i + 2] = i + 2;
            //Debug.Log (triangles1[3*i].ToString() + ", " + triangles1[3*i+1].ToString() + ", " + triangles1[i*3+2].ToString());
        }
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // Create a new game object with MeshFilter and MeshRenderer components
        phiObj = new GameObject("ClosedLoopObject");
        phiObj.transform.parent = cPhiPlane.transform.parent;
        
        phiObj.AddComponent<MeshFilter>().mesh = mesh;
        phiObj.AddComponent<MeshRenderer>().material.color = fillColor;
        phiObj.transform.localPosition = new Vector3 (0, 13, 0);
        phiObj.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    private void FillClosedLoop2(Color fillColor)
    {
        if (arcPoints == null || arcPoints.Count < 3)
            return;

        // Convert the array of points to a Vector3 array
        
        Vector3[] vertices = new Vector3[arcPoints.Count];
        for (int i = 0; i < arcPoints.Count; i++)
        {
            vertices[i] = new Vector3(arcPoints[i].x, arcPoints[i].y, arcPoints[i].z);
        }
        //System.Array.Reverse (vertices);
        // Create triangles to form the closed loop shape
        int[] triangles = new int[3 * (arcPoints.Count - 2)];
        for (int i = 0; i < arcPoints.Count - 2; i++)
        {
            triangles[3 * i] = 0;
            triangles[3 * i + 1] = i + 1;
            triangles[3 * i + 2] = i + 2;
            //Debug.Log (triangles1[3*i].ToString() + ", " + triangles1[3*i+1].ToString() + ", " + triangles1[i*3+2].ToString());
        }
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // Create a new game object with MeshFilter and MeshRenderer components
        thetaObj = new GameObject("ClosedLoopObject2");
        thetaObj.transform.parent = thetaPlane.transform.parent;
        
        thetaObj.AddComponent<MeshFilter>().mesh = mesh;
        thetaObj.AddComponent<MeshRenderer>().material.color = fillColor;
        thetaObj.transform.localPosition = new Vector3 (0, 13, 0);
        thetaObj.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    void PlaneFunc1 () {
        if (coordChoice==0) {
            if (planeToggle1.isOn) {
                currentPosition = Sphere.localPosition - centerPosition;
                xPlane.transform.localScale = Vector3.zero;
                xPlane.transform.localScale = new Vector3 (0.1f, currentPosition.y, currentPosition.z);

                xPlane.transform.localPosition = new Vector3 (currentPosition.x, centerPosition.y + currentPosition.y / 2, currentPosition.z / 2);
                xPlane.SetActive (true);
                //cPhiPlane.SetActive(false);
                //Debug.Log (Sphere.localPosition);
                //sPhiLine.SetActive(false);
            }
            else xPlane.SetActive (false);
            rPlane.SetActive (false);
            r1Plane.SetActive (false);
            phiLine.SetActive (false);
            Destroy (thetaObj);

        }
        if (coordChoice == 1) {
            if (planeToggle1.isOn) { // draw the sphere in sphereical corrdinate
                currentPosition = Sphere.localPosition - centerPosition;
                sphericalCompute (currentPosition);
                rPlane.transform.localScale = Vector3.zero;
                rPlane.transform.localPosition = new Vector3 (0, centerPosition.y, 0);
                rPlane.transform.localScale = new Vector3 (sCoord.r*2, sCoord.r*2, sCoord.r*2);
                //Debug.Log (rPlane.transform.localPosition);
                
                rPlane.SetActive (true);
                
            }
            else rPlane.SetActive (false);
            xPlane.SetActive (false); 
            r1Plane.SetActive (false);

            createArc (50, 0, sCoord.phi, sCoord.r1);
            Destroy (phiObj);
            FillClosedLoop(Color.blue);
            cPhiPlaneLineRenderer.positionCount = arcPoints.Count;
            for (int i =0; i < arcPoints.Count; i++) {
                cPhiPlaneLineRenderer.SetPosition (i, arcPoints[i]);
            }
            phiLine.SetActive (true);

        }
        if (coordChoice == 2) {
            if (planeToggle1.isOn) { // draw cylinder in cylinderical coordinate
                currentPosition = Sphere.localPosition - centerPosition;
                //Debug.Log ("cylindrPlanes currentPos= " + currentPosition.ToString());
                cylindericalCompute (currentPosition);
                //Debug.Log ("cylinderPlanes: r1=" + sCoord.r1.ToString());
                r1Plane.transform.localScale = Vector3.zero;
                r1Plane.transform.localPosition = new Vector3 (0, centerPosition.y + currentPosition.y/2, 0);
                r1Plane.transform.localScale = new Vector3 (cCoord.r1*2, currentPosition.y/2, cCoord.r1*2);

                r1Plane.SetActive (true);
            }
            else r1Plane.SetActive (false);
            xPlane.SetActive (false);
            rPlane.SetActive(false);
            Destroy (thetaObj);

            createArc (50, 0, cCoord.phi, cCoord.r1);
            Destroy (phiObj);
            
            cPhiPlaneLineRenderer.positionCount = arcPoints.Count;
            for (int i =0; i < arcPoints.Count; i++) {
                cPhiPlaneLineRenderer.SetPosition (i, arcPoints[i]);
            }
            phiLine.SetActive (true);
            FillClosedLoop(Color.blue);
        }
    }

    void PlaneFunc2 () {
        if (coordChoice==0) {
            if (planeToggle2.isOn) {
                currentPosition = Sphere.localPosition - centerPosition;
                yPlane.transform.localScale = Vector3.zero;
                yPlane.transform.localScale = new Vector3 (currentPosition.x, 0.1f, currentPosition.z);

                yPlane.transform.localPosition = new Vector3 (currentPosition.x/2, centerPosition.y + currentPosition.y, currentPosition.z / 2);
                yPlane.SetActive (true);
            }
            else yPlane.SetActive (false);
            thetaPlane.SetActive (false);
            cPhiPlane.SetActive (false);
            phiLine.SetActive (false);
            Destroy (thetaObj);
        }
        if (coordChoice == 1) {
            if (planeToggle2.isOn) {
                createThetaLine (currentPosition);
                Destroy (thetaObj);
                FillClosedLoop2 (Color.blue);
            }
            else Destroy (thetaObj);
            yPlane.SetActive(false);
        }

        if (coordChoice == 2) {
            if (planeToggle2.isOn) {   // phi plane
                cPhiPlane.transform.localPosition = new Vector3 (0, centerPosition.y, 0);
                cPhiPlane.transform.localScale = new Vector3 (Mathf.Abs (cCoord.r1), Mathf.Abs (currentPosition.y), 0.05f);
                cPhiPlane.transform.localEulerAngles = new Vector3(0, -cCoord.phi, 0);
            
                float x = cCoord.r1 * Mathf.Cos (degreeToRadian (cCoord.phi)) + centerPosition.x;
                float y = centerPosition.y + currentPosition.y/2;
                float z = cCoord.r1 * Mathf.Sin (degreeToRadian (cCoord.phi)) + centerPosition.z;

                cPhiPlane.transform.localPosition = new Vector3 (x/2, y, z/2);
            
                cPhiPlane.SetActive (true);
            }
            else cPhiPlane.SetActive (false);
            yPlane.SetActive (false);
            thetaPlane.SetActive (false); 
            Destroy (thetaObj);
        }
    }
    void PlaneFunc3 () {
        if (coordChoice==0) {
            if (planeToggle3.isOn) {
                currentPosition = Sphere.localPosition - centerPosition;
                zPlane.transform.localScale = Vector3.zero;
                zPlane.transform.localScale = new Vector3 (currentPosition.x, currentPosition.y, 0.1f);

                zPlane.transform.localPosition = new Vector3 (currentPosition.x/2, centerPosition.y + currentPosition.y / 2, currentPosition.z);
                zPlane.SetActive (true);
            }
            else zPlane.SetActive (false); 
            phiPlane.SetActive (false); 
            cZPlane.SetActive (false);
            phiLine.SetActive (false);
            Destroy (thetaObj);
        }
        if (coordChoice == 1) {
            if (planeToggle3.isOn) {
                cPhiPlane.transform.localPosition = new Vector3 (0, centerPosition.y, 0);
                cPhiPlane.transform.localScale = new Vector3 (Mathf.Abs (cCoord.r1), Mathf.Abs (currentPosition.y), 0.05f);
                cPhiPlane.transform.localEulerAngles = new Vector3(0, -cCoord.phi, 0);
                
                
                float x = cCoord.r1 * Mathf.Cos (degreeToRadian (cCoord.phi)) + centerPosition.x;
                float y = centerPosition.y + currentPosition.y/2;
                float z = cCoord.r1 * Mathf.Sin (degreeToRadian (cCoord.phi)) + centerPosition.z;

                cPhiPlane.transform.localPosition = new Vector3 (x/2, y, z/2);
                
                cPhiPlane.SetActive (true);
            }
            else cPhiPlane.SetActive (false);
            zPlane.SetActive(false);
            cZPlane.SetActive (false);
        }
        if (coordChoice == 2) {
            if (planeToggle3.isOn) {
                currentPosition = Sphere.localPosition - centerPosition;
                cZPlane.transform.localScale = Vector3.zero;
                cZPlane.transform.localScale = new Vector3 (currentPosition.x, 0.1f, currentPosition.z);

                cZPlane.transform.localPosition = new Vector3 (currentPosition.x/2, centerPosition.y + currentPosition.y, currentPosition.z/2);
                cZPlane.SetActive (true);
            }
            else cZPlane.SetActive (false); 
            phiPlane.SetActive (false); 
            zPlane.SetActive (false);
            Destroy (thetaObj);
            //phiLine.SetActive (false);            
        }
    }

    void spherePlanes (bool isOn) {
        if (isOn) {
            currentPosition = Sphere.localPosition - centerPosition;
            sphericalCompute (currentPosition);
            //Debug.Log ("spherePlanes = " + currentPosition.ToString());
            //Debug.Log ("spherePlanes: r=" + sCoord.r.ToString());
            rPlane.transform.localScale = Vector3.zero;
            rPlane.transform.localPosition = new Vector3 (0, centerPosition.y, 0);
            rPlane.transform.localScale = new Vector3 (sCoord.r*2, sCoord.r*2, sCoord.r*2);

            //Debug.Log (rPlane.transform.localPosition);
            rPlane.SetActive (true);
            xPlane.SetActive (false);
            r1Plane.SetActive (false);

            createArc (50, 0, sCoord.phi, sCoord.r1*0.5f);
            cPhiPlaneLineRenderer.positionCount = arcPoints.Count;
            for (int i =0; i < arcPoints.Count; i++) {
                cPhiPlaneLineRenderer.SetPosition (i, arcPoints[i]);
            }
        }
        else rPlane.SetActive (false);
    }

    void cylinderPlanes (bool isOn) {
        if (isOn) {
            currentPosition = Sphere.localPosition - centerPosition;
            //Debug.Log ("cylindrPlanes currentPos= " + currentPosition.ToString());
            cylindericalCompute (currentPosition);
            //Debug.Log ("cylinderPlanes: r1=" + sCoord.r1.ToString());
            r1Plane.transform.localScale = Vector3.zero;
            r1Plane.transform.localPosition = new Vector3 (0, centerPosition.y + currentPosition.y/2, 0);
            r1Plane.transform.localScale = new Vector3 (cCoord.r1*2, currentPosition.y/2, cCoord.r1*2);
            r1Plane.SetActive (true);
            xPlane.SetActive (false);
            rPlane.SetActive (false);

            createArc (50, 0, cCoord.phi, cCoord.r1*0.5f);
            cPhiPlaneLineRenderer.positionCount = arcPoints.Count;
            for (int i =0; i < arcPoints.Count; i++) {
                cPhiPlaneLineRenderer.SetPosition (i, arcPoints[i]);
            }
            cPhiPlane.SetActive (true);

            cPhiPlane.transform.localPosition = new Vector3 (0, centerPosition.y, 0);
            cPhiPlane.transform.localScale = new Vector3 (Mathf.Abs (cCoord.r1), Mathf.Abs (currentPosition.y), 0.05f);
            cPhiPlane.transform.localEulerAngles = new Vector3(0, -cCoord.phi, 0);
            
            
            float x = cCoord.r1 * Mathf.Cos (degreeToRadian (cCoord.phi)) + centerPosition.x;
            float y = centerPosition.y + currentPosition.y/2;
            float z = cCoord.r1 * Mathf.Sin (degreeToRadian (cCoord.phi)) + centerPosition.z;

            cPhiPlane.transform.localPosition = new Vector3 (x/2, y, z/2);
            
            cPhiPlane.SetActive (true);

        }
        else r1Plane.SetActive (false);
    }

    public void xPlaneToggle () { 
        /*
        Toggle planeToggle = xPlaneToggleGroup.GetFirstActiveToggle();
        if (planeToggle == null) {
            xPlane.SetActive (false);
            rPlane.SetActive (false);
            r1Plane.SetActive (false);
            xToggleVar = false;
            return;
        }
        */
        PlaneFunc1 ();
        //xToggleVar = true;
    }

    public void yPlaneToggle () {
        /*
        Toggle planeToggle = yPlaneToggleGroup.GetFirstActiveToggle();
        if (planeToggle == null) {
            if (coordChoice == 0) yPlane.SetActive (false);
            if (coordChoice == 1) thetaPlane.SetActive (false);
            if (coordChoice == 2) cPhiPlane.SetActive (false);
            return;
        }
        */
        PlaneFunc2 ();
    }
    public void zPlaneToggle () {
        /*
        Toggle planeToggle = zPlaneToggleGroup.GetFirstActiveToggle();
        if (planeToggle == null) {
            if (coordChoice == 0) zPlane.SetActive (false);
            if (coordChoice == 1) phiPlane.SetActive (false);
            if (coordChoice == 2) cZPlane.SetActive (false);
            return;
        }
        */
        PlaneFunc3 ();
    }

    public void xSliderChange (float slider_value) {
        Vector3 pos;
        if (!xSliderActive) return;
        //Debug.Log ("xSliderChange " + slider_value.ToString() + ", CordDirection=" + CordManager.CordDirection.ToString());
        if (float.IsNaN(slider_value)) return;
        if (Mathf.Abs(slider_value) < 0.001) return;
              
        //if (!x_sliderChange) return;
        currentPosition = Sphere.localPosition - center.localPosition;
        sphericalCompute(currentPosition);
        cCoord.r1 = sCoord.r; cCoord.phi = sCoord.phi; cCoord.z = currentPosition.y;

        if (coordChoice == 0) {
            float xTemp = cartesianPos.x + slider_value;
            xText.text = "x=" + xTemp.ToString("0.##");
            currentPosition.x = xTemp;
            SphereText.text = currentPosition.ToString();
        }

        if (coordChoice == 1) {
            float rTemp = sphiricalPos.x + slider_value;
            xText.text = "r=" + rTemp.ToString("0.##");
            
            pos = new Vector3 (rTemp, sCoord.theta, sCoord.phi);
            SphereText.text = pos.ToString();
            float theta = degreeToRadian (sCoord.theta);
            float phi = degreeToRadian (sCoord.phi);
            
            float x = rTemp * Mathf.Sin(theta) * Mathf.Cos(phi);
            float y = rTemp * Mathf.Cos(theta);
            float z = rTemp * Mathf.Sin (theta) * Mathf.Sin (phi);

            currentPosition = new Vector3 (x, y, z);

            //Debug.Log ("xSliderChange: rTemp=" + rTemp.ToString());
            //Debug.Log ("xSliderChange: currentPosition=" + currentPosition.ToString());
            
            //Sphere.localPosition = currentPosition + centerPosition;
        }
        if (coordChoice == 2) {
            //getSphiricalCoord (currentPosition);
            float r1Temp = cylindricalPos.x + slider_value;
            xText.text = "ρ=" + r1Temp.ToString("0.##");
            
            pos = new Vector3 (r1Temp, radianToDegree (cCoord.phi), cCoord.z);
            SphereText.text = pos.ToString();

            float phi = degreeToRadian (cCoord.phi);
            
            float x = r1Temp * Mathf.Cos(phi);
            float y = cCoord.z;
            float z = r1Temp * Mathf.Sin (phi);

            currentPosition = new Vector3 (x, y, z);
            //Sphere.localPosition = currentPosition + centerPosition;
        }
        Sphere.localPosition = currentPosition + centerPosition;
        sphericalCompute(currentPosition);
        cCoord.r1 = sCoord.r1; cCoord.phi = sCoord.phi; cCoord.z = currentPosition.y;
        //Debug.Log("currentPosition = " + currentPosition.ToString());
        //Debug.Log ("r=" + sCoord.r.ToString());

        //CartUnitVectorDirection();
        //SpherUnitVectorDirection(sCoord);
        //CylinderUnitVectorDirection(cCoord);

        PlaneFunc1 ();
        PlaneFunc2 ();
        PlaneFunc3 ();
        
    }

    public void ySliderChange (float slider_value) {
        Vector3 pos;
        if (!ySliderActive) return;
        //Debug.Log ("ySliderChange " + slider_value.ToString() + ", CordDirection=" + CordDirection.ToString());
        if (float.IsNaN(slider_value)) return;
        if (Mathf.Abs(slider_value) < 0.001) return;
        
        currentPosition = Sphere.localPosition - centerPosition;
        sphericalCompute (currentPosition);
        cCoord.r1 = sCoord.r1; cCoord.phi = sCoord.phi; cCoord.z = currentPosition.y;
        
        if (CordManager.CordDirection == 0) {
            float zTemp = cartesianPos.z + slider_value;
            yText.text = "y=" + zTemp.ToString("0.##");
            currentPosition.z = zTemp;
            SphereText.text = currentPosition.ToString();
            //Sphere.localPosition = currentPosition + centerPosition;
        }
        if (CordManager.CordDirection == 1) {
            float t = sphiricalPos.y + slider_value; // t in degree
            float phi = degreeToRadian (sCoord.phi); // phi in radian
            yText.text = "θ=" + t.ToString("0.##");
            pos = new Vector3 (sCoord.r, t, phi);
            SphereText.text = pos.ToString();

            float x = sCoord.r * Mathf.Sin(degreeToRadian(t)) * Mathf.Cos(phi);
            float y = sCoord.r * Mathf.Cos(degreeToRadian(t));
            float z = sCoord.r * Mathf.Sin (degreeToRadian(t)) * Mathf.Sin (phi);

            currentPosition = new Vector3 (x, y, z);
            //txt4.text = (currentPosition + centerPosition).ToString();
            //Sphere.localPosition = currentPosition + centerPosition;
        }
        if (CordManager.CordDirection == 2) {
            //getSphiricalCoord (currentPosition); // phi in degree
            float phi = cylindricalPos.y + slider_value; // phi in degree 
            yText.text = "Φ=" + phi.ToString("0.##");
            
            pos = new Vector3 (cCoord.r1, phi, cCoord.z);
            SphereText.text = pos.ToString();

            float x = cCoord.r1 * Mathf.Cos(degreeToRadian(phi)); // phi in degree
            float y = cCoord.z;
            float z = cCoord.r1 * Mathf.Sin (degreeToRadian(phi)); // phi in degree
            currentPosition = new Vector3 (x, y, z);
            //Debug.Log ("ySliderChange: cCoord.r1="+ cCoord.r1.ToString());
            //Debug.Log ("ySliderChange: currentPosition="+ currentPosition.ToString());
            //txt4.text = (currentPosition + centerPosition).ToString();
            //Sphere.localPosition = currentPosition + centerPosition;
        }
        Sphere.localPosition = currentPosition + centerPosition;
        sphericalCompute(currentPosition);
        cCoord.r1 = sCoord.r1; cCoord.phi = sCoord.phi; cCoord.z = currentPosition.y;

        //CartUnitVectorDirection();
        //SpherUnitVectorDirection(sCoord);
        //CylinderUnitVectorDirection(cCoord);

        PlaneFunc1();
        PlaneFunc2();
        PlaneFunc3();
    }

    public void zSliderChange (float slider_value) {
        Vector3 pos;
        if (!zSliderActive) return;
        //Debug.Log ("zSliderChange " + slider_value.ToString() + ", CordDirecton=" + CordDirection.ToString());
        if (float.IsNaN(slider_value)) return;
        if (Mathf.Abs(slider_value) < 0.001) return;
        
        currentPosition = Sphere.localPosition - centerPosition;
        sphericalCompute (currentPosition);
        cCoord.r1 = sCoord.r1; cCoord.phi = sCoord.phi; cCoord.z = currentPosition.y;

        if (CordManager.CordDirection == 0) {
            float yTemp = cartesianPos.y + slider_value;
            zText.text = "z=" + yTemp.ToString("0.##");
            currentPosition.y = yTemp;
            SphereText.text = currentPosition.ToString();
            //Sphere.localPosition = currentPosition + centerPosition;
        }
        if (CordManager.CordDirection == 1) {
            float h = sphiricalPos.z + slider_value; // h is in degree
            float theta = degreeToRadian (sCoord.theta); // theta in radian, sCoord.theta in degree
            zText.text = "Φ=" + h.ToString("0.##");
            
            pos = new Vector3 (sCoord.r, sCoord.theta, h);
            
            SphereText.text = pos.ToString();
            
            float x = sCoord.r * Mathf.Sin (theta) * Mathf.Cos(degreeToRadian(h));
            float y = sCoord.r * Mathf.Cos (theta);
            float z = sCoord.r * Mathf.Sin (theta) * Mathf.Sin (degreeToRadian(h));

            currentPosition = new Vector3 (x, y, z);
            //Sphere.localPosition = currentPosition + centerPosition;
        }
        if (CordManager.CordDirection == 2) {
            float zTemp = cartesianPos.y + slider_value;
            //float phi = degreeToRadian (cCoord.phi); // phi in radian but cCoord.phi in degree
            zText.text = "z=" + zTemp.ToString("0.##");

            currentPosition.y = zTemp;
            pos = new Vector3 (cCoord.r1, cCoord.phi, zTemp);
            SphereText.text = pos.ToString();

            //Sphere.localPosition = currentPosition + centerPosition;
        }
        Sphere.localPosition = currentPosition + centerPosition;
        sphericalCompute(currentPosition);
        cCoord.r1 = sCoord.r1; cCoord.phi = sCoord.phi; cCoord.z = currentPosition.y;

        //CartUnitVectorDirection();
        //SpherUnitVectorDirection(sCoord);
        //CylinderUnitVectorDirection(cCoord);
        PlaneFunc1();
        PlaneFunc2();
        PlaneFunc3();
    }
    
}

