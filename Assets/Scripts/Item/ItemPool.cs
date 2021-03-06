﻿using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Collections.Generic;

struct Bundle
{
    public Bundle(string path, Action action)
    {
        this.sprite = Resources.Load<Sprite>("Items/" + path);
        this.action = action;
    }
    public Sprite sprite;
    public Action action;
}
public static class ItemPool
{
    static Dictionary<int, Bundle> items;

    public static void AssignItem(Item item, int id = 0)
    {
        if (id == 0 || !items.ContainsKey(id))
            id = GenerateId();
        Bundle bundle = items[id];
        item.id = id;
        item.getAction = bundle.action;
        item.GetComponent<SpriteRenderer>().sprite = bundle.sprite;
        Remove(id);
    }

    static int GenerateId()
    {
        if (items.Count == 0)
            return 1;//extraDmg item id
        List<int> ids = new List<int>();
        ids.AddRange(items.Keys);
        return ids[Random.Range(0, ids.Count)];
    }

    public static void Remove(int id)
    {
        if (id == 1)
            return;
        items.Remove(id);
    }

    static ItemPool()
    {
        items = new Dictionary<int, Bundle>();

        items.Add(1, new Bundle("extradmg", () =>
        {
            Player.instance.attack.dmgUps++;
        }));

        items.Add(2, new Bundle("extrajump", () =>
            {
                if (Player.instance.jumper is DefaultJumper)
                {
                    var jumper = (DefaultJumper)Player.instance.jumper;
                    jumper.AddExtraJumps(1);
                }
            }
        ));

        items.Add(3, new Bundle("dmgafterdash", () =>
            {
                Player.instance.eventManager.SubscribeHandler("dashFinish", new DmgAfterDash());
            }
        ));

        items.Add(4, new Bundle("crit", () =>
            {
                Player.instance.eventManager.SubscribeInterceptor("shoot", new Crit());
            }
        ));

        items.Add(5, new Bundle("distancedmg", () =>
            {
                (Player.instance.attack as BulletWeapon).bulletFactory.AddModifier(new DistanceDmg());
            }
        ));

        items.Add(6, new Bundle("fly", () =>
        {
            Player.instance.mover = new Flyer(Player.instance);
            Player.instance.gravityScale = 0;
            Player.instance.jumper = null;
        }));

        items.Add(7, new Bundle("piercingshot", () =>
        {
            BulletWeapon w = Player.instance.attack as BulletWeapon;
            w.bulletFactory.SetSprite(Resources.Load<Sprite>("triangle"));
            Player.instance.eventManager.SubscribeInterceptor("bulletUnitHit", new LambdaActionListner((ActionParams ap) =>
            {
                ap.forbid = true;
                return false;
            }));
        }));

        items.Add(8, new Bundle("healonkill", () =>
        {
            Player.instance.eventManager.SubscribeHandler("kill", new LambdaActionListner((ActionParams ap) =>
            {
                if(Random.Range(0f, 1f) < 0.7f)
                {
                    Player.instance.stats.hp++;
                    HealOnKillBlood.Create(Player.instance.transform.position);
                }
                return false;
            }));
        }));

        items.Add(9, new Bundle("enemyexplode", () =>
        {
            Player.instance.eventManager.SubscribeHandler("kill", new LambdaActionListner((ActionParams ap) =>
            {
                OrbBomb.CreateExplosion(LayerMask.GetMask("Enemy", "Level"), (ap["victim"] as Unit).transform.position);
                return false;
            }));
        }));
    }
    class DmgAfterDash : ActionListener
    {
        public bool Handle(ActionParams ap)
        {
            ap.unit.AddBuff(new DoubleDamageBuff(ap.unit, 3));
            return false;
        }
    }

    class Crit : ActionListener
    {
        public bool Handle(ActionParams ap)
        {
            if (Random.Range(0f, 1f) > 0.3f)
                return false;
            var b = ap["bullet"] as Bullet;
            b.GetComponent<SpriteRenderer>().color = Color.red;
            b.dmgMult += 2;
            return false;
        }
    }

    class DistanceDmg : BulletProcessingModifier
    {
        Vector2? prev;
        public void Modify(Bullet bullet)
        {
            if (prev == null)
            {
                prev = bullet.transform.position.ToV2();
                return;
            }
            float dist = (prev.Value - bullet.transform.position.ToV2()).magnitude;
            bullet.dmgMult += dist / 7;
            bullet.transform.localScale += new Vector3(dist / 18, dist / 18, 0);
            prev = bullet.transform.position.ToV2();
        }

        public BulletProcessingModifier Instantiate()
        {
            return new DistanceDmg();
        }
    }

}