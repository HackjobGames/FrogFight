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

        private void Start()
        {
            FractureGameobject();
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }

        public void FractureGameobject()
        {
            Fracture.FractureGameObject(
                gameObject,
                4,
                chunks,
                insideMaterial,
                outsideMaterial,
                density,
                effect
            );
        }
    }
}