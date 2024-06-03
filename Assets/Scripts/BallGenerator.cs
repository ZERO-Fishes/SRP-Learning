using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BallGenerator : MonoBehaviour
{
    public Mesh BallMesh;
    public Material BallMaterial;
    private static readonly int baseColorId=Shader.PropertyToID("_BaseColor");
    private Matrix4x4[] matrices = new Matrix4x4[1023];

    private Vector4[] baseColors = new Vector4[1023];

    private MaterialPropertyBlock block;

    private void Awake()
    {
        for (int i = 0; i < matrices.Length; i++)
        {
            matrices[i] = Matrix4x4.TRS(Random.insideUnitSphere * 10f, Quaternion.identity, Vector3.one);//不同位置
            baseColors[i] = new Vector4(Random.value, Random.value, Random.value, 1f);//不同颜色
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (block==null)
        {
            block = new MaterialPropertyBlock();
            block.SetVectorArray(baseColorId,baseColors);
        }

        Graphics.DrawMeshInstanced(BallMesh, 0, BallMaterial, matrices, 1023, block);
    }
}
