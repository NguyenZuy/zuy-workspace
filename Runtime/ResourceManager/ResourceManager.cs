using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace Zuy.Workspace.Resource
{
    public class ResourceManager : Base.BaseSingleton<ResourceManager>
    {
        [Header("Sprite Atlases")]
        public List<SpriteAtlas> atlas;
    }
}
