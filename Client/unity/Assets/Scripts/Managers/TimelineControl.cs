using System.Globalization;
using System.Runtime.InteropServices.WindowsRuntime;
using Client;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineControl : MonoBehaviour
{
    public PlayableDirector playableDirector;  // The PlayableDirector controlling the Timeline
    [Range(0f, 1f)] public float timelinePosition;  // The value between 0 and 1 to control the timeline's position
    public float transitionSpeed = 1f;  // The speed at which the timeline position changes

    private void Update()
    {
        if (playableDirector == null)
        {
            return;
        }

        var length = (float) playableDirector.duration;

        playableDirector.time = Mathf.Clamp(Mathf.Lerp((float) playableDirector.time,length * timelinePosition, transitionSpeed * Time.deltaTime), 0, length - 0.001f);
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
        
        timelinePosition = 1f - timelineMessage.value;
        
        // if (float.TryParse(timelineMessage.value, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsedValue))
        // {
        //     Debug.Log($"Parsed value successfully: {parsedValue}");
        //     timelinePosition = 1f - parsedValue;
        // }
    }
}