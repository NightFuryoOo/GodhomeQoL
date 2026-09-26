#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ToggleableBindings.Extensions;
using UnityEngine;

namespace ToggleableBindings.Utility
{
    internal readonly struct CoroutineBuilder
    {
        private readonly IEnumerable<Instruction> _instructions;

        public static CoroutineBuilder New { get; } = new();

        public IEnumerable<Instruction> Instructions
        {
            get => _instructions ?? Enumerable.Empty<Instruction>();
            init => _instructions = value;
        }

        public CoroutineBuilder(IEnumerable<Instruction> instructions)
        {
            _instructions = instructions;
        }

        public CoroutineBuilder WithYield(params object?[]? toYield)
        {
            var instrs = Instructions;
            instrs = toYield != null
                ? instrs.Concat(toYield.Select(o => new Instruction(o)))
                : instrs.Concat(Instruction.YieldNull);

            return new CoroutineBuilder(instrs);
        }

        public CoroutineBuilder WithAction(params Action?[]? actions)
        {
            var instrs = Instructions;
            if (actions != null)
                instrs = instrs.Concat(actions.Select(a => new Instruction(a)));
            else
                instrs = instrs.Concat(Instruction.YieldNull);

            return new CoroutineBuilder(instrs);
        }

        public Coroutine Start()
        {
            return CoroutineController.Start(AsCoroutine());
        }

        public Coroutine Start(string id)
        {
            return CoroutineController.Start(AsCoroutine(), id);
        }

        public IEnumerator AsCoroutine()
        {
            foreach (var instr in Instructions)
            {
                if (instr.IsYield)
                    yield return instr.Yield;
                else
                    instr.Action?.Invoke();
            }
        }

        public readonly struct Instruction
        {
            public static Instruction YieldNull { get; } = new();

            public Action? Action { get; }

            public object? Yield { get; }

            public bool IsYield => Action == null;

            public Instruction(Action? action) : this()
            {
                Action = action;
            }

            public Instruction(object? yield) : this()
            {
                Yield = yield;
            }

            public static implicit operator Instruction(Action? value) => new(value);

            public static implicit operator Instruction(YieldInstruction value) => new(value);

            public static implicit operator Instruction(CustomYieldInstruction value) => new(value);
        }
    }
}