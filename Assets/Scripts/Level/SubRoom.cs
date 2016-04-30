﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

class SubRoom : MonoBehaviour
{
    public Room room;
    Dictionary<Vector2, Door> doors = new Dictionary<Vector2,Door>();
    public List<Enemy> enemiesAlive = new List<Enemy>();

    void Awake()
    {
    }

    void Start()
    {
        if (enemiesAlive.Count > 0)
            DisableDoors();
    }

    public void CreateWalls()
    {
        foreach (var dir in Map.dirs)
        {
            SubRoom s = Level.instance.map.GetRelativeTo(this, dir);
            if (!room.subRooms.Contains(s))
            {
                CreateSingleWall(dir);
            }
        }
    }

    private void CreateSingleWall(Vector2 dir)
    {
        GameObject wall = Instantiate(Resources.Load<GameObject>("Level/Wall"));
        Transform parent = transform.FindChild("Walls").transform;
        wall.transform.position = transform.position + Vector2.Scale(Level.roomSize / 2, dir).ToV3();
        if(dir.y != 0)
        {
            wall.transform.Rotate(0, 0, 90);
            wall.transform.localScale *= Level.roomSize.x / Level.roomSize.y;
        }
        wall.transform.SetParent(parent);
    }

    public void CreateDoors()
    {
        foreach(var dir in Map.dirs)
        {
            SubRoom s = Level.instance.map.GetRelativeTo(this, dir);
            if (s != null && !room.subRooms.Contains(s))
            {
                CreateSingleDoor(dir);
            }
        }
    }
    
    private void CreateSingleDoor(Vector2 dir)
    {
        Transform parent = transform.FindChild("Doors");
        Vector2 doorDistance = new Vector2((Level.roomSize.x - 1) / 2, (Level.roomSize.y - 1) / 2);
        GameObject doorInstance = Instantiate(Resources.Load<GameObject>("Level/Door"));
        doorInstance.name = "Door" + dir;
        doorInstance.transform.position = transform.position + Vector2.Scale(doorDistance, dir).ToV3();
        if(dir == Vector2.down)
            doorInstance.transform.Rotate(0, 0, 270);
        else if(dir == Vector2.left)
            doorInstance.transform.Rotate(0, 0, 180);
        else if(dir == Vector2.up)
            doorInstance.transform.Rotate(0, 0, 90);
        doorInstance.transform.SetParent(parent.transform);
        doors.Add(dir, doorInstance.GetComponent<Door>());
    }

    public void GenerateEnemies(string name, int amount)
    {
        GameObject prefab = Resources.Load<GameObject>("Enemies/" + name);
        Transform platforms = transform.FindChild("Platforms");
        for (int i = 0; i < amount; i++)
        {
            BoxCollider2D randomPlatform = platforms.GetChild(Random.Range(0, platforms.childCount)).GetComponent<BoxCollider2D>();
            Vector2 rightUpperCorner = randomPlatform.transform.position.ToV2() + Vector2.Scale(randomPlatform.size, randomPlatform.transform.localScale.ToV2()) / 2 + randomPlatform.offset;
            Vector2 position = rightUpperCorner - new Vector2(randomPlatform.size.x * 0.9f * transform.localScale.x, 0) * Random.Range(0f, 1f) + new Vector2(0, 0.4f);
            GameObject instance = Instantiate(prefab);
            instance.transform.SetParent(transform);
            instance.transform.position = position;
            enemiesAlive.Add(instance.GetComponent<Enemy>());
            //instance.SetActive(gameObject.activeInHierarchy);
        }
    }

    public void EnemyDied(Enemy enemy)
    {
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