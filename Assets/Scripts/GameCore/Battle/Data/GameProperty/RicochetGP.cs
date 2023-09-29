using System;

namespace Battle.Data.GameProperty
{
    public struct RicochetData
    {
        public int decreasePercent;
        public decimal radius;
        public int ricochetCount;

        public static implicit operator float(RicochetData data)
        {
            var newBinary = 1_000_000_00;
            newBinary += data.decreasePercent * 100000;
            newBinary += ((int)(data.radius * 10)) * 100;
            newBinary += data.ricochetCount;
            return newBinary;
        }

        public static RicochetData operator +(RicochetData a, RicochetData b)
        {
            a.decreasePercent = Math.Max(a.decreasePercent, b.decreasePercent) - Math.Min(a.decreasePercent, b.decreasePercent);
            a.radius += b.radius;
            a.ricochetCount += b.ricochetCount;

            return a;
        }

        public static RicochetData Get(float value)
        {
            var val = (int)value;
            var data = new RicochetData()
            {
                decreasePercent = val / 100000 % 1000,
                radius = (val / 100 % 1000) / 10m,
                ricochetCount = val % 100
            };

            return data;
        }
    }
    
    [Serializable]
    public class RicochetGP : BaseGameProperty
    {
        protected override float ComputeValue(float val)
        {
            var data = RicochetData.Get(val);
            var valData = RicochetData.Get(value);

            data += valData;
            
            return data;
        }
    }
}