using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PixelDough.Bouncer
{
    public class CollectableController : MonoBehaviour
    {
        [SerializeField] private Transform modelHolder;
        [SerializeField] private ParticleSystem collectParticleSystem;

        private float _randomAnimateOffset = 0f;
        private float _startOffsetY = 0f;

        private void Start()
        {
            _randomAnimateOffset = Random.Range(0f, 360f);
            _startOffsetY = modelHolder.transform.localPosition.y;
            modelHolder.transform.Rotate(transform.up, _randomAnimateOffset);
        }

        private void Update()
        {
            modelHolder.transform.Rotate(transform.up, 180f * Time.deltaTime);
            modelHolder.transform.localPosition = new Vector3(0f,
                _startOffsetY + Mathf.Sin(_randomAnimateOffset + Time.time * 2f) / 10f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.attachedRigidbody) return;
            if (other.attachedRigidbody.CompareTag("Player"))
            {
                collectParticleSystem.transform.parent = null;
                collectParticleSystem.Play();
                gameObject.SetActive(false);
            }
        }
    }
}
