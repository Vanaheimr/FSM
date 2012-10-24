using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace de.ahzf.Vanaheimr.FSM.TestApplication1
{

    public enum State
    {
        Start,
        End,
        Error
    }

    public enum Signal
    {
        Hello,
        GoToHell
    }

    public class Program
    {

        public static void Main(String[] Args)
        {

            var FSM1 = new Machine<State, Signal>("FSM1", State.Error) { StartState = State.Start };
//            FSM1.ErrorStates.Add(States.Error);

            FSM1.AddTransition(State.Start, Signal.Hello, () => Console.WriteLine("Hello received!"), State.End);

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
