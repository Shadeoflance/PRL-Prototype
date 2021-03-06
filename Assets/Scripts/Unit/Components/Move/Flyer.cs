﻿using UnityEngine;

class Flyer : Mover
{
    public Flyer(Unit unit)
        : base(unit)
    {
        unit.gravityScale = 0;
        unit.rb.gravityScale = 0;
    }

    public override void Move(Vector2 dir)
    {
        unit.rb.velocity = dir * unit.stats.speed;
        if (dir.magnitude > 0)
        {
            unit.direction = dir.x > 0 ? 1 : -1;
        }
    }
}