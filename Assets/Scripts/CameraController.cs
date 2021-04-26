using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform cameraTransform;
    public GameObject mainCamera;
    public GameObject birdsEye;

    public float normalSpeed;
    public float fastSpeed;
    public float movementSpeed;
    public float movementTime;
    public float rotationAmount;
    public Vector3 zoomAmount;

    public float minZoom;
    public float maxZoom;

    public Vector3 newPosition;
    public Quaternion newRotation;
    public Vector3 newZoom;

    private bool isMainActive;

    void Start()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
        
        isMainActive = true;
        birdsEye.SetActive(false);
    }

    void Update()
    {
        HandleMovementInput();
    }

    void HandleMovementInput()
    {

        if (Input.GetKeyDown(KeyCode.V))
        {
            if(isMainActive)
            {
                mainCamera.SetActive(false);
                birdsEye.SetActive(true);
                isMainActive = false;
            }
            else
            {
                mainCamera.SetActive(true);
                birdsEye.SetActive(false);
                isMainActive = true;
            }
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            movementSpeed = fastSpeed;
        }
        else
        {
            movementSpeed = normalSpeed;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            newPosition += (transform.forward * movementSpeed);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newPosition += (transform.forward * -movementSpeed);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newPosition += (transform.right * movementSpeed);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            newPosition += (transform.right * -movementSpeed);
        }

        if (Input.GetKey(KeyCode.Z))
        {
            newPosition += (transform.up * movementSpeed);
        }
        if (Input.GetKey(KeyCode.X))
        {
            newPosition += (transform.up * -movementSpeed);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }
        if (Input.GetKey(KeyCode.E))
        {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }

        if (Input.GetKey(KeyCode.R))
        {
            newRotation *= Quaternion.Euler(Vector3.right * -rotationAmount*0.2f);

        }
        if (Input.GetKey(KeyCode.F))
        {
            newRotation *= Quaternion.Euler(Vector3.right * rotationAmount * 0.2f);

        }

        newZoom.y = Mathf.Clamp(newZoom.y, minZoom, maxZoom);
        newZoom.z = Mathf.Clamp(newZoom.z, -maxZoom, -minZoom);

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);


    }
}
