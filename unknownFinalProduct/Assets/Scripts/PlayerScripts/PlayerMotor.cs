using UnityEngine;

//Player should always have rigid body and apply physics
[RequireComponent(typeof(Rigidbody))]

public class PlayerMotor : MonoBehaviour
{
    [SerializeField]
    private Camera cam = null;
    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private float cameraRotationX = 0f ;
    private float currentCameraRotationX = 0f ;
    private Vector3 thrusterForce = Vector3.zero;

    [SerializeField]
    private float cameraRotationLimit = 85f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void FixedUpdate()  //Runs every iteration
    {
        PerformMovement();
        PerformRotation();      
    }


    //Recieve Parameters from PlayerController for movement, rotation and cameraRotation

    //Gets movement vector
    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }
    
    //Gets rotational vector
    public void Rotate(Vector3 _rotation)
    {
        rotation = _rotation;
    }
    
    //Gets camera rotational vector
    public void RotateCamera(float _cameraRotationX)
    {
        cameraRotationX = _cameraRotationX;
    }

    public void ApplyThruster(Vector3 _thruster)
    {
        thrusterForce = _thruster;
    }
    //Activities 
    void PerformMovement()
    {
        if(velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }

        if(thrusterForce != Vector3.zero)
        {
            rb.AddForce(thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }

    void PerformRotation()
    {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        if(cam != null)
        {
            //set rotation and clamping to camera rotation around a axis
           currentCameraRotationX -= cameraRotationX;
           currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

            //Rotation Applied to the transform of our camera
           cam.transform.localEulerAngles = new Vector3 (currentCameraRotationX, 0f, 0f);
        }
    }
    
}
