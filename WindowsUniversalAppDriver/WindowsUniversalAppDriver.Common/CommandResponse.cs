namespace WindowsUniversalAppDriver.Common
{
    #region

    using System.Net;

    #endregion

    public class CommandResponse
    {
        public HttpStatusCode HttpStatusCode { get; set; }

        public string Content { get; set; }

        public static CommandResponse Create(HttpStatusCode code, string content)
        {
            return new CommandResponse { HttpStatusCode = code, Content = content };
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", this.HttpStatusCode, Content);
        }
    }
}
