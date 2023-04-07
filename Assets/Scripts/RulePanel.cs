using UnityEngine;
using TMPro;

public class RulePanel : MonoBehaviour
{
    [SerializeField] private TMP_Text ruleNumText;
    [SerializeField] private TMP_Text ruleDescText;

    public void SetRuleNumText(int num)
    {
        ruleNumText.text = "Rule: " + num;
    }

    public void SetRuleDescText(string desc)
    {
        ruleDescText.text = desc;
    }
}
