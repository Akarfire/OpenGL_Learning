using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace OpenGL_Learning.Engine
{
    // Texture type enumeration
    public enum TextureType { ColorMap, DepthMap, ComputeShaderOutput }
    
    // Texture class
    public class Texture
    {
        public int textureHandle { get; private set; }
        public TextureType textureType { get; private set; } = TextureType.ColorMap;

        public Texture(int width, int height, TextureType type = TextureType.ColorMap)
        {
            textureType = type;
            InitTexture(width, height, null);
        }

        public Texture(string inTexturePath, TextureType type = TextureType.ColorMap)
        {
            StbImage.stbi_set_flip_vertically_on_load(1);
            ImageResult textureImage = ImageResult.FromStream(File.OpenRead(inTexturePath), ColorComponents.RedGreenBlueAlpha);

            if (textureImage == null)
            {
                throw new Exception($"ERROR: Failed to load texture: {inTexturePath}");
            }

            textureType = type;

            InitTexture(textureImage.Width, textureImage.Height, textureImage.Data);
        }

        private void InitTexture(int width, int height, byte[] pixels = null)
        {
            textureHandle = GL.GenTexture();

            UseTexture(TextureUnit.Texture0);

            // Color map case
            if (textureType == TextureType.ColorMap)
            {
                GL.TexParameter(TextureTarget.Texture2D,
                    TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);

                GL.TexParameter(TextureTarget.Texture2D,
                    TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

                GL.TexParameter(TextureTarget.Texture2D,
                    TextureParameterName.TextureMinFilter,
                    (int)TextureMinFilter.Linear);

                GL.TexParameter(TextureTarget.Texture2D,
                    TextureParameterName.TextureMagFilter,
                    (int)TextureMagFilter.Linear);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }

            // Depth texture case
            else if (textureType == TextureType.DepthMap) 
            {
                GL.TexParameter(TextureTarget.Texture2D,
                    TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);

                GL.TexParameter(TextureTarget.Texture2D,
                    TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

                GL.TexParameter(TextureTarget.Texture2D,
                    TextureParameterName.TextureMinFilter,
                    (int)TextureMinFilter.Nearest);

                GL.TexParameter(TextureTarget.Texture2D,
                    TextureParameterName.TextureMagFilter,
                    (int)TextureMagFilter.Nearest);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent24, width, height, 0, PixelFormat.DepthComponent, PixelType.Float, pixels);
            }

            // Compute shader output, uses rgb32f for higher precission
            else if (textureType == TextureType.ComputeShaderOutput)
            {
                GL.TexParameter(TextureTarget.Texture2D,
                    TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);

                GL.TexParameter(TextureTarget.Texture2D,
                    TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

                GL.TexParameter(TextureTarget.Texture2D,
                    TextureParameterName.TextureMinFilter,
                    (int)TextureMinFilter.Nearest);

                GL.TexParameter(TextureTarget.Texture2D,
                    TextureParameterName.TextureMagFilter,
                    (int)TextureMagFilter.Nearest);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba32f, width, height, 0, PixelFormat.Rgba, PixelType.Float, pixels);
            }

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public int GetHandle() { return textureHandle; }

        public void UseTexture(TextureUnit unit)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, textureHandle);
        }

        public void DeleteTexture() { GL.DeleteTexture(textureHandle); }
    }
}
