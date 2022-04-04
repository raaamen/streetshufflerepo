using System;
using System.Collections.Generic;
using UnityEngine;

namespace StreetPerformers
{
    /// <summary>
    /// Participant class for dialogues. Holds a name, animator, and list of talk sprites for the participant.
    /// </summary>
    public class Participant : MonoBehaviour
    {
        //Struct containing a talk sprite and its string id
        [Serializable]
        public struct ParticipantSprites
        {
            public string _spriteId;
            public Sprite _sprite;
        }

        //Name of the participant as shown in dialogues
        public string characterName;
        //Animator for the partipant (currently unused)
        public GameObject characterAnim;
        //List of talk sprites and their id for this participant
        public List<ParticipantSprites> _talkSprites;

        [Tooltip("Number represents letters per second that their text types out")]
        public float _textSpeed = 40f;

        /// <summary>
        /// Gets a talk sprite associated with the given id. Returns the default talk sprite of the
        /// participant if no others are found.
        /// </summary>
        /// <param name="id"></param> String id of the talk sprite
        /// <returns></returns>
        public Sprite GetSprite(string id)
        {
            Sprite defaultSprite = _talkSprites[0]._sprite;
            foreach(ParticipantSprites spriteStruct in _talkSprites)
            {
                if(spriteStruct._spriteId == id)
                {
                    return spriteStruct._sprite;
                }
                else if(spriteStruct._spriteId == "Default")
                {
                    defaultSprite = spriteStruct._sprite;
                }
            }
            return defaultSprite;
        }
    }
}

