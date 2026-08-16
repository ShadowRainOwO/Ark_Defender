using UnityEngine;

public class DoorInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private bool isOpen;

    public string GetInteractText()
    {
        return isOpen ? "关闭门" : "打开门";
    }

    public void Interact()
    {
        isOpen = !isOpen;
        Debug.Log(isOpen ? "门已打开" : "门已关闭", this);
    }

    public void OnFocus()
    {
    }

    public void OnLoseFocus()
    {
    }
}
