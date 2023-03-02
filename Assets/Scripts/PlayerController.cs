using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Math = System.Math;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _cameraSpeed = 5;
    [SerializeField] private Vector2 xbounds;
    [SerializeField] private Vector2 ybounds;
    [SerializeField] private Vector2 sizebounds;
    [SerializeField] private PlayerUI ui;
    private Camera cam;
    private Vector2 inputVector = Vector2.zero;
    public enum mode { GameStart, BeginTurn, SettlerActions, MovingSettler };
    public mode currentControllerMode;

    private float zoomInput;
    // Start is called before the first frame update
    void Start()
    {
        cam = this.GetComponent<Camera>();
        ui = GetComponentInChildren<PlayerUI>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(inputVector * _cameraSpeed * Time.deltaTime);
        transform.position = new Vector3(Math.Clamp(transform.position.x, xbounds.x, xbounds.y), Math.Clamp(transform.position.y, ybounds.x, ybounds.y), transform.position.z);
        float zoomScalar = 1;
        if (this.GetComponent<PlayerInput>().currentControlScheme == "Gamepad")
        {
            zoomScalar = 5;
        }
        
        cam.orthographicSize += zoomInput * zoomScalar * Time.deltaTime / -2;
        cam.orthographicSize = Math.Clamp(cam.orthographicSize, sizebounds.x, sizebounds.y);
    }

    public void MovementInputChanged(InputAction.CallbackContext context)
    {
        inputVector = context.action.ReadValue<Vector2>().normalized;
    }

    public void ZoomInputChanged(InputAction.CallbackContext context)
    {
       // Debug.Log(context.action.ReadValue<float>());
        zoomInput = context.action.ReadValue<float>();
    }

    public void SwapView(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

            TileManager tm = FindObjectOfType<TileManager>();
            if (tm != null)
            {
                tm.ViewChangeMap();
                if (tm.IsViewingPrediction())
                {
                    ui.PredictionView();
                }
                else
                {
                    ui.NoPredictionView();
                }
            }
        }
    }
}
