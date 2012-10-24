using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace de.ahzf.Vanaheimr.FSM.TestApplication1
{

    public enum myStates
    {
        Start,
        End,
        Error
    }

    public enum mySignals
    {
        Hello,
        GoToHell
    }

    public class Program
    {

        public static void Main(String[] Args)
        {

            var FSM1 = new Machine<myStates, mySignals>("FSM1", myStates.Error);

            var Start = FSM1.AddState(myStates.Start);
            var End   = FSM1.AddState(myStates.End);

            var Hello = FSM1.AddSignal(mySignals.Hello);

            FSM1.AddTransition(myStates.Start, mySignals.Hello, () => Console.WriteLine("Hello received!"), myStates.End);

            Console.WriteLine(FSM1.CurrentState);

            FSM1.ProcessSignal(mySignals.Hello);

            Console.WriteLine(FSM1.CurrentState);

            FSM1.ProcessSignal(mySignals.GoToHell);

            Console.WriteLine(FSM1.CurrentState);

            FSM1.ProcessSignal(mySignals.Hello);

            Console.WriteLine(FSM1.CurrentState);

        }

    }

}
