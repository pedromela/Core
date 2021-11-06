using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace UtilsLib.Utils
{
    public class Waiter 
    {
        private int _count = 0;
        private AutoResetEvent _autoEvent = null;
        public Waiter(int count)
        {
            _count = count;
            _autoEvent = new AutoResetEvent(false);
        }

        public void Set() 
        {
            _count--;
            if (_count <= 0)
            {
                _count = 0;
                _autoEvent.Set();
            }
            UtilsLib.DebugMessage(String.Format("Waiter::Set() : {0} left.", _count));
        }

        public void WaitOne() 
        {
            _autoEvent.WaitOne();
        }
    }
}
