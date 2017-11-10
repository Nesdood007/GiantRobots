using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour {

    
    public int CurrentHealth = 100;
    public int StartingHealth = 100;
    public int Armor = 0;
    public int Strength = 10;
    Dictionary<Effects, int> Resistances;
    Dictionary<Effects, int> DamageEffects;
    public Dictionary<Effects, float> ResistancePercentages;
    Dictionary<Effects, float> DamageEffectsPercentages;


    // Use this for initialization
    void Start () {
        Resistances = new Dictionary<Effects, int>();
        DamageEffects = new Dictionary<Effects, int>();
        ResistancePercentages = new Dictionary<Effects, float>();
        DamageEffectsPercentages = new Dictionary<Effects, float>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CalculateDamageEffects()
    {
        DamageEffectsPercentages.Clear();
        foreach (var r in DamageEffects.Keys)
        {
            var value = DamageEffects[r];
            if (value == 1)
                DamageEffectsPercentages.Add(r, 1.1f);
            else
                DamageEffectsPercentages.Add(r, (.01f * Mathf.Floor((float)(10f*(1f+value*0.415f))))+1);
        }
    }

    public void CalculateResistances()
    {
        ResistancePercentages.Clear();
        foreach (var r in Resistances.Keys)
        {
            var value = Resistances[r];
            if (value == 1)
                ResistancePercentages.Add(r, .9f);
            else
                ResistancePercentages.Add(r, 1 - (.01f * Mathf.Floor((float)(10f * (1f + value * 0.415f)))));
        }
    }

    public void AddResistance(Effects effect)
    {
        if (Resistances.ContainsKey(effect))
            Resistances[effect]++;
        else
            Resistances.Add(effect, 1);

        CalculateResistances();
    }

    public void RemoveResistance(Effects effect)
    {
        if (Resistances.ContainsKey(effect))
        {
            Resistances[effect]--;      
            if (Resistances[effect] == 0)
                Resistances.Remove(effect);
        }

        CalculateResistances();
    }

    public void AddDamageEffect(Effects effect)
    {
        if (DamageEffects.ContainsKey(effect))
            DamageEffects[effect]++;
        else
            DamageEffects.Add(effect, 1);

        CalculateDamageEffects();
    }

    public void RemoveDamageEffect(Effects effect)
    {
        if (DamageEffects.ContainsKey(effect))
        {
            DamageEffects[effect]--;
            if (DamageEffects[effect] == 0)
                DamageEffects.Remove(effect);
        }
        CalculateDamageEffects();
    }

    public int GetDamage()
    {
        return Strength;
    }

    public Dictionary<Effects, int> GetEffectDamage()
    {
        var damage = new Dictionary<Effects, int>();
        if (DamageEffectsPercentages.Count == 0)
            return damage;
        foreach(var e in DamageEffectsPercentages.Keys)
        {
            damage.Add(e, (int)Mathf.Floor(DamageEffectsPercentages[e] * Strength));
        }

        return damage;
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= (int)(damage * 1-( Armor/10f));
    }

    public void TakeDamage(int damage, Effects effect)
    {
        if (!Resistances.ContainsKey(effect))
            TakeDamage(damage);
        else
        {
            CurrentHealth -= ((int)Mathf.Ceil(ResistancePercentages[effect] * damage));
        }
    }    
}
