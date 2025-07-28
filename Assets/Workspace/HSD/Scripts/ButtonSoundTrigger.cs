using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSoundTrigger : MonoBehaviour
{
    [SerializeField] AudioClip buttonSelectClip;
    [SerializeField] AudioClip buttonHighlightClip;

    private void PlaySelectSound() => Manager.Audio.PlaySFX(buttonSelectClip, Vector3.zero);

    private void PlayHighlightSound() => Manager.Audio.PlaySFX(buttonHighlightClip, Vector3.zero);
}
