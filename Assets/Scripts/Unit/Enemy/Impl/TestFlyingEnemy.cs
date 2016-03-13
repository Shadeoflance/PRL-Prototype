﻿using UnityEngine;

class TestFlyingEnemy : Enemy
{
    void Start()
    {
        controller = new TFEController(this);
        mover = new Flyer(this, speed);
        health = new Health(this, 2);
        walking = new WalkingState(this);
        airborne = new AirborneState(this);
    }
}

class TFEController : IController
{
    Unit unit;

    public TFEController(Unit unit)
    {
        this.unit = unit;
    }

    public bool NeedAttack()
    {
        return false;
    }

    public bool NeedJump()
    {
        return false;
    }

    public Vector2 NeedVel()
    {
        if (Player.instance == null)
            return Vector2.zero;
        Vector2 toPlayer = VectorUtils.V3ToV2(Player.instance.transform.position - unit.transform.position);
        if (toPlayer.magnitude < 4)
            return toPlayer.normalized;
        else return Vector2.zero;
    }

    public void Update()
    {
    }
}