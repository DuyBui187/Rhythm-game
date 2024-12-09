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

    [Header(" Settings ")]
    public float fillDuration = 0.5f;
    public float decreaseRate = 0.02f;
    public float frenzyDuration = 5f;

    private bool isFrenzyMode = false;

    [Header(" UI ")]
    public Toggle playAutoToggle;
    public Slider fillEnergy;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        playAutoToggle.isOn = false;
        fillEnergy.value = 0;
    }

    void Update()
    {
        if (isFrenzyMode) return;

        DecreaseEnergyBar();
    }

    public void SetPlayAuto()
    {
        songManager.playAuto = playAutoToggle.isOn;
    }

    public void IncreaseEnergyBar(float fillRate)
    {
        if (isFrenzyMode) return;

        FillEnergySmooth(fillRate);
    }

    private void FillEnergySmooth(float fillRate)
    {
        float targetFill = Mathf.Min(fillEnergy.value + fillRate, 1);

        LeanTween.value(fillEnergy.value, targetFill, fillDuration)
            .setOnUpdate((value) => fillEnergy.value = value)
            .setOnComplete(() =>
            {
                if (fillEnergy.value >= 1f)
                {
                    isFrenzyMode = true;
                    StartFrenzyMode();
                }
            });
    }

    private void StartFrenzyMode()
    {
        LeanTween.value(fillEnergy.value, 0, frenzyDuration)
            .setOnUpdate((value) => fillEnergy.value = value)
            .setOnComplete(() =>
            {
                isFrenzyMode = false;
            });
    }

    private void DecreaseEnergyBar()
    {
        if (fillEnergy.value > 0)
            fillEnergy.value = Mathf.Max(fillEnergy.value - decreaseRate * Time.deltaTime, 0);
    }
}
