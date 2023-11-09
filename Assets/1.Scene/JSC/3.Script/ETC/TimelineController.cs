using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelineController : MonoBehaviour
{
    public PlayableDirector playableDirector;
    public TimelineAsset timeline;


    public void Play()
    {
        // ���� playableDirector�� ��ϵǾ� �ִ� Ÿ�Ӷ����� ����
        playableDirector.Play();
    }

    public void PlayFromTimeline()
    {
        // ���ο� timeline�� ����
        playableDirector.Play(timeline);
    }
}
