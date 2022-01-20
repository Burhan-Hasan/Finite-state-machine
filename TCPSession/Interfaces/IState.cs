using System;
using System.Collections.Generic;
using System.Text;

namespace TCPSession.Interfaces
{
    internal interface IState
    {
        string Name { get; }
        void DispatchEvent(SessionState sessionState, string eventName);
    }
}
