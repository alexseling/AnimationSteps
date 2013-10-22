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
            bool Valid { get; set; }
            Quaternion Rotation { get; set; }
            Vector3 Translation { get; set; }
            int CurrentKeyframe { get; set; }
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

            private struct BoneInfo : Bone
            {
                private int currentKeyframe;     // Current keyframe for bone
                private int nextKeyframe;

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
            public void IncrementBoneKeyframe(int b) { boneInfos[b].CurrentKeyframe++; }
            public void SetBoneValid(int b, bool v) { boneInfos[b].Valid = v; }
            public void SetBoneRotation(int b, Quaternion r) { boneInfos[b].Rotation = r; }
            public void SetBoneTranslation(int b, Vector3 t) { boneInfos[b].Translation = t; }

            /// <summary>
            /// The keyframes in the animation. We have an array of bones
            /// each with a list of keyframes.
            /// </summary>
            public List<Keyframe>[] Keyframes;

            /// <summary>
            /// Initialize for use
            /// </summary>
            public void Initialize()
            {
                boneCnt = Keyframes.Length;
                boneInfos = new BoneInfo[boneCnt];

                for (int b = 0; b < boneCnt; b++)
                {
                    boneInfos[b].CurrentKeyframe = -1;
                    boneInfos[b].Valid = false;
                }
            }
        }
    }
}
