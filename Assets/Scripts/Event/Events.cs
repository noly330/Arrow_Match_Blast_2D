public static class Events
{
    public class OnLoadMap
    {
        public int mapID;                                                                                                
    }

    public class OnArrowClickStart
    {
        public int arrowID;
    }
    public class OnArrowClickSucceed
    {
        public int arrowID;
    }

    public class OnArrowClickFail
    {
        public int arrowID;
    }

    public class OnArrowAllPointImageClear
    {
        public int arrowID;
    }
}
