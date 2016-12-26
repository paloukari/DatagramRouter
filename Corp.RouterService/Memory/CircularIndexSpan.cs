using System.Collections.Generic;

namespace Corp.RouterService.Memory
{
    class CircularIndexSpan : IEnumerable<KeyValuePair<int, int>>
    {
        //index and size
        private Dictionary<int, int> _spans;
        private int _span;
        private int _max;
        #region ctors

        private CircularIndexSpan()
        {

        }
        
        internal CircularIndexSpan(CircularIndex start, int span)
        {
            int startIndex = start.Index;
            int endIndex = (startIndex + span) % start.Max;

            _span = span;
            _max = start.Max;
            _spans = new Dictionary<int, int>();

            if (startIndex < endIndex)
            {
                _spans.Add(startIndex, _span);
            }
            else
            {                
                if (_max - startIndex != 0)
                    _spans.Add(startIndex, _max - startIndex);
                if (_span - (_max - startIndex) != 0)
                    _spans.Add(0, span - (_max - startIndex));
            }
        }
        #endregion

        public static implicit operator int(CircularIndexSpan c)
        {
            return c._span;
        }
        #region IEnumerable<KeyValuePair<int,int>> Members

        public IEnumerator<KeyValuePair<int, int>> GetEnumerator()
        {
            return _spans.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
