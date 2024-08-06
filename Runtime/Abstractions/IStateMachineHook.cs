using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace StateMachine
{
    public interface IStateMachineHook : IDisposable
    {
        UniTask OnBeforeEnter(HookEnterPayload payload, CancellationToken cancellationToken);
        UniTask OnAfterEnter(HookEnterPayload payload, CancellationToken cancellationToken);

        UniTask OnBeforeExit(HookExitPayload payload, CancellationToken cancellationToken);
        UniTask OnAfterExit(HookExitPayload payload, CancellationToken cancellationToken);
    }
    
    public interface IStateMachineHook<T, TTrigger> : IStateMachineHook where T : IState<TTrigger> where TTrigger : Enum
    {
    }
}
