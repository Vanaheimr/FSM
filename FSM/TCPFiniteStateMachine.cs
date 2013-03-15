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

    public enum TCPState
    {

        [StartState]
        Closed,

        Listen,
        SYNReceived,
        SYNSent,
        Established,

        FINWait1,
        Closing,
        FINWait2,
        TimeWait,

        CloseWait,
        LastACK,

        [FatalErrorState]
        Error

    }

    public enum TCPSignal
    {
        Listen
    }

    /// <summary>
    /// A finite state machine for the Transmission Control Protocol.
    /// </summary>
    public class TCPStateMachine : FiniteStateMachine<TCPState, TCPSignal>
    {

        #region Constructor(s)

        #region TCPStateMachine()

        /// <summary>
        /// Create a new finite state machine.
        /// </summary>
        public TCPStateMachine()
            : this(Guid.NewGuid().ToString())
        { }

        #endregion

        #region TCPStateMachine(FSMName)

        /// <summary>
        /// Create a new finite state machine.
        /// </summary>
        /// <param name="FSMName">A name or identification for this finite state machine.</param>
        public TCPStateMachine(String FSMName)
            : base(FSMName)
        {

            AddTransition(TCPState.Closed, TCPSignal.Listen, () => { }, TCPState.Listen);
        
        
        }

        #endregion

        #endregion

    }

}
