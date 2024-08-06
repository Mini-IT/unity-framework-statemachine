using System;
using MiniContainer;

namespace StateMachine
{
    public static class StateMachineExtension
    {
        public static void RegisterStateMachine<TTrigger>(this IBaseDIService diService, IContainer container) where TTrigger : Enum
        {
            diService.RegisterFactory<IState<TTrigger>>(container);
            diService.Register<IStateMachine<TTrigger>, StateMachine<TTrigger>>(ServiceLifeTime.Transient);
        }
    }
}
