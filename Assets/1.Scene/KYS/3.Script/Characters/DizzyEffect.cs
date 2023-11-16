using UnityEngine;
using System.Linq;
using System;
using UnityEngine.Rendering;

class DizzyEffect : StatusEffect
{
    private GameObject distortionQuad = null;
    private GameObject distortionPostProcessing = null; 

    public DizzyEffect(StatusEffectedCharacter target) : base(StatusEffectType.Dizzy, target)
    {
        try
        {
            distortionQuad = Camera.main.transform.GetChild(0).gameObject;
        }
        catch (Exception)
        {
            throw new Exception("Main Camera에 1. MainCamera 태그가 안 붙었거나 2. Dizzyness ScreenDistortion 게임 오브젝트가 자식으로 할당되지 않았습니다.");
        }
        
        distortionPostProcessing = GameObject.FindObjectsOfType<Volume>(true).Where(x => x.gameObject.CompareTag("StatusEffect")).Select(x => x.gameObject).FirstOrDefault();

        if (distortionPostProcessing == null)
            throw new Exception("씬에 Dizzyness PostProcessing 오브젝트가 존재하지 않습니다.");

        distortionQuad.SetActive(false);
        distortionPostProcessing.SetActive(false);
    }

    public override void ApplyEffect() { }

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
