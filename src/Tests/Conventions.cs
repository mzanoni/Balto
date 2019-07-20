namespace Tests
{
    public interface IPing
    {
        string PingIt();
    }

    class Ping : IPing
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
        string ZongIt();
    }

    public class Fong : IZong
    {
        public string ZongIt()
        {
            return "test";
        }
    }
}
