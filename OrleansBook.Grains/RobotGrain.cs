using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using OrleansBook.GrainInterfaces;
using OrleansBook.Grains.Dtos;
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
        private readonly IPersistentState<RobotState> _state;

        public RobotGrain(
            ILogger<RobotGrain> logger,
            [PersistentState("robotState", "robots")] IPersistentState<RobotState> state)
        {
            _logger = logger;
            _state = state;
        }

        public async Task AddInstruction(string instruction)
        {
            var key = this.GetPrimaryKeyString();
            _logger.LogWarning("{Key} adding '{Instruction}'", key, instruction);

            _state.State.Instructions.Enqueue(instruction);
            await _state.WriteStateAsync();
        }
        public Task<int> GetInstructionCount()
        {
            return Task.FromResult(_state.State.Instructions.Count);
        }
        public async Task<string> GetNextInstruction()
        {
            if (_state.State.Instructions.Count == 0)
            {
                return null;
            }
            var instruction = _state.State.Instructions.Dequeue();

            var key = this.GetPrimaryKeyString();
            _logger.LogWarning("{Key} executing '{Instruction}'", key, instruction);

            await _state.WriteStateAsync();
            return instruction;
        }
    }
}
