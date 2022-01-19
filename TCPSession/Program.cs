using System;

namespace TCPSession
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var tcp = new TCPSession();

            var events = new string[] { "APP_PASSIVE_OPEN", "APP_SEND", "RCV_SYN_ACK" };
            for (int i = 0; i < events.Length; i++)
            tcp.EmitEvent(events[i]);

            Console.WriteLine(tcp.SessionState.State.Name);
        }
    }

    class TCPSession
    {
        public SessionState SessionState { get; }

        public TCPSession()
        {
            SessionState = new SessionState();
        }

        public void EmitEvent(string eventName)
        {
            SessionState.State.DispatchEvent(SessionState, eventName);
        }

    }

    interface IState
    {
        string Name { get; }
        void DispatchEvent(SessionState sessionState, string eventName);
    }
    class SessionState
    {
        public IState State { get; set; }

        public SessionState()
        {
            State = new ClosedState();
        }
    }


    class ErrorState : IState
    {
        public string Name => "ERROR";

        static IState instance = new ErrorState();
        public IState GetInstance => instance;
        public void DispatchEvent(SessionState sessionState, string eventName) { }
    }

    class ClosedState : IState
    {
        public string Name => "CLOSED";

        static IState instance = new ClosedState();
        public IState GetInstance => instance;
        public void DispatchEvent(SessionState sessionState, string eventName)
        {
            sessionState.State = eventName.ToUpper() switch
            {
                "APP_PASSIVE_OPEN" => new ListenState(),
                "APP_ACTIVE_OPEN" => new SynSentState(),
                _ => new ErrorState()
            };
        }
    }

    class ListenState : IState
    {
        public string Name => "LISTEN";

        static IState instance = new ListenState();
        public IState GetInstance => instance;
        public void DispatchEvent(SessionState sessionState, string eventName)
        {
            sessionState.State = eventName.ToUpper() switch
            {
                "RCV_SYN" => new SynRcvdState(),
                "APP_SEND" => new SynSentState(),
                "APP_CLOSE" => new ClosedState(),
                _ => new ErrorState()
            };
        }
    }

    class SynSentState : IState
    {
        public string Name => "SYN_SENT";

        static IState instance = new SynSentState();
        public IState GetInstance => instance;
        public void DispatchEvent(SessionState sessionState, string eventName)
        {
            sessionState.State = eventName.ToUpper() switch
            {
                "RCV_SYN" => new SynRcvdState(),
                "RCV_SYN_ACK" => new EstablishedState(),
                "APP_CLOSE" => new ClosedState(),
                _ => new ErrorState()
            };
        }
    }

    class SynRcvdState : IState
    {
        public string Name => "SYN_RCVD";
        static IState instance = new SynRcvdState();
        public IState GetInstance => instance;
        public void DispatchEvent(SessionState sessionState, string eventName)
        {
            sessionState.State = eventName.ToUpper() switch
            {
                "RCV_ACK" => new EstablishedState(),
                "APP_CLOSE" => new FinWait1State(),
                _ => new ErrorState()
            };
        }
    }

    class EstablishedState : IState
    {
        public string Name => "ESTABLISHED";

        static IState instance = new EstablishedState();
        public IState GetInstance => instance;
        public void DispatchEvent(SessionState sessionState, string eventName)
        {
            sessionState.State = eventName.ToUpper() switch
            {
                "APP_CLOSE" => new FinWait1State(),
                "RCV_FIN" => new CloseWaitState(),
                _ => new ErrorState()
            };
        }
    }

    class CloseWaitState : IState
    {
        public string Name => "CLOSE_WAIT";

        static IState instance = new CloseWaitState();
        public IState GetInstance => instance;
        public void DispatchEvent(SessionState sessionState, string eventName)
        {
            sessionState.State = eventName.ToUpper() switch
            {
                "APP_CLOSE" => new LastAckState(),
                _ => new ErrorState()
            };
        }
    }

    class LastAckState : IState
    {
        public string Name => "LAST_ACK";

        static IState instance = new LastAckState();
        public IState GetInstance => instance;
        public void DispatchEvent(SessionState sessionState, string eventName)
        {
            sessionState.State = eventName.ToUpper() switch
            {
                "RCV_ACK" => new ClosedState(),
                _ => new ErrorState()
            };
        }
    }

    class FinWait1State : IState
    {
        public string Name => "FIN_WAIT_1";
        static IState instance = new FinWait1State();
        public IState GetInstance => instance;
        public void DispatchEvent(SessionState sessionState, string eventName)
        {
            sessionState.State = eventName.ToUpper() switch
            {
                "RCV_FIN" => new ClosingState(),
                "RCV_FIN_ACK" => new TimeWaitState(),
                "RCV_ACK" => new FinWait2State(),
                _ => new ErrorState()
            };
        }
    }

    class FinWait2State : IState
    {
        public string Name => "FIN_WAIT_2";
        static IState instance = new FinWait2State();
        public IState GetInstance => instance;
        public void DispatchEvent(SessionState sessionState, string eventName)
        {
            sessionState.State = eventName.ToUpper() switch
            {
                "RCV_FIN" => new TimeWaitState(),
                _ => new ErrorState()
            };
        }
    }

    class ClosingState : IState
    {
        public string Name => "CLOSING";
        static IState instance = new ClosingState();
        public IState GetInstance => instance;
        public void DispatchEvent(SessionState sessionState, string eventName)
        {
            sessionState.State = eventName.ToUpper() switch
            {
                "RCV_ACK" => new TimeWaitState(),
                _ => new ErrorState()
            };
        }
    }

    class TimeWaitState : IState
    {
        public string Name => "TIME_WAIT";
        static IState instance = new TimeWaitState();
        public IState GetInstance => instance;
        public void DispatchEvent(SessionState sessionState, string eventName)
        {
            sessionState.State = eventName.ToUpper() switch
            {
                "APP_TIMEOUT" => new ClosedState(),
                _ => new ErrorState()
            };
        }
    }
}
