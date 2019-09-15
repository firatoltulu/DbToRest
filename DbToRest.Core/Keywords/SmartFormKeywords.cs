namespace DbToRest.Core.Keywords
{
    public static class SmartFormKeywords
    {
        public static class StandartToken
        {
            public static string Validate { get { return "validate"; } }
            public static string Maxlength { get { return "maxLength"; } }
            public static string PrimaryKey { get { return "Id"; } }
            public static string Hidden { get { return "hidden"; } }
            public static string Readonly { get { return "readonly"; } }

            public static string Required { get { return "required"; } }
            public static string Column { get { return "column"; } }

            public static string Value { get { return "value"; } }

            public static string Source { get { return "source"; } }

            public static string caption { get { return "source"; } }


        }

        public static class Actions
        {
            public static string Custom { get { return "Custom"; } }
            public static string AddNote { get { return "addNote"; } }

            public static string Delete { get { return "delete"; } }

            public static string AddDocument { get { return "addDocument"; } }

            public static string NoteListView { get { return "NoteListView"; } }
            public static string DocumentListView { get { return "DocumentListView"; } }

            public static string FormHistoryListView { get { return "FormHistoryListView"; } }
        }
    }
}