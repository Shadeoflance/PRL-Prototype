﻿using UnityEngine;

class RoomChangeEffect : MonoBehaviour
{
    Vector3 from, to;
    Vector2 dir;
    float time, initialTime = 0.5f;
    SubRoom current, next;
    void Start()
    {
        next.room.Enable();
        time = initialTime;
        Player.instance.enabled = false;
        dir = (next.transform.position - current.transform.position).ToV2().normalized;
        from = CameraScript.instance.transform.position;
        to = CameraScript.GetPositionToSide(next, -dir);
    }

    void End()
    {
        Player.instance.enabled = true;
        current.room.Disable();
        Player.instance.transform.position = next.transform.position + 
            Vector2.Scale(-dir, new Vector2(8f, 5.5f)).ToV3();
        Destroy(gameObject);
    }
    
    public static void Create(SubRoom current, SubRoom next)
    {
        GameObject newInstance = new GameObject("Room Change Effect");
        RoomChangeEffect effect = newInstance.AddComponent<RoomChangeEffect>();
        effect.current = current;
        effect.next = next;
    }

    void Update()
    {
        CameraScript.instance.transform.position = Vector3.Lerp(from, to, 1 - time / initialTime);
        time -= Time.deltaTime;
        if (time < 0)
            End();
    }
}