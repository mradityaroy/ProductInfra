using Amazon.CDK;
using Amazon.CDK.AWS.EC2;

namespace ProductInfra
{
    public class EcsStackProps : StackProps
    {
        public IVpc Vpc { get; set; }
    }
}
