using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Dots.Infra.AC.Utils
{
    public interface IFlowBuilder<in T>
    {
        IFlowBuilder<T> WithVoid(Action action, T priority, string name = null);
        IFlowBuilder<T> WithTask(Func<UniTask> action, T priority, string name = null);
        IReadOnlyFlow Build(CancellationToken token, string name);
    }

    public class FlowBuilder<T> : IFlowBuilder<T>
    {
        private readonly SortedList<T, Step> _steps = new SortedList<T, Step>();

        public IFlowBuilder<T> WithVoid(Action action, T priority, string name = null)
        {
            var step = new Step(action, name ?? $"Step priority: {priority}");
            AddStep(priority, step);
            return this;
        }

        public IFlowBuilder<T> WithTask(Func<UniTask> task, T priority, string name = null)
        {
            var step = new Step(task, name ?? $"Step priority: {priority}");
            AddStep(priority, step);
            return this;
        }

        public IReadOnlyFlow Build(CancellationToken token, string name)
        {
            var flow = new Flow(token, name);

            for (var i = 0; i < _steps.Count; i++)
            {
                var s = _steps.Values[i];

                switch (s.Type)
                {
                    case StepType.Action:
                        flow.WithVoid(s.Action, s.Name);
                        break;
                    case StepType.Task:
                        flow.WithTask(s.Task, s.Name);
                        break;
                    default:
                        throw new InvalidOperationException($"Undefained step type: {s.Type}.");
                }
            }

            _steps.Clear();

            return new ReadOnlyFlow(flow);
        }

        private void AddStep(T priority, Step step)
        {
            if (_steps.ContainsKey(priority))
            {
                Debug.LogWarning($"Step with priority '{priority}' existed and was removed.");
            }

            Debug.Log($"Step #{_steps.Count} with priority '{priority}' was added.");
            _steps[priority] = step;
        }

        private sealed class Step
        {
            public readonly Func<UniTask> Task;
            public readonly Action Action;
            public readonly string Name;
            public readonly StepType Type;

            public Step(Func<UniTask> task, string name)
            {
                Type = StepType.Task;
                Task = task;
                Name = name;
            }

            public Step(Action action, string name)
            {
                Type = StepType.Action;
                Action = action;
                Name = name;
            }
        }

        private enum StepType
        {
            Action,
            Task
        }
    }
}