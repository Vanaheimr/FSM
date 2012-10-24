using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace de.ahzf.Vanaheimr.FSM
{

    public class Machine<TStates, TSignal>
        //where TStates : IEquatable<TStates>
        //where TSignal : IEquatable<TSignal>
    {

        private Dictionary<TStates, Dictionary<TSignal, Tuple<Action, TStates>>> Transitions;

        private HashSet<TSignal> Signals;

        public TStates CurrentState { get; private set; }

        public String Name { get; private set; }

        public TStates ErrorState { get; private set; }


        public Machine(String Name, TStates ErrorState)
        {
            this.Name = Name;
            this.Transitions = new Dictionary<TStates, Dictionary<TSignal, Tuple<Action, TStates>>>();
            this.Signals = new HashSet<TSignal>();
            this.ErrorState = ErrorState;
            AddState_(ErrorState);
        }

        private TStates AddState_(TStates State)
        {
            Transitions.Add(State, new Dictionary<TSignal, Tuple<System.Action, TStates>>());
            return State;
        }

        public TStates AddState(TStates Id)
        {

            var State = AddState_(Id);

            if (CurrentState == null)
                CurrentState = State;

            return State;

        }

        public void SetState(TStates State)
        {
            CurrentState = State;
        }

        public TSignal AddSignal(TSignal Signal)
        {
            Signals.Add(Signal);
            return Signal;
        }



        public void AddTransition(TStates Source, TSignal Signal, Action Action, TStates Target)
        {

            if (!Signals.Contains(Signal))
                throw new Exception("Unknown signal!");

            var d1 = Transitions[Source];

            d1.Add(Signal, new Tuple<Action, TStates>(Action, Target));

        }

        public void ProcessSignal(TSignal Signal)
        {

            if (CurrentState.Equals(ErrorState))
                return;

            var d1 = Transitions[CurrentState];

            Tuple<Action, TStates> Tuple = null;

            if (d1.TryGetValue(Signal, out Tuple))
            {
                Tuple.Item1();
                CurrentState = Tuple.Item2;
            }

            else
            {
                CurrentState = ErrorState;
            }

        }

    }

}
