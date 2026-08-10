using UnityEngine;


public class ContainerInteract : MonoBehaviour, IInteractable
{


    public string containerName;



    public string GetInteractText()
    {
        return "搜索 " + containerName;
    }



    public void Interact()
    {

        Debug.Log(
            "打开容器"
        );


        //以后连接：
        //背包系统
        //随机掉落系统

    }



    public void OnFocus()
    {

    }



    public void OnLoseFocus()
    {

    }

}