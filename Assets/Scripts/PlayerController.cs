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

    [SerializeField] private float cursorSpeed = 5f;
    private Vector2 cursorPosition = Vector2.zero;
    private Vector2 cursorInputVector = Vector2.zero;
    private bool useCursor = true;
    public bool isController = false;

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
        cam.orthographicSize += zoomInput * Time.deltaTime / -2;
        cam.orthographicSize = Math.Clamp(cam.orthographicSize, sizebounds.x, sizebounds.y);

        if(useCursor)
        {
            cursorPosition += (cursorInputVector * cursorSpeed * Time.deltaTime);
        }
    }

    public void MovementInputChanged(InputAction.CallbackContext context)
    {
        inputVector = context.action.ReadValue<Vector2>().normalized;
    }

    public void CursorMovementInputChanged(InputAction.CallbackContext context)
    {
        if(isController)
        {
            cursorInputVector = context.action.ReadValue<Vector2>().normalized;
        }
        else
        {
            cursorPosition = context.action.ReadValue<Vector2>();
        }
    }

    public void ZoomInputChanged(InputAction.CallbackContext context)
    {
        Debug.Log(context.action.ReadValue<float>());
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

    public void Select(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            TileManager tm = FindObjectOfType<TileManager>();
            if(tm != null)
            {
                Tile tileAtSelectPos = tm.GetTileAtLocation(cursorPosition);
                Debug.Log(tileAtSelectPos.GetCurrentTileType());
            }
        }
    }
}
