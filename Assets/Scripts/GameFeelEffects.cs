using UnityEngine;

public static class GameFeelEffects
{
    public static void PlayBurst(Vector3 position, Color color, int particleCount)
    {
        GameObject burstObject = new GameObject("Game Feel Burst");
        burstObject.transform.position = position;

        ParticleSystem particles = burstObject.AddComponent<ParticleSystem>();
        ParticleSystem.MainModule main = particles.main;
        main.duration = 0.25f;
        main.loop = false;
        main.playOnAwake = false;
        main.startColor = color;
        main.startLifetime = 0.45f;
        main.startSpeed = 2.4f;
        main.startSize = 0.12f;
        main.maxParticles = Mathf.Clamp(particleCount, 1, 100);

        ParticleSystem.EmissionModule emission = particles.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)main.maxParticles) });

        ParticleSystem.ShapeModule shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.08f;

        ParticleSystemRenderer particleRenderer = particles.GetComponent<ParticleSystemRenderer>();
        particleRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        particleRenderer.receiveShadows = false;

        Shader unlitShader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        if (unlitShader == null)
            unlitShader = Shader.Find("Sprites/Default");

        Material burstMaterial = null;
        if (unlitShader != null)
        {
            burstMaterial = new Material(unlitShader)
            {
                name = "Runtime Game Feel Burst"
            };
            particleRenderer.sharedMaterial = burstMaterial;
        }

        particles.Play();
        float effectLifetime = main.duration + main.startLifetime.constantMax;
        Object.Destroy(burstObject, effectLifetime);
        if (burstMaterial != null)
            Object.Destroy(burstMaterial, effectLifetime);
    }
}
