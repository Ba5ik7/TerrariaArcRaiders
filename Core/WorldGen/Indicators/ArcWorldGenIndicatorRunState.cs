using System;
using System.Collections.Generic;
using System.Linq;

namespace TerrariaArcRaiders.Core.WorldGen.Indicators
{
    public sealed class ArcWorldGenIndicatorRunState
    {
        private readonly HashSet<ArcWorldGenStage> _completedStages = new();

        public void Reset()
        {
            _completedStages.Clear();
        }

        public void MarkCompleted(ArcWorldGenStage stage)
        {
            _completedStages.Add(stage);
        }

        public bool IsCompleted(ArcWorldGenStage stage)
        {
            return _completedStages.Contains(stage);
        }

        public IReadOnlyList<ArcWorldGenStage> GetCompletedStagesInOrder()
        {
            return _completedStages.OrderBy(s => (int)s).ToList();
        }

        public static ArcWorldGenIndicatorRunState ForCompletedStages(IEnumerable<ArcWorldGenStage> completedStages)
        {
            var state = new ArcWorldGenIndicatorRunState();
            if (completedStages == null)
            {
                return state;
            }

            foreach (var stage in completedStages)
            {
                state.MarkCompleted(stage);
            }

            return state;
        }
    }
}
