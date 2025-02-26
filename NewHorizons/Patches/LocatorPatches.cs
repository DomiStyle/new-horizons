using HarmonyLib;
using NewHorizons.Utility;

namespace NewHorizons.Patches
{
    [HarmonyPatch]
    public static class LocatorPatches
    {
        public static AstroObject _attlerock;
        public static AstroObject _eye;
        public static AstroObject _focal;
        public static AstroObject _hollowsLantern;
        public static AstroObject _mapSatellite;
        public static AstroObject _sunStation;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Locator), nameof(Locator.RegisterCloakFieldController))]
        public static bool Locator_RegisterCloakFieldController()
        {
            return Locator._cloakFieldController == null;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CloakFieldController), nameof(CloakFieldController.isPlayerInsideCloak), MethodType.Getter)]
        public static void CloakFieldController_isPlayerInsideCloak(CloakFieldController __instance, ref bool __result)
        {
            __result = __result || Components.CloakSectorController.isPlayerInside;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CloakFieldController), nameof(CloakFieldController.isProbeInsideCloak), MethodType.Getter)]
        public static void CloakFieldController_isProbeInsideCloak(CloakFieldController __instance, ref bool __result)
        {
            __result = __result || Components.CloakSectorController.isProbeInside;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CloakFieldController), nameof(CloakFieldController.isShipInsideCloak), MethodType.Getter)]
        public static void CloakFieldController_isShipInsideCloak(CloakFieldController __instance, ref bool __result)
        {
            __result = __result || Components.CloakSectorController.isShipInside;
        }

        // Locator Fixes
        // Vanilla doesn't register these AstroObjects for some reason. So here is a fix.

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Locator), nameof(Locator.GetAstroObject))]
        public static bool Locator_GetAstroObject(AstroObject.Name astroObjectName, ref AstroObject __result)
        {
            switch (astroObjectName)
            {
                case AstroObject.Name.Eye:
                    __result = _eye;
                    return false;
                case AstroObject.Name.HourglassTwins:
                    __result = _focal;
                    return false;
                case AstroObject.Name.MapSatellite:
                    __result = _mapSatellite;
                    return false;
                case AstroObject.Name.SunStation:
                    __result = _sunStation;
                    return false;
                case AstroObject.Name.TimberMoon:
                    __result = _attlerock;
                    return false;
                case AstroObject.Name.VolcanicMoon:
                    __result = _hollowsLantern;
                    return false;
                default:
                    break;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Locator), nameof(Locator.RegisterAstroObject))]
        public static bool Locator_RegisterAstroObject(AstroObject astroObject)
        {
            if (astroObject.GetAstroObjectName() == AstroObject.Name.None) return false;

            // Sun Station name change because for some dumb reason it doesn't use AstroObject.Name.SunStation
            if (!string.IsNullOrEmpty(astroObject._customName) && astroObject._customName.Equals("Sun Station"))
            {
                if (astroObject.gameObject.name == "SunStation_Body")
                {
                    astroObject._name = AstroObject.Name.SunStation;
                    _sunStation = astroObject;
                    return false;
                }
                // Debris uses same custom name because morbius, so let us change that.
                else if (astroObject.gameObject.name == "SS_Debris_Body") astroObject._customName = "Sun Station Debris";
                return true;
            }

            switch (astroObject.GetAstroObjectName())
            {
                case AstroObject.Name.Eye:
                    _eye = astroObject;
                    return false;
                case AstroObject.Name.HourglassTwins:
                    _focal = astroObject;
                    return false;
                case AstroObject.Name.MapSatellite:
                    _mapSatellite = astroObject;
                    return false;
                case AstroObject.Name.SunStation:
                    _sunStation = astroObject;
                    return false;
                case AstroObject.Name.TimberMoon:
                    _attlerock = astroObject;
                    return false;
                case AstroObject.Name.VolcanicMoon:
                    _hollowsLantern = astroObject;
                    return false;
                default:
                    break;
            }
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Locator), nameof(Locator.ClearReferences))]
        public static void Locator_ClearReferences()
        {
            _attlerock = null;
            _eye = null;
            _focal = null;
            _hollowsLantern = null;
            _mapSatellite = null;
            _sunStation = null;
        }
    }
}
