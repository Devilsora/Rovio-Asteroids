using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
  [SerializeField] private AudioMixer mainVolume;
  [SerializeField] private Dropdown resolutionDropdown;
  private List<Resolution> resolutions = new List<Resolution>();
  
  public bool onTitleScreen;
  
  void Start()
  {
    foreach (Resolution r in Screen.resolutions)
    {
      if(r.refreshRate == 60)
        resolutions.Add(r);
    }

    //clear resolution dropdown
    resolutionDropdown.ClearOptions();
    
    //get resolutions to make list from
    List<string> resolutionNames = new List<string>();

    //go through resolutions and remove all the ones that don't have 60 hz refresh rates



    int currResIndex = 0, index = 0;
    foreach (Resolution r in resolutions)
    {
      string newOp = r.width + " x " + r.height; 

      resolutionNames.Add(newOp);

      if (r.height == Screen.currentResolution.height && r.width == Screen.currentResolution.width)
      {
        currResIndex = index;
      }

      index++;
    }
    
    resolutionDropdown.AddOptions(resolutionNames);
    resolutionDropdown.value = currResIndex;
    resolutionDropdown.RefreshShownValue();
    
  }


  //awake and disable functions for use as a pause menu in game vs. an options menu out of game
  public void Awake()
  {
    if (!onTitleScreen)
    {
      Time.timeScale = 0.0f;
    }
  }

  public void OnDisable()
  {
    if (!onTitleScreen)
    {
      Time.timeScale = 1f;
    }
  }

  public void BackToGame()
  {
    gameObject.SetActive(false);
  }

  public void RestartGame()
  {
    SceneManager.LoadScene(1);
  }

  public void BackToStart()
  {
    SceneManager.LoadScene(0);
  }


  public void SetMainVolume(float volume)
  {
    mainVolume.SetFloat("MainVolume", volume);
  }

  public void SetFullscreen(bool fullscreen)
  {
    Screen.fullScreen = fullscreen;
  }

  public void SetQuality(int index)
  {
    QualitySettings.SetQualityLevel(index);
  }

  public void SetResolution(int resIndex)
  {
    Resolution newRes = resolutions[resIndex];
    Screen.SetResolution(newRes.width, newRes.height, Screen.fullScreen);
  }

}
