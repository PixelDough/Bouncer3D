using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PixelDough.Bouncer
{
    public class CollectableController : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private Transform modelHolder;
        [SerializeField] private ParticleSystem collectParticleSystem;

        [Range(1, 10)]
        [SerializeField] private int count = 1;
        public int Count => count;
        
        private float _randomAnimateOffset = 0f;
        private float _startOffsetY = 0f;

        private void Start()
        {
            _randomAnimateOffset = Random.Range(0f, 360f);
            _startOffsetY = modelHolder.transform.localPosition.y;
            modelHolder.transform.Rotate(Vector3.up, _randomAnimateOffset);
        }

        private void Update()
        {
            modelHolder.transform.Rotate(Vector3.up, 180f * Time.deltaTime);
            modelHolder.transform.localPosition = new Vector3(0f,
                _startOffsetY + Mathf.Sin(_randomAnimateOffset + Time.time * 2f) / 10f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.attachedRigidbody) return;
            if (other.attachedRigidbody.CompareTag("Player"))
            {
                PlayerController playerController = other.attachedRigidbody.GetComponent<PlayerController>();
                playerController.CollectShells(count);
                collectParticleSystem.transform.parent = null;
                collectParticleSystem.Play();
                gameObject.SetActive(false);
            }
        }

        public void SetMesh(Mesh mesh, Material material)
        {
            meshFilter.mesh = mesh;
            meshRenderer.material = material;
        }
    }
}
