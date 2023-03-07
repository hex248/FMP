using UnityEngine;
using System.Collections;

[ExecuteAlways]
public class ReplacementShaderEffect : MonoBehaviour
{
    public Shader ReplacementShader;


    void OnEnable()
    {
        if (ReplacementShader != null)
            GetComponent<Camera>().SetReplacementShader(ReplacementShader, "");
    }

    void OnDisable()
    {
        GetComponent<Camera>().ResetReplacementShader();
    }
}
