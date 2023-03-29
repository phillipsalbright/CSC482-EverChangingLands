using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleSetManager : Singleton<TileManager>
{
    [SerializeField]
    private RuleSetHolder ruleSets;
    // Start is called before the first frame update
    void Start()
    {
        
        if(ruleSets == null){
            ruleSets = new RuleSetHolder();

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
