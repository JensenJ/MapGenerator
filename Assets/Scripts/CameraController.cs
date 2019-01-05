using UnityEngine;

public class CameraController : MonoBehaviour
{
    public LayerMask groundLayer;
    public float raycastDistance = 1000f;

    [System.Serializable]
    public class PositionSettings
    {
        public bool invertPan = true;
        public float panSmooth = 7f;
        public float distanceFromGround = 400;
        public bool allowZoom = true;
        public float zoomSmooth = 5;
        public float zoomStep = 5;
        public float maxZoom = 25;
        public float minZoom = 1000;
        public float minPan = -1800f;
        public float maxPan = 1000;
        public float newDistance = 400;
    }

    [System.Serializable]
    public class OrbitSettings
    {
        public float xRot = 30;
        public float yRot = 45;
        public bool allowYOrbit = true;
        public float yOrbitSmooth = 0.5f;
    }

    [System.Serializable]
    public class InputSettings
    {
        public string PAN = "MousePan";
        public string ORBIT_Y = "MouseTurn";
        public string ZOOM = "Mouse ScrollWheel";
    }

    public PositionSettings position = new PositionSettings();
    public OrbitSettings orbit = new OrbitSettings();
    public InputSettings input = new InputSettings();

    Vector3 destination = Vector3.zero;
    Vector3 camVel = Vector3.zero;
    Vector3 previousMousePos = Vector3.zero;
    Vector3 currentMousePos = Vector3.zero;
    float panInput, orbitInput, zoomInput;
    int panDirection = 0;

    void Start()
    {
        panInput = 0;
        orbitInput = 0;
        zoomInput = 0;
    }

    void GetInput()
    {
        panInput = Input.GetAxis(input.PAN);
        orbitInput = Input.GetAxis(input.ORBIT_Y);
        zoomInput = Input.GetAxis(input.ZOOM);

        previousMousePos = currentMousePos;
        currentMousePos = Input.mousePosition;
    }

    void Update()
    {
        GetInput();
        if (position.allowZoom)
        {
            Zoom();
        }
        if (orbit.allowYOrbit)
        {
            Rotate();
        }
        PanWorld();
    }

    void FixedUpdate()
    {
        HandleCameraDistance();
    }

    void PanWorld()
    {
        Vector3 targetPos = transform.position;
        if (position.invertPan)
        {
            panDirection = -1;
        }
        else
        {
            panDirection = 1;
        }
        if(panInput > 0)
        {
            if(targetPos.x > position.maxPan || targetPos.z > position.maxPan || targetPos.x < position.minPan || targetPos.z < position.minPan)
            {
                
            }
            else
            {
                targetPos += transform.right * (currentMousePos.x - previousMousePos.x) * position.panSmooth * panDirection * Time.deltaTime;
                targetPos += Vector3.Cross(transform.right, Vector3.up) * (currentMousePos.y - previousMousePos.y) * position.panSmooth * panDirection * Time.deltaTime;
            }
        }
        if(targetPos.x > position.maxPan)
        {
            targetPos.x = position.maxPan;
        }else if (targetPos.x < position.minPan)
        {
            targetPos.x = position.minPan;
        }else if(targetPos.z > position.maxPan)
        {
            targetPos.z = position.maxPan;
        }else if(targetPos.z < position.minPan)
        {
            targetPos.z = position.minPan;
        }
        transform.position = targetPos;
    }

    void HandleCameraDistance()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, raycastDistance, groundLayer))
        {
            destination = Vector3.Normalize(transform.position - hit.point) * position.distanceFromGround;
            destination += hit.point;
            transform.position = Vector3.SmoothDamp(transform.position, destination, ref camVel, 0.3f);
        }
    }

    void Zoom()
    {
        position.newDistance += position.zoomStep * -zoomInput;
        position.distanceFromGround = Mathf.Lerp(position.distanceFromGround, position.newDistance, position.zoomSmooth * Time.deltaTime);

        if(position.distanceFromGround < position.maxZoom)
        {
            position.distanceFromGround = position.maxZoom;
            position.newDistance = position.maxZoom;
        }
        if(position.distanceFromGround > position.minZoom)
        {
            position.distanceFromGround = position.minZoom;
            position.newDistance = position.minZoom;
        }
    }
    
    void Rotate()
    {
        if (Input.GetKeyUp(KeyCode.Q))
        {
            orbit.yRot -= 90;
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            orbit.yRot += 90;
        }

        transform.rotation = Quaternion.Euler(orbit.xRot, orbit.yRot, 0);

    }
}