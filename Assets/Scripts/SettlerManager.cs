using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettlerManager : MonoBehaviour
{
    [SerializeField] private List<Settler> settlers;
    [SerializeField] private int numberOfSettlers = 3;

    // Start is called before the first frame update
    void Start()
    {
        settlers = new List<Settler>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
