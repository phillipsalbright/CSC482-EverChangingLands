using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _turnText;
    [SerializeField] private TMP_Text _woodText;
    [SerializeField] private TMP_Text _foodText;

    [SerializeField] private Button nextTurnButton;

    [SerializeField] private GameObject previewElement;
    // Start is called before the first frame update
    void Start()
    {
        GameManager g = GameManager.Instance;
        _turnText.text = "Turn " + g.GetTurnNum();
        _woodText.text = "Wood: " + g.WoodRemaining;
        _foodText.text = "Food: " + g.FoodRemaining;
        nextTurnButton.onClick.AddListener(g.AdvanceTurn);
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
}
