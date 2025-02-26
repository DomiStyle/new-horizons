using NewHorizons.Builder.Atmosphere;
using System.Collections.Generic;
using UnityEngine;
using Logger = NewHorizons.Utility.Logger;
namespace NewHorizons.Components
{
    [RequireComponent(typeof(SunLightController))]
    [RequireComponent(typeof(SunLightParamUpdater))]
    public class StarLightController : MonoBehaviour
    {
        private static readonly int SunIntensity = Shader.PropertyToID("_SunIntensity");
        private static readonly float hearthSunDistanceSqr = 8593 * 8593;

        public static StarLightController Instance { get; private set; }

        private List<StarController> _stars = new List<StarController>();
        private StarController _activeStar;

        private SunLightController _sunLightController;
        private SunLightParamUpdater _sunLightParamUpdater;

        public void Awake()
        {
            Instance = this;
            _sunLightController = GetComponent<SunLightController>();
            _sunLightController.enabled = true;
            _sunLightParamUpdater = GetComponent<SunLightParamUpdater>();
            _sunLightParamUpdater._sunLightController = _sunLightController;
        }

        public static void AddStar(StarController star)
        {
            if (star == null) return;

            Logger.LogVerbose($"Adding new star to list: {star.gameObject.name}");
            Instance._stars.Add(star);
        }

        public static void RemoveStar(StarController star)
        {
            Logger.LogVerbose($"Removing star from list: {star?.gameObject?.name}");
            if (Instance._stars.Contains(star))
            {
                if (Instance._activeStar != null && Instance._activeStar.Equals(star))
                {
                    Instance._stars.Remove(star);
                    if (Instance._stars.Count > 0) Instance.ChangeActiveStar(Instance._stars[0]);
                }
                else
                {
                    Instance._stars.Remove(star);
                }
            }
        }

        public void Update()
        {
            if (_activeStar == null || !_activeStar.gameObject.activeInHierarchy)
            {
                if (_stars.Contains(_activeStar)) _stars.Remove(_activeStar);
                if (_stars.Count > 0) ChangeActiveStar(_stars[0]);
                else gameObject.SetActive(false);

                foreach (var (_, material) in AtmosphereBuilder.Skys)
                {
                    material.SetFloat(SunIntensity, 0);
                }

                return;
            }

            // Update atmo shaders
            foreach (var (planet, material) in AtmosphereBuilder.Skys)
            {
                var sqrDist = (planet.transform.position - _activeStar.transform.position).sqrMagnitude;
                var intensity = Mathf.Min(_activeStar.Intensity / (sqrDist / hearthSunDistanceSqr), 1f);

                material.SetFloat(SunIntensity, intensity);
            }

            foreach (var star in _stars)
            {
                if (star == null) continue;
                if (!(star.gameObject.activeSelf && star.gameObject.activeInHierarchy)) continue;

                // Player is always at 0,0,0 more or less so if they arent using the map camera then wtv
                var origin = Vector3.zero;
                if (PlayerState.InMapView())
                {
                    origin = Locator.GetActiveCamera().transform.position;
                }

                if (star.Intensity * (star.transform.position - origin).sqrMagnitude < _activeStar.Intensity * (_activeStar.transform.position - origin).sqrMagnitude)
                {
                    ChangeActiveStar(star);
                    break;
                }
            }
        }

        private void ChangeActiveStar(StarController star)
        {
            if (_sunLightController == null || _sunLightParamUpdater == null) return;

            if (_activeStar != null) _activeStar.Disable();

            Logger.LogVerbose($"Switching active star: {star.gameObject.name}");

            _activeStar = star;

            star.Enable();

            _sunLightController._sunBaseColor = star.SunColor;
            _sunLightController._sunBaseIntensity = star.Intensity;
            _sunLightController._sunLight = star.Light;
            _sunLightController._ambientLight = star.AmbientLight;

            _sunLightParamUpdater.sunLight = star.Light;
            _sunLightParamUpdater._sunController = star.transform.GetComponent<SunController>();
            _sunLightParamUpdater._propID_SunPosition = Shader.PropertyToID("_SunPosition");
            _sunLightParamUpdater._propID_OWSunPositionRange = Shader.PropertyToID("_OWSunPositionRange");
            _sunLightParamUpdater._propID_OWSunColorIntensity = Shader.PropertyToID("_OWSunColorIntensity");

            // For the param thing to work it wants this to be on the star idk
            this.transform.parent = star.transform;
            this.transform.localPosition = Vector3.zero;
        }
    }
}
