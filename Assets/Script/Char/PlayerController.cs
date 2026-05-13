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

    private void Awake()
    {
        defaultInput = new PlayerInput();

        defaultInput.OnFoot.Movement.performed += e => input_Movement = e.ReadValue<Vector2>();
        defaultInput.OnFoot.View.performed += e => input_View = e.ReadValue<Vector2>();


        defaultInput.Enable();


        newCamRotation = camHolder.localRotation.eulerAngles;
        newCharactorRotation = transform.localRotation.eulerAngles;

        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        CalculateView();
        CalculateMovement();
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


        characterController.Move(newMovementSpeed);

    }


}
