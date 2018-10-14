namespace PowerSocketServer
{
    public static class Message
    {
        public const string NEXT_SLIDE = "nextSlide";
        public const string PREV_SLIDE = "prevSlide";
        public const string AUTHENTICATE = "authenticate";
        public const string SYNC = "sync"; // request for state update
        public const string BROADCAST = "broadcast"; // send broadcast
    }
}
