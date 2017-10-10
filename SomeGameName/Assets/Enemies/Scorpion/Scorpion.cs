using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Scorpion : MonoBehaviour
{
    public int health = 10;
    public int damage = 5;
    public float speed = 5f;
    public bool canWanderThroughRegions = false;
    public float spawnRate = 5f;
    bool oldWanderingBounds;
    ScorpionObject scorpionObject;
    CharacterController characterController;


    void Awake()
    {
        scorpionObject = new ScorpionObject(health, damage, speed, canWanderThroughRegions, spawnRate);
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
       if(!oldWanderingBounds && scorpionObject.WanderingBoundsHasBeenSet)
        {
            scorpionObject.SetRandomDirection(transform);
            scorpionObject.SetRandomStartingPosition(transform);
        }
        if (scorpionObject.WanderingBoundsHasBeenSet && scorpionObject.IsAlive && ScorpionObject.GameManager.GameIsGoing)
        {
            scorpionObject.Speed = speed;
            scorpionObject.Move(characterController, transform);            
        }        
        oldWanderingBounds = scorpionObject.WanderingBoundsHasBeenSet;
    }

    public void SetEnemyWanderingBounds()
    {
        var regions = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Manager>().RegionPositions;
        if (regions.Keys.Contains(scorpionObject.PrimaryRegion))
        {
            scorpionObject.wanderingBounds = regions[scorpionObject.PrimaryRegion];
            scorpionObject.WanderingBoundsHasBeenSet = true;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player" || collider.gameObject.tag == "Enemy")
        {
            scorpionObject.SetRandomDirection(transform);
            Debug.Log("?");
        }
    }

}

public class ScorpionObject : EnemyBase<Desert>
{
    public Vector3 direction;
    public Rect wanderingBounds;
    public bool WanderingBoundsHasBeenSet = false;
    public Regions PrimaryRegion = Regions.Desert;    

    public ScorpionObject(int health, int damage, float speed, bool canWanderThroughRegions, float spawnRate)
        : base(health, damage, speed, canWanderThroughRegions, spawnRate, Rarity.Common)
    {     
        
    }

    public void SetRandomDirection()
    {
        direction = new Vector3((Random.Next() %2==0? -1 : 1) *(Random.Next() % 100), 0, (Random.Next() % 2 == 0 ? -1 : 1) * (Random.Next() % 100)).normalized;
        //transform.rotation = Quaternion.LookRotation(transform.TransformDirection(direction));
    }

    public void SetRandomDirection(Transform transform)
    {
        direction = new Vector3((Random.Next() % 2 == 0 ? -1 : 1) * (Random.Next() % 100), 0, (Random.Next() % 2 == 0 ? -1 : 1) * (Random.Next() % 100)).normalized;
        transform.rotation = Quaternion.LookRotation(transform.TransformDirection(direction));
    }

    public void SetRandomStartingPosition(Transform transform)
    {
        var startingPosition = new Vector3(Random.Next() % wanderingBounds.width + wanderingBounds.xMin, 0, Random.Next() % wanderingBounds.height + wanderingBounds.yMin);
        transform.position = new Vector3(startingPosition.x, Terrain.activeTerrain.SampleHeight(startingPosition) + 2f, startingPosition.z);
    }

    public void SetEnemyWanderingBounds()
    {
        var regions = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Manager>().RegionPositions;
        
        if (regions.Keys.Contains(PrimaryRegion))
        {
            wanderingBounds = regions[PrimaryRegion];
            WanderingBoundsHasBeenSet = true;
            
        }
    }

    internal Rect GetEnemyWanderingBounds()
    {
        return wanderingBounds;
    }

    public override void ExecuteOnUpdate()
    {

    }

    public override void Move(CharacterController characterController, Transform transform)
    {
        
        var newPos = transform.TransformDirection(-.5f, 0, -.5f);

        newPos *= Speed * Time.deltaTime;
        characterController.SimpleMove(newPos + Gravity);

        var projectedPosition = characterController.velocity + transform.position;
        if (projectedPosition.x < wanderingBounds.xMin || projectedPosition.x > wanderingBounds.xMax || projectedPosition.z < wanderingBounds.yMin || projectedPosition.z > wanderingBounds.yMax || TerrainModifier.Grasslands.PointIsInCircle(new Vector2(projectedPosition.x, projectedPosition.z)))
        {
            SetRandomDirection(transform);
        }
    }

   
    

    public override void Attack()
    {

    }

    public override void DropItem()
    {

    }
}
