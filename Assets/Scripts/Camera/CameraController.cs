using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public float horizontalSpeed;
    public float verticalSpeed;
    //public float horizontalRotationSpeed;
    //public float verticalRotationSpeed;
    //private Vector3 previousMousePos;
    //private bool cameraMode;
    //public float mouseResetDelay;
    //private float mouseResetTime;
    //private bool mouseReset;
    public Slider cameraXRotation;
    public Slider cameraZoom;

    // Start is called before the first frame update
    void Start()
    {
        //previousMousePos = Input.mousePosition;
        //cameraMode = true;
        //ToggleCamera(cameraMode);
        //mouseResetTime = Time.time + mouseResetDelay;
        //mouseReset = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CameraMovement();

        //reset mouse position
        //if (cameraMode)
        //{
        //stop camera rotation if mouse reseted in the last update
        //if (mouseReset)
        //{
        //    previousMousePos = Input.mousePosition;
        //    mouseReset = false;
        //}
        //else
        //{
        CameraRotation();
        //}

        //ResetMousePosition();
        //}

        //toggle camera look
        //if (Input.GetButtonDown("Fire1"))
        //{
        //    cameraMode = !cameraMode;
        //    ToggleCamera(cameraMode);
        //}

        CameraZoom();
    }

    private void OnValidate()
    {
        horizontalSpeed = Mathf.Clamp(horizontalSpeed, 0, 50);
        verticalSpeed = Mathf.Clamp(verticalSpeed, 0, 50);
        //horizontalRotationSpeed = Mathf.Clamp(horizontalRotationSpeed, 0, 150);
        //verticalRotationSpeed = Mathf.Clamp(verticalRotationSpeed, 0, 150);
    }

    private void ToggleCamera(bool activate)
    {
        if (activate)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void CameraMovement()
    {
        //camera movement
        var directionVector =
            (transform.right.normalized * Input.GetAxis("Horizontal") * horizontalSpeed) +
            (transform.forward.normalized * Input.GetAxis("Vertical") * verticalSpeed);
        directionVector.y = 0;
        transform.position += directionVector * Time.deltaTime / Time.timeScale;
    }

    private void CameraRotation()
    {
        transform.rotation = new Quaternion(Mathf.Clamp(cameraXRotation.value, 0.25f, 0.65f),
                0, 0, transform.rotation.w);
        //camera rotation
        //if (Input.mousePosition != previousMousePos)
        //{
        //    Vector3 rotationVector = (Input.mousePosition - previousMousePos) * Time.deltaTime;
        //    rotationVector.x = -rotationVector.y;
        //    Transform tempTransform = transform;
        //    tempTransform.Rotate(rotationVector);
        //    tempTransform.rotation = new Quaternion(Mathf.Clamp(tempTransform.rotation.x, 0.25f, 0.65f),
        //        0, 0, tempTransform.rotation.w);

        //    transform.rotation = tempTransform.rotation;

        //    previousMousePos = Input.mousePosition;
        //}
    }

    private void CameraZoom()
    {
        Camera.main.orthographicSize = cameraZoom.value;
    }

    //private void ResetMousePosition()
    //{
    //    Cursor.lockState = CursorLockMode.Confined;
    //    if (mouseResetTime <= Time.time)
    //    {
    //        Cursor.lockState = CursorLockMode.Locked;
    //        mouseResetTime = Time.time + mouseResetDelay;
    //        mouseReset = true;
    //    }
    //}
}
