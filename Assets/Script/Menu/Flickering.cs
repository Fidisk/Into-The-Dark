using UnityEngine;
using UnityEngine.Rendering.Universal; 
using System.Collections;

public class Flickering : MonoBehaviour
{
    public Light2D light2D;       
    public float minRadius = 5f; 
    public float maxRadius = 15f; 
    public float speed = 2f;      
    public float minIntensity = 0.5f;
    public float maxIntensity = 2f;
    public float intensityFlickerSpeed = 20f;

    private bool growing = true;  

    void Update()
    {
        if (light2D == null) return;

        if (growing)
        {
            light2D.pointLightOuterRadius += speed * Time.deltaTime * Random.Range(0.5f,1.5f);
            if (light2D.pointLightOuterRadius >= maxRadius)
            {
                light2D.pointLightOuterRadius = maxRadius;
                growing = false;
            }
        }
        else
        {
            light2D.pointLightOuterRadius -= speed * Time.deltaTime * Random.Range(0.5f,1.5f);
            if (light2D.pointLightOuterRadius <= minRadius)
            {
                light2D.pointLightOuterRadius = minRadius;
                growing = true;
            }
        }

        light2D.intensity = Mathf.Lerp(minIntensity, maxIntensity, Mathf.PerlinNoise(Time.time * intensityFlickerSpeed, 0f));
    }
}