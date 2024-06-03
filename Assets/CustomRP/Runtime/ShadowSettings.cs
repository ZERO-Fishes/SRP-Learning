using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class ShadowSettings
{
    /// 阴影纹理大小，enum方便切换，后续调用的时候直接强制类型转换
    public enum TextureSize
    {
        _256=256,_512=512,_1024=1024,_2048=2048,_4096=4096,_8192=8192
    }
    /// 直接光阴影设置
    [System.Serializable]
    public struct DirLightShadowSetting
    {
        public TextureSize atlasSize;
    }
    
    
    /// 最大阴影距离
    [Min(0f)] public float maxDistance = 100f;
    /// 直接光阴影设置成员
    public DirLightShadowSetting _DirLightShadowSetting = new DirLightShadowSetting {atlasSize = TextureSize._1024};
}
