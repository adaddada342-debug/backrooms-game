using Backrooms.Soundscape.Assets;
using UnityEngine;

namespace Backrooms.Soundscape.Runtime
{
    public class SoundscapeRuntime : MonoBehaviour
    {
        public SoundscapePlan plan;
        public bool createAudioSourcesOnStart = true;
        public bool useGeneratedPlaceholderTones = false;
        public SoundscapeClipLibrary clipLibrary;

        public void Configure(SoundscapePlan newPlan)
        {
            plan = newPlan;
        }

        private void Start()
        {
            if (createAudioSourcesOnStart)
            {
                CreateAudioSources();
            }
        }

        private void CreateAudioSources()
        {
            if (plan == null || plan.emitters == null)
            {
                return;
            }

            foreach (SoundscapeEmitterPlan emitter in plan.emitters)
            {
                if (emitter == null)
                {
                    continue;
                }

                GameObject sourceObject = new GameObject("SoundscapeEmitter_" + emitter.emitterId);
                sourceObject.transform.SetParent(transform);
                sourceObject.transform.position = emitter.position;
                AudioSource source = sourceObject.AddComponent<AudioSource>();
                SoundscapeClipSlot slot = clipLibrary == null ? null : clipLibrary.FindByTag(emitter.soundTag);
                source.clip = slot == null ? null : slot.clip;
                source.loop = slot == null ? emitter.loop : slot.loop;
                source.volume = slot == null ? emitter.volume : emitter.volume * slot.defaultVolume;
                source.spatialBlend = 1f;
                source.minDistance = emitter.minDistance;
                source.maxDistance = emitter.maxDistance;
                // TODO: Replace null clips with approved audio assets or tiny generated placeholders after the audio pipeline is approved.
            }
        }
    }
}
