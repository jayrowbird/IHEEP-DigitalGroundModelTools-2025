//The MIT License (MIT)

//Copyright (c) 2014 Dave Kerr

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.


using OpenTK.Mathematics;
using System.Text;
using System.Xml.Linq;

namespace FileFormatWavefront.Model
{
    /// <summary>
    /// Represents a material.
    /// </summary>
    public class Material
    {
        //Use NoColor to test for the existence of a color value or no color
        public readonly static Color4 NoColor = new Color4(-1, -1, -1, -1);

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; internal set; } = string.Empty;

        /// <summary>
        /// Gets the ambient.
        /// </summary>
        public Color4 Ambient { get; internal set; }

        /// <summary>
        /// Gets the diffuse.
        /// </summary>
        public Color4 Diffuse { get; internal set; } = NoColor;
        /// <summary>
        /// Gets the specular.
        /// </summary>
        public Color4 Specular { get; internal set; }
        /// <summary>
        /// Gets the Transmittance.
        /// </summary>
        public Color4 Transmittance { get; internal set; }

        /// <summary>
        /// Gets the Emission.
        /// </summary>
        public Color4 Emission { get; internal set; }
        /// <summary>
        /// Gets the shininess.
        /// </summary>
        public float Shininess { get; internal set; }
        /// <summary>
        /// Gets the transparency.
        /// </summary>
        public float? Transparency { get; internal set; }

        /// <summary>
        /// Gets the ambient texture map.
        /// </summary>
        public TextureMap? TextureMapAmbient { get; internal set; }

        /// <summary>
        /// Gets the diffuse texture map.
        /// </summary>
        public TextureMap? TextureMapDiffuse { get; internal set; }

        /// <summary>
        /// Gets the specular texture map.
        /// </summary>
        public TextureMap? TextureMapSpecular { get; internal set; }

        /// <summary>
        /// Gets the specular highlight texture map.
        /// </summary>
        public TextureMap? TextureMapSpecularHighlight { get; internal set; }

        /// <summary>
        /// Gets the alpha texture map.
        /// </summary>
        public TextureMap? TextureMapAlpha { get; internal set; }

        /// <summary>
        /// Gets the bump texture map.
        /// </summary>
        public TextureMap? TextureMapBump { get; internal set; }

        /// <summary>
        /// Gets the illumination model.
        /// See the specification at http://paulbourke.net/dataformats/mtl/ for details on this value.
        /// </summary>
        /// <value>
        /// The illumination model.
        /// </value>
        public int IlluminationModel { get; internal set; }

        /// <summary>
        /// Gets the optical density, also known as the Index of Refraction.
        /// </summary>
        public float? OpticalDensity { get; internal set; }

        /// <summary>
        /// Gets the occasionally used bump strength.
        /// </summary>
        public float? BumpStrength { get; internal set; }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("Material Name:[{0}] Ambient:[{1}] Diffuse:[{2}] Specular:[{3}]", Name, Ambient, Diffuse, Specular));
            string TextureMapAmbientName = "null";
            string TextureMapDiffuseName = "null";
            string TextureMapSpecularName = "null";
            if (TextureMapAmbient != null)
            {
                TextureMapAmbientName = TextureMapAmbient.Path;
            }
            if (TextureMapDiffuse != null)
            {
                TextureMapDiffuseName = TextureMapDiffuse.Path;
            }
            if (TextureMapSpecular != null)
            {
                TextureMapSpecularName = TextureMapSpecular.Path;
            }
            sb.AppendLine(string.Format("TextureMapAmbient :[{0}] TextureMapDiffuse:[{1}] TextureMapSpecular:[{2}] ", TextureMapAmbientName, TextureMapDiffuseName, TextureMapSpecularName));

            return sb.ToString();
        }
    }
}