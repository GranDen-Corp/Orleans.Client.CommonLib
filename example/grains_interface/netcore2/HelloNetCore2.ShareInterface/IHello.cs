using System.Threading.Tasks;

namespace HelloNetCore2.ShareInterface
{
    public interface IHello : Orleans.IGrainWithIntegerKey
    {
        Task<string> SayHello(string greeting);
    }
}
