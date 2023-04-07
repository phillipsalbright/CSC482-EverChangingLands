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
        Stone,
        Person,
        None,
    }

    [Serializable]
    public struct ResourceTexts
    {
        public ResourceTypes resourceType;
        public TMP_Text resourceText;
        public Sprite resourceSprite;
    }
    [SerializeField]
    private List<ResourceTexts> resourceTexts = new List<ResourceTexts>();
    private Dictionary<ResourceTypes, Sprite> resourceSprites = new Dictionary<ResourceTypes, Sprite>();
    private Dictionary<ResourceTypes, TMP_Text> resourceTextDict = new Dictionary<ResourceTypes, TMP_Text>();
    private Dictionary<ResourceTypes, int> resourceCounts = new Dictionary<ResourceTypes, int>();

    [SerializeField] private AudioSource woodCollectionSound;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        foreach (ResourceTexts resourceText in resourceTexts)
        {
            if (resourceText.resourceType != ResourceTypes.Person)
            {
                resourceTextDict.Add(resourceText.resourceType, resourceText.resourceText);
                resourceCounts.Add(resourceText.resourceType, 10);
                resourceText.resourceText.text = "10";
            }
            else
            {
                Debug.Log("Person");
            }
            resourceSprites.Add(resourceText.resourceType, resourceText.resourceSprite);
        }
        Debug.Log(resourceSprites[ResourceTypes.Person].name);
    }

    public bool AddResource(ResourceTypes resourceType, int amount)
    {
        if (resourceType == ResourceTypes.None)
        {
            return false;
        }
        else if (resourceType == ResourceTypes.Wood)
        {
            if(!woodCollectionSound.isPlaying)
            {
                woodCollectionSound.Play();
            }
        }
        resourceCounts[resourceType] += amount;
        UpdateUI(resourceType);
        return true;
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

    public Sprite GetResourceSprite(ResourceTypes resourceType)
    {
        return resourceSprites[resourceType];
    }
}
