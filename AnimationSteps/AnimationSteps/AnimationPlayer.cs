using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaAux;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AnimationSteps
{
    public class AnimationPlayer
    {
        private AnimationClips.Clip clip = null;

        private bool looping = false;
        private double speed = 1.0f;
        private const double maxSpeed = 2;
        private const double minSpeed = 0;

        private double time = 0;

        public double Time { get { return time; } set { time = value; } }

        /// <summary>
        /// Indicates if the playback should "loop" or not.
        /// </summary>
        public bool Looping { get { return looping; } set { looping = value; } }

        /// <summary>
        /// Playback speed
        /// </summary>
        public double Speed { get { return speed; } set { speed = value; } }

        public interface Bone
        {
            bool Valid { get; set; }
            Quaternion Rotation { get; set; }
            Vector3 Translation { get; set; }
        }

        private struct BoneInfo : Bone
        {
            private int currentKeyframe;     // Current keyframe for bone
            private int nextKeyframe;

            private bool valid;

            private Quaternion rotation;
            private Vector3 translation;

            public int CurrentKeyframe { get { return currentKeyframe; } set { currentKeyframe = value; } }
            public int NextKeyframe { get { return nextKeyframe; } set { nextKeyframe = value; } }
            public bool Valid { get { return valid; } set { valid = value; } }
            public Quaternion Rotation { get { return rotation; } set { rotation = value; } }
            public Vector3 Translation { get { return translation; } set { translation = value; } }
        }

        private BoneInfo[] boneInfos;
        private int boneCnt;

        public Bone GetBone(int b) { return boneInfos[b]; }
        public int BoneCount { get { return boneCnt; } }
        public void SetBoneKeyframe(int b, int k)
        {
            boneInfos[b].CurrentKeyframe = k;
            if (k == clip.Keyframes[b].Count - 1)
            {
                boneInfos[b].NextKeyframe = k;
            }
            else
            {
                boneInfos[b].NextKeyframe = k + 1;
            }
        }
        public void SetBoneValid(int b, bool v) { boneInfos[b].Valid = v; }
        public void SetBoneRotation(int b, Quaternion r) { boneInfos[b].Rotation = r; }
        public void SetBoneTranslation(int b, Vector3 t) { boneInfos[b].Translation = t; }

        public AnimationPlayer(AnimationClips.Clip clip)
        {
            this.clip = clip;
        }

        public void Initialize()
        {
            time = 0; 
            boneCnt = clip.Keyframes.Length;
            boneInfos = new BoneInfo[boneCnt];

            for (int b = 0; b < boneCnt; b++)
            {
                boneInfos[b].CurrentKeyframe = -1;
                boneInfos[b].NextKeyframe = -1;
                boneInfos[b].Valid = false;
            }
        }

        public void Update(double delta)
        {
            MouseState mouseState = Mouse.GetState();
            float height = GraphicsDeviceManager.DefaultBackBufferHeight;

            if (mouseState.RightButton == ButtonState.Pressed)
            {
                speed = ((height - mouseState.Y) / height) * 2;
                if (speed < minSpeed) speed = minSpeed;
                if (speed > maxSpeed) speed = maxSpeed;
            }
            time += delta * speed;

            if (time > clip.Duration)
            {
                Initialize();
            }

            for (int b = 0; b < BoneCount; b++)
            {
                List<AnimationClips.Keyframe> keyframes = clip.Keyframes[b];
                if (keyframes.Count == 0)
                    continue;

                // The time needs to be greater than or equal to the
                // current keyframe time and less than the next keyframe 
                // time.
                while (boneInfos[b].CurrentKeyframe < 0 ||
                    (boneInfos[b].CurrentKeyframe < keyframes.Count - 1 &&
                    keyframes[boneInfos[b].CurrentKeyframe + 1].Time <= time))
                {
                    // Advance to the next keyframe
                    SetBoneKeyframe(b, boneInfos[b].CurrentKeyframe + 1);
                }

                //
                // Update the bone
                //
                int c = boneInfos[b].CurrentKeyframe;
                int n = boneInfos[b].NextKeyframe;
                if (c >= 0)
                {
                    Quaternion rotation;
                    Vector3 translation;

                    AnimationClips.Keyframe keyframe1 = keyframes[c];
                    AnimationClips.Keyframe keyframe2 = keyframes[n];

                    if (c != n)
                    {
                        float t = (float)((time - keyframe1.Time) / (keyframe2.Time - keyframe1.Time));
                        rotation = Quaternion.Slerp(keyframe1.Rotation, keyframe2.Rotation, t);
                        translation = Vector3.Lerp(keyframe1.Translation, keyframe2.Translation, t);
                    }
                    else
                    {
                        rotation = keyframe1.Rotation;
                        translation =  keyframe1.Translation;
                    }
                    //AnimationClips.Keyframe keyframeNext = keyframes[c + 1];

                    /*clip.boneInfos[b].Valid = true;
                    clip.boneInfos[b].Rotation = keyframe.Rotation;
                    clip.boneInfos[b].Translation = keyframe.Translation;*/

                    SetBoneValid(b, true);
                    SetBoneRotation(b, rotation);
                    SetBoneTranslation(b, translation);
                }
            }
        }
    }
}
