using UnityEngine;
using UnityEngine.Rendering;

public class LightingFix : MonoBehaviour
{
    void Awake()
    {
        DynamicGI.UpdateEnvironment();
    }
} 