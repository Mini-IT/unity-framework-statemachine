using Cysharp.Threading.Tasks;
using System.Threading;

namespace StateMachine
{
    public abstract class StateBase : IPureState
    {
        public abstract UniTask OnBeforeEnter(CancellationToken cancellationToken);
        public abstract UniTask OnEnter(CancellationToken cancellationToken);

        public abstract UniTask OnBeforeExit(CancellationToken cancellationToken);
        public abstract UniTask OnExit(CancellationToken cancellationToken);

        public abstract void Dispose();
    }

    public abstract class StateBase<TPayload> : StateBase, IPayloadedState<TPayload>
    {
        public abstract UniTask OnBeforeEnter(TPayload payload, CancellationToken cancellationToken);
        public abstract UniTask OnEnter(TPayload payload, CancellationToken cancellationToken);

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
