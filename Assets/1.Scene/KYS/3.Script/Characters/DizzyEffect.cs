using UnityEngine;
using System.Linq;
using System;
using UnityEngine.Rendering;

class DizzyEffect : StatusEffect
{
    private readonly GameObject distortionQuad = null;
    private readonly GameObject distortionPostProcessing = null;

    public DizzyEffect(StatusEffectedCharacter target) : base(StatusEffectType.Dizzy, target)
    {
        try
        {
            distortionQuad = Camera.main.transform.GetChild(0).gameObject;
        }
        catch (Exception)
        {
            throw new Exception("Main Camera에 Dizzyness ScreenDistortion 게임 오브젝트가 자식으로 할당되지 않았습니다. 에디터의 프로젝트 탭 내 서치바에서 검색 후 할당해주세요.");
        }

        distortionPostProcessing = GameObject.FindObjectsOfType<Volume>(true).Where(x => x.gameObject.CompareTag("StatusEffect")).Select(x => x.gameObject).FirstOrDefault();

        if (distortionPostProcessing == null)
            throw new Exception("씬에 Dizzyness PostProcessing 오브젝트가 존재하지 않습니다.  에디터의 프로젝트 탭 내 서치바에서 검색 후 할당해주세요.");

        distortionQuad.SetActive(false);
        distortionPostProcessing.SetActive(false);
    }

    public override void CustomStartEffect()
    {
        distortionQuad.SetActive(true);
        distortionPostProcessing.SetActive(true);
    }

    public override void CustomEndEffect()
    {
        distortionQuad.SetActive(false);
        distortionPostProcessing.SetActive(false);
    }
}
