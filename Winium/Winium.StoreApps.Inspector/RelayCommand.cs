namespace Winium.StoreApps.Inspector
{
    #region

    using System;
    using System.Windows.Input;

    #endregion

    public class RelayCommand : ICommand
    {
        #region Fields

        private readonly Func<bool> canExecuteEvaluator;

        private readonly Action methodToExecute;

        #endregion

        #region Constructors and Destructors

        public RelayCommand(Action methodToExecute, Func<bool> canExecuteEvaluator)
        {
            this.methodToExecute = methodToExecute;
            this.canExecuteEvaluator = canExecuteEvaluator;
        }

        public RelayCommand(Action methodToExecute)
            : this(methodToExecute, null)
        {
        }

        #endregion

        #region Public Events

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }

            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        #endregion

        #region Public Methods and Operators

        public bool CanExecute(object parameter)
        {
            if (this.canExecuteEvaluator == null)
            {
                return true;
            }

            var result = this.canExecuteEvaluator.Invoke();
            return result;
        }

        public void Execute(object parameter)
        {
            this.methodToExecute.Invoke();
        }

        #endregion
    }
}
