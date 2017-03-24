namespace Winium.Mobile.Driver
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Winium.Mobile.Common;
    using Winium.Mobile.Driver.CommandExecutors;

    #endregion

    internal class CommandExecutorDispatchTable
    {
        #region Fields

        private static HashSet<string> DriverBoundCommands = new HashSet<string>
                                                          {
                                                              DriverCommand.Status,
                                                              DriverCommand.NewSession,
                                                              DriverCommand.Close,
                                                              DriverCommand.Quit,
                                                              DriverCommand.GetOrientation,
                                                              DriverCommand.Contexts,
                                                              DriverCommand.GetContext,
                                                              DriverCommand.SetContext
                                                          };

        private Dictionary<string, Type> commandExecutorsRepository;

        #endregion

        #region Constructors and Destructors

        public CommandExecutorDispatchTable()
        {
            this.ConstructDispatcherTable();
        }

        #endregion

        #region Public Methods and Operators

        public CommandExecutorBase GetExecutor(Command command)
        {
            var automator = Automator.Automator.InstanceForSession(command.SessionId);

            if (automator.CurrentContext != null && automator.CurrentContext != DefaultContextNames.NativeAppContextName)
            {
                if (!DriverBoundCommands.Contains(command.Name))
                    return new CommandExecutorWebContextForward();
            }

            // TODO inject dependencies into command executor
            Type executorType;
            if (this.commandExecutorsRepository.TryGetValue(command.Name, out executorType))
            {
            }
            else
            {
                executorType = typeof(CommandExecutorForward);
            }

            return (CommandExecutorBase)Activator.CreateInstance(executorType);
        }

        #endregion

        #region Methods

        private void ConstructDispatcherTable()
        {
            this.commandExecutorsRepository = new Dictionary<string, Type>();

            // TODO: bad const
            const string ExecutorsNamespace = "Winium.Mobile.Driver.CommandExecutors";

            var q =
                (from t in Assembly.GetExecutingAssembly().GetTypes()
                 where t.IsClass && t.Namespace == ExecutorsNamespace
                 select t).ToArray();

            var fields = typeof(DriverCommand).GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var field in fields)
            {
                var localField = field;
                var executor = q.FirstOrDefault(x => x.Name.Equals(localField.Name + "Executor"));
                if (executor != null)
                {
                    this.commandExecutorsRepository.Add(localField.GetValue(null).ToString(), executor);
                }
            }
        }

        #endregion
    }
}
