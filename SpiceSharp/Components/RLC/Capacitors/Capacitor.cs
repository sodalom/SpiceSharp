﻿using SpiceSharp.Attributes;
using SpiceSharp.Components.Capacitors;
using SpiceSharp.ParameterSets;
using SpiceSharp.Validation;
using System;
using System.Linq;

namespace SpiceSharp.Components
{
    /// <summary>
    /// A capacitor
    /// </summary>
    /// <seealso cref="Component"/>
    /// <seealso cref="IParameterized{P}"/>
    /// <seealso cref="Capacitors.Parameters"/>
    /// <seealso cref="IRuleSubject"/>
    [Pin(0, "C+"), Pin(1, "C-"), Connected]
    public class Capacitor : Component<ComponentBindingContext>,
        IParameterized<Parameters>,
        IRuleSubject
    {
        /// <inheritdoc/>
        public Parameters Parameters { get; } = new Parameters();

        /// <summary>
        /// Gets the pin count.
        /// </summary>
        [ParameterName("pincount"), ParameterInfo("Number of pins")]
        public const int PinCount = 2;

        /// <summary>
        /// Initializes a new instance of the <see cref="Capacitor"/> class.
        /// </summary>
        /// <param name="name">The name of the capacitor.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is <c>null</c>.</exception>
        public Capacitor(string name)
            : base(name, PinCount)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Capacitor"/> class.
        /// </summary>
        /// <param name="name">The name of the capacitor.</param>
        /// <param name="pos">The positive node.</param>
        /// <param name="neg">The negative node.</param>
        /// <param name="cap">The capacitance value.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is <c>null</c>.</exception>
        public Capacitor(string name, string pos, string neg, double cap)
            : this(name)
        {
            Parameters.Capacitance = cap;
            Connect(pos, neg);
        }

        /// <inheritdoc/>
        void IRuleSubject.Apply(IRules rules)
        {
            var p = rules.GetParameterSet<ComponentRuleParameters>();
            var nodes = Nodes.Select(name => p.Factory.GetSharedVariable(name)).ToArray();
            foreach (var rule in rules.GetRules<IConductiveRule>())
                rule.AddPath(this, ConductionTypes.Ac, nodes[0], nodes[1]);
        }
    }
}
