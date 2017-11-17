using System.Text;
using SuperSocket.ProtoBase;

namespace SSCA.Socket
{
    public class HxReceiveFilter : TerminatorReceiveFilter<StringPackageInfo>
    {
        public HxReceiveFilter() : base(Encoding.UTF8.GetBytes("#"))
        {
        }

        public override StringPackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            var a = bufferStream.ReadString((int) bufferStream.Length, Encoding.UTF8);
            return new StringPackageInfo(a, new BasicStringParser());
        }
    }
}