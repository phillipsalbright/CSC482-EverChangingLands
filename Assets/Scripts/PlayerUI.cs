using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _turnText;

    [SerializeField] private Button nextTurnButton;

    [SerializeField] private GameObject previewElement;

    [SerializeField] private RectTransform _cursorParent;

    [SerializeField] private Image _normalCursorImage;

    [SerializeField] private Image _hoverCursorImage;

    [SerializeField] private PlayerController _playerController;

    private Vector2 midScreen;

    private PlayerInput _controls;

    private Vector2 _cursorPosition;

    private Vector2 _rightStick;

    [SerializeField] private GameObject _normalTurnHUD;
    [SerializeField] private GameObject _startGameHUD;
    [SerializeField] private GameObject _settlerActionHUD;
    [SerializeField] private TMP_Text setSettlerText;
    private int _settlersToPlace;
    private Settler _selectedSettler;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        GameManager g = GameManager.Instance;
        nextTurnButton.onClick.AddListener(g.AdvanceTurn);
        midScreen = new Vector2(Screen.width, Screen.height);
        _playerController = GetComponentInParent<PlayerController>();
        _cursorPosition = midScreen;
        SettlerManager sm = FindObjectOfType<SettlerManager>();
        _settlersToPlace = sm.GetInitialNumberOfSettlers();
        if (_settlersToPlace > 0)
        {
            setSettlerText.text = "Place Settlers: " + _settlersToPlace;
            SetMode(PlayerController.mode.GameStart);
        } else
        {
            SetMode(PlayerController.mode.BeginTurn);
        }

        _turnText.text = "Turn " + 1;
    }

    protected void OnEnable()
    {
        GameManager g = GameManager.Instance;
        g.OnTurnChanged += HandleTurnChange;
    }

    protected void OnDisable()
    {
        GameManager g = GameManager.Instance;
        if (g == null)
        {
            return;
        }
        g.OnTurnChanged -= HandleTurnChange;
    }

    private void HandleTurnChange(int newTurn)
    {
        _turnText.text = "Turn " + newTurn;
    }

    // Update is called once per frame
    void Update()
    {
        bool _isGamepad = this.GetComponentInParent<PlayerInput>().currentControlScheme == "Gamepad";
       // Debug.Log(this.GetComponentInParent<PlayerInput>().currentControlScheme);
        if (!_isGamepad)
        {
            if (Mouse.current != null && !_isGamepad)
            {
                _cursorPosition = Mouse.current.position.ReadValue();
            }
            else
            {
                _cursorPosition = midScreen;
            }

            _cursorParent.position = _cursorPosition;
            return;
        }

        Vector2 delta = _rightStick * Time.deltaTime * 400;
        _cursorPosition += delta;
        _cursorPosition.x = Mathf.Clamp(_cursorPosition.x, 0, Screen.width);
        _cursorPosition.y = Mathf.Clamp(_cursorPosition.y, 0, Screen.height);
        InputState.Change(Mouse.current.position, _cursorPosition);
        _cursorParent.position = _cursorPosition;
    }

    public void PredictionView()
    {
        previewElement.SetActive(true);
        nextTurnButton.gameObject.SetActive(false);
    }

    public void NoPredictionView()
    {
        previewElement.SetActive(false);
        nextTurnButton.gameObject.SetActive(true);
    }

    public void MoveCursorGamepad(InputAction.CallbackContext context)
    {
        _rightStick = context.ReadValue<Vector2>();
    }

    public void ControllerSelect(InputAction.CallbackContext context)
    {
        Debug.Log("Select");
        if(context.performed)
        {
            Ray ray = Camera.main.ScreenPointToRay(_cursorPosition);
            ray.direction = new Vector3(0, 0, 1);
            int layer_mask = LayerMask.GetMask("Settler");
            int button_mask = LayerMask.GetMask("UI");
            TileManager tm = FindObjectOfType<TileManager>();
            Tile tile = tm.GetTileAtLocation(ray.GetPoint(10f));
            //Debug.LogWarning(Physics.Raycast(Camera.main.ScreenPointToRay(_cursorPosition), Mathf.Infinity, button_mask));
             RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity,  layer_mask))
            {
                Settler s = hit.transform.gameObject.GetComponent<Settler>();
                //GameManager.Instance.SelectTile(tm.GetTileAtLocation(ray.GetPoint(10f)));
                if (_playerController.currentControllerMode == PlayerController.mode.BeginTurn)
                {
                    GameManager.Instance.SelectTile(s.GetCurrentTile());
                    _selectedSettler = s;
                    //Debug.Log(tm.GetTileAtLocation(ray.GetPoint(10f)).GetCurrentTileType());
                    //Debug.Log(tile.GetCurrentTileType());
                    SetMode(PlayerController.mode.SettlerActions);
                }
            } else {
                if (_playerController.currentControllerMode == PlayerController.mode.GameStart)
                {
                    SettlerManager sm = FindObjectOfType<SettlerManager>();
                    if (sm.GetCurrentNumberOfSettlers() < sm.GetInitialNumberOfSettlers())
                    {
                        if(sm.AddSettlerAtTile(tm.GetTileAtLocation(ray.GetPoint(10f))))
                        {
                            _settlersToPlace--;
                            setSettlerText.text = "Place Settlers: " + _settlersToPlace;
                        }
                    }
                    if (sm.GetCurrentNumberOfSettlers() >= sm.GetInitialNumberOfSettlers())
                    {
                        SetMode(PlayerController.mode.BeginTurn);
                    }

                }  else if (_playerController.currentControllerMode == PlayerController.mode.MovingSettler)
                {
                    _selectedSettler.MoveSettler(tm.GetTileAtLocation(ray.GetPoint(10f)));
                    SetMode(PlayerController.mode.BeginTurn);
                } else 
                {
                    var raycastResult = new List<RaycastResult>();
                    PointerEventData p = new PointerEventData(EventSystem.current);
                    p.position = _cursorPosition;
                    EventSystem.current.RaycastAll( p, raycastResult);
                    if (raycastResult.Count <= 0)
                    {
                        SetMode(PlayerController.mode.BeginTurn);
                    }
                }
            }
        }
    }

    public void SetMode(PlayerController.mode newMode)
    {
        switch (newMode)
        {
            case PlayerController.mode.BeginTurn:
                _normalTurnHUD.SetActive(true);
                _startGameHUD.SetActive(false);
                _settlerActionHUD.SetActive(false);
                GameManager.Instance.DeleteSelection();
                _playerController.currentControllerMode = PlayerController.mode.BeginTurn;
                break;
            case PlayerController.mode.GameStart:
                _normalTurnHUD.SetActive(false);
                _startGameHUD.SetActive(true);
                _settlerActionHUD.SetActive(false);

                _playerController.currentControllerMode = PlayerController.mode.GameStart;
                break;
            case PlayerController.mode.SettlerActions:
                _normalTurnHUD.SetActive(false);
                _startGameHUD.SetActive(false);
                _settlerActionHUD.SetActive(true);

                _playerController.currentControllerMode = PlayerController.mode.SettlerActions;
                break;
            case PlayerController.mode.MovingSettler:
                _normalTurnHUD.SetActive(false);
                _startGameHUD.SetActive(false);
                _settlerActionHUD.SetActive(false);

                _playerController.currentControllerMode = PlayerController.mode.MovingSettler;
                break;
        }
    }

    public void SetMode(int newMode)
    {
        SetMode((PlayerController.mode) newMode);
    }

}