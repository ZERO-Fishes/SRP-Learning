using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ChangeMaterialCol : MonoBehaviour
{
    private Material materialToChange;
    private static int baseColorId = Shader.PropertyToID("_BaseColor");
    private static int metallicId = Shader.PropertyToID("_Metallic");
    private static int smoothnessId = Shader.PropertyToID("_Smoothness");
    public Color BaseColor = Color.red;

    [SerializeField, Range(0f, 1f)] 
    public float Merallic = 0f, Smoothness = 0.5f;
    private static MaterialPropertyBlock block;

    private void OnValidate()
    {
        if (block==null)
        {
            block = new MaterialPropertyBlock();
        }

        block.SetColor(baseColorId, BaseColor);//设置材质球的颜色
        block.SetFloat(metallicId,Merallic);
        block.SetFloat(smoothnessId,Smoothness);
        if (GetComponent<Renderer>()!=null)
        {
            GetComponent<Renderer>().SetPropertyBlock(block);
        }
        else
        {
            Debug.Log("需要MeshRenderer");
        }
        
    }

    private void Awake()
    {
        OnValidate();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
