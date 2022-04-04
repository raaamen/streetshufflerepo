using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameAudio", menuName = "ScriptableObjects/GameAudio")]

public class GameAudio : SingletonScriptableObject<GameAudio>
{
    //Music Events
    public string Battle_Music; //General Battle Music
    public string Map_Music; //Music for Map duh
    public string Victory_Jingle; //Music for when the player wins against their opponent
    public string Defeat_Jingle; //Music for when the player is defeated

    [Space(20)]

    //Combat SFX
    public string Card_Hit; //Played an Attack card
    public string Card_Armor; //Played an Armor Card
    public string Card_Special; //Played a Special Card
    public string Card_Pickup; //Card is Clicked and enlargens
    public string Card_Retract; //Card is put back in player's hand
    public string Player_Death; //Player's health is zero
    public string Player_Health_Drop; //Player's health goes down

    [Space(20)]

    //UI SFX
    public string Button_Forward; //General UI Sound
    public string Button_Backward; //Used when the player goes back in menus or closes out of a menu
    public string Level_Select; //Player Selects level or New Game
    public string Card_History_Popup; //Card History Menu drops down
    public string Card_Histroy_Popout; //Card Histroy Menu goes away
    public string End_Turn_Button; //Player clicks end turn button
    public string Menu_Popup; //General Menu pop up
    public string Menu_Popout; //When the menu goes away
    public string Advance_Text; //When the player advances text during dialogue
}
