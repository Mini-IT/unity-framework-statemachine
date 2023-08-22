using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace StateMachine
{
    public interface IState : IDisposable
    {
        UniTask OnPreExit(CancellationToken cancellationToken);
        UniTask OnExit(CancellationToken cancellationToken);
    }

    public interface IPureState : IState
    {
        UniTask OnPreEnter(CancellationToken cancellationToken);
        UniTask OnEnter(CancellationToken cancellationToken);
    }

    public interface IPayloadedState<in TPayload> : IState
    {
        UniTask OnPreEnter(TPayload payload, CancellationToken cancellationToken);
        UniTask OnEnter(TPayload payload, CancellationToken cancellationToken);
    }
}