using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace StateMachine
{
    public interface IStatePayload { }

    public interface IState<TTrigger> where TTrigger : Enum
    {
        UniTask OnBeforeExit(TTrigger currentTrigger, TTrigger nextTrigger, CancellationToken cancellationToken);
        UniTask OnExit(TTrigger currentTrigger, TTrigger nextTrigger, CancellationToken cancellationToken);
    }

    public interface IPayloadedState<TTrigger, in TPayload> : IState<TTrigger>
        where TTrigger : Enum
        where TPayload : IStatePayload
    {
        UniTask OnBeforeEnter(TTrigger trigger, TPayload payload, CancellationToken cancellationToken);
        UniTask OnEnter(TTrigger trigger, TPayload payload, CancellationToken cancellationToken);
    }
}
