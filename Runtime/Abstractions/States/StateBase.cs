using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace StateMachine
{
	public class StateBase<TTrigger> : IPureState<TTrigger>, IDisposable where TTrigger : Enum
    {
        public virtual UniTask OnBeforeEnter(TTrigger trigger, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnEnter(TTrigger trigger, CancellationToken cancellationToken)
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

    public class StateBase<TTrigger, TPayload> : StateBase<TTrigger>, IPayloadedState<TTrigger, TPayload> where TTrigger : Enum
    {
        public virtual UniTask OnBeforeEnter(TTrigger trigger, TPayload payload, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnEnter(TTrigger trigger, TPayload payload, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public sealed override UniTask OnEnter(TTrigger trigger, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public sealed override UniTask OnBeforeEnter(TTrigger trigger, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
    }
}
