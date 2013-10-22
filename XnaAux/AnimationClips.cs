using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XnaAux
{
    /// <summary>
    /// Class that contains animation clips data.  This class is 
    /// shared between content processing and the runtime.
    /// </summary>
    public class AnimationClips
    {
        /// <summary>
        /// An Keyframe is a rotation and translation for a moment in time.
        /// </summary>
        public class Keyframe
        {
            public double Time;             // The keyframe time
            public Quaternion Rotation;     // The rotation for the bone
            public Vector3 Translation;     // The translation for the bone
        }

        public interface Bone
        {
            bool Valid { get; }
            Quaternion Rotation { get; }
            Vector3 Translation { get; }
        }

        /// <summary>
        /// The clips for this set of animation clips.
        /// </summary>
        public Dictionary<string, Clip> Clips = new Dictionary<string, Clip>();

        /// <summary>
        /// An animation clip is a set of keyframes.  
        /// </summary>
        public class Clip
        {
            /// <summary>
            /// Name of the animation clip
            /// </summary>
            public string Name;

            /// <summary>
            /// Duration of the animation clip
            /// </summary>
            public double Duration;


            private bool looping = false;
            private double speed = 1.0f;

            private double time = 0;

            public double Time { get { return time; } set { time = value; } }


            private struct BoneInfo : Bone
            {
                private int currentKeyframe;     // Current keyframe for bone
                private bool valid;

                private Quaternion rotation;
                private Vector3 translation;

                public int CurrentKeyframe { get { return currentKeyframe; } set { currentKeyframe = value; } }
                public bool Valid { get { return valid; } set { valid = value; } }
                public Quaternion Rotation { get { return rotation; } set { rotation = value; } }
                public Vector3 Translation { get { return translation; } set { translation = value; } }
            }

            private BoneInfo[] boneInfos;
            private int boneCnt;

            public int BoneCount { get { return boneCnt; } }
            public Bone GetBone(int b) { return boneInfos[b]; }

            /// <summary>
            /// The keyframes in the animation. We have an array of bones
            /// each with a list of keyframes.
            /// </summary>
            public List<Keyframe>[] Keyframes;

            /// <summary>
            /// Indicates if the playback should "loop" or not.
            /// </summary>
            public bool Looping { get { return looping; } set { looping = value; } }

            /// <summary>
            /// Playback speed
            /// </summary>
            public double Speed { get { return speed; } set { speed = value; } }

            /// <summary>
            /// Initialize for use
            /// </summary>
            public void Initialize()
            {
                boneCnt = Keyframes.Length;
                boneInfos = new BoneInfo[boneCnt];

                time = 0;
                for (int b = 0; b < boneCnt; b++)
                {
                    boneInfos[b].CurrentKeyframe = -1;
                    boneInfos[b].Valid = false;
                }
            }


            /// <summary>
            /// Update the clip position
            /// </summary>
            /// <param name="delta">The amount of time that has passed.</param>
            public void Update(double delta)
            {
                time += delta;

                for (int b = 0; b < boneInfos.Length; b++)
                {
                    List<AnimationClips.Keyframe> keyframes = Keyframes[b];
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
                        boneInfos[b].CurrentKeyframe++;
                    }

                    //
                    // Update the bone
                    //

                    int c = boneInfos[b].CurrentKeyframe;
                    if (c >= 0)
                    {
                        AnimationClips.Keyframe keyframe = keyframes[c];
                        boneInfos[b].Valid = true;
                        boneInfos[b].Rotation = keyframe.Rotation;
                        boneInfos[b].Translation = keyframe.Translation;
                    }
                }
            }

        }

    }
}
