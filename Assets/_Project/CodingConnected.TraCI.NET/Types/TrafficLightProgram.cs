using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingConnected.TraCI.NET.Types
{

    /// <summary>
    /// see http://sumo.dlr.de/wiki/Simulation/Traffic_Lights#Defining_New_TLS-Programs
    /// </summary>
    public class TrafficLightProgram : ComposedTypeBase
    {
        /// <summary>
        /// Name of the program
        /// </summary>
        public string ProgramId;

        /// <summary>
        /// Number of phase to start with
        /// </summary>
        public int PhaseIndex;

        /// <summary>
        /// List of phases
        /// </summary>
        public List<TrafficLightProgramPhase> Phases;

        public TrafficLightProgram()
        {
            Phases = new List<TrafficLightProgramPhase>();
        }

    }

    public class TrafficCompleteLightProgram : ComposedTypeBase
    {

        public int NumberOfLogics;

        public List<TrafficLightLogics> TrafficLightLogics;

        public TrafficCompleteLightProgram()
        {
            TrafficLightLogics = new List<TrafficLightLogics>();
        }

    }

    public class TrafficLightLogics
    {
        public string SubId;
        public int Type;
        public CompoundObject SubParameter; 
        public int CurrentPhaseIndex;
        public int NumberOfPhases;
        public List<TrafficLightProgramPhase> TrafficLightPhases;

        public TrafficLightLogics()
        {
            TrafficLightPhases = new List<TrafficLightProgramPhase>();
        }

    }

    public class TrafficLightProgramPhase
    {
        /// <summary>
        /// Duration in ms
        /// </summary>
        public double Duration;

        public double MinDuration;

        public double MaxDuration;

        public string Definition;
    }
}
