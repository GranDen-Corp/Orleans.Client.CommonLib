using System.Threading.Tasks;

namespace HelloNetCore3.ShareInterface
{
    public interface IHello : Orleans.IGrainWithIntegerKey
    {
        Task<string> SayHello(string greeting);
    }
}
