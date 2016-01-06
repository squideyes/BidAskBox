using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace BidAskBox.Library
{
    public class DelegateCommand : ICommand
    {
        private readonly Action executeMethod = null;
        private readonly Func<bool> canExecuteMethod = null;
        private bool isAutomaticRequeryDisabled = false;
        private List<WeakReference> canExecuteChangedHandlers;

        public DelegateCommand(Action executeMethod)
            : this(executeMethod, null, false)
        {
        }

        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
            : this(executeMethod, canExecuteMethod, false)
        {
        }

        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod,
            bool isAutomaticRequeryDisabled)
        {
            if (executeMethod == null)
                throw new ArgumentNullException(nameof(executeMethod));

            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
            this.isAutomaticRequeryDisabled = isAutomaticRequeryDisabled;
        }

        public bool CanExecute()
        {
            if (canExecuteMethod != null)
                return canExecuteMethod();

            return true;
        }

        public void Execute()
        {
            if (executeMethod != null)
                executeMethod();
        }

        public bool IsAutomaticRequeryDisabled
        {
            get
            {
                return isAutomaticRequeryDisabled;
            }
            set
            {
                if (isAutomaticRequeryDisabled != value)
                {
                    if (value)
                    {
                        CommandManagerHelper.RemoveHandlersFromRequerySuggested(
                            canExecuteChangedHandlers);
                    }
                    else
                    {
                        CommandManagerHelper.AddHandlersToRequerySuggested(
                            canExecuteChangedHandlers);
                    }

                    isAutomaticRequeryDisabled = value;
                }
            }
        }

        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        protected virtual void OnCanExecuteChanged()
        {
            CommandManagerHelper.CallWeakReferenceHandlers(canExecuteChangedHandlers);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (!isAutomaticRequeryDisabled)
                    CommandManager.RequerySuggested += value;

                CommandManagerHelper.AddWeakReferenceHandler(
                    ref canExecuteChangedHandlers, value, 2);
            }
            remove
            {
                if (!isAutomaticRequeryDisabled)
                    CommandManager.RequerySuggested -= value;

                CommandManagerHelper.RemoveWeakReferenceHandler(
                    canExecuteChangedHandlers, value);
            }
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        void ICommand.Execute(object parameter)
        {
            Execute();
        }
    }

    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> executeMethod = null;
        private readonly Func<T, bool> canExecuteMethod = null;

        private bool isAutomaticRequeryDisabled = false;
        private List<WeakReference> canExecuteChangedHandlers;

        public DelegateCommand(Action<T> executeMethod)
            : this(executeMethod, null, false)
        {
        }

        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
            : this(executeMethod, canExecuteMethod, false)
        {
        }

        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod,
            bool isAutomaticRequeryDisabled)
        {
            if (executeMethod == null)
                throw new ArgumentNullException("executeMethod");

            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
            this.isAutomaticRequeryDisabled = isAutomaticRequeryDisabled;
        }

        public bool CanExecute(T parameter)
        {
            if (canExecuteMethod != null)
                return canExecuteMethod(parameter);

            return true;
        }

        public void Execute(T parameter)
        {
            if (executeMethod != null)
                executeMethod(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        protected virtual void OnCanExecuteChanged()
        {
            CommandManagerHelper.CallWeakReferenceHandlers(canExecuteChangedHandlers);
        }

        public bool IsAutomaticRequeryDisabled
        {
            get
            {
                return isAutomaticRequeryDisabled;
            }
            set
            {
                if (isAutomaticRequeryDisabled != value)
                {
                    if (value)
                    {
                        CommandManagerHelper.RemoveHandlersFromRequerySuggested(
                            canExecuteChangedHandlers);
                    }
                    else
                    {
                        CommandManagerHelper.AddHandlersToRequerySuggested(
                            canExecuteChangedHandlers);
                    }
                    isAutomaticRequeryDisabled = value;
                }
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (!isAutomaticRequeryDisabled)
                    CommandManager.RequerySuggested += value;

                CommandManagerHelper.AddWeakReferenceHandler(
                    ref canExecuteChangedHandlers, value, 2);
            }
            remove
            {
                if (!isAutomaticRequeryDisabled)
                    CommandManager.RequerySuggested -= value;

                CommandManagerHelper.RemoveWeakReferenceHandler(
                    canExecuteChangedHandlers, value);
            }
        }

        bool ICommand.CanExecute(object parameter)
        {
            if (parameter == null && typeof(T).IsValueType)
                return (canExecuteMethod == null);

            return CanExecute((T)parameter);
        }

        void ICommand.Execute(object parameter)
        {
            Execute((T)parameter);
        }
    }

    internal class CommandManagerHelper
    {
        internal static void CallWeakReferenceHandlers(List<WeakReference> handlers)
        {
            if (handlers != null)
            {
                EventHandler[] callees = new EventHandler[handlers.Count];

                int count = 0;

                for (int i = handlers.Count - 1; i >= 0; i--)
                {
                    WeakReference reference = handlers[i];

                    EventHandler handler = reference.Target as EventHandler;

                    if (handler == null)
                    {
                        handlers.RemoveAt(i);
                    }
                    else
                    {
                        callees[count] = handler;
                        count++;
                    }
                }

                for (int i = 0; i < count; i++)
                {
                    EventHandler handler = callees[i];

                    handler(null, EventArgs.Empty);
                }
            }
        }

        internal static void AddHandlersToRequerySuggested(List<WeakReference> handlers)
        {
            if (handlers != null)
            {
                foreach (WeakReference handlerRef in handlers)
                {
                    EventHandler handler = handlerRef.Target as EventHandler;

                    if (handler != null)
                        CommandManager.RequerySuggested += handler;
                }
            }
        }

        internal static void RemoveHandlersFromRequerySuggested(List<WeakReference> handlers)
        {
            if (handlers != null)
            {
                foreach (WeakReference handlerRef in handlers)
                {
                    EventHandler handler = handlerRef.Target as EventHandler;

                    if (handler != null)
                        CommandManager.RequerySuggested -= handler;
                }
            }
        }

        internal static void AddWeakReferenceHandler(
            ref List<WeakReference> handlers, EventHandler handler)
        {
            AddWeakReferenceHandler(ref handlers, handler, -1);
        }

        internal static void AddWeakReferenceHandler(
            ref List<WeakReference> handlers, EventHandler handler, int defaultListSize)
        {
            if (handlers == null)
            {
                handlers = (defaultListSize > 0 ?
                    new List<WeakReference>(defaultListSize) : new List<WeakReference>());
            }

            handlers.Add(new WeakReference(handler));
        }

        internal static void RemoveWeakReferenceHandler(
            List<WeakReference> handlers, EventHandler handler)
        {
            if (handlers != null)
            {
                for (int i = handlers.Count - 1; i >= 0; i--)
                {
                    var reference = handlers[i];

                    var existingHandler = reference.Target as EventHandler;

                    if ((existingHandler == null) || (existingHandler == handler))
                        handlers.RemoveAt(i);
                }
            }
        }
    }
}
