using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

enum BGMtype
{
    Intro,
    Town,
    Dungeon,
    Boss1,
    Boss2,
    Ending,
};
public class SoundManagerBGM : MonoBehaviour
{       
    private GameObject bgm;
    private GameObject player;
    private AudioSource source;

    [Header("��Ʈ�� - ���� - ���� - ����12 - ����")]
    [SerializeField] private AudioClip[] bgmType;     

    //BGM���� Ȯ��
    private int _type = 0;

    //���� Scene �̸�Ȯ�κ���
    private string sceneName;

    private bool bgmChange = false;
    
    
    private void Awake()
    {
        sceneName = this.gameObject.scene.name;
        if (sceneName != "Intro")
        {
            player = GameObject.Find("Player");
        }
        bgm = GameObject.Find("SoundManager");
        source = bgm.GetComponent<AudioSource>();
    }
    private void Update()
    {
        if (!bgmChange)
        {   
            switch (sceneName)
            {
                case "Intro":
                    StarteBGM_Intro();
                    return;
                case "MainGame":
                    StartBGM_MainGame();
                    return;
                case "BossRoom":
                    StartBGM_BossRoom();
                    return;
                case "GameEnd":
                    StarteBGM_Ending();
                    return;
            }

        }
    }
    
    private void StarteBGM_Intro()
    {
        _type = (int)BGMtype.Intro;
        if (source.isPlaying && source.clip == bgmType[_type])
        {
            return;
        }
        else
        {
            StartCoroutine(PlayBGM(BGMtype.Intro));
            
        }
    }
    private void StartBGM_MainGame()
    {   
        
        //������ �ʾ��� ��
        if (player.transform.position.z<-100)
        {
            _type = (int)BGMtype.Town;
            if (source.isPlaying && source.clip == bgmType[_type])
            {
                return;
            }
            else
            {
                //StopCoroutine("PlayBGM");
                StartCoroutine(PlayBGM(BGMtype.Town));
            }
        }
        //Player�� ������ ������ ��
        else
        {
            _type = (int)BGMtype.Dungeon;
            if (source.isPlaying&&source.clip == bgmType[_type])
            {                
                return;
            }
            else
            {
                //StopCoroutine("PlayBGM");
                StartCoroutine(PlayBGM(BGMtype.Dungeon));
            }
        }
    }
    private void StartBGM_BossRoom()
    {

        //���� 1��
        if (true)
        {
            _type = (int)BGMtype.Boss1;
            if (source.isPlaying && source.clip == bgmType[_type])
            {
                return;
            }
            else
            {
                StartCoroutine(PlayBGM(BGMtype.Boss1));
            }
        }
        //���� 2��
        else
        {
            _type = (int)BGMtype.Boss2;
            if (source.isPlaying && source.clip == bgmType[_type])
            {
                return;
            }
            else
            {
                StartCoroutine(PlayBGM(BGMtype.Boss2));
            }
        }
    }
    private void StarteBGM_Ending()
    {
        _type = (int)BGMtype.Ending;
        if (source.isPlaying && source.clip == bgmType[_type])
        {
            return;
        }
        else
        {
            StartCoroutine(PlayBGM(BGMtype.Ending));
        }
    }
    
    private IEnumerator PlayBGM(BGMtype type)
    {
        //���� �� �ݺ��� �����ϱ� ����
        bgmChange = true;

        float startVolume = 0f;
        float lerpDuration = 10f;
        float time = 0f;
        float volume = source.volume;

        //���� ������� ���ǰ� �����ʴٸ� ���̵�ƿ� �ֱ� 5�ʰ�
        if (source.clip!= bgmType[_type]&& source.clip!=null)
        {
            while (time < 5f)
            {
                source.volume = Mathf.Lerp(volume, 0.0f, time / 5f);
                time += Time.deltaTime;
                yield return null;
            }
            time = 0;
        }
        switch (type)
        {
            case BGMtype.Intro:
                source.clip = bgmType[_type];
                _type = 0;
                break;
            case BGMtype.Town:
                source.clip = bgmType[_type];
                _type = 1;
                break;
            case BGMtype.Dungeon:
                source.clip = bgmType[_type];
                _type = 2;
                break;
            case BGMtype.Boss1:
                source.clip = bgmType[_type];
                _type = 3;
                break;
            case BGMtype.Boss2:
                source.clip = bgmType[_type];
                _type = 4;
                break;
            case BGMtype.Ending:
                source.clip = bgmType[_type];
                _type = 5;
                break;
        }
        source.Play();

        //���� ���� �� 20�ʿ� ���� ������ ���̵��� ȿ��
        while (time<11f)
        {
            source.volume = Mathf.Lerp(startVolume, 0.5f,time/lerpDuration);
            time += Time.deltaTime;
            yield return null;
        }
        bgmChange = false;
    }
}
