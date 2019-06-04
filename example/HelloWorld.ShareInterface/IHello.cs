using System.Threading.Tasks;

namespace HelloWorld.ShareInterface
{
    public interface IHello : Orleans.IGrainWithIntegerKey
    {
        Task<string> SayHello(string greeting);
    }
}