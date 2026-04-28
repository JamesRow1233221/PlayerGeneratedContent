using UnityEngine;

public class SFXCar : MonoBehaviour
{
    public Rigidbody carRigidbody;

    public WheelCollider[] wheelColliders;

    public AudioClip engineIdleClip;
    public AudioClip engineHighClip;
    [Range(0.5f, 1.5f)] public float engineMinPitch = 0.7f;
    [Range(1.0f, 3.0f)] public float engineMaxPitch = 1.3f;
    public float maxSpeedForPitch = 40f;
    [Range(0.0f, 1.0f)] public float engineIdleVolume = 0.6f;
    [Range(0.0f, 1.0f)] public float engineHighMaxVolume = 1.0f;

    public AudioClip tireScreechClip;
    [Range(0.0f, 1.0f)] public float tireScreechMaxVolume = 0.9f;
    public float slipThreshold = 0.3f;
    public float screechSpeedFallback = 8f;
    public float screechFadeSpeed = 5f;

    public AudioClip[] collisionClips;
    public float minCollisionForce = 2f;
    public float maxCollisionForce = 20f;
    public float collisionCooldown = 0.3f;

    private AudioSource engineIdleSource;
    private AudioSource engineHighSource;
    private AudioSource tireScreechSource;
    private AudioSource collisionSource;

    private float lastCollisionTime = -999f;

    private void Awake()
    {
        if (carRigidbody == null)
            carRigidbody = GetComponent<Rigidbody>();

        SetupAudioSources();
    }

    private void Update()
    {
        float speed = carRigidbody != null ? carRigidbody.linearVelocity.magnitude : 0f;
        float speedRatio = Mathf.Clamp01(speed / maxSpeedForPitch);

        UpdateEngine(speedRatio);
        UpdateTireScreech(speed);
    }

    private void SetupAudioSources()
    {
        engineIdleSource = CreateAudioSource("Engine_Idle", engineIdleClip, engineIdleVolume, loop: true);
        engineHighSource = CreateAudioSource("Engine_High", engineHighClip, 0f, loop: true);
        tireScreechSource = CreateAudioSource("TireScreech", tireScreechClip, 0f, loop: true);
        collisionSource = CreateAudioSource("Collision", null, 1f, loop: false);

        if (engineIdleSource.clip != null) engineIdleSource.Play();
        if (engineHighSource.clip != null) engineHighSource.Play();
        if (tireScreechSource.clip != null) tireScreechSource.Play();
    }

    private AudioSource CreateAudioSource(string sourceName, AudioClip clip, float volume, bool loop)
    {
        GameObject go = new GameObject(sourceName);
        go.transform.SetParent(transform, false);

        AudioSource src = go.AddComponent<AudioSource>();
        src.clip = clip;
        src.volume = volume;
        src.loop = loop;
        src.playOnAwake = false;
        src.spatialBlend = 1f;  // 3D audio
        src.rolloffMode = AudioRolloffMode.Logarithmic;
        src.minDistance = 2f;
        src.maxDistance = 50f;

        return src;
    }

    private void UpdateEngine(float speedRatio)
    {
        float targetPitch = Mathf.Lerp(engineMinPitch, engineMaxPitch, speedRatio);

        engineIdleSource.pitch = targetPitch;
        engineHighSource.pitch = targetPitch;

        // Cross-fade: idle fades out as high RPM fades in
        engineIdleSource.volume = Mathf.Lerp(engineIdleVolume, engineIdleVolume * 0.2f, speedRatio);
        engineHighSource.volume = Mathf.Lerp(0f, engineHighMaxVolume, speedRatio);
    }

    private void UpdateTireScreech(float speed)
    {
        float targetVolume = 0f;

        if (wheelColliders != null && wheelColliders.Length > 0)
        {
            float maxSlip = 0f;

            foreach (WheelCollider wc in wheelColliders)
            {
                WheelHit hit;
                if (wc.GetGroundHit(out hit))
                {
                    float lateralSlip = Mathf.Abs(hit.sidewaysSlip);
                    float longSlip = Mathf.Abs(hit.forwardSlip);
                    float slip = Mathf.Max(lateralSlip, longSlip * 0.5f);
                    if (slip > maxSlip) maxSlip = slip;
                }
            }

            if (maxSlip > slipThreshold)
            {
                float slipRatio = Mathf.InverseLerp(slipThreshold, slipThreshold + 0.7f, maxSlip);
                targetVolume = Mathf.Lerp(0f, tireScreechMaxVolume, slipRatio);
            }
        }
        else
        {
            // Fallback: approximate via angular velocity
            if (carRigidbody != null)
            {
                float angularY = Mathf.Abs(carRigidbody.angularVelocity.y);
                bool fastTurn = speed > screechSpeedFallback && angularY > 0.3f;
                targetVolume = fastTurn ? tireScreechMaxVolume : 0f;
            }
        }

        tireScreechSource.volume = Mathf.MoveTowards(
            tireScreechSource.volume,
            targetVolume,
            screechFadeSpeed * Time.deltaTime
        );
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collisionClips == null || collisionClips.Length == 0) return;
        if (Time.time - lastCollisionTime < collisionCooldown) return;

        float impulse = collision.impulse.magnitude;
        if (impulse < minCollisionForce) return;

        float volume = Mathf.Clamp01(
            Mathf.InverseLerp(minCollisionForce, maxCollisionForce, impulse)
        );

        AudioClip clip = collisionClips[Random.Range(0, collisionClips.Length)];
        collisionSource.PlayOneShot(clip, volume);
        lastCollisionTime = Time.time;

        Debug.Log($"[CarSFX] Collision with '{collision.gameObject.name}' | Impulse: {impulse:F1} | Volume: {volume:F2}");
    }

    public void SetEngineMute(bool muted)
    {
        engineIdleSource.mute = muted;
        engineHighSource.mute = muted;
    }

    public void SetScreechMute(bool muted)
    {
        tireScreechSource.mute = muted;
    }

    public void StopAll()
    {
        engineIdleSource.Stop();
        engineHighSource.Stop();
        tireScreechSource.Stop();
    }
}


