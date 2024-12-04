using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRateStats : MonoBehaviour
{
    static float deltaTime = 0.0f;
    static float lowestFPS = float.MaxValue;
    static float highestFPS = 0.0f;
    static float totalFPS = 0.0f;
    static int frameCount = 0;
    static int percentileFrames = 1000;
    static float[] frameTimes;
    static int currentFrame = 0;
    static float startTime = 0.0f;
    public static bool isUpdating = true;

    private void Start()
    {
        frameTimes = new float[percentileFrames];
        startTime = Time.time;
    }

    private void Update()
    {
        float timeSinceStart = Time.time - startTime;
        if (timeSinceStart >= 3f && isUpdating)
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

            // Calculate FPS
            float fps = 1.0f / deltaTime;

            // Update metrics
            UpdateMetrics(fps);

            // Record frame time
            RecordFrameTime(deltaTime);
        }
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 4 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 50;
        style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        float msec = deltaTime * 1000.0f;

        string text = string.Format("FPS: {0:0.} | Lowest: {1:0.} | Highest: {2:0.} | 1% Low: {3:0.} | Average: {4:0.}",
                                    1.0f / deltaTime, lowestFPS, highestFPS, GetPercentileFPS(99), totalFPS / frameCount);

        GUI.Label(rect, text, style);
    }

    void UpdateMetrics(float fps)
    {
        // Update lowest FPS
        lowestFPS = Mathf.Min(lowestFPS, fps);

        // Update highest FPS
        highestFPS = Mathf.Max(highestFPS, fps);

        // Update total FPS for average calculation
        totalFPS += fps;

        // Update frame count
        frameCount++;
    }

    void RecordFrameTime(float frameTime)
    {
        // Record frame time in the array
        frameTimes[currentFrame] = frameTime;

        // Move to the next frame slot
        currentFrame = (currentFrame + 1) % percentileFrames;
    }

    float GetPercentileFPS(float percentile)
    {
        // Sort the frame times
        System.Array.Sort(frameTimes);

        // Find the index corresponding to the desired percentile
        int index = (int)((percentile / 100.0f) * percentileFrames);

        // Return the frame time at that index
        return 1.0f / frameTimes[index];
    }

    public static void ResetStats()
    {
        deltaTime = 0.0f;
        lowestFPS = float.MaxValue;
        highestFPS = 0.0f;
        totalFPS = 0.0f;
        frameCount = 0;
        percentileFrames = 1000;
        Array.Clear(frameTimes, 0, frameTimes.Length);
        currentFrame = 0;
        startTime = 0.0f;
    }

    public static void ToggleUpdate(bool value)
    {
        isUpdating = value;
    }
}
