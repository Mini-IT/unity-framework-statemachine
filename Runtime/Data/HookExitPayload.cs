using System;

namespace StateMachine
{
    public struct HookExitPayload
    {
        public readonly Type currentType;
        public readonly Type targetType;

        public HookExitPayload(Type currentType, Type targetType)
        {
            this.targetType = targetType;
            this.currentType = currentType;
        }
    }
}