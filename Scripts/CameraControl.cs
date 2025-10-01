using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public GameObject parentModel;
    private float rotationSpeed = 500.0f;
    private Vector3 mouseWorldPosStart;
    private float zoomScale = 10.0f;
    private float zoomMin = 0.10f;
    private float zoomMax = 100.0f;
    private float maxFieldOfView = 60.0f;
    private float minFieldOfView = 0.0f;
    private float defaultFieldOfView = 60.0f;

    // Start is called before the first frame update
    //void Start(){}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.Mouse2)) 
        {
            camOrbit();
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.F))
        {
            FitToScreen();
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.R))
        {
            ResetCamera();
        }
        if (Input.GetMouseButtonDown(2) && !Input.GetKey(KeyCode.LeftShift)) 
        {
            mouseWorldPosStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButton(2) && !Input.GetKey(KeyCode.LeftShift))
        {
            Pan();
        }
        Zoom(Input.GetAxis("Mouse ScrollWheel"));
    }

    private void camOrbit() 
    {
        if (Input.GetAxis("Mouse Y") != 0 || Input.GetAxis("Mouse X") != 0) 
        {
            float verticalInput = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            float horizontalInput = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.right, -verticalInput);
            transform.Rotate(Vector3.up, -horizontalInput, Space.World);
        }
    }

    private Bounds GetBound(GameObject parentGameObj)  
    {
        Bounds bound = new Bounds(parentGameObj.transform.position, Vector3.zero);
        var rList = parentGameObj.GetComponentsInChildren(typeof(Renderer));
        foreach (Renderer r in rList) 
        {
            bound.Encapsulate(r.bounds);
        }
        return bound;
    }

    private void Pan() 
    {
        if (Input.GetAxis("Mouse Y") != 0 || Input.GetAxis("Mouse X") != 0) 
        {
            Vector3 mouseWorldPosDiff = mouseWorldPosStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);  //GetPerspectivePos();
            transform.position += mouseWorldPosDiff;
        }
    }

    private void Zoom(float zoomDiff) 
    {
        if (zoomDiff != 0) 
        {
            mouseWorldPosStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //mouseWorldPosStart = GetPerspectivePos();
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - zoomDiff * zoomScale, zoomMin, zoomMax);
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView - zoomDiff * zoomScale, minFieldOfView, maxFieldOfView);
            Vector3 mouseWorldPosDiff = mouseWorldPosStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);  //GetPerspectivePos();
            transform.position += mouseWorldPosDiff;
        }

    }

    private void FitToScreen() 
    {
        Camera.main.fieldOfView = defaultFieldOfView;
        Bounds bound = GetBound(parentModel);
        Vector3 boundSize = bound.size;
        float boundDiagonal = Mathf.Sqrt((boundSize.x * boundSize.x) + (boundSize.y * boundSize.y) + (boundSize.z * boundSize.z));
        float camDistanceToBoundCentre = boundDiagonal / 2.0f / (Mathf.Tan(Camera.main.fieldOfView / 2.0f * Mathf.Deg2Rad));
        float camDistanceToBoundWithOffset = camDistanceToBoundCentre + boundDiagonal / 2.0f - (Camera.main.transform.position - transform.position).magnitude;
        transform.position = bound.center + (-transform.forward * camDistanceToBoundWithOffset);
    }

    public Vector3 GetPerspectivePos() 
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(transform.forward, 0.0f);
        float dist;
        plane.Raycast(ray, out dist);
        return ray.GetPoint(dist);
    }

    public void ChangeNavigationView(int option) 
    {
        if (option == 0) 
        {
            CamRotate(15,45,0); // Isometric view
        }
        else if (option == 1)
        {
            CamRotate(-90, 0, 0); // Down view
        }
        else if (option == 2)
        {
            CamRotate(0, 0, 0); // Front view
        }
        else if (option == 3)
        {
            CamRotate(0, -90, 0); // Back view
        }
        else 
        {
            CamRotate(90, 0, 0); // Top view
        }
    }

    public void CamRotate(float xRotation, float yRotation, float zRotation) 
    {
        transform.eulerAngles = new Vector3(xRotation, yRotation, zRotation);
    }

    public void ResetCamera() 
    {
        transform.eulerAngles = new Vector3(15, 45, 0);
        transform.position = new Vector3(0, 0, 0);
    }
}
