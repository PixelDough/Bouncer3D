using System;
using DG.Tweening;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class Medal : MonoBehaviour
    {
        private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");
        [SerializeField] private MeshRenderer noneMeshRenderer;
        [SerializeField] private MeshRenderer medalMeshRenderer;

        public enum MedalState { None = 0, Bronze = 1, Silver = 2, Gold = 3, Platinum = 4 }
        private readonly float[] _medalOffsets = {0f, -0.75f, -0.5f, -0.25f, 0f};
        
        private void Start()
        {
            noneMeshRenderer.enabled = true;
            medalMeshRenderer.enabled = false;
        }

        private void Update()
        {
            transform.localEulerAngles = new Vector3(Mathf.Sin(Time.time * 2f) * 15, Mathf.Cos(Time.time * 2f) * 15, 0f);
        }

        public void SetMedalState(MedalState medalState)
        {
            if (medalState == MedalState.None)
            {
                medalMeshRenderer.enabled = false;
                noneMeshRenderer.enabled = true;
            }
            else
            {
                medalMeshRenderer.enabled = true;
                noneMeshRenderer.enabled = false;

                medalMeshRenderer.material.SetTextureOffset(BaseMap,
                    new Vector2(_medalOffsets[(int)medalState], medalMeshRenderer.material.GetTextureOffset(BaseMap).y));

                transform.localScale = Vector3.one;
                transform.DOPunchScale(Vector3.one * -0.25f, 0.5f, 6);
            }
        }
    }
}
