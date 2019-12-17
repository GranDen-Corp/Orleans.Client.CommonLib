using System.Threading.Tasks;

namespace HelloNetStandard2_1.ShareInterface
{
    public interface IHello : Orleans.IGrainWithIntegerKey
    {
        Task<string> SayHello(string greeting);
    }
}