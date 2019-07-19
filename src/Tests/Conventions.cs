namespace Tests
{
    public interface IPing
    {
        string PingIt();
    }

    internal class Ping : IPing
    {
        public string PingIt()
        {
            return "test";
        }
    }

    public interface IPong
    {
        string Pong();
    }

    public interface IZong
    {
        string Zong();
    }

    public class Zong1 : IZong
    {
        public string Zong()
        {
            return "test";
        }
    }

    public class Zong2 : IZong
    {
        public string Zong()
        {
            return "test";
        }
    }
}
