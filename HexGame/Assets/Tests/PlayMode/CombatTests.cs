using System.Collections.Generic;
using NUnit.Framework;

public class CombatTests
{
    [Test]
    public void SortedRollsDefenderTiesAndExtraDiceDetermineSurvivors()
    {
        Queue<int> dice = new Queue<int>(new[] { 2, 6, 3, 6, 4 });
        CombatResolver.CombatResult result = CombatResolver.ResolveCombat(3, 2, () => dice.Dequeue());

        CollectionAssert.AreEqual(new[] { 6, 3, 2 }, result.attackerRolls);
        CollectionAssert.AreEqual(new[] { 6, 4 }, result.defenderRolls);
        Assert.AreEqual(1, result.attackerSurvivors);
        Assert.AreEqual(1, result.defenderSurvivors);
        Assert.IsFalse(result.attackerWins);
    }

    [Test]
    public void DefenderExtraDiceCauseAttackerLosses()
    {
        Queue<int> dice = new Queue<int>(new[] { 6, 1, 2, 3 });
        CombatResolver.CombatResult result = CombatResolver.ResolveCombat(1, 3, () => dice.Dequeue());

        Assert.AreEqual(0, result.attackerSurvivors);
        Assert.AreEqual(2, result.defenderSurvivors);
        Assert.IsFalse(result.attackerWins);
    }

    [Test]
    public void NeutralCaptureAndDiceCap()
    {
        CombatResolver.CombatResult neutral = CombatResolver.ResolveCombat(3, 0, () => 1);
        Assert.IsTrue(neutral.attackerWins);
        Assert.AreEqual(3, neutral.attackerSurvivors);
        Assert.AreEqual(0, neutral.defenderSurvivors);

        CombatResolver.CombatResult capped = CombatResolver.ResolveCombat(12, 12, () => 6);
        Assert.AreEqual(10, capped.attackerRolls.Count);
        Assert.AreEqual(10, capped.defenderRolls.Count);
        Assert.AreEqual(2, capped.attackerSurvivors);
        Assert.AreEqual(12, capped.defenderSurvivors);
    }
}
