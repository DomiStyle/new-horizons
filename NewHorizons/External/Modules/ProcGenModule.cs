﻿using System.ComponentModel.DataAnnotations;
using NewHorizons.Utility;
using Newtonsoft.Json;

namespace NewHorizons.External.Modules
{
    [JsonObject]
    public class ProcGenModule
    {
        public MColor color;

        [Range(0, double.MaxValue)] public float scale;
    }
}