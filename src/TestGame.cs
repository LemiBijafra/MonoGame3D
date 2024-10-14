using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using BdfFontParser;

namespace Mg3d
{
    public class TestGame : Game
    {
        private GraphicsDeviceManager grDevMngr;

        private readonly HashSet<string> selection = new();
        private static TestGame singleton;
        public static TestGame Singleton
        {
            get
            {
                singleton ??= new TestGame();
                return singleton;
            }
            set
            {
                singleton = new TestGame();
            }
        }
        private Effect tvShader;
        private Effect stormyNightShader;
        private Effect phongShader;
        private ModelLoader modelLoader;
        private const float moveStep = 0.05f;

        private Camera Camera { get; set; } = new Camera();
        public TestGame()
        {
            grDevMngr = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            grDevMngr.PreferredBackBufferWidth = 1024;
            grDevMngr.PreferredBackBufferHeight = 768;
            grDevMngr.IsFullScreen = false;
            IsMouseVisible = true;
            grDevMngr.PreparingDeviceSettings += PreparingDeviceSettings;
            grDevMngr.ApplyChanges();

            // -> Playground

            var black = new RawColor(0, 0, 0);
            var white = new RawColor(255, 255, 255);
            var font = new BdfFont("../../../Content/fonts/10x20.bdf");
            var map = font.GetMapOfString("Hello World");
            var bmp = new RawBitmap(map.GetLength(0), map.GetLength(1));
            for (int x = 0; x < bmp.Width; x++)
                for (int y = 0; y < bmp.Height; y++)
                    bmp.SetPixel(x, y, map[x, y] ? white : black);
            //bmp.Save("foo.bmp");

            // <-
        }
        private void PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            grDevMngr.PreferMultiSampling = true;
            e.GraphicsDeviceInformation.PresentationParameters.MultiSampleCount = 4;
        }

        protected override void LoadContent()
        {
            //tvShader = Content.Load<Effect>(@"SelfPlayingPong");
            tvShader = Content.Load<Effect>(@"TvNoise");
            stormyNightShader = Content.Load<Effect>(@"StormyNight");
            phongShader = Content.Load<Effect>(@"Phong");
            phongShader.Parameters["PointLightPos"]?.SetValue(new Vector3(0, 0, 0));
            phongShader.Parameters["PointLightColor"]?.SetValue(new Vector3(1, 1, 1));
            phongShader.Parameters["PointLightIntensity"]?.SetValue(10f);
            phongShader.Parameters["PointLightDecayExp"]?.SetValue(2f);
            phongShader.Parameters["SpecularColor"]?.SetValue(new Vector3(1, 1, 1));
            //? phongShader.Parameters["SpecularIntensity"].SetValue(1);
            phongShader.Parameters["Shininess"]?.SetValue(10);
            LoadImportedScene();
        }

        protected void ProcessInputEvents()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                Camera.RotateLeft(1);
            }
            else
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                Camera.RotateRight(1);
            }
            else
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                Camera.Forward(moveStep);
            }
            else
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                Camera.Backward(moveStep);
            }
        }
        protected override void Draw(GameTime gameTime)
        {
            ProcessInputEvents();
            GraphicsDevice.Clear(Color.Black);
            var myRast = new RasterizerState();
            myRast.CullMode = CullMode.CullClockwiseFace;
            myRast.DepthClipEnable = true;
            myRast.MultiSampleAntiAlias = true;
            GraphicsDevice.RasterizerState = myRast;

            tvShader.Parameters["fTimer"].SetValue(((float)gameTime.TotalGameTime.TotalSeconds));
            stormyNightShader.Parameters["fTimer"].SetValue(((float)gameTime.TotalGameTime.TotalSeconds));
            DrawImportedScene(GraphicsDevice, gameTime);
        }
        private Mg3d.Node node;
        Dictionary<string, Effect> meshEffectMap;
        public void LoadImportedScene()
        {
            modelLoader = new ModelLoader();
            modelLoader.LoadFromFile("../../../Content/exported_3d_models/LowPolyInterior_1.obj");
            node = FromAssimp.ConvertNodeTree(GraphicsDevice, modelLoader.Scene, modelLoader.Scene.RootNode, null);
            // Here we define the shaders for particular meshes:
            meshEffectMap = new Dictionary<string, Effect>
            {
                { "Screen", tvShader},
                { "Window", stormyNightShader},
                { "Cage_Bars", stormyNightShader}
            };
        }

        public void DrawImportedScene(GraphicsDevice grDev, GameTime gameTime)
        {
            //var viewMx = Camera.EvaluateCamera(/*gameTime.TotalGameTime.TotalMilliseconds*/);
            var viewMx = Camera.Matrix;
            var projMx = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60), 800 / 480f, .1f, 100f);
            var overallMx = viewMx * projMx;
            //TODO set automatically
            tvShader.Parameters["ModelViewProjMx"].SetValue(overallMx);
            stormyNightShader.Parameters["ModelViewProjMx"].SetValue(overallMx);
            phongShader.Parameters["ModelViewProjMx"].SetValue(overallMx);
            var tmpMx = Matrix.Invert(viewMx);
            var normMx = Matrix.Transpose(tmpMx);
            phongShader.Parameters["ModelViewProjMx"].SetValue(overallMx);
            phongShader.Parameters["ModelViewMx"].SetValue(viewMx);
            phongShader.Parameters["NormMx"].SetValue(normMx);
            
            Renderer.DrawNodeRecur(grDev, phongShader, meshEffectMap, node);
        }
        public static void RunIt()
        {
            if (singleton == null)
            {
                singleton = new TestGame();
                singleton.Run();
            }
        }
    }
}
