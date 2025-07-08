using UnityEngine;

public class CombatTest : MonoBehaviour
{
    void Start()
    {
        TestCombatResolution();
    }
    
    void TestCombatResolution()
    {
        Debug.Log("=== Testing Combat Resolution ===");
        
        // Test 1: Equal armies
        CombatResolver.CombatResult result1 = CombatResolver.ResolveCombat(3, 3);
        Debug.Log($"Test 1 (3v3): Attacker {result1.attackerSurvivors}, Defender {result1.defenderSurvivors}, Winner: {(result1.attackerWins ? "Attacker" : "Defender")}");
        
        // Test 2: More attackers
        CombatResolver.CombatResult result2 = CombatResolver.ResolveCombat(5, 2);
        Debug.Log($"Test 2 (5v2): Attacker {result2.attackerSurvivors}, Defender {result2.defenderSurvivors}, Winner: {(result2.attackerWins ? "Attacker" : "Defender")}");
        
        // Test 3: More defenders
        CombatResolver.CombatResult result3 = CombatResolver.ResolveCombat(2, 5);
        Debug.Log($"Test 3 (2v5): Attacker {result3.attackerSurvivors}, Defender {result3.defenderSurvivors}, Winner: {(result3.attackerWins ? "Attacker" : "Defender")}");
        
        // Test 4: Attack empty tile
        CombatResolver.CombatResult result4 = CombatResolver.ResolveCombat(3, 0);
        Debug.Log($"Test 4 (3v0): Attacker {result4.attackerSurvivors}, Defender {result4.defenderSurvivors}, Winner: {(result4.attackerWins ? "Attacker" : "Defender")}");
        
        // Test 5: Large army test (should cap at MAX_DICE = 10)
        CombatResolver.CombatResult result5 = CombatResolver.ResolveCombat(15, 12);
        Debug.Log($"Test 5 (15v12): Attacker {result5.attackerSurvivors}, Defender {result5.defenderSurvivors}, Winner: {(result5.attackerWins ? "Attacker" : "Defender")}");
        
        Debug.Log("=== Combat Resolution Tests Complete ===");
    }
}