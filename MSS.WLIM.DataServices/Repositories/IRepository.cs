using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSS.WLIM.DataServices.Repositories
{
    public interface IRepository<T>
    {
        public Task<IEnumerable<T>> GetAll();
        public Task<T> Get(string Id);
        public Task<T> Create(T _object);
        public Task<T> Update(T _object);
        public Task<bool> Delete(string Id);
    }
}
