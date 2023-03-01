using System;

namespace Battle.Data.GameProperty
{
    public struct RicochetData
    {
        private int percentValue;
        private decimal radius;
        private int ricochet;

        public static implicit operator decimal(RicochetData data)
        {
            var newBinary = 1_000_000_00;
            newBinary += data.percentValue * 100000;
            newBinary += ((int)(data.radius * 10)) * 100;
            newBinary += data.ricochet;
            return newBinary;
        }

        public static RicochetData operator +(RicochetData a, RicochetData b)
        {
            a.percentValue = Math.Max(a.percentValue, b.percentValue) - Math.Min(a.percentValue, b.percentValue);
            a.radius += b.radius;
            a.ricochet += b.ricochet;

            return a;
        }

        public static RicochetData Get(decimal value)
        {
            var val = (int)value;
            var data = new RicochetData()
            {
                percentValue = val / 100000 % 1000,
                radius = (val / 100 % 1000) / 10m,
                ricochet = val % 100
            };

            return data;
        }
    }
    
    [Serializable]
    public class RicochetGP : BaseGameProperty
    {
        public override decimal ComputeValue(decimal val)
        {
            var data = RicochetData.Get(val);
            var valData = RicochetData.Get(value);

            data += valData;
            
            return data;
        }
    }
}