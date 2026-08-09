using UnityEngine;

public class PlayerInteractionTrigger : MonoBehaviour
{
    public InteractionManager interactionManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            BaseInteractable interactObj = other.GetComponent<BaseInteractable>();
            if (interactObj != null)
            {
                interactionManager.AddInteractable(interactObj);
                Debug.Log("´¥Åö£º" + other.name);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            BaseInteractable interactObj = other.GetComponent<BaseInteractable>();
            if (interactObj != null)
            {
                interactionManager.RemoveInteractable(interactObj);
                Debug.Log("ÒÑÀë¿ª£º" + other.name);
            }
        }
    }
}