public interface Interactable
{
    //显示给玩家看的名字
    string GetInteractText();

    //执行交互
    void Interact();

    //进入范围
    void OnFocus();

    //离开范围
    void OnLoseFocus();
}
