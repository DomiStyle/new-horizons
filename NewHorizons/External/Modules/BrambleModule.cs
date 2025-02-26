using NewHorizons.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewHorizons.External.Modules
{
    
    [JsonObject]
    public class BrambleModule
    {
        /// <summary>
        /// Defining this value will make this body a bramble dimension. Leave it null to not do that.
        /// </summary>
        public BrambleDimensionInfo dimension;

        /// <summary>
        /// Place nodes/seeds that take you to other bramble dimensions
        /// </summary>
        public BrambleNodeInfo[] nodes;

        
        [JsonObject]
        public class BrambleDimensionInfo
        {
            /// <summary>
            /// The color of the fog inside this dimension. 
            /// Leave blank for the default grayish color: (84, 83, 73)
            /// </summary>
            public MColor fogTint;

            /// <summary>
            /// The density of the fog inside this dimension. The default is 6.
            /// </summary>
            [DefaultValue(6f)] public float fogDensity = 6f;

            /// <summary>
            /// The name of the *node* that the player is taken to when exiting this dimension.
            /// </summary>
            public string linksTo;

            /// <summary>
            /// The internal radius (in meters) of the dimension. 
            /// The default is 750 for the Hub, Escape Pod, and Angler Nest dimensions, and 500 for the others.
            /// </summary>
            [DefaultValue(750f)] public float radius = 750f;

            /// <summary>
            /// An array of integers from 0-5. By default, all entrances are allowed. To force this dimension to warp players in from only one point (like the anglerfish nest dimension in the base game) set this value to [3], [5], or similar. Values of 0-5 only.
            /// </summary>
            public int[] allowedEntrances;
        }

        
        [JsonObject]
        public class BrambleNodeInfo
        {
            /// <summary>
            /// The physical position of the node
            /// </summary>
            public MVector3 position;
            
            /// <summary>
            /// The physical rotation of the node
            /// </summary>
            public MVector3 rotation;

            /// <summary>
            /// The physical scale of the node, as a multiplier of the original size. 
            /// Nodes are 150m across, seeds are 10m across.
            /// </summary>
            [DefaultValue(1f)] public float scale = 1f;

            /// <summary>
            /// The name of the planet that hosts the dimension this node links to
            /// </summary>
            public string linksTo;

            /// <summary>
            /// The name of this node. Only required if this node should serve as an exit.
            /// </summary>
            public string name;

            /// <summary>
            /// Set this to true to make this node a seed instead of a node the player can enter
            /// </summary>
            [DefaultValue(false)] public bool isSeed = false;

            /// <summary>
            /// The color of the fog inside the node. 
            /// Leave blank for the default yellowish white color: (255, 245, 217, 255)
            /// </summary>
            public MColor fogTint;

            /// <summary>
            /// The color of the light from the node. Alpha controls brightness.
            /// Leave blank for the default white color.
            /// </summary>
            public MColor lightTint;

            /// <summary>
            /// Should this node have a point of light from afar? 
            /// By default, nodes will have a foglight, while seeds won't, and neither will if not in a dimension.
            /// </summary>
            public bool? hasFogLight;

            /// <summary>
            /// An array of integers from 0-5. By default, all exits are allowed. To force this node to warp players out from only one hole set this value to [3], [5], or similar. Values of 0-5 only.
            /// </summary>
            public int[] possibleExits;

            #region Obsolete

            [Obsolete("farFogTint is deprecated, please use fogTint instead")]
            public MColor farFogTint;

            #endregion Obsolete
        }
    }
}
