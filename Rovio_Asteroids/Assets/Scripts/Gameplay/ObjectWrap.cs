using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//handle object wrap
public class ObjectWrap : MonoBehaviour
{
  //screen vars
  private bool isWrappingX = false;
  private bool isWrappingY = false;

  //object renderer
  private Renderer renderer;

  //screen values to use  - maybe store values somewhere else instead of loading each time on every object that needs to wrap?
  private float screenWidth;
  private float screenHeight;

  void Start()
  {
    renderer = GetComponentInChildren<Renderer>();
    var screenBotLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, transform.position.z));
    var screenTopRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, transform.position.z));

    screenWidth =  screenTopRight.x - screenBotLeft.x;
    screenHeight = screenTopRight.y - screenTopRight.y;
  }

  void Update()
  {
    ScreenWrap();
  }

  public void ScreenWrap()
  {
    //note that this works different in editor since isVisible checks ALL cameras including the editor scene camera
    if (renderer.isVisible)
    {
      isWrappingX = false;
      isWrappingY = false;
      return;
    }

    if (isWrappingX && isWrappingY)
    {
      return;
    }

    //if get to here, start object wrap

    //get the converted position of the object
    var cam = Camera.main;
    Vector2 newPos = transform.position;
    Vector3 convertedPos = cam.WorldToViewportPoint(transform.position);

    //determine if the current position is off the screen now
    if (!isWrappingX && (convertedPos.x > 1 || convertedPos.x < 0))
    {
      newPos.x = -newPos.x;
      isWrappingX = true;
    }

    if (!isWrappingY && (convertedPos.y > 1 || convertedPos.y < 0))
    {
      newPos.y = -newPos.y;
      isWrappingY = true;
    }
    transform.position = newPos;
  }
}
