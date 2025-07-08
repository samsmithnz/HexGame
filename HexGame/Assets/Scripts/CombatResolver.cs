using System.Collections.Generic;
using UnityEngine;

public class CombatResolver
{
    private const int MAX_DICE = 10;
    private const int DIE_SIDES = 6;
    
    public struct CombatResult
    {
        public int attackerSurvivors;
        public int defenderSurvivors;
        public bool attackerWins;
        public List<int> attackerRolls;
        public List<int> defenderRolls;
    }
    
    public static CombatResult ResolveCombat(int attackerArmies, int defenderArmies)
    {
        // Cap dice to reasonable maximum
        int attackerDice = Mathf.Min(attackerArmies, MAX_DICE);
        int defenderDice = Mathf.Min(defenderArmies, MAX_DICE);
        
        // Roll dice for both sides
        List<int> attackerRolls = RollDice(attackerDice);
        List<int> defenderRolls = RollDice(defenderDice);
        
        // Sort rolls in descending order
        attackerRolls.Sort((a, b) => b.CompareTo(a));
        defenderRolls.Sort((a, b) => b.CompareTo(a));
        
        // Compare dice and calculate losses
        int attackerLosses = 0;
        int defenderLosses = 0;
        
        int battles = Mathf.Min(attackerRolls.Count, defenderRolls.Count);
        for (int i = 0; i < battles; i++)
        {
            if (attackerRolls[i] > defenderRolls[i])
            {
                defenderLosses++;
            }
            else
            {
                attackerLosses++;
            }
        }
        
        // Handle extra dice (unpaired)
        if (attackerRolls.Count > defenderRolls.Count)
        {
            defenderLosses += attackerRolls.Count - defenderRolls.Count;
        }
        else if (defenderRolls.Count > attackerRolls.Count)
        {
            attackerLosses += defenderRolls.Count - attackerRolls.Count;
        }
        
        // Calculate survivors
        int attackerSurvivors = attackerArmies - attackerLosses;
        int defenderSurvivors = defenderArmies - defenderLosses;
        
        // Ensure no negative survivors
        attackerSurvivors = Mathf.Max(0, attackerSurvivors);
        defenderSurvivors = Mathf.Max(0, defenderSurvivors);
        
        return new CombatResult
        {
            attackerSurvivors = attackerSurvivors,
            defenderSurvivors = defenderSurvivors,
            attackerWins = attackerSurvivors > defenderSurvivors,
            attackerRolls = attackerRolls,
            defenderRolls = defenderRolls
        };
    }
    
    private static List<int> RollDice(int count)
    {
        List<int> rolls = new List<int>();
        for (int i = 0; i < count; i++)
        {
            rolls.Add(Random.Range(1, DIE_SIDES + 1));
        }
        return rolls;
    }
}