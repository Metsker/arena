using __Scripts.Assemblies.Utilities.Extensions;
using Generated;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Arena.__Scripts.Bootstrap
{
    public class Bootstrapper : MonoBehaviour
    {
        [SerializeField] private ScenesInBuild nextScene;

        private void Start()
        {
            SceneManager.LoadScene(nextScene.ToInt());
        }
    }
}
