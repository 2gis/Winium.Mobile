namespace Winium.Mobile.Driver.CommandExecutors
{
    #region

    using Winium.Mobile.Connectivity.Gestures;
    using Winium.Mobile.Driver.Automator;

    #endregion

    internal class TouchSingleTapExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            var elementId = Automator.GetValue<string>(this.ExecutedCommand.Parameters, "element");
            if (elementId == null)
            {
                return this.JsonResponse();
            }

            var tapPoint = this.Automator.RequestElementLocation(elementId).GetValueOrDefault();
            this.Automator.EmulatorController.PerformGesture(new TapGesture(tapPoint));

            return this.JsonResponse();
        }

        #endregion
    }
}
