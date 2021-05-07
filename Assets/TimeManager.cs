using UnityEngine;

public class TimeManager : MonoBehaviour
{
    #region Singleton

    public static TimeManager instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    public void SetTimeScale(float newTimeScale)
    {
        Time.timeScale = newTimeScale;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

}
