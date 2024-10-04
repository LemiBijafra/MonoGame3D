using Assimp;
using System.Diagnostics;
using System;

namespace Mg3d
{
    public class ModelLoader
    {
        public Scene Scene { get; private set; }
        public void LoadFromFile(string filePath)
        {
            var importer = new AssimpContext();
            try
            {

                Scene = importer.ImportFile(filePath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Debug.Assert(false, filePath + "\n\n" + "A problem loading the model occured: \n " + filePath + " \n" + e.Message);
                Scene = null;
            }
        }
    }
}