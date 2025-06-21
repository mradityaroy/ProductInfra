using Amazon.CDK;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECS.Patterns;
using Amazon.CDK.AWS.ECR;
using Amazon.CDK.AWS.IAM;
using Constructs;

namespace ProductInfra
{
    public class EcsStack : Stack
    {
        public EcsStack(Construct scope, string id, EcsStackProps props) : base(scope, id, props)
        {
            var vpc = props.Vpc;

            var cluster = new Cluster(this, "AppCluster", new ClusterProps
            {
                Vpc = vpc
            });

            // Reference the existing ECR repository using the correct ARN
            var repository = Repository.FromRepositoryArn(this, "EcrRepo",
                "arn:aws:ecr:us-east-1:876143322976:repository/product-app");

            var fargateService = new ApplicationLoadBalancedFargateService(this, "FargateService", new ApplicationLoadBalancedFargateServiceProps
            {
                Cluster = cluster,
                DesiredCount = 1,
                ListenerPort = 80,
                TaskImageOptions = new ApplicationLoadBalancedTaskImageOptions
                {
                    Image = ContainerImage.FromEcrRepository(repository, "latest"),  // Use specific tag
                    ContainerPort = 80
                },
                PublicLoadBalancer = true
            });

            // Grant ECR pull permissions to the ECS Task Execution Role
            var executionRole = fargateService.TaskDefinition.ExecutionRole as Role;

            if (executionRole != null)
            {
                executionRole.AddToPolicy(new PolicyStatement(new PolicyStatementProps
                {
                    Effect = Effect.ALLOW,
                    Actions = new[]
                    {
                        "ecr:GetAuthorizationToken",
                        "ecr:BatchCheckLayerAvailability",
                        "ecr:GetDownloadUrlForLayer"
                    },
                    Resources = new[] { "*" }
                }));
            }

            // Output the Load Balancer DNS
            new CfnOutput(this, "LoadBalancerDNS", new CfnOutputProps
            {
                Value = fargateService.LoadBalancer.LoadBalancerDnsName
            });
        }
    }
}
