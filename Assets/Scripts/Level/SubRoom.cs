﻿using UnityEngine;
using System.Collections.Generic;

class SubRoom : MonoBehaviour
{
    public Door rightD, leftD, topD, botD;
    public Room room;
    public GameObject rightW, leftW, topW, botW;
    Dictionary<Vector2, Door> doors = new Dictionary<Vector2,Door>();
    public List<Enemy> enemiesAlive = new List<Enemy>();

    void Awake()
    {
        doors.Add(Vector2.up, topD);
        doors.Add(Vector2.down, botD);
        doors.Add(Vector2.left, leftD);
        doors.Add(Vector2.right, rightD);
    }

    void Start()
    {
        foreach (Transform a in transform)
            if (a.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                enemiesAlive.Add(a.GetComponent<Enemy>());
        if (enemiesAlive.Count > 0)
            DisableDoors();
    }

    public void EnemyDied(Enemy enemy)
    {
        Debug.LogWarning("enemy died");
        enemiesAlive.Remove(enemy);
    }

    public void DoorTouch(Door door)
    {
        foreach (var a in doors.Keys)
        {
            Door d = doors[a];
            if(door == d)
                Level.instance.ChangeRoom(a, this);
        }
    }

    public void Disable()
    {
        foreach (var a in doors.Values)
            if (a != null)
                a.Close();
        gameObject.SetActive(false);
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void EnableDoors()
    {
        foreach (var a in doors.Values)
            if(a != null)
                a.Enable();
    }

    public void DisableDoors()
    {
        foreach (var a in doors.Values)
            if(a != null)
                a.Disable();
    }

    public void WrapInRoom()
    {
        room = new Room(this);
    }
}