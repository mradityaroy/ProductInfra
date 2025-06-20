using Amazon.CDK;

namespace ProductInfra
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();

            var env = new Amazon.CDK.Environment
            {
                Account = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT"),
                Region = "us-east-1"
            };

            var networkStack = new NetworkStack(app, "NetworkStacks", new StackProps
            {
                Env = env
            });

            new EcsStack(app, "EcsStack", new EcsStackProps
            {
                Vpc = networkStack.Vpc,
                Env = env
            });

            app.Synth();
        }
    }
}
