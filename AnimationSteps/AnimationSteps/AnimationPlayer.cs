using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaAux;
using Microsoft.Xna.Framework;

namespace AnimationSteps
{
    public class AnimationPlayer
    {
        private AnimationClips.Clip clip = null;

        private bool looping = false;
        private double speed = 1.0f;

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
            /*time += delta;

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
                    clip.GetBone(b).CurrentKeyframe++;
                }

                //
                // Update the bone
                //

                int c = clip.GetBone(b).CurrentKeyframe;
                if (c >= 0)
                {
                    AnimationClips.Keyframe keyframe = keyframes[c];
                    clip.GetBone(b).Valid = true;
                    clip.GetBone(b).Rotation = keyframe.Rotation;
                    clip.GetBone(b).Translation = keyframe.Translation;
                }
            }*/
        }
    }
}
