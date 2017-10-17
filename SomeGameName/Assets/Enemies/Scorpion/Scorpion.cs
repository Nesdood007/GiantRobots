using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Scorpion : MonoBehaviour
{
    public bool AreDebugging = false;
    public bool canWanderThroughRegions = false;
    public int damage = 5;
    public int health = 10;
    public float playerAttackRadius = 5f;
    public float playerRadiusVision = 20f;
    public float spawnRate = 5f;
    public float speed = 5f;


    bool oldCanBegin = false;
    CharacterController characterController;
    Texture2D healthBarFull;
    Texture2D healthBarEmpty;
    ScorpionObject scorpionObject;
    Collider collider;
    HealthBar healthBar;
    
    void Awake()
    {
        
        characterController = GetComponent<CharacterController>();
        collider = GetComponent<BoxCollider>();
        healthBarFull = (Texture2D)Resources.Load("fullHealth");
        healthBarEmpty = (Texture2D)Resources.Load("emptyHealth");
       
        healthBar = GetComponentInChildren<HealthBar>();
        healthBar.totalHealth = health;
        healthBar.enabled = false;
        scorpionObject = new ScorpionObject(health, damage, speed, canWanderThroughRegions, spawnRate, playerRadiusVision, playerAttackRadius);
    }

    void Update()
    {
       if(!oldCanBegin && scorpionObject.CanBegin)
        {
            scorpionObject.SetRandomDirection(transform);
            scorpionObject.SetRandomStartingPosition(transform);
        }
        if (scorpionObject.CanBegin && scorpionObject.IsAlive && ScorpionObject.GameManager.GameIsGoing)
        {
            if (!scorpionObject.IsAttacking)
                scorpionObject.Move(characterController, transform);
            else
            {
                if (!healthBar.isActiveAndEnabled)
                    healthBar.enabled = true;
                healthBar.currentHealth = scorpionObject.Health;
                scorpionObject.Attack(characterController, transform);
            }
        }
        oldCanBegin = scorpionObject.CanBegin;

        //DEBUG
        if(AreDebugging && scorpionObject.CanBegin)
        {
            scorpionObject.Speed = speed;
            scorpionObject.AttackStartingRange = playerAttackRadius;
            healthBar.totalHealth = health;
        }
    }

    //public void OnGUI()
    //{
    //    if (scorpionObject.CanBegin && scorpionObject.IsAlive && ScorpionObject.GameManager.GameIsGoing)
    //    {
    //        if (scorpionObject.IsAttacking)
    //            scorpionObject.OnGUIAttack(transform, collider);
    //    }
    //}

    public void SetWanderingBounds()
    {
        scorpionObject.SetWanderingBounds();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Enemy")
            scorpionObject.SetRandomDirection(transform);
        else if (collider.gameObject.tag == "Player")
        {
            scorpionObject.IsRunningAtPlayer = false;
            collider.gameObject.GetComponent<Combat>().TakeDamage(scorpionObject.Damage);
        }
    }

    public void OnDeath()
    {
        GameObject.Destroy(this);
    }

}

public class ScorpionObject : RoamingEnemy
{
   
    public ScorpionObject(int health, int damage, float speed, bool canWanderThroughRegions, float spawnRate, float playerVisionRadius, float attackStartingRange)
        : base(health, damage, speed, spawnRate, Rarity.Common, Regions.Desert, playerVisionRadius, canWanderThroughRegions)
    {
        IsRunningAtPlayer = false;
        AttackStartingRange = attackStartingRange;
    }

    public float AttackStartingRange
    {
        get;
        set;
    }

    public bool IsRunningAtPlayer
    {
        get;
        internal set;
    }

    public override void Attack(CharacterController characterController, Transform transform)
    {
        if(TargetPlayer == null || TargetPlayerCollider == null)
        {
            IsAttacking = false;
            return;
        }        
        
        transform.rotation = Quaternion.Euler(RemoveX(Quaternion.LookRotation(TargetPlayer.transform.position - transform.position).eulerAngles));
        var distance = Vector3.Distance(TargetPlayer.transform.position, transform.position);

        if (!IsRunningAtPlayer && distance <= AttackStartingRange)
        {
            var newPos = RemoveY(transform.TransformDirection(-Vector3.forward));

            newPos *= Speed * Time.deltaTime;
            characterController.SimpleMove(newPos + Gravity);           
        } else
        {
            IsRunningAtPlayer = true;
            var newPos = RemoveY(transform.TransformDirection(Vector3.forward));

            newPos *= Speed * Time.deltaTime * 3f;
            characterController.SimpleMove(newPos + Gravity);
        }

    }

    public void OnGUIAttack(Transform transform, Collider collider)
    {
        //DrawHealthBar(TargetPlayerCamera, transform, collider);
    }

    public override void DropItem()
    {

    }
}
