using System;
using UnityEngine;

namespace StateMachine
{
    public static class Errors
    {
        public static void StateMachineException(string message)
        {
            throw new Exception(message);
        }

        public static void Log(string message)
        {
            Debug.Log(message);
        }
    }
}