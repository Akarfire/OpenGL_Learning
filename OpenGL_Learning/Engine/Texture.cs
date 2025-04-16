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
        int textureHandle;

        public Texture(string inTexturePath)
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

            StbImage.stbi_set_flip_vertically_on_load(1);
            ImageResult textureImage = ImageResult.FromStream(File.OpenRead(inTexturePath), ColorComponents.RedGreenBlueAlpha);


            if (textureImage == null)
            {
                Console.WriteLine("Failed to load texture!");
                return;
            }

            Console.WriteLine("Texture loaded: Width = " + textureImage.Width + ", Height = " + textureImage.Height);


            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, textureImage.Width, textureImage.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, textureImage.Data);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

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
