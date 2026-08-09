using UnityEngine;


public class NPCInteract : MonoBehaviour, Interactable
{


    public string npcName;


    public string GetInteractText()
    {
        return "与 " + npcName + " 交谈";
    }



    public void Interact()
    {

        Debug.Log(
            "打开NPC菜单"
        );


        //这里以后接：
        //交易系统
        //任务系统
        //对话系统

    }



    public void OnFocus()
    {
        Debug.Log(
            "玩家靠近NPC"
        );
    }



    public void OnLoseFocus()
    {

    }

}