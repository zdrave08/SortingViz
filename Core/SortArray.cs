using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortingViz.Core
{
    public abstract class SortArray
    {
        protected int[]? values;
        public int Length => values?.Length ?? 0;
        public void Shuffle()
        {
            var rnd = new Random();

            if(values == null || values.Length <= 1) return;

            for (int i = values.Length - 1; i > 0; i--) 
            { 
                int j = rnd.Next(i + 1); 
                (values[i], values[j]) = (values[j], values[i]); 
            }
        }
        public abstract void SetActive(int index);
        public abstract void SetInactive(int index);
        public abstract void SetDone(int index);
        public abstract void SetActive(int start, int end);
        public abstract void SetInactive(int start, int end);
        public abstract void SetDone(int start, int end);
    }
}
