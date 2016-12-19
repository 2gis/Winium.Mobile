namespace TestApp
{
    using System;

    public class TestDto
    {
        public string Text { get; set; }

        public long Value { get; set; }

        public DateTime Date { get; set; }
    }

    // ReSharper disable UnusedMember.Global
    public class AutomationApi
    {
        public static string Echo(string text)
        {
            return text;
        }

        public static TestDto ReturnStubState()
        {
            return new TestDto { Date = new DateTime(1985, 10, 21, 01, 20, 0), Text = "Flux", Value = 3 };
        }
    }

    // ReSharper restore UnusedMember.Global
}