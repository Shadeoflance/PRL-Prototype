﻿using UnityEngine;
using System;

class Level : MonoBehaviour
{
    public static Vector2 roomSize = new Vector2(20, 15);
    public Map map = new Map(3);
    public static Level instance;
    public Room current;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(instance);
            instance = this;
        }
    }

    void Start()
    {
        GameObject roomPrefab = Resources.Load<GameObject>("Level/SubRoom");
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (i == 1 && j == 0)
                    continue;
                map[i, j] = Instantiate<GameObject>(roomPrefab).GetComponent<SubRoom>();
            }
        }
        map[1, 2].room.Unite(map[2, 2].room);
        map.FixDoors();
        current = map[1, 1].room;
        current.Enable();
        Player.instance.transform.position = current.subRooms[0].transform.position;
        map.UpdateUI();
        CameraScript.instance.RefreshBorders();
    }

    public void ChangeRoom(Vector2 dir, SubRoom subRoom)
    {
        SubRoom next = map.GetRelativeTo(subRoom, dir);
        RoomChangeEffect.Create(subRoom, next);
        current = next.room;
        map.UpdateUI();
        CameraScript.instance.RefreshBorders();
    }
}