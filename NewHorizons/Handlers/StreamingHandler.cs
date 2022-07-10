﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Logger = NewHorizons.Utility.Logger;

namespace NewHorizons.Handlers
{
    /// <summary>
    /// handles streaming meshes so they stay loaded
    /// </summary>
    public static class StreamingHandler
    {
        private static readonly Dictionary<Material, string> _materialCache = new();
        private static readonly Dictionary<GameObject, string[]> _objectCache = new();

        public static void Init()
        {
            _materialCache.Clear();
            _objectCache.Clear();
        }

        /// <summary>
        /// makes it so that this object's streaming stuff will be connected to the given sector
        /// </summary>
        public static void SetUpStreaming(GameObject obj, Sector sector)
        {
            // find the asset bundles to load
            // tries the cache first, then builds
            if (!_objectCache.TryGetValue(obj, out var assetBundles))
            {
                var assetBundlesList = new List<string>();

                var tables = Resources.FindObjectsOfTypeAll<StreamingMaterialTable>();
                foreach (var streamingHandle in obj.GetComponentsInChildren<StreamingMeshHandle>())
                {
                    var assetBundle = streamingHandle.assetBundle;
                    assetBundlesList.SafeAdd(assetBundle);

                    if (streamingHandle is StreamingRenderMeshHandle or StreamingSkinnedMeshHandle)
                    {
                        var materials = streamingHandle.GetComponent<Renderer>().sharedMaterials;

                        if (materials.Length == 0) continue;

                        // Gonna assume that if theres more than one material its probably in the same asset bundle anyway right
                        if (_materialCache.TryGetValue(materials[0], out assetBundle))
                        {
                            assetBundlesList.SafeAdd(assetBundle);
                        }
                        else
                        {
                            foreach (var table in tables)
                            {
                                foreach (var lookup in table._materialPropertyLookups)
                                {
                                    if (materials.Contains(lookup.material))
                                    {
                                        _materialCache.SafeAdd(lookup.material, table.assetBundle);
                                        assetBundlesList.SafeAdd(table.assetBundle);
                                    }
                                }
                            }
                        }
                    }
                }

                assetBundles = assetBundlesList.ToArray();
                _objectCache[obj] = assetBundles;
            }

            if (sector)
            {
                // load it if ur already in the sector
                if (sector.ContainsAnyOccupants(DynamicOccupant.Player | DynamicOccupant.Probe))
                {
                    foreach (var assetBundle in assetBundles)
                    {
                        Logger.LogVerbose($"loading bundle {assetBundle}");
                        StreamingManager.LoadStreamingAssets(assetBundle);
                    }
                }

                sector.OnSectorOccupantsUpdated += () =>
                {
                    if (sector.ContainsAnyOccupants(DynamicOccupant.Player | DynamicOccupant.Probe))
                    {
                        foreach (var assetBundle in assetBundles)
                        {
                            Logger.LogVerbose($"loading bundle {assetBundle}");
                            StreamingManager.LoadStreamingAssets(assetBundle);
                        }
                    }
                    else
                    {
                        foreach (var assetBundle in assetBundles)
                        {
                            Logger.LogVerbose($"UNloading bundle {assetBundle}");
                            StreamingManager.UnloadStreamingAssets(assetBundle);
                        }
                    }
                };
            }
            else
            {
                // just load it immediately and hope for the best
                foreach (var assetBundle in assetBundles)
                {
                    Logger.LogVerbose($"loading bundle {assetBundle}");
                    StreamingManager.LoadStreamingAssets(assetBundle);
                }
            }
        }
    }
}