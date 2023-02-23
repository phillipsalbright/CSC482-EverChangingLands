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
    [SerializeField] private TMP_Text _woodText;
    [SerializeField] private TMP_Text _foodText;

    [SerializeField] private Button nextTurnButton;

    [SerializeField] private GameObject previewElement;

    [SerializeField] private RectTransform _cursorParent;

    [SerializeField] private Image _normalCursorImage;

    [SerializeField] private Image _hoverCursorImage;

    private Vector2 midScreen;

    private PlayerInput _controls;

    private Vector2 _cursorPosition;

    private Vector2 _rightStick;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        GameManager g = GameManager.Instance;
        _turnText.text = "Turn " + g.GetTurnNum();
        _woodText.text = "Wood: " + g.WoodRemaining;
        _foodText.text = "Food: " + g.FoodRemaining;
        nextTurnButton.onClick.AddListener(g.AdvanceTurn);
        midScreen = new Vector2(Screen.width, Screen.height);
        _cursorPosition = midScreen;
    }

    protected void OnEnable()
    {
        GameManager g = GameManager.Instance;
        g.OnFoodChanged += HandleFoodChange;
        g.OnWoodChanged += HandleWoodChange;
        g.OnTurnChanged += HandleTurnChange;
    }

    protected void OnDisable()
    {
        GameManager g = GameManager.Instance;
        if (g == null)
        {
            return;
        }
        g.OnFoodChanged -= HandleFoodChange;
        g.OnWoodChanged -= HandleWoodChange;
        g.OnTurnChanged -= HandleTurnChange;
    }

    private void HandleTurnChange(int newTurn)
    {
        _turnText.text = "Turn " + newTurn;
    }

    private void HandleFoodChange(int oldFoodCount, int newFoodCount)
    {
        _foodText.text = "Wood: " + newFoodCount;
    }

    private void HandleWoodChange(int oldWoodCount, int newWoodCount)
    {

        _woodText.text = "Food: " + newWoodCount;
    }

    // Update is called once per frame
    void Update()
    {
        bool _isGamepad = this.GetComponentInParent<PlayerInput>().currentControlScheme == "Gamepad";
        Debug.Log(this.GetComponentInParent<PlayerInput>().currentControlScheme);
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

}