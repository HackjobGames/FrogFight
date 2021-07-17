using UnityEngine;
using Random = System.Random;

namespace Project.Scripts.Fractures
{
    public class FractureThis : MonoBehaviour
    {
        [SerializeField] private int chunks = 500;
        [SerializeField] private float density = 50;
            
        [SerializeField] private Material insideMaterial;
        [SerializeField] private Material outsideMaterial;

        [SerializeField] private GameObject effect;

        private Random rng = new Random();

        private void Start()
        {
            FractureGameobject();
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }

        public void FractureGameobject()
        {
            var seed = rng.Next();
            Fracture.FractureGameObject(
                gameObject,
                seed,
                chunks,
                insideMaterial,
                outsideMaterial,
                density,
                effect
            );
        }
    }
}