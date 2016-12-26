

namespace Corp.RouterService.Memory
{
    internal class CircularIndex
    {
        private int _max = 0;
        private int _index = 0;

        internal int Max
        {
            get { return _max; }
        }

        internal int Index
        {
            get { return _index; }
        }

        private CircularIndex()
        {

        }

        internal CircularIndex(int index, int max)
        {
            //Debug.Assert(index <= max);

            _index = index;
            _max = max;
        }

        public static CircularIndex operator +(CircularIndex c1, int c2)
        {
            return new CircularIndex(c1._index + c2 >= c1._max ? c1._index + c2 - c1._max : c1._index + c2, c1._max);
        }

        public static explicit operator int(CircularIndex index)
        {
            return index._index;
        }        
    }
}
