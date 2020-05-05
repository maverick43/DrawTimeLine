using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawTimeLine
{

    class State
    {
        public State()
        {

        }

        public State(State s)
        {
            this.Time = s.Time;
            this.state = s.state;
        }

        public string Time
        {
            set;
            get;
        }
        public string state
        {
            set;
            get;
        }
    }

    class TimeState
    {
        public TimeState()
        {
            State = new List<State>();
        }
        public string Date
        {
            set;
            get;
        }

        public List<State> State
        {
            set;
            get;
        }
    }
}
