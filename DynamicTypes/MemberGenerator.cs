﻿using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace DynamicTypes
{
    /// <summary>
    /// BaseClasss for Member Generators, Can be used to define Member Groups or Single Members
    /// </summary>
    public abstract class MemberGenerator
    {
        public MemberGenerator()
        {

        }

        #region Properties

        /// <summary>
        /// A type is always helpful for all Actions u Plan
        /// </summary>
        public Type? Type { get; set; }

        /// <summary>
        /// Name of the Member
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Attributes that will be attached to the Member
        /// </summary>
        public List<AttributeGenerator> Attributes { get; set; } = new List<AttributeGenerator>();

        /// <summary>
        /// Some members have to be defined Early. Please use this to Check for Redundancies 
        /// </summary>
        public bool Defined { get; set; }

        /// <summary>
        /// The Member that contains the Property used in case of Interfaces / Abstract etc.
        /// </summary>
        public Type? OverrideDefinition
        {
            get { return OverrideDefinitions.FirstOrDefault(); }
            set
            {
                if (OverrideDefinitions.Count == 0)
                    OverrideDefinitions.Add(value);
                else
                    OverrideDefinitions[0] = value;
            }
        }
        public List<Type?> OverrideDefinitions { get; set; } = new List<Type?>();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="MemberGenerator"/>
        /// </summary>
        /// <param name="type">A type is always helpful for all Actions u Plan, can be null</param>
        protected MemberGenerator(Type? type)
        {
            Type = type;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Defines a Member (Creates Field, Properties and Methods), please note, Definition of a member can be done anytime if necessary.
        /// </summary>
        /// <param name="tb"></param>
        public abstract void DefineMember(TypeBuilder tb, TypeGenerator tg);

        /// <summary>
        /// Will be when the Final Type is compiled
        /// </summary>
        /// <param name="cg"></param>
        public virtual void Compiled(TypeGenerator cg) { }

        #endregion

    }
}
