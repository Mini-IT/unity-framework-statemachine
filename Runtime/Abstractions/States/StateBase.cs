using Cysharp.Threading.Tasks;
using System.Threading;

namespace StateMachine
{
    public abstract class StateBase : IPureState
    {
        public abstract UniTask OnPreEnter(CancellationToken cancellationToken);
        public abstract UniTask OnEnter(CancellationToken cancellationToken);

        public abstract UniTask OnPreExit(CancellationToken cancellationToken);
        public abstract UniTask OnExit(CancellationToken cancellationToken);

        public abstract void Dispose();
    }

    public abstract class StateBase<TPayload> : StateBase, IPayloadedState<TPayload>
    {
        public abstract UniTask OnPreEnter(TPayload payload, CancellationToken cancellationToken);
        public abstract UniTask OnEnter(TPayload payload, CancellationToken cancellationToken);

        public sealed override UniTask OnEnter(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public sealed override UniTask OnPreEnter(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
    }
}
