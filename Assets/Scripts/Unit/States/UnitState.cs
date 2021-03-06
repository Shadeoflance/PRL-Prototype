﻿using UnityEngine;

public class UnitState
{
    protected Unit unit;
    public UnitState(Unit unit)
    {
        this.unit = unit;
    }

    public virtual void Jump()
    {
        if(unit.jumper != null)
            unit.jumper.Jump();
    }

    public virtual void Attack()
    {
        if (unit.attack != null)
            unit.attack.DoAttack();
    }

    public virtual void Move(Vector2 dir)
    {
        if(unit.mover != null)
            unit.mover.Move(dir);
    }

    public virtual void TakeDamage(float amount, GameObject source, bool invertBump = false)
    {
        unit.health.TakeDamage(amount, source, invertBump);
    }

    public virtual void Update() 
    {
    }

    public virtual void FixedUpdate()
    {
    }

    /// <summary>
    /// Change state to a new one.
    /// </summary>
    /// <param name="state">New state.</param>
    public virtual bool Transit(UnitState state)
    {
        unit.currentState = state;
        return true;
    }
}