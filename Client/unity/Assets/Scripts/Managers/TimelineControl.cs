using System.Globalization;
using System.Runtime.InteropServices.WindowsRuntime;
using Client;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineControl : MonoBehaviour
{
    public PlayableDirector playableDirector;  // The PlayableDirector controlling the Timeline
    [Range(0f, 1f)] public float timelinePosition;  // The value between 0 and 1 to control the timeline's position

    private void Update()
    {
        if (playableDirector == null)
        {
            return;
        }

        var length = playableDirector.duration;

        playableDirector.time = length * timelinePosition;

        if (timelinePosition == 1f)
        {
            playableDirector.time = length - 0.001;
        }
    }
    
    
    public void UpdateTimelinePosition(string message)
    {
        if (message == null)
        {
            return;
        }

        var timelineMessage = JsonUtility.FromJson<LightMessage>(message);
        if (timelineMessage is not { type: "Light" })
        {
            return;
        }
        
        if (float.TryParse(timelineMessage.value, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsedValue))
        {
            Debug.Log($"Parsed value successfully: {parsedValue}");
            timelinePosition = 1f - parsedValue;
        }
    }
}