using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using XnaAux;

namespace AnimationPipeline
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    ///
    /// TODO: change the ContentProcessor attribute to specify the correct
    /// display name for this processor.
    /// </summary>
    [ContentProcessor(DisplayName = "Animation Processor")]
    public class AnimationProcessor : ModelProcessor
    {
        ModelContent model;

        /// <summary>
        /// Bones lookup table, converts bone names to indices.
        /// </summary>
        private Dictionary<string, int> bones = new Dictionary<string, int>();

        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            model = base.Process(input, context);

            AnimationClips clips = ProcessAnimations(model, input, context);
            model.Tag = clips;

            return model;
        }

        /// <summary>
        /// Process the animation content for the model.
        /// </summary>
        /// <param name="model">Model as computed by ModelProcessor</param>
        /// <param name="input">The input content loaded from FBX</param>
        /// <param name="context">Context object for error and warning messages</param>
        /// <returns></returns>
        private AnimationClips ProcessAnimations(ModelContent model,
                                        NodeContent input, ContentProcessorContext context)
        {
            // First build a lookup table so we can determine the 
            // index into the list of bones from a bone name.
            for (int i = 0; i < model.Bones.Count; i++)
            {
                bones[model.Bones[i].Name] = i;
            }

            AnimationClips animationClips = new AnimationClips();

            ProcessAnimationsRecursive(input, animationClips);
            return animationClips;
        }

        /// <summary>
        /// Recursive function that processes the entire scene graph, collecting up
        /// all of the animation data.
        /// </summary>
        /// <param name="input">The input scene graph node</param>
        /// <param name="animationClips">The animation clips object we put animation in</param>
        private void ProcessAnimationsRecursive(NodeContent input, AnimationClips animationClips)
        {
            //System.Diagnostics.Trace.WriteLine(input.Name);
            foreach (KeyValuePair<string, AnimationContent> animation in input.Animations)
            {
                // Do we have this animation before?
                AnimationClips.Clip clip;
                if (!animationClips.Clips.TryGetValue(animation.Key, out clip))
                {
                    // Never before seen clip

                    clip = new AnimationClips.Clip();
                    clip.Name = animation.Key;
                    clip.Duration = animation.Value.Duration.TotalSeconds;
                    clip.Keyframes = new List<AnimationClips.Keyframe>[bones.Count];
                    for (int b = 0; b < bones.Count; b++)
                        clip.Keyframes[b] = new List<AnimationClips.Keyframe>();

                    animationClips.Clips[animation.Key] = clip;
                }

                //
                // For each channel, determine the bone and then process all of the 
                // keyframes for that bone.
                //

                foreach (KeyValuePair<string, AnimationChannel> channel in animation.Value.Channels)
                {
                    // What is the bone index?
                    int boneIndex;
                    if (!bones.TryGetValue(channel.Key, out boneIndex))
                        continue;           // Ignore if not a named bone

                    foreach (AnimationKeyframe keyframe in channel.Value)
                    {
                        Matrix transform = keyframe.Transform;      // Keyframe transformation

                        AnimationClips.Keyframe newKeyframe = new AnimationClips.Keyframe();
                        newKeyframe.Time = keyframe.Time.TotalSeconds;

                        transform.Right = Vector3.Normalize(transform.Right);
                        transform.Up = Vector3.Normalize(transform.Up);
                        transform.Backward = Vector3.Normalize(transform.Backward);
                        newKeyframe.Rotation = Quaternion.CreateFromRotationMatrix(transform);
                        newKeyframe.Translation = transform.Translation;

                        clip.Keyframes[boneIndex].Add(newKeyframe);
                    }
                }
            }

            foreach (NodeContent child in input.Children)
            {
                ProcessAnimationsRecursive(child, animationClips);
            }

        }

    }
}