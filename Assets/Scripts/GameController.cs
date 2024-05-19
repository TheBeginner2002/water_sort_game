using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private BottleController firstController;
    [SerializeField] private BottleController secondController;
    [SerializeField] private float timeBottleUp = 5f;

    private void Update()
    {
        InputManager();
    }

    void InputManager()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            if (hit.collider != null)
            {
                if (hit.collider.GetComponent<BottleController>() != null)
                {
                    if (firstController == null)
                    {
                        firstController = hit.collider.GetComponent<BottleController>();
                        var tempPos = firstController.transform.position;
                        firstController.transform.position = Vector3.Lerp(tempPos,tempPos + new Vector3(0,.5f,0) ,timeBottleUp);
                    }
                    else
                    {
                        if (firstController == hit.collider.GetComponent<BottleController>())
                        {
                            var tempPos = firstController.transform.position;
                            firstController.transform.position = Vector3.Lerp(tempPos,tempPos - new Vector3(0,.5f,0),timeBottleUp);
                            firstController = null;
                        }
                        else
                        {
                            secondController = hit.collider.GetComponent<BottleController>();
                            firstController.BottleControllerRef = secondController;
                            
                            firstController.UpdateTopColorValues();
                            secondController.UpdateTopColorValues();  
                            if (secondController.FillBottleCheck(firstController.TopColor))
                            {
                                firstController.StartColorTransfer();
                                secondController = null;
                                firstController = null;
                            }
                            else
                            {
                                firstController = null;
                                secondController = null;
                            }
                        }
                    }
                }
            }
        }
    }
    
}
