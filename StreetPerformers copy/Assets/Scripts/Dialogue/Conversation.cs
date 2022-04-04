using System.Collections.Generic;
using UnityEngine;

namespace StreetPerformers
{
    public class Conversation : MonoBehaviour
    {
        public Participant[] _participants;
        public TextAsset textAsset;

        private List<string> dialogueList = new List<string>();
        private int _currentLineIndex = 0;
        private DialogueManager dialogueController;

        private Participant _curParticipant;

        private int _lineCount = 0;

        private bool _enemySpeaking = false;

        private bool _finished = false;

        public void StartConvo(DialogueManager dialogue)
        {
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("InDialogue", 0);

            _lineCount = 0;
            _currentLineIndex = 0;
            var lines = textAsset.text.Split("\n"[0]);
            
            foreach (var obj in lines)
            {
                _lineCount++;
                dialogueList.Add(obj);
            }
            dialogueController = dialogue;
            dialogueController.BeginConvo(this);
            readLine(_currentLineIndex);
        }

        public void Next()
        {
            if(_finished)
            { return; }

            if (_currentLineIndex >= _lineCount - 1)
            {
                dialogueController.EndConvo();
                _finished = true;
            }
            else
            {
                readLine(_currentLineIndex);
            }
        }

        private void ReadLine()
        {
            readLine(_currentLineIndex);
        }

        void readLine(int lineNum)
        {
            if(_finished)
            { return; }

            if (dialogueList[lineNum].Contains("["))
            {
                string[] newName = dialogueList[lineNum].Split('[', ']');
                if(newName[1].Contains("#"))
                {
                    _enemySpeaking = true;
                }
                else
                {
                    _enemySpeaking = false;
                }

                bool validParticipant = false;
                foreach (Participant person in _participants)
                {
                    if (newName[1].Contains(person.characterName))
                    {
                        _curParticipant = person;
                        string spriteId = "Default";
                        string charName = person.characterName;

                        if(newName[1].Contains("-"))
                        {
                            newName = newName[1].Split('-');
                            if(newName[1].Contains("("))
                            {
                                charName = newName[1].Split('(', ')')[1];
                                spriteId = newName[2];
                            }
                            else
                            {
                                spriteId = newName[1];
                            }
                        }
                        
                        else if(newName[1].Contains("("))
                        {
                            charName = newName[1].Split('(', ')')[1];
                        }
                        dialogueController.SwitchName(person.characterName, charName, person.GetSprite(spriteId), _enemySpeaking);
                        validParticipant = true;
                        break;
                    }
                }
                if(!validParticipant)
                {
                    dialogueController.SwitchName("", "", null, _enemySpeaking);
                    dialogueController.NextLine("", _curParticipant._textSpeed);
                    _currentLineIndex = _currentLineIndex + 1;
                    CancelInvoke("ReadLine");
                    Invoke("ReadLine", 1f);
                }
                else
                {
                    CancelInvoke("ReadLine");
                    _currentLineIndex = _currentLineIndex + 1;
                    readLine(_currentLineIndex);
                }
            }
            else
            {
                Showline(_currentLineIndex);
            }
        }

        void Showline(int lineToShow)
        {
            //print(dialogueList[lineToShow].ToString());
            dialogueController.NextLine(dialogueList[lineToShow], _curParticipant._textSpeed);
            _currentLineIndex = _currentLineIndex + 1;
        }
    }
}