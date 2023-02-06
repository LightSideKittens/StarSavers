using System;

namespace Battle.MusicEventSystem.Soundvent
{
    public class SoundventAttribute : Attribute
    {
        public string groupName;
        public bool isShort;

        public SoundventAttribute(bool isShort, string groupName)
        {
            this.isShort = isShort;
            this.groupName = groupName;
        }
    }
}