using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace StateMachine
{
    public interface IStateMachine<TTrigger> : IDisposable where TTrigger : Enum
    {
        IState<TTrigger> CurrentState { get; }
        TTrigger CurrentTrigger { get; }

        void SubscribeOnStateChanged(Action<IState<TTrigger>> callback);
        void UnsubscribeOnStateChanged(Action<IState<TTrigger>> callback);

        /// <summary>
        /// Rigister a new state
        /// </summary>
        /// <typeparam name="T">State type</typeparam>
        /// <returns></returns>
        StateMachine<TTrigger>.StateConfiguration Register<T>(TTrigger trigger) where T : IState<TTrigger>;

        /// <summary>
        /// Registers a transision from one state to another
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        StateMachine<TTrigger>.StateConfiguration AllowTransition(TTrigger from, TTrigger to);

        /// <summary>
        /// Registers transisions from one state to the states listed in <paramref name="to"/>
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        StateMachine<TTrigger>.StateConfiguration AllowTransitions(TTrigger from, TTrigger[] to);

        /// <summary>
        /// Start transition to the new state
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="cancellationToken"></param>
        UniTask Fire(TTrigger trigger, CancellationToken cancellationToken);

        /// <summary>
        /// Start transition to the new state with payload if state requires payload
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="payload"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TPayload"></typeparam>
        UniTask Fire<TPayload>(TTrigger trigger, TPayload payload, CancellationToken cancellationToken);

        /// <summary>
        /// Adds a hook to the state machine that will be called on every state change.
        /// </summary>
        /// <param name="hook">Implementation of the hook</param>
        /// <exception cref="InvalidOperationException">Will be thrown if such already attached. Duplications not allowed</exception>
        void AddHook(IStateMachineHook hook);

        /// <summary>
        /// Removes a hook from the state machine.
        /// Second and consecutive calls for the same hook will be ignored.
        /// </summary>
        /// <param name="hook">Implementation of the hook</param>
        void RemoveHook(IStateMachineHook hook);
    }
}
