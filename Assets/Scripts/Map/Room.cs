using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
    public GameObject doorLeft, doorRight, doorUp, doorDown;//用来存放各个位置的门

    public bool roomLeft, roomRight, roomUp, roomDown;//判断上下左右是否有房间

    public Text textnumber;
    public int stepToStart;//距离初始点的网格距离
    public int doorNumber;

    void Start()
    {
        //对应方向的门是否显示，关联对应方向是否有其他房间
        doorLeft.SetActive(roomLeft);
        doorRight.SetActive(roomRight);
        doorUp.SetActive(roomUp);
        doorDown.SetActive(roomDown);
    }

    public void UpdateRoom(float xOffset, float yOffset)
    {
        //计算距离初始点的网格距离
        stepToStart = (int)(Mathf.Abs(transform.position.x / xOffset) + Mathf.Abs(transform.position.y / yOffset));

        string text = stepToStart.ToString();
        textnumber.text = text;

        if (roomUp)
            doorNumber++;
        if (roomDown)
            doorNumber++;
        if (roomLeft)
            doorNumber++;
        if (roomRight)
            doorNumber++;
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            
            CamerController.instance.ChangeTarget(transform);
        }
    }
}