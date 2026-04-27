using System;
using Stateless;

namespace BugPro
{
    public enum BugState { New, Open, InProgress, Fixed, Resolved, Closed, Reopened, Rejected, Deferred }
    public enum BugTrigger { Start, Assign, Fix, Verify, Close, Reopen, Reject, Defer, Resume, Abandon }

    public class Bug
    {
        private readonly StateMachine<BugState, BugTrigger> _machine;

        public BugState CurrentState => _machine.State;
        public string Title { get; set; }

        public Bug(string title = "Untitled Bug")
        {
            Title = title;
            _machine = new StateMachine<BugState, BugTrigger>(BugState.New);
            ConfigureStateMachine();
        }

        private void ConfigureStateMachine()
        {
            _machine.Configure(BugState.New)
                .Permit(BugTrigger.Start, BugState.Open);

            _machine.Configure(BugState.Open)
                .Permit(BugTrigger.Assign, BugState.InProgress)
                .Permit(BugTrigger.Reject, BugState.Rejected)
                .Permit(BugTrigger.Defer, BugState.Deferred);

            _machine.Configure(BugState.InProgress)
                .Permit(BugTrigger.Fix, BugState.Fixed)
                .Permit(BugTrigger.Abandon, BugState.Open);

            _machine.Configure(BugState.Fixed)
                .Permit(BugTrigger.Verify, BugState.Resolved)
                .Permit(BugTrigger.Reopen, BugState.Reopened);

            _machine.Configure(BugState.Resolved)
                .Permit(BugTrigger.Close, BugState.Closed)
                .Permit(BugTrigger.Reopen, BugState.Reopened);

            _machine.Configure(BugState.Reopened)
                .Permit(BugTrigger.Assign, BugState.InProgress)
                .Permit(BugTrigger.Fix, BugState.Fixed);

            _machine.Configure(BugState.Deferred)
                .Permit(BugTrigger.Resume, BugState.Open);
        }

        public void Fire(BugTrigger trigger) => _machine.Fire(trigger);
        public bool CanFire(BugTrigger trigger) => _machine.CanFire(trigger);
        public IEnumerable<BugTrigger> PermittedTriggers => _machine.PermittedTriggers;
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Демонстрация WorkFlow бага ===");
            var bug = new Bug("UI-101");
            Console.WriteLine($"Создан баг: {bug.Title}, Статус: {bug.CurrentState}");

            bug.Fire(BugTrigger.Start);
            Console.WriteLine($"-> {bug.CurrentState}");

            bug.Fire(BugTrigger.Assign);
            Console.WriteLine($"-> {bug.CurrentState}");

            bug.Fire(BugTrigger.Fix);
            Console.WriteLine($"-> {bug.CurrentState}");

            bug.Fire(BugTrigger.Verify);
            Console.WriteLine($"-> {bug.CurrentState}");

            bug.Fire(BugTrigger.Close);
            Console.WriteLine($"-> {bug.CurrentState}");

            Console.WriteLine("\nРабочий процесс успешно завершен.");
        }
    }
}