using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public void StartGame()
    {
      SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
      Application.Quit();
    }

    public void OpenWindow(GameObject obj)
    {
      obj.SetActive(true);
    }

    public void CloseWindow(GameObject obj)
    {
      obj.SetActive(false);
    }
}
