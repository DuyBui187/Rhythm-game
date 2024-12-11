using RhythmGameStarter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeverManager : MonoBehaviour
{
    public static FeverManager instance;

    [Header(" Elements ")]
    public SongManager songManager;
    public BackgroundFever bgFever;

    [Header(" Settings ")]
    public float fillDuration = 0.5f;
    public float decreaseRate = 0.02f;
    public float frenzyDuration = 5f;

    private bool isFrenzyMode = false;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (isFrenzyMode) return;

        DecreaseEnergyBar();
    }

    public void IncreaseEnergyBar(float fillRate)
    {
        if (isFrenzyMode) return;

        FillEnergySmooth(fillRate);
    }

    private void FillEnergySmooth(float fillRate)
    {
        var fillEnergy = UIManager.instance.fillEnergy;
        float targetFill = Mathf.Min(fillEnergy.value + fillRate, 1);

        LeanTween.value(fillEnergy.value, targetFill, fillDuration)
            .setOnUpdate(value => fillEnergy.value = value)
            .setOnComplete(() =>
            {
                if (fillEnergy.value >= 1f)
                {
                    isFrenzyMode = true;
                    bgFever.ActivateFeverMode();
                    songManager.AddMultiplierStat(1f);
                    StartFrenzyMode();
                }
            });
    }

    private void StartFrenzyMode()
    {
        var fillEnergy = UIManager.instance.fillEnergy;

        LeanTween.value(fillEnergy.value, 0, frenzyDuration)
            .setOnUpdate(value => fillEnergy.value = value)
            .setOnComplete(() =>
            {
                isFrenzyMode = false;
                bgFever.ReturnToInitialPosition();
                songManager.AddMultiplierStat(-1f);
            });
    }

    private void DecreaseEnergyBar()
    {
        var fillEnergy = UIManager.instance.fillEnergy;
        if (fillEnergy.value > 0)
            fillEnergy.value = Mathf.Max(fillEnergy.value - decreaseRate * Time.deltaTime, 0);
    }
}
