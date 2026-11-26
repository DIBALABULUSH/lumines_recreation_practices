using System.Collections.Generic;

namespace Lumines
{
    public class ObjectPool<T>
    {
        private readonly Queue<T> _objQueue;

        public ObjectPool()
        {
            _objQueue = new Queue<T>();
        }

        public bool Recycle(T obj)
        {
            _objQueue.Enqueue(obj);
            return true;
        }

        public T GetObj()
        {
            return _objQueue.Count > 0 ? _objQueue.Dequeue() : default;
        }
        
    }
}