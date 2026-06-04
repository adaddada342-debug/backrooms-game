using System;
using UnityEngine;

namespace Backrooms.Materials
{
    [Serializable]
    public class PrimitiveMaterialProfile
    {
        public string materialId;
        public string displayName;
        public MaterialRole role;
        public Color baseColor;
        public Color emissionColor;
        public float smoothness;
        public float metallic;
        public float emissionIntensity;
        public bool transparent;
        public float alpha;
        public string notes;
    }
}
