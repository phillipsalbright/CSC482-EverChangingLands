using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    private TileManager tileManager;
    [SerializeField] private int _startingFoodCount;
    private int _woodRemaining;
    [SerializeField] private int _startingWoodCount;
    private UnityEvent<int, int> _onFoodChanged = new();
    private UnityEvent<int, int> _onWoodChanged = new();
    private UnityEvent<int> _onTurnChanged = new();
    private int _foodRemaining;
    private int _turn;

    public int FoodRemaining
    {
        get => _foodRemaining;
        set
        {
            int temp = _foodRemaining;
            _foodRemaining = value;
            _onFoodChanged.Invoke(temp, _foodRemaining);
        }
    }

    public int WoodRemaining
    {
        get => _woodRemaining;
        set
        {
            int temp = _woodRemaining;
            _woodRemaining = value;
            _onFoodChanged.Invoke(temp, _woodRemaining);
        }
    }

    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
        tileManager = FindObjectOfType<TileManager>();
        _turn = 1;
        FoodRemaining = _startingFoodCount;
        WoodRemaining = _startingWoodCount;

    }

    public event UnityAction<int, int> OnFoodChanged
    {
        add => _onFoodChanged.AddListener(value);
        remove => _onFoodChanged.RemoveListener(value);
    }

    public event UnityAction<int, int> OnWoodChanged
    {
        add => _onWoodChanged.AddListener(value);
        remove => _onWoodChanged.RemoveListener(value);
    }

    public event UnityAction<int> OnTurnChanged
    {
        add => _onTurnChanged.AddListener(value);
        remove => _onTurnChanged.RemoveListener(value);
    }

    public int GetTurnNum()
    {
        return _turn;
    }

    public void AdvanceTurn()
    {
        _turn++;
        _onTurnChanged.Invoke(_turn);
        tileManager.AdvanceTurn();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
