using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CustomLitShaderGUI : ShaderGUI
{
    private MaterialEditor editor;
    private Object[] materials;
    private MaterialProperty[] properties;
    
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {

        editor = materialEditor;
        this.properties = properties;
        materials = materialEditor.targets;
    }
}
