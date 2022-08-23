using NewHorizons.External.Modules.VariableSize;
using UnityEngine;

namespace NewHorizons.Components
{
    public class RingOpacityController : MonoBehaviour
    {
        private static readonly int Alpha = Shader.PropertyToID("_Alpha");

        public AnimationCurve opacityCurve { get; protected set; }
        public float CurrentOpacity { get; protected set; }

        private MeshRenderer _meshRenderer;

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

            if (_meshRenderer == null) return;

            _meshRenderer.material.SetFloat(Alpha, CurrentOpacity);
        }

        public void SetOpacityCurve(VariableSizeModule.TimeValuePair[] curve)
        {
            opacityCurve = new AnimationCurve();
            foreach (var pair in curve)
            {
                opacityCurve.AddKey(new Keyframe(pair.time, pair.value));
            }
        }

        public void SetMeshRenderer(MeshRenderer meshRenderer) => _meshRenderer = meshRenderer;
    }
}
