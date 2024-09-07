using System;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace PixelDough.Bouncer
{
    public class CollectableController : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private Transform modelHolder;
        [SerializeField] private CollectableAttractor collectableAttractor;
        [SerializeField] private ParticleSystem collectParticleSystemPrefab;

        private static ParticleSystem _collectParticleSystemInstance;
        
        [Range(1, 10)]
        [SerializeField] private int count = 1;
        public int Count => count;
        
        private float _randomAnimateOffset = 0f;
        private float _startOffsetY = 0f;

        private Vector3 _localPosition;

        public enum CollectedStates
        {
            None,
            Held,
            LockedIn
        }
        public CollectedStates collectedState = CollectedStates.None;

        private void Start()
        {
            _localPosition = transform.localPosition;
            
            _randomAnimateOffset = Random.Range(0f, 360f);
            _startOffsetY = modelHolder.transform.localPosition.y;
            modelHolder.transform.Rotate(Vector3.up, _randomAnimateOffset);

            if (!_collectParticleSystemInstance)
                _collectParticleSystemInstance = Instantiate(collectParticleSystemPrefab);
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
                // LevelManager.Instance.LevelProgress.AddCollectable(this);
                _collectParticleSystemInstance.transform.position = transform.position;
                _collectParticleSystemInstance.Play();
                gameObject.SetActive(false);
            }
        }

        public void Activate()
        {
            transform.localPosition = _localPosition;
            gameObject.SetActive(true);
            collectedState = CollectedStates.None;
            collectableAttractor.ResetValues();
        }

        public void SetMesh(Mesh mesh, Material material)
        {
            meshFilter.mesh = mesh;
            meshRenderer.material = material;
        }
    }
}
