using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsControl : MonoBehaviour
{
        
    [SerializeField] private GameObject fastEffect;
    [SerializeField] private GameObject walkSmoke;
    [SerializeField] private GameObject jumpEffect;
    private AudioSource jumpSound;
        
    [SerializeField] private ParticleSystem landEffect;
        
    private PlayerControl pc;
    private Animator _animator;
    private Rigidbody rb;
    private GameManager gm;

    private float landEffectCooldown;
            
    private WaitForSeconds ZeroOne = new WaitForSeconds(0.1f);

    // Start is called before the first frame update
    void Start()
    {
        pc = transform.parent.GetComponent<PlayerControl>();
        gm = GameManager.Instance;

        
        landEffect.gameObject.SetActive(true);
        jumpEffect.SetActive(true);
        jumpSound = jumpEffect.GetComponent<AudioSource>();

        fastEffect.SetActive(false);
        walkSmoke.SetActive(false);
    }

    private void Update()
    {
        if (landEffectCooldown > 0) landEffectCooldown -= Time.deltaTime;
    }


    public void SetData(Animator a, Rigidbody r)
    {
        _animator = a;
        rb = r;      
    }
    
    
    public void MakeJumpFX()
    {
        jumpSound.Play();
    }

    public void WalkSmoke(bool isActive) => walkSmoke.SetActive(isActive);

    public void MakeFastEffect(float duration)
    {        
        StartCoroutine(playEffectBreakable(duration, fastEffect));
    }


    
    public void MakeLandEffect()
    {
        if (landEffectCooldown > 0) return;

        landEffectCooldown = 1;

        landEffect.Play();
    }

    
    private IEnumerator playEffect(float duration, GameObject fx)
    {
        fx.SetActive(false);
        fx.SetActive(true);
        yield return new WaitForSeconds(duration);
        fx.SetActive(false);
    }

    private IEnumerator playEffectBreakable(float duration, GameObject fx)
    {
        fx.SetActive(false);
        fx.SetActive(true);
        for (float i = 0; i < duration; i += 0.1f)
        {
            yield return ZeroOne;
            if (pc.IsDead) break;
        }
        fx.SetActive(false);
    }

}
