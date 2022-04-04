using StreetPerformers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlossaryCharacter : MonoBehaviour
{
    [Header("Home Page")]
    [SerializeField] private GameObject _enemyHeader = null;
    [SerializeField] private List<GameObject> _layoutGroupObjects = null;

    [Header("Individual Character Page")]
    [SerializeField] private TextMeshProUGUI _nameText = null;
    [SerializeField] private TextMeshProUGUI _levelText = null;
    [SerializeField] private GameObject _nextLevelButton = null;
    [SerializeField] private GameObject _prevLevelButton = null;
    [SerializeField] private GameObject _nextCharacterButton = null;
    [SerializeField] private GameObject _prevCharacterButton = null;
    [SerializeField] private Image _charImage = null;
    [SerializeField] private GameObject _switchButtonObj = null;
    [SerializeField] private TextMeshProUGUI _switchButtonText = null;

    [SerializeField] private GameObject _homePage = null;
    [SerializeField] private GameObject _characterPage = null;
    [SerializeField] private GameObject _lorePage = null;
    [SerializeField] private GameObject _cardPage = null;

    [SerializeField] private GameObject _card4;
    [SerializeField] private GameObject _card7;
    [SerializeField] private GameObject _card9;
    [SerializeField] private GameObject _card13;

    [SerializeField] private GameObject _displayCardPanel = null;
    [SerializeField] private DisplayCard _displayCard = null;
    [SerializeField] private Button _nextCard = null;
    [SerializeField] private Button _prevCard = null;

    [SerializeField] private List<Sprite> _characterImages = null;
    [SerializeField] private List<Sprite> _clownImages = null;
    [SerializeField] private List<Sprite> _singerImages = null;
    [SerializeField] private List<Sprite> _musicianImages = null;

    [SerializeField] private List<TextAsset> _loreTextList = null;
    [SerializeField] private TextMeshProUGUI _loreText = null;

    private string _currentCharacter = "";
    private int _currentIndex = 0;
    private int _currentLevel = 1;

    private List<string> _characterOrder = new List<string>
    {
        "Ace",
        "Mascot",
        "Contortionist",
        "Mime",
        "Musician",
        "Singer",
        "Clown",
        "Cop",
        "Wild Guy",
        "Statue",
        "Magician"
    };

    private Dictionary<string, GameObject> _characterCards = null;

    private Dictionary<string, List<int>> _characterLevels = new Dictionary<string, List<int>>();

    private CardScriptable[] _cards;
    private List<CardScriptable> _extraCards = new List<CardScriptable>();
    private int _displayCardIndex = 0;

    private void Awake()
    {
        AddPartyLevel("Ace");
        AddPartyLevel("Mascot");
        AddPartyLevel("Contortionist");
        AddPartyLevel("Mime");

        foreach(HubAreaData hubArea in ProgressionHandler.Instance._hubAreas.Values)
        {
            if(!hubArea._active)
            { continue; }

            foreach(BattleData battle in hubArea._battles)
            {
                if(!battle._attempted)
                { continue; }

                foreach(EnemyStruct enemy in battle._enemyList)
                {
                    AddEnemyLevel(enemy);
                }
            }
        }

        foreach(string enemyName in _characterLevels.Keys)
        {
            _characterLevels[enemyName].Sort();
        }

        _characterCards = new Dictionary<string, GameObject>
        {
            { "Ace", _card13 },
            { "Mascot", _card13 },
            { "Contortionist", _card13 },
            { "Mime", _card13 },
            { "Musician", _card4 },
            { "Singer", _card4 },
            { "Clown", _card4 },
            { "Cop", _card9 },
            { "Wild Guy", _card7 },
            { "Statue", _card7 },
            { "Magician", _card9 },
        };

        Initialize("Ace", true);

        //Disable character buttons of characters that haven't appeared yet
        foreach(GameObject layoutGroupObj in _layoutGroupObjects)
        {
            for(int i = 0; i < layoutGroupObj.transform.childCount; i++)
            {
                GameObject charButton = layoutGroupObj.transform.GetChild(i).gameObject;
                if(!_characterLevels.ContainsKey(charButton.name))
                {
                    charButton.SetActive(false);
                }
            }
        }
        if(_characterLevels.Count <= 4)
        {
            _enemyHeader.SetActive(false);
        }
    }

    private void AddPartyLevel(string partyName)
    {
        if(!PartyHandler.Instance._characterLevels.ContainsKey(partyName))
        { return; }

        _characterLevels.Add(partyName, new List<int>());

        int level = PartyHandler.Instance._characterLevels[partyName];
        _characterLevels[partyName].Add(level);
    }

    private void AddEnemyLevel(EnemyStruct enemy)
    {
        string enemyName = enemy._enemyName;
        if(enemyName == "Finale")
        {
            enemyName = "Magician";
        }

        if(enemyName == "Mascot" || enemyName == "Mime" || enemyName == "Contortionist")
        { return; }

        int enemyLevel = enemy._enemyLevel;

        if(!_characterLevels.ContainsKey(enemyName))
        {
            _characterLevels.Add(enemyName, new List<int>());
        }

        if(!_characterLevels[enemyName].Contains(enemyLevel))
        {
            _characterLevels[enemyName].Add(enemyLevel);
        }
    }

    public void Initialize(string character, bool fromMainPage = false)
    {
        _currentCharacter = character;

        if(fromMainPage)
        {
            _lorePage.SetActive(true);
            _cardPage.SetActive(false);
            _switchButtonText.text = "View Cards";
        }

        _nameText.text = character;
        _currentIndex = _characterOrder.IndexOf(_currentCharacter);

        if(GetNextAvailableCharacter() >= 0)
        {
            _nextCharacterButton.SetActive(true);
        }
        else
        {
            _nextCharacterButton.SetActive(false);
        }

        if(GetPrevAvailableCharacter() >= 0)
        {
            _prevCharacterButton.SetActive(true);
        }
        else
        {
            _prevCharacterButton.SetActive(false);
        }

        List<int> levels = _characterLevels[_currentCharacter];
        _currentLevel = levels[levels.Count - 1];

        LevelChanged();

        switch(_currentCharacter)
        {
            case "Clown":
                _charImage.sprite = _clownImages[levels.Count - 1];
                break;
            case "Singer":
                _charImage.sprite = _singerImages[levels.Count - 1];
                break;
            case "Musician":
                _charImage.sprite = _musicianImages[levels.Count - 1];
                break;
            default:
                _charImage.sprite = _characterImages[_currentIndex];
                break;
        }
        
        _charImage.SetNativeSize();

        SetLore(_currentIndex);
    }

    private void InitializeCards(string character, int level)
    {
        _card4.SetActive(false);
        _card7.SetActive(false);
        _card9.SetActive(false);
        _card13.SetActive(false);

        _displayCardPanel.SetActive(false);

        _characterCards[character].SetActive(true);

        _cards = null;
        _extraCards = new List<CardScriptable>();
        int extraCardTotalCount = 0;

        switch(character)
        {
            case "Ace":
            case "Mascot":
            case "Contortionist":
            case "Mime":
                _cards = Resources.LoadAll<CardScriptable>("ScriptableObjects/" + character + "/Party");
                for (int j = 0; j < _cards.Length; j++)
                {
                    CardScriptable scriptable = _cards[j];
                    if (PartyHandler.Instance._upgradedCards.Contains(scriptable._saveId))
                    {
                        _cards[j] = (CardScriptable)Resources.Load("ScriptableObjects/" + character + "/Upgraded/" + scriptable._id);
                    }
                    Array.Sort(_cards, CompareCardStrings);
                }
                break;
            case "Musician":
            case "Singer":
            case "Clown":
            case "Wild Guy":
            case "Statue":
            case "Magician":
                _cards = Resources.LoadAll<CardScriptable>("ScriptableObjects/" + character + "/Level" + level);
                break;
            case "Cop":
                _cards = Resources.LoadAll<CardScriptable>("ScriptableObjects/" + character + "/Level" + level);
                _extraCards.Add(Resources.Load<CardScriptable>("ScriptableObjects/" + character + "/Ace-Citation" + level));
                if(level >= 4)
                {
                    _extraCards.Add(Resources.Load<CardScriptable>("ScriptableObjects/" + character + "/Ace-Ticket" + level));
                }
                extraCardTotalCount = 2;
                break;
        }

        int i = 0;
        for(;  i < _cards.Length; i++)
        {
            GameObject card = _characterCards[character].transform.GetChild(i).gameObject;
            if(_cards[i]._requiredLevel <= level)
            {
                card.GetComponent<DisplayCard>().Initialize(_cards[i]);
                card.GetComponent<Button>().enabled = true;
                card.transform.Find("Background").gameObject.SetActive(false);
            }
            else
            {
                card.GetComponent<DisplayCard>().FlipToBack(character);
                card.GetComponent<Button>().enabled = false;
                card.transform.Find("Background").gameObject.SetActive(false);
            }
        }

        
        int endCards = _characterCards[character].transform.childCount;
        if(_extraCards.Count > 0)
        {
            endCards -= extraCardTotalCount;
        }

        for(; i < endCards; i++)
        {
            GameObject card = _characterCards[character].transform.GetChild(i).gameObject;
            card.GetComponent<DisplayCard>().FlipToBack(character);
            card.GetComponent<Button>().enabled = false;
            card.transform.Find("Background").gameObject.SetActive(false);
        }

        if(_extraCards.Count > 0)
        {
            for(; i < endCards + extraCardTotalCount; i++)
            {
                GameObject card = _characterCards[character].transform.GetChild(i).gameObject;
                int extraIndex = i - endCards;
                if(_extraCards.Count >= extraIndex + 1)
                {
                    card.GetComponent<DisplayCard>().Initialize(_extraCards[extraIndex]);
                    card.GetComponent<Button>().enabled = true;
                    card.transform.Find("Background").gameObject.SetActive(true);
                }
                else
                {
                    card.GetComponent<DisplayCard>().FlipToBack(character);
                    card.GetComponent<Button>().enabled = false;
                    card.transform.Find("Background").gameObject.SetActive(false);
                }
            }
        }
    }

    private void SetLore(int characterIndex)
    {
        TextAsset loreText = _loreTextList[characterIndex];

        string[] lines = loreText.text.Split("\n"[0]);
        string loreString = "";

        foreach(string line in lines)
        {
            if(line.StartsWith("["))
            {
                int currentLoreLevel = int.Parse(line.Substring(1, line.Length - 3));

                if(currentLoreLevel > _currentLevel)
                {
                    loreString += "<i><color=#BBB>Reach level " + currentLoreLevel + " to learn more.</color></i>";
                    break;
                }

                continue;
            }
            loreString += line + "\n";
        }

        _loreText.text = loreString;
    }

    private int CompareCardStrings(CardScriptable card1, CardScriptable card2)
    {
        string id1 = card1._id.Split('-')[1];
        string id2 = card2._id.Split('-')[1];
        if(id1.Equals("Ace"))
        {
            return 1;
        }
        else if(id2.Equals("Ace"))
        {
            return -1;
        }
        else
        {
            int id1Int = int.Parse(id1);
            int id2Int = int.Parse(id2);
            if(id1Int <= id2Int)
            {
                return -1;
            }
            else
            {
                return 11;
            }
        }
    }

    public void OnButtonClicked(string buttonId)
    {
        switch(buttonId)
        {
            case "Switch":
                if(_lorePage.activeInHierarchy)
                {
                    _lorePage.SetActive(false);
                    _cardPage.SetActive(true);
                    _displayCardPanel.SetActive(false);
                    _switchButtonText.text = "View Lore";
                }
                else
                {
                    _lorePage.SetActive(true);
                    _cardPage.SetActive(false);
                    _switchButtonText.text = "View Cards";
                }
                break;
            case "Return":
                _homePage.SetActive(true);
                _characterPage.SetActive(false);
                break;
            case "NextLevel":
                ChangeLevel(1);
                break;
            case "PrevLevel":
                ChangeLevel(-1);
                break;
            case "NextCharacter":
                if(_currentIndex >= _characterOrder.Count - 1)
                { return; }

                _currentIndex = GetNextAvailableCharacter();
                Initialize(_characterOrder[_currentIndex], false);
                break;
            case "PrevCharacter":
                if(_currentIndex <= 0)
                { return; }

                _currentIndex = GetPrevAvailableCharacter();
                Initialize(_characterOrder[_currentIndex], false);
                break;
            case "NextCard":
                OnCardButtonPressed(_displayCardIndex + 1, 1);
                break;
            case "PrevCard":
                OnCardButtonPressed(_displayCardIndex - 1, -1);
                break;
            case "ExitDisplay":
                _displayCardPanel.SetActive(false);
                break;
        }
    }

    private void ChangeLevel(int direction)
    {
        List<int> levels = _characterLevels[_currentCharacter];
        int curIndex = levels.IndexOf(_currentLevel);

        if(direction > 0 && curIndex >= levels.Count - 1)
        {
            return;
        }

        if(direction < 0 && curIndex <= 0)
        {
            return;
        }

        curIndex += direction;

        _currentLevel = levels[curIndex];
        LevelChanged();
    }

    public void OnCharacterButtonPressed(string character)
    {
        _homePage.SetActive(false);
        _characterPage.SetActive(true);
        Initialize(character, false);
    }

    private void LevelChanged()
    {
        _levelText.text = "" + _currentLevel;

        int curIndex = _characterLevels[_currentCharacter].IndexOf(_currentLevel);
        if(curIndex >= _characterLevels[_currentCharacter].Count - 1)
        {
            _nextLevelButton.SetActive(false);
        }
        else
        {
            _nextLevelButton.SetActive(true);
        }

        if(curIndex <= 0)
        {
            _prevLevelButton.SetActive(false);
        }
        else
        {
            _prevLevelButton.SetActive(true);
        }

        InitializeCards(_currentCharacter, _currentLevel);

        switch(_currentCharacter)
        {
            case "Clown":
                _charImage.sprite = _clownImages[curIndex];
                break;
            case "Singer":
                _charImage.sprite = _singerImages[curIndex];
                break;
            case "Musician":
                _charImage.sprite = _musicianImages[curIndex];
                break;
            default:
                break;
        }
        _charImage.SetNativeSize();
    }

    private int GetNextAvailableCharacter()
    {
        if(_currentIndex == _characterOrder.Count - 1)
        {
            return -1;
        }

        for(int i = _currentIndex + 1; i < _characterOrder.Count; i++)
        {
            string character = _characterOrder[i];
            if(_characterLevels.ContainsKey(character))
            {
                return i;
            }
        }

        return -1;
    }

    private int GetPrevAvailableCharacter()
    {
        if(_currentIndex == 0)
        {
            return -1;
        }

        for(int i = _currentIndex - 1; i >= 0; i--)
        {
            string character = _characterOrder[i];
            if(_characterLevels.ContainsKey(character))
            {
                return i;
            }
        }

        return -1;
    }

    public void OnCardButtonPressed(int cardIndex)
    {
        OnCardButtonPressed(cardIndex, 0);
    }

    public void OnCardButtonPressed(int cardIndex, int direction)
    {
        _displayCardIndex = cardIndex;

        Transform characterCards = _characterCards[_currentCharacter].transform;
        while(direction != 0 && !characterCards.GetChild(_displayCardIndex).GetComponent<DisplayCard>()._visible)
        {
            _displayCardIndex += direction;
        }

        if(_displayCardIndex <= 0)
        {
            _prevCard.interactable = false;
        }
        else
        {
            _prevCard.interactable = true;
        }

        bool extraCardAvailable = _extraCards.Count > 0;
        int extraCardIndex = 0;
        if(extraCardAvailable)
        {
            extraCardIndex = _displayCardIndex - 7;
            //TODO: IF ANY OTHER CHARACTERS HAVE EXTRA CARDS, MUST HANDLE THAT HERE
            if(extraCardIndex >= _extraCards.Count - 1)
            {
                extraCardAvailable = false;
            }
        }

        if(!extraCardAvailable && (_displayCardIndex >= _cards.Length - 1 || _cards[_displayCardIndex + 1]._requiredLevel > _currentLevel))
        {
            _nextCard.interactable = false;
        }
        else
        {
            _nextCard.interactable = true;
        }

        _displayCardPanel.SetActive(true);

        if(_displayCardIndex >= _cards.Length)
        {
            _displayCard.Initialize(_extraCards[extraCardIndex]);
        }
        else
        {
            _displayCard.Initialize(_cards[_displayCardIndex]);
        }
    }
}
