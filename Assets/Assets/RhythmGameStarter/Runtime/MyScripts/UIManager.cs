using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RhythmGameStarter;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header(" Elements ")]
    public SongManager songManager;

    [Header(" UI ")]
    public Toggle playAutoToogle;

    void Start()
    {
        playAutoToogle.isOn = false;
    }

    public void SetPlayAuto()
    {
        songManager.playAuto = playAutoToogle.isOn;
    }
}
