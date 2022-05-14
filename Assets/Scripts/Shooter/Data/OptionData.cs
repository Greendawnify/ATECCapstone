using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OptionData 
{
    public bool showPregameInfo;
    public bool playMusic;
    public bool playSound;

    public OptionData() {
        showPregameInfo = true;
        playMusic = true;
        playSound = true;
    }

    public void SetUXInfo(bool on) {
        showPregameInfo = on;
    }

    public void SetPlayMusic(bool music) {
        playMusic = music;
    }

    public void SetPlaySound(bool s) {
        playSound = s;
    }
}
