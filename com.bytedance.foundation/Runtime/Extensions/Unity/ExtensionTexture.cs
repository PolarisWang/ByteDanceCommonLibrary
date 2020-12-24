using UnityEngine;

namespace ByteDance.Foundation
{
    public static class ExtensionTexture
    {
        public static long TexRunningMemSize(this Texture pTex)
        {
            if (pTex == null)
                return 0;
            Texture2D l_tex = pTex as Texture2D;
            if (l_tex != null)
            {
                float l_pixSize = TexPixelSizeBytes(l_tex.format);
                return (long)(pTex.height * pTex.width * l_pixSize);
            }
            else
            {
                return pTex.height * pTex.width * 4;
            }
        }

        private static float TexPixelSizeBytes(TextureFormat pFormat)
        {
            switch (pFormat)
            {
                case TextureFormat.ARGB32:
                    return 4;
                case TextureFormat.ARGB4444:
                    return 2;
                case TextureFormat.RGBA32:
                    return 4;
                case TextureFormat.RGB24:
                    return 3;
                case TextureFormat.RGBA4444:
                    return 2;
                case TextureFormat.PVRTC_RGB2:
                    return 0.25f;
                case TextureFormat.PVRTC_RGBA2:
                    return 0.25f;
                case TextureFormat.PVRTC_RGB4:
                    return 0.5f;
                case TextureFormat.PVRTC_RGBA4:
                    return 0.5f;
                case TextureFormat.ETC_RGB4:
                    return 0.5f;
                case TextureFormat.ETC2_RGBA8:
                    return 1;
                case TextureFormat.ETC2_RGB:
                    return 0.5f;
                default:
                    return 4;//默认是rgba32
            }
        }
    }
}