using System;
using System.Collections.Generic;
using UnityEngine;

namespace UTalk.Data
{
    public enum EActorType
    {
        Player,
        NPC
    }

    [Serializable]
    public class Actor
    {
        public int Id = 0;
        public string Name = "";
        public Sprite Image = null;
        public EActorType ActorType = EActorType.NPC;
    }
}
