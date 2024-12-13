using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmGameStarter
{
    public class TargetBoundary : MonoBehaviour
    {
        private TrackManager trackManager;

        private void Awake()
        {
            trackManager = GetComponentInParent<TrackManager>();
        }

        void OnTriggerExit(Collider col)
        {
            switch (col.tag)
            {
                case "Note":

                    if (trackManager.useNotePool)
                        trackManager.ResetNoteToPool(col.gameObject);

                    else
                        Destroy(col.gameObject);

                    break;

                case "Trap":

                    if (trackManager.useNotePool)
                        trackManager.ResetTrapToPool(col.gameObject);

                    else
                        Destroy(col.gameObject);

                    break;
            }
        }
    }
}