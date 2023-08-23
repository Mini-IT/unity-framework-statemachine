using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace StateMachine
{
    public interface IState : IDisposable
    {
        UniTask OnBeforeExit(CancellationToken cancellationToken);
        UniTask OnExit(CancellationToken cancellationToken);
    }

    public interface IPureState : IState
    {
        UniTask OnBeforeEnter(CancellationToken cancellationToken);
        UniTask OnEnter(CancellationToken cancellationToken);
    }

    public interface IPayloadedState<in TPayload> : IState
    {
        UniTask OnBeforeEnter(TPayload payload, CancellationToken cancellationToken);
        UniTask OnEnter(TPayload payload, CancellationToken cancellationToken);
    }
}
