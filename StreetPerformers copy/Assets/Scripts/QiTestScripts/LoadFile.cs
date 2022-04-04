using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StreetPerformers;
using System;

[System.Serializable]
public class LoadFile
{
    private DateTime _lastPlayed;
    private int _totalHours;
    private int _totalMinutes;

    private Dictionary<string, int> _characterLevels;

    private Dictionary<string, int> _characterExp;

    public int _addedMaxHealth { get; set; }

    private List<string> _partyMembers;
    
    private List<string> _encounteredEnemies;

    private List<KeyValuePair<string, int>> _defeatedEnemies;
    private List<KeyValuePair<string, int>> _attemptedBattles;

    private bool _finishedTutorial = false;

    private List<string> _upgradedCards;

    private string _spawnArea = "Corner Store";

    /// <summary>
    /// this is just a plain constructor 
    /// </summary>
    public LoadFile()
    {
        _characterLevels = new Dictionary<string, int>();
        _characterExp = new Dictionary<string, int>();
        _partyMembers = new List<string>();
        _encounteredEnemies = new List<string>();
        _defeatedEnemies = new List<KeyValuePair<string, int>>();
        _attemptedBattles = new List<KeyValuePair<string, int>>();
        _finishedTutorial = false;
        _upgradedCards = new List<string>();
        _spawnArea = "Corner Store";
    }

    public List<KeyValuePair<string, int>> GetDefeatedEnemies()
    {
        if(_defeatedEnemies == null)
        { _defeatedEnemies = new List<KeyValuePair<string, int>>(); }

        return new List<KeyValuePair<string, int>>(_defeatedEnemies);
    }

    public void SetDefeatedEnemies(List<KeyValuePair<string, int>> defeatedEnemies)
    {
        _defeatedEnemies = new List<KeyValuePair<string, int>>(defeatedEnemies);
    }

    public List<KeyValuePair<string, int>> GetAttemptedBattles()
    {
        if(_attemptedBattles == null)
        { _attemptedBattles = new List<KeyValuePair<string, int>>(); }

        return new List<KeyValuePair<string, int>>(_attemptedBattles);
    }

    public void SetAttemptedBattles(List<KeyValuePair<string, int>> attemptedBattles)
    {
        _attemptedBattles = new List<KeyValuePair<string, int>>(attemptedBattles);
    }

    public List<string> GetEncounteredEnemies()
    {
        if (_encounteredEnemies == null)
        { _encounteredEnemies = new List<string>(); }

        return new List<string>(_encounteredEnemies);
    }
    public void SetEncounteredEnemies(List<string> enemies)
    {
        _encounteredEnemies = new List<string>(enemies);
    }

    public Dictionary<string, int> GetLevel()
    {
        if (_characterLevels == null)
        { _characterLevels = new Dictionary<string, int>(); }

        return new Dictionary<string, int>(_characterLevels);
    }

    public void SetLevel(Dictionary<string, int> level)
    {
        _characterLevels = new Dictionary<string, int>(level);
    }

    public Dictionary<string, int> GetExp()
    {
        if (_characterExp == null)
        { _characterExp = new Dictionary<string, int>(); }

        return new Dictionary<string, int>(_characterExp);
    }

    public void SetExp(Dictionary<string, int> exp)
    {
        _characterExp = new Dictionary<string, int>(exp);
    }

    public List<string> GetMembers()
    {
        if (_partyMembers == null)
        { _partyMembers = new List<string>(); }

        return new List<string>(_partyMembers);
    }

    public void SetMembers(List<string> members)
    {
        _partyMembers = new List<string>(members);
    }

    public TimeSpan GetPlayTime()
    {
        TimeSpan span = new TimeSpan(_totalHours, _totalMinutes, 0);
        return span;
    }

    public void SetPlayTime(DateTime now, DateTime startOfSession)
    {
        _lastPlayed = now;
        TimeSpan span = now - startOfSession;
        _totalHours += span.Hours;
        _totalMinutes += span.Minutes;
    }

    public DateTime GetLastPlayed()
    {
        return _lastPlayed;
    }

    public void SetFinishedTutorial(bool finished)
    {
        _finishedTutorial = finished;
    }

    public bool GetFinishedTutorial()
    {
        return _finishedTutorial;
    }

    public void SetUpgradedCards(List<string> upgradedCards)
    {
        _upgradedCards = new List<string>(upgradedCards);
    }

    public List<string> GetUpgradedCards()
    {
        if(_upgradedCards == null)
        { _upgradedCards = new List<string>(); }

        return new List<string>(_upgradedCards);
    }

    public void SetSpawnArea(string spawn)
    {
        _spawnArea = spawn;
    }

    public string GetSpawnArea()
    {
        if(_spawnArea == null)
        { _spawnArea = "Corner Store"; }

        return _spawnArea;
    }
}

