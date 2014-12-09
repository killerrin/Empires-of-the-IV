using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Utilities;
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
            set { m_orbitalRotation = value; GenerateBoundingBox(); }
        }
        
        Vector3 m_rotation;
        public Vector3 Rotation
        {
            get { return m_rotation; }
            set { m_rotation = value; GenerateBoundingBox(); }
        }

        Vector3 m_scale;
        public Vector3 Scale
        {
            get { return m_scale; }
            set { m_scale = value; GenerateBoundingBox(); }
        }

        Vector3 m_position;
        public Vector3 Position
        {
            get { return m_position; }
            set { m_position = value; GenerateBoundingBox(); }
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
        VertexPositionNormalTexture[] m_vertices;
        int[] m_indices;

        Vector3[,] m_terrainVertsPos;
        float[,] m_heightData;


        int m_terrainWidth = 4;
        public int TerrainWidth { get { return m_terrainWidth; } }

        int m_terrainHeight = 3;
        public int TerrainHeight { get { return m_terrainHeight; } }

        float m_highestHeightPoint;
        public float HighestHeight { get { return m_highestHeightPoint * m_scale.Y; } }

        BasicEffect m_effect;
        public BasicEffect Effect { get { return m_effect; } }

        BoundingBox m_boundingBox;
        public BoundingBox BoundingBox { get { return m_boundingBox; } }
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
            GenerateBoundingBox();
        }
        private void LoadHeightData(Texture2D heightMap)
        {
            m_heightMap = heightMap;
            m_terrainWidth = heightMap.Width;
            m_terrainHeight = heightMap.Height;

            float tempHighestHeight = -1.0f;

            Color[] heightMapColors = new Color[m_terrainWidth * m_terrainHeight];
            heightMap.GetData(heightMapColors);

            m_heightData = new float[m_terrainWidth, m_terrainHeight];
            for (int x = 0; x < m_terrainWidth; x++) {
                for (int y = 0; y < m_terrainHeight; y++) {
                    m_heightData[x, y] = heightMapColors[x + y * m_terrainWidth].R / 5.0f;

                    if (m_heightData[x, y] > tempHighestHeight) { tempHighestHeight = m_heightData[x, y]; }
                }
            }

            m_highestHeightPoint = tempHighestHeight;
        }
        
        private void SetUpVertices()
        {
            Vector3 centerAlign = new Vector3(-m_terrainWidth / 2.0f, 0, m_terrainHeight / 2.0f);
            
            m_vertices = new VertexPositionNormalTexture[m_terrainWidth * m_terrainHeight];
            m_terrainVertsPos = new Vector3[m_terrainWidth, m_terrainHeight];

            for (int x = 0; x < m_terrainWidth; x++) {
                for (int y = 0; y < m_terrainHeight; y++) {
                    int vertIndex = x + y * m_terrainWidth;
                    //Debug.WriteLine("Terrain Setup: {0}", vertIndex);

                    m_terrainVertsPos[x, y] = new Vector3(x, m_heightData[x, y], -y) + centerAlign;
                    m_vertices[vertIndex].Position = m_terrainVertsPos[x, y];

                    m_vertices[vertIndex].TextureCoordinate.X = (float)x / 30.0f;
                    m_vertices[vertIndex].TextureCoordinate.Y = (float)y / 30.0f;
                }
                //Debug.WriteLine("\n");
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

        private void GenerateBoundingBox()
        {
            // Get list of points
            Matrix world = WorldMatrix;
            List<Vector3> points = new List<Vector3>();

            for (int i = 0; i < m_vertices.Length; i++) {
                points.Add(Vector3.Transform(m_vertices[i].Position, world));
            }
            m_boundingBox = BoundingBox.CreateFromPoints(points);
        }
        #endregion

        #region Interface Implimentation
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        void IRenderable.Draw(GameTime gameTime, Camera camera, GraphicsDeviceManager graphics) { Draw(gameTime, camera, graphics); }
        #endregion


        public bool IsOnHeightmap(Vector3 point)
        {
            if (point.X > m_boundingBox.Min.X &&
                point.X < m_boundingBox.Max.X &&
                //point.Y > m_boundingBox.Min.Y ||
                //point.Y < m_boundingBox.Max.Y ||
                point.Z > m_boundingBox.Min.Z &&
                point.Z < m_boundingBox.Max.Z) 
            {
                 return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the Height at a point on the Terrain
        /// </summary>
        /// <param name="point">The Vector3 Position of the point with data oriented along X/Z Axis</param>
        /// <returns>The height for the given point on the map</returns>
        public float GetHeightAtPoint(Vector3 point)
        {
            if (!IsOnHeightmap(point)) return float.MaxValue;

            // Grab the X and Z for easy access
            float pointX = point.X;
            float pointZ = point.Z;

            // Pre calculate the World Matrix
            Matrix world = WorldMatrix;

            // Hold the Grid Counters
            int posX = -1;
            int posZ = -1;
            
            // Search Along the X
            for (int x = 0; x < m_terrainWidth; x++) {
                Vector3 vertAtWorld = Vector3.Transform(m_terrainVertsPos[x, 0], world);
                //Debug.WriteLine("X: {0} | Height: {1} | WorldPos: {2}", x, m_heightData[x, 0], vertAtWorld.ToString());

                if (pointX <= vertAtWorld.X) {
                    posX = x;

                    //Debug.WriteLine("PointX Pos: {0} | VertAtWorld: {1} | VertGridSpace: {2}", pointX, vertAtWorld.X, posX);
                    break;
                }
            }

            // Search along the Z
            for (int z = 0; z < m_terrainHeight; z++) {
                Vector3 vertAtWorld = Vector3.Transform(m_terrainVertsPos[0, z], world);
                //Debug.WriteLine("X: {0} | Height: {1} | WorldPos: {2}", z, m_heightData[0, z], vertAtWorld.ToString());

                if (pointZ >= vertAtWorld.Z) {
                    posZ = z;

                    //Debug.WriteLine("PointZ Pos: {0} | VertAtWorld: {1} | VertGridSpace: {2}", pointZ, vertAtWorld.Z, posZ);
                    break;
                }
            }

            // If any of the values are still bad, return empty
            if (posX == -1 || posZ == -1) 
            {
                Debug.WriteLine("A Value on the Terrain Grid could not be found: {0}, {1}", posX, posZ);
                return float.MaxValue;
            }

            // Get the vertex position
            Vector3 vert = Vector3.Transform(m_terrainVertsPos[posX, posZ], world);
            
            // Now we get the height data
            float height = vert.Y;
            
            // Lerp the value between the two and return it
            //Debug.WriteLine("Terrain Height: {0} \n", height);
            return height;
        }

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
            m_effect.World = WorldMatrix;
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

            //m_boundingBox.DrawBoundingBox(graphics, Color.Red, camera, Matrix.Identity);
        }
        #endregion
    }
}
