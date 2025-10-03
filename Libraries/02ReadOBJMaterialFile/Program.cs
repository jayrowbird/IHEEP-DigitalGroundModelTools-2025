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


using FileFormatWavefront;
using FileFormatWavefront.Model;

namespace ReadOBJMaterials
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Read OBJ Material file");

            var materials = new List<Material>();
            var messages = new List<Message>();

            string materialPath = @".\TestData\material.lib";
            bool loadTextureImages = false;

            var fileLoadResult = FileFormatMtl.Load(materialPath, loadTextureImages);
            materials.AddRange(fileLoadResult.Model);
            messages.AddRange(fileLoadResult.Messages);

            Console.WriteLine("Done Read OBJ Material file");
            Console.ReadKey();
        }
    }
}
