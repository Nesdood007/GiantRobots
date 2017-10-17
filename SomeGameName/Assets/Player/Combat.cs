using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour {

    public float startingHealth = 100f;
    public float damage = 5f;
    public bool IsAlive = true;
    float health;

    public float Health
    {
        get { return health; }
        private set { health = value; }
    }

    public float PercentHealth
    {
        get { return health/startingHealth; }
    }
    
	// Use this for initialization
	void Start () {
        Health = startingHealth;
	}

    // Update is called once per frame
    void Update() {
        if (!IsAlive)
            OnDeath();

    }

    public void TakePercentDamage(float percent)
    {
        if (percent <= 0 || percent > 1)
            return;
        TakeDamage(Health * percent);
    }

    public void TakeDamage(float damage)
    {
        if (damage > 0)
            Health -= damage;
        if (health < 0)
        {
            IsAlive = false;
            OnDeath();            
        }

        Debug.Log(Health);
    }

    public void OnDeath()
    {
        Destroy(this);
    }
}
