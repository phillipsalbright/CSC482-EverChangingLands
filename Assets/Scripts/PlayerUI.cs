using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerUI : Singleton<PlayerUI>
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

    private PlayerController.mode lastMode;

    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _gameOverMenu;
    [SerializeField] private GameObject _normalTurnHUD;
    [SerializeField] private GameObject _startGameHUD;
    [SerializeField] private GameObject _settlerActionHUD;
    [SerializeField] private GameObject _buildingHUD;
    [SerializeField] private GameObject _tileFlippingHUD;
    [SerializeField] private GameObject _selectedTileToFlipHUD;
    [SerializeField] private GameObject _viewTileInfoHUD;
    [SerializeField] private Button[] _tileFlippingButtons;
    [SerializeField] private TMP_Text setSettlerText;
    [SerializeField] private List<GameObject> huds = new List<GameObject>();

    private Tile _selectedTileToFlip;
    private int _settlersToPlace;
    private Settler _selectedSettler;
    private bool _isGamepad;
    private bool _paused = false;

    [SerializeField] private AudioSource selectSound;
    [SerializeField] private AudioSource infoSound;

    // Start is called before the first frame update
    void Start()
    {
        huds.Add(_pauseMenu);
        huds.Add(_gameOverMenu);
        huds.Add(_normalTurnHUD);
        huds.Add(_startGameHUD);
        huds.Add(_settlerActionHUD);
        huds.Add(_buildingHUD);
        huds.Add(_tileFlippingHUD);
        huds.Add(_selectedTileToFlipHUD);
        huds.Add(_viewTileInfoHUD);
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
        if (SettlerManager.Instance.GetNumberAliveSettlers() <= 0 && _playerController.currentControllerMode != PlayerController.mode.GameStart)
        {
            SetMode(PlayerController.mode.GameOver);
        }
    }

    // Update is called once per frame
    void Update()
    {
        _isGamepad = this.GetComponentInParent<PlayerInput>().currentControlScheme == "Gamepad";
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
        infoSound.Stop();
        infoSound.Play();
    }

    public void NoPredictionView()
    {
        previewElement.SetActive(false);
        nextTurnButton.gameObject.SetActive(true);
        infoSound.Stop();
        infoSound.Play();
    }

    public void MoveCursorGamepad(InputAction.CallbackContext context)
    {
        _rightStick = context.ReadValue<Vector2>();
    }

    public void ControllerSelect(InputAction.CallbackContext context)
    {
        if(!selectSound.isPlaying)
        {
            selectSound.Play();
        }

        if(context.performed)
        {
            Ray ray = Camera.main.ScreenPointToRay(_cursorPosition);
            ray.direction = new Vector3(0, 0, 1);
            int layer_mask = LayerMask.GetMask("Settler");
            int button_mask = LayerMask.GetMask("UI");
            TileManager tm = FindObjectOfType<TileManager>();
            Tile tile = tm.GetTileAtLocation(ray.GetPoint(10f));
            RaycastHit hit;

            var raycastResult = new List<RaycastResult>();
            PointerEventData p = new PointerEventData(EventSystem.current);
            p.position = _cursorPosition;
            EventSystem.current.RaycastAll(p, raycastResult);
            if (raycastResult.Count <= 0)
            {
                //SetMode(PlayerController.mode.BeginTurn);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layer_mask) && (_playerController.currentControllerMode == PlayerController.mode.BeginTurn || _playerController.currentControllerMode == PlayerController.mode.SettlerActions))
                {
                    Settler s = hit.transform.gameObject.GetComponent<Settler>();
                    GameManager.Instance.SelectTile(s.GetCurrentTile(), 3);
                    _selectedSettler = s;
                    SetMode(PlayerController.mode.SettlerActions);
                }
                else
                {
                    if (_playerController.currentControllerMode == PlayerController.mode.GameStart)
                    {
                        SettlerManager sm = FindObjectOfType<SettlerManager>();
                        if (sm.GetCurrentNumberOfSettlers() < sm.GetInitialNumberOfSettlers())
                        {
                            if (sm.AddSettlerAtTile(tm.GetTileAtLocation(ray.GetPoint(10f))))
                            {
                                _settlersToPlace--;
                                setSettlerText.text = "Place Settlers: " + _settlersToPlace;
                                Tile t = tm.GetTileAtLocation(ray.GetPoint(10f));
                                BuildingManager.Instance.PlaceInitialHouse(t.GetTilePos2());
                            }
                        }
                        if (sm.GetCurrentNumberOfSettlers() >= sm.GetInitialNumberOfSettlers())
                        {
                            SetMode(PlayerController.mode.BeginTurn);
                        }

                    }
                    else if (_playerController.currentControllerMode == PlayerController.mode.MovingSettler)
                    {
                        _selectedSettler.MoveSettler(tm.GetTileAtLocation(ray.GetPoint(10f)));
                        SetMode(PlayerController.mode.BeginTurn);
                    }
                    else if (_playerController.currentControllerMode == PlayerController.mode.Flipping)
                    {
                        Tile t = tm.GetTileAtLocation(ray.GetPoint(10f));
                        if (t.GetIsValid())
                        {
                            _selectedTileToFlip = t;
                            GameManager.Instance.SelectTile(_selectedTileToFlip, 4);
                            SetMode(PlayerController.mode.SelectFlipTile);
                        }
                    }
                    else if (_playerController.currentControllerMode == PlayerController.mode.SelectFlipTile)
                    {
                        Tile t = tm.GetTileAtLocation(ray.GetPoint(10f));
                        if (t.GetIsValid())
                        {
                            _selectedTileToFlip = t;
                            GameManager.Instance.DisplayFlipTiles(_selectedSettler.GetCurrentTile());
                            GameManager.Instance.SelectTile(_selectedTileToFlip, 4);
                            SetMode(PlayerController.mode.SelectFlipTile);
                        }
                    } else if (_playerController.currentControllerMode == PlayerController.mode.viewingTileInfo) 
                    {
                        Tile t = tm.GetTileAtLocation(ray.GetPoint(10f));
                        GameManager.Instance.DeleteSelection();
                        GameManager.Instance.SelectTile(t, 4);
                        FindObjectOfType<InformationHUD>().SetInformation(t.GetCurrentTileType(), WeatherManager.Instance.GetCurrentWeather());
                    } else if (_playerController.currentControllerMode != PlayerController.mode.Paused && _playerController.currentControllerMode != PlayerController.mode.GameOver)
                    {
                        SetMode(PlayerController.mode.BeginTurn);
                    }
                }
            }
            else if (_isGamepad)
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, button_mask))
                {
                    hit.collider.gameObject.GetComponent<Button>().onClick.Invoke();

                }
                Debug.Log(raycastResult[0]);
                raycastResult[0].gameObject.GetComponentInParent<Button>().onClick.Invoke();
            }

           
        }
    }

    public void SetMode(PlayerController.mode newMode)
    {
        switch (newMode)
        {
            case PlayerController.mode.BeginTurn:
                SwapHUD(2);
                GameManager.Instance.DeleteSelection();
                _playerController.currentControllerMode = PlayerController.mode.BeginTurn;
                break;
            case PlayerController.mode.GameStart:
                SwapHUD(3);
                _playerController.currentControllerMode = PlayerController.mode.GameStart;
                break;
            case PlayerController.mode.SettlerActions:
                SwapHUD(4);
                _settlerActionHUD.transform.Find("MoveSettlerButton").gameObject.GetComponent<Button>().interactable = _selectedSettler.GetCanMove();
                _settlerActionHUD.transform.Find("CollectResourceButton").gameObject.GetComponent<Button>().interactable = _selectedSettler.GetCanCollect();
                _settlerActionHUD.transform.Find("BuildStructureButton").gameObject.GetComponent<Button>().interactable = BuildingManager.Instance.hasBuilding(_selectedSettler.GetCurrentTile());
                _settlerActionHUD.transform.Find("FlipTileButton").gameObject.GetComponent<Button>().interactable = _selectedSettler.GetCanFlip();
                _settlerActionHUD.transform.Find("FlipTileButton").gameObject.GetComponent<Button>().interactable = BuildingManager.Instance.hasBuilding(_selectedSettler.GetCurrentTile());
                _playerController.currentControllerMode = PlayerController.mode.SettlerActions;
                GameManager.Instance.DeleteSelection();
                GameManager.Instance.SelectTile(_selectedSettler.GetCurrentTile(), 3);
                break;
            case PlayerController.mode.MovingSettler:
                SwapHUD(-1);
                GameObject.FindObjectOfType<GameManager>().DisplayMoveTiles(_selectedSettler.GetCurrentTile());

                _playerController.currentControllerMode = PlayerController.mode.MovingSettler;
                break;
            case PlayerController.mode.Building:
                SwapHUD(5);
                BuildingListManager.Instance.OpenBuildingList(_selectedSettler.GetCurrentTile().GetCurrentTileType(), _selectedSettler.GetCurrentTile().GetTilePos2());
                //_buildingHUD.transform.Find("BuildLumberButton").gameObject.GetComponent<Button>().interactable = BuildingManager.Instance.canAfford(BuildingManager.BuildingName.Lumber) && BuildingManager.Instance.canBuild(BuildingManager.BuildingName.Lumber, _selectedSettler.GetCurrentTile().GetCurrentTileType());
                //_buildingHUD.transform.Find("BuildFarmButton").gameObject.GetComponent<Button>().interactable = BuildingManager.Instance.canAfford(BuildingManager.BuildingName.Farm) && BuildingManager.Instance.canBuild(BuildingManager.BuildingName.Farm, _selectedSettler.GetCurrentTile().GetCurrentTileType());
                //_buildingHUD.transform.Find("BuildWellButton").gameObject.GetComponent<Button>().interactable = BuildingManager.Instance.canAfford(BuildingManager.BuildingName.WaterWell) && BuildingManager.Instance.canBuild(BuildingManager.BuildingName.WaterWell, _selectedSettler.GetCurrentTile().GetCurrentTileType());
                //_buildingHUD.transform.Find("BuildHouseButton").gameObject.GetComponent<Button>().interactable = BuildingManager.Instance.canAfford(BuildingManager.BuildingName.House) && BuildingManager.Instance.canBuild(BuildingManager.BuildingName.House, _selectedSettler.GetCurrentTile().GetCurrentTileType());
                _playerController.currentControllerMode = PlayerController.mode.Building;
                break;
            case PlayerController.mode.Flipping:
                SwapHUD(6);
                GameManager.Instance.DisplayFlipTiles(_selectedSettler.GetCurrentTile());
                _playerController.currentControllerMode = PlayerController.mode.Flipping;
                break;
            case PlayerController.mode.SelectFlipTile:
                SwapHUD(7);
                List<TileInfo.TileSwitch> switches = TileInfo.Instance.GetTileSwitches(_selectedTileToFlip.GetCurrentTileType());
                for (int i = 0; i < _tileFlippingButtons.Length; i++)
                {
                    if (i < switches.Count)
                    {
                        _tileFlippingButtons[i].gameObject.SetActive(true);
                        _tileFlippingButtons[i].gameObject.GetComponentInChildren<TMP_Text>().text = Enum.GetName(typeof(Tile.TileTypes), switches[i].switchTile);
                        _tileFlippingButtons[i].interactable = true;
                        for (int j = 0; j < switches[i].requiredResources.Count; j++)
                        {
                            if (switches[i].requiredResourcesCount[j] > ResourceManager.Instance.getResourceCount(switches[i].requiredResources[j]))
                            {
                                _tileFlippingButtons[i].interactable = false;
                            }
                        }
                    }
                    else
                    {
                        _tileFlippingButtons[i].gameObject.SetActive(false);
                    }
                }
                _playerController.currentControllerMode = PlayerController.mode.SelectFlipTile;
                break;
            case PlayerController.mode.GameOver:
                SwapHUD(1);
                _playerController.currentControllerMode = PlayerController.mode.GameOver;
                break;
            case PlayerController.mode.Paused:
                lastMode = _playerController.currentControllerMode;
                _playerController.currentControllerMode = PlayerController.mode.Paused;
                SwapHUD(0);
                break;
            case PlayerController.mode.viewingTileInfo:
                _playerController.currentControllerMode = PlayerController.mode.viewingTileInfo;
                SwapHUD(8);
                break;
        }
    }

    public void SwapHUD(int index)
    {
        for (int i = 0; i < huds.Count; i++)
        {
            huds[i].SetActive(false);
        }

        if (index >= 0 && index < huds.Count)
        {
            huds[index].SetActive(true);
        }
    }

    public void SetMode(int newMode)
    {
        SetMode((PlayerController.mode) newMode);
    }


    public void SelectBuilding(int building)
    {
        BuildingManager.Instance.buildBuilding((BuildingManager.BuildingName) building, _selectedSettler.GetCurrentTile().GetTilePos2());
        SetMode(PlayerController.mode.SettlerActions);
    }

    public void CollectResource()
    {
        _selectedSettler.CollectResource();
        _settlerActionHUD.transform.Find("CollectResourceButton").gameObject.GetComponent<Button>().interactable = _selectedSettler.GetCanCollect();
    }

    public void AllSettlersCollectResources()
    {
        for (int i = 0; i < SettlerManager.Instance.GetCurrentNumberOfSettlers(); i++)
        {
            SettlerManager.Instance.GetSettlers()[i].GetComponent<Settler>().CollectResource();
        }
    }

    public void SwapTile(int newTile)
    {
        List<TileInfo.TileSwitch> switches = TileInfo.Instance.GetTileSwitches(_selectedTileToFlip.GetCurrentTileType());
        _selectedSettler.FlipTile(_selectedTileToFlip, switches[newTile].switchTile);
        for (int i = 0; i < switches[newTile].requiredResources.Count; i++)
        {
            ResourceManager.Instance.RemoveResource(switches[newTile].requiredResources[i], switches[newTile].requiredResourcesCount[i]);
        }
        SetMode(2);
    }

    public void PauseGame(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!_paused)
            {
                SetMode(PlayerController.mode.Paused);
                _paused = true;
            }
            else
            {
                UnpauseGame();
            }
        }
    }

    public void DestroyBuilding()
    {
        
    }

    public void UnpauseGame()
    {
        SetMode(lastMode);
        _paused = false;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        CustomMap[] customMaps = FindObjectsOfType<CustomMap>();
        foreach(CustomMap c in customMaps)
        {
           Destroy(c.gameObject);
        }
        SceneManager.LoadScene(0);
    }
}