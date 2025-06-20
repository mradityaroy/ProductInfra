using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Constructs;

namespace ProductInfra
{
    public class NetworkStack : Stack
    {
        public Vpc Vpc { get; private set; }

        public NetworkStack(Construct scope, string id, StackProps props = null) : base(scope, id, props)
        {
            Vpc = new Vpc(this, "AppVpc", new VpcProps
            {
                MaxAzs = 2, // two availability zones
                NatGateways = 1
            });
        }
    }
}
