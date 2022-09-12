using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text timerText;
    public float secondsCount;
    public int minuteCount;
    public int hourCount;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        UpdateTimerUI();
    }

    public void UpdateTimerUI()
    {
        //set timer UI
        secondsCount += Time.deltaTime;
        if (hourCount == 0)
        {
            timerText.text = minuteCount.ToString("00") + ":" + ((int)secondsCount).ToString("00") + "";
        }
        if(hourCount >= 1)
        {
            timerText.text = hourCount + ":" + minuteCount.ToString("00") + ":" + ((int)secondsCount).ToString("00") + "";
        }
        if (secondsCount >= 60)
        {
            minuteCount++;
            secondsCount = 0;
        }
        else if (minuteCount >= 60)
        {
            hourCount++;
            minuteCount = 0;
        }
    }
}
