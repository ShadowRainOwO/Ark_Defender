using UnityEngine;


public class DoorInteract : MonoBehaviour, Interactable
{


    public bool isOpen;



    public string GetInteractText()
    {
        return isOpen ?
        "关闭门" :
        "打开门";
    }



    public void Interact()
    {

        isOpen = !isOpen;


        if (isOpen)
        {
            Debug.Log("门打开");
        }
        else
        {
            Debug.Log("门关闭");
        }

    }



    public void OnFocus()
    {

    }



    public void OnLoseFocus()
    {

    }

}