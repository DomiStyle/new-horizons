using NewHorizons.External;
using NewHorizons.External.Modules;
using UnityEngine;

namespace NewHorizons.Builder
{
    internal interface IProxyBuilder<T> where T : Module
    {
        public abstract GameObject MakeProxy(GameObject root, T module, BaseModule baseModule);
    }
}
