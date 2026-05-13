using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using Unity.Mathematics;
using Unity.VisualScripting;
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
    public Transform feetTransfrom;

    [Header("설정")]
    public PlayerSettingsModel playerSet;

    public float viewClampYmin= -70;
    public float viewClampYmax= 80;

    public LayerMask playerMask;

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
    public CharacterStance PlayerCrouchStance;
    public CharacterStance PlayerProneStance;

    private float stanceCheckForError = 0.05f;

    private float cameraHeight;
    private float cameraHeightVelocity;

    private Vector3 stanceCapsuleCenterVelocity;
    private float stanceCapsuleHeightVelocity;





    private void Awake()
    {
        defaultInput = new PlayerInput();

        defaultInput.OnFoot.Movement.performed += e => input_Movement = e.ReadValue<Vector2>();
        defaultInput.OnFoot.View.performed += e => input_View = e.ReadValue<Vector2>();
        defaultInput.OnFoot.Jump.performed += e => Jump();

        defaultInput.OnFoot.Crouch.performed += e => Crouch();
        defaultInput.OnFoot.Prone.performed += e => Prone();

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
        CalculateStance();


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



    private void CalculateStance()
    {
        var currentStance = PlayerStandStance;

        if (playerStance == PlayerStance.Crouching)
        {
            currentStance = PlayerCrouchStance;
        }
        else if(playerStance == PlayerStance.Prone)
        {
            currentStance = PlayerProneStance;
        }


        cameraHeight = Mathf.SmoothDamp(camHolder.localPosition.y, currentStance.CameraHeight, ref cameraHeightVelocity, playerStanceSmoothing);

        camHolder.localPosition = new Vector3(camHolder.localPosition.x, cameraHeight, camHolder.localPosition.z);


        characterController.height = Mathf.SmoothDamp(characterController.height, currentStance.StanceCollider.height,ref stanceCapsuleHeightVelocity, playerStanceSmoothing);
        characterController.center = Vector3.SmoothDamp(characterController.center, currentStance.StanceCollider.center,ref stanceCapsuleCenterVelocity, playerStanceSmoothing);




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


    private void Crouch()
    {
        if(playerStance ==  PlayerStance.Crouching)
        {
            if (StandCheack())



            playerStance = PlayerStance.Standing;
            return;
        }

        playerStance = PlayerStance.Crouching;
    }


    private void Prone()
    {
        playerStance = PlayerStance.Prone;
    }

    private bool StandCheack(float stanceCheckheight)
    {
        var start = new Vector3(feetTransfrom.position.x,feetTransfrom.position.y + characterController.radius + stanceCheckForError, feetTransfrom.position.z);
        var end = new Vector3(feetTransfrom.position.x, feetTransfrom.position.y - characterController.radius - stanceCheckForError + stanceCheckheight, feetTransfrom.position.z);





        return Physics.CheckCapsule(start,end,characterController.radius, playerMask);
    }

}
