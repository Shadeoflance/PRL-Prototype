﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class Player : Unit
{
    public bool god = false;
    public Dasher dasher;
    public Slamer slamer;
    public BoxCollider2D main;
    public Transform gun;
    public int pixels = 0;
    public int orbs = 0;
    public float jumpForce, jumpHeight;
    protected override void Awake()
    {
        base.Awake();
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(instance.gameObject);
            instance = this;
        }
    }
    protected override void Start()
    {
        stats.hp = hp;
        stats.speed = speed;
        stats.damage = damage;
		controller = new PlayerController(this);
        jumper = new DefaultJumper(this, 1);
        mover = new DefaultMover(this);
        attack = new BulletWeapon(this);
        health = new Health(this);
        dasher = new DefaultDasher(this);
        slamer = new DefaultSlamer(this);
        currentState = new PlayerWalkingState(this);
        walking = new PlayerWalkingState(this);
        airborne = new PlayerAirborneState(this);
        eventManager.SubscribeHandler("jumpButtonDown", new JumpInvoker());
        eventManager.SubscribeHandler("dashButtonDown", new DashInvoker());
        eventManager.SubscribeHandler("takeDmg", new DamageBoost());
        eventManager.SubscribeHandler("bombButtonDown", new BombDropInvoker());
        base.Start();
        if(god)
        {
            mover = new Flyer(this);
            stats.hp = 100;
            stats.damage = 10;
        }
	}

    public static float Distance(Vector2 v)
    {
        if (instance == null)
            return float.MaxValue;
        return (v.ToV3() - instance.transform.position).magnitude;
    }

    public static void IgnoreEnemyCollisions(bool value)
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), value);
    }

    public static Player instance;
    protected override void Update()
    {
        base.Update();
        if(dasher != null)
            dasher.Update();
        if(slamer != null)
            slamer.Update();


        Vector2 dir, vel = controller.NeedVel();
        if (vel.y > 0)
        {
            dir = Vector2.up;
            gun.transform.rotation = Quaternion.AngleAxis(90, Vector3.forward);
        }
        else if (currentState == airborne && vel.y < 0)
        {
            dir = Vector2.down;
            gun.transform.rotation = Quaternion.AngleAxis(270, Vector3.forward);
        }
        else
        {
            dir = new Vector2(direction, 0);
            if(direction == -1)
                gun.transform.rotation = Quaternion.AngleAxis(180, Vector3.forward);
            else gun.transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
        }
        //gun.transform.Rotate(0, 0, Vector3.Angle(gun.transform.right, dir));
        
    }

    public override void AddBuff(Buff b)
    {
        base.AddBuff(b);
        b.ChangeToPlayerBuff();
    }

    public void AddPixel(int amount)
    {
        pixels += amount;
        PickupsUI.Update();
    }

    public void AddOrbs(int amount)
    {
        orbs += amount;
        PickupsUI.Update();
    }

    class JumpInvoker : ActionListener
    {
        public bool Handle(ActionParams ap)
        {
            ap.unit.currentState.Jump();
            return false;
        }
    }

    class DashInvoker : ActionListener
    {
        public bool Handle(ActionParams ap)
        {
            (ap.unit.currentState as PlayerState).Dash();
            return false;
        }
    }

    class BombDropInvoker : ActionListener
    {
        public bool Handle(ActionParams ap)
        {
            if (instance.orbs > 0)
            {
                OrbBomb.Drop(ap.unit.transform.position);
                instance.orbs--;
                PickupsUI.Update();
            }
            return false;
        }
    }
}
