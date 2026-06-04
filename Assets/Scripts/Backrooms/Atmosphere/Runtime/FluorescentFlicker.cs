using System;
using UnityEngine;

namespace Backrooms.Atmosphere.Runtime
{
    public class FluorescentFlicker : MonoBehaviour
    {
        public Light targetLight;
        public Renderer targetRenderer;
        public float baseIntensity = 1.2f;
        public float flickerChancePerSecond = 0.08f;
        public float flickerIntensityDrop = 0.35f;
        public float flickerDuration = 0.06f;
        public bool deterministic = false;
        public int seed = 0;

        private System.Random random;
        private float flickerTimer;
        private Material targetMaterial;

        private void Awake()
        {
            if (targetLight == null)
            {
                targetLight = GetComponent<Light>();
            }

            if (targetRenderer == null)
            {
                targetRenderer = GetComponent<Renderer>();
            }

            if (targetRenderer != null)
            {
                targetMaterial = targetRenderer.material;
            }

            random = deterministic ? new System.Random(seed) : null;
            if (targetLight != null)
            {
                baseIntensity = targetLight.intensity;
            }
        }

        private void Update()
        {
            if (targetLight == null && targetRenderer == null)
            {
                return;
            }

            if (flickerTimer > 0f)
            {
                flickerTimer -= Time.deltaTime;
                ApplyIntensity(baseIntensity * Mathf.Clamp01(1f - flickerIntensityDrop));
                return;
            }

            ApplyIntensity(baseIntensity);
            float chanceThisFrame = Mathf.Max(0f, flickerChancePerSecond) * Time.deltaTime;
            if (NextDouble() < chanceThisFrame)
            {
                flickerTimer = Mathf.Max(0.01f, flickerDuration);
            }
        }

        private void ApplyIntensity(float intensity)
        {
            if (targetLight != null)
            {
                targetLight.intensity = intensity;
            }

            if (targetMaterial != null && targetMaterial.HasProperty("_EmissionColor"))
            {
                targetMaterial.SetColor("_EmissionColor", new Color(0.75f, 1f, 0.72f) * Mathf.Max(0f, intensity));
            }
        }

        private double NextDouble()
        {
            return deterministic && random != null ? random.NextDouble() : UnityEngine.Random.value;
        }
    }
}
