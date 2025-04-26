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
    public class Texture
    {
        public int textureHandle { get; private set; }

        public Texture(int width, int height, PixelInternalFormat pixelInternalFormat = PixelInternalFormat.Rgba, PixelFormat pixelFormat = PixelFormat.Rgba, PixelType pixelType = PixelType.UnsignedByte)
        {
            InitTexture(width, height, null, pixelInternalFormat, pixelFormat);
        }

        public Texture(string inTexturePath)
        {
            StbImage.stbi_set_flip_vertically_on_load(1);
            ImageResult textureImage = ImageResult.FromStream(File.OpenRead(inTexturePath), ColorComponents.RedGreenBlueAlpha);

            if (textureImage == null)
            {
                throw new Exception($"ERROR: Failed to load texture: {inTexturePath}");
            }

            InitTexture(textureImage.Width, textureImage.Height, textureImage.Data);
        }

        private void InitTexture(int width, int height, byte[] pixels = null, PixelInternalFormat pixelInternalFormat = PixelInternalFormat.Rgba, PixelFormat pixelFormat = PixelFormat.Rgba, PixelType pixelType = PixelType.UnsignedByte)
        {
            textureHandle = GL.GenTexture();

            UseTexture(TextureUnit.Texture0);

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

            GL.TexImage2D(TextureTarget.Texture2D, 0, pixelInternalFormat, width, height, 0, pixelFormat, pixelType, pixels);
            //GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

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
