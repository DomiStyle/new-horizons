using NewHorizons.External.Modules.VariableSize;
using NewHorizons.Utility;
using UnityEngine;

namespace NewHorizons.Components
{
    public class RingOpacityController : MonoBehaviour
    {
        private static readonly int Alpha = Shader.PropertyToID("_Alpha");

        public AnimationCurve opacityCurve { get; protected set; }
        public float CurrentOpacity { get; protected set; }

        private MeshRenderer _meshRenderer;
        private RingFluidVolume _ringFluidVolume;

        protected void FixedUpdate()
        {
            if (opacityCurve != null)
            {
                CurrentOpacity = opacityCurve.Evaluate(TimeLoop.GetMinutesElapsed());
            }
            else
            {
                CurrentOpacity = 1;
            }

            if (_ringFluidVolume != null)
            {
                if (Mathf.Approximately(CurrentOpacity, 0) && _ringFluidVolume.IsVolumeActive()) _ringFluidVolume.SetVolumeActivation(false);
                else if (!_ringFluidVolume.IsVolumeActive()) _ringFluidVolume.SetVolumeActivation(true);
            }

            if (_meshRenderer == null) return;

            _meshRenderer.material.SetFloat(Alpha, CurrentOpacity);
        }

        public void SetOpacityCurve(TimeValuePair[] curve)
        {
            opacityCurve = new AnimationCurve();
            foreach (var pair in curve)
            {
                opacityCurve.AddKey(new Keyframe(pair.time, pair.value));
            }
        }

        public void SetMeshRenderer(MeshRenderer meshRenderer) => _meshRenderer = meshRenderer;
        public void SetRingFluidVolume(RingFluidVolume ringFluidVolume) => _ringFluidVolume = ringFluidVolume;
    }
}
