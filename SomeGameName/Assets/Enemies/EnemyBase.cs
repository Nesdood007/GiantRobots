using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class EnemyBase<T> 
    where T : RegionBase
{
    public static Manager GameManager;
    public static Vector3 Gravity;
    public static System.Random Random;
    System.Object lockGM = new System.Object();
    System.Object lockGravity = new System.Object();
    System.Object lockRandom = new System.Object();

    public EnemyBase(int health, int damage, float speed, bool canWanderThroughRegions, float spawnRate, Rarity rarity)        
    {
        Health = health;
        Damage = damage;
        Speed = speed;
        CanWanderThroughRegions = CanWanderThroughRegions;
        IsAlive = true;
        Rarity = rarity;
        SpawnRate = spawnRate;

        lock(lockGM)
            GameManager = GameManager ?? GameObject.FindGameObjectWithTag("GameManager").GetComponent<Manager>();
        lock(lockGravity)
            Gravity = Vector3.down * 5f;
        lock(lockRandom)
            Random = Random ?? new System.Random();
    }

    public void OnUpdate()
    {
        if (GameManager.GameIsGoing && IsAlive)
        {
            ExecuteOnUpdate();
        }
    }

    public bool IsAlive
    {
        get; set;
    }

    public int Health
    {
        get;
        private set;
    }

    public int Damage
    {
        get;
        private set;
    }

    public float Speed
    {
        get;
        set;
    }

    public bool CanWanderThroughRegions
    {
        get;
        private set;
    }

    public Effects DamageEffect
    {
        get;
        protected set;
    }

    public List<Effects> Resistances
    {
        get;
        protected set;
    }

    public float SpawnRate
    {
        get;
        private set;
    }

    public Rarity Rarity
    {
        get;
        private set;
    }

    public abstract void ExecuteOnUpdate();

    public abstract void Move(CharacterController characterController, Transform transform);

    public abstract void Attack();

    public abstract void DropItem();

}
