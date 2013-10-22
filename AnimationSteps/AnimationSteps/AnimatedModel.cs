using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using XnaAux;

namespace AnimationSteps
{
    /// <summary>
    /// A class for a model we will animate
    /// </summary>
    public class AnimatedModel
    {
        /// <summary>
        /// Reference to the game that uses this class
        /// </summary>
        private StepGame game;

        /// <summary>
        /// The underlying XNA model
        /// </summary>
        private Model model;

        /// <summary>
        /// The bond transforms as loaded from the model
        /// </summary>
        private Matrix[] bindTransforms;

        /// <summary>
        /// The current bone transforms we will use
        /// </summary>
        private Matrix[] boneTransforms;

        /// <summary>
        /// The computed absolute transforms
        /// </summary>
        private Matrix[] absoTransforms;


        /// <summary>
        /// Name of the asset we are going to load
        /// </summary>
        private string asset;

        private AnimationClips.Clip clip = null;
        private AnimationPlayer player = null;


        #region Construction and Initialization


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">The game for this model</param>
        /// <param name="asset">Name of the model asset</param>
        public AnimatedModel(StepGame game, string asset)
        {
            this.game = game;
            this.asset = asset;
        }


        /// <summary>
        /// This function is called to load content into this component
        /// of our game.
        /// </summary>
        /// <param name="content">The content manager to load from.</param>
        public void LoadContent(ContentManager content)
        {
            model = content.Load<Model>(asset);

            // Allocate the array to the number of bones we have
            int boneCnt = model.Bones.Count;
            bindTransforms = new Matrix[boneCnt];
            boneTransforms = new Matrix[boneCnt];
            absoTransforms = new Matrix[boneCnt];

            // Copy the bone transforms from the model to our local arrays
            model.CopyBoneTransformsTo(bindTransforms);
            model.CopyBoneTransformsTo(boneTransforms);
            model.CopyAbsoluteBoneTransformsTo(absoTransforms);

            PlayClip("Take 001");
        }

        #endregion

        #region Update

        /// <summary>
        /// This function is called to update this component of our game
        /// to the current game time.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            double delta = gameTime.ElapsedGameTime.TotalSeconds;

            if (player != null && clip != null)
            {
                // Update the clip
                player.Update(delta);

                for (int b = 0; b < clip.BoneCount; b++)
                {
                    AnimationClips.Bone bone = clip.GetBone(b);
                    if (!bone.Valid)
                        continue;

                    Vector3 scale = new Vector3(bindTransforms[b].Right.Length(),
                        bindTransforms[b].Up.Length(),
                        bindTransforms[b].Backward.Length());

                    boneTransforms[b] = Matrix.CreateScale(scale) *
                        Matrix.CreateFromQuaternion(bone.Rotation) *
                        Matrix.CreateTranslation(bone.Translation);
                }

                model.CopyBoneTransformsFrom(boneTransforms);
            }

            model.CopyBoneTransformsFrom(boneTransforms);
            model.CopyAbsoluteBoneTransformsTo(absoTransforms);
        }

        #endregion

        #region Drawing

        /// <summary>
        /// Play an animation clip on this model.
        /// </summary>
        /// <param name="name"></param>
        public AnimationClips.Clip PlayClip(string name)
        {
            AnimationClips clips = model.Tag as AnimationClips;
            if (clips != null)
            {
                clip = clips.Clips[name];
                player = new AnimationPlayer(clip);
                player.Looping = true;
                player.Initialize();
            }

            return clip;
        }

        /// <summary>
        /// This function is called to draw this game component.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="gameTime"></param>
        public void Draw(GraphicsDeviceManager graphics, GameTime gameTime)
        {
            Matrix transform = Matrix.Identity;

            DrawModel(graphics, model, transform);
        }

        private void DrawModel(GraphicsDeviceManager graphics, Model model, Matrix world)
        {

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = absoTransforms[mesh.ParentBone.Index] * world;
                    effect.View = game.Camera.View;
                    effect.Projection = game.Camera.Projection;
                }
                mesh.Draw();
            }
        }

        #endregion

    }
}