﻿using System.Collections.Generic;
using UnityEngine;

class Room
{
    public List<SubRoom> subRooms = new List<SubRoom>();
    public RoomUI roomUI;

    public Room(SubRoom subRoom)
    {
        subRooms.Add(subRoom);
    }

    public void Unite(Room r)
    {
        foreach (var a in r.subRooms)
        {
            a.room = this;
            subRooms.Add(a);
        }
    }

    public void EnemyDied(Enemy enemy)
    {
        foreach (var a in subRooms)
            a.EnemyDied(enemy);
        foreach (var a in subRooms)
            if (a.enemiesAlive.Count > 0)
                return;
        EnableDoors();
    }

    public void InitUI()
    {
        roomUI = new RoomUI(this);
    }

    public void Enable()
    {
        foreach (var a in subRooms)
            a.Enable();
    }

    public void Disable()
    {
        foreach (var a in subRooms)
            a.Disable();
    }

    public void EnableDoors()
    {
        foreach (var a in subRooms)
            a.EnableDoors();
    }

    public void DisableDoors()
    {
        foreach (var a in subRooms)
            a.DisableDoors();
    }
}