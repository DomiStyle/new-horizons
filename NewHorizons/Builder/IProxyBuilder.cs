using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewHorizons.External;
using UnityEngine;

namespace NewHorizons.Builder
{
    internal interface IProxyBuilder<T> where T : Module
    {
        public abstract (GameObject gameObject, Component component) MakeProxy(GameObject root, Sector sector, T module);
    }
}
