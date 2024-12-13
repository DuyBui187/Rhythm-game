using RhythmGameStarter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public int damage;

    #region RUNTIME_FIELD
    [NonSerialized] public int noteType;
    [NonSerialized] public bool inUse;
    [NonSerialized] public float trapTime;

    [NonSerialized] public Track parentTrack;

    #endregion

    public void ResetForPool()
    {
        inUse = false;
        parentTrack = null;
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "LineArea")
        {
            
        }
    }
}
