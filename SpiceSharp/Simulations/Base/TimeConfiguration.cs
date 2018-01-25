﻿using SpiceSharp.Attributes;
using SpiceSharp.IntegrationMethods;

namespace SpiceSharp.Simulations
{
    /// <summary>
    /// Configuration for a <see cref="TimeConfiguration"/>
    /// </summary>
    public class TimeConfiguration : ParameterSet
    {
        /// <summary>
        /// Gets or sets the integration method
        /// </summary>
        public IntegrationMethod Method { get; set; } = new Trapezoidal();

        /// <summary>
        /// Gets or sets the initial timepoint that should be exported
        /// </summary>
        [NameAttribute("init"), NameAttribute("start"), InfoAttribute("The starting timepoint")]
        public double InitTime { get; set; } = 0.0;

        /// <summary>
        /// Gets or sets the final simulation timepoint
        /// </summary>
        [NameAttribute("final"), NameAttribute("stop"), InfoAttribute("The final timepoint")]
        public double FinalTime { get; set; } = double.NaN;

        /// <summary>
        /// Gets or sets the step
        /// </summary>
        [NameAttribute("step"), InfoAttribute("The timestep")]
        public double Step { get; set; } = double.NaN;

        /// <summary>
        /// Gets or sets the maximum timestep
        /// </summary>
        [NameAttribute("maxstep"), InfoAttribute("The maximum allowed timestep")]
        public double MaxStep
        {
            get
            {
                if (double.IsNaN(maxstep))
                    return (FinalTime - InitTime) / 50.0;
                return maxstep;
            }
            set { maxstep = value; }
        }
        double maxstep = double.NaN;

        /// <summary>
        /// Get the minimum timestep allowed
        /// </summary>
        [NameAttribute("deltamin"), InfoAttribute("The minimum delta for breakpoints")]
        public double DeltaMin { get { return 1e-13 * MaxStep; } }

        /// <summary>
        /// Maximum number of iterations for each time point
        /// </summary>
        public int TranMaxIterations = 10;

        /// <summary>
        /// Use initial conditions
        /// </summary>
        public bool UseIC = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public TimeConfiguration()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="step"></param>
        /// <param name="stop"></param>
        public TimeConfiguration(double step, double stop)
        {
            Step = step;
            FinalTime = stop;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="step">Step</param>
        /// <param name="stop">Stop</param>
        /// <param name="max">Maximum timestep</param>
        public TimeConfiguration(double step, double stop, double max)
        {
            Step = step;
            FinalTime = stop;
            MaxStep = max;
        }
    }
}
