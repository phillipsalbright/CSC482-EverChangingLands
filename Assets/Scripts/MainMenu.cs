    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject customPanel;
    [SerializeField]
    private GameObject startPanel;
    [SerializeField]
    private CustomMap customMap;
    public void StartGame()
    {
        Destroy(customMap.gameObject);
        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void StartGameCustom()
    {
        customMap.LoadCustom();
        SceneManager.LoadScene(1);
    }
}
