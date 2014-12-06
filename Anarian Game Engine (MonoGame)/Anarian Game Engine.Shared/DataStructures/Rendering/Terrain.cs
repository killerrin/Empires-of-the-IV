using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Anarian.Interfaces;
using Anarian.Helpers;

namespace Anarian.DataStructures.Rendering
{
    public class Terrain : IUpdatable, IRenderable
    {
        #region Fields/Properties
        bool m_active;
        bool m_visible;

        Texture2D m_heightMap;
        Texture2D m_texture;

        public bool Active
        {
            get { return m_active; }
            set { m_active = value; }
        }
        public bool Visible
        {
            get { return m_visible; }
            set { m_visible = value; }
        }

        public Texture2D HeightMap
        {
            get { return m_heightMap; }
            protected set { m_heightMap = value; }
        }
        public Texture2D Texture
        {
            get { return m_texture; }
            set { m_texture = value; }
        }
        #endregion

        #region Translations
        Vector3 m_orbitalRotation;
        public Vector3 OrbitalRotation
        {
            get { return m_orbitalRotation; }
            set { m_orbitalRotation = value; }
        }
        
        Vector3 m_rotation;
        public Vector3 Rotation
        {
            get { return m_rotation; }
            set { m_rotation = value; }
        }

        Vector3 m_scale;
        public Vector3 Scale
        {
            get { return m_scale; }
            set { m_scale = value; }
        }

        Vector3 m_position;
        public Vector3 Position
        {
            get { return m_position; }
            set { m_position = value; }
        }

        public Matrix WorldMatrix
        {
            get
            {
                Matrix scale = Matrix.CreateScale(Scale);

                Vector3 worldRotation = Rotation;
                Matrix rotX = Matrix.CreateRotationX(worldRotation.X);
                Matrix rotY = Matrix.CreateRotationY(worldRotation.Y);
                Matrix rotZ = Matrix.CreateRotationZ(worldRotation.Z);
                Matrix rotation = rotX * rotY * rotZ;

                Matrix translation = Matrix.CreateTranslation(Position);

                Vector3 worldOrbitalRotation = OrbitalRotation;
                Matrix rotOX = Matrix.CreateRotationX(worldOrbitalRotation.X);
                Matrix rotOY = Matrix.CreateRotationY(worldOrbitalRotation.Y);
                Matrix rotOZ = Matrix.CreateRotationZ(worldOrbitalRotation.Z);
                Matrix orbitalRotation = rotOX * rotOY * rotOZ;

                return scale * rotation * translation * orbitalRotation;
            }
        }
        #endregion

        #region TerrainData
        //VertexBuffer m_terrainVertexBuffer;
        //IndexBuffer m_terrainIndexBuffer;
        //VertexDeclaration m_terrainVertexDeclaration;

        VertexPositionNormalTexture[] m_vertices;
        int[] m_indices;

        int m_terrainWidth = 4;
        int m_terrainHeight = 3;
        float[,] m_heightData;

        BasicEffect m_effect;
        public BasicEffect Effect { get { return m_effect; } }
        #endregion

        public Terrain(GraphicsDeviceManager graphics, Texture2D heightMap, Texture2D texture)
        {
            // Default Variables
            m_active = true;
            m_visible = true;

            m_orbitalRotation = Vector3.Zero;

            m_position = Vector3.Zero;
            m_rotation = Vector3.Zero;
            m_scale = Vector3.One;

            // Store the Texture
            m_texture = texture;

            // Finally, Setup the terrain
            SetupTerrain(graphics.GraphicsDevice, heightMap);
        }

        #region Terrain Setup
        private void SetupTerrain(GraphicsDevice graphics, Texture2D heightMap)
        {
            LoadHeightData(heightMap);

            SetUpVertices();
            SetUpIndices();
            CalculateNormals();

            SetupEffects(graphics);
        }
        private void LoadHeightData(Texture2D heightMap)
        {
            m_heightMap = heightMap;
            m_terrainWidth = heightMap.Width;
            m_terrainHeight = heightMap.Height;

            Color[] heightMapColors = new Color[m_terrainWidth * m_terrainHeight];
            heightMap.GetData(heightMapColors);

            m_heightData = new float[m_terrainWidth, m_terrainHeight];
            for (int x = 0; x < m_terrainWidth; x++)
                for (int y = 0; y < m_terrainHeight; y++)
                    m_heightData[x, y] = heightMapColors[x + y * m_terrainWidth].R / 5.0f;
        }
        
        private void SetUpVertices()
        {
            m_vertices = new VertexPositionNormalTexture[m_terrainWidth * m_terrainHeight];
            for (int x = 0; x < m_terrainWidth; x++) {
                for (int y = 0; y < m_terrainHeight; y++) {
                    m_vertices[x + y * m_terrainWidth].Position = new Vector3(x, m_heightData[x, y], -y);

                    //m_vertices[x + y * m_terrainWidth].Color = Color.White;
                    m_vertices[x + y * m_terrainWidth].TextureCoordinate.X = (float)x / 30.0f;
                    m_vertices[x + y * m_terrainWidth].TextureCoordinate.Y = (float)y / 30.0f;
                }
            }
        }

        private void SetUpIndices()
        {
            m_indices = new int[(m_terrainWidth - 1) * (m_terrainHeight - 1) * 6];
            int counter = 0;
            for (int y = 0; y < m_terrainHeight - 1; y++) {
                for (int x = 0; x < m_terrainWidth - 1; x++) {
                    int lowerLeft = x + y * m_terrainWidth;
                    int lowerRight = (x + 1) + y * m_terrainWidth;
                    int topLeft = x + (y + 1) * m_terrainWidth;
                    int topRight = (x + 1) + (y + 1) * m_terrainWidth;

                    m_indices[counter++] = topLeft;
                    m_indices[counter++] = lowerRight;
                    m_indices[counter++] = lowerLeft;

                    m_indices[counter++] = topLeft;
                    m_indices[counter++] = topRight;
                    m_indices[counter++] = lowerRight;
                }
            }
        }

        private void CalculateNormals()
        {
            for (int i = 0; i < m_vertices.Length; i++)
                m_vertices[i].Normal = new Vector3(0, 0, 0);

            for (int i = 0; i < m_indices.Length / 3; i++) {
                int index1 = m_indices[i * 3];
                int index2 = m_indices[i * 3 + 1];
                int index3 = m_indices[i * 3 + 2];

                Vector3 side1 = m_vertices[index1].Position - m_vertices[index3].Position;
                Vector3 side2 = m_vertices[index1].Position - m_vertices[index2].Position;
                Vector3 normal = Vector3.Cross(side1, side2);

                m_vertices[index1].Normal += normal;
                m_vertices[index2].Normal += normal;
                m_vertices[index3].Normal += normal;
            }

            for (int i = 0; i < m_vertices.Length; i++)
                m_vertices[i].Normal.Normalize();
        }

        private void SetupEffects(GraphicsDevice graphics)
        {
            m_effect = new BasicEffect(graphics);
            
            m_effect.TextureEnabled = true;
            m_effect.Texture = m_texture;

            m_effect.EnableDefaultLighting();
        }
        #endregion

        #region Interface Implimentation
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        void IRenderable.Draw(GameTime gameTime, Camera camera, GraphicsDeviceManager graphics) { Draw(gameTime, camera, graphics); }
        #endregion

        #region Update/Draw
        public void Update(GameTime gameTime)
        {
            if (!m_active) return;
        }

        public void Draw(GameTime gameTime, Camera camera, GraphicsDeviceManager graphics)
        {
            if (!m_active) return;
            if (!m_visible) return;

            // Prep the Graphics Device
            graphics.GraphicsDevice.RasterizerState.CullMode = CullMode.None;

            // Begin Drawing the World

            // Since the world will be generated outwards from its side, we are offsetting the orgin of the world to its center
            m_effect.World = Matrix.CreateTranslation(-m_terrainWidth / 2.0f, 0, m_terrainHeight / 2.0f) * WorldMatrix;
            m_effect.View = camera.View;
            m_effect.Projection = camera.Projection;
            
            foreach (EffectPass pass in m_effect.CurrentTechnique.Passes) {
                pass.Apply();

                graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    m_vertices, 0, m_vertices.Length,
                    m_indices, 0, m_indices.Length / 3,
                    VertexPositionNormalTexture.VertexDeclaration);
            }
        }
        #endregion
    }
}
