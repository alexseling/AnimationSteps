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

        public AnimationPlayer(AnimationClips.Clip clip)
        {
            this.clip = clip;
        }

        public void Initialize()
        {
            time = 0;
            clip.Initialize();
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
                time = 0;
                clip.Initialize();
            }

            for (int b = 0; b < clip.BoneCount; b++)
            {
                List<AnimationClips.Keyframe> keyframes = clip.Keyframes[b];
                if (keyframes.Count == 0)
                    continue;

                // The time needs to be greater than or equal to the
                // current keyframe time and less than the next keyframe 
                // time.
                while (clip.GetBone(b).CurrentKeyframe < 0 ||
                    (clip.GetBone(b).CurrentKeyframe < keyframes.Count - 1 &&
                    keyframes[clip.GetBone(b).CurrentKeyframe + 1].Time <= time))
                {
                    // Advance to the next keyframe
                    clip.SetBoneKeyframe(b, clip.GetBone(b).CurrentKeyframe + 1);
                }

                //
                // Update the bone
                //
                int c = clip.GetBone(b).CurrentKeyframe;
                int n = clip.GetBone(b).NextKeyframe;
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

                    /*clip.GetBone(b).Valid = true;
                    clip.GetBone(b).Rotation = keyframe.Rotation;
                    clip.GetBone(b).Translation = keyframe.Translation;*/

                    clip.SetBoneValid(b, true);
                    clip.SetBoneRotation(b, rotation);
                    clip.SetBoneTranslation(b, translation);
                }
            }
        }
    }
}
