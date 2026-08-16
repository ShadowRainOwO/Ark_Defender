public interface IInteractable
{
    string GetInteractText();
    void Interact();
    void OnFocus();
    void OnLoseFocus();
}
