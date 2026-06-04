using System;
using System.Collections.Generic;

namespace Backrooms.Materials
{
    [Serializable]
    public class PrimitiveMaterialLibrary
    {
        public string libraryId;
        public string displayName;
        public List<PrimitiveMaterialProfile> materials = new List<PrimitiveMaterialProfile>();

        public PrimitiveMaterialProfile FindByRole(MaterialRole role)
        {
            if (materials == null)
            {
                return null;
            }

            foreach (PrimitiveMaterialProfile profile in materials)
            {
                if (profile != null && profile.role == role)
                {
                    return profile;
                }
            }

            return null;
        }

        public PrimitiveMaterialProfile FindById(string materialId)
        {
            if (materials == null || string.IsNullOrWhiteSpace(materialId))
            {
                return null;
            }

            foreach (PrimitiveMaterialProfile profile in materials)
            {
                if (profile != null && string.Equals(profile.materialId, materialId, StringComparison.OrdinalIgnoreCase))
                {
                    return profile;
                }
            }

            return null;
        }

        public bool HasRole(MaterialRole role)
        {
            return FindByRole(role) != null;
        }
    }
}
