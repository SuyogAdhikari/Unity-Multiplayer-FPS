using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(ConfigurableJoint))]

public class PlayerController : MonoBehaviour
{
    //even if the data is private ...  serializefield make it available in the inspector menu
    [SerializeField]
    private float speed = 5f;
    
    [SerializeField]
    private float lookSensitivity = 3f;

    [SerializeField]
    private float thrusterForce = 1000f;

    
    


    [Header("Spring Settings :")]
    [SerializeField]
    private JointProjectionMode jointMode;

    [SerializeField]
    private float jointSpring = 20f;
    [SerializeField]
    private float jointMaxForce = 40f;


    //Component caching 
    private PlayerMotor motor;
    private ConfigurableJoint joint;
    private Animator animator;


    void Start()
    {
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();
        animator = GetComponent<Animator>();
        SetJointSettings(jointSpring);
    }

    void Update()
    {
        //-----------MOVEMENT SECTION----------------
        
        // horizontal ranges from (-1, 0, 0) to (1, 0, 0) 
        // vertical ranges from (0, 0, -1) to (0, 0, 1) 
        float _xMov = Input.GetAxis("Horizontal");
        float _zMov = Input.GetAxis("Vertical");         
        //the 'horizontal' and 'vertical' axes are enlisted under edit -> project settings -> input


        Vector3 _movHorizontal = transform.right * _xMov;   // on standing still default vectors (0, 0, 0) ; while moving right vectors (1, 0, 0) and (-1, 0, 0) while on left
        Vector3 _movVertical = transform.forward * _zMov;   // on standing still default vectors (0, 0, 0) ; while moving forward vectors (0, 0, 1) and (0, 0, -1) while on backwards

        //Final Movement Vector combining x and z movement
        Vector3 _velocity = (_movHorizontal + _movVertical) * speed;

        //Animate Movement
        animator.SetFloat("ForwardVelocity", _zMov); 

        //Apply Movement
        motor.Move(_velocity);

        //-----------MOVEMENT SECTION END----------------



        //-----------ROTATION SECTION----------------

        //Player ROTATION SECTION X (changing the Y axis for X rotation)
        float _yRot = Input.GetAxisRaw("Mouse X");  //Gets the X axis of mouse movement

        Vector3 _rotation = new Vector3 (0f, _yRot, 0f) * lookSensitivity; //this is for rotation of player left and right... around Y axis

        //Apply Rotation
        motor.Rotate(_rotation);


        //Camera ROTATION SECTION Y (changing the X axis for Y rotation)

        float _xRot = Input.GetAxisRaw("Mouse Y");

        float _cameraRotationX = _xRot * lookSensitivity; //this is for rotation of player left and right... around Y axis

        //Apply Rotation
        motor.RotateCamera(_cameraRotationX); 
        
        //-----------ROTATION SECTION END----------------


        //--------------- Thruster Force Start

        //Calculation of thruster force based on player input
        Vector3 _thrusterForce = Vector3.zero;
        if(Input.GetButton ("Jump"))
        {
            _thrusterForce = Vector3.up * thrusterForce;
            SetJointSettings(0f);
        }else{
            SetJointSettings(jointSpring);
        }

        //Apply thruster force
        motor.ApplyThruster(_thrusterForce);
    }

    private void SetJointSettings(float _jointSpring)
    {
        joint.yDrive = new JointDrive {
            positionSpring = _jointSpring,
            maximumForce = jointMaxForce 
        };
    }
}
