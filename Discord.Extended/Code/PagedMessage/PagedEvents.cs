namespace Discord.Extended
{
    public static class PagedEvents
    {
        public static void PreviousPage(ref PagedMessage message) => message.PreviousPage();
        public static void NextPage(ref PagedMessage message) => message.NextPage();
        public static void FirstPage(ref PagedMessage message) => message.SetPage(0);
        public static void LastPage(ref PagedMessage message) => message.SetPage(message.Pages.Length - 1);
    }
}
