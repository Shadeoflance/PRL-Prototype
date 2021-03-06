﻿using UnityEngine;
using System.Collections.Generic;

public class DefaultSlamer : Slamer
{
    public float speed, range, dmgMultiplier, stunDuration;
    public DefaultSlamer(Player p, float dmgMultiplier = 3f, float speed = 50, float range = 3, float stunDuration = 3f)
        : base(p)
    {
        this.speed = speed;
        this.range = range;
        this.dmgMultiplier = dmgMultiplier;
        this.stunDuration = stunDuration;
    }

    public override void Slam()
    {
        if (!CanSlam())
            return;
        base.Slam();
        player.currentState.Transit(new SlamState(player, speed, range));
        player.eventManager.SubscribeHandler("slamFinish", new SlamDmg(dmgMultiplier));
        player.eventManager.SubscribeHandler("slamFinish", new SlamStun(stunDuration));
    }

    class SlamDmg : ActionListener
    {
        float dmgMult;
        public SlamDmg(float dmgMult)
        {
            this.dmgMult = dmgMult;
        }
        public bool Handle(ActionParams ap)
        {
            foreach (var a in (IEnumerable<Enemy>)ap["enemies"])
            {
                if (a == null)
                    continue;
                ap.unit.attack.DealDamage(a, dmgMult);
            }
            return true;
        }
    }

    class SlamStun : ActionListener
    {
        float duration;
        public SlamStun(float duration)
        {
            this.duration = duration;
        }

        public bool Handle(ActionParams ap)
        {
            foreach (var a in (IEnumerable<Enemy>)ap["enemies"])
            {
                if (a == null)
                    continue;
                a.AddBuff(new StunnedBuff(a, duration));
            }
            return true;
        }
    }
}