using System.Text;
using BasePools = NorthwoodLib.Pools;

namespace HelpSense.API.Features.Pool
{

    public class StringBuilderPool : IPool<StringBuilder>
    {
        private StringBuilderPool()
        {
        }

        public static StringBuilderPool Pool { get; } = new();

        public StringBuilder Get() => BasePools.StringBuilderPool.Shared.Rent();

        public StringBuilder Get(int capacity) => BasePools.StringBuilderPool.Shared.Rent(capacity);

        public void Return(StringBuilder obj) => BasePools.StringBuilderPool.Shared.Return(obj);

        public string ToStringReturn(StringBuilder obj)
        {
            string s = obj.ToString();

            Return(obj);

            return s;
        }
    }
}