using UnityEngine;
using System.Linq;
using System;

namespace RhythmGameStarter
{
    public class TrackTriggerArea : MonoBehaviour
    {
        public TouchWrapperEvent OnNoteTrigger;

        public void TriggerNote(TouchWrapper touch)
        {
            OnNoteTrigger.Invoke(touch);
        }
    }
}