using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using static PlayerModel;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;

    private PlayerInput defaultInput;

    public Vector2 input_Movement;
    public Vector2 input_View;


    private Vector3 newCamRotation;
    private Vector3 newCharactorRotation;

    [Header("Ref")]
    public Transform camHolder;

    [Header("설정")]
    public PlayerSettingsModel playerSet;

    public float viewClampYmin= -70;
    public float viewClampYmax= 80;

    [Header("중력")]
    public float gravityAmount;
    public float gravityMin;
    private float playerGravity;

    public Vector3 jumpingForce;
    private Vector3 jumpingForceVelocity;

    [Header("자세")]
    public PlayerStance playerStance;

    public float playerStanceSmoothing;

    public CharacterStance PlayerStandStance;
    public CharacterStance PlayerCroucStance;
    public CharacterStance PlayerProneStance;

    private float cameraHeight;
    private float cameraHeightVelocity;

    private Vector3 stanceCapsuleCenter;

    private void Awake()
    {
        defaultInput = new PlayerInput();

        defaultInput.OnFoot.Movement.performed += e => input_Movement = e.ReadValue<Vector2>();
        defaultInput.OnFoot.View.performed += e => input_View = e.ReadValue<Vector2>();
        defaultInput.OnFoot.Jump.performed += e => Jump();

        defaultInput.Enable();


        newCamRotation = camHolder.localRotation.eulerAngles;
        newCharactorRotation = transform.localRotation.eulerAngles;

        characterController = GetComponent<CharacterController>();

        cameraHeight = camHolder.localPosition.y;
    }

    private void Update()
    {
        CalculateView();
        CalculateMovement();
        CalculateJump();
        CalculateCameraHeight();
    }


    private void CalculateView()
    {

        newCharactorRotation.y += playerSet.ViewXSensitivity * input_View.x * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(newCharactorRotation);


        newCamRotation.x += playerSet.ViewYSensitivity * input_View.y * Time.deltaTime;

        newCamRotation.x = Mathf.Clamp(newCamRotation.x, viewClampYmin, viewClampYmax);

        camHolder.localRotation = Quaternion.Euler(newCamRotation);
    }


    private void CalculateMovement()
    {
        var verticalSpeed = playerSet.WalkingFowardSpeed * input_Movement.y * Time.deltaTime;
        var horizontalSpeed = playerSet.WalkingStrafeSpeed * input_Movement.x * Time.deltaTime;


        var newMovementSpeed = new Vector3(horizontalSpeed, 0,verticalSpeed);

        newMovementSpeed = transform.TransformDirection(newMovementSpeed);

        
        
        if(playerGravity > gravityMin)
        {
            playerGravity -= gravityAmount * Time.deltaTime;
        }

        

        if(playerGravity < -0.1f && characterController.isGrounded)
        {
            playerGravity = -0.1f;
        }
     

        newMovementSpeed.y += playerGravity;

        newMovementSpeed += jumpingForce * Time.deltaTime;

        characterController.Move(newMovementSpeed);

    }

    private void CalculateJump()
    {
        jumpingForce = Vector3.SmoothDamp(jumpingForce,Vector3.zero, ref jumpingForceVelocity, playerSet.JumpingFalloff);
    }



    private void CalculateCameraHeight()
    {
        var stanceHeight = PlayerStandStance.CameraHeight;

        if (playerStance == PlayerStance.Crouching)
        {
            stanceHeight = PlayerCroucStance.CameraHeight;
        }
        else if(playerStance == PlayerStance.Prone)
        {
            stanceHeight = PlayerProneStance.CameraHeight;
        }


        cameraHeight = Mathf.SmoothDamp(camHolder.localPosition.y, stanceHeight, ref cameraHeightVelocity, playerStanceSmoothing);

        camHolder.localPosition = new Vector3(camHolder.localPosition.x, cameraHeight, camHolder.localPosition.z);

    }




    private void Jump()
    {
        if(!characterController.isGrounded)
        {
            return;
        }

        jumpingForce = Vector3.up * playerSet.JumpingHeight;
        playerGravity = 0;

    }







}
