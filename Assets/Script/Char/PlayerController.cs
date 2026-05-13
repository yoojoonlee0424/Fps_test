using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static 

public class PlayerController : MonoBehaviour
{
    private PlayerInput defaultInput;

    public Vector2 input_Movement;
    public Vector2 input_View;


    private Vector3 newCamRotation;

    [Header("Ref")]
    public Transform camHolder;

    [Header("설정")]
    public PlayerSettingsModel playerSettings;

    private void Awake()
    {
        defaultInput = new PlayerInput();

        defaultInput.OnFoot.Movement.performed += e => input_Movement = e.ReadValue<Vector2>();
        defaultInput.OnFoot.View.performed += e => input_View = e.ReadValue<Vector2>();


        defaultInput.Enable();


        newCamRotation = camHolder.localRotation.eulerAngles;

    }

    private void Update()
    {
        CalculateView();
        CalculateMovement();
    }


    private void CalculateView()
    {



        camHolder.localRotation = Quaternion.Euler(newCamRotation);
    }


    private void CalculateMovement()
    {

    }


}
