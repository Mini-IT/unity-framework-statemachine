using System;

namespace StateMachine
{
    public struct HookEnterPayload
    {
        public readonly Type targetType;

        public HookEnterPayload(Type targetType)
        {
            this.targetType = targetType;
        }
    }
}