namespace WindowsPhoneDriver.OuterDriver
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using OpenQA.Selenium.Remote;

    using DriverCommand = WindowsPhoneDriver.Common.DriverCommand;

    internal class UriDispatchTables
    {
        #region Fields

        private UriTemplateTable deleteDispatcherTable;

        private UriTemplateTable getDispatcherTable;

        private UriTemplateTable postDispatcherTable;

        #endregion

        #region Constructors and Destructors

        public UriDispatchTables(Uri prefix)
        {
            this.ConstructDispatcherTables(prefix);
        }

        #endregion

        #region Public Methods and Operators

        public UriTemplateMatch Match(string httpMethod, Uri uriToMatch)
        {
            return this.FindDispatcherTable(httpMethod).MatchSingle(uriToMatch);
        }

        #endregion

        #region Methods

        internal UriTemplateTable FindDispatcherTable(string httpMethod)
        {
            UriTemplateTable tableToReturn = null;
            switch (httpMethod)
            {
                case CommandInfo.GetCommand:
                    tableToReturn = this.getDispatcherTable;
                    break;

                case CommandInfo.PostCommand:
                    tableToReturn = this.postDispatcherTable;
                    break;

                case CommandInfo.DeleteCommand:
                    tableToReturn = this.deleteDispatcherTable;
                    break;
            }

            return tableToReturn;
        }

        private void ConstructDispatcherTables(Uri prefix)
        {
            this.getDispatcherTable = new UriTemplateTable(prefix);
            this.postDispatcherTable = new UriTemplateTable(prefix);
            this.deleteDispatcherTable = new UriTemplateTable(prefix);

            var fields = typeof(DriverCommand).GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var field in fields)
            {
                var commandName = field.GetValue(null).ToString();
                var commandInformation = CommandInfoRepository.Instance.GetCommandInfo(commandName);
                var commandUriTemplate = new UriTemplate(commandInformation.ResourcePath);
                var templateTable = this.FindDispatcherTable(commandInformation.Method);
                templateTable.KeyValuePairs.Add(new KeyValuePair<UriTemplate, object>(commandUriTemplate, commandName));
            }

            this.getDispatcherTable.MakeReadOnly(false);
            this.postDispatcherTable.MakeReadOnly(false);
            this.deleteDispatcherTable.MakeReadOnly(false);
        }

        #endregion
    }
}
