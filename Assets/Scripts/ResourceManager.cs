using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ResourceManager : Singleton<ResourceManager>
{
    public enum ResourceTypes
    {
        Wood,
        Water,
        Food,
    }

    [Serializable]
    public struct ResourceTexts
    {
        public ResourceTypes resourceType;
        public TMP_Text resourceText;
    }
    [SerializeField]
    private List<ResourceTexts> resourceTexts = new List<ResourceTexts>();
    private Dictionary<ResourceTypes, TMP_Text> resourceTextDict = new Dictionary<ResourceTypes, TMP_Text>();
    private Dictionary<ResourceTypes, int> resourceCounts = new Dictionary<ResourceTypes, int>();
    // Start is called before the first frame update
    void Awake()
    {
        foreach (ResourceTexts resourceText in resourceTexts)
        {
            resourceTextDict.Add(resourceText.resourceType, resourceText.resourceText);
            resourceCounts.Add(resourceText.resourceType, 10);
            resourceText.resourceText.text = "10";
        }
    }

    public void AddResource(ResourceTypes resourceType, int amount)
    {
        resourceCounts[resourceType] += amount;
        UpdateUI(resourceType);
    }

    public void RemoveResource(ResourceTypes resourceType, int amount)
    {
        resourceCounts[resourceType] -= amount;
        UpdateUI(resourceType);
    }

    public void UpdateUI(ResourceTypes resourceType)
    {
        resourceTextDict[resourceType].text = resourceCounts[resourceType].ToString();
    }

    public int getResourceCount(ResourceTypes resourceType) {
        return resourceCounts[resourceType];
    }
}
