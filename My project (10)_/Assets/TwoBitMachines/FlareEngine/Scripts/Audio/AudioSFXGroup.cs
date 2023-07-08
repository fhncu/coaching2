using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
        [AddComponentMenu ("Flare Engine/一Audio/AudioSFXGroup")]
        public class AudioSFXGroup : ReactionBehaviour
        {
                [SerializeField] public List<Audio> sfx = new List<Audio> ( );
                [SerializeField] public bool attenuate = false;
                [SerializeField] public float distance = 20f;

                [SerializeField] public bool active;
                [SerializeField] public int singalIndex;

                public void PlaySFX (string name)
                {
                        if (AudioManager.get == null || Attenuate (out float percent))
                        {
                                return;
                        }
                        for (int i = 0; i < sfx.Count; i++)
                        {
                                if (sfx[i].clip.name == name)
                                {
                                        PlayAudio (sfx[i], sfx[i].volume * percent);
                                        return;
                                }
                        }
                }

                public void PlaySFX (int index)
                {
                        if (AudioManager.get == null || Attenuate (out float percent))
                        {
                                return;
                        }
                        for (int i = 0; i < sfx.Count; i++)
                        {
                                if (i == index)
                                {
                                        PlayAudio (sfx[i], sfx[i].volume * percent);
                                        return;
                                }
                        }
                }

                public void PlaySFXProbability50 (int index)
                {
                        if (Random.Range (0.0f, 1f) >= 0.5f)
                        {
                                return;
                        }
                        if (AudioManager.get == null || Attenuate (out float percent))
                        {
                                return;
                        }
                        for (int i = 0; i < sfx.Count; i++)
                        {
                                if (i == index)
                                {
                                        PlayAudio (sfx[i], sfx[i].volume * percent);
                                        return;
                                }
                        }
                }

                public override void Activate (ImpactPacket packet)
                {
                        for (int i = 0; i < sfx.Count; i++)
                        {
                                PlayAudio (sfx[i], sfx[i].volume);
                        }
                }

                private bool Attenuate (out float percent)
                {
                        percent = 1f;
                        if (!attenuate) return false;
                        float dist = distance * distance;
                        float diff = (WorldManager.gameCam.transform.position - this.transform.position).sqrMagnitude;
                        if (diff > dist) return true;
                        if (diff < 144f) return false;
                        percent = 1f - (diff - 144f) / dist;
                        percent *= percent;
                        return false;
                }

                private void PlayAudio (Audio audio, float volume)
                {
                        AudioManager.get.PlaySFX (audio, volume);
                }

        }
}