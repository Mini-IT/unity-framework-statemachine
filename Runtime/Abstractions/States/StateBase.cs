using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace StateMachine
{
    public class StateBase<TTrigger, TPayload> : IPayloadedState<TTrigger, TPayload>, IDisposable
        where TTrigger : Enum
        where TPayload : IStatePayload
    {
        public virtual UniTask OnBeforeEnter(TTrigger trigger, TPayload payload, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnEnter(TTrigger trigger, TPayload payload, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnBeforeExit(TTrigger currentTrigger, TTrigger nextTrigger, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnExit(TTrigger currentTrigger, TTrigger nextTrigger, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public virtual void Dispose() { }
    }
}
