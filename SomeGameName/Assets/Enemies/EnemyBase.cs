﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public abstract class EnemyBase    
{
    public static Manager GameManager;
    public static readonly Vector3 Gravity = Vector3.down * 5f;
    public static System.Random Random;
    static Texture2D healthBarFull;
    static Texture2D healthBarEmpty;

    System.Object lockGM = new System.Object();
    System.Object lockRandom = new System.Object();
    

    float healthBarWidth = 50f;
    float healthBarHeight = 10f;
    float startingHealth;

    public EnemyBase(int health, int damage, float speed, float spawnRate, Rarity rarity, Regions primaryRegion)        
    {
        Health = health;
        startingHealth = Health;
        Damage = damage;
        Speed = speed;        
        IsAlive = true;
        Rarity = rarity;
        SpawnRate = spawnRate;
        PrimaryRegion = primaryRegion;
        lock (lockGM)
            GameManager = GameManager ?? GameObject.FindGameObjectWithTag("GameManager").GetComponent<Manager>();
        lock(lockRandom)
            Random = Random ?? new System.Random();   
        
    }

    public Regions PrimaryRegion
    {
        get;
        protected set;
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

    public Vector3 RemoveY(Vector3 direction)
    {
        return new Vector3(direction.x, 0f, direction.z);
    }

    public Vector3 RemoveX(Vector3 direction)
    {
        return new Vector3(0f, direction.y, direction.z);
    }

    

    public abstract void Move(CharacterController characterController, Transform transform);

    public abstract void Attack(CharacterController characterController, Transform transform);

    public abstract void DropItem();

}
