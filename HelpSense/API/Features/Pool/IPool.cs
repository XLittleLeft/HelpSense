using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSense.API.Features.Pool
{
    public interface IPool<T>
    {
        public T Get();

        public void Return(T obj);
    }
}
