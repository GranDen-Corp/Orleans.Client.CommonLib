using Orleans.CodeGeneration;
using System.Threading.Tasks;

namespace HelloNetStandard2.ShareInterface
{
    public interface IHello : Orleans.IGrainWithIntegerKey
    {
        Task<string> SayHello(string greeting);
    }
}