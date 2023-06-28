using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewPlayerMove : MonoBehaviour
{
    //public InputAction playerControls;
    public float moveSpeed = 5f;
    public float fastMoveSpeed = 10f;
    public float rotateSpeed = 20f;
    public float sideSpeed = 20f;
    public InputActionAsset inputActions;

    private CharacterController myController;
    InputActionMap keyActionMap;
    //InputAction action;
    float inputX, inputZ;
    Vector3 moveDirection = Vector2.zero;
    Vector2 movement = Vector3.zero;
    //public Key keyboardKeyToCheck = Key.Space;
    bool altKeyPressed = false, shiftKeyPressed = false;

    private void Start()
    {
        myController = GetComponent<CharacterController>();
        keyActionMap = inputActions.FindActionMap ("Player");
        keyActionMap.Enable();
        //action = keyActionMap["altKey"];
        keyActionMap["altKey"].performed += OnAltPerformed;
        keyActionMap["altKey"].canceled += OnAltCanceled;

        keyActionMap["shiftKey"].performed += OnShiftPerformed;
        keyActionMap["shiftKey"].canceled += OnShiftCanceled;
    }

    private void OnDestroy()
    {
        keyActionMap.Disable();
    }

    void OnAltPerformed(InputAction.CallbackContext context) {
        altKeyPressed = true;
    }

    void OnAltCanceled(InputAction.CallbackContext context) {
        altKeyPressed = false;
    }
    void OnShiftPerformed(InputAction.CallbackContext context) {
        shiftKeyPressed = true;
    }

    void OnShiftCanceled(InputAction.CallbackContext context) {
        shiftKeyPressed = false;
    }

    private void Update()
    {
        movement = keyActionMap["Move"].ReadValue<Vector2>();
        inputX = movement.x;
        inputZ = movement.y;
        Vector3 moveDirection;
        if (altKeyPressed) moveDirection = new Vector3(inputX, 0, inputZ);
        else {
            moveDirection = new Vector3(0, 0, inputZ);
            transform.Rotate(0, inputX * rotateSpeed * Time.deltaTime, 0);
        }
        moveDirection = transform.TransformDirection(moveDirection);
        if (shiftKeyPressed) moveDirection *= fastMoveSpeed;
        else moveDirection *= moveSpeed;
        myController.Move(moveDirection * Time.deltaTime);
    }
}
