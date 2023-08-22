// ReSharper disable SuspiciousTypeConversion.Global

using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MiniContainer;

namespace StateMachine
{
    public sealed class StateMachine<TTrigger> : IStateMachine<TTrigger> where TTrigger : Enum
    {
        private readonly Dictionary<TTrigger, Type> _stateTypes;
        private readonly Dictionary<TTrigger, HashSet<TTrigger>> _transitions;
        private readonly IFactoryService<IState> _stateFactory;
        private readonly IScopeManager _scopeManager;
        private readonly HashSet<IStateMachineHook> _hooks = new HashSet<IStateMachineHook>();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        
        private Action<IState> _onStateChanged;
        private int _scope;
        public IState CurrentState { get; private set; }

        public StateMachine(IFactoryService<IState> stateStateFactory, IScopeManager scopeManager)
        {
            _stateFactory = stateStateFactory;
            _scopeManager = scopeManager;
            _stateTypes = new Dictionary<TTrigger, Type>();
            _transitions = new Dictionary<TTrigger, HashSet<TTrigger>>();
        }

        public void Dispose()
        {
            _scopeManager.ReleaseScope(_scope);
        }

        public void SubscribeOnStateChanged(Action<IState> callback)
        {
            _onStateChanged -= callback;
            _onStateChanged += callback;
        }

        public void UnsubscribeOnStateChanged(Action<IState> callback)
        {
            _onStateChanged -= callback;
        }

        /// <inheritdoc cref="IStateMachine{TTrigger}.Register{T}"/>
        public StateMachine<TTrigger>.StateConfiguration Register<T>(TTrigger trigger) where T : IState
        {
            _stateTypes[trigger] = typeof(T);
            return new StateConfiguration(this, trigger);
        }

        /// <inheritdoc cref="IStateMachine{TTrigger}.AllowTransition(TTrigger, TTrigger)"/>
        public StateMachine<TTrigger>.StateConfiguration AllowTransition(TTrigger from, TTrigger to)
        {
            var configuration = new StateConfiguration(this, from);

            if (to.Equals(from))
            {
                return configuration;
            }

            if (!_transitions.TryGetValue(from, out var transitions))
            {
                transitions = new HashSet<TTrigger>();
                _transitions.Add(from, transitions);
            }

            transitions.Add(to);

            return configuration;
        }

        /// <inheritdoc cref="IStateMachine{TTrigger}.AllowTransitions(TTrigger, TTrigger)"/>
        public StateMachine<TTrigger>.StateConfiguration AllowTransitions(TTrigger from, TTrigger[] to)
        {
            if (!_transitions.TryGetValue(from, out var transitions))
            {
                transitions = new HashSet<TTrigger>();
                _transitions.Add(from, transitions);
            }

            foreach (var trigger in to)
            {
                if (!to.Equals(from))
                {
                    transitions.Add(trigger);
                }
            }

            return new StateConfiguration(this, from);
        }

        /// <summary>
        /// Start transition to the new state
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="cancellationToken"></param>
        public async UniTask Fire(TTrigger trigger, CancellationToken cancellationToken)
        {
            try
            {
                await _semaphore.WaitAsync(cancellationToken);
                await FireInternal(trigger, cancellationToken);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Start transition to the new state with payload if state requires payload
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="payload"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TPayload"></typeparam>
        public async UniTask Fire<TPayload>(TTrigger trigger, TPayload payload, CancellationToken cancellationToken)
        {
            await FireInternal(trigger, payload, cancellationToken);
        }

        /// <summary>
        /// Adds a hook to the state machine that will be called on every state change.
        /// </summary>
        /// <param name="hook">Implementation of the hook</param>
        /// <exception cref="InvalidOperationException">Will be thrown if such already attached. Duplications not allowed</exception>
        public void AddHook(IStateMachineHook hook)
        {
            if (_hooks.Contains(hook))
            {
                throw new InvalidOperationException(
                    $"This hook already attached to the state machine {hook.GetType()}");
                
            }

            _hooks.Add(hook);
        }

        /// <summary>
        /// Removes a hook from the state machine.
        /// Second and consecutive calls for the same hook will be ignored.
        /// </summary>
        /// <param name="hook">Implementation of the hook</param>
        public void RemoveHook(IStateMachineHook hook)
        {
            if (_hooks.Contains(hook))
            {
                _hooks.Remove(hook);
            }
        }
        
        private async UniTask Enter(Type stateType, CancellationToken cancellationToken)
        {
            // Create a new scope
            var newScope = _scopeManager.CreateScope();

            // Create a new state
            var state = _stateFactory.GetService(stateType);
            var pureState = (IPureState)state;

            // Notify the states that their states are going to change
            if (CurrentState != null)
            {
                await CurrentState.OnPreExit(cancellationToken);
            }
            await pureState.OnPreEnter(cancellationToken);

            // Exit the previous state
            await ExitCurrentState(stateType, cancellationToken);

            // Switch to the new scope and state
            _scope = newScope;
            CurrentState = state;

            foreach (var stateMachineHook in _hooks)
            {
                await stateMachineHook.OnBeforeEnter(new HookEnterPayload(stateType), cancellationToken);
            }
            
            await pureState.OnEnter(cancellationToken);

            foreach (var stateMachineHook in _hooks)
            {
                await stateMachineHook.OnAfterEnter(new HookEnterPayload(stateType), cancellationToken);
            }
            
            _onStateChanged?.Invoke(CurrentState);
        }

        private async UniTask Enter<TPayload>(Type stateType, TPayload payload, CancellationToken cancellationToken)
        {
            // Create a new scope
            var newScope = _scopeManager.CreateScope();

            // Create a new state
            var state = _stateFactory.GetService(stateType);
            var payloadState = (IPayloadedState<TPayload>)state;

            // Notify the states that their states are going to change
            if (CurrentState != null)
            {
                await CurrentState.OnPreExit(cancellationToken);
            }
            await payloadState.OnPreEnter(payload, cancellationToken);

            // Exit the previous state
            await ExitCurrentState(stateType, cancellationToken);

            // Switch to the new scope and state
            _scope = newScope;
            CurrentState = payloadState;

            foreach (var stateMachineHook in _hooks)
            {
                await stateMachineHook.OnBeforeEnter(new HookEnterPayload(stateType), cancellationToken);
            }
            
            await payloadState.OnEnter(payload, cancellationToken);

            foreach (var stateMachineHook in _hooks)
            {
                await stateMachineHook.OnAfterEnter(new HookEnterPayload(stateType), cancellationToken);
            }

            _onStateChanged?.Invoke(CurrentState);
        }

        private async UniTask ExitCurrentState(Type stateType, CancellationToken cancellationToken)
        {
            if (CurrentState != null)
            {            
                foreach (var stateMachineHook in _hooks)
                {
                    await stateMachineHook.OnBeforeExit(new HookExitPayload(CurrentState.GetType(), stateType),
                        cancellationToken);
                }

                await CurrentState.OnExit(cancellationToken);
                
                foreach (var stateMachineHook in _hooks)
                {
                    await stateMachineHook.OnAfterExit(new HookExitPayload(CurrentState.GetType(), stateType),
                        cancellationToken);
                }

                _scopeManager.ReleaseScope(_scope);
            }
            else
            {
                await UniTask.CompletedTask;
            }
        }

        private async UniTask FireInternal(TTrigger trigger, CancellationToken cancellationToken)
        {
            var type = VerifyAndReturnStateType(trigger);

            if (type == null) return;
            
            await Enter(type, cancellationToken);
        }

        private async UniTask FireInternal<TPayload>(TTrigger trigger, TPayload payload,
            CancellationToken cancellationToken)
        {
            var type = VerifyAndReturnStateType(trigger);

            if (type == null) return;
            
            await Enter(type, payload, cancellationToken);
        }

        private Type VerifyAndReturnStateType(TTrigger trigger)
        {
            if (!_stateTypes.TryGetValue(trigger, out Type nextStateType))
            {
                Errors.StateMachineException(
                    $"State Machine '{GetType().Name}' has no registered state for trigger '{trigger}'");
                return null;
            }

            if (CurrentState != null)
            {
                // Check if this transitions is allowed

                // TODO: optimize the search
                var currentStateType = CurrentState.GetType();
                TTrigger currentStateTrigger = default;
                bool found = false;
                foreach (var pair in _stateTypes)
                {
                    if (pair.Value == currentStateType)
                    {
                        currentStateTrigger = pair.Key;
                        found = true;
                        break;
                    }
                }

                if (!found || !_transitions.TryGetValue(currentStateTrigger, out var list))
                {
                    Errors.StateMachineException(
                        $"State Machine '{GetType().Name}' - no transition is allowed from the current state '{currentStateType.Name}'");
                    return null;
                }

                if (!list.Contains(trigger))
                {
                    Errors.StateMachineException(
                        $"State Machine '{GetType().Name}' - transition from '{currentStateTrigger}' to '{trigger}' is not allowed");
                    return null;
                }
            }

            return nextStateType;
        }

        public readonly struct StateConfiguration
        {
            private readonly IStateMachine<TTrigger> _stateMachine;
            private readonly TTrigger _trigger;

            public StateConfiguration(IStateMachine<TTrigger> stateMachine, TTrigger trigger)
            {
                _stateMachine = stateMachine;
                _trigger = trigger;
            }

            /// <summary>
            /// Allow transition to the new state by trigger and state type
            /// </summary>
            /// <param name="trigger">Trigger that will "trigger" transition</param>
            /// <returns>Configuration for the same state</returns>
            /// <exception cref="ArgumentException"></exception>
            public StateConfiguration AllowTransition(TTrigger trigger)
            {
                _stateMachine.AllowTransition(_trigger, trigger);
                return this;
            }
        }
    }
}
