﻿using System;
using SpiceSharp.Circuits;
using SpiceSharp.Attributes;
using SpiceSharp.Sparse;
using SpiceSharp.Components.CCCS;
using SpiceSharp.Simulations;

namespace SpiceSharp.Behaviors.CCCS
{
    /// <summary>
    /// Behavior for a <see cref="Components.CurrentControlledCurrentsource"/>
    /// </summary>
    public class LoadBehavior : Behaviors.LoadBehavior, IConnectedBehavior
    {
        /// <summary>
        /// Necessary parameters and behaviors
        /// </summary>
        BaseParameters bp;
        VSRC.LoadBehavior vsrcload;

        /// <summary>
        /// Nodes
        /// </summary>
        public int CCCScontBranch { get; protected set; }
        int CCCSposNode, CCCSnegNode;
        protected MatrixElement CCCSposContBrptr { get; private set; }
        protected MatrixElement CCCSnegContBrptr { get; private set; }

        /// <summary>
        /// Properties
        /// </summary>
        /// <param name="state">State</param>
        /// <returns></returns>
        [NameAttribute("i"), NameAttribute("c"), InfoAttribute("Current")]
        public double GetCurrent(State state) => state.Solution[CCCScontBranch] * bp.CCCScoeff;
        [NameAttribute("v"), InfoAttribute("Voltage")]
        public double GetVoltage(State state) => state.Solution[CCCSposNode] - state.Solution[CCCSnegNode];
        [NameAttribute("p"), InfoAttribute("Power")]
        public double GetPower(State state) => (state.Solution[CCCSposNode] - state.Solution[CCCSnegNode]) * state.Solution[CCCScontBranch] * bp.CCCScoeff;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name</param>
        public LoadBehavior(Identifier name) : base(name) { }

        /// <summary>
        /// Create an export method
        /// </summary>
        /// <param name="property">Property name</param>
        /// <returns></returns>
        public override Func<State, double> CreateExport(string property)
        {
            // We avoid using reflection for common components
            switch (property)
            {
                case "c":
                case "i": return GetCurrent;
                case "v": return GetVoltage;
                case "p": return GetPower;
                default: return null;
            }
        }

        /// <summary>
        /// Setup behavior
        /// </summary>
        /// <param name="provider">Data provider</param>
        public override void Setup(SetupDataProvider provider)
        {
            // Get parameters
            bp = provider.GetParameters<BaseParameters>();

            // Get behaviors (0 = CCCS behaviors, 1 = VSRC behaviors)
            vsrcload = provider.GetBehavior<VSRC.LoadBehavior>(1);
        }

        /// <summary>
        /// Connect the behavior
        /// </summary>
        /// <param name="pins">Pins</param>
        public void Connect(params int[] pins)
        {
            CCCSposNode = pins[0];
            CCCSnegNode = pins[1];
        }

        /// <summary>
        /// Get matrix pointers
        /// </summary>
        /// <param name="nodes">Nodes</param>
        /// <param name="matrix">Matrix</param>
        public override void GetMatrixPointers(Nodes nodes, Matrix matrix)
        {
            CCCScontBranch = vsrcload.VSRCbranch;
            CCCSposContBrptr = matrix.GetElement(CCCSposNode, CCCScontBranch);
            CCCSnegContBrptr = matrix.GetElement(CCCSnegNode, CCCScontBranch);
        }
        
        /// <summary>
        /// Execute behavior
        /// </summary>
        /// <param name="sim">Base simulation</param>
        public override void Load(BaseSimulation sim)
        {
            CCCSposContBrptr.Add(bp.CCCScoeff.Value);
            CCCSnegContBrptr.Sub(bp.CCCScoeff.Value);
        }
    }
}
