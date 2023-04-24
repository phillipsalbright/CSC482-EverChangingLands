using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHUD : MonoBehaviour
{
    [SerializeField] private GameObject[] pages;
    private int currentPage = 0;


    public void MoveForward()
    {
        currentPage = (currentPage + 1) % pages.Length;
        OpenPage(currentPage);
    }

    public void MoveBack()
    {
        currentPage = (currentPage - 1) % pages.Length;
        OpenPage(currentPage);
    }

    private void OpenPage(int page)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            if (i == page)
            {
                pages[i].SetActive(true);
            }
            else
            {
                pages[i].SetActive(false);
            }
        }
    }

    public void OpenMenu()
    {
        currentPage = 0;
        OpenPage(currentPage);
    }

    public void CloseMenu()
    {
        this.gameObject.SetActive(false);
    }
}
