using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class GameplayTests
{
    private GameManager game;
    private HexSelectionManager selection;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        yield return SceneManager.LoadSceneAsync("Main");
        yield return null;
        yield return null;
        game = GameManager.Instance;
        selection = HexSelectionManager.Instance;
        Assert.IsNotNull(game);
        Assert.IsNotNull(selection);
    }

    private HexTile Tile(int x, int y)
    {
        return game.GetAllTiles().Find(tile => tile.name == $"HexTile_{x}_{y}");
    }

    private object Invoke(object target, string name, params object[] arguments)
    {
        MethodInfo method = target.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(method);
        return method.Invoke(target, arguments);
    }

    private void ClearBoard()
    {
        foreach (HexTile tile in game.GetAllTiles())
        {
            tile.SetHexColor(HexColor.None, 0);
        }
    }

    [Test]
    public void InitializesOneSelectionManagerAndStartingArmies()
    {
        Assert.AreEqual(1, Object.FindObjectsByType<HexSelectionManager>(FindObjectsSortMode.None).Length);
        Assert.AreEqual(50, game.GetAllTiles().Count);
        Assert.AreEqual(3, game.GetAllTiles().FindAll(tile => tile.hexColor == HexColor.Blue && tile.armyCount == 3).Count);
        Assert.AreEqual(3, game.GetAllTiles().FindAll(tile => tile.hexColor == HexColor.Green && tile.armyCount == 3).Count);
        Assert.AreEqual(HexColor.Blue, game.currentPlayer);
        Assert.AreEqual(1, game.turn);
        Assert.IsFalse(game.gameEnded);
    }

    [TestCase(2, 2, new[] { "HexTile_3_2", "HexTile_1_2", "HexTile_2_3", "HexTile_2_1", "HexTile_3_1", "HexTile_1_1" })]
    [TestCase(3, 2, new[] { "HexTile_4_2", "HexTile_2_2", "HexTile_3_3", "HexTile_3_1", "HexTile_4_3", "HexTile_2_3" })]
    public void NeighborsMatchGridForBothColumnParities(int x, int y, string[] expected)
    {
        List<HexTile> neighbors = (List<HexTile>)Invoke(selection, "GetNeighbors", Tile(x, y));
        CollectionAssert.AreEquivalent(expected, neighbors.ConvertAll(tile => tile.name));
        foreach (HexTile neighbor in neighbors)
        {
            List<HexTile> reverse = (List<HexTile>)Invoke(selection, "GetNeighbors", neighbor);
            CollectionAssert.Contains(reverse, Tile(x, y));
        }
    }

    [Test]
    public void TurnChangesAwardOnlyTheActivePlayerAndRespectCap()
    {
        ClearBoard();
        Tile(0, 0).SetHexColor(HexColor.Blue, 3);
        Tile(9, 4).SetHexColor(HexColor.Green, 6);
        Tile(9, 3).SetHexColor(HexColor.Green, 7);
        game.NextTurn();
        Assert.AreEqual(HexColor.Green, game.currentPlayer);
        Assert.AreEqual(1, game.turn);
        Assert.AreEqual(3, Tile(0, 0).armyCount);
        Assert.AreEqual(7, Tile(9, 4).armyCount);
        Assert.AreEqual(7, Tile(9, 3).armyCount);
        game.NextTurn();
        Assert.AreEqual(HexColor.Blue, game.currentPlayer);
        Assert.AreEqual(2, game.turn);
        Assert.AreEqual(4, Tile(0, 0).armyCount);
        Assert.IsFalse(game.gameEnded);
    }

    [Test]
    public void OpponentWithTilesAndArmiesPreventsVictory()
    {
        ClearBoard();
        Tile(0, 0).SetHexColor(HexColor.Blue, 1);
        Tile(9, 4).SetHexColor(HexColor.Green, 1);
        game.CheckWinCondition();
        Assert.IsFalse(game.gameEnded);
    }

    [TestCase(HexColor.Blue, HexColor.Green)]
    [TestCase(HexColor.Green, HexColor.Blue)]
    public void EliminatingOpponentWinsWithNeutralTiles(HexColor winner, HexColor loser)
    {
        ClearBoard();
        Tile(0, 0).SetHexColor(winner, 3);
        game.CheckWinCondition();
        Assert.IsTrue(game.gameEnded);
        Assert.AreEqual($"Player {winner} wins!", GameObject.Find("WinText").GetComponent<Text>().text);
        Assert.AreEqual(HexColor.None, Tile(1, 1).hexColor);
        Assert.AreEqual(0, game.GetAllTiles().FindAll(tile => tile.hexColor == loser).Count);
    }

    [Test]
    public void OpponentWithNoArmiesIsEliminated()
    {
        ClearBoard();
        Tile(0, 0).SetHexColor(HexColor.Blue, 1);
        Tile(9, 4).SetHexColor(HexColor.Green, 0);
        game.CheckWinCondition();
        Assert.IsTrue(game.gameEnded);
        Assert.AreEqual("Player Blue wins!", GameObject.Find("WinText").GetComponent<Text>().text);
    }

    [Test]
    public void NeutralCaptureVictoryAndRestartClearSelection()
    {
        ClearBoard();
        HexTile source = Tile(2, 2);
        HexTile neutral = Tile(2, 3);
        HexTile opponent = Tile(9, 4);
        source.SetHexColor(HexColor.Blue, 7);
        opponent.SetHexColor(HexColor.Green, 1);
        Invoke(selection, "HandleTileClick", source);
        Invoke(selection, "HandleTileClick", neutral);
        Assert.AreEqual(HexColor.Blue, neutral.hexColor);
        Assert.AreEqual(6, neutral.armyCount);
        Assert.AreEqual(1, source.armyCount);
        Assert.IsFalse(game.gameEnded);

        HexTile lastOpponent = Tile(2, 4);
        opponent.SetHexColor(HexColor.None, 0);
        lastOpponent.SetHexColor(HexColor.Green, 1);
        neutral.SetHexColor(HexColor.Blue, 7);
        Invoke(selection, "HandleTileClick", neutral);
        Invoke(selection, "HandleTileClick", lastOpponent);
        Assert.IsTrue(game.gameEnded);
        Assert.AreEqual(HexColor.Blue, lastOpponent.hexColor);
        game.NextTurn();
        Assert.AreEqual(HexColor.Blue, game.currentPlayer);
        GameObject.Find("PlayAgainButton").GetComponent<Button>().onClick.Invoke();
        Assert.IsFalse(game.gameEnded);
        Assert.AreEqual(HexColor.Blue, game.currentPlayer);
        Assert.AreEqual(1, game.turn);
        FieldInfo field = typeof(HexSelectionManager).GetField("selectedTile", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNull(field.GetValue(selection));
        Assert.AreEqual(3, game.GetAllTiles().FindAll(tile => tile.hexColor == HexColor.Blue && tile.armyCount == 3).Count);
        Assert.AreEqual(3, game.GetAllTiles().FindAll(tile => tile.hexColor == HexColor.Green && tile.armyCount == 3).Count);
    }
}
