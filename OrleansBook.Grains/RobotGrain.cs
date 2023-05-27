using Microsoft.Extensions.Logging;
using OrleansBook.GrainInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrleansBook.Grains
{
    public class RobotGrain : Grain, IRobotGrain
    {
        private readonly ILogger<RobotGrain> _logger;

        public RobotGrain(ILogger<RobotGrain> logger)
        {
            _logger = logger;
        }

        private Queue<string> instructions = new Queue<string>();
        public Task AddInstruction(string instruction)
        {
            var key = this.GetPrimaryKeyString();
            _logger.LogWarning("{Key} adding '{Instruction}'", key, instruction);

            this.instructions.Enqueue(instruction);
            return Task.CompletedTask;
        }
        public Task<int> GetInstructionCount()
        {
            return Task.FromResult(this.instructions.Count);
        }
        public Task<string> GetNextInstruction()
        {
            if (this.instructions.Count == 0)
            {
                return Task.FromResult<string>(null);
            }
            var instruction = this.instructions.Dequeue();

            var key = this.GetPrimaryKeyString();
            _logger.LogWarning("{Key} executing '{Instruction}'", key, instruction);

            return Task.FromResult(instruction);
        }
    }
}
