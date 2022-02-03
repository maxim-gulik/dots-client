using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Dots.Infra.AC.Utils
{
    public class Flow
    {
        private readonly CancellationToken _token;
        private readonly bool _enableLogs;
        private readonly string _name;
        private readonly List<Step> _steps;

        public Flow(CancellationToken token, string name, bool enableLogs = false)
        {
            _token = token;
            _enableLogs = enableLogs;
            _name = name ?? string.Empty;
            _steps = new List<Step>();
        }

        public Flow WithTask(Func<UniTask> task, string name = null)
        {
            _steps.Add(new Step(task, name ?? $"Step #{_steps.Count}"));
            return this;
        }
        public Flow WithVoid(Action action, string name = null)
        {
            _steps.Add(new Step(action, name ?? $"Step #{_steps.Count}"));
            return this;
        }

        public Flow WithFlow(Flow flow, string name = null)
        {
            _steps.Add(new Step(() => flow.RunAsync(), name ?? $"Step #{_steps.Count}"));
            return this;
        }

        public async UniTask RunAsync()
        {
            Debug($"Flow '{_name}' started");
            var flowStopwatch = Stopwatch.StartNew();
            foreach (var step in _steps)
            {
                if (_token.IsCancellationRequested)
                {
                    Debug($"Flow '{_name}' canceled");
                    break;
                }

                Debug($"Flow '{_name}' step '{step.Name}' started");
                var stepStopwatch = Stopwatch.StartNew();
                try
                {
                    if (step.IsTask)
                    {
                        await step.Task();
                    }
                    else
                    {
                        step.Action();
                    }
                }
                catch (FlowCanceledException e)
                {
                    Debug($"Flow '{_name}' step '{step.Name}' canceled after {stepStopwatch.ElapsedMilliseconds}ms: {e.Message}");
                    break;
                }
                catch (Exception)
                {
                    Error($"Flow '{_name}' step '{step.Name}' failed after {stepStopwatch.ElapsedMilliseconds}ms");
                    throw;
                }

                Debug($"Flow '{_name}' step '{step.Name}' completed after {stepStopwatch.ElapsedMilliseconds}ms");
            }

            Debug($"Flow '{_name}' completed after {flowStopwatch.ElapsedMilliseconds}ms");
        }

        private class Step
        {
            public readonly Func<UniTask> Task;
            public readonly Action Action;
            public readonly string Name;
            public readonly bool IsTask;

            public Step(Func<UniTask> task, string name)
            {
                IsTask = true;
                Task = task;
                Name = name;
            }

            public Step(Action action, string name)
            {
                Action = action;
                Name = name;
            }
        }
        
        [Conditional("ENABLE_LOGGING")]
        private void Error(string text)
        {
            if (_enableLogs)
            {
                UnityEngine.Debug.LogError(text);
            }
        }
        
        [Conditional("ENABLE_LOGGING")]
        public void Debug(string text)
        {
            if (_enableLogs)
            {
                UnityEngine.Debug.Log(text);
            }
        }
    }
    
    public interface IReadOnlyFlow
    {
        UniTask RunAsync();
    }

    public class ReadOnlyFlow : IReadOnlyFlow
    {
        private readonly Flow _flow;

        public ReadOnlyFlow(Flow flow)
        {
            _flow = flow;
        }

        public UniTask RunAsync()
        {
            return _flow.RunAsync();
        }
    }

    public class FlowCanceledException : Exception
    {
        public FlowCanceledException(string message) : base(message)
        {
        }

        public FlowCanceledException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}