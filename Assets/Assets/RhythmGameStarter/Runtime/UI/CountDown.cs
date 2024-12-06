using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace RhythmGameStarter
{
    public class CountDown : MonoBehaviour
    {
        [Comment("The total seconds will be seconds + 1 for the last word display!")]
        public int seconds = 3;

        public bool enabledLastWord = true;

        [Visibility("enabledLastWord", true)] public string lastWord = "Go!";

        [Title("Events"), CollapsedEvent] public StringEvent onCountDown;

        [FormerlySerializedAs("onCountDownFinshed")] [CollapsedEvent]
        public UnityEvent onCountDownFinished;

        public void StartCountDown()
        {
            StopCoroutine(nameof(CountDownCoroutine));
            StartCoroutine(nameof(CountDownCoroutine));
        }
        
        public void OnDisable() {
            StopCoroutine(nameof(CountDownCoroutine));
        }

        private IEnumerator CountDownCoroutine()
        {
            var total = seconds;
            if (!enabledLastWord)
                total--;

            for (int i = total; i >= 0; i--)
            {
                if (enabledLastWord)
                {
                    if (i == 0)
                        onCountDown.Invoke(lastWord);
                    else
                        onCountDown.Invoke((i).ToString());
                }
                else
                {
                    onCountDown.Invoke((i + 1).ToString());
                }

                yield return new WaitForSeconds(1);
            }

            onCountDownFinished.Invoke();
        }
    }
}