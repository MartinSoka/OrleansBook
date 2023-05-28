namespace OrleansBook.Tests
{
    [Collection(ClusterCollection.Name)]
    public class RobotGrainTests
    {
        private readonly TestCluster _cluster;

        public RobotGrainTests(ClusterFixture fixture)
        {
            _cluster = fixture.Cluster;
        }

        [Fact]
        public async Task TestInstructions()
        {
            var robot = _cluster.GrainFactory.GetGrain<IRobotGrain>("test");
            await robot.AddInstruction("Do the dishes");
            await robot.AddInstruction("Take out the trash");
            await robot.AddInstruction("Do the laundry");
            Assert.Equal(3, await robot.GetInstructionCount());
            Assert.Equal(await robot.GetNextInstruction(), "Do the dishes");
            Assert.Equal(await robot.GetNextInstruction(), "Take out the trash");
            Assert.Equal(await robot.GetNextInstruction(), "Do the laundry");
            Assert.Null(await robot.GetNextInstruction());
            Assert.Equal(0, await robot.GetInstructionCount());
        }
    }

    public class ClusterFixture : IDisposable
    {
        public ClusterFixture()
        {
            var builder = new TestClusterBuilder();
            Cluster = builder
                .AddSiloBuilderConfigurator<SiloBuilderConfigurator>()
                .Build();
            Cluster.Deploy();
        }

        public void Dispose() => Cluster.StopAllSilos();

        public TestCluster Cluster { get; }
    }

    [CollectionDefinition(ClusterCollection.Name)]
    public class ClusterCollection : ICollectionFixture<ClusterFixture>
    {
        public const string Name = "ClusterCollection";
    }

    class SiloBuilderConfigurator : ISiloConfigurator
    {
        public void Configure(ISiloBuilder hostBuilder)
        {
            hostBuilder.AddMemoryGrainStorage("robots");
            var mockState = new Mock<IPersistentState<RobotState>>();
            mockState.SetupGet(s => s.State).Returns(new RobotState());
            hostBuilder.ConfigureServices(services =>
            {
                services.AddSingleton<IPersistentState<RobotState>>(mockState.Object);
                services.AddSingleton<ILogger<RobotGrain>>(new Mock<ILogger<RobotGrain>>().Object);
            });
        }
    }
}