using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RhythmGameStarter;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header(" Elements ")]
    public SongManager songManager;

    [Header(" UI ")]
    public Toggle playAutoToggle;
    public Slider fillEnergy;
    public Image songProgressBar;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        playAutoToggle.isOn = false;
        fillEnergy.value = 0;
    }

    public void SetPlayAuto()
    {
        songManager.playAuto = playAutoToggle.isOn;
    }
}
