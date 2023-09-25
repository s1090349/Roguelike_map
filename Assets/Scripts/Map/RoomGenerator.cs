using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomGenerator : MonoBehaviour
{
    public enum Direction { up, down, left, right };
    public Direction direction;

    [Header("房間信息")]
    public GameObject roomPrefab;
    public GameObject BossroomPrefab;
    public int roomNumber;
    public UnityEngine.Color startColor, endColor, bossColor; //开始房间、终点房、boss房颜色
    private GameObject endRoom;//最后的房间
    private GameObject bossRoom;//boss的房间

    [Header("位置控制")]
    public Transform generatorPoint;
    public float xOffset;
    public float yOffset;
    public LayerMask roomLayer;
    public int maxStep;
    public List<Room> rooms = new List<Room>();

    public GameObject Map;
    private bool map = true;

    List<GameObject> farRooms = new List<GameObject>();

    List<GameObject> lessFarRooms = new List<GameObject>();

    List<GameObject> oneWayRooms = new List<GameObject>();


    public WallType wallType;

    void Start()
    {
        for (int i = 0; i < roomNumber; i++)
        {
            rooms.Add(Instantiate(roomPrefab, generatorPoint.position, Quaternion.identity).GetComponent<Room>());

            //改变point位置
            ChangePointPos();
        }

        rooms[0].GetComponent<SpriteRenderer>().color = startColor;
        endRoom = rooms[0].gameObject;

        foreach (var room in rooms)
        {
            //sqrMagnitude 是指向量长度的平方,用于比较向量之间的大小
            if (room.transform.position.sqrMagnitude > endRoom.transform.position.sqrMagnitude)
            {
                endRoom = room.gameObject;
            }
        }
        endRoom.GetComponent<SpriteRenderer>().color = endColor;

        //已最远房间为起点，在周围创建一个boss房
        //以最远房间为起点，在周围创建一个boss房
        Vector3[] offsets = { new Vector3(0, yOffset, 0), new Vector3(0, -yOffset, 0), new Vector3(-xOffset, 0, 0), new Vector3(xOffset, 0, 0) };
        foreach (Vector3 offset in offsets)
        {
            Vector3 bossPosition = endRoom.transform.position;
            bossPosition += offset;
            if (!IfPositionCreated(bossPosition))
            {
                bossRoom = Instantiate(BossroomPrefab, bossPosition, Quaternion.identity);
                rooms.Add(bossRoom.GetComponent<Room>());
                bossRoom.GetComponent<SpriteRenderer>().color = bossColor;
                break;
            }
        }


        foreach (var room in rooms)
        {
            //if (room.transform.position.sqrMagnitude > endRoom.transform.position.sqrMagnitude)
            //{
            //    endRoom = room.gameObject;
            //}
            SetupRoom(room, room.transform.position);
        }
        FindEndRoom();

        
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            map = !map;
            if (map)
                Map.SetActive(false);
            else
                Map.SetActive(true);
        }
        //if (Input.anyKeyDown)
        //{
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //}

    }

    public void ChangePointPos()
    {
        do
        {
            direction = (Direction)Random.Range(0, 4);

            switch (direction)
            {
                case Direction.up:
                    generatorPoint.position += new Vector3(0, yOffset, 0);
                    break;
                case Direction.down:
                    generatorPoint.position += new Vector3(0, -yOffset, 0);
                    break;
                case Direction.left:
                    generatorPoint.position += new Vector3(-xOffset, 0, 0);
                    break;
                case Direction.right:
                    generatorPoint.position += new Vector3(xOffset, 0, 0);
                    break;
            }
        } while (Physics2D.OverlapCircle(generatorPoint.position, 0.2f, roomLayer));
    }

    public bool IfPositionCreated(Vector3 position)
    {
        foreach (var room in rooms)
        {
            if (room.gameObject.transform.position == position) return true;
        }
        return false;
    }

    public void SetupRoom(Room newRoom, Vector3 roomPosition)
    {
        newRoom.roomRight = IfPositionCreated(roomPosition + new Vector3(xOffset, 0, 0));
        newRoom.roomLeft = IfPositionCreated(roomPosition + new Vector3(-xOffset, 0, 0));
        newRoom.roomUp = IfPositionCreated(roomPosition + new Vector3(0, yOffset, 0));
        newRoom.roomDown = IfPositionCreated(roomPosition + new Vector3(0, -yOffset, 0));


        newRoom.UpdateRoom(xOffset, yOffset);

        switch (newRoom.doorNumber)
        {
            case 1:
                if (newRoom.roomUp)
                    Instantiate(wallType.singleUp, roomPosition, Quaternion.identity);
                if (newRoom.roomDown)
                    Instantiate(wallType.singleDown, roomPosition, Quaternion.identity);
                if (newRoom.roomLeft)
                    Instantiate(wallType.singleLeft, roomPosition, Quaternion.identity);
                if (newRoom.roomRight)
                    Instantiate(wallType.singleRight, roomPosition, Quaternion.identity);
                break;
            case 2:
                if (newRoom.roomUp & newRoom.roomRight)
                    Instantiate(wallType.doubleUR, roomPosition, Quaternion.identity);
                if (newRoom.roomUp & newRoom.roomLeft)
                    Instantiate(wallType.doubleUL, roomPosition, Quaternion.identity);
                if (newRoom.roomUp & newRoom.roomDown)
                    Instantiate(wallType.doubleUD, roomPosition, Quaternion.identity);
                if (newRoom.roomDown & newRoom.roomRight)
                    Instantiate(wallType.doubleDR, roomPosition, Quaternion.identity);
                if (newRoom.roomDown & newRoom.roomLeft)
                    Instantiate(wallType.doubleDL, roomPosition, Quaternion.identity);
                if (newRoom.roomRight & newRoom.roomLeft)
                    Instantiate(wallType.doubleRL, roomPosition, Quaternion.identity);
                break;
            case 3:
                if (newRoom.roomDown & newRoom.roomUp & newRoom.roomLeft)
                    Instantiate(wallType.tripleDUL, roomPosition, Quaternion.identity);
                if (newRoom.roomDown & newRoom.roomUp & newRoom.roomRight)
                    Instantiate(wallType.tripleDUR, roomPosition, Quaternion.identity);
                if (newRoom.roomRight & newRoom.roomLeft & newRoom.roomDown)
                    Instantiate(wallType.tripleRLD, roomPosition, Quaternion.identity);
                if (newRoom.roomUp & newRoom.roomRight & newRoom.roomLeft)
                    Instantiate(wallType.tripleURL, roomPosition, Quaternion.identity);
                break;
            case 4:
                if (newRoom.roomUp & newRoom.roomDown & newRoom.roomRight & newRoom.roomLeft)
                    Instantiate(wallType.fourDoors, roomPosition, Quaternion.identity);
                break;


        }
    }
    public void FindEndRoom()
    {
        //最大数值 最远距离数字
        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].stepToStart > maxStep)
                maxStep = rooms[i].stepToStart;
        }

        //获得最远房间和第二远
        foreach (var room in rooms)
        {
            if (room.stepToStart == maxStep)
                farRooms.Add(room.gameObject);
            if (room.stepToStart == maxStep - 1)
                lessFarRooms.Add(room.gameObject);
        }

        for (int i = 0; i < farRooms.Count; i++)
        {
            if (farRooms[i].GetComponent<Room>().doorNumber == 1)
                oneWayRooms.Add(farRooms[i]);//最远房间里的单侧门加入
        }

        for (int i = 0; i < lessFarRooms.Count; i++)
        {
            if (lessFarRooms[i].GetComponent<Room>().doorNumber == 1)
                oneWayRooms.Add(lessFarRooms[i]);//第二远远房间里的单侧门加入
        }

        if (oneWayRooms.Count != 0)
        {
            endRoom = oneWayRooms[Random.Range(0, oneWayRooms.Count)];
        }
        else
        {
            endRoom = farRooms[Random.Range(0, farRooms.Count)];
        }
    }

    //public string Getcurrentroomtag(gameobject room)
    //{
    //    获取特定房间的标签
    //    string roomtag = room.tag;
    //    print(roomtag);
    //    return roomtag;
    //}
}

[System.Serializable]

public class WallType
{
    public GameObject singleLeft, singleRight, singleUp, singleDown,
                      doubleDL, doubleDR, doubleRL, doubleUD, doubleUR, doubleUL,
                      tripleDUL, tripleDUR, tripleRLD, tripleURL,
                      fourDoors;
}
