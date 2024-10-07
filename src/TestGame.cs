﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System;

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
        public TestGame()
        {
            grDevMngr = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            grDevMngr.PreferredBackBufferWidth = 1024;
            grDevMngr.PreferredBackBufferHeight = 768;
            grDevMngr.IsFullScreen = false;
            IsMouseVisible = true;
            grDevMngr.ApplyChanges();
        }

        protected override void LoadContent()
        {
            //tvShader = Content.Load<Effect>(@"SelfPlayingPong");
            tvShader = Content.Load<Effect>(@"TvNoise");
            stormyNightShader = Content.Load<Effect>(@"StormyNight");
            phongShader = Content.Load<Effect>(@"Phong");
            LoadImportedScene();
        }
        protected override void Draw(GameTime gameTime)
        {
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
            modelLoader.LoadFromFile("../../../Content/LowPolyInterior_1.obj");
            node = FromAssimp.ConvertNodeTree(GraphicsDevice, modelLoader.Scene, modelLoader.Scene.RootNode, null);
            // Here we define the shaders for particular meshes:
            meshEffectMap = new Dictionary<string, Effect>
            {
                { "Screen", tvShader},
                { "Window", stormyNightShader}
            };
        }
        public Matrix EvaluateCamera(double ticks)
        {
            const int ticksPerCircle = 20000;
            const double r = 2;
            double angle = (double)((int)ticks % ticksPerCircle) / ticksPerCircle * 2 * Math.PI;
            double x = r * Math.Cos(angle);
            double z = r * Math.Sin(angle);
            return Matrix.CreateLookAt(new Vector3((float)x, 2, (float)z), new Vector3(0, 1.5f, 0), Vector3.UnitY);
        }
        public void DrawImportedScene(GraphicsDevice grDev, GameTime gameTime)
        {
            var viewMx = EvaluateCamera(gameTime.TotalGameTime.TotalMilliseconds);
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
            phongShader.Parameters["PointLightPos"].SetValue(new Vector3(0, 0, 0));
            phongShader.Parameters["PointLightColor"].SetValue(new Vector3(1, 1, 1));
            phongShader.Parameters["PointLightIntensity"].SetValue(30f);
            phongShader.Parameters["PointLightDecayExp"].SetValue(2.7f);
            phongShader.Parameters["SpecularColor"].SetValue(new Vector3(1, 1, 1));
            //? phongShader.Parameters["SpecularIntensity"].SetValue(1);
            phongShader.Parameters["Shininess"].SetValue(29);
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
