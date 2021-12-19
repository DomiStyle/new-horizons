![new horizons thumbnail](https://user-images.githubusercontent.com/22628069/146660122-4d1a515c-b795-4781-882d-4080ef837d15.png)

A planet creator for Outer Wilds.

Planets are created using a JSON file format structure, and placed in the `planets` folder.

## How to create your own planets using configs:

Coming soon

There is a template [here](https://github.com/xen-42/ow-new-horizons-config-template) if you want to release your own planet mod using configs. You can learn how the configs work by picking apart the [Real Solar System](https://github.com/xen-42/outer-wilds-real-solar-system) mod or the [New Horizons Examples](https://github.com/xen-42/ow-new-horizons-examples) mod.

## How to use New Horizons in other mods:

First create the following interface in your mod:

```
public interface INewHorizons
{
    void Create(Dictionary<string, object> config);

    void LoadConfigs(IModBehaviour mod);

    GameObject GetPlanet(string name);
}
```

In your main `ModBehaviour` class you can get the NewHorizons API like so:
```
INewHorizons NewHorizonsAPI = ModHelper.Interaction.GetModApi<INewHorizons>("xen.NewHorizons")
```

You can then use the API's `LoadConfigs()` method to load from a "planets" folder, or use the `Create()` and `GetPlanet` methods to create planets and do whatever with them. Just make sure you create planets in the `Start()` method or at least before the SolarSystem scene loads, or they will not be created.

## Credits:
Authors:
- xen (from New Horizons v0.1.0 onwards)
- Mister_Nebula (created original titled Marshmallow)

Marshmallow was made with help from:
- TAImatem
- AmazingAlek
- Raicuparta
- and the Outer Wilds discord server.
