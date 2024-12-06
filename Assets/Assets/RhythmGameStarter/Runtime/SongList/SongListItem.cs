using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RhythmGameStarter
{
    public class SongListItem : MonoBehaviour
    {
        [NonSerialized]
        public SongItem targetSongItem;
        public TextMeshProUGUI label;
        public TextMeshProUGUI authorLabel;
        public TextMeshProUGUI indexLabel;
        public Button button;
        public Image difficultiesFill;
        public Image coverArtImage;
        [CollapsedEvent]
        public UnityEvent onItemSetup;
    }
}