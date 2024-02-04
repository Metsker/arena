using UnityEngine;
using UnityEngine.Rendering;
namespace Arena.__Scripts.Shared.Utils.Extensions
{
    public static class ResourcesUtils {
        public static void LoadVolumeProfile(this Volume volume, string path) {
            var profile = Resources.Load<VolumeProfile>(path);
            volume.profile = profile;
        }
    }
}