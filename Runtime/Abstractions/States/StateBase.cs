using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace StateMachine
{
    public class StateBase : IPureState, IDisposable
    {
        public virtual UniTask OnBeforeEnter(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnEnter(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnBeforeExit(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnExit(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public virtual void Dispose() { }
    }

    public class StateBase<TPayload> : StateBase, IPayloadedState<TPayload>
    {
        public virtual UniTask OnBeforeEnter(TPayload payload, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnEnter(TPayload payload, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public sealed override UniTask OnEnter(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public sealed override UniTask OnBeforeEnter(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
    }
}
