﻿using UnityEngine;
using System.Collections.Generic;

public class Bullet : MonoBehaviour
{
    public Rigidbody2D rb;
    public Vector2 needVel;
    public float speed;
    public float? life = null;
    public Unit unit;
    public float dmgMult;
    public int dmgMask;
    public Group<BulletProcessingModifier> modifiers = new Group<BulletProcessingModifier>();

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        modifiers.Refresh();
        foreach (var a in modifiers)
            a.Modify(this);
        rb.velocity = needVel * speed;
        transform.Rotate(0, 0, Vector3.Angle(transform.up, rb.velocity));
        if (life != null)
        {
            life -= Time.deltaTime;
            if (life < 0)
                Destroy(gameObject);
        }
    }

    HashSet<Unit> alreadyHit = new HashSet<Unit>();
    void OnTriggerEnter2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer & dmgMask) != 0)
        {
            Unit victim = collision.gameObject.GetComponent<Unit>();
            if (alreadyHit.Contains(victim))
                return;
            else
                alreadyHit.Add(victim);
            ActionParams ap = new ActionParams();
            ap["victim"] = victim;
            ap["bullet"] = this;
            ap["dmgMult"] = dmgMult;
            unit.eventManager.InvokeInterceptors("bulletUnitHit", ap);
            if (!ap.forbid)
            {
                unit.eventManager.InvokeHandlers("bulletDestroy", null);
                Destroy(gameObject);
            }
            unit.attack.DealDamage(victim, (float)ap.parameters["dmgMult"]);
            unit.eventManager.InvokeHandlers("bulletUnitHit", ap);
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Level"))
        {
            if (unit is Player)
            {
                if (collision.gameObject.tag == "Door")
                    collision.GetComponent<Door>().Open();
            }
            ActionParams ap = new ActionParams();
            ap["other"] = collision.gameObject;
            ap["bullet"] = this;
            unit.eventManager.InvokeInterceptors("bulletLevelHit", ap);
            if (!ap.forbid)
            {
                ap = new ActionParams();
                ap["bullet"] = this;
                unit.eventManager.InvokeHandlers("bulletDestroy", ap);
                Destroy(gameObject);
            }
        }
    }
}