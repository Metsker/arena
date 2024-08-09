using Assemblies.Utilities.Extensions;
using Generated;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tower.Bootstrap
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
