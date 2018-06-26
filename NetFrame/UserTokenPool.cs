using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetFrame
{
    public class UserTokenPool
    {
        private Stack<UserToken> pool;

        public UserTokenPool(int max)
        {
            pool = new Stack<UserToken>(max);
        }
        public UserToken pop()
        {
            return pool.Pop();
        }
        public void push(UserToken token)
        {
            if (token != null)
                pool.Push(token);
        }
        public int Size
        {
            get { return pool.Count; }
        }
    }
}
