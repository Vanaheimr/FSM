using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace de.ahzf.Vanaheimr.FSM.TestApplication1
{

    public enum State
    {

        [StartState]
        Start,

        Middle,

        [AcceptingState]
        End,

        [FatalErrorState]
        Error

    }

    public enum Signal
    {
        Hello,
        World,
        GoToHell
    }

    public class Program
    {

        public static void Main(String[] Args)
        {

            var FSM1 = new FiniteStateMachine<State, Signal>("FSM1");

            FSM1.AddTransition(State.Start,  Signal.Hello, () => Console.WriteLine("Hello received!"), State.Middle);
            FSM1.AddTransition(State.Middle, Signal.World, () => Console.WriteLine("World received!"), State.End);

            Console.WriteLine(FSM1.CurrentState);

            FSM1.ProcessSignal(Signal.Hello);

            Console.WriteLine(FSM1.CurrentState);

            FSM1.ProcessSignal(Signal.GoToHell);

            Console.WriteLine(FSM1.CurrentState);

            FSM1.ProcessSignal(Signal.Hello);

            Console.WriteLine(FSM1.CurrentState);

        }

    }

}
