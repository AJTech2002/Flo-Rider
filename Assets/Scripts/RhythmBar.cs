using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RhythmBar : MonoBehaviour
{
    public RhythmEngine engine;

    [Header("UI References")] 
    public Transform whistleIcon;
    public RectTransform barContainer;

    private float width
    {
        get {
            return barContainer.rect.width;
        }
    }
    
    
    
    private float barTotalDurationS = 2.0f; // How far ahead the bar should show notes, in seconds
    
    public void StartSong()
    {
        
    }
    
}