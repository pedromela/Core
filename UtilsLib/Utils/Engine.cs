using System;
using System.Reactive.Linq;

namespace UtilsLib.Utils
{
    public class Engine : MyLogger
    {

        public bool Started = false;

        public Engine()
        {

        }

        public virtual void Run()
        {
            try
            {

            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
        }

        public virtual IObservable<int> Init()
        {
            try
            {

            }
            catch (Exception e)
            {
                DebugMessage(e);
            }
            return Observable.Empty<int>();
        }
    }
}
