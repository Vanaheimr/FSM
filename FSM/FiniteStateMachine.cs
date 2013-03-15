/*
 * Copyright (c) 2010-2013, Achim 'ahzf' Friedland <achim@graph-database.org>
 * This file is part of FSM <http://www.github.com/Vanaheimr/FSM>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;

#endregion

namespace eu.Vanaheimr.FSM
{

    /// <summary>
    /// A simple enum-based finite state machine.
    /// </summary>
    /// <typeparam name="TState">The type of the states.</typeparam>
    /// <typeparam name="TSignal">The type of the signals.</typeparam>
    public class FiniteStateMachine<TState, TSignal>
        where TState  : struct, IComparable, IFormattable, IConvertible // all interfaces of System.Enum ;)
        where TSignal : struct, IComparable, IFormattable, IConvertible
    {

        #region Data

        private HashSet<TSignal> Signals;
        
        private Dictionary<TState, Dictionary<TSignal, Tuple<Action, TState>>> Transitions;

        #endregion

        #region Properties

        #region Name

        /// <summary>
        /// The name or identification for this finite state machine.
        /// </summary>
        public String Name { get; private set; }

        #endregion

        #region StartState

        /// <summary>
        /// The starting state.
        /// </summary>
        public TState StartState { get; private set; }

        private Boolean StartStateSet;

        #endregion

        #region CurrentState

        /// <summary>
        /// The current state.
        /// </summary>
        public TState CurrentState { get; private set; }

        #endregion

        #region AcceptingStates

        private readonly HashSet<TState> _AcceptingStates;

        /// <summary>
        /// All accepting states of the finite state machine.
        /// </summary>
        public IEnumerable<TState> AcceptingStates
        {
            get
            {
                return _AcceptingStates;
            }
        }

        /// <summary>
        /// Returns true if the finite state machine is in an accepting state.
        /// </summary>
        public Boolean InAcceptingState
        {
            get
            {
                return _AcceptingStates.Contains(CurrentState);
            }
        }

        #endregion

        #region ErrorStates

        private readonly HashSet<TState> _ErrorStates;

        /// <summary>
        /// All error states including the golbal error state of the finite state machine.
        /// </summary>
        public IEnumerable<TState> ErrorStates
        {
            get
            {
                return _ErrorStates;
            }
        }

        /// <summary>
        /// The fatal error state.
        /// </summary>
        public TState FatalErrorState { get; private set; }

        private Boolean FatalErrorStateSet;

        /// <summary>
        /// Returns true if the finite state machine is in an error state.
        /// </summary>
        public Boolean InErrorState
        {
            get
            {
                return ErrorStates.Contains(CurrentState);
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        #region FiniteStateMachine()

        /// <summary>
        /// Create a new finite state machine.
        /// </summary>
        public FiniteStateMachine()
            : this(Guid.NewGuid().ToString())
        { }

        #endregion

        #region FiniteStateMachine(FSMName)

        /// <summary>
        /// Create a new finite state machine.
        /// </summary>
        /// <param name="FSMName">A name or identification for this finite state machine.</param>
        public FiniteStateMachine(String FSMName)
        {

            if (!typeof(TState).IsEnum)
                throw new Exception("TState is not an enum!");

            if (!typeof(TSignal).IsEnum)
                throw new Exception("TSignal is not an enum!");

            this.Name                = FSMName;
            this.Transitions         = new Dictionary<TState, Dictionary<TSignal, Tuple<Action, TState>>>();
            this.Signals             = new HashSet<TSignal>();
            this._AcceptingStates    = new HashSet<TState>();
            this._ErrorStates        = new HashSet<TState>();
            this.StartStateSet       = false;
            this.FatalErrorStateSet  = false;

            foreach (TState state in Enum.GetValues(typeof(TState)))
            {
                
                AddState(state);
                var memInfo = typeof(TState).GetMember(state.ToString());

                var attr = memInfo[0].GetCustomAttributes(false);

                if (attr.Where(_ => _ is StartStateAttribute).Any())
                {

                    if (StartStateSet)
                        throw new Exception("Duplicate StartState!");

                    StartState    = state;
                    StartStateSet = true;

                }

                if (attr.Where(_ => _ is AcceptingStateAttribute).Any())
                {

                    if (_ErrorStates.Contains(state))
                        throw new Exception("illegal!");

                    _AcceptingStates.Add(state);

                }

                if (attr.Where(_ => _ is FatalErrorStateAttribute).Any())
                {

                    if (FatalErrorStateSet)
                        throw new Exception("Duplicate FatalErrorState!");

                    if (_AcceptingStates.Contains(state))
                        throw new Exception("illegal!");

                    FatalErrorState    = state;
                    FatalErrorStateSet = true;

                    _ErrorStates.Add(state);

                }

                if (attr.Where(_ => _ is ErrorStateAttribute).Any())
                {

                    if (_AcceptingStates.Contains(state))
                        throw new Exception("illegal!");

                    _ErrorStates.Add(state);

                }

            }



            foreach (TSignal signal in Enum.GetValues(typeof(TSignal)))
                AddSignal(signal);


            CurrentState = StartState;

        }

        #endregion

        #endregion


        #region (private) AddState(State)

        private TState AddState(TState State)
        {
            Transitions.Add(State, new Dictionary<TSignal, Tuple<System.Action, TState>>());
            return State;
        }

        #endregion

        #region (private) AddSignal(State)

        private TSignal AddSignal(TSignal Signal)
        {
            Signals.Add(Signal);
            return Signal;
        }

        #endregion


        #region AddTransition(Source, Signal, Action, Target)

        /// <summary>
        /// Adds a new transition.
        /// </summary>
        /// <param name="Source">The source state.</param>
        /// <param name="Signal">The transition signal.</param>
        /// <param name="Action">The transition action.</param>
        /// <param name="Target">The target state.</param>
        public void AddTransition(TState Source, TSignal Signal, Action Action, TState Target)
        {

            if (!Signals.Contains(Signal))
                throw new Exception("Unknown signal!");

            var d1 = Transitions[Source];

            d1.Add(Signal, new Tuple<Action, TState>(Action, Target));

        }

        #endregion

        #region ProcessSignal(Signal)

        /// <summary>
        /// Process the incoming signal.
        /// </summary>
        /// <param name="Signal">A transition signal.</param>
        public void ProcessSignal(TSignal Signal)
        {

            if (ErrorStates.Contains(CurrentState))
                return;

            var d1 = Transitions[CurrentState];

            Tuple<Action, TState> Tuple = null;

            if (d1.TryGetValue(Signal, out Tuple))
            {
                Tuple.Item1();
                CurrentState = Tuple.Item2;
            }

            else
            {
                CurrentState = FatalErrorState;
            }

        }

        #endregion

    }

}
