using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StreetPerformers
{
    /// <Script Explanation>
    //Unity Set Up

    //Here is a script you could use to control your volume settings for your Master, Music, and Sound Effects levels using FMOD's Buses and Unity's Sliders.
    //In Unity Attach this script to an Audio Manager Game Object.
    //Make three Sliders these will be used to control you Maset, Music, and SFX Volume Levels later on.
    //Set each sliders value to be between 0 and 1.
    //Attach the Audio Manager Gameobject to the SLider's 'On Value Change (single)' event.
    //Where it says 'no funtion' click the drop down and access the Audio Settings Script.
    //Designate the *Dynamic Float* values to each slider respectively, don't use the functions only use the dynamic floats.


    //FMOD Session Set Up

    //Pluging this script in is not enough however. There is a bit of set up you will be required to do in you FMOD session so it knows what events are being effected by each slider.
    //In FMOD if you go to Window > Mixer, it should open another tab with all your busses (most likely just the master) and events you have in your session. Here you can create new groups to be controlled.
    //Right click on your Master Bus and create a new group. I would make two groups, one for music and one for sound effects. These should both be parented under the Master Bus but kept separate from one another other.
    //Make sure these are named cap specific, 'Music' and 'SFX' so the pathing being set up in this script doesn't get confused.

    //From here now you have your buses set up to be controlled but you'll notice nothing happens if you change your sliders in game.
    //This is due to the fact that your audio events are not put under any bus but the master.
    //Go through the FMOD Events you have and designate them to the specific bus you want them to be controlled under ie. if you have a 'Background Music Event' put that under the Music Bus.
    //Once these events are set into their correct buses and everything is Built in FMOD, you should be able to control your volumes in game via a settings menu.

    //Project Tips

    //Be sure to have some way to store your float values since you want these consistent throughout the player's playthrough. FMOD will keep your volume levels consistent between scenes regardless but Unitywill not unless you store those values somewhere.
    /// </Script Explanation>

    public class AudioSettings : MonoBehaviour
    {
        //Event Variable to be played later
        FMOD.Studio.EventInstance SFXVolumeTest;

        //Bus Variables
        FMOD.Studio.Bus Master;
        FMOD.Studio.Bus Music;
        FMOD.Studio.Bus SFX;

        public float MasterVolume = 1f;
        public float MusicVolume = 1f;
        public float SFXVolume = 1f;

        private void Awake()
        {
            //Sets all the FMOD Buses to a variable so you don't have to write it out everytime
            //Again make sure your pathing matches exactly to what you named your busses in your FMOD Session
            Master = FMODUnity.RuntimeManager.GetBus("bus:/Master");
            Music = FMODUnity.RuntimeManager.GetBus("bus:/Master/Music");
            SFX = FMODUnity.RuntimeManager.GetBus("bus:/Master/SFX");

            //SFX Instance to be played when the SFX Slider is moved
            //you can rename this pathing to an event you have in your session this is just what I chose to use
            //SFXVolumeTest = FMODUnity.RuntimeManager.CreateInstance("event:/Test Sound Effect");

            SFXVolume = PlayerPrefs.GetFloat("Sound", 1f);
            MusicVolume = PlayerPrefs.GetFloat("Music", 1f);

            transform.Find("SoundSlider").Find("Slider").GetComponent<Slider>().value = SFXVolume;
            transform.Find("MusicSlider").Find("Slider").GetComponent<Slider>().value = MusicVolume;
        }

        // Update is called once per frame
        void Update()
        {
            //Sets all the Volume Levels of each Bus to the current float value
            Master.setVolume(MasterVolume);
            Music.setVolume(MusicVolume);
            SFX.setVolume(SFXVolume);
        }

        public void MasterVolumeLevel(float newMasterVolume)
        {
            //Sets the FMOD Volume Level to the Slider's current float value
            MasterVolume = newMasterVolume;
        }

        public void MusicVolumeLevel(float newMusicVolume)
        {
            //Sets the FMOD Volume Level to the Slider's current float value
            MusicVolume = newMusicVolume;
            PlayerPrefs.SetFloat("Music", MusicVolume);
        }

        public void SFXVolumeLevel(float newSFXVolume)
        {
            //Sets the FMOD Volume Level to the Slider's current float value
            SFXVolume = newSFXVolume;
            PlayerPrefs.SetFloat("Sound", SFXVolume);

            //Plays a sample sound effect when the SFX Slider is moved and makes sure it doesn't play over itself
            //This is used so the player can hear exactly how loud the sound effects are with the changes they made in realtime rather than guessing
            FMOD.Studio.PLAYBACK_STATE PbState;
            SFXVolumeTest.getPlaybackState(out PbState);
            if(PbState != FMOD.Studio.PLAYBACK_STATE.PLAYING)
            {
                SFXVolumeTest.start();
            }
        }
    }

}
